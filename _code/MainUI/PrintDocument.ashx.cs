using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Shared;
using MainUI.Shared;
using Winnovative.WnvHtmlConvert;
using AICPA.Destroyer.User.Event;

namespace MainUI.Handlers
{
    public class PrintDocument : IHttpHandler, IRequiresSessionState
    {
        protected int totalDocCount = 0;
        protected ContentWrapper wrapper = null;
        protected ContextManager contextManager = null;

        public string printDescendants = null;
        public string ShowCodificationSources = null;
        public string IsPDF = null;
        public string doPrint = null;

        public bool IsCodificationDocument = false;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            printDescendants = context.Request[ContextManager.REQPARAM_PRINTDESCENDANTS];
            ShowCodificationSources = context.Request[ContextManager.REQPARAM_SHOWCODIFICATIONSOURCES];
            IsPDF = context.Request[ContextManager.REQPARAM_PRINTTOPDF];

            doPrint = context.Request[ContextManager.REQPARAM_DOPRINT];

            string targetDoc = context.Request[ContextManager.REQPARAM_TARGETDOC];
            string targetPtr = context.Request[ContextManager.REQPARAM_TARGETPTR];

            string id = context.Request[ContextManager.REQPARAM_ID];
            string type = context.Request[ContextManager.REQPARAM_TYPE];
            string joinSectionsUrl = HttpUtility.UrlDecode(context.Request["joinSectionsUrl"]);

            contextManager = new ContextManager(context);

            string docString = string.Empty;

            // If we have a joinSectionsUrl, then we are printing a join sections page
            if (!string.IsNullOrEmpty(joinSectionsUrl))
            {
                GetDocuments getDocs = new GetDocuments();
                docString = getDocs.getDocStringsFromUrlParams(joinSectionsUrl, context);
            }
            // If we have valid id/type values on the query string
            else if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(type))
            {
                int nodeId = Convert.ToInt32(id);
                //NodeType nodeType = NodeType.Document;

                var container = PrimaryContentContainer.ConstructContainer(contextManager.CurrentSite, nodeId, type);
                wrapper = container.PrimaryContent;
            }
            else // Otherwise, use targetdoc/targetptr instead of id/type
            {
                var contentLink = new ContentLink(contextManager.CurrentSite, targetDoc, targetPtr);
                wrapper = new ContentWrapper(contentLink.Document);
            }

            if (docString == string.Empty)
                IsCodificationDocument = wrapper.Document.Book.Name.StartsWith("faf-");
            else
                IsCodificationDocument = joinSectionsUrl.Contains("show_sources");

            if (docString == string.Empty && !string.IsNullOrEmpty(printDescendants))
            {
                //If the document name is the same as the book name, this is the title page of the book, so start at the book level
                if (wrapper.Document.Name == wrapper.Document.Book.Name)
                {
                    //get the xml that represents this book node and its immediate children
                    string tocXml = contextManager.CurrentSite.GetTocXml(wrapper.Document.Book.Id, NodeType.Document);
                    docString = GetSubDocumentHTML(tocXml);
                }
                else
                {
                    //get the xml that represents this top node and its immediate children
                    string tocXml = contextManager.CurrentSite.GetTocXml(wrapper.Document.Id, NodeType.Document);
                    docString = GetSubDocumentHTML(tocXml);
                }
            }
            else
            {
                if (docString != string.Empty || (wrapper.Document.PrimaryFormat != null && wrapper.Document.PrimaryFormat.Description == "text/html"))
                {
                    if (docString == string.Empty)
                    {
                        wrapper.Document.PrimaryFormat.LogContentAccess(contextManager.CurrentUser);
                        docString = DestroyerBpc.ByteArrayToStr(wrapper.Document.PrimaryFormat.Content);
                    }

                    if (IsCodificationDocument)
                    {
                        docString = DocumentDisplayHelpers.IncludeCodificationStyleSheets(docString, !string.IsNullOrEmpty(ShowCodificationSources));

                        // remove noPrint spans
                        docString = DocumentDisplayHelpers.RemoveNoPrintSpans(docString);
                    }

                    // If it's an old FASB or archived book, then add the "this document is not current" notice
                    if (wrapper != null && DocumentDisplayHelpers.IsDocumentNotCurrent(wrapper.Document.Book.Name))
                    {
                        if (!string.IsNullOrEmpty(doPrint))
                        {
                            docString = DocumentDisplayHelpers.IncludeDocumentNotCurrentNotice(docString, false);
                        } else {
                            docString = IncludeDocumentNotCurrentNoticePrintPreview(docString);
                        }
                    }
                }
            }


            if (docString != string.Empty)
            {
                string baseURL;

                if (contextManager.ShouldReplacePrintBaseUrl)
                {
                    //string queryString = context.Request.Url.Query;
                    //baseURL = contextManager.PrintBaseUrl + queryString;
                    baseURL = contextManager.PrintBaseUrl;
                }
                else
                {
                    //baseURL = context.Request.Url.ToString();
                    baseURL = "http://" + context.Request.Url.Host + ":" + context.Request.Url.Port;
                }

                

                docString = AddPrintCSSTag(docString);

                string copyright;

                if (wrapper == null)
                {
                    // this means we have a FASB ASC Joined section

                    // TODO: We should probably get this from the DB or something (at least a constant) rather than hard coding the string.
                    copyright =
                        DestroyerBpc.ReplaceCopyrightCurrentYear(
                            "Copyright &copy; 2009-[[current-year]], Financial Accounting Standards Board, Norwalk, Connecticut. All Rights Reserved.");
                }
                else
                {
                    copyright = wrapper.Document.Book.Copyright;
                }

                if (!string.IsNullOrEmpty(IsPDF) && !string.IsNullOrEmpty(doPrint))
                {
                    copyright = copyright.Replace("&copy;", "\u00a9");

                    // Adjust css to remove background and replace table icons with actual tables
                    docString = docString.Replace("main.css", "main-pdf.css");

                    // Remove javascript icon links
                    docString = Regex.Replace(docString, "<a( title=\\\"[^\\\"]+\\\")? class=\\\"[^\\\"]+\\\" onclick=\\\"doLink\\([^\\)]+\\);\\\" href=\\\"#\\\"( title=\\\"[^\\\"]+\\\")?>", "", RegexOptions.IgnoreCase);

                    // Remove javascript links
                    docString = Regex.Replace(docString, "<a class=\\\"[^\\\"]+\\\" onclick=\\\"doLink\\([^\\)]+\\);\\\" href=\\\"#\\\">", "", RegexOptions.IgnoreCase);

                    // Remove footnote links
                    docString = Regex.Replace(docString, "<sup><a href=\\\"[^\\\"]+\\\" name=\\\"[^\\\"]+\\\">", "<sup>", RegexOptions.IgnoreCase);

                    // Show hidden large images
                    docString = Regex.Replace(docString, " style=\\\"display:none;\\\" onclick=\\\"javascript:showSmall\\('[^']+'\\)\\\"", "", RegexOptions.IgnoreCase);

                    // Show larger content (instead of placeholder icons)    
                    docString = Regex.Replace(docString, "-large\\\" style=\\\"display: none\\\"", "style=\"display: block;\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    
                    // Remove divs that are in place of larger content
                    docString = Regex.Replace(docString, "onclick=\\\"javascript:showLarge\\('[^']+'\\)", "style=\"display: none;\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    
                    // Remove table collapse minus icons
                    docString = Regex.Replace(docString, "src=\\\"/resources/table-minus.gif\\\"", "src=\"/resources/table-minus.gif\" style=\"display: none;\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    
                    // Remove table thumbnail icons
                    docString = Regex.Replace(docString, "tableThumbnail.gif\\\"></div>\\r\\n<div style=\\\"display:none\\\"", "tableThumbnail.gif\"></div><div style=\"display: block\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    docString = Regex.Replace(docString, "tableThumbnail.gif\\\"></div>\\r\\n<div class=\\\"expandedTable\\\" style=\\\"display:none\\\"", "tableThumbnail.gif\"></div><div class=\"expandedTable\" style=\"display: block\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Find GetResource href's with ResourceBookName and ResourceName
                    string resourceCall = string.Format("Handlers/GetResource.ashx\\?{0}=([^&=]+)&{1}=([^\"&=]+)",
                                                        ContextManager.REQPARAM_RESOURCEBOOKNAME,
                                                        ContextManager.REQPARAM_RESOURCENAME);

                    docString = Regex.Replace(docString, resourceCall,
                        m => GetResource.GetResourcePath(m.Groups[1].Value, m.Groups[2].Value, contextManager),
                        RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // User Preferences css
                    docString = docString.Replace("Handlers/GetResource.ashx?type=user_preferences", "Styles/UserPreferencePrintDefault.css");
                    
                    // Find static style href's
                    docString = Regex.Replace(docString, "(Styles/[^\\.\"]+\\.css)",
                        m => context.Server.MapPath(m.Groups[1].Value),
                        RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // FASB Logo Banner image
                    string fasbPath = "images/portal/fasb_logo.gif";
                    docString = docString.Replace(fasbPath, context.Server.MapPath(fasbPath));

                    //Send the document to the PDF converter. 
                    string title = "Joined Sections";
                    if (wrapper != null)
                    {
                        // TODO: do we really want the book title and not the document title
                        title = wrapper.Document.Book.Title;
                    }

                    ConvertHTMLStringToPDF(docString, baseURL, title, copyright, context);
                 }
                else
                {
                    string fullHtml = docString;
                    if (!string.IsNullOrEmpty(doPrint)) {
                        fullHtml = "<HTML><HEAD><TITLE>" + copyright + "</TITLE>";
                        fullHtml += "<style>";
                        fullHtml += " .joinSectionsTopic,.joinSectionsSubtopic { display:none;}";
                        fullHtml += "</style>";

                        fullHtml += "<script type=\"text/javascript\" src=\"/js/jquery/jquery-1.11.2.min.js\"></script>";
                        fullHtml += "<SCRIPT language='JavaScript'>";
                        fullHtml += "$(document).ready(function () {  $(\"body\").css(\"background\", \"white\"); \r\n$(\"div[id$='-large']\").css(\"display\", \"block\");\r\n $(\"div[id$='-small']\").css(\"display\", \"none\"); \r\n$(\"img[src='resources/table-minus.gif']\").css(\"display\", \"none\");\r\n";                        
                        fullHtml += "$(\".joinSectionsTopic\").hide(); \r\n $(\".joinSectionsTopic\").remove(); \r\n";
                        fullHtml += " $(\".joinSectionsSubtopic\").hide(); \r\n $(\".joinSectionsSubtopic\").remove();\r\n";
                        fullHtml += "});";
                        fullHtml += "function readyToPrint(){window.print();}";                        
                        fullHtml += "</SCRIPT>";
                        fullHtml += "</HEAD><BODY onload='readyToPrint();'></BODY></HTML>";
                        fullHtml += docString;
                    } else {
                        fullHtml += "<p class=\"copyright float\" style=\"padding-left:20px;\">" + copyright + "</p>";
                    }

                    context.Response.Write(fullHtml);
                    context.Response.End();
                }
            }
        }

       

        private string GetSubDocumentHTML(string tocXml)
        {
            //our html string to return
            string retHtml = String.Empty;

            //load the xml string into an xml document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(tocXml);

            //get all the child document nodes that are represented in the xml
            XmlNodeList docNodes = xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENT);
            int docCount = 0;
            foreach (XmlNode docNode in docNodes)
            {
                //get the document name and "has children" status
                string docName = DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_DOCUMENTNAME]);
                int docId = int.Parse(DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_DOCUMENTID]));
                bool hasChildren = bool.Parse(DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_HASCHILDREN]));

                //if we are the first document in the xml, only print this out if we are also the top document 
                if (docCount > 0 | totalDocCount == 0)
                {
                    //instantiate the document
                    IDocument subDoc = wrapper.Document.Book.Documents[docName];
                    if (subDoc.PrimaryFormat.Description == "text/html")
                    {
                        subDoc.PrimaryFormat.LogContentAccess(contextManager.CurrentUser);
                        string content = DestroyerBpc.ByteArrayToStr(subDoc.PrimaryFormat.Content);

                        // If it's an old FASB or archived book, then add the "this document is not current" notice (but only to first document)
                        if (totalDocCount == 0 && DocumentDisplayHelpers.IsDocumentNotCurrent(wrapper.Document.Book.Name))
                        {
                            if (!string.IsNullOrEmpty(doPrint))
                            {
                                content = DocumentDisplayHelpers.IncludeDocumentNotCurrentNotice(content, false);
                            } else {
                                content = IncludeDocumentNotCurrentNoticePrintPreview(content);
                            }
                        }

                        // strip out codification headers from all codification content, except the first one
                        if (IsCodificationDocument && totalDocCount != 0)
                        {
                            content = DocumentDisplayHelpers.RemoveCodificationHeaders(content);
                        }

                        if (IsCodificationDocument)
                        {
                            // remove all codification footers
                            // sburton: we may want to alter this to leave the footers of the last doc...
                            content = DocumentDisplayHelpers.RemoveCodificationFooters(content);

                            // add necessary codification stylesheets
                            content = DocumentDisplayHelpers.IncludeCodificationStyleSheets(content, !string.IsNullOrEmpty(ShowCodificationSources));

                            // remove noPrint spans
                            content = DocumentDisplayHelpers.RemoveNoPrintSpans(content);
                        }

                        retHtml += content;
                    }
                    totalDocCount++;
                }

                //recurse if there are children of this document node and if we are not the very first document node
                if (hasChildren && docCount > 0)
                {
                    string subdocTocXml = contextManager.CurrentSite.GetTocXml(docId, NodeType.Document);
                    retHtml += GetSubDocumentHTML(subdocTocXml);
                }
                docCount++;
            }

            //return the html string
            return retHtml;
        }

        /// <summary>
        /// Convert the HTML code from the specified URL to a PDF document and send the 
        /// document as an attachment to the browser
        /// </summary>

        private void ConvertHTMLStringToPDF(string htmlString, string baseURL, string bookname, string copyright, HttpContext context)
        {
            int numberOfCharsToLog = 400;
            string logString = htmlString.Substring(0, Math.Min(htmlString.Length, numberOfCharsToLog));
            IEvent logEvent = new Event(EventType.Info, DateTime.Now, DestroyerBpc.INFO_PRINT_TO_PDF,
                                          DestroyerBpc.MODULE_WEBSERVICES, "ConvertHTMLStringToPDF",
                                          "First 400 chars of html string", logString, contextManager.CurrentUser);
            logEvent.Save(true);

            logEvent = new Event(EventType.Info, DateTime.Now, DestroyerBpc.INFO_PRINT_TO_PDF,
                                          DestroyerBpc.MODULE_WEBSERVICES, "ConvertHTMLStringToPDF",
                                          "BaseURL", baseURL, contextManager.CurrentUser);
            logEvent.Save(true);

			try
			{
				            // Create the PDF converter. Optionally you can specify the virtual browser 
				// width as parameter. 1024 pixels is default, 0 means autodetect
				PdfConverter pdfConverter = new PdfConverter();
				
				// set the license key
				pdfConverter.LicenseKey = "ACsxIDEgMzYyIDYuMCAzMS4xMi45OTk5";

				// set the converter options
				pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
				pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
				pdfConverter.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Portrait;
				pdfConverter.NavigationTimeout = 120;

				// set if header and footer are shown in the PDF - optional - default is false 
				// pdfConverter.PdfDocumentOptions.ShowHeader = true;
				// pdfConverter.PdfDocumentOptions.ShowFooter = true;
				
				// set to generate a pdf with selectable text or a pdf with embedded image - optional - default is true
				// pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
				
				// set if the HTML content is resized if necessary to fit the PDF page width - optional - default is true
				pdfConverter.PdfDocumentOptions.FitWidth = true;

				// set the embedded fonts option - optional - default is false
				// pdfConverter.PdfDocumentOptions.EmbedFonts = true;

				// set the live HTTP links option - optional - default is true
				pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;

				// set if the JavaScript is enabled during conversion to a PDF with selectable text 
				// - optional - default is false
				pdfConverter.ScriptsEnabled = true;

				// set if the images in PDF are compressed with JPEG to reduce the PDF document size - optional - default is true
				// pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;

				// enable auto-generated bookmarks for a specified list of tags (e.g. H1 and H2)
				pdfConverter.PdfBookmarkOptions.TagNames = new string[] { "H1", "H2" };
				
				// set PDF document description - optional
				// pdfConverter.PdfDocumentInfo.AuthorName = "AICPA Online Documentation";

				// add HTML header
				AddHeader(pdfConverter, copyright);
				
				// add HTML footer
				// AddFooter(pdfConverter, copyright);

				// Performs the conversion and get the pdf document bytes that you can further 
				// save to a file or send as a browser response
				//
				// The baseURL parameter helps the converter to get the CSS files and images
				// referenced by a relative URL in the HTML string. This option has effect only if the HTML string
				// contains a valid HEAD tag. The converter will automatically inserts a <BASE HREF="baseURL"> tag. 
				byte[] pdfBytes = null;

				pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(htmlString, baseURL);

				// send the PDF document as a response to the browser for download
				context.Response.Clear();
				context.Response.AddHeader("Content-Type", "application/pdf");
				context.Response.AddHeader("Content-Disposition", "attachment; filename=" + bookname.Replace(' ', '_') + ".pdf; size=" + pdfBytes.Length.ToString());
				context.Response.Flush();
				context.Response.BinaryWrite(pdfBytes);

                logEvent = new Event(EventType.Info, DateTime.Now, DestroyerBpc.INFO_PRINT_TO_PDF,
                              DestroyerBpc.MODULE_WEBSERVICES, "ConvertHTMLStringToPDF",
                              "Done generating pdf", "Done generating pdf", contextManager.CurrentUser);
                logEvent.Save(true);

                context.Response.Flush();
				context.Response.End();
				
			}
			catch (Exception ex)
			{
				logEvent = new Event(EventType.Error, DateTime.Now, DestroyerBpc.INFO_PRINT_TO_PDF,
							  DestroyerBpc.MODULE_WEBSERVICES, "ConvertHTMLStringToPDF",
							  "Error in ConvertHtmlStringToPdf", ex.Message, contextManager.CurrentUser);
                logEvent.Save(true);
				
			}

        }

        private void AddHeader(PdfConverter pdfConverter, string copyright)
        {
            string thisPageURL = HttpContext.Current.Request.Url.AbsoluteUri;
            string headerAndFooterHtmlUrl = thisPageURL.Substring(0, thisPageURL.LastIndexOf('/')) + "/Uploads/HeaderAndFooterHtml.htm";

            //enable header
            pdfConverter.PdfDocumentOptions.ShowHeader = true;
            // set the header height in points
            pdfConverter.PdfHeaderOptions.HeaderHeight = 60;
            //write the CopyrightFooter
            pdfConverter.PdfHeaderOptions.TextArea = new TextArea(-20, 50, copyright,
                new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));
            pdfConverter.PdfHeaderOptions.TextArea.EmbedTextFont = true;
            pdfConverter.PdfHeaderOptions.TextArea.TextAlign = HorizontalTextAlign.Center;
            // set the header HTML area
            // pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, -1, pdfConverter.PdfHeaderOptions.HeaderHeight,
            //             headerAndFooterHtmlUrl, 1024, -1);
            // pdfConverter.PdfHeaderOptions.HtmlToPdfArea.FitHeight = true;
            // pdfConverter.PdfHeaderOptions.HtmlToPdfArea.EmbedFonts = true;
        }

        // Currently unused
        private void AddFooter(PdfConverter pdfConverter, string copyright)
        {
            string thisPageURL = HttpContext.Current.Request.Url.AbsoluteUri;
            string headerAndFooterHtmlUrl = thisPageURL.Substring(0, thisPageURL.LastIndexOf('/')) + "/Uploads/HeaderAndFooterHtml.htm";
            //string headerAndFooterHtmlUrl
            //enable footer
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            // set the footer height in points
            pdfConverter.PdfFooterOptions.FooterHeight = 60;
            //write the page number
            pdfConverter.PdfFooterOptions.TextArea = new TextArea(5, 50, copyright,
                new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));
            //write the CopyrightFooter
            //pdfConverter.PdfFooterOptions.TextArea = new TextArea(0, 20, "This is page &p; of &P;  ",
            //    new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));
            pdfConverter.PdfFooterOptions.TextArea.EmbedTextFont = true;
            pdfConverter.PdfFooterOptions.TextArea.TextAlign = HorizontalTextAlign.Center;
            // set the footer HTML area
            // pdfConverter.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, -1, pdfConverter.PdfFooterOptions.FooterHeight,
            //             headerAndFooterHtmlUrl, 1024, -1);
            // pdfConverter.PdfFooterOptions.HtmlToPdfArea.FitHeight = true;
            // pdfConverter.PdfFooterOptions.HtmlToPdfArea.EmbedFonts = true;
        }

        private string AddPrintCSSTag(string wholeFile)
        {
            string style = "<link rel=\"stylesheet\" href=\"/Styles/print.css\" type=\"text/css\" />";
            wholeFile = Regex.Replace(wholeFile, "</head>", style + "</head>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            return wholeFile;
        }

        public static string IncludeDocumentNotCurrentNoticePrintPreview(string wholeFile)
        {
            string scrollBarStyleAddition = string.Empty;
            
            string styles = @"<style type=""text/css"">
    #docNotCurrentHeader
    {
    position: absolute;
    top: 0px;
    left:0px;
    right:0px;
    margin: 0;
    padding-top:10px;
    display: block;
    width: 100%;
    height: 23px;
    background: #e5ebf5;
    z-index: 4;
    border-top: 1px solid #999;
    border-bottom: 1px solid #999;
    }
	#originalDoc
	{
		display: block;" + scrollBarStyleAddition + @"
		overflow: auto;
		position: relative;
		z-index: 3;
	}
	.headerPad
	{
		display: block;
		width: 18px;
		height: 28px;
		float: left;
	}
	.contentPad
	{
		display: block;
		height: 28px;
	}
</style>";

            const string bodyReplaceRegex = @"<body$1>
	<div id=""docNotCurrentHeader""
	style=""font-family: Verdana, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: darkblue; top: 36px;"">
		<div class=""headerPad""></div>
		This document is not current. Please see the
		<a href=""#"" onclick=""doLink('faf-noticeToConstituents', '115496', false);""
		style=""text-decoration: underline;"" onmouseover=""this.style.color = 'red';"" onmouseout=""this.style.color = '#258';"">FASB Accounting Standards Codification</a>
		for current accounting guidance.
	</div>
	<div id=""originalDoc"">
		<div class=""contentPad""></div>
		$2
	</div>
</body>";

            wholeFile = Regex.Replace(wholeFile, "<head>", "<head>" + styles, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Regex.Replace(wholeFile, "<body([^>])*?>(.*?)</body>", bodyReplaceRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
    }
}