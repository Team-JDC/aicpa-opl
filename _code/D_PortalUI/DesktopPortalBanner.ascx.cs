using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace AICPA.Destroyer.UI.Portal {

    public partial  class DesktopPortalBanner : System.Web.UI.UserControl {
        protected System.Web.UI.WebControls.Label WelcomeMessage;

        public int          tabIndex;
        public bool         ShowTabs = true;
        protected String    LogoffLink = "";
		private int			initialDocument = 2; //2 for toc display : 3 for document with hidden toc

        protected void Page_Load(object sender, System.EventArgs e) {

			if(Page.IsPostBack)
			{
				if(this.topSearchTxt.Text != "" || this.topSearchTxt.Text != string.Empty)
				{
					string goTo = "DesktopDefault.aspx?tabindex=3&tabid=5&Search="+this.topSearchTxt.Text;
					Response.Redirect(goTo);
				}
			}

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            // Dynamically Populate the Portal Site Name
            siteName.Text = portalSettings.PortalName;

            // If user logged in, customize welcome message
            if (Request.IsAuthenticated == true) {
        
                //WelcomeMessage.Text = "Welcome " + Context.User.Identity.Name + "! <" + "span class=Accent" + ">|<" + "/span" + ">";

                // if authentication mode is Cookie, provide a logoff link
                if (Context.User.Identity.AuthenticationType == "Forms") {
                    LogoffLink = "<" + "span class=\"Accent\">|</span>\n" + "<" + "a href=" + Global.GetApplicationPath(Request) + "/Admin/Logoff.aspx class=SiteLink> Logoff" + "<" + "/a>";
                }
            }

            // Dynamically render portal tab strip
            if (ShowTabs == true) {

                tabIndex = portalSettings.ActiveTab.TabIndex;
				
				//jjs: this is for situations where all you have passed in is the tab id, not the index
				if(tabIndex == -1)
				{
					//find the index of the specified tab
					int tabId = portalSettings.ActiveTab.TabId;
					for(int i = 0; i < portalSettings.DesktopTabs.Count; i++)
					{
						TabStripDetails tab = (TabStripDetails)portalSettings.DesktopTabs[i];
						if(tab.TabId == tabId)
						{
							tabIndex = i;
							break;
						}
					}

					//if not found, just set the index to zero so we dont mess anything up with our -1
					if(tabIndex == -1)
					{
						tabIndex = 0;
					}

				}

                // Build list of tabs to be shown to user                                   
                ArrayList authorizedTabs = new ArrayList();
                int addedTabs = 0;

                for (int i=0; i < portalSettings.DesktopTabs.Count; i++) {
            
                    TabStripDetails tab = (TabStripDetails)portalSettings.DesktopTabs[i];

					if (PortalSecurity.IsInRoles(tab.AuthorizedRoles) && i != 1) { 
                        authorizedTabs.Add(tab);
                    }

                    if (addedTabs == tabIndex) {
                        tabs.SelectedIndex = addedTabs;
                    }

					//dam: When customer select to display TOC
					if(Session["TocVisibility"] != null && tab.TabId == 3)
					{
						if(Session["TocVisibility"].ToString() == "true")
						{
							tab.TabId = 2;
						}
						else
						{
							tab.TabId = 3;
						}
					}
					else if(Session["TocVisibility"] == null && tab.TabId == 3)
					{
						tab.TabId = initialDocument;
					}

                    addedTabs++;
                }          

                // Populate Tab List at Top of the Page with authorized tabs
                tabs.DataSource = authorizedTabs;
                tabs.DataBind();
				//tabs.Attributes.Add("onclick","showWaiting(false)");
				topSearchBtn.Attributes.Add("onclick","showWaiting(false)");
            }
        }
        
        public DesktopPortalBanner() {
            this.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Init(object sender, EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
        }

		#region Web Form Designer generated code
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

		}
		#endregion

		protected void topSearchSubmit(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string goTo = "DesktopDefault.aspx?tabindex=3&tabid=5&Search="+this.topSearchTxt.Text;
			Response.Redirect(goTo);
		}

		private void tabs_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
			DataList dl = tabs;

			if(tabs.SelectedIndex != e.Item.ItemIndex)
			{
				e.Item.Attributes.Add("onclick","showWaiting(false)");
			}
		}

		public string getOnClickEvent(int id)
		{
			string str = string.Empty;
			
			if(id != tabs.SelectedIndex)
			{
				str = "onClick='showWaiting(false);'";
			}

			return str;
		}
    }
}
