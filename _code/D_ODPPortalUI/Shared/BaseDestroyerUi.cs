using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;

namespace D_ODPPortalUI.Shared
{
    /// <summary>
    /// Summary description for BaseDestroyerUi.
    /// </summary>
    public class DestroyerUi : Page
    {
        public const string USER_SECURITYBYPASSDOMAIN_KEY = "User_SecurityBypassDomain";

        public static string UserSecurityBypassDomainValue =
            ConfigurationManager.AppSettings[USER_SECURITYBYPASSDOMAIN_KEY];

        #region Enums

        #region DocCmd enum

        public enum DocCmd
        {
            DisplayDocument = 0,
            JoinSectionsRecurse = 1
        }

        #endregion

        #region DocTab enum

        public enum DocTab
        {
            Document = 0,
            Goto = 1,
            WhatLinksHere = 2,
            Archive = 3,
            CrossRef = 4,
            JoinSections = 5
        }

        #endregion

        #region HelpTopic enum

        /// <summary>
        /// help topics
        /// </summary>
        public enum HelpTopic
        {
            Document = 0,
            MySubscriptions = 1,
            Search = 2,
            QuickFind = 3,
            Toc = 4
        }

        #endregion

        #region LinkReferrer enum

        /// <summary>
        /// enum that defines where a content link came from 
        /// </summary>
        public enum LinkReferrer
        {
            Content = 0,
            HomePage = 1,
            SearchResults = 2,
            Unknown = 3
        }

        #endregion

        #region PortalTab enum

        /// <summary>
        /// portal tabs (these reflect ids placed in PortalCfg.xml)
        /// </summary>
        public enum PortalTab
        {
            Home = 1,
            TocDoc = 2,
            Document = 3,
            Search = 5,
            QuickFind = 4,
            Help = 98,
            Admin = 99
        }

        #endregion

        #region TocNodeValue enum

        /// <summary>
        /// enum that defines key names stored in the toc
        /// </summary>
        public enum TocNodeValue
        {
            NodeTypeDesc = 0,
            NodeId = 1,
            NodeName = 2
        }

        #endregion

        #endregion Enums

        #region Constants

        //errors

        public const string ALTERNATEBOOK_SUFFIX = "_fallback";
        public const string APPPARAM_QUICKFIND_DT_XML = "DocumentTypeXml";
        public const string APPPARAM_QUICKFIND_SM_XML = "SubjectMatterXml";
        public const string BOOK_PREFIX_AAG = "aag";
        public const string BOOK_PREFIX_ABM = "abm";
        public const string BOOK_PREFIX_ARA = "ara";
        public const string BOOK_PREFIX_ARCH = "arch";
        public const string BOOK_PREFIX_ATT = "att";
        public const string BOOK_PREFIX_CHK = "chk";
        public const string BOOK_PREFIX_EMAP = "emap";
        public const string BOOK_PREFIX_FAF = "faf";
        public const string BOOK_PREFIX_FASB = "fasb";
        public const string BOOK_PREFIX_GASB = "gasb";
        public const string BOOK_PREFIX_WHATSNEW = "whatsnew";
        public const string COMMAND_SUBSCRIPTIONCLICK = "SubscriptionClick";
        public const string DISPLAYPATH_SEP = " > ";

        public const string ERROR_DOCUMENTNOTFOUND =
            "The requested document does not exist on this site (targetdoc: '{0}'; targetptr: '{1}').";

        public const string ERROR_RESOURCEBOOKACCESSDENIED =
            "You do not have access to the specified resource (resource book: '{0}', resource: '{1}'.)";

        public const string ERROR_RESOURCEFOLDERNOTFOUND =
            "A resource folder was not found for the specified resource book (resource book: '{0}', resource: '{1}'.)";

        public const string ERROR_RESOURCENAMEINVALID =
            "The specified resource name is invalid (resource book: '{0}', resource: '{1}'.)";

        public const string ERROR_RESOURCENOTFOUND =
            "The specified resource was not found (resource book: '{0}', resource: '{1}'.)";

        public const string ERROR_SEARCHINDEXOUTOFDATE =
            "Error instantiating the document. The site index may be out-of-date and should be rebuilt.";

        public const string ERROR_TOCSYNCFAILURE = "Error synchronizing the table of contents to the path '{0}'.";
        public const string HILITE_ANCHORNAME = "destroyer_hilite";
        public const string HILITE_ENDTAG = "</span>";
        public const string PAGE_AUTHENTICATIONFAILED = "D_AuthFailed.aspx";
        public const string PAGE_SESSIONTIMEDOUT = "D_SessionTimeOut.aspx";
        public const char RADTREE_IDSEPCHAR = ':';
        public const char RADTREE_TOCPATHSEPCHAR = '/';
        public const char RADTREE_VALUESEPCHAR = '~';
        public const string REQPARAM_BOOKFOLDERID = "d_bfid";

        //request parameter constants
        public const string REQPARAM_CURRENTBOOKNAME = "d_bn";
        public const string REQPARAM_CURRENTDOCUMENTANCHORNAME = "d_an";
        public const string REQPARAM_CURRENTDOCUMENTFORMATID = "d_ft";
        public const string REQPARAM_CURRENTDOCUMENTNAME = "d_dn";
        public const string REQPARAM_HELPTOPIC = "h_tp";
        public const string REQPARAM_HITHIGHLIGHTS = "d_hh";
        public const string REQPARAM_INDEXINGSTAGING = "staging";
        public const string REQPARAM_LINKREFERRER = "linkreferrer";
        public const string REQPARAM_LINKTYPE = "linktype";
        public const string REQPARAM_PRINTDESCENDANTS = "p_pd";
        public const string REQPARAM_SEARCHDIMENSIONVALUEIDS = "s_n";
        public const string REQPARAM_SITEFOLDERID = "d_sfid";
        public const string REQPARAM_TARGETCMD = "targetcmd";
        public const string REQPARAM_TARGETDOC = "targetdoc";
        public const string REQPARAM_TARGETPTR = "targetptr";
        public const string REQPARAM_TARGETTAB = "targettab";
        public const string REQPARAM_TOCSYNCPATH = "t_sp";
        public const string ROLLUP_COUNT = "RollUp_Count";
        public const string ROLLUP_TITLE_AAG = "Audit and Accounting Guides/Audit Risk Alerts";
        public const string ROLLUP_TITLE_ARCH = "Archive Library";
        public const string ROLLUP_TITLE_CHK = "Checklists and Illustrative Financial Statements";
        public const string ROLLUP_TITLE_EMAP = "E-MAP Library";
        public const string ROLLUP_TITLE_FASB = "FASB Accounting Standards Codification";
        public const string ROLLUP_TITLE_GASB = "GASB Library";
        public const string SESSPARAM_CURRENTBOOK = "CurrentBook";
        public const string SESSPARAM_CURRENTDOCUMENT = "CurrentDocument";
        public const string SESSPARAM_CURRENTDOCUMENTANCHOR = "CurrentDocumentAnchor";
        public const string SESSPARAM_CURRENTSEARCHRESULTS = "CurrentSearchResults";
        public const string SESSPARAM_CURRENTSITE = "CurrentSite";
        public const string SESSPARAM_CURRENTSITESTATUS = "CurrentSiteStatus";
        public const string SESSPARAM_CURRENTUSER = "CurrentUser";
        public const string SESSPARAM_QUERYCACHE = "querycache";
        public const string SESSPARAM_SHOWHIDESOURCE = "showhidesource";
        public static string EMPTY_STRING = string.Empty;

        public static string HILITE_BEGINTAG = "<a name='" + HILITE_ANCHORNAME +
                                               "'></a><span style='color:white; background-color:navy;'>";

        #endregion Constants

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IUser GetCurrentUser(Page page)
        {
            //initialize to null
            IUser retUser = null;

            if (string.IsNullOrEmpty(UserSecurityBypassDomainValue))
            {
                //try to get the user from the session
                retUser = (IUser) page.Session[SESSPARAM_CURRENTUSER];
                if (retUser == null)
                {
                    // The user should be created in the Seamless login page or it should be on the session.  Else the users session has timed out.
                    page.Response.Redirect(PAGE_SESSIONTIMEDOUT);
                }
            }
            else
            {
                retUser = new User(Guid.NewGuid(), ReferringSite.Csc);
                string domainList = UserSecurityBypassDomainValue;
                retUser.LogOn(page.Session.SessionID, domainList);
                page.Session[SESSPARAM_CURRENTUSER] = retUser;
            }

            //return the user
            return retUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ISite GetCurrentSite(Page page)
        {
            //for testing site cache performance, turn this on
            bool CACHE_SITE_ON_SESSION = true;

            //initialize to null
            ISite retSite = null;

            //get the current user
            IUser user = GetCurrentUser(page);

            //caching?
            if (CACHE_SITE_ON_SESSION)
            {
                //see if the site is on the session
                retSite = (ISite) page.Session[SESSPARAM_CURRENTSITE];
                if (retSite == null)
                {
                    //get the site status from the session
                    var siteStatus = (SiteStatus) page.Session[SESSPARAM_CURRENTSITESTATUS];

                    //get the site
                    retSite = new Site(user, siteStatus);

                    //test: store the site on the session
                    page.Session[SESSPARAM_CURRENTSITE] = retSite;
                }
            }
            else
            {
                //get the site status from the session
                var siteStatus = (SiteStatus) page.Session[SESSPARAM_CURRENTSITESTATUS];

                //get the site
                retSite = new Site(user, siteStatus);
            }

            //return the site
            return retSite;
        }

        /// <summary>
        /// Retrieves the current book from the url or, if not found, the session
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
//		public static IBook GetCurrentBookx(System.Web.UI.Page page)
//		{
//			//this is the book we will return
//			IBook book = null;
//
//			//get the current site
//			ISite site = GetCurrentSite(page);
//			
//			//grab the current book name from the request
//			string bookName = page.Request.Params[REQPARAM_CURRENTBOOKNAME];
//
//			//get the book from the site
//			book = site.Books[bookName];
//
//			//return the book
//			return book;
//			//to determine if we need to refresh the book stored on the session
//			bool refreshSession = false;
//
//			//initialize to null
//			IBook retBook = null;
//
//			//get the current site
//			ISite site = GetCurrentSite(page);
//
//			//don't bother trying to return anything if there are no accessible books on the site
//			if(site.Books.Count > 0)
//			{
//				//try to grab the current book name from the request
//				string currBookName = page.Request.Params[REQPARAM_CURRENTBOOKNAME];
//            
//				//if it is there...
//				if(currBookName != EMPTY_STRING && currBookName != null)
//				{
//					//get the actual book from the book name
//					retBook = site.Books[currBookName];
//
//					//be sure to update the session with the new book
//					refreshSession = true;
//				}
//				else
//				{
//					//try to grab the current book from the session
//					retBook = (IBook)page.Session[SESSPARAM_CURRENTBOOK];
//
//					//if it's not there...
//					if(retBook == null)
//					{
//						//...just grab the first book on the site
//						retBook = site.Books[0];
//
//						//be sure to update the session with the new book
//						refreshSession = true;		
//					}
//				}
//
//				//only refresh our session if we need to
//				if(refreshSession)
//				{
//					//store the book in our session
//					page.Session[SESSPARAM_CURRENTBOOK] = retBook;
//				}
//			}
//
//			//pass back the book
//			return retBook;
//		}
        /// <summary>
        /// Retrieves the current document from the url and stores on the session
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static IDocument GetCurrentDocument(Page page)
        {
            //this is the document we will return
            IDocument doc = null;

            //grab the current doc name from the request
            string docName = page.Request.Params[REQPARAM_CURRENTDOCUMENTNAME];

            //grab the current book name from the request
            string bookName = page.Request.Params[REQPARAM_CURRENTBOOKNAME];

            //get the doc from the session
            var sessionDoc = (IDocument) page.Session[SESSPARAM_CURRENTDOCUMENT];

            //if the request string is not there or incomplete, or if the requested doc matches the one already in the session, grab the doc from the session
            if (((docName == string.Empty || docName == null) || (bookName == string.Empty || bookName == null)) ||
                (sessionDoc != null && (sessionDoc.Book.Name == bookName && sessionDoc.Name == docName)))
            {
                doc = sessionDoc;
            }
            else //otherwise...
            {
                ISite site = null;
                IBook book = null;

                //get the current site
                site = GetCurrentSite(page);
                if (site != null)
                {
                    //now grab the book from the site
                    book = site.Books[bookName];
                    if (book != null)
                    {
                        //now grab the doc from the book
                        doc = book.Documents[docName];

                        //store the document on the session
                        page.Session[SESSPARAM_CURRENTDOCUMENT] = doc;
                    }
                    else
                    {
                        //the book did not exist...so instantiate a site with no user and see if the book exists there
                        ISite allSite = new Site(site.Id);
                        book = allSite.Books[bookName];
                        if (book != null)
                        {
                            doc = book.Documents[docName];
                            if (doc != null)
                            {
                                doc.InSubscription = false;
                            }
                        }
                    }
                }

                //if we get this far without getting a book, then the url has specified an invalid book
                if (book == null)
                {
                    throw new Exception("The specified book is invalid.");
                }

                //if we get this far without getting a doc, then the url has specified an invalid book
                if (doc == null)
                {
                    throw new Exception("The specified document is invalid.");
                }
            }

            //return the doc
            return doc;
        }

        /// <summary>
        /// Retrieves the current document format from the url
        /// </summary>
        /// <param name="page"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IDocumentFormat GetCurrentDocumentFormat(Page page)
        {
            //initialize to null
            IDocumentFormat retDocFormat = null;

            //get the current document
            IDocument doc = GetCurrentDocument(page);

            //try to grab the current doc format id from the request
            string currDocFormatId = page.Request.Params[REQPARAM_CURRENTDOCUMENTFORMATID];

            if (currDocFormatId != EMPTY_STRING && currDocFormatId != null)
            {
                //get the actual document format object and pass it back
                var contentType = (ContentType) int.Parse(currDocFormatId);
                retDocFormat = doc.Formats[contentType];
            }

            //return the format
            return retDocFormat;
        }

        /// <summary>
        /// Retrieves the current document anchor from the url or, if not found, the session
        /// </summary>
        /// <param name="page"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IDocumentAnchor GetCurrentDocumentAnchor(Page page)
        {
            //this is the document anchor we will return
            IDocumentAnchor docAnchor = null;

            //get the current document
            IDocument doc = GetCurrentDocument(page);

            if (doc != null)
            {
                //grab the current docanchor name from the request
                string docAnchorName = page.Request.Params[REQPARAM_CURRENTDOCUMENTANCHORNAME];

                if (docAnchorName != null && docAnchorName != string.Empty)
                {
                    //get the docanchor from the doc
                    docAnchor = doc.GetDocumentAnchor(docAnchorName);
                }
                //store the document anchor on the session
                page.Session[SESSPARAM_CURRENTDOCUMENTANCHOR] = docAnchor;
            }
            //return the docanchor
            return docAnchor;
        }

        /// <summary>
        /// Retrieves the parameter for the document tabs see enum doctab above.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>DocTab</returns>
        public static DocTab GetDocumentTab(Page page)
        {
            //grab tab specified in the querysting from the request
            //public const string REQPARAM_TARGETTAB = "targettab";
            string tabIndex = page.Request.Params[REQPARAM_TARGETTAB];
            if ((tabIndex != null) && (tabIndex != string.Empty))
            {
                object retObj = Enum.Parse(typeof (DocTab), tabIndex);

                if (retObj != null)
                {
                    return (DocTab) retObj;
                }
            }

            return DocTab.Document;
        }

        ///<summary>
        ///Retrieves the target command parameter from request quesry string and returns an enum
        ///</summary>
        ///<param name="page"></param>
        ///<returns>DocCmd</returns>
        public static DocCmd GetDocumentCommand(Page page)
        {
            //grab command specified in the querysting from the request
            //public const string REQPARAM_TARGETCMD = "targetcmd";
            string cmdIndex = page.Request.Params[REQPARAM_TARGETCMD];
            if ((cmdIndex != null) && (cmdIndex != string.Empty))
            {
                object retObj = Enum.Parse(typeof (DocCmd), cmdIndex);

                if (retObj != null)
                {
                    return (DocCmd) retObj;
                }
            }
            return DocCmd.DisplayDocument;
        }


        /// <summary>
        /// Retrieves the current toc sync path from the url or, if not found, the session
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetTocSyncPath(Page page)
        {
            //initialize to empty
            string tocSyncPath = EMPTY_STRING;

            //try to grab from the request
            tocSyncPath = page.Request.Params[REQPARAM_TOCSYNCPATH];

            //if it is not there, go to the session
            if (tocSyncPath == EMPTY_STRING || tocSyncPath == null)
            {
                tocSyncPath = (string) page.Session[REQPARAM_TOCSYNCPATH];
            }
            else
            {
                //store in our session
                page.Session[REQPARAM_TOCSYNCPATH] = tocSyncPath;

                //clear the request by redirecting to ourself without the query param
                var strikeParams = new ArrayList();
                strikeParams.Add(REQPARAM_TOCSYNCPATH);
                StrikeQueryStringParam(page, strikeParams);
            }

            //pass it back
            return tocSyncPath;
        }

        /// <summary>
        /// Performs a redirect in order to remove a query string param
        /// </summary>
        /// <param name="strikeKey">The name of the query string param to remove</param>
        public static void StrikeQueryStringParam(Page page, ArrayList strikeKeys)
        {
            //clear the request by redirecting to ourself without the query param
            string requestQuery = page.Request.RawUrl;
            string newUrl = requestQuery.Substring(0, requestQuery.IndexOf("?"));
            int count = 0;
            foreach (string key in page.Request.QueryString)
            {
                string urlParamSepChar = "?";
                if ((count++) > 0)
                {
                    urlParamSepChar = "&";
                }
                if (!strikeKeys.Contains(key))
                {
                    newUrl += urlParamSepChar + key + "=" + page.Request.QueryString[key];
                }
            }
            page.Response.Redirect(newUrl);
        }

        /// <summary>
        /// Create a toc sync path from the given reference xml
        /// </summary>
        /// <param name="referenceXml">The reference xml</param>
        /// <param name="nodeCount">The number of nodes to render</param>
        /// <returns></returns>
        public static string GetTocSyncPathFromXml(string referenceXml, int nodeCount)
        {
            string tocSyncPath = EMPTY_STRING;

            var tocSyncPathXml = new XmlDocument();
            tocSyncPathXml.LoadXml(referenceXml);

            XmlNodeList nodes = tocSyncPathXml.SelectNodes("/*/*");
            for (int i = 0; i < nodes.Count && i < nodeCount; i++)
            {
                XmlNode node = nodes[i];
                string nodeId = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_ID]);
                string nodeTypeDesc = node.LocalName;

                tocSyncPath += nodeTypeDesc + RADTREE_VALUESEPCHAR + nodeId;

                if (i != nodes.Count - 1 && i != nodeCount - 1)
                {
                    tocSyncPath += RADTREE_TOCPATHSEPCHAR;
                }
            }
            return tocSyncPath;
        }

        /// <summary>
        /// Returns a document's id path for use in synching the table of contents
        /// </summary>
        /// <param name="doc">The document for which you wish to retrieve a path</param>
        /// <returns>A string representing the document's toc sync path, represented in the form of NodeTypeDesc1~NodeId1/NodeTypeDesc2~NodeId2/NodeTypeDescn~NodeIdn</returns>
        public static string GetTocPath(IDocument doc)
        {
            string docTocPath = EMPTY_STRING;

            if (doc != null)
            {
                //get the site reference xml from the document and load it into an xml doc
                string docSiteRefXml = doc.SiteReferencePath;
                var docSiteRefXmlDoc = new XmlDocument();
                docSiteRefXmlDoc.LoadXml(docSiteRefXml);

                //now loop thru the reference nodes and build a path
                XmlNodeList nodes = docSiteRefXmlDoc.SelectNodes("/*/*");
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];
                    string id = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_ID]);
                    string nodeTypeDesc = node.LocalName;
                    string pathSep = EMPTY_STRING;
                    if (i != nodes.Count - 1)
                    {
                        pathSep += RADTREE_TOCPATHSEPCHAR;
                    }
                    docTocPath += nodeTypeDesc + RADTREE_VALUESEPCHAR + id + pathSep;
                }
            }
            return docTocPath;
        }

        /// <summary>
        /// Returns a document anchor's id path for use in synching the table of contents
        /// </summary>
        /// <param name="docAnchor">The document anchor for which you wish to retrieve a path</param>
        /// <returns>A string representing the document anchor's toc sync path, represented in the form of NodeTypeDesc1~NodeId1/NodeTypeDesc2~NodeId2/NodeTypeDescn~NodeIdn</returns>
        public static string GetTocPath(IDocumentAnchor docAnchor)
        {
            string docAnchorTocPath = EMPTY_STRING;

            if (docAnchor != null)
            {
                //get the site reference xml from the document anchor and load it into an xml doc
                string docAnchorSiteRefXml = docAnchor.SiteReferencePath;
                var docAnchorSiteRefXmlDoc = new XmlDocument();
                docAnchorSiteRefXmlDoc.LoadXml(docAnchorSiteRefXml);

                //now loop thru the reference nodes and build a path
                XmlNodeList nodes = docAnchorSiteRefXmlDoc.SelectNodes("/*/*");
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];
                    string id = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_ID]);
                    string nodeTypeDesc = node.LocalName;
                    string pathSep = EMPTY_STRING;
                    if (i != nodes.Count - 1)
                    {
                        pathSep += RADTREE_TOCPATHSEPCHAR;
                    }
                    docAnchorTocPath += nodeTypeDesc + RADTREE_VALUESEPCHAR + id + pathSep;
                }
            }
            return docAnchorTocPath;
        }

        /// <summary>
        /// Returns a site folder's id path for use in synching the table of contents
        /// </summary>
        /// <param name="siteFolder">The site folder for which you wish to retrieve a path</param>
        /// <returns>A string representing the site folder's toc sync path, represented in the form of NodeTypeDesc1~NodeId1/NodeTypeDesc2~NodeId2/NodeTypeDescn~NodeIdn</returns>
        public static string GetTocPath(ISiteFolder siteFolder)
        {
            string siteFolderTocPath = EMPTY_STRING;

            if (siteFolder != null)
            {
                //get the site reference xml from the site folder and load it into an xml doc
                string siteFolderSiteRefXml = siteFolder.SiteReferencePath;
                var siteFolderSiteRefXmlDoc = new XmlDocument();
                siteFolderSiteRefXmlDoc.LoadXml(siteFolderSiteRefXml);

                //now loop thru the reference nodes and build a path
                XmlNodeList nodes = siteFolderSiteRefXmlDoc.SelectNodes("/*/*");
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes[i];
                    string id = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_ID]);
                    string nodeTypeDesc = node.LocalName;
                    string pathSep = EMPTY_STRING;
                    if (i != nodes.Count - 1)
                    {
                        pathSep += RADTREE_TOCPATHSEPCHAR;
                    }
                    siteFolderTocPath += nodeTypeDesc + RADTREE_VALUESEPCHAR + id + pathSep;
                }
            }
            return siteFolderTocPath;
        }

        /// <summary>
        /// Method that returns an alternate or fallback book name for a specified book name. This is how the application handles link resolution for duplicate books in the toc.
        /// </summary>
        /// <param name="books">A book collection to search for alternate books</param>
        /// <param name="bookName">The name of the book for which an alternate is sought</param>
        /// <returns>Returns a string or null if no alternate book was available.</returns>
        public static string GetAlternateBookName(IBookCollection books, string bookName)
        {
            //the book name to return
            string retBookName = string.Empty;

            if (books.Count > 0)
            {
                var alternateBooks = new ArrayList();
                foreach (IBook book in books)
                {
                    if (book.Name.StartsWith(bookName + ALTERNATEBOOK_SUFFIX))
                    {
                        alternateBooks.Add(book.Name);
                    }
                }

                if (alternateBooks.Count > 0)
                {
                    //sort in alpha order
                    alternateBooks.Sort();

                    //set our return book, the first book by alpha order
                    retBookName = (string) alternateBooks[0];
                }
            }
            //logic added to resolve ara-links to aam if the user has access to aam but not aras
            //also logic to make ras a fallback if the currentbook is 
            if (retBookName == string.Empty)
            {
                if (bookName.StartsWith("ara-"))
                {
                    foreach (IBook book in books)
                    {
                        if (book.Name == "aam")
                        {
                            retBookName = "aam";
                            break;
                        }
                    }
                }
                if (bookName == "ps")
                {
                    foreach (IBook book in books)
                    {
                        if (book.Name == "ras")
                        {
                            retBookName = "ras";
                            break;
                        }
                    }
                }
            }

            //return the book name
            return retBookName;
        }

        /// <summary>
        /// Displays the help tab with the specified topic
        /// </summary>
        /// <param name="page"></param>
        /// <param name="helpTopic">String representing the help topic to display</param>
        public static void ShowHelp(Page page, HelpTopic helpTopic)
        {
            ShowTab(page, PortalTab.Help, "&" + REQPARAM_HELPTOPIC + "=" + helpTopic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portalTab"></param>
        /// <param name="reqExtra"></param>
        public static void ShowTab(Page page, PortalTab portalTab, string reqExtra)
        {
            var tabId = (int) portalTab;
            int tabIndex = 0;

            switch (tabId)
            {
                case 1:
                    tabIndex = 0;
                    break;
                case 2:
                    tabIndex = 1;
                    break;
                case 3:
                    tabIndex = 1;
                    break;
                case 4:
                    tabIndex = 2;
                    break;
                case 5:
                    tabIndex = 3;
                    break;
                case 98:
                    tabIndex = 4;
                    break;
            }

            //page.Response.Redirect("~/DesktopDefault.aspx?tabindex=4&tabid=" + (int)portalTab + "&" + reqExtra);
            page.Response.Redirect("~/DesktopDefault.aspx?tabindex=" + tabIndex + "&tabid=" + tabId + "&" + reqExtra);
        }
    }
}