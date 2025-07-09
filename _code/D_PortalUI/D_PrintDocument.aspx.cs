using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Text.RegularExpressions;

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Subscription;
using Winnovative.WnvHtmlConvert;

namespace AICPA.Destroyer.UI.Portal
{
    /// <summary>
    /// Summary description for D_PrintDocument.
    /// </summary>
    public partial class D_PrintDocument : DestroyerUi
    {
        protected AICPA.Destroyer.Content.Site.ISite site = null;
        protected IBook book = null;
        protected IDocument doc = null;
        protected int totalDocCount = 0;

        protected bool IsCodificationDocument = false;
        protected bool ShowCodificationSources = false;
        // Add for Print to pdf, djf 2/23/10
        protected bool IsPDF = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //see whether the user desires to print the current displayed page or the current displayed page with all descendants
            bool printDescendants = bool.Parse(Request[REQPARAM_PRINTDESCENDANTS]);

            //get codification specific parameters
            // TODO: move constants into common file, if they're going to stay
            const string REQPARAM_ISCODIFICATION = "isCodification";
            const string REQPARAM_SHOWCODIFICATIONSOURCES = "showCodificationSources";
            const string REQPARAM_PRINTTOPDF = "printToPDF";
            if (Request[REQPARAM_ISCODIFICATION] != null)
            {
                IsCodificationDocument = bool.Parse(Request[REQPARAM_ISCODIFICATION]);
            }

            if (Request[REQPARAM_SHOWCODIFICATIONSOURCES] != null)
            {
                ShowCodificationSources = bool.Parse(Request[REQPARAM_SHOWCODIFICATIONSOURCES]);
            }
            // Added for Print to pdf, djf 2/23/10
            if (Request[REQPARAM_PRINTTOPDF] != null)
            {
                IsPDF = bool.Parse(Request[REQPARAM_PRINTTOPDF]);
            }
            //get the current site and the current document
            site = GetCurrentSite(this.Page);
            doc = GetCurrentDocument(this.Page);
            book = doc.Book;

            //populate the html string differently depending on what the user wants to print out
            string docString = string.Empty;
            if (printDescendants)
            {
                //get the xml that represents this top node and its immediate children
                string tocXml = site.GetTocXml(doc.Id, NodeType.Document);
                docString = GetSubDocumentHTML(tocXml);
            }
            else
            {
                if (doc.PrimaryFormat != null && doc.PrimaryFormat.Description == "text/html")
                {
                    docString = DestroyerBpc.ByteArrayToStr(doc.PrimaryFormat.Content);

                    if (IsCodificationDocument)
                    {
                        docString = IncludeCodificationStyleSheets(docString, ShowCodificationSources);

                        // remove noPrint spans
                        docString = RemoveNoPrintSpans(docString);
                    }

                    // If it's an old FASB or archived book, then add the "this document is not current" notice
                    if (IsDocumentNotCurrent(book.Name))
                    {
                        docString = IncludeDocumentNotCurrentNotice(docString, false);
                    }
                }
            }

            if (docString != string.Empty)
            {
                if (IsPDF)
                {
                    string fullHtml = "<HTML><HEAD><TITLE>" + doc.Book.Copyright + "</TITLE>";
                    fullHtml += "<SCRIPT language='JavaScript'>";
                    fullHtml += "function readyToPrint()";
                    fullHtml += "</SCRIPT>";
                    fullHtml += "</HEAD><BODY onload='readyToPrint();'></BODY></HTML>";
                    fullHtml += docString;
                    string baseURL = Page.Request.Url.ToString();
                    string copyright = doc.Book.Copyright.Replace("&copy", "\u00a9"); 

                    //Send the document to the PDF converter.
                    ConvertHTMLStringToPDF(fullHtml, baseURL, doc.Book.Title, copyright);

                }
                else
                {
                    string fullHtml = "<HTML><HEAD><TITLE>" + doc.Book.Copyright + "</TITLE>";
                    fullHtml += "<SCRIPT language='JavaScript'>";
                    fullHtml += "function readyToPrint(){window.print(); }";
                    fullHtml += "</SCRIPT>";
                    fullHtml += "</HEAD><BODY onload='readyToPrint();'></BODY></HTML>";
                    fullHtml += docString;

                    // output the actual document contents to the response output stream
                    Response.Write(fullHtml);

                    // end the response
                    Response.End();
                }

            }


            // jjs 9/15/06: this is the old morales code that loads up each page in a frame.
            // this approach was found by Lois not to work on large print jobs (frames end up
            // coming out in different orders in the job)

            //			//see whether the user desires to print the current displayed page or the current displayed page with all descendants
            //			bool printDescendants = bool.Parse(Request[REQPARAM_PRINTDESCENDANTS]);
            //
            //			//get the current site and the current document
            //			site = GetCurrentSite(this.Page);
            //			doc= GetCurrentDocument(this.Page);
            //			book = doc.Book;
            //
            //			//prepare a string to hold our document content
            //            string outerHtmlContent = EMPTY_STRING;
            //			string innerHtmlContent = EMPTY_STRING;
            //			string rowAttVal = EMPTY_STRING;
            //			
            //			//setting up the Copyright frame
            //			string copyRightFrame = string.Format("<FRAME NAME='printHeader' src='D_PrintCopyRight.aspx?cr={0}'>", doc.Book.Copyright);
            //			
            //			//populate the html string differently depending on what the user wants to print out
            //			if(printDescendants)
            //			{
            //				//get the xml that represents this top node and its immediate children
            //				string tocXml = site.GetTocXml(doc.Id, NodeType.Document);
            //
            //                //get the frames that we can use to create a printable frameset comprised of all documents
            //				innerHtmlContent += GetPrintFrames(tocXml);
            //
            //				//construct the rows attribute for our frameset
            //				for(int i = 0; i < totalDocCount; i++)
            //				{
            //                    rowAttVal += "*";
            //					if(i != totalDocCount-1)
            //					{
            //						rowAttVal += ",";
            //					}
            //				}
            //			}
            //			else
            //			{
            //				string frameString = string.Format("<FRAME src='D_ViewDocument.aspx?{0}={1}&{2}={3}'>", REQPARAM_CURRENTBOOKNAME, doc.Book.Name, REQPARAM_CURRENTDOCUMENTNAME, doc.Name);
            //                innerHtmlContent += frameString;
            //				rowAttVal = "*";
            //			}
            //
            //			//put the outer html around our inner html
            //			rowAttVal = "98%,"+rowAttVal;
            //			outerHtmlContent += "<HTML><HEAD><TITLE>" + FormatDocRef(doc.SiteReferencePath)+ "</TITLE>";
            //			outerHtmlContent += "<script language='JavaScript'>";
            //			outerHtmlContent += " function readyToPrint(){";
            //			outerHtmlContent += "	var objFrames = document.frames;";
            //			outerHtmlContent += "	var objFF = document.getElementById('theFrameSet').childNodes;";
            //			outerHtmlContent += "	var copyRight = '"+doc.Book.Copyright+"';";
            //			outerHtmlContent += "	if(objFrames){";
            //			outerHtmlContent += "		for(j=0;j<objFrames.length;j++){";
            //			outerHtmlContent += "			objFrames[j].document.title = copyRight;";
            //			outerHtmlContent += "		}";
            //			outerHtmlContent += "		window.print();";
            //			outerHtmlContent += "		window.close();";
            //			outerHtmlContent += "		return;";
            //			outerHtmlContent += "	}else{";
            //			outerHtmlContent += "		for(j=0;j<objFF.length;j++){";
            //			outerHtmlContent += "			copyRight = copyRight.replace('&copy;','©');";
            //			outerHtmlContent += "			objFF[j].contentDocument.title = copyRight;";
            //			outerHtmlContent += "		}";
            //			outerHtmlContent += "		window.print();";
            //			outerHtmlContent += "		return;";
            //			outerHtmlContent += "	}";
            //			outerHtmlContent += "	return;";
            //			outerHtmlContent += " }";
            //			outerHtmlContent += "</script>";
            //			outerHtmlContent += "</HEAD>";
            //			outerHtmlContent += "<FRAMESET id='theFrameSet' rows='" + rowAttVal + "' onload='readyToPrint();' >";
            //			//outerHtmlContent += copyRightFrame;
            //			outerHtmlContent += innerHtmlContent;
            //			outerHtmlContent += "</FRAMESET></div></HTML>";
            //
            //			//write out the results
            //			Response.Write(outerHtmlContent);
        }

        private string GetSubDocumentHTML(string tocXml)
        {
            //our html string to return
            string retHtml = EMPTY_STRING;

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
                    IDocument subDoc = book.Documents[docName];
                    if (subDoc.PrimaryFormat.Description == "text/html")
                    {
                        string content = DestroyerBpc.ByteArrayToStr(subDoc.PrimaryFormat.Content);

                        // If it's an old FASB or archived book, then add the "this document is not current" notice (but only to first document)
                        if (totalDocCount == 0 && IsDocumentNotCurrent(book.Name))
                        {
                            content = IncludeDocumentNotCurrentNotice(content, false);
                        }

                        // strip out codification headers from all codification content, except the first one
                        if (IsCodificationDocument && totalDocCount != 0)
                        {
                            content = RemoveCodificationHeaders(content);
                        }

                        if (IsCodificationDocument)
                        {
                            // remove all codification footers
                            // sburton: we may want to alter this to leave the footers of the last doc...
                            content = RemoveCodificationFooters(content);

                            // add necessary codification stylesheets
                            content = IncludeCodificationStyleSheets(content, ShowCodificationSources);

                            // remove noPrint spans
                            content = RemoveNoPrintSpans(content);
                        }

                        retHtml += content;
                    }
                    totalDocCount++;
                }

                //recurse if there are children of this document node and if we are not the very first document node
                if (hasChildren && docCount > 0)
                {
                    string subdocTocXml = site.GetTocXml(docId, NodeType.Document);
                    retHtml += GetSubDocumentHTML(subdocTocXml);
                }
                docCount++;
            }

            //return the html string
            return retHtml;
        }

        /// <summary>
        /// Recursive helper method for assembling the print content
        /// </summary>
        /// <param name="tocXml"></param>
        /// <returns></returns>
        private string GetPrintFrames(string tocXml)
        {
            //our html string to return
            string retHtml = EMPTY_STRING;

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
                    IDocument subDoc = book.Documents[docName];

                    //write out the frame html
                    string frameString = string.Format("<FRAME id='frames_{4}' src='D_ViewDocument.aspx?{0}={1}&{2}={3}'>", REQPARAM_CURRENTBOOKNAME, subDoc.Book.Name, REQPARAM_CURRENTDOCUMENTNAME, subDoc.Name, totalDocCount + 1);
                    retHtml += frameString;
                    totalDocCount++;
                }

                //recurse if there are children of this document node and if we are not the very first document node
                if (hasChildren && docCount > 0)
                {
                    string subdocTocXml = site.GetTocXml(docId, NodeType.Document);
                    retHtml += GetPrintFrames(subdocTocXml);
                }

                docCount++;
            }

            //return the html string
            return retHtml;
        }

        /// <summary>
        /// Private method to get the site reference for the print document's title
        /// </summary>
        /// <param name="siteReferenceXml"></param>
        private string FormatDocRef(string siteReferenceXml)
        {
            //string that we will return
            string retDocRef = EMPTY_STRING;

            //get the xml string into an xml document
            XmlDocument siteReferenceXmlDoc = new XmlDocument();
            siteReferenceXmlDoc.LoadXml(siteReferenceXml);

            //grab all of the second-level nodes and go through them
            XmlNodeList nodes = siteReferenceXmlDoc.SelectNodes("/*/*");
            for (int i = 0; i < nodes.Count; i++)
            {
                //our current node
                XmlNode node = nodes[i];

                //get the title attribute and add it to the return string
                string nodeTitle = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_TITLE]);
                retDocRef += nodeTitle;

                //add the sep char unless we are at the last node
                if (i != nodes.Count - 1)
                {
                    retDocRef += " > ";
                }
            }

            //return the string
            return retDocRef;
        }


        /// <summary>
        /// Convert the HTML code from the specified URL to a PDF document and send the 
        /// document as an attachment to the browser
        /// </summary>

        private void ConvertHTMLStringToPDF(string htmlString, string baseURL, string bookname, string copyright)
        {
            //string htmlString = textBoxHTMLCode.Text;
            //string baseURL =  textBoxBaseURL.Text.Trim();

            // Create the PDF converter. Optionally you can specify the virtual browser 
            // width as parameter. 1024 pixels is default, 0 means autodetect
            PdfConverter pdfConverter = new PdfConverter();
            // set the license key
            pdfConverter.LicenseKey = "Q2hzY3Jjc2N3bXNjcHJtcnFtenp6eg==";
            // set the converter options
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Portrait;

            // set if header and footer are shown in the PDF - optional - default is false 
            // pdfConverter.PdfDocumentOptions.ShowHeader = true;
            // pdfConverter.PdfDocumentOptions.ShowFooter = true;
            // set to generate a pdf with selectable text or a pdf with embedded image - optional - default is true
            // pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
            // set if the HTML content is resized if necessary to fit the PDF page width - optional - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;
            // 
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
            //set PDF document description - optional
            // pdfConverter.PdfDocumentInfo.AuthorName = "AICPA Online Documentation";

            // add HTML header
            AddHeader(pdfConverter, copyright);
            // add HTML footer
            //AddFooter(pdfConverter, copyright);

            // Performs the conversion and get the pdf document bytes that you can further 
            // save to a file or send as a browser response
            //
            // The baseURL parameter helps the converter to get the CSS files and images
            // referenced by a relative URL in the HTML string. This option has efect only if the HTML string
            // contains a valid HEAD tag. The converter will automatically inserts a <BASE HREF="baseURL"> tag. 
            byte[] pdfBytes = null;
            //htmlString ="'<html><head><title>Simple HTML String</title></head><body><table><!-- first page --><!-- Insert a CSS page break with page-break-after:always--><tr><td style='page-break-after:always;font-family: Verdana; font-size: medium; color: Blue'><a href='http://www.winnovative-software.com'> Hello World!!!</a></td> </tr><!-- Second page --><tr><td style='font-family: Verdana; font-size: medium; color: Blue'>Hello World from Second Page !!!</td></tr></table></body></html>";

            pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(htmlString, baseURL);

            // send the PDF document as a response to the browser for download
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Type", "binary/octet-stream");
            response.AddHeader("Content-Disposition",
                "attachment; filename=" + bookname + ".pdf; size=" + pdfBytes.Length.ToString());
            response.Flush();
            response.BinaryWrite(pdfBytes);
            response.Flush();
            response.End();
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



        #region Codification Specific Methods
        // sburton: These methods handle the specific cases of the codification, such as
        //   join sections and print friendly

        public static string RemoveCodificationHeaders(string wholeFile)
        {
            // the banner is inside the header, so by stripping it out first, we don't have a problem of nested divs

            wholeFile = Regex.Replace(wholeFile, "<div id=\"banner\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            wholeFile = Regex.Replace(wholeFile, "<div class=\"runhead\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return wholeFile;
        }

        public static string RemoveCodificationFooters(string wholeFile)
        {
            return Regex.Replace(wholeFile, "<div class=\"runfoot\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static string IncludeCodificationStyleSheets(string wholeFile, bool ShowSources)
        {
            if (ShowSources)
            {
                string pattern = "hideCodificationSources.css";
                string replacement = "showCodificationSources.css";

                wholeFile = Regex.Replace(wholeFile, pattern, replacement, RegexOptions.IgnoreCase);
            }
            else
            {
                // currently we don't have any "print" stylesheets that need to be added
            }

            return wholeFile;
        }

        public static string RemoveNoPrintSpans(string wholeFile)
        {
            return Regex.Replace(wholeFile, "<span class=\"noPrint\">.+?</span>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
        #endregion

        #region DocumentNotCurrentNotice Helper Methods
        public static bool IsDocumentNotCurrent(string bookName)
        {
            return bookName.IndexOf(DestroyerUi.BOOK_PREFIX_FASB) > -1 ||
                bookName.IndexOf(DestroyerUi.BOOK_PREFIX_ARCH) > -1;
        }

        /// <summary>
        /// Adds the div above the content to say that it is out of date.
        /// </summary>
        /// <param name="wholeFile">the text of the whole file</param>
        /// <param name="prepareForScrollBars">True will limit the content to "100%" which then provides scrollbars.
        /// This is best for on-screen viewing.  False lets the document run over, which is better for printouts.</param>
        /// <returns></returns>
        public static string IncludeDocumentNotCurrentNotice(string wholeFile, bool prepareForScrollBars)
        {
            string scrollBarStyleAddition = string.Empty;

            if (prepareForScrollBars)
            {
                scrollBarStyleAddition = " height: 100%; max-height: 100%; ";
            }

            string styles = @"<style type=""text/css"">
	html
	{
		height: 100%;
		max-height: 100%;
		overflow: hidden;
		padding: 0;
		margin: 0;
		border: 0;
	}
	body
	{
		height: 100%;
		max-height: 100%;
		overflow: hidden;
		padding: 0;
		margin: 0;
		border: 0;
	}
	#docNotCurrentHeader
	{
		position: absolute;
		top: 0;
		right:18px;
		margin: 0;
		display: block;
		width: 100%;
		height: 28px;
		background: #fff;
		z-index: 4;
		border-bottom: 1px solid #000;
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

            string bodyReplaceRegex = @"<body$1>
	<div id=""docNotCurrentHeader""
	style=""font-family: Verdana, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: darkblue;"">
		<div class=""headerPad""></div>
		This document is not current. Please see the
		<a href=""D_Link.aspx?targetdoc=faf-noticeToConstituents&targetptr=115496""
		target=""_top"" style=""text-decoration: underline;"" onmouseover=""this.style.color = 'red';"" onmouseout=""this.style.color = '#258';"">FASB Accounting Standards Codification</a>
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
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
