using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI.HtmlControls;
using MainUI.WS;
using System.Text;
using MainUI.Shared;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;

namespace MainUI
{
    public partial class ODPMaster : System.Web.UI.MasterPage
    {
        #region Context Manager
        private MainUI.Shared.ContextManager _contextManager = null;
        private MainUI.Shared.ContextManager ContextManager
        {
            get
            {
                if (_contextManager == null)
                {
                    _contextManager = new MainUI.Shared.ContextManager(Context);
                }

                return _contextManager;
            }
        }

        private MainUI.WS.Content _ContentService = null;
        private MainUI.WS.Content ContentService
        {
            get
            {
                if (_ContentService == null)
                {
                    _ContentService = new WS.Content();
                }
                return _ContentService;
            }
        }

        private WS.EndecaServices _EndecaService = null;
        private WS.EndecaServices EndecaService
        {
            get
            {
                if (_EndecaService == null)
                {
                    _EndecaService = new WS.EndecaServices();
                }
                return _EndecaService;
            }
        }


        private WS.HomePage _HomePageService = null;
        private WS.HomePage HomePageService
        {
            get
            {
                if (_HomePageService == null)
                {
                    _HomePageService = new WS.HomePage();
                }
                return _HomePageService;
            }
        }

        private WS.DocumentTools _DocumentService = null;
        private WS.DocumentTools DocumentService
        {
            get
            {
                if (_DocumentService == null)
                {
                    _DocumentService = new WS.DocumentTools();
                }
                return _DocumentService;
            }
        }

        private Shared.HomePageNode _HomePageNode = null;
        private Shared.HomePageNode HomePageNode
        {
            get
            {
                if (_HomePageNode == null)
                {
                    _HomePageNode = HomePageService.GetRecentDocuments();
                }
                return _HomePageNode;
            }
        }
        //List<MainUI.Shared.MyDocument> l = c.GetLibraryBooks();
        private List<Shared.SiteNode> _MyLibraryList = null;
        private List<Shared.SiteNode> MyLibrary
        {
            get
            {
                if (_MyLibraryList == null)
                {                    
                    _MyLibraryList = ContentService.GetMyLibraryBooks();                    
                }
                return _MyLibraryList;
            }
        }
        


        
        #endregion
        
        protected void Page_Init(object sender, EventArgs e)
        {

        }

        private StringBuilder BuildMyLibraryMenu()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb = new StringBuilder();
                foreach (Shared.SiteNode sn in MyLibrary)
                {
                    sb.Append(string.Format("<li><a href=\"/content/{0}/{1}\"><span>{2}</span></a></li>", sn.Type, sn.Id, sn.Title));
                }
                HomePageNode node = new HomePageNode();
                
                node.Domain = ContextManager.CurrentUser.UserSecurity.Domain;
                if (node.IsCoso)
                {
                    string cosopath = node.CosoPath;
                    string linkOnclick = "onclick=\"top.doCosoLink('" + ContextManager.CurrentUser.UserSecurity.User.UserId + "', '" + ContextManager.CurrentUser.UserSecurity.Domain + "', 'COSO','" + cosopath + "')\"";
                    string cosolink = "<li " + linkOnclick + " style=\"cursor:pointer\"><a><span>COSO Collection</span></a></li>";
                    //sb.Append(string.Format("<li " + linkOnclick + "><a href=\"/content/{0}/{1}\"><span>{2}</span></a></li>", sn.Type, sn.Id, sn.Title));
                    sb.Append(cosolink);
                }
            }
            catch (Exception ex)
            {
                //
                throw;
            }
            return sb;

        }

        private StringBuilder BuildSearchMenu()
        {


            StringBuilder sb = new StringBuilder();
            try
            {
                MainUI.WS.EndecaServices.SearchResultResponse sr = EndecaService.DoBlankSearch();
                //foreach (Shared.SiteNode sn in MyLibrary)
                //{
                //    sb.Append(string.Format("<option>{0}</option>", sn.Title));
                //}
                sb.Append(string.Format("<option value=\"null\" selected>All Collections:</option>"));
                for (int i = 0; i < sr.DimensionResults.Count; i++)
                {
                    sb.Append(string.Format("<option value='" + sr.DimensionResults[i].DimensionId + "'>" + sr.DimensionResults[i].DimensionName + "</option>"));
                }
            }
            catch (Exception ex)
            {
                //There was an error connecting to Endeca.  We're going to log it and return an empty string.  This is not ideal, but it's better than the alternative of just blowing up which we were doing before.
                IEvent logEvent = null;
                logEvent = new Event(EventType.Usage,
                   DateTime.Now,
                   3,
                   "ODPMaster.Master.cs",
                   "BuildSearchMenu",
                   "Endeca_Error",
                   ex.Message);
                logEvent.Save();

            }
            return sb;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!ContextManager.HasCurrentUser)
            {
                WS.Toolbars tb = new Toolbars();
                tb.LogError("Redirecting to SessionExpired... HasCurrentUser = false");
                //Response.Redirect("/Login?relayState=" + Request.Url.AbsoluteUri);
                Response.Redirect(ConfigurationManager.AppSettings[ContextManager.WEBCONFIG_C2B_LOGINURL] + "?relayState=" + Request.Url.AbsoluteUri);
            }

            

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultPageRedirect"]))
            {
                Response.Redirect(ConfigurationManager.AppSettings["DefaultPageRedirect"]);
            }

            if (!IsPostBack)
            {

                try
                {
                    string nodeType = string.Empty;
                    if (Page.RouteData.Values["nodetype"] != null)
                    {
                        nodeType = Page.RouteData.Values["nodetype"].ToString();
                    }

                    string targetDoc = string.Empty;
                    if (Page.RouteData.Values["targetdoc"] != null)
                    {
                        targetDoc = Page.RouteData.Values["targetdoc"].ToString();
                    }

                    string targetPtr = string.Empty;
                    if (Page.RouteData.Values["targetptr"] != null)
                    {
                        targetPtr = Page.RouteData.Values["targetptr"].ToString();
                    }

                    if (this.liLibrary != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<a class=\"top_level\">" +
                        "<span class=\"icon\">" +
                        "<img src=\"/elements/img/primary-nav-mobile-icon-sprite.png\" alt=\"\"/></span>My Library<img class=\"arrow\" src=\"/elements/img/primary-nav-top-arrow.png\" alt=\"\" />" +
                        "</a>");
                        sb.Append("<ul class=\"submenu\">");
                        sb.Append(BuildMyLibraryMenu().ToString());
                        sb.Append("</ul>");
                        liLibrary.InnerHtml = sb.ToString();
                    }

                    if (this.liHistory != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<a class=\"top_level\">" +
                        "<span class=\"icon\"><img src=\"/elements/img/primary-nav-mobile-icon-sprite.png\" alt=\"\"/></span>" +
                        "History<img class=\"arrow\" src=\"/elements/img/primary-nav-top-arrow.png\" alt=\"\" /></a>");
                        sb.Append("<ul class=\"submenu\">");

                        if (HomePageNode.RecentDocs.Count == 0)
                        {
                            sb.Append("<li><a href=\"\"><span>None <img src=\"/elements/img/primary-nav-double-arrow.png\" alt=\"\" /></span></a></li>");
                        }
                        else
                        {
                            foreach (Shared.RecentDocument rd in HomePageNode.RecentDocs)
                            {
                                sb.Append(string.Format("<li><a href=\"/content/link/{1}/{2}\"><span>{0} <img src=\"/elements/img/primary-nav-double-arrow.png\" alt=\"\" /></span></a></li>", rd.Title, rd.TargetDoc, rd.TargetPtr));
                            }
                        }

                        sb.Append("</ul>");
                        this.liHistory.InnerHtml = sb.ToString();
                    }

                    if (this.liLibraryPhone != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<a>" +
                        "<span id=\"libraryBtn\">My Library <i class=\"fa fa-angle-down\"></i></span>" +
                        "</a>");
                        sb.Append("<ul class=\"submenu\">");
                        sb.Append(BuildMyLibraryMenu().ToString());
                        sb.Append("</ul>");
                        liLibraryPhone.InnerHtml = sb.ToString();
                    }

                    if (this.searchDropdown != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<select name=\"did\">");
                        sb.Append(BuildSearchMenu().ToString());
                        sb.Append("</select>");
                        searchDropdown.InnerHtml = sb.ToString();
                    }

                    if (this.searchDropdownMobile != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<select name=\"did\">");
                        sb.Append(BuildSearchMenu().ToString());
                        sb.Append("</select>");
                        searchDropdownMobile.InnerHtml = sb.ToString();
                    }

                    if (this.liHistoryPhone != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<a>" +
                        "<span>History <i class=\"fa fa-angle-down\"></i></span></a>");
                        sb.Append("<ul class=\"submenu\">");

                        if (HomePageNode.RecentDocs.Count == 0)
                        {
                            sb.Append("<li><a href=\"\"><span>None <img src=\"/elements/img/primary-nav-double-arrow.png\" alt=\"\" /></span></a></li>");
                        }
                        else
                        {
                            foreach (Shared.RecentDocument rd in HomePageNode.RecentDocs)
                            {
                                sb.Append(string.Format("<li><a href=\"/content/link/{1}/{2}\"><span>{0} <img src=\"/elements/img/primary-nav-double-arrow.png\" alt=\"\" /></span></a></li>", rd.Title, rd.TargetDoc, rd.TargetPtr));
                            }
                        }

                        sb.Append("</ul>");
                        this.liHistoryPhone.InnerHtml = sb.ToString();
                    }
                }
                catch (Exception ex)
                {
                    //
                    throw;
                }
                

            }

        }
    }
}