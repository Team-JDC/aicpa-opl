using System;
using System.Web;
using System.Web.SessionState;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using MainUI.Shared;
using AICPA.Destroyer.User;
using System.Text.RegularExpressions;
using MainUI.WS;

namespace MainUI.Handlers
{
    /// <summary>
    /// Summary description for GetDocument
    /// </summary>
    public class GetDocuments : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string[] targetDocs = context.Request.QueryString.GetValues(ContextManager.REQPARAM_TARGETDOC);
            string[] targetPtrs = context.Request.QueryString.GetValues(ContextManager.REQPARAM_TARGETPTR);

            string[] ids = context.Request.QueryString.GetValues(ContextManager.REQPARAM_ID);
            string[] types = context.Request.QueryString.GetValues(ContextManager.REQPARAM_TYPE);
            string[] tableStyle = context.Request.QueryString.GetValues(ContextManager.REQPARAM_TABLESTYLE);
            
            bool hilite = context.Request.Params[ContextManager.REQPARAM_HITHIGHLIGHTS] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_HITHIGHLIGHTS]);

            bool showSources = context.Request.Params[ContextManager.REQPARAM_SHOWSOURCES] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_SHOWSOURCES]);

            bool joinSections = context.Request.Params[ContextManager.REQPARAM_JOINSECTIONS] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_JOINSECTIONS]);

            var contextManager = new ContextManager(context);
            ContentWrapper wrapper;
            string crumb = string.Empty;
            Content ct = new Content();

            if (targetDocs != null && targetPtrs != null)
            {

                if (targetDocs.Length > 0 && targetPtrs.Length > 0)
                {
                    crumb = ct.GetFullBreadcrumbStrByTargetDocTargetPtr(targetDocs[0], targetPtrs[0]);
                }
                if (targetDocs.Length > 1 && targetPtrs.Length > 1)
                {
                    string docString = getDocStringsForDocumentsByTargets(contextManager, targetDocs, targetPtrs, hilite, showSources);

                    // Write to response stream and end the response
                    contextManager.Context.Response.Write(docString);
                    contextManager.Context.Response.End();
                }
                else if (targetDocs.Length == 1 && targetPtrs.Length == 1)
                {
                    var contentLink = new ContentLink(contextManager.CurrentSite, targetDocs[0], targetPtrs[0]);
                    wrapper = new ContentWrapper(contentLink.Document);

                    if (wrapper.HasDocument)
                    {
                        //make sure the document is in our subscription
                        if (wrapper.Document.InSubscription && IsBookInSubscription(wrapper.Document.Book.Name, contextManager))
                        {
                            //see if we have a current search result object
                            StreamBackDocument(contextManager, wrapper.Document, hilite, showSources, tableStyle, crumb);
                        }
                        else
                        {
                            // context.Response.Redirect("D_ViewDocumentNotAuthorized.aspx", true);
                            context.Response.Write("Not Authorized; replace this with a redirect to whatever the replacement migration of D_ViewDocumentNotAuthorized.aspx should be.");
                        }
                    }
                    else if (wrapper.HasUri)
                    {
                        // this ends the Response stream (execution of this entire page/class will halt)
                        contextManager.Context.Server.Transfer("~/" + wrapper.Uri);
                    }
                    else
                    {
                        throw new Exception("Expected an IPrimaryContentContainer to have a ContentWrapper with either Uri or Document");
                    }
                }
            }
            else if (ids != null && types != null)
            {
                if (ids.Length > 0 && types.Length > 0)
                {
                    int temp = int.Parse(ids[0]);
                    crumb = ct.GetFullBreadcrumbStr(temp, types[0]);
                }
                else
                {
                    crumb = string.Empty;
                }

                if (ids.Length > 1 && types.Length > 1)
                {
                    string docString = getDocStringsForDocumentsByIds(contextManager, ids, types, hilite, showSources, joinSections);
                    docString = checkTableStyle(docString, tableStyle);

                    contextManager.Context.Response.AppendHeader("content-disposition", "filename=joindoc.html");
                    contextManager.Context.Response.AppendHeader("Cache-Control", "no-cache, must-revalidate");
                    // Write to response stream and end the response
                    contextManager.Context.Response.Write(docString);
                    contextManager.Context.Response.End();
                }
                else if (ids.Length == 1 && types.Length == 1)
                {
                    int nodeId = Convert.ToInt32(ids[0]);

                    IPrimaryContentContainer container = PrimaryContentContainer.ConstructContainer(contextManager.CurrentSite, nodeId, types[0]);
                    wrapper = container.PrimaryContent;

                    if (wrapper.HasDocument)
                    {
                        //make sure the document is in our subscription
                        if (wrapper.Document.InSubscription && IsBookInSubscription(wrapper.Document.Book.Name, contextManager))
                        {
                            //see if we have a current search result object
                            StreamBackDocument(contextManager, wrapper.Document, hilite, showSources, tableStyle, crumb);
                        }
                        else
                        {
                            // context.Response.Redirect("D_ViewDocumentNotAuthorized.aspx", true);
                            context.Response.Write("Not Authorized; replace this with a redirect to whatever the replacement migration of D_ViewDocumentNotAuthorized.aspx should be.");
                        }
                    }
                    else if (wrapper.HasUri)
                    {
                        // this ends the Response stream (execution of this entire page/class will halt)
                        contextManager.Context.Server.Transfer("~/" + wrapper.Uri);
                    }
                    else
                    {
                        throw new Exception("Expected an IPrimaryContentContainer to have a ContentWrapper with either Uri or Document");
                    }
                }
            }
        }

        private string checkTableStyle(string docString, string[] tableStyle)
        {
            if (tableStyle != null)
            {
                string style = tableStyle[0];
                MatchCollection tables = Regex.Matches(docString, "(<table[^>]+class=\"" + style + "\".+?</table>)|(<h[23][^>]*>.+?</h[23]+>)", RegexOptions.Singleline);
                string tableToInsert = "";
                foreach (Match table in tables)
                {
                    if (!table.ToString().Contains("ps_para_number"))
                    {
                        tableToInsert = tableToInsert + "<br>\n" + table.ToString();
                    }
                }
                //string heading = Regex.Match(docString, "<h2[^>]+>.+?</h2>", RegexOptions.Singleline).ToString();
                docString = Regex.Replace(docString, "(<body[^>]*>).+(</body>)", "$1" + tableToInsert + "$2", RegexOptions.Singleline);
                docString = Regex.Replace(docString, "Show Revision History for this subtopic", "");
            }
            return docString;
        }

        private void StreamBackDocument(ContextManager contextManager, IDocument doc, bool hilite, bool showSources, string[] tableStyle, string crumb = "")
        {
            IBook book = doc.Book;
            IDocumentFormat format = doc.PrimaryFormat;
            ISearchResults searchResults = contextManager.SearchResults;

            

            if (format != null)
            {
                // Serve up the file by name
                contextManager.Context.Response.AppendHeader("content-disposition", "filename=" + doc.Name);
                contextManager.Context.Response.AppendHeader("Cache-Control", "no-cache, must-revalidate");

                // set the content type for the Response to that of the 
                // document to display.  For example. "application/msword"
                contextManager.Context.Response.ContentType = format.Description;

                if (format.Description == "application/pdf")
                {
                    contextManager.Context.Response.WriteFile(format.Uri);
                    contextManager.Context.Response.End();

                    return;
                }

                //set our bytes to be highlighted or not
                byte[] contentBytes;

                if (hilite && searchResults != null && format.ContentTypeId == (int)ContentType.TextHtml &&
                    searchResults.WordInterpretations != null && searchResults.WordInterpretations.Length > 0)
                {
                    contentBytes = format.GetHighlightedContent(searchResults.WordInterpretations,
                                                                ContextManager.HILITE_BEGINTAG, ContextManager.HILITE_ENDTAG);
                }
                else
                {
                    format.LogContentAccess(contextManager.CurrentUser);
                    contentBytes = format.Content;
                }

                string contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

                contentText = Regex.Replace(contentText, "<body([^>]{0,})>", "<body$1>\n" +
               "<div class=\"detail\"><div id=\"leftcol\" class=\"col-sm-9\"><div class=\"leftcol_inner\">" + crumb +
                   "<div class=\"leftcol_content\"><i class=\"fa fa-bookmark docBookmark\" style=\"display:none\"></i>");

                // If we're suppossed to show sources, add that stylesheet in the content
                if (showSources)
                {
                    contentText = DocumentDisplayHelpers.IncludeCodificationStyleSheets(contentText, true);
                }

                // If it's an old FASB or archived book, then add the "this document is not current" notice
                if (DocumentDisplayHelpers.IsDocumentNotCurrent(book.Name))
                {
                    contentText = DocumentDisplayHelpers.IncludeDocumentNotCurrentNotice(contentText, true);
                }

                // add some references to javascript and css that we need in the iframe approach
                contentText = AddJavaScriptAndCssRefs(contentText, contextManager.CurrentUser.ReferringSiteValue);


                // add uinique Id to Notes Css
                contentText = AddUniqueIdToNotesCss(contentText);

                // add params to User Preferences Css
                contentText = AddParamsToUserPreferencesCss(contentText, contextManager);

                contentText = checkTableStyle(contentText, tableStyle);

                // Write to response stream and end the response
                contextManager.Context.Response.Write(contentText);
                contextManager.Context.Response.End();
            }
            else
            {
                throw new Exception(string.Format("DocumentInstance with Id {0} has no PrimaryFormat", doc.Id));
            }
        }

        private bool IsBookInSubscription(string bookName, ContextManager contextManager)
        {
            bool isInSubscription = false;

            // check if the document is in the book list
            string[] authenticatedBookNames = contextManager.CurrentUser.UserSecurity.BookName;
            foreach (string authenticatedBookName in authenticatedBookNames)
            {
                if (bookName == authenticatedBookName)
                {
                    isInSubscription = true;
                    break;
                }
            }

            return isInSubscription;
        }

        private string AddJavaScriptAndCssRefs(string content, ReferringSite referringSite)
        {
            // this could potentially be combined with the AddDivForCssAndImageCheck method above
            // for better performance

            string newCssAndJs = string.Empty;

            // subscription check (for the locked icons next to the links)
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='Handlers/GetResource.ashx?type=subscription_access' />\r\n";
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='Styles/Base.css' />\r\n";
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='Styles/notes.css' />\r\n";

            newCssAndJs += "<link rel='stylesheet' type='text/css' href='resources/jquery.treeview.css' />\r\n";
            newCssAndJs += "<script type='text/javascript' src='/js/jquery/jquery-3.7.1.min.js'></script>\r\n";
            newCssAndJs += "<script type='text/javascript' src='/js/includeInIframe.js'></script>\r\n";

            if ((referringSite == ReferringSite.Ethics || (referringSite == ReferringSite.EthicsUser)))
                newCssAndJs += "<script type='text/javascript' src='js/textOperations.js'></script>\r\n";

            // TODO: these are needed for the faf toc, we should probably just put them in the faf content,
            // so we don't have to include them for every content page.
            newCssAndJs += "<script src='resources/jquery.cookie.js' type='text/javascript'></script>\r\n";
            newCssAndJs += "<script src='resources/jquery.treeview.js' type='text/javascript'></script>\r\n";
            newCssAndJs += "<script src='resources/jquery.treeview.async.js' type='text/javascript'></script>\r\n";
            newCssAndJs += "<script src='js/processFeatures.js' type='text/javascript'></script>\r\n";

            string feat = (HttpContext.Current.Session["D_FEATURES"] == null ? string.Empty : HttpContext.Current.Session["D_FEATURES"].ToString());
            string returnUrl = (HttpContext.Current.Session["D_RETURNURL"] == null ? string.Empty : HttpContext.Current.Session["D_RETURNURL"].ToString());
            newCssAndJs += "<script type='text/javascript'> " +
                           " var d_features = [" + feat + "]; " +
                           " var d_returnurl = '" + returnUrl + "';" +
                           "</script>";
            string headOrig = "<head>";
            string headReplace = headOrig + newCssAndJs;
            content = Regex.Replace(content, "<head[^>]+>", "<head>", RegexOptions.IgnoreCase);
            string newContent = content.Replace(headOrig, headReplace);

            // These two lines add 'top.' to all the onclicks
            string oldLinkPattern = @"(\s+onclick=[""'])";
            string replacement = @"$1top.";
            newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            oldLinkPattern = @"top\.javascript:";
            replacement = @"javascript:";
            newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            oldLinkPattern = @"top\.return ";
            replacement = @"return top.";
            newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            oldLinkPattern = @"top\.showLarge";
            replacement = @"showLarge";
            newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            oldLinkPattern = @"top\.showSmall";
            replacement = @"showSmall";
            newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            // These two lines only add 'top.' to the doLink onclicks
            //string oldLinkPattern = @"(\s+onclick=[""'])(doLink)";
            //string replacement = @"$1top.$2";
            //newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            //oldLinkPattern = @"(\s+onclick=[""'])(addNote)";
            //replacement = @"$1top.$2";
            //newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            //oldLinkPattern = @"(\s+onclick=[""'])(editNote)";
            //replacement = @"$1top.$2";
            //newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);

            //TODO: this change should happen in the content instead:
            //$(document).ready(function () {
            //var pageName = '305-10';
            //loadFafToc(pageName);
            //});

            //newContent = newContent.Replace("loadFafToc", "top.loadFafToc");

            return newContent;
        }


        //private string AddJavaScriptAndCssRefs(string content, ReferringSite referringSite)
        //{
        //    string newCssAndJs = string.Empty;
            
        //    if ((referringSite == ReferringSite.Ethics || (referringSite == ReferringSite.EthicsUser)))
        //        newCssAndJs += "<script type='text/javascript' src='js/textOperations.js'></script>";

        //    string headOrig = "<head>";
        //    string headReplace = headOrig + newCssAndJs;
        //    content = Regex.Replace(content, "<head[^>]+>", "<head>", RegexOptions.IgnoreCase);
        //    string newContent = content.Replace(headOrig, headReplace);


        //    return newContent;
        //}

        private string AddUniqueIdToNotesCss(string content)
        {
            // we may want to push this down to the API

            // <link href="Handlers/GetResource.ashx?type=notes&targetdoc=ps" rel="stylesheet" type="text/css"/>
            string notesCssRef = "Handlers/GetResource.ashx?type=notes&";
            string uniqueNotesCssRef = string.Format("{0}uid={1}&", notesCssRef, Guid.NewGuid().ToString());

            string newContent = content.Replace(notesCssRef, uniqueNotesCssRef);

            return newContent;
        }

        private string AddParamsToUserPreferencesCss(string content, ContextManager contextManager)
        {
            string userPrefsCssRef = "Handlers/GetResource.ashx?type=user_preferences";
            string newUserPrefsCssRef = userPrefsCssRef;

            foreach (string preferenceKey in contextManager.CurrentUser.Preferences.Keys)
            {
                newUserPrefsCssRef = newUserPrefsCssRef + "&" + preferenceKey + "=" + contextManager.CurrentUser.Preferences[preferenceKey];
            }

            string newContent = content.Replace(userPrefsCssRef, newUserPrefsCssRef);

            return newContent;
        }

        /// <summary>
        /// Will get the document strings for a given Document IDs/Types.  If the doMerge parameter is set to true the 
        /// documents will be merged into one big document.  The header content will be merged and the body content.
        /// </summary>
        /// <param name="contextManager"></param>
        /// <param name="ids"></param>
        /// <param name="types"></param>
        /// <param name="hilite"></param>
        /// <param name="showSources"></param>
        /// <param name="doMerge">True if you want to merge the document or false to just concatenate the documents into one</param>
        /// <returns></returns>
        private string getDocStringsForDocumentsByIds(ContextManager contextManager, string[] ids, string[] types, bool hilite, bool showSources, bool doMerge = false)
        {
            ContentWrapper wrapper;
            string docString = string.Empty;

            for (int i = 0; i < ids.Length; i++)
            {
                int nodeId = Convert.ToInt32(ids[i]);

                if ((types[i] == NodeType.DocumentAnchor.ToString()) || 
                    (types[i] == "6"))
                    continue;

                NodeType ntype;
                IPrimaryContentContainer container = null;
                if (Enum.TryParse<NodeType>(types[i], out ntype))
                {
                    container = PrimaryContentContainer.ConstructContainer(contextManager.CurrentSite, nodeId, ntype.ToString());
                }
                else
                {
                    container = PrimaryContentContainer.ConstructContainer(contextManager.CurrentSite, nodeId, types[i]);
                }
                //if (Enum.IsDefined(typeof(NodeType), colorValue) | colorValue.ToString().Contains(","))  

                wrapper = container.PrimaryContent;

                // NOTE: do we really need this if statement?  
                // No, if we will always get a contentLink and a wrapper.
                if (wrapper.HasDocument)
                {
                    IDocument doc = wrapper.Document;
                    IBook book = doc.Book;
                    IDocumentFormat format = doc.PrimaryFormat;
                    ISearchResults searchResults = contextManager.SearchResults;

                    if (format != null && format.Description == "text/html")
                    {
                        string contentText = "";

                        //Check to see if the book is in the current user's subscription
                        if (!IsBookInSubscription(doc.Book.Name, contextManager))
                        {
                            contentText = "<div><p>The document " + doc.Name + " was not joined because it is not in your subscription.</p></div>";
                        }
                        else
                        {
                            //*********************************
                            // Hit Highlighting
                            //*********************************
                            //set our bytes to be highlighted or not
                            byte[] contentBytes;

                            if (hilite && searchResults != null && format.ContentTypeId == (int)ContentType.TextHtml &&
                                searchResults.WordInterpretations != null && searchResults.WordInterpretations.Length > 0)
                            {
                                contentBytes = format.GetHighlightedContent(searchResults.WordInterpretations,
                                                                            ContextManager.HILITE_BEGINTAG, ContextManager.HILITE_ENDTAG);
                            }
                            else
                            {
                                format.LogContentAccess(contextManager.CurrentUser);
                                contentBytes = format.Content;
                            }

                            contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

                            if (i == 0)
                            {
                                contentText = Regex.Replace(contentText, "<body([^>]{0,})>", "<body$1>\n" +
                                    "<div class=\"detail\"><div id=\"leftcol\" class=\"col-sm-9\"><div class=\"leftcol_inner\">" +
                                    "<div class=\"leftcol_content\"><i class=\"fa fa-bookmark docBookmark\" style=\"display:none\"></i>");
                            }

                            //*********************************
                            // Formatting the document at i
                            //*********************************

                            // strip out codification headers from all codification content, except the first one
                            if (i != 0)
                            {
                                contentText = DocumentDisplayHelpers.RemoveCodificationHeaders(contentText);
                            }

                            // If it's an old FASB or archived book, then add the "this document is not current" notice
                            if (DocumentDisplayHelpers.IsDocumentNotCurrent(book.Name))
                            {
                                contentText = DocumentDisplayHelpers.IncludeDocumentNotCurrentNotice(contentText, true);
                            }

                            // If we're suppossed to show sources, add that stylesheet in the content
                            contentText = DocumentDisplayHelpers.IncludeCodificationStyleSheets(contentText, showSources);

                            // remove noPrint spans
                            contentText = DocumentDisplayHelpers.RemoveNoPrintSpans(contentText);

                            // remove all codification footers, except the last one
                            if (i != ids.Length - 1)
                            {
                                contentText = DocumentDisplayHelpers.RemoveCodificationFooters(contentText);
                            }
                        }

                        if (doMerge)
                            docString = joinDocuments(docString, contentText);                        
                        else docString += contentText;
                    }
                    else
                    {
                        throw new Exception(string.Format("DocumentInstance with Id {0} has no PrimaryFormat or the PrimaryFormat is not text/html", doc.Id));
                    }
                }
                else
                {
                    throw new Exception("Cannot join multiple documents where content for a doesn't exist.");
                }
            }

            if (doMerge)
            {
                // add some references to javascript and css that we need in the iframe approach
                docString = AddJavaScriptAndCssRefs(docString, contextManager.CurrentUser.ReferringSiteValue);
            }


            // add uinique Id to Notes Css
            docString = AddUniqueIdToNotesCss(docString);

            // add params to User Preferences Css
            docString = AddParamsToUserPreferencesCss(docString, contextManager);

            
            if (doMerge)
            {
                //Add Copy/Paste Buffer area.
                docString = Regex.Replace(docString, "</body>", "<div id=\"ideal\"></div></body>");
            }



            return docString;
        }
        
        /// <summary>
        /// Will take the document in contentText and merge the HEAD content into the HEAD section of the docString.  Same thing for
        /// the BODY
        /// </summary>
        /// <param name="docString">original document</param>
        /// <param name="contentText">new document</param>
        /// <returns></returns>
        private string joinDocuments(string docString, string contentText)
        {
            var result = docString;
            // First document should be returned
            if (string.IsNullOrEmpty(docString))
                return contentText;
            string headerContent = string.Empty;
            MatchCollection mc = Regex.Matches(contentText,"<head[^>]*?>(.*?)</head>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (mc.Count > 0) 
                headerContent = mc[0].Groups[1].Value;
            string bodyContent = string.Empty;

            mc = Regex.Matches(contentText,"<body[^>]*?>(.*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (mc.Count > 0)
                bodyContent = mc[0].Groups[1].Value;

            string head = "</head>";
            string body = "</body>";
            // Copy the head content into the Head section at the bottom.
            result = result.Replace(head, headerContent + head);
            // copy the body content into the Body section at the bottom. 
            result = result.Replace(body, bodyContent + body);
            return result;
        }

        
        private string getDocStringsForDocumentsByTargets(ContextManager contextManager, string[] targetDocs, string[] targetPtrs, bool hilite, bool showSources)
        {
            ContentWrapper wrapper;
            string docString = string.Empty;

            for (int i = 0; i < targetDocs.Length; i++)
            {
                var contentLink = new ContentLink(contextManager.CurrentSite, targetDocs[i], targetPtrs[i]);
                wrapper = new ContentWrapper(contentLink.Document);

                // NOTE: do we really need this if statement?  
                // No, if we will always get a contentLink and a wrapper.
                if (wrapper.HasDocument)
                {
                    IDocument doc = wrapper.Document;
                    IBook book = doc.Book;
                    IDocumentFormat format = doc.PrimaryFormat;
                    ISearchResults searchResults = contextManager.SearchResults;

                    if (format != null && format.Description == "text/html")
                    {
                        string contentText = "";

                        //Check to see if the book is in the current user's subscription
                        if (!IsBookInSubscription(doc.Book.Name, contextManager))
                        {
                            contentText = "<div><p>The document " + doc.Name + " was not joined because it is not in your subscription.</p></div>";
                        }
                        else
                        {
                            //*********************************
                            // Hit Highlighting
                            //*********************************
                            //set our bytes to be highlighted or not
                            byte[] contentBytes;

                            if (hilite && searchResults != null && format.ContentTypeId == (int)ContentType.TextHtml &&
                                searchResults.WordInterpretations != null && searchResults.WordInterpretations.Length > 0)
                            {
                                contentBytes = format.GetHighlightedContent(searchResults.WordInterpretations,
                                                                            ContextManager.HILITE_BEGINTAG, ContextManager.HILITE_ENDTAG);
                            }
                            else
                            {
                                format.LogContentAccess(contextManager.CurrentUser);
                                contentBytes = format.Content;
                            }

                            contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

                            if (i == 0)
                            {
                                contentText = Regex.Replace(contentText, "<body([^>]{0,})>", "<body$1>\n" +
                                    "<div class=\"detail\"><div id=\"leftcol\" class=\"col-sm-9\"><div class=\"leftcol_inner\">" + 
                                    "<div class=\"leftcol_content\"><i class=\"fa fa-bookmark docBookmark\" style=\"display:none\"></i>");
                            }

                            //*********************************
                            // Formatting the document at i
                            //*********************************

                            // strip out codification headers from all codification content, except the first one
                            if (i != 0)
                            {
                                contentText = DocumentDisplayHelpers.RemoveCodificationHeaders(contentText);
                            }

                            // If it's an old FASB or archived book, then add the "this document is not current" notice
                            if (DocumentDisplayHelpers.IsDocumentNotCurrent(book.Name))
                            {
                                contentText = DocumentDisplayHelpers.IncludeDocumentNotCurrentNotice(contentText, true);
                            }

                            // If we're suppossed to show sources, add that stylesheet in the content
                            contentText = DocumentDisplayHelpers.IncludeCodificationStyleSheets(contentText, showSources);

                            // remove noPrint spans
                            contentText = DocumentDisplayHelpers.RemoveNoPrintSpans(contentText);

                            // remove all codification footers, except the last one
                            if (i != targetDocs.Length - 1)
                            {
                                contentText = DocumentDisplayHelpers.RemoveCodificationFooters(contentText);
                            }
                        }

                        docString += contentText;
                    }
                    else
                    {
                        throw new Exception(string.Format("DocumentInstance with Id {0} has no PrimaryFormat or the PrimaryFormat is not text/html", doc.Id));
                    }
                }
                else
                {
                    throw new Exception("Cannot join multiple documents where content for a doesn't exist.");
                }
            }
            return docString;
        }

        public string getDocStringsFromUrlParams(string parameters, HttpContext context)
        {
            var contextManager = new ContextManager(context);

            System.Collections.ArrayList targetDocs = new System.Collections.ArrayList();
            System.Collections.ArrayList targetPtrs = new System.Collections.ArrayList();
            System.Collections.ArrayList ids = new System.Collections.ArrayList();
            System.Collections.ArrayList types = new System.Collections.ArrayList();

            string[] pairs = parameters.Split('&');

            for (int i = 0; i < pairs.Length; i++)
            {
                string[] pair = pairs[i].Split('=');
                if (pair[0] == "targetdoc")
                    targetDocs.Add(pair[1]);
                else if (pair[0] == "targetptr")
                    targetPtrs.Add(pair[1]);
                else if (pair[0] == "id")
                    ids.Add(pair[1]);
                else if (pair[0] == "type")
                    types.Add(pair[1]);
            }

            bool hilite = parameters.Contains("hilite");

            bool showSources = parameters.Contains("show_sources");

            string docString = "";

            if (targetDocs.Count>0 && targetPtrs.Count>0)
            {
                docString = getDocStringsForDocumentsByTargets(contextManager, (string[])targetDocs.ToArray(typeof(string)), (string[])targetPtrs.ToArray(typeof(string)), hilite, showSources);
            }
            else if (ids.Count > 0 && types.Count > 0)
            {
                docString = getDocStringsForDocumentsByIds(contextManager, (string[])ids.ToArray(typeof(string)), (string[])types.ToArray(typeof(string)), hilite, showSources);
            }

            return docString;
        }
    }
}