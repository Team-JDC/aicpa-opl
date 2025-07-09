#region

using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;
using System.Text.RegularExpressions;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for Site.
    /// </summary>
    public class Site : DestroyerBpc, ISite
    {
        #region Constants

        //errors
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_NODETYPEINVALIDCONTEXT = "Node type not valid in this context ('{0}', '{1}')";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_SITERETRIEVALFAILURE =
            "This site is not currently available, we apologize for the inconvenience, please try again later.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_SITEMAKEFILEINVALIDXML =
            "There was a problem loading the makefile XML for site '{1}'. Make sure that the makefile is well formed XML.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_DUPLICATESITEBOOK = "The book '{0}' already exists within this site.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_SITEBOOKADDSITENOTSAVED =
            "Error adding book to site. The site has pending changes and should be saved before adding books to it.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_SITEBOOKADDBOOKNOTSAVED =
            "Error adding book to site. The book has pending changes and should be saved before adding it to a site.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_BOOKNOTFOUND = "The book '{0}' does not exists within this site.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_INVALIDBOOKINTOCXML =
            "The book with id '{0}' was found in the site makefile XML, but this book is not currently associated with the site. You must add the book to the site before adding it to the site's table of contents.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_INVALIDSITESTATUSFORRETRIEVAL =
            "You cannot construct a site by specifying the site status of '{0}'.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_INVALIDSTATUSCHANGE = "You may not set the site status from '{0}' to '{1}'.";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_INVALIDSITEXML = "The site XML is invalid: {0}";
        //public static string ERROR_INVALIDSTATECHANGENOBOOKS = "You may not promote a site to staging or production without first adding books to the site.";
        //public static string ERROR_INVALIDSTATECHANGENOTOC = "The table of contents for this site is missing or corrupted. Please rebuild the site before promoting to staging or production.";

        //site specific constants
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static int SITE_INITIALVERSION;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_INITIALSITETEMPLATEXML = "<" + XML_ELE_MAKEFILE + ">" + "<" + XML_ELE_SITE + "/>" +
                                                           "</" + XML_ELE_MAKEFILE + ">";

        //config keys
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_ARCHIVE_BOOK_ID_LIST_KEY = "Site_Archive_Book_Id_List";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_REFERENCING_LINKS_BOOK_ID_LIST_KEY = "Site_Referencing_Links_Book_Id_List";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_XREF_BOOK_ID_LIST_KEY = "Site_XRef_Book_Id_List";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_JSECTION_BOOK_ID_LIST_KEY = "Site_JSection_Book_Id_List";

        //config values
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_ARCHIVE_BOOK_ID_LIST =
            ConfigurationSettings.AppSettings[SITE_ARCHIVE_BOOK_ID_LIST_KEY];

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_REFERENCING_LINKS_BOOK_ID_LIST =
            ConfigurationSettings.AppSettings[SITE_REFERENCING_LINKS_BOOK_ID_LIST_KEY];

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_XREF_BOOK_ID_LIST = ConfigurationSettings.AppSettings[SITE_XREF_BOOK_ID_LIST_KEY];

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string SITE_JSECTION_BOOK_ID_LIST =
            ConfigurationSettings.AppSettings[SITE_JSECTION_BOOK_ID_LIST_KEY];

        #endregion

        #region Private

        #region Private Fields

        //the following fields are used for construction only, not for property storage
        private static readonly Hashtable tocXmlHash = new Hashtable();
        private readonly User.User activeUser;
        private readonly int siteId = -1;
        private readonly string siteName;
        private readonly SiteStatus siteStatus = SiteStatus.Null;
        private readonly int siteVersion = -1;
        private IBookCollection activeBookCollection;
        //the following fields are used for property storage
        private IBookDalc activeBookDalc;
        private string activeSiteBookXml;
        private ISiteDalc activeSiteDalc;
        private SiteDs activeSiteDs;
        private ISiteIndex activeSiteIndex;
        private SiteDs.SiteRow activeSiteRow;
        private ISiteTocNodeCollection activeSiteTocNodeCollection;

        #endregion

        #region Private Accessors

        /// <summary>
        ///   This is the processed version of the SiteXml (SiteXml template has been processed and post-build information has been added)
        /// </summary>
        private string activeSiteXml;

        /// <summary>
        ///   Private accessor for retrieving the active Site DALC
        /// </summary>
        private ISiteDalc ActiveSiteDalc
        {
            get { return activeSiteDalc ?? (activeSiteDalc = new SiteDalc()); }
        }

        /// <summary>
        ///   Private accessor for retrieving the active Book DALC
        /// </summary>
        private IBookDalc ActiveBookDalc
        {
            get { return activeBookDalc ?? (activeBookDalc = new BookDalc()); }
        }

        /// <summary>
        ///   Private accessor for retrieving the active Site dataset
        /// </summary>
        private SiteDs ActiveSiteDs
        {
            get
            {
                if (activeSiteDs == null)
                {
                    //if the id is present, try retrieval by id
                    if (siteId >= 0)
                    {
                        activeSiteDs = ActiveSiteDalc.GetSite(siteId);
                    }
                    else
                    {
                        //if the site status is present, try retrieval by this field
                        if (siteStatus != SiteStatus.Null)
                        {
                            activeSiteDs = ActiveSiteDalc.GetSite(siteStatus);
                        }
                        else
                        {
                            //if the site name and site version are present, try retrieval by these fields
                            if (siteName != null && siteVersion >= 0)
                            {
                                activeSiteDs = ActiveSiteDalc.GetSite(siteName, siteVersion);
                            }
                            else
                            {
                                //if the site name is present, try retrieval by it
                                if (siteName != null)
                                {
                                    activeSiteDs = ActiveSiteDalc.GetSite(siteName);
                                }
                                else
                                {
                                    //otherwise we should just create  a new empty site dataset with an empty site row
                                    activeSiteDs = new SiteDs();
                                    activeSiteRow = activeSiteDs.Site.AddSiteRow(SITE_INITIALVERSION, EMPTY_STRING,
                                                                                 EMPTY_STRING, (int) SiteStatus.Null,
                                                                                 (int) SiteStatus.Unassigned,
                                                                                 EMPTY_STRING, EMPTY_BOOL, EMPTY_STRING,
                                                                                 EMPTY_BOOL,
                                                                                 (int) SiteBuildStatus.NotBuilt,
                                                                                 (int) SiteIndexBuildStatus.NotBuilt,
                                                                                 SITE_INITIALSITETEMPLATEXML);
                                }
                            }
                        }
                    }
                }
                if (activeSiteDs.Site.Rows.Count == 0)
                {
                    throw new Exception(ERROR_SITERETRIEVALFAILURE);
                }
                return activeSiteDs;
            }
        }

        /// <summary>
        ///   Private accessor for retrieving the active SiteRow
        /// </summary>
        private SiteDs.SiteRow ActiveSiteRow
        {
            get
            {
                if (activeSiteRow == null)
                {
                    if (ActiveSiteDs.Site.Rows.Count > 0)
                    {
                        activeSiteRow = (SiteDs.SiteRow) ActiveSiteDs.Site.Rows[0];
                    }
                }
                return activeSiteRow;
            }
        }

        /// <summary>
        ///   Private accessor for retrieving the active SiteIndex
        /// </summary>
        private ISiteIndex ActiveSiteIndex
        {
            get { return activeSiteIndex ?? (activeSiteIndex = new SiteIndex(this)); }
        }

        /// <summary>
        ///   Private accessor for retrieving the active User
        /// </summary>
        private User.User ActiveUser
        {
            get
            {
                if (activeUser == null)
                {
                    //TODO: Is there some sort of proxy user we would like to use if no user has been provided? If left like this,
                    // with no user associated with the site, access to all content is implied. 
                }
                return activeUser;
            }
        }

        /// <summary>
        ///   For retrieving books associated with this site.
        /// </summary>
        private IBookCollection ActiveBookCollection
        {
            get {
                return activeBookCollection ??
                       (activeBookCollection =
                        ActiveUser == null
                            ? new BookCollection(this)
                            : new BookCollection(this, ActiveUser.UserSecurity.BookName));
            }
        }

        /// <summary>
        ///   For retrieving a collection of site table of contents nodes associated with this site
        /// </summary>
        private ISiteTocNodeCollection ActiveSiteTocNodeCollection
        {
            get { return activeSiteTocNodeCollection ?? (activeSiteTocNodeCollection = new SiteTocNodeCollection(this)); }
        }

        /// <summary>
        ///   For retrieving xml that describes the site.
        /// </summary>
        private string ActiveSiteTemplateXml
        {
            get { return ActiveSiteRow.SiteTemplateXml; }
            set { ActiveSiteRow.SiteTemplateXml = value; }
        }

        private string ActiveSiteXml
        {
            get
            {
                if (BuildStatus == SiteBuildStatus.Built && activeSiteXml == null)
                {
                    activeSiteXml = GetXmlFromTocNodes(ActiveSiteTocNodeCollection, true);
                    if (ActiveUser != null)
                    {
                        activeSiteXml = FilterTocXml(activeSiteXml, ActiveUser.UserSecurity.BookName, true);
                    }
                }
                else if(BuildStatus != SiteBuildStatus.Built)
                {
                    //if the site has not yet been built, just return the resolved version of the site template
                    activeSiteXml = ProcessSiteTemplateXml(SiteTemplateXml);
                }
                return activeSiteXml;
            }
        }

        /// <summary>
        ///   XML string describing the structure of the site and all books contained in the site.
        /// </summary>
        private string ActiveSiteBookXml
        {
            get
            {
                if (activeSiteBookXml == null)
                {
                    //put the book xml into the site xml
                    XmlDocument siteXml = new XmlDocument();
                    siteXml.LoadXml(SiteXml);
                    XmlNodeList nodes = siteXml.SelectNodes("//" + XML_ELE_BOOK);
                    foreach (XmlNode node in nodes)
                    {
                        string bookName = GetAttributeValue(node.Attributes[XML_ATT_BOOKNAME]);
                        IBook currentBook = ActiveBookCollection[bookName];

                        //don't try to load the xml if it does not exist! (book has not yet been built)
                        if (currentBook == null) continue;
                        //Log.LogEvent(EventType.Info, 1, "Site", "ActiveSiteBookXml", "Debug", string.Format("1. Current Book:{0}", currentBook.Name));
                        string bookXmlStr = currentBook.BookXml;
                        //Log.LogEvent(EventType.Info, 1, "Site", "ActiveSiteBookXml", "Debug", string.Format("2. Current Book:{0} XML:{1}", currentBook.Name, bookXmlStr));
                        
                        //we load the book into a dom object in order to avoid getting the root element
                        XmlDocument bookXml = new XmlDocument();
                        bookXml.LoadXml(bookXmlStr);

                        //add the book xml into our site xml
                        node.InnerXml = bookXml.DocumentElement.InnerXml;
                    }
                    activeSiteBookXml = siteXml.OuterXml;
                }
                return activeSiteBookXml;
            }
        }

        /// <summary>
        ///   Retrieve or set the build status of a site index.
        ///   A setter is available if, on the presentation level, you wish to add a request for a build but handle the build later in an offline process.
        ///   When this technique is used, the client is required to call Build() on all flagged site indexes, as a build will not automatically take place.
        /// </summary>
        private SiteIndexBuildStatus ActiveIndexBuildStatus
        {
            get { return (SiteIndexBuildStatus) ActiveSiteRow.IndexBuildStatusCode; }
            set { ActiveSiteRow.IndexBuildStatusCode = (int) value; }
        }

        #region Private Methods

        /// <summary>
        ///   Determines whether or not the site xml is valid.
        /// </summary>
        /// <param name = "siteXml">The site xml</param>
        /// <returns>True if the xml is valid, false if not</returns>
        private bool IsSiteXmlValid(string siteXml)
        {
            //for now, just check for well-formedness
            bool retVal = false;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(siteXml);
                retVal = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return retVal;
        }

        /// <summary>
        ///   Filters out nodes that are not part of the specified book list and nodes that are hidden
        /// </summary>
        /// <param name = "tocXml">The toc XML to be filtered.</param>
        /// <param name = "bookList">The set of books that are to remain in the returned XML</param>
        /// <param name = "filterHidden">Determines whether or not hidden nodes should be filtered out.</param>
        /// <returns>An XML string that matches the provided tocXml, but filtered to contain only books within bookList.</returns>
        private string FilterTocXml(string tocXml, string[] bookList, bool filterHidden)
        {
            //the xml to return
            string retXml = "";

            //load the xml into a dom object
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(tocXml);

            //create an xpath expression that will snag all book nodes that fall outside of our book list
            string filterXpath = "//" + XML_ELE_BOOK;
            for (int i = 0; i < bookList.Length; i++)
            {
                if (i == 0)
                {
                    filterXpath += "[";
                }
                filterXpath += "@" + XML_ATT_BOOKNAME + "!='" + bookList[i] + "'";
                if (i != bookList.Length - 1)
                {
                    filterXpath += " and ";
                }
                else
                {
                    filterXpath += "]";
                }
            }

            //now remove those nodes
            XmlNodeList deletionNodes = xmlDoc.SelectNodes(filterXpath);
            foreach (XmlNode deletionNode in deletionNodes)
            {
                deletionNode.ParentNode.RemoveChild(deletionNode);
            }

            if (filterHidden)
            {
                //select all hidden nodes and delete them
                XmlNodeList hiddenNodes = xmlDoc.SelectNodes("*/*[@" + XML_ATT_HIDDEN + "='" + bool.TrueString + "']");
                foreach (XmlNode hiddenNode in hiddenNodes)
                {
                    hiddenNode.ParentNode.RemoveChild(hiddenNode);
                }
            }

            //finally, remove all SiteFolders that do not hold subscription books
            XmlNodeList siteFolderNodes = xmlDoc.SelectNodes("*/" + XML_ELE_SITEFOLDER);
            foreach (XmlNode siteFolderNode in
                siteFolderNodes.Cast<XmlNode>().Where(siteFolderNode => !ContainsSubscriptionBooks(siteFolderNode, bookList)))
            {
                siteFolderNode.ParentNode.RemoveChild(siteFolderNode);
            }

            //and return our filtered XML
            retXml = xmlDoc.OuterXml;

            //cleanup amersands that are sometimes corrupted in the xml
            retXml = retXml.Replace("&amp;amp;", "&amp;");
            return retXml;
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteFolderNode"></param>
        /// <param name = "bookList"></param>
        /// <returns></returns>
        private bool ContainsSubscriptionBooks(XmlNode siteFolderNode, string[] bookList)
        {
            //make sure we are fed only SiteFolder nodes, because we don't eat anything else
            if (siteFolderNode.LocalName != NodeType.SiteFolder.ToString())
            {
                throw new Exception(string.Format(ERROR_NODETYPEINVALIDCONTEXT, siteFolderNode.LocalName,
                                                  "ContainsSubscriptionBooks"));
            }

            //our return value
            bool retVal = false;

            //if it has children, get the toc structure underneath the SiteFolder node
            int siteFolderId = int.Parse(GetAttributeValue(siteFolderNode.Attributes[XML_ATT_SITEFOLDERID]));
            bool siteFolderHasChildren = bool.Parse(GetAttributeValue(siteFolderNode.Attributes[XML_ATT_HASCHILDREN]));
            if (siteFolderHasChildren)
            {
                //get the xml from the SiteFolder node and load it into an xml document
                string siteFolderTocXml = GetTocXml(siteFolderId, NodeType.SiteFolder);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(siteFolderTocXml);

                //see if there are any books in the current folder that exist in our subscription
                XmlNodeList bookNodes = xmlDoc.SelectNodes("*/" + XML_ELE_BOOK);
                ArrayList books = new ArrayList(bookList);
                if ((from XmlNode bookNode in bookNodes select GetAttributeValue(bookNode.Attributes[XML_ATT_BOOKNAME])).Any(bookName => books.Contains(bookName)))
                {
                    retVal = true;
                }

                //if we haven't found a subscription book yet, check the SiteFolders by recursing
                if (!retVal)
                {
                    XmlNodeList subSiteFolderNodes = xmlDoc.SelectNodes("*/" + XML_ELE_SITEFOLDER);
                    foreach (XmlNode subSiteFolderNode in subSiteFolderNodes)
                    {
                        retVal = ContainsSubscriptionBooks(subSiteFolderNode, bookList);
                        if (retVal)
                        {
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///   Method for creating the site toc nodes and associated data
        /// </summary>
        /// <param name = "siteXml">An XML string representing the table of contents structure for the site.</param>
        private void BuildSiteFromXml(string siteXml)
        {
            //remove all existing books from the site
            foreach (IBook book in Books)
            {
                RemoveBook(book);
            }

            //load the makefile into a dom document
            XmlDocument makeXml = new XmlDocument();
            try
            {
                makeXml.LoadXml(siteXml);
            }
            catch (Exception e)
            {
                throw new XmlException(string.Format(ERROR_SITEMAKEFILEINVALIDXML, Name), e);
            }

            //add all books in the makefile to the site
            XmlNodeList bookNodes = makeXml.SelectNodes("//" + XML_ELE_BOOK);
            foreach (Book.Book book in from XmlNode bookNode in bookNodes
                                       select int.Parse(GetAttributeValue(bookNode.Attributes[XML_ATT_BOOKID]))
                                       into bookId select new Book.Book(bookId))
            {
                AddBook(book);
            }

            //process all makefile elements
            XmlNodeList nodes = makeXml.SelectNodes("//*");

            //create a new site folder dataset
            SiteFolderDs siteFolderDs = new SiteFolderDs();

            //create a new site toc node dataset
            SiteTocNodeDs siteTocNodeDs = new SiteTocNodeDs();

            //go through each node to collect hierarchy information
            foreach (XmlNode node in nodes)
            {
                DateTime dateAdded = DateTime.Now;
                string nodeName = node.LocalName;
                int tocLeft = GetNodeLeftValue(node);
                int tocRight = GetNodeRightValue(node);
                bool hasChildren = !(tocLeft == tocRight - 1);

                switch (nodeName)
                {
                    case XML_ELE_MAKEFILE:
                        break;
                    case XML_ELE_SITE:
                        //add our site's information to the site toc node dataset
                        siteTocNodeDs.SiteTocNode.AddSiteTocNodeRow(Id, Id, tocLeft, tocRight, (byte) NodeType,
                                                                    NodeType.ToString(), string.Empty, string.Empty,
                                                                    hasChildren);
                        break;
                    case XML_ELE_BOOK:
                        int bookId = int.Parse(GetAttributeValue(node.Attributes[XML_ATT_BOOKID]));

                        //add our book's information to the sitetocnode table
                        siteTocNodeDs.SiteTocNode.AddSiteTocNodeRow(Id, bookId, tocLeft, tocRight, (byte) NodeType.Book,
                                                                    NodeType.Book.ToString(), string.Empty, string.Empty,
                                                                    hasChildren);

                        break;
                    case XML_ELE_SITEFOLDER:
                        string siteFolderName = GetAttributeValue(node.Attributes[XML_ATT_SITEFOLDERNAME]);
                        string siteFolderTitle = GetAttributeValue(node.Attributes[XML_ATT_SITEFOLDERTITLE]);
                        string siteFolderUri = GetAttributeValue(node.Attributes[XML_ATT_SITEFOLDERURI]);

                        //update folders in the active bookdataset
                        siteFolderDs.SiteFolder.AddSiteFolderRow(siteFolderName, siteFolderTitle, siteFolderUri,
                                                                 string.Empty, tocLeft, tocRight, hasChildren);
                        break;
                }
            }

            //save the site folder dataset
            ActiveSiteDalc.Save(siteFolderDs);

            //populate our site toc node dataset using the site folder values returned in the book folder dataset
            foreach (SiteFolderDs.SiteFolderRow siteFolderRow in siteFolderDs.SiteFolder.Rows)
            {
                siteTocNodeDs.SiteTocNode.AddSiteTocNodeRow(Id, siteFolderRow.FolderId, siteFolderRow.Left,
                                                            siteFolderRow.Right, (byte) NodeType.SiteFolder,
                                                            NodeType.SiteFolder.ToString(), siteFolderRow.Name,
                                                            siteFolderRow.Title, siteFolderRow.HasChildren);
            }

            //save the site toc node dataset
            ActiveSiteDalc.Save(siteTocNodeDs);

            //set these private fields to null so they will be repopulated
            activeSiteTocNodeCollection = null;
            //this.activeSiteXml = null;
            activeSiteBookXml = null;
        }

        /// <summary>
        ///   Returns an integer representing the given xml node's left toc entry value
        /// </summary>
        /// <param name = "xmlNode">The node for which you wish to retrieve a left toc entry value</param>
        /// <returns>An integer representing the specified xml node's left toc entry value</returns>
        private int GetNodeLeftValue(XmlNode xmlNode)
        {
            int leftVal = 0;
            int preceding =
                xmlNode.SelectNodes("preceding::*[local-name(.)='" + XML_ELE_SITE + "' or local-name(.)='" +
                                    XML_ELE_SITEFOLDER + "' or local-name(.)='" + XML_ELE_BOOK + "']").Count;
            int ancestor =
                xmlNode.SelectNodes("ancestor::*[local-name(.)='" + XML_ELE_SITE + "' or local-name(.)='" +
                                    XML_ELE_SITEFOLDER + "' or local-name(.)='" + XML_ELE_BOOK + "']").Count - 1;
                /* -1 is for disregarding the makefile element */
            leftVal = 1 + (preceding*2) + ancestor + 1;
            return leftVal;
        }

        /// <summary>
        ///   Returns an integer representing the given xml node's right toc entry value
        /// </summary>
        /// <param name = "xmlNode">The node for which you wish to retrieve a right toc entry value</param>
        /// <returns>An integer representing the specified xml node's right toc entry value</returns>
        private int GetNodeRightValue(XmlNode xmlNode)
        {
            int rightVal = 0;
            int descendant =
                xmlNode.SelectNodes("descendant::*[local-name(.)='" + XML_ELE_SITE + "' or local-name(.)='" +
                                    XML_ELE_SITEFOLDER + "' or local-name(.)='" + XML_ELE_BOOK + "']").Count;
            int leftVal = GetNodeLeftValue(xmlNode);
            rightVal = leftVal + (descendant*2) + 1;
            return rightVal;
        }

        #endregion Private Methods

        /// <summary>
        ///   Helper method for turning a site makefile into site xml
        /// </summary>
        /// <param name = "siteTemplateXml">A string holding the site template xml</param>
        /// <returns>A string containing the site xml</returns>
        private string ProcessSiteTemplateXml(string siteTemplateXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(siteTemplateXml);

            XmlNodeList nodes = xmlDoc.SelectNodes("//*");

            foreach (XmlNode node in nodes)
            {
                string elementName = node.LocalName;
                switch (elementName)
                {
                    case XML_ELE_MAKEFILE:
                        break;
                    case XML_ELE_SITE:
                        //add the name attribute
                        XmlAttribute nameAtt = xmlDoc.CreateAttribute(XML_ATT_SITENAME);
                        nameAtt.Value = Name;
                        node.Attributes.Append(nameAtt);

                        //add the title attribute
                        XmlAttribute titleAtt = xmlDoc.CreateAttribute(XML_ATT_SITETITLE);
                        titleAtt.Value = Title;
                        node.Attributes.Append(titleAtt);

                        //add the id attribute
                        XmlAttribute idAtt = xmlDoc.CreateAttribute(XML_ATT_SITEID);
                        idAtt.Value = Id.ToString();
                        node.Attributes.Append(idAtt);

                        //add the has children attribute
                        XmlAttribute hasChildrenAtt = xmlDoc.CreateAttribute(XML_ATT_HASCHILDREN);
                        hasChildrenAtt.Value = true.ToString();
                        node.Attributes.Append(hasChildrenAtt);

                        //add the guid attribute
                        XmlAttribute guidAtt = xmlDoc.CreateAttribute(XML_ATT_GUID);
                        guidAtt.Value = Guid.NewGuid().ToString();
                        node.Attributes.Append(guidAtt);
                        break;
                    case XML_ELE_SITEFOLDER:
                        break;
                    case XML_ELE_BOOK:

                        int bookId = int.Parse(GetAttributeValue(node.Attributes[XML_ATT_BOOKID]));
                        IBook book = Books.GetBookByBookInstanceId(bookId);

                        //add the name attribute
                        XmlAttribute bookNameAtt = xmlDoc.CreateAttribute(XML_ATT_BOOKNAME);
                        bookNameAtt.Value = book.Name;
                        node.Attributes.Append(bookNameAtt);

                        //add the title attribute
                        XmlAttribute bookTitleAtt = xmlDoc.CreateAttribute(XML_ATT_BOOKTITLE);
                        bookTitleAtt.Value = book.Title;
                        node.Attributes.Append(bookTitleAtt);

                        //add the id attribute
                        XmlAttribute bookIdAtt = xmlDoc.CreateAttribute(XML_ATT_BOOKID);
                        bookIdAtt.Value = book.Id.ToString();
                        node.Attributes.Append(bookIdAtt);

                        //add the has children attribute
                        XmlAttribute bookHasChildrenAtt = xmlDoc.CreateAttribute(XML_ATT_HASCHILDREN);
                        bookHasChildrenAtt.Value = true.ToString();
                        node.Attributes.Append(bookHasChildrenAtt);

                        //add the guid attribute
                        XmlAttribute bookGuidAtt = xmlDoc.CreateAttribute(XML_ATT_GUID);
                        bookGuidAtt.Value = Guid.NewGuid().ToString();
                        node.Attributes.Append(bookGuidAtt);
                        break;
                }
            }

            return xmlDoc.DocumentElement.InnerXml;
        }

        #endregion

        #endregion

        #region Public

        #region Constructors

        /// <summary>
        ///   Retrieve a site using its id.
        ///   This constructor accepts a user object that will determine which book nodes of the site are retrievable.
        /// </summary>
        /// <param name = "user">The user for which the site is being constructed</param>
        /// <param name = "siteId">The internal ID of the site to construct</param>
        public Site(IUser user, int siteId)
        {
            this.siteId = siteId;
            activeUser = (User.User) user;
        }

        /// <summary>
        ///   Retrieve a site using its id.
        /// </summary>
        /// <param name = "siteId">The internal ID of the site to construct</param>
        public Site(int siteId) : this((IUser) null, siteId)
        {
        }

        /// <summary>
        ///   Retrieve a site using its status. The site with the given status will be returned.
        ///   This constructor accepts a user object that will determine which book nodes of the site are retrievable.
        /// </summary>
        /// <param name = "user"></param>
        /// <param name = "siteStatus"></param>
        public Site(IUser user, SiteStatus siteStatus)
        {
            if (siteStatus != SiteStatus.Staging && siteStatus != SiteStatus.PreProduction &&
                siteStatus != SiteStatus.Production)
            {
                throw new Exception(string.Format(ERROR_INVALIDSITESTATUSFORRETRIEVAL, siteStatus));
            }
            //Caused warning for ambiguous call djf 1/15/10, sitename is not passed into this structure
            //this.siteName = siteName;
            this.siteStatus = siteStatus;
            activeUser = (User.User) user;
        }

        /// <summary>
        ///   Retrieve a site using its status. The site with the given status will be returned.
        /// </summary>
        /// <param name = "siteStatus"></param>
        public Site(SiteStatus siteStatus) : this(null, siteStatus)
        {
        }

        /// <summary>
        ///   Retrieve a site using its name and version.
        ///   This constructor accepts a user object that will determine which book nodes of the site are retrievable.
        /// </summary>
        /// <param name = "user"></param>
        /// <param name = "siteName"></param>
        /// <param name = "siteVersion"></param>
        public Site(IUser user, string siteName, int siteVersion)
        {
            this.siteName = siteName;
            this.siteVersion = siteVersion;
            activeUser = (User.User) user;
        }

        /// <summary>
        ///   Retrieve a site using its name and version.
        /// </summary>
        /// <param name = "siteName"></param>
        /// <param name = "siteVersion"></param>
        public Site(string siteName, int siteVersion) : this(null, siteName, siteVersion)
        {
        }

        /// <summary>
        ///   Retrieve a site using its name. The most recent version of the site by this name will be retrieved.
        ///   This constructor accepts a user object that will determine which book nodes of the site are retrievable.
        /// </summary>
        /// <param name = "user"></param>
        /// <param name = "siteName"></param>
        public Site(IUser user, string siteName)
        {
            this.siteName = siteName;
            activeUser = (User.User) user;
        }

        /// <summary>
        ///   Retrieve a site using its name. The most recent version of the site by this name will be retrieved.
        /// </summary>
        /// <param name = "siteName"></param>
        public Site(string siteName) : this(null, siteName)
        {
        }

        /// <summary>
        ///   Creates a new empty site object. You may set the properties and save the object.
        /// </summary>
        public Site()
        {
        }

        /// <summary>
        ///   Creates a new site object with the specified properties.
        /// </summary>
        /// <param name = "siteName"></param>
        /// <param name = "siteTitle"></param>
        /// <param name = "siteDesc"></param>
        /// <param name = "siteSearchUri"></param>
        public Site(string siteName, string siteTitle, string siteDesc, string siteSearchUri)
        {
            ActiveSiteRow.Name = siteName;
            ActiveSiteRow.Title = siteTitle;
            ActiveSiteRow.Description = siteDesc;
            ActiveSiteRow.SearchUri = siteSearchUri;
        }

        #endregion Constructors

        #region Internal Constructors

        ///<summary>
        ///  Creates a new Site object using a datarow provided by a SiteCollection.
        ///  Note that the access visibility is "internal."  This constructor is intended for
        ///  use only by SiteCollection; users of this API can't use this constructor 
        ///  themselves.  Note also that this object's dataset will be a reference to the
        ///  dataset in the SiteCollection that spawned it - it doesn't "belong" to this Site
        ///  object directly in this case.
        ///</summary>
        ///<param name = "siteRow"></param>
        internal Site(SiteDs.SiteRow siteRow)
        {
            activeSiteRow = siteRow;
            activeSiteDs = (SiteDs) activeSiteRow.Table.DataSet;
        }

        #endregion Internal Constructors

        #region Properties

        /// <summary>
        ///   The current user object. Contains null if the site was created outside of the context of a user.
        /// </summary>
        public IUser User
        {
            get { return ActiveUser; }
        }

        /// <summary>
        ///   The internal id of a site. This value is generated by the system and is read-only.
        /// </summary>
        public int Id
        {
            get { return ActiveSiteRow.SiteId; }
        }

        /// <summary>
        ///   The version of the site. This value is generated by the system and is read-only.
        /// </summary>
        public int Version
        {
            get { return ActiveSiteRow.SiteVersion; }
        }

        /// <summary>
        ///   The name of the site.
        /// </summary>
        public string Name
        {
            get { return ActiveSiteRow.Name; }
            set { ActiveSiteRow.Name = value; }
        }

        /// <summary>
        ///   The descriptive title of the site.
        /// </summary>
        public string Title
        {
            get { return ActiveSiteRow.Title; }
            set { ActiveSiteRow.Title = value; }
        }

        /// <summary>
        ///   The books associated with the site. The books contained in this collection are represented in the order they appear in the table of contents.
        /// </summary>
        public IBookCollection Books
        {
            get { return ActiveBookCollection; }
            set { activeBookCollection = value; }
        }

        /// <summary>
        ///   The current status of the site.
        /// </summary>
        public SiteStatus Status
        {
            get { return (SiteStatus) ActiveSiteRow.SiteStatusCode; }
            set
            {
                ActiveSiteRow.SiteStatusCode = (int) value;
                //jjs 11/22/06: Why did we ever do this? This line causes
                //				the requested site status to become invalid
                //				whenever we access the value of the site status
                //this.ActiveSiteRow.RequestedSiteStatusCode = (int)SiteStatus.Null;
            }
//
//
//
//
//				//this condition would indicate that no books have been added to the site
//				if(this.ActiveBookCollection.Count == 0 && (value == SiteStatus.Staging || value == SiteStatus.Production))
//				{
//					throw new Exception(string.Format(ERROR_INVALIDSTATECHANGENOBOOKS));
//				}
//
//				//this condition would indicate that we have a missing or corrupted table of contents
//				if(this.ActiveSiteTocNodeCollection.Count <= 1 && (value == SiteStatus.Staging || value == SiteStatus.Production))
//				{
//					throw new Exception(string.Format(ERROR_INVALIDSTATECHANGENOTOC));
//				}
//
//				//reset the book collection because certain fields (iseditable is one) will need to be refetched
//				this.activeBookCollection = null;
//
//				//set the new status
//				this.ActiveSiteRow.SiteStatusCode = (int)value;
//			}
        }

        /// <summary>
        ///   The requested status of the site.
        /// </summary>
        public SiteStatus RequestedStatus
        {
            get { return (SiteStatus) ActiveSiteRow.RequestedSiteStatusCode; }
            set
            {
                ActiveSiteRow.RequestedSiteStatusCode = (int) value;
//				//the requested site status
//				SiteStatus requestedBuildStatus = (SiteStatus)value;
//
//				//unassigned -> staging
//				if(this.Status == SiteStatus.Unassigned && requestedBuildStatus == SiteStatus.Staging)
//				{
//					//make sure we flag the site and the index to be built
//					this.BuildStatus = SiteBuildStatus.BuildRequested;
//					this.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
//					this.ActiveSiteRow.RequestedSiteStatusCode = (int)requestedBuildStatus;
//				} 
//				//staging - > production
//				else if(this.Status == SiteStatus.Staging && requestedBuildStatus == SiteStatus.Production)
//				{
//					//make sure we flag the site and the index to be built
//					this.BuildStatus = SiteBuildStatus.BuildRequested;
//					this.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
//					this.ActiveSiteRow.RequestedSiteStatusCode = (int)requestedBuildStatus;
//				}
//				//staging -> unassigned
//				else if(this.Status == SiteStatus.Staging && requestedBuildStatus == SiteStatus.Unassigned)
//				{
//					this.ActiveSiteRow.RequestedSiteStatusCode = (int)requestedBuildStatus;
//				}
//				//production -> unassigned
//				else if(this.Status == SiteStatus.Production && requestedBuildStatus == SiteStatus.Unassigned)
//				{
//					this.ActiveSiteRow.RequestedSiteStatusCode = (int)requestedBuildStatus;
//				}
//				else
//				{
//					throw new Exception(string.Format(ERROR_INVALIDSTATUSCHANGE, this.Status, requestedBuildStatus));
//				}
            }
        }

        /// <summary>
        ///   The online state of the site.
        /// </summary>
        public bool Online
        {
            get { return ActiveSiteRow.Online; }
            set { ActiveSiteRow.Online = value; }
        }

        /// <summary>
        ///   The site description. This is a paragraph-length description of the site and its contents.
        /// </summary>
        public string Description
        {
            get { return ActiveSiteRow.Description; }
            set { ActiveSiteRow.Description = value; }
        }

        /// <summary>
        ///   The base URI to use for searching the site
        /// </summary>
        public string SearchUri
        {
            get { return ActiveSiteRow.SearchUri; }
            set { ActiveSiteRow.SearchUri = value; }
        }

        /// <summary>
        /// </summary>
        public bool Archived
        {
            get { return ActiveSiteRow.Archived; }
            set { ActiveSiteRow.Archived = value; }
        }

        /// <summary>
        ///   Indicates build state for the site
        /// </summary>
        public SiteBuildStatus BuildStatus
        {
            get { return (SiteBuildStatus) ActiveSiteRow.BuildStatusCode; }
            set { ActiveSiteRow.BuildStatusCode = (int) value; }
        }

        /// <summary>
        ///   Indicates build state for the site's index
        /// </summary>
        public SiteIndexBuildStatus IndexBuildStatus
        {
            get { return ActiveIndexBuildStatus; }
            set { ActiveIndexBuildStatus = value; }
        }

        /// <summary>
        ///   The the index associated with the site for providing indexing and search functionality.
        /// </summary>
        public ISiteIndex SiteIndex
        {
            get { return ActiveSiteIndex; }
        }

        /// <summary>
        ///   Xml string representing a template that describes the structure of the site.
        /// </summary>
        public string SiteTemplateXml
        {
            get { return ActiveSiteTemplateXml; }
            set
            {
                string siteXml = value;
                
                //remove characters that will make the xml invalid before we save it.
                siteXml = siteXml.Replace("&apos;", "'");
                siteXml = siteXml.Replace("&amp;", "&");
                siteXml = siteXml.Replace("&", "&amp;");
                siteXml = Regex.Replace(siteXml, "='([^'=]+)'([^'=]*)' ", "='$1&apos;$2' ");
                siteXml = siteXml.Replace("`", "&apos;");

                if (IsSiteXmlValid(siteXml))
                {
                    ActiveSiteTemplateXml = siteXml;
                    
                }
                else
                {
                    throw new Exception(string.Format(ERROR_INVALIDSITEXML, siteXml));
                }
            }
        }

        /// <summary>
        ///   Xml string describing the structure of the site
        /// </summary>
        public string SiteXml
        {
            get { return ActiveSiteXml; }
        }

        /// <summary>
        ///   Xml string describing the structure of the site and all books contained in the site
        /// </summary>
        public string SiteBookXml
        {
            get { return ActiveSiteBookXml; }
        }

        /// <summary>
        ///   Indicates whether or not the site has pending changes that need to be committed
        /// </summary>
        public bool HasChanges
        {
            get { return ActiveSiteDs.HasChanges(); }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node id
        /// </summary>
        public int NodeId
        {
            get { return Id; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node type
        /// </summary>
        public NodeType NodeType
        {
            get { return NodeType.Site; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node right value
        /// </summary>
        public int Right
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node left value
        /// </summary>
        public int Left
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicating whether or not the toc node has children nodes
        /// </summary>
        public bool HasChildren
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicates whether or not the node should be hidden in the toc
        /// </summary>
        public bool Hidden
        {
            get { return false; }
        }

        /// <summary>
        ///   ITocNode interface property that returns the uri of the node
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Commits all changes to a site to the database
        /// </summary>
        public void Save()
        {
            //save the main site data
            ActiveSiteDalc.Save(ActiveSiteDs);
            //
        }

        /// <summary>
        ///   Builds a site from the SiteTemplateXml
        /// </summary>
        public void Build()
        {
            try
            {
                //don't do anything but make sure an index build is requested if we are going from staging to production (the siteindex build will promote us once the index build is complete)
                if (Status == SiteStatus.Staging && RequestedStatus == SiteStatus.Production)
                {
                    IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
                }
                else
                {
                    //build our site from the template
                    BuildSiteFromXml(SiteTemplateXml);
                }

                //if we get here we were successful, so set our build status to Built
                BuildStatus = SiteBuildStatus.Built;
                //jjs 11/22/06: only set the site status to the requested status when there is not
                //				a site index build happening as well
                if (IndexBuildStatus != SiteIndexBuildStatus.BuildRequested)
                {
                    Status = RequestedStatus;
                }
                activeSiteXml = null;
                activeSiteBookXml = null;
                Save();
            }
            catch (Exception e)
            {
                //If we get here, something went wrong with the build. Log the problem and set the BuildStatus to Error.
                //log the error
                Event siteEvent = new Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_SITEBUILD_ERROR, MODULE_SITE,
                                            METHOD_SITEBUILD, Id.ToString(), e.Message);
                siteEvent.Save(false);
                //set our Error status and save the site
                BuildStatus = SiteBuildStatus.Error;
                IndexBuildStatus = SiteIndexBuildStatus.NotBuilt;
                Status = SiteStatus.Unassigned;
                RequestedStatus = SiteStatus.Unassigned;
                Save();
            }
        }

        /// <summary>
        ///   Locates the document that follows the specified document. ToC order is used to determine the next document.
        /// </summary>
        /// <param name = "contextDocument">The context document to use for finding the next document</param>
        /// <returns>An interface for accessing the next document, or null if no next document is found.</returns>
        public IDocument GetNextDocument(IDocument contextDocument)
        {
            IDocument nextDoc = contextDocument.Book.GetNextDocument(contextDocument);

            /* MBennett - Getting rid of next navigation at end of a book
            if (nextDoc == null) //the book did not have a next document, so we need to look in the next book
            {
                IBook nextBook = GetNextBook(contextDocument.Book);
                if (nextBook != null) //there was a next book, so return its first document
                {
                    nextDoc = nextBook.Documents[0];
                }
                
                else //there was no next book, so this is final document in the set, return to first document
                {
                    // Determine first document of set.
                    IBook firstBook = Books[0];
                    if (firstBook.Documents != null)
                    {
                        IDocument firstDoc = firstBook.Documents[0];

                        nextDoc = firstDoc;
                    }
                }
            }*/

            //just let nextDoc be returned as null if no nextDoc was found
            return nextDoc;
        }

        /// <summary>
        ///   Locates the document that precedes the specified document. ToC order is used to determine the previous document.
        /// </summary>
        /// <param name = "contextDocument">The context document to use for finding the previous document</param>
        /// <returns>An interface for accessing the previous document, or null if no previous document is found.</returns>
        public IDocument GetPreviousDocument(IDocument contextDocument)
        {
            IDocument prevDoc = contextDocument.Book.GetPreviousDocument(contextDocument);

            /* MBennett - Getting rid of previous navigation at beginning of a book
            if (prevDoc == null) //the book did not have a prev document, so we need to look in the prev book
            {
                IBook prevBook = GetPreviousBook(contextDocument.Book);
                if (prevBook != null) //there was a previous book, so return its last document
                {
                    prevDoc = prevBook.Documents[prevBook.Documents.Count - 1];
                }
                
                else //there was no previous book, so this is first document in the set, go to last document
                {
                    // Determine first document of set.
                    //nextDoc = SiteIndex
                    int lastBookIndex = Books.Count - 1;
                    if (Books[lastBookIndex].Documents != null)
                    {
                        int lastDocumentIndex = Books[lastBookIndex].Documents.Count - 1;
                        if (lastDocumentIndex > -1)
                        {
                            IDocument lastDoc = Books[lastBookIndex].Documents[lastDocumentIndex];

                            prevDoc = lastDoc;
                        }
                    }
                }
            }*/

            //just let prevDoc be returned as null if no prevDoc was found
            return prevDoc;
        }

        /// <summary>
        ///   Locates the book that follows the specified book. ToC order is used to determine the next book.
        /// </summary>
        /// <param name = "contextBook">The context book to use for finding the next book</param>
        /// <returns>An interface for accessing the next book, or null if no next book is found.</returns>
        public IBook GetNextBook(IBook contextBook)
        {
            //the book to return
            IBook retBook = null;

            //the temporary site toc node row we will use
            SiteTocNodeDs.SiteTocNodeRow siteTocNodeRow = null;

            //get the user
            IUser user = User;

            //filter if we have a user
            if (user != null)
            {
                siteTocNodeRow = ActiveSiteDalc.GetNextBook(Id, contextBook.Id,
                                                            TranslateBookDomain(user.UserSecurity.BookName));
            }
            else
            {
                siteTocNodeRow = ActiveSiteDalc.GetNextBook(Id, contextBook.Id);
            }

            //instantiate a book if we got results
            if (siteTocNodeRow != null)
            {
                retBook = new Book.Book(this, siteTocNodeRow.NodeId);
            }

            //return the book
            return retBook;
        }

        /// <summary>
        ///   Locates the book that precedes the specified book. ToC order is used to determine the previous book.
        /// </summary>
        /// <param name = "contextBook">The context book to use for finding the previous book</param>
        /// <returns>An interface for accessing the previous book, or null if no previous book is found.</returns>
        public IBook GetPreviousBook(IBook contextBook)
        {
            //the book to return
            IBook retBook = null;

            //the temporary site toc node row we will use
            SiteTocNodeDs.SiteTocNodeRow siteTocNodeRow = null;

            //get the user
            IUser user = User;

            //filter if we have a user
            if (user != null)
            {
                siteTocNodeRow = ActiveSiteDalc.GetPreviousBook(Id, contextBook.Id,
                                                                TranslateBookDomain(user.UserSecurity.BookName));
            }
            else
            {
                siteTocNodeRow = ActiveSiteDalc.GetPreviousBook(Id, contextBook.Id);
            }

            //instantiate a book if we got results
            if (siteTocNodeRow != null)
            {
                retBook = new Book.Book(this, siteTocNodeRow.NodeId);
            }

            //return the book
            return retBook;
        }

        /// <summary>
        ///   Adds the specified book to the site.
        /// </summary>
        /// <param name = "book"></param>
        public void AddBook(IBook book)
        {
            //throw an error if the book already exists in the site
            if (Books[book.Name] != null)
            {
                throw new Exception(string.Format(ERROR_DUPLICATESITEBOOK, book.Name));
            }

            //throw an error if coder is trying to add a book before saving the site
            if (HasChanges)
            {
                throw new Exception(ERROR_SITEBOOKADDSITENOTSAVED);
            }

            //throw an error if coder is trying to add a book before saving the book
            if (book.HasChanges)
            {
                throw new Exception(ERROR_SITEBOOKADDBOOKNOTSAVED);
            }

            //add the sitebook row to our active site dataset
            ActiveSiteDs.SiteBook.AddSiteBookRow(ActiveSiteRow, book.Id);

            //add the sitebook entry to the db
            ActiveSiteDalc.InsertSiteBooks(ActiveSiteDs.SiteBook);

            //reset the book collection because it has new data in it
            activeBookCollection = null;

            //reset the site toc node collection because it has new data in it
            activeSiteTocNodeCollection = null;
        }

        /// <summary>
        ///   Removes the specified book from the site.
        /// </summary>
        /// <param name = "book"></param>
        public void RemoveBook(IBook book)
        {
            //make sure the book exists on the site
            if (Books[book.Name] == null)
            {
                throw new Exception(string.Format(ERROR_BOOKNOTFOUND, book.Name));
            }

            //remove the sitebook entry from the active site dataset
            SiteDs.SiteBookRow sbr =
                (SiteDs.SiteBookRow)
                ActiveSiteDs.SiteBook.Select(string.Format("SiteId = '{0}' AND BookInstanceId = {1}", Id, book.Id))[0];
            ActiveSiteDs.SiteBook.RemoveSiteBookRow(sbr);


            //todo jjs 6/23: This is hokey...change to 
            //remove the sitebook entry from the db
            ActiveSiteDalc.RemoveSiteBook(Id, book.Id);

            //reset the book collection because it has new data in it
            activeBookCollection = null;

            //reset the site toc node collection because it has new data in it
            activeSiteTocNodeCollection = null;
        }

        public string GetTocXml(int nodeId, NodeType nodeType)
        {
            return GetTocXml(nodeId, nodeType, false);
        }

        /// <summary>
        ///   Retrieves an XML string describing the specified node and its immediate children.
        /// </summary>
        /// <param name = "nodeId">The id of the node you wish to retrieve.</param>
        /// <param name = "nodeType">The type of node you wish to retrieve.</param>
        /// <returns></returns>
        public string GetTocXml(int nodeId, NodeType nodeType, bool ignoreAnchors)
        {
            //our xml to return
            string retXml = string.Empty;

            //our hash key
            string hashKey = nodeType + ":" + nodeId;
            string BookHashKey = "Book:" + nodeId;

            //if the xml for this guy is in our hashtable, return it; otherwise get it from the dalc
            if (tocXmlHash.ContainsKey(hashKey))
            {
                retXml = (string) tocXmlHash[hashKey];
                if (retXml == "")
                {
                    retXml = (string)tocXmlHash[BookHashKey];
                }
            }
            else if (tocXmlHash.ContainsKey(BookHashKey))
            {
                retXml = (string)tocXmlHash[BookHashKey];
            }
            else
            {
                lock (tocXmlHash)
                {
                    if (tocXmlHash.ContainsKey(hashKey))
                    {
                        retXml = (string)tocXmlHash[hashKey];
                    }
                    else
                    {
                        //we retrieve the xml from the site or the book node collection bpc, depending on the type of node we are dealing with
                        switch (nodeType)
                        {
                            case NodeType.Site:
                            case NodeType.SiteFolder:

                                //ask for the xml from the site toc node collection
                                SiteTocNodeCollection siteTocNodes = new SiteTocNodeCollection(Id, nodeId, nodeType);
                                retXml = GetXmlFromTocNodes(siteTocNodes, false, ignoreAnchors);

                                break;
                            case NodeType.Book:
                            case NodeType.BookFolder:
                            case NodeType.Document:
                            case NodeType.DocumentAnchor:

                                //ask for the xml from the book toc node collection
                                BookTocNodeCollection bookTocNodes = new BookTocNodeCollection(nodeId, nodeType);
                                retXml = GetXmlFromTocNodes(bookTocNodes, false);

                                break;
                            default:
                                throw new Exception(string.Format(ERROR_NODETYPEINVALIDCONTEXT, nodeType, "GetTocXml"));
                        }

                        //add the unfiltered xml to the static hash
                        tocXmlHash[hashKey] = retXml;
                    }
                }
            }

            //now filter the xml to remove anything not meant for the active user, if there is an active user
            if (ActiveUser != null)
            {
                string[] books = ActiveUser.UserSecurity.BookName ?? new string[0];
                if (retXml != "")
                {
                    retXml = FilterTocXml(retXml, books, true);
                }
                else
                { 
                    //We should never get here, but this is a good spot for a breakpoint when debugging.
                }
            }

            //pass back our XML
            return retXml;
        }

        /// <summary>
        ///   Gets Alternate Book for a given conbination of Book,Target Pointer and User Site
        /// </summary>
        /// <param name = "targetPtr">Link Target Pointer</param>
        /// <param name = "targetDoc">Link Book</param>
        /// <param name = "siteDomain">List of books for given user/site.  Comma delimited
        ///   book1,book2,book3</param>
        /// <returns>Only one alternate book if the target pointer was reused</returns>
        public string AlternateBook(string targetPtr, string targetDoc, string siteDomain)
        {
            return ActiveSiteDalc.AlternateBook(Id, targetPtr, targetDoc, siteDomain);
        }

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        public string[] GetXRefStandardTypes()
        {
            return ActiveSiteDalc.GetXRefStandardTypes();
        }

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        public string[] GetXRefSubtopicsByTopic(string Topic)
        {
            return ActiveSiteDalc.GetXRefSubtopicsByTopic(Topic);
        }

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        public string[] GetXRefSectionsByTopicSubtopic(string Topic, string Subtopic)
        {
            return ActiveSiteDalc.GetXRefSectionsByTopicSubtopic(Topic, Subtopic);
        }

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        public string[] GetXRefTopics()
        {
            return ActiveSiteDalc.GetXRefTopics();
        }

        /// <summary>
        ///   Gets the collection of unique Standard Numbers for a specified Standard Type from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter Standard Numbers on.</param>
        /// <returns>String array containing the Standard Numbers for the specified Standard Type.</returns>
        public string[] GetXRefStandardNumbersForStandardType(string standardType)
        {
            return ActiveSiteDalc.GetXRefStandardNumbersForStandardType(standardType);
        }

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Standard Type and Standard Number from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter on in searching for cross-referenced Codification entires.</param>
        /// <param name = "standardNum">Standard Number in which to filter on in searching for cross-referenced Codification entries.</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        public SiteDs.XRefRow[] GetXRefCodByStandard(string standardType, string standardNum)
        {
            return ActiveSiteDalc.GetXRefCodByStandard(standardType, standardNum);
        }

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Topic,Subtopic and section
        /// </summary>
        /// <param name = "topic">Codification Topic</param>
        /// <param name = "subtopic">Codification SubTopic</param>
        /// <param name = "section">Codification Section</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        public SiteDs.XRefRow[] GetXRefStandardByCod(string topic, string subtopic, string section)
        {
            return ActiveSiteDalc.GetXRefStandardByCod(topic, subtopic, section);
        }

        /// <summary>
        /// Gets the S join topics.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinTopics()
        {
            return ActiveSiteDalc.GetSJoinTopics();
        }

        /// <summary>
        /// Gets the S join docs by topic section.	
        /// </summary>
        /// <param name="topicNum">The topic num.</param>
        /// <param name="sectionNum">The section num.</param>
        /// <param name="intersection">The intersection.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinDocsByTopicSection(string topicNum, String sectionNum, int intersection)
        {
            return ActiveSiteDalc.GetSJoinDocsByTopicSection(topicNum, sectionNum, intersection);
        }

        /// <summary>
        /// Gets the S join sections by topic.	
        /// </summary>
        /// <param name="topicNum">The topic num.</param>
        /// <param name="intersection">The intersection.</param>
        /// <param name="sec">The sec.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinSectionsByTopic(string topicNum, int intersection, int sec)
        {
            return ActiveSiteDalc.GetSJoinSectionsByTopic(topicNum, intersection, sec);
        }

        /// <summary>
        /// Gets the subtopic by topic.	
        /// </summary>
        /// <param name="topicNum">The topic num.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSubtopicByTopic(string topicNum)
        {
            return ActiveSiteDalc.GetSubtopicByTopic(topicNum);
        }

        /// <summary>
        /// Gets the section by topic subtopic.	
        /// </summary>
        /// <param name="topicNum">The topic num.</param>
        /// <param name="subTopicNum">The sub topic num.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSectionByTopicSubtopic(string topicNum, string subTopicNum)
        {
            return ActiveSiteDalc.GetSectionByTopicSubtopic(topicNum, subTopicNum);
        }


        ///<summary>
        ///  Gets a list of book names that will contain archived Documents
        ///</summary>
        public string getArchiveList()
        {
            return SITE_ARCHIVE_BOOK_ID_LIST;
        }

        /// <summary>
        /// Gets the reference link list.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string getReferenceLinkList()
        {
            return SITE_REFERENCING_LINKS_BOOK_ID_LIST;
        }

        /// <summary>
        /// Gets the X ref list.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string getXRefList()
        {
            return SITE_XREF_BOOK_ID_LIST;
        }

        /// <summary>
        /// Gets the J section list.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string getJSectionList()
        {
            return SITE_JSECTION_BOOK_ID_LIST;
        }

        #endregion

        #endregion
    }
}