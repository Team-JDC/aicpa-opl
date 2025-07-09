#region
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.User;

using System.Text;
using System.Collections.Generic;
#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for DocumentFormat.
    /// </summary>
    public class DocumentFormat : DestroyerBpc, IDocumentFormat
    {
        #region Constants

        //errors
        private const string MODULE_DOCUMENTFORMAT = "DocumentFormat";
        private const string METHOD_CONTENT = "Content";

        private const string USAGE_EVENT_CONTENT = "Content Accessed";
        private const string MESSAGE_CONTENT = "Book={0};Document={1};Format={2}";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_HIGHLIGHTINGNOTSUPPORTEDFORCONTENTTYPE =
            "The system does not support hit highlighting for the format '{0}'.";

        #endregion Constants

        #region Private

        #region Private Accessors

        private readonly IDocument activeDocument;

        private readonly DocumentDs.DocumentFormatRow activeDocumentFormatRow;

        private byte[] activeContent;

        /// <summary>
        /// </summary>
        private IDocument ActiveDocument
        {
            get { return activeDocument; }
        }

        /// <summary>
        ///   Private accessor for retrieving the document format table row for this document format
        /// </summary>
        private DocumentDs.DocumentFormatRow ActiveDocumentFormatRow
        {
            get
            {
                if (activeDocumentFormatRow == null)
                {
                    throw new NullReferenceException(
                        "The private field activeDocumentFormatRow has not been initialized");
                }
                return activeDocumentFormatRow;
            }
        }

        /// <summary>
        ///   Private accessor for retrieving the content for this document format
        /// </summary>
        private byte[] ActiveContent
        {
            get
            {
                if (activeContent == null)
                {
                    //open the Uri, read it into the byte array, and return it
                    FileStream fs = new FileStream(Uri, FileMode.Open);
                    activeContent = new byte[fs.Length];
                    fs.Read(activeContent, 0, (int) fs.Length);
                    fs.Close();
                }
                return activeContent;
            }
        }

        #endregion Private Accessors

        #region Private Helper Methods



        private string insertClientMarkup(string[] highlightTerms, string docString, string hitAnchor)
        {
            StringBuilder Document = new StringBuilder();


            string retString = string.Empty;
            int pos = docString.IndexOf("</head>");
            if (pos > -1)
            {
                Document.Append(docString.Substring(0, pos));

                Document.Append("<script type=\"text/javascript\" src=\"/js/jquery.highlight-3-mod.js\"></script>");
                Document.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"/Styles/highlight.css\"></link>");
                Document.Append("<script type=\"text/javascript\" src=\"/js/opl2015/highlight.js\"></script>");

            }
            int body = docString.IndexOf("<body");
            int end = docString.IndexOf(">", body + 1)+1;
            if (end > -1)
            {
                Document.Append(docString.Substring(pos,(end-pos)+1));
                Document.Append(string.Format("<input id=\"hhighlight\" type=\"hidden\" value=\"{0}\"/>",string.Join(" ",highlightTerms)));
                Document.Append(string.Format("<input id=\"hitanchor\" type=\"hidden\" value=\"{0}\"/>", hitAnchor));   
            }
            Document.Append(docString.Substring(end));

            return Document.ToString();
        }

        /// <summary>
        /// Will remove noteButtons and contentLink span items.
        /// </summary>
        /// <param name="docString">HTML for document</param>
        /// <returns>Modified HTML</returns>
        private string RemoveLinks(string docString)
        {
            //string CONTENTLINK_REG = "<span *class=\"contentLink\".*?</span>";
            
            string CONTENTLINK_REG = "<span class=\"contentLink\"><img[^>]+src=\"images/portal/main-lock_small\\.gif\"><a[^>]+onclick=\"doLink\\([^\\)]+\\);?\"[^>]+>(.+?)</a><a[^>]+onclick=\"doLink\\([^\\)]+\\);?\"[^>]+><img[^>]+src=\"images/icon-open-new-doc\\.gif\"[^>]+></a></span>";
            
            string NOTESLINK_REG = "<span *class=\"notesButtons\".*?<span class=\"addNote\".*?</span><span class=\"editNote\".*?</span>[\n]? *</span>";

            // We need to save the inner text here. 
            docString = Regex.Replace(docString,CONTENTLINK_REG,"$1",RegexOptions.IgnoreCase|RegexOptions.Singleline);
            docString = Regex.Replace(docString,NOTESLINK_REG,"",RegexOptions.IgnoreCase|RegexOptions.Singleline);

            return docString;

        }


        /// <summary>
        ///   Applies hit highlighting to an HTML document using the specified highlight terms
        /// </summary>
        /// <param name = "docStr"></param>
        /// <param name = "highlightTerms"></param>
        /// <param name = "highlightBeginMarkup"></param>
        /// <param name = "highlightEndMarkup"></param>
        /// <returns></returns>
        private string HighlightHtml(string docStr, string[] highlightTerms, string highlightBeginMarkup,
                                     string highlightEndMarkup)
        {
            return HighlightHtml(docStr, highlightTerms, highlightBeginMarkup, highlightEndMarkup, false);
        }        

        /// <summary>
        ///   Applies hit highlighting to an HTML document using the specified highlight terms
        /// </summary>
        /// <param name = "docStr"></param>
        /// <param name = "highlightTerms"></param>
        /// <param name = "highlightBeginMarkup"></param>
        /// <param name = "highlightEndMarkup"></param>
        /// <param name = "ignoreCase">Ignore case when searching</param>
        /// <returns></returns>
        private string HighlightHtml(string docStr, string[] highlightTerms, string highlightBeginMarkup,
                                     string highlightEndMarkup, bool ignoreCase)
        {
            //this constant is a regular expresion that matches on the template defined in MARKUP_PLACEHOLDER_TEMPLATE
            //note: the begin character ('~') and end character ('`') must not be the same
            const string REGEX_PLACEHOLDER_SINGLE = "(~[0-9]+`)";

            //this constant is a regular expression that will match multiple adjacent occurances of REGEX_PLACEHOLDER_SINGLE
            const string REGEX_PLACEHOLDER_MULTIPLE = REGEX_PLACEHOLDER_SINGLE + "+";

            //this constant is a template for a replacement token used for non-term sequences of text
            //note: the begin character ('~') and end character ('`') must not be the same
            //note: this template must be matched upon by REGEX_PLACEHOLDER_SINGLE
            const string MARKUP_PLACEHOLDER_TEMPLATE = "~{0}`";

            //this string represents our begin markup placeholder token
            string HILITE_BEGINMARKUP_PLACEHOLDER = string.Format(MARKUP_PLACEHOLDER_TEMPLATE, int.MaxValue - 1);

            //this string represents our end markup placeholder token
            string HILITE_ENDMARKUP_PLACEHOLDER = string.Format(MARKUP_PLACEHOLDER_TEMPLATE, int.MaxValue);

            //this is a list of patterns we wish to remove from the text before trying to add our hit highlighting
            //note: one of these patterns needs to exclude the delimiters that we use in our token template
            string[] removeList = new[]
                                      {
                                          "<span class=\"noindex\"><span class=\"glossaryheader\">.*?</span><span class=\"glossarybody\">.*?</span></span>"
                                          , @"<script[^>]*>.*?</script>", @"<head[^>]*>.*?</head>", @"<!--.*?-->",
                                          @"<[^>]*>", @"</[^>]*>", @"\r\n", @"[^A-Za-z0-9'\-~`]"
                                      };

            //associative arrays used to hold the match text and the placeholder text
            ArrayList matchList = new ArrayList();
            ArrayList placeholderList = new ArrayList();

            //set our return string to our document string
            string retStr = docStr;

            //match on each of the items in our remove list
            int count = 0;
            RegexOptions options = RegexOptions.Multiline | RegexOptions.Singleline;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;

            foreach (string removeItem in removeList)
            {
                //get all matches for this pattern (be sure it is case sensitive)
                MatchCollection matches = Regex.Matches(retStr, removeItem,
                                                        options);

                //use this hashtable to determine whether or not we have already matched on a given string (important optimization!)
                Hashtable uniqueMatches = new Hashtable();

                //go through each of the matches
                foreach (string matchString in
                    matches.Cast<Match>().Select(match => match.Groups[0].Value).Where(
                        matchString => !uniqueMatches.Contains(matchString)))
                {
                    //add the matchString to our collection so we will not hit it again (at least not for this removeItem)
                    uniqueMatches.Add(matchString, null);

                    //create a sequential placeholder
                    string placeholder = string.Format(MARKUP_PLACEHOLDER_TEMPLATE, count);

                    //replace our matched text with the sequential placeholder
                    retStr = retStr.Replace(matchString, placeholder);

                    //store our replacement text and our placeholder text
                    matchList.Add(matchString);
                    placeholderList.Add(placeholder);

                    //increment our match counter
                    count++;
                }
            }

            //apply our hit highlighting
            foreach (string highlightTerm in highlightTerms)
            {
                //replace whitespace with the whitespace escape sequence (this will help our phrase query terms match across line breaks)
                string findStrRePatt = REGEX_PLACEHOLDER_SINGLE +
                                       Regex.Replace(highlightTerm, " ", REGEX_PLACEHOLDER_MULTIPLE) +
                                       REGEX_PLACEHOLDER_SINGLE;

                //match on our pattern and set the literal find string to the result of our match (be sure it is case sensitive)
                MatchCollection matches = Regex.Matches(retStr, findStrRePatt,
                                                        options);//                                                        RegexOptions.Multiline | RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    string findStr = match.Groups[0].Value;

                    //set our replace string by placing markup around our literal find string
                    string beforeWhitespace = Regex.Match(findStr, "^" + REGEX_PLACEHOLDER_SINGLE).Groups[0].Value;
                    string afterWhitespace = Regex.Match(findStr, REGEX_PLACEHOLDER_SINGLE + "$").Groups[0].Value;
                    string cleanTerm = Regex.Replace(findStr, "^" + REGEX_PLACEHOLDER_SINGLE, "");
                    cleanTerm = Regex.Replace(cleanTerm, REGEX_PLACEHOLDER_SINGLE + "$", "");

                    string replaceStr = beforeWhitespace + HILITE_BEGINMARKUP_PLACEHOLDER + cleanTerm +
                                        HILITE_ENDMARKUP_PLACEHOLDER + afterWhitespace;

                    //replace our literal find string with our replace string
                    retStr = retStr.Replace(findStr, replaceStr);
                }
            }

            //restore our replacement text in reverse order
            for (int i = matchList.Count - 1; i >= 0; i--)
            {
                string placeholderText = (string) placeholderList[i];
                string replacementText = (string) matchList[i];
                retStr = retStr.Replace(placeholderText, replacementText);
            }

            //replace our hithighlighting tokens with the actual markup
            retStr = retStr.Replace(HILITE_BEGINMARKUP_PLACEHOLDER, highlightBeginMarkup);
            retStr = retStr.Replace(HILITE_ENDMARKUP_PLACEHOLDER, highlightEndMarkup);

            // sburton 2009-12-07: This was not checked in and live prior to redesign, so I
            // am commenting it out, so we don't introduce new code, but have it
            // in case we want it later
            //---// sburton 2009-08-07: Added to address issue of anchor tags inside links
            //---retStr = MoveAnchorsOustideLinks(retStr);

            return retStr;
        }

        // sburton 2009-12-07: This was not checked in and live prior to redesign, so I
        // am commenting it out, so we don't introduce new code, but have it
        // in case we want it later
        //		private const string ANCHOR_TAG_REPLACEMENT = "~~knowlysis-replace";
        //		/// <summary>
        //		/// sburton 2009-08-07: The anchor tag used for next/prev hit was causing problems
        //		/// when the term was inside a link.  This method moves the anchor tag to come directly before
        //		/// the link.
        //		/// 
        //		/// For performance consideration, we will pass in the likely offending anchor tag rather than
        //		/// replace all one by one.
        //		/// </summary>
        //		/// <param name="inputStr"></param>
        //		/// <returns></returns>
        //		private string MoveAnchorsOustideLinks(string inputStr, string offendingAnchorTag)
        //		{
        //			// first replace offending tag with a replacement
        //			string retStr = inputStr.Replace(offendingAnchorTag, ANCHOR_TAG_REPLACEMENT);
        //			
        //			// find any "a" tags
        //			string aTagPattern = string.Format("<a[^/>]*>.*?</a>");
        //
        //			//string retStr = Regex.Replace(retStr, aTagPattern, new MatchEvaluator(this.HandleATags));
        //
        //			// put the actual tag back in
        //			retStr = retStr.Replace(ANCHOR_TAG_REPLACEMENT, offendingAnchorTag);
        //
        //			return retStr;
        //		}

        #endregion

        #endregion

        #region Public

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name = "documentFormatRow"></param>
        public DocumentFormat(DocumentDs.DocumentFormatRow documentFormatRow, IDocument document)
        {
            activeDocumentFormatRow = documentFormatRow;
            moduleName = MODULE_DOCUMENTFORMAT;
            activeDocument = document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Description
        {
            get { return ActiveDocumentFormatRow.Description; }
        }

        /// <summary>
        ///   The identifier of the content type description
        /// </summary>
        public int ContentTypeId
        {
            get { return ActiveDocumentFormatRow.ContentTypeId; }
        }

        /// <summary>
        /// </summary>
        public string Uri
        {
            get
            {
                string uri = ActiveDocumentFormatRow.Uri;
                string docUri = string.Empty;
                if (uri != string.Empty)
                {
                    docUri = Path.Combine(Book.Book.BOOK_CONTENTFOLDER, uri);
                }
                return docUri;
            }
        }

        /// <summary>
        ///   A byte array containing the content of the document in the context format.
        /// </summary>
        public byte[] Content
        {
            get
            {
                if (ActiveDocument.Book != null && ActiveDocument.Book.Site != null &&
                    ActiveDocument.Book.Site.User != null)
                {
                    // 2010-12-27 sburton:  because of how we are requesting docs via Id rather
                    // than target doc and target pointer, we usually do not have a site (and therefore user object)
                    // at this level, so we have decided to have the UI call the LogContentAccess directly.

                    //LogContentAccess(ActiveDocument.Book.Site.User);
                }

                return ActiveContent;
            }
        }

        public void LogContentAccess(IUser user)
        {
            // log that a user has access this document's content.
            if (Event.IsEventToBeLogged(EventType.Usage, USAGE_SEVERITY_DOCUMENT_ACCESSED, moduleName,
                                        METHOD_CONTENT))
            {
                if (ActiveDocument.Book != null && user != null)
                {
                    IEvent logEvent = new Event(EventType.Usage, DateTime.Now, USAGE_SEVERITY_DOCUMENT_ACCESSED,
                                                ModuleName, METHOD_CONTENT, USAGE_EVENT_CONTENT,
                                                string.Format(MESSAGE_CONTENT, ActiveDocument.Book.Name,
                                                              ActiveDocument.Name, Description),
                                                user.UserId, user.UserSecurity.SessionId);
                    logEvent.Save(false);
                }
            }
        }

        /// <summary>
        /// </summary>
        public long ContentLength
        {
            get { return ActiveContent.Length; }
        }

        /// <summary>
        ///   A boolean value indicating whether or not this is the primary format for a document.
        /// </summary>
        public bool IsPrimary
        {
            get { return ActiveDocumentFormatRow.Primary; }
        }

        /// <summary>
        ///   A boolean value indicating whether or not the content should be "automatically downloaded" in the client UI.
        /// </summary>
        public bool IsAutoDownload
        {
            get { return ActiveDocumentFormatRow.AutoDownload; }
        }

        #endregion

        #region Methods


        /// <summary>
        ///   Returns a byte array containing the document format's content with hit highlighting markup applied. This
        ///   implementation only supports hit highlighting insertion for HTML and XML.
        /// </summary>
        /// <param name = "highlightTerms">The terms you wish to highlight</param>
        /// <returns></returns>
        public byte[] GetHighlightedContent(string[] highlightTerms, string highlightBeginMarkup,
                                            string highlightEndMarkup)
        {
            return GetHighlightedContent(highlightTerms, highlightBeginMarkup, highlightEndMarkup, false);
        }
        
        /// <summary>
        ///   Returns a byte array containing the document format's content with hit highlighting markup applied. This
        ///   implementation only supports hit highlighting insertion for HTML and XML.
        /// </summary>
        /// <param name = "highlightTerms">The terms you wish to highlight</param>
        /// <returns></returns>
        public byte[] GetHighlightedContent(string[] highlightTerms, string highlightBeginMarkup,
                                            string highlightEndMarkup, bool ignoreCase)
        {
            //our return byte array
            byte[] retBytes = null;

            //switch on the format - not all formats support hit highlighting
            string format = Description;
            ContentType contentType = GetContentTypeFromDescription(format);
            //(ContentType)Enum.Parse(typeof(ContentType), NormalizeContentTypeDescription(format))
            switch (contentType)
            {
                case ContentType.TextHtml:
                    //get the document bytes and convert to a string
                    string docString = ByteArrayToStr(Content);
                    //Temp fix to remove unencode spaces
                    docString = docString.Replace("&#32;"," ");
                    //apply the html highlighting
                    string docHighlightString = HighlightHtml(docString, highlightTerms, highlightBeginMarkup,
                                                              highlightEndMarkup, ignoreCase);
                    retBytes = StrToByteArray(docHighlightString);

                    break;
                case ContentType.TextXml:
                default:
                    throw new Exception(string.Format(ERROR_HIGHLIGHTINGNOTSUPPORTEDFORCONTENTTYPE, format));
            }
            return retBytes;
        }

        public byte[] GetClientHighlightContent(string[] highlightTerms)
        {
            return GetClientHighlightContent(highlightTerms, string.Empty);
        }

        public byte[] GetClientHighlightContent(string[] highlightTerms, string hitAnchor)
        {
           
            byte[] retBytes = null;

            //switch on the format - not all formats support hit highlighting
            string format = Description;
            ContentType contentType = GetContentTypeFromDescription(format);
            //(ContentType)Enum.Parse(typeof(ContentType), NormalizeContentTypeDescription(format))
            switch (contentType)
            {
                case ContentType.TextHtml:
                    //get the document bytes and convert to a string
                    string docString = ByteArrayToStr(Content);
                    string fileStream = insertClientMarkup(highlightTerms, docString, hitAnchor);
                    retBytes = StrToByteArray(fileStream);
                    break;
                case ContentType.TextXml:
                default:
                    throw new Exception(string.Format(ERROR_HIGHLIGHTINGNOTSUPPORTEDFORCONTENTTYPE, format));
            }
            

            return retBytes;
        }

        /// <summary>
        /// Get the Content without all the EditNote and contentLink items.
        /// </summary>
        /// <returns>byte array</returns>
        public byte[] GetContentNoDoLinks()
        {
            byte[] retBytes = null;

            //switch on the format - not all formats support hit highlighting
            string format = Description;
            ContentType contentType = GetContentTypeFromDescription(format);
            //(ContentType)Enum.Parse(typeof(ContentType), NormalizeContentTypeDescription(format))
            switch (contentType)
            {
                case ContentType.TextHtml:
                    //get the document bytes and convert to a string
                    string docString = ByteArrayToStr(Content);
                    string fileStream = RemoveLinks(docString);
                    retBytes = StrToByteArray(fileStream);                    
                    break;
                case ContentType.TextXml:
                default:
                    throw new Exception(string.Format(ERROR_HIGHLIGHTINGNOTSUPPORTEDFORCONTENTTYPE, format));
            }


            return retBytes;

        }

        /// <summary>
        /// Removes all html tags from string and leaves only plain text
        /// Removes content of <xml></xml> and <style></style> tags as aim to get text content not markup /meta data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string HtmlStrip(string input)
        {
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", string.Empty); // remove all <xml></xml> tags and anything inbetween.  
            return Regex.Replace(input, @"<(.|\n)*?>", string.Empty); // remove any tags but not there content "<p>bob<span> johnson</span></p>" becomes "bob johnson"
        }

        public void LogEvent(string method, string name, string description)
        {

            IEvent logEvent = new Event(EventType.Usage, DateTime.Now, USAGE_SEVERITY_DOCUMENT_ACCESSED,
                                        ModuleName, method, name, description);
            logEvent.Save(false);
        }

       

        public Dictionary<string, int> GetDocumentHits(string[] highlightTerms)
        {
		    string HILITE_ANCHORNAME = "destroyer_hilite";
            //string HILITE_BEGINTAG = "<a name='" + HILITE_ANCHORNAME + "'></a><span style='color:white; background-color:navy;'>";
            //string HILITE_ENDTAG = "</span>";
            string HILITE_BEGINTAG = "<a name='" + HILITE_ANCHORNAME + "'>";
            string HILITE_ENDTAG = "</a>";
                       

            byte[] contentBytes = null;
            //DateTime before = DateTime.Now;
            contentBytes = GetHighlightedContent(highlightTerms, HILITE_BEGINTAG, HILITE_ENDTAG, true);
            //DateTime after = DateTime.Now;
            //LogEvent("GetDocumentHits", "After GetHighLightedContent", string.Format("{0}:Elapsed Time (ms) {1}", this.Description, after.Subtract(before).TotalMilliseconds));
            string contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

            SortedDictionary<int, string> title = new SortedDictionary<int, string>();
            Dictionary<string, int> sectionVals = new Dictionary<string, int>();
            //string htmlMatch = "<h[0-9]+ class=\"title\"[^>]*?>.*?<a name=\"([^\"]+)\"[^>]*?></a>(.*?)</h[0-9]+>";
            string htmlMatch = "<h[0-9]+ class=\"title\"[^>]*?>(.*?)</h[0-9]+>";
            string anchorMatch = "<a name=\"([^\"]+)\"[^>]*?></a>";
            //<h[0-9]+ class=\"title\"[^>]*?>.*?<a name=\"([^\"]+)\"[^>]*?></a><span class=[^>]+>(.*?)</span>

            //key to document
            string key = string.Format("{0}|{1}|{2}",this.ActiveDocument.Name, 0,this.ActiveDocument.Title);
            title.Add(0, key);
            sectionVals.Add(key, 0);
            //before = DateTime.Now;
            foreach (Match match in Regex.Matches(contentText, htmlMatch,RegexOptions.Singleline| RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(match.Groups[0].ToString(), anchorMatch))
                {
                    string anchor = Regex.Match(match.Groups[0].ToString(), anchorMatch).Groups[1].ToString();
                    string text = match.Groups[1].ToString();
                    if (!Regex.IsMatch(text, "<span class=\"ps_para_number\">"))
                    {
                        text = HtmlStrip(text);
                        text = Regex.Replace(text, "\r\n", " ").Trim();

                        if (text.Trim().Equals(string.Empty))
                            text = this.ActiveDocument.Title;
                        else if (text.Trim().Length > 255)
                            text = text.Substring(0, 255) + "...";

                        key = string.Format("{0}|{1}|{2}", anchor, match.Index, text);
                        // clean up
                        key = Regex.Replace(key, "<a name='destroyer_hilite'>(.*?)</a>", "$1", RegexOptions.Singleline);
                        key = Regex.Replace(key, "<sup>.*?</sup>", string.Empty, RegexOptions.IgnoreCase);
                        key = Regex.Replace(key, "\\(.*?\\)", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);


                        title.Add(match.Index, key);
                        sectionVals.Add(key, 0);
                    }
                }
            }
            //after = DateTime.Now;
            //LogEvent("GetDocumentHits", "After Build Titles", string.Format("{0}:Elapsed Time (ms) {1}", this.Description, after.Subtract(before).TotalMilliseconds));

            //before = DateTime.Now;
            foreach (Match match in Regex.Matches(contentText, "<a name='destroyer_hilite'>"))
            {
                if (title.Count > 0)
                {
                    string val = title.Last(x => match.Index >= x.Key).Value;
                    sectionVals[val]++;
                }
            }
            //after = DateTime.Now;
            //LogEvent("GetDocumentHits", "After Build Table of hits", string.Format("{0}:Elapsed Time (ms) {1}", this.Description, after.Subtract(before).TotalMilliseconds));

            var retVal = sectionVals.Where(x => x.Value > 0).ToDictionary(gdc => gdc.Key, gdc => gdc.Value);
            if (retVal.Count == 0)
            {                
                sectionVals[sectionVals.Keys.First()]++;
                retVal = sectionVals.Where(x => x.Value > 0).ToDictionary(gdc => gdc.Key, gdc => gdc.Value);
            }


            return retVal;
           
        }       


        #endregion

        #endregion
    }
}