using System;
using System.Web;
using System.Web.SessionState;
using System.Text.RegularExpressions;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using MainUI.Shared;
using System.Text.RegularExpressions;
using System.Xml;
using MainUI.WS;
using System.Collections.Generic;
using System.Configuration;


namespace MainUI.Handlers
{
    /// <summary>
    /// Summary description for GetDocument
    /// </summary>
    public class GetDocument : IHttpHandler, IRequiresSessionState
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
            string targetDoc = context.Request[ContextManager.REQPARAM_TARGETDOC];
            string targetPtr = context.Request[ContextManager.REQPARAM_TARGETPTR];

            string id = context.Request[ContextManager.REQPARAM_ID];
            string type = context.Request[ContextManager.REQPARAM_TYPE];


            bool hilite = context.Request.Params[ContextManager.REQPARAM_HITHIGHLIGHTS] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_HITHIGHLIGHTS]);

            string hitAnchor = context.Request.Params[ContextManager.REQPARAM_HITANCHOR] == null
                              ? string.Empty
                              : context.Request[ContextManager.REQPARAM_HITANCHOR];

            string tableId = context.Request.Params[ContextManager.REQPARAM_TABLE] == null
                              ? string.Empty
                              : context.Request[ContextManager.REQPARAM_TABLE];

            bool showSources = context.Request.Params[ContextManager.REQPARAM_SHOWSOURCES] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_SHOWSOURCES]);

            bool isLMS = context.Request.Params[ContextManager.REQPARAM_LMS] == null
                              ? false
                              : bool.Parse(context.Request.Params[ContextManager.REQPARAM_LMS]);

            ContextManager contextManager = new ContextManager(context);
            ContentWrapper wrapper;

            string crumb = string.Empty;
            Content ct = new Content();
            List<BreadcrumbNode> bns = new List<BreadcrumbNode>();
            
            // If we have valid id/type values on the query string
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(type))
            {
                int nodeId = Convert.ToInt32(id);
                IPrimaryContentContainer container = PrimaryContentContainer.ConstructContainer(contextManager.CurrentSite, nodeId, type);
                wrapper = container.PrimaryContent;
                crumb = ct.GetFullBreadcrumbLinks(nodeId, type);
                bns = ct.GetFullBreadcrumb(nodeId, type);
            }
            else // Otherwise, use targetdoc/targetptr instead of id/type
            {
                ContentLink contentLink = new ContentLink(contextManager.CurrentSite, targetDoc, targetPtr);                
                wrapper = new ContentWrapper(contentLink.Document);
                SubscriptionSiteNode ssn = ct.ResolveContentLink(targetDoc, targetPtr);
                crumb = ct.GetFullBreadcrumbLinksByTargetDocTargetPtr(targetDoc, targetPtr);
                bns = ct.GetFullBreadcrumb(ssn.Id, ssn.Type);
                
            }

            if (wrapper.HasDocument)
            {   
                //make sure the document is in our subscription
                if (wrapper.Document.InSubscription && IsBookInSubscription(wrapper.Document.Book.Name, contextManager))
                {
                    IUser user = contextManager.CurrentUser;

                    // Do we need to show the user a license agreement before letting them see the content?
                    if (user.ReferringSiteValue == ReferringSite.C2b &&
                        user.LicenseAgreementValue != LicenseAgreementStatus.Agreed &&
                        Book.IsAICPABookWithLicenseAgreement(wrapper.Document.Book))
                    {
                        contextManager.Context.Server.Transfer(string.Format("~/static/LicenseAgreement.aspx?{0}={1}&{2}={3}",
                            ContextManager.REQPARAM_ID, wrapper.Document.Id,
                            ContextManager.REQPARAM_TYPE, Enum.GetName(typeof(NodeType), NodeType.Document)));
                    }
                    else
                    {
                        StreamBackDocument(contextManager, context,  wrapper.Document, hilite, showSources, isLMS, crumb, hitAnchor, tableId, bns);
                    }
                }
                else
                {
                    //    context.Response.Redirect("D_ViewDocumentNotAuthorized.aspx", true);
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

        private void StreamBackDocument(ContextManager contextManager, HttpContext context,  IDocument doc, bool hilite, bool showSources, bool isLMS, string crumb, string hitAnchor, string tableId, List<BreadcrumbNode> breadcrumbNodes)
        {
            IBook book = doc.Book;
            IDocumentFormat format = doc.PrimaryFormat;
            ISearchResults searchResults = contextManager.SearchResults;

            string targetDoc = context.Request[ContextManager.REQPARAM_TARGETDOC];
            string targetPtr = context.Request[ContextManager.REQPARAM_TARGETPTR];

            string id = context.Request[ContextManager.REQPARAM_ID];
            string type = context.Request[ContextManager.REQPARAM_TYPE];

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
                    string[] words;
                    if (searchResults.SearchCriteria.SearchType == SearchType.ExactPhrase)
                        words = new string[] { searchResults.SearchCriteria.Keywords.Replace(" ", "+") };
                    else words = searchResults.WordInterpretations;

                    contentBytes = format.GetClientHighlightContent(words, hitAnchor);
                    
                }
                else if (isLMS)
                {
                    contentBytes = format.GetContentNoDoLinks();

                } else {
                    format.LogContentAccess(contextManager.CurrentUser);
                    contentBytes = format.Content;
                }

                string contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

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
                contentText = AddJavaScriptAndCssRefs(contentText,contextManager.CurrentUser.ReferringSiteValue);

                // add uinique Id to Notes Css
                contentText = AddUniqueIdToNotesCss(contentText);

                // add uinique Id to Bookmark CSS
                contentText = AddUniqueIdToBookmarkCss(contentText);

                // add params to User Preferences Css
                contentText = AddParamsToUserPreferencesCss(contentText, contextManager);
                    
                // add a div and an image to test for css and image loaded
                contentText = AddDivForCssAndImageCheck(contentText);
                
                
                // add the breadcrumb
                contentText = Regex.Replace(contentText, "<body([^>]{0,})>", "<body$1>\n"+
                "<div class=\"detail\"><div id=\"leftcol\" class=\"col-sm-9\"><div class=\"leftcol_inner\">" + crumb +
                    "<div class=\"leftcol_content\"><i class=\"fa fa-bookmark docBookmark\" style=\"display:none\"></i>");


                if (!string.IsNullOrEmpty(targetDoc))
                {
                    string newScriptTag = string.Format("<script>var tDoc=\"{0}\";\nvar tPtr=\"{1}\";</script>", targetDoc, targetPtr);
                    contentText = Regex.Replace(contentText, "<body", newScriptTag + "\n<body");
                }
                else
                {
                    string newScriptTag = string.Format("<script>var nodeId=\"{0}\";\nvar nodeType=\"{1}\"</script>", id, type);
                    contentText = Regex.Replace(contentText, "<body", newScriptTag + "\n<body");
                
                }

                if (isLMS)
                    contentText = Regex.Replace(contentText, "<body([^>]{0,})>", "<body$1>\n<p style=\"font-size: 12; color: #000000; font-weight: bold\">" + crumb + "</p>");
                //Add Copy/Paste Buffer area.
                contentText = Regex.Replace(contentText, "</body>", "<div id=\"ideal\"></div>"+
                    "</div><!-- leftcol_content -->"+
                    "</div><!-- leftcol_inner --> </div><!-- leftcol --></div><!-- detail.. --></body>");


                //for note popup
                contentText = Regex.Replace(contentText, "</body>", GetModalHTML() + "</body>");
                


                if (!string.IsNullOrEmpty(tableId))
                {
                    string includes = "<script type=\"text/javascript\" src=\"/js/opl2015/handlers.js\"></script>\n<script type=\"text/javascript\" src=\"/js/opl2015/webServiceHelpers.js\"></script>\n<link rel=\"stylesheet\" href=\"/elements/css/main.css\" />\n";
                    string expression = "<div[^>]+class=\"expandedTable\"[^>]*id=\"" + tableId + "\">.+?<table.+?</table>.*?</div>";
                    string table = Regex.Match(contentText, expression, RegexOptions.Singleline).ToString();
                    table = Regex.Replace(table, "<div class=\"expandedTable\" style=\"display:none;?\"", "<div class=\"expandedTable\"");
                    globalTable = table;
                    contentText = Regex.Replace(contentText, "(<body[^>]*>).+(</body>)", new MatchEvaluator(ReplaceBody), RegexOptions.Singleline);
                    contentText = Regex.Replace(contentText, "(</head>)", includes+"</head>", RegexOptions.Singleline);
                    

                }

                string script = string.Format("<script>{0}</script>", AICPAAnalytics.ContentTracking(breadcrumbNodes));

                contentText = Regex.Replace(contentText, "</body>", script + "</body>");              

                // Write to response stream and end the response
                contextManager.Context.Response.Write(contentText);
                contextManager.Context.Response.End();
            }
            else
            {
                throw new Exception(string.Format("DocumentInstance with Id {0} has no PrimaryFormat", doc.Id));
            }
        }

        private static string globalTable = "";
        private string ReplaceBody(Match m)
        { 
            string retStr = "";
            string body = m.ToString();
            retStr = "<body class=\"fullTable\">" + globalTable + "</body>";
            return retStr;
        }

        private string GetModalHTML()
        {
            return " <!-- Modal -->" +
"        <div class=\"modal fade\" id=\"myModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\">" +
"            <div class=\"modal-dialog\">" +
"                <div class=\"modal-content\">" +
"                    <div class=\"modal-header\">" +
"                        <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
"                        <h3 class=\"modal-title\" id=\"myModalLabel\">Add Note</h3>" +
"                    </div>" +
"                    <div class=\"modal-body\">" +
"                        <label>Title*</label>" +
"                        <br>" +
"                        <input id=\"modalTitle\" type=\"text\" placeholder=\"Title is required\">" +
"                        <br>" +
"                        <label>Note Contents</label>" +
"                        <br>" +
"                        <textarea id=\"modalTextArea\"></textarea>" +
"                    </div>" +
"                    <div class=\"modal-footer\">" +
"                        <button type=\"button\" id=\"btnModalSave\" class=\"btn btn-primary\" data-dismiss=\"modal\">Save</button>" +
"                        <button type=\"button\" id=\"btnModalClose\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>" +
"                    </div>" +
"                </div>" +
"            </div>" +
"        </div>" +
"        <!-- End Modal -->    " +
"<!-- Modal Are You Sure (AYS) -->" +
"        <div class=\"modal fade\" id=\"myModalAYS\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\">" +
"			<div class=\"modal-dialog\">" +
"				<div class=\"modal-content\">" +
"					<div id=\"aysDivHeader\" class=\"modal-header\">" +
"						Are you sure?" +
"					</div>" +
"					<div id=\"aysDivBody\" class=\"modal-body\">" +
"						Are you sure you want to delete? " +
"					</div>" +
"					<div class=\"modal-footer\">" +
"						<button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Cancel</button>" +
"						<a href=\"#\" class=\"btn btn-danger danger\">Delete</a>" +
"					</div>" +
"				</div>" +
"			</div>" +
"		</div>" +
"        <!-- End Modal -->		";

           
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

        private string AddUniqueIdToNotesCss(string content)
        {
            // we may want to push this down to the API

            // <link href="Handlers/GetResource.ashx?type=notes&targetdoc=ps" rel="stylesheet" type="text/css"/>
            string notesCssRef = "/Handlers/GetResource.ashx?type=notes&";
            string uniqueNotesCssRef = string.Format("{0}uid={1}&", notesCssRef, Guid.NewGuid().ToString());

            string newContent = content.Replace(notesCssRef, uniqueNotesCssRef);

            return newContent;
        }

        private string AddUniqueIdToBookmarkCss(string content)
        {
            // we may want to push this down to the API

            //<link href="Handlers/GetResource.ashx?type=bookmark&amp;fed-egy&amp;targetdoc=fed-egy" rel="stylesheet" type="text/css">
            string bookmarkCssRef = "/Handlers/GetResource.ashx?type=bookmark&";
            string uniqueBookmarkCssRef = string.Format("{0}uid={1}&", bookmarkCssRef, Guid.NewGuid().ToString());

            return content.Replace(bookmarkCssRef, uniqueBookmarkCssRef);
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

        private string AddDivForCssAndImageCheck(string content)
        {
            //first add image and div to the content
            string body = "</body>";
            string bodyReplacement = @"
        <div id='css-loaded'>
            <img src='/images/loadTest.gif' alt='' />
        </div>
    </body>";
            string newContent = content.Replace(body, bodyReplacement);

            //second add to the content
            string head = "</head>";
            string headReplace = "<link rel='stylesheet' type='text/css' href='/Handlers/GetResource.ashx?type=css_loaded_test&uid=" + Guid.NewGuid().ToString() + "' /></head>";
            string newContent2 = newContent.Replace(head, headReplace);

            return newContent2;
        }

        private string AddJavaScriptAndCssRefs(string content, ReferringSite referringSite)
        {
            // this could potentially be combined with the AddDivForCssAndImageCheck method above
            // for better performance

            string newCssAndJs = string.Empty;

            // subscription check (for the locked icons next to the links)
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='/Handlers/GetResource.ashx?type=subscription_access' />";
            //newCssAndJs += "<link rel='stylesheet' type='text/css' href='/Styles/Base.css' />";
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='/Styles/notes.css' />";
            
            newCssAndJs += "<link rel='stylesheet' type='text/css' href='/resources/jquery.treeview.css' />";
            //newCssAndJs += "<script type='text/javascript' src='/js/jquery-1.4.2.min.js'></script>";
            //newCssAndJs += "<script type='text/javascript' src='/js/includeInIframe.js'></script>";

            //if ((referringSite == ReferringSite.Ethics || (referringSite == ReferringSite.EthicsUser)))
            //    newCssAndJs += "<script type='text/javascript' src='/js/textOperations.js'></script>";

            // TODO: these are needed for the faf toc, we should probably just put them in the faf content,
            // so we don't have to include them for every content page.
            //newCssAndJs += "<script src='/resources/jquery.cookie.js' type='text/javascript'></script>";
            //newCssAndJs += "<script src='/resources/jquery.treeview.js' type='text/javascript'></script>";
            //newCssAndJs += "<script src='/resources/jquery.treeview.async.js' type='text/javascript'></script>";
            //newCssAndJs += "<script src='/js/processFeatures.js' type='text/javascript'></script>";

            //string feat = (HttpContext.Current.Session["D_FEATURES"] == null ? string.Empty : HttpContext.Current.Session["D_FEATURES"].ToString());
            //string returnUrl = (HttpContext.Current.Session["D_RETURNURL"] == null ? string.Empty : HttpContext.Current.Session["D_RETURNURL"].ToString());
            //newCssAndJs += "<script type='text/javascript'> " +
            //               " var d_features = [" + feat + "]; " +
            //               " var d_returnurl = '"+returnUrl + "';"+
            //               "</script>";
            string headOrig = "<head>";
            string headReplace = headOrig + newCssAndJs;
            content = Regex.Replace(content, "<head[^>]+>", "<head>", RegexOptions.IgnoreCase);
            string newContent = content.Replace(headOrig, headReplace);

            // These two lines add 'top.' to all the onclicks
            string oldLinkPattern = @"(\s+onclick=[""'])";
            string replacement = @"$1top.";
            //JTS: This regular expression was timing out in some cases.  I have replaced it with the 4 string replace statements below.
            //newContent = Regex.Replace(newContent, oldLinkPattern, replacement, RegexOptions.IgnoreCase);
            newContent = newContent.Replace("onclick=\"", "onclick=\"top.");
            newContent = newContent.Replace("onClick=\"", "onClick=\"top.");
            newContent = newContent.Replace("onclick=\'", "onclick=\'top.");
            newContent = newContent.Replace("onClick=\'", "onClick=\'top.");
            

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


    }
}