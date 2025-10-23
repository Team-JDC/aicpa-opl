using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.SessionState;

using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;

namespace MainUI.Shared
{
    public class ContextManager
    {
        public ContextManager(HttpContext context)
        {
            Context = context;
        }

        public HttpContext Context { get; set; }
        protected HttpSessionState Session
        {
            get
            {
                return Context.Session;
            }
        }

        #region Application Wide Constants
        // Request Parameters
        public const string REQPARAM_HITHIGHLIGHTS = "d_hh";
        public const string REQPARAM_HITANCHOR = "hitanchor";
        public const string REQPARAM_TARGETDOC = "targetdoc";
        public const string REQPARAM_TARGETPTR = "targetptr";
        public const string REQPARAM_DOCID = "docid";
        public const string REQPARAM_ID = "id";
        public const string REQPARAM_TYPE = "type";
        public const string REQPARAM_TABLE = "table";
        public const string REQPARAM_CURRENTDOCUMENTFORMATID = "d_ft";
        public const string REQPARAM_ISCODIFICATION = "isCodification";
        public const string REQPARAM_SHOWCODIFICATIONSOURCES = "showCodificationSources";
        public const string REQPARAM_PRINTTOPDF = "printToPDF";
        public const string REQPARAM_PRINTDESCENDANTS = "printSubdocuments";
        public const string REQPARAM_DOPRINT = "doPrint";
        public const string REQPARAM_SHOWSOURCES = "show_sources";
        public const string REQPARAM_RESOURCEBOOKNAME = "r_bn";
        public const string REQPARAM_RESOURCENAME = "r_rn";
        public const string REQPARAM_SUB = "sub";
        public const string REQPARAM_LMS = "lms";
        public const string REQPARAM_DOCNUM = "docnum";
        public const string REQPARAM_DISABLEFEATURES = "d_feat";
        public const string REQPARAM_JOINSECTIONS = "joinsecs";
        public const string REQPARAM_TABLESTYLE = "tablestyle";


        // Session Keys
        public const string SESSION_USER = "user";
        public const string SESSION_SITE = "site";
        public const string SESSION_MYDOCS = "mydocs";
        public const string SESSION_CURRENTSEARCHRESULTS = "CurrentSearchResults";
        public const string SESSION_CURRENTSEARCHCRITERIA = "CurrentSearchCriteria";
        public const string SESSION_SHOWHIDESOURCE = "showhidesource";
        public const string SESSION_MYDOCUMENT_DICT = "mydocumentdict";
        public const string SESSION_ANALYTICS_LOGOUT = "analyticslogout";
        
        
        // Cookie Keys
        public const string COOKIE_ETHICS = "ethics";
        public const string COOKIE_KEY_USERID = "ethics_userid";
        public const string COOKIE_KEY_REFERRINGSITE = "ethics_referringsite";

        // Web Config Keys
        public const string WEBCONFIG_SECURITYBYPASS = "User_SecurityBypassDomain";
        public const string WEBCONFIG_SITESTATUS = "PortalApp_SiteStatus";
        public const string WEBCONFIG_FORUMURL = "ForumUrl";
        public const string WEBCONFIG_FEEDBACK_EMAIL_ADDRESS = "FeedbackEmailAddress";
        public const string WEBCONFIG_IS_XML_UNDER_WEB_APP = "IsXmlUnderWebApp";
        public const string WEBCONFIG_XMLFILE_HOMEPAGE = "XmlFile_HomePage";
        public const string WEBCONFIG_XMLFILE_TAXONOMY = "XmlFile_Taxonomy";
        public const string WEBCONFIG_XMLFILE_TIPS_AND_TECHNIQUES = "XmlFile_TipsAndTechniques";
        public const string WEBCONFIG_XMLFILE_WHATS_NEW = "XmlFile_WhatsNew";
        public const string WEBCONFIG_XMLFILE_SITE_FOLDERS = "XmlFile_SiteFolders";
        public const string WEBCONFIG_FAF_ARCHIVE_FOLDER = "FafArchiveFolder";
        public const string WEBCONFIG_BOOK_CONTENT_FOLDER = "Book_ContentFolder";
        public const string WEBCONFIG_PRINT_BASE_URL = "PrintBaseUrl";
        public const string WEBCONIFG_SHOULD_REPLACE_PRINT_BASE_URL = "ShouldReplacePrintBaseUrl";
        public const string WEBCONFIG_XMLFILE_VALID_SITES = "XmlFile_ValidSites";
        public const string WEBCONFIG_LMS_HASHSEED = "LMSHashSeed";
        public const string WEBCONFIG_C2B_LOGINURL = "C2bLoginUrl";


        //book prefixes
        public const string BOOK_PREFIX_FASB = "fasb";
        public const string BOOK_PREFIX_FAF = "faf";
        public const string BOOK_PREFIX_GASB = "gasb";
        public const string BOOK_PREFIX_EMAP = "emap";
        public const string BOOK_PREFIX_AAG = "aag";
        public const string BOOK_PREFIX_ATT = "att";
        public const string BOOK_PREFIX_ABM = "abm";
        public const string BOOK_PREFIX_ARA = "ara";
        public const string BOOK_PREFIX_CHK = "chk";
        public const string BOOK_PREFIX_WHATSNEW = "whatsnew";
        public const string BOOK_PREFIX_ARCH = "arch";

        // XML Constants
        public const string XML_ATT_ID = "Id";

        // Other Constants
        public const string HILITE_ANCHORNAME = "destroyer_hilite";
        public static string HILITE_BEGINTAG = "<a name='" + HILITE_ANCHORNAME + "'></a><span style='color:white; background-color:navy;'>";
        public const string HILITE_ENDTAG = "</span>";
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns true if the current user is authenticated.  Requires the context of a Session.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                bool isAuthenticated = false;

                // we may want to add additional checks here, but for now, just check that there is a user on the session
                if (CurrentUser != null)
                {
                    isAuthenticated = true;
                }

                return isAuthenticated;
            }
        }

        public bool HasCurrentUser
        {
            get
            {
                return Session[SESSION_USER] != null || !string.IsNullOrEmpty(SecurityBypassDomainValue);
            }

        }
        /// <summary>
        /// Gets the current user from the Session.
        /// </summary>
        public IUser CurrentUser
        {
            get
            {
                IUser user = null;
                // this checks for the user on the session, if it's not there, it tries to get it, then puts it on the session.
                // we may also consider caching it in this class

                if (Session[SESSION_USER] == null)
                {
                    // the user should have been there from the SeamlessLogin page
                    // so either the user isn't authenticated or we're in security bypass mode

                    if (!string.IsNullOrEmpty(SecurityBypassDomainValue))
                    {
                        // we are in security bypass mode
                        user = new User(Guid.NewGuid(), ReferringSite.Csc);
                        string domainList = SecurityBypassDomainValue;

                        user.LogOn(Session.SessionID, domainList);

                        Session[SESSION_USER] = user;
                    }
                    else
                    {
                        throw new UserNotAuthenticated("UserNotAuthenticated: No user object on the session.");
                    }
                }
                else
                {
                    // user is already on session
                    user = (IUser)Session[SESSION_USER];
                }

                return user;
            }
        }

        /// <summary>
        /// Gets the current site from the session (puts it there if not already present).  Requires an authenticated user.
        /// </summary>
        public ISite CurrentSite
        {
            get
            {
                ISite site;

                if (Session[SESSION_SITE] == null)
                {                    
                    site = new Site(CurrentUser, CurrentSiteStatus);
                    Session[SESSION_SITE] = site;
                }
                else
                {
                    // site was already on session
                    site = (ISite)Session[SESSION_SITE];
                }

                return site;
            }
        }

        /// <summary>
        /// Code used to Hash the MyDocuments so we don't have to keep looking up the targetdoc/targetptrs
        /// </summary>
        public Dictionary<string,MyDocument> MyDocumentHash
        {
            get
            {
                Dictionary<string, MyDocument> d;
                if (Session[SESSION_MYDOCUMENT_DICT] == null)
                {
                    d = new Dictionary<string, MyDocument>();
                    Session[SESSION_MYDOCUMENT_DICT] = d;
                }
                else
                {
                    d = (Dictionary<string, MyDocument>)Session[SESSION_MYDOCUMENT_DICT];

                }

                return d;
            }

            set
            {
                Session[SESSION_MYDOCUMENT_DICT] = value;
            }
        }

        /// <summary>
        /// Gets the current list of "MyDocuments" from the session.  Creates an empty list if not already present.
        /// </summary>
        public List<IDocument> MyDocuments
        {
            get
            {
                List<IDocument> list;

                if (Session[SESSION_MYDOCS] == null)
                {
                    list = new List<IDocument>();
                    Session[SESSION_MYDOCS] = list;
                }
                else
                {
                    list = (List<IDocument>)Session[SESSION_MYDOCS];
                }

                return list;
            }
        }

        public ISearchResults SearchResults
        {
            get
            {
                return (ISearchResults)Session[SESSION_CURRENTSEARCHRESULTS];
            }
            set
            {
                Session[SESSION_CURRENTSEARCHRESULTS] = value;
            }
        }

        public ISearchCriteria SearchCriteria
        {
            get
            {
                return (ISearchCriteria)Session[SESSION_CURRENTSEARCHCRITERIA];
            }
            set
            {
                Session[SESSION_CURRENTSEARCHCRITERIA] = value;
            }
        }

        public bool ShowSources
        {
            get
            {
                if (Session[SESSION_SHOWHIDESOURCE] == null)
                {
                    Session[SESSION_SHOWHIDESOURCE] = false;
                }

                return (bool) Session[SESSION_SHOWHIDESOURCE];
            }
            set
            {
                Session[SESSION_SHOWHIDESOURCE] = value;
            }
        }

        public string ForumUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_FORUMURL];
            }
        }

        public string FeedbackEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_FEEDBACK_EMAIL_ADDRESS];
            }
        }

        public bool IsXmlUnderWebApp
        {
            get
            {
                bool isUnderWebApp;

                string value = ConfigurationManager.AppSettings[WEBCONFIG_IS_XML_UNDER_WEB_APP];

                if (string.IsNullOrEmpty(value) || value.ToLower() == "false")
                {
                    isUnderWebApp = false;
                }
                else
                {
                    isUnderWebApp = true;
                }

                return isUnderWebApp;
            }
        }

        public string XmlFile_HomePage
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_HOMEPAGE, IsXmlUnderWebApp);
            }
        }

        public string XmlFile_Taxonomy
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_TAXONOMY, IsXmlUnderWebApp);
            }
        }

        public string XmlFile_TipsAndTechniques
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_TIPS_AND_TECHNIQUES, IsXmlUnderWebApp);
            }
        }

        public string XmlFile_WhatsNew
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_WHATS_NEW, IsXmlUnderWebApp);
            }
        }

        public string XmlFile_SiteFolders
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_SITE_FOLDERS, IsXmlUnderWebApp);
            }
        }

        public string XmlFile_ValidSites
        {
            get
            {
                return GetXmlFileSystemPath(WEBCONFIG_XMLFILE_VALID_SITES, IsXmlUnderWebApp);
            }
        }   

        public string LMSHashSeed
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_LMS_HASHSEED]; 
            }
        }   

        private string GetXmlFileSystemPath(string webConfigKey, bool isUnderWebRoot)
        {
            string path = ConfigurationManager.AppSettings[webConfigKey];

            if (isUnderWebRoot)
            {
                path = Context.Server.MapPath("~/" + path);
            }

            return path;
        }

        public string FafArchiveFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_FAF_ARCHIVE_FOLDER];
            }
        }

        public string BookContentFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_BOOK_CONTENT_FOLDER];
            }
        }

        public string PrintBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[WEBCONFIG_PRINT_BASE_URL];
            }
        }

        public bool ShouldReplacePrintBaseUrl
        {
            get
            {
                bool shouldReplace;

                string value = ConfigurationManager.AppSettings[WEBCONIFG_SHOULD_REPLACE_PRINT_BASE_URL];

                if (string.IsNullOrEmpty(value) || value.ToLower() == "false")
                {
                    shouldReplace = false;
                }
                else
                {
                    shouldReplace = true;
                }

                return shouldReplace;

            }
        }


        #endregion

        #region Private Properties
        private SiteStatus CurrentSiteStatus
        {
            get
            {
                // sburton: this could be cleaned up.  I took it right from the old code
                return (SiteStatus)Enum.Parse(typeof(SiteStatus), ConfigurationManager.AppSettings[WEBCONFIG_SITESTATUS], true);
            }
        }

        /// <summary>
        /// Gets the security bypass domain value from the web.config if present.
        /// </summary>
        private string SecurityBypassDomainValue
        {
            get
            {
                return string.Empty;
                //return ConfigurationManager.AppSettings[WEBCONFIG_SECURITYBYPASS];
            }
        }

        #endregion

        #region Public Methods
        public void ClearSession()
        {
            try
            {

                AICPA.Destroyer.User.Event.IEvent logEvent;
                logEvent = new AICPA.Destroyer.User.Event.Event(AICPA.Destroyer.User.Event.EventType.Info
                    ,DateTime.Now
                    , 1
                    ,"ContextManager.cs"
                    , "ClearSession"
                    ,"Clearing Session"
                    ,"Clearing Session..",
                    CurrentUser);
                logEvent.Save(false);
            }
            catch
            {
                //
            }
            Session.Abandon();
        }

        /// <summary>
        ///   Gets the site status.
        /// </summary>
        /// <param name = "siteStatus">The site status.</param>
        /// <returns></returns>
        public SiteStatus GetSiteStatus(string siteStatus)
        {
            SiteStatus _siteStatus = new SiteStatus();
            switch (siteStatus)
            {
                case "Staging":
                    _siteStatus = SiteStatus.Staging;
                    break;
                case "Production":
                    _siteStatus = SiteStatus.Production;
                    break;
                case "Pre-production":
                    _siteStatus = SiteStatus.PreProduction;
                    break;
                default:
                    _siteStatus = SiteStatus.Unassigned;
                    break;
            }
            return _siteStatus;
        }

        public void AddSiteCookie(string name, Dictionary<string, string> values, int days)
        {
            HttpCookie c = new HttpCookie(name);
            foreach (KeyValuePair<string, string> kvp in values)
            {
                c.Values.Add(kvp.Key, kvp.Value);
            }
            c.Expires = DateTime.Now.AddDays(days);
            c.HttpOnly = true;
            Context.Response.SetCookie(c);
        }

        public HttpCookie GetSiteCookie(string name)
        {
            return Context.Request.Cookies[ContextManager.COOKIE_ETHICS];
        }

        public void RemoveCookie(string name)
        {
            AddSiteCookie(name, new Dictionary<string, string>(), -1);
        }

        #endregion

        public void KeepSessionAlive()
        {
            IUser u = CurrentUser;
            Session["KeepAlive"] = DateTime.Now.ToString();

            // TODO: do we need to do anything in the DB to keep alive?
        }
    }
}