using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Data.SqlClient;

namespace AICPA.Destroyer.UI.Portal {

    public partial class MobileDefault : System.Web.UI.MobileControls.MobilePage {

        public MobileDefault() {
            Page.Init += new System.EventHandler(Page_Init);
        }

        ArrayList authorizedTabs = new ArrayList();

        //*********************************************************************
        //
        // Page_Init Event Handler
        //
        // The Page_Init event handler executes at the very beginning of each page
        // request (immediately before Page_Load).
        //
        // The Page_Init event handler calls the PopulateTabs utility method
        // to insert empty tabs into the tab view. It then determines the tab
        // index of the currently requested portal, and then calls the
        // PopulateTabView utility method to dynamically populate the
        // active portal view.
        //
        //*********************************************************************

        protected void Page_Init(object sender, EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();

            int tabIndex = 0;
            int tabID = 1;

            // Obtain current tab index and tab id settings
            String tabSetting = (String)HiddenVariables["ti"];

            if (tabSetting != null) {

                int comma = tabSetting.IndexOf(',');
                tabIndex = Int32.Parse(tabSetting.Substring(0, comma));
                tabID = Int32.Parse(tabSetting.Substring(comma + 1));
            }

            // Obtain PortalSettings from Current Context
            LoadPortalSettings(tabIndex, tabID);

            // Populate tab list with empty tabs
            PopulateTabStrip();

            // Populate the current tab view
            PopulateTabView(tabIndex);
        }

        //*********************************************************************
        //
        // PopulateTabStrip method
        //
        // The PopulateTabStrip method is used to dynamically create and add
        // tabs for each tab view defined in the portal configuration.
        //
        //*********************************************************************

        private void PopulateTabStrip() {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            for (int i=0;i < portalSettings.MobileTabs.Count; i++) {

                // Create a MobilePortalTab control for the tab,
                // and add it to the tab view.

                TabStripDetails tab = (TabStripDetails)portalSettings.MobileTabs[i];

                if (PortalSecurity.IsInRoles(tab.AuthorizedRoles)) {

                    MobilePortalTab tabPanel = new MobilePortalTab();
                    tabPanel.Title = tab.TabName;

                    TabView.Panes.Add(tabPanel);
                }
            }
        }

        //*********************************************************************
        //
        // PopulateTabView method
        //
        // The PopulateTabView method dynamically populates a portal tab
        // with each module defined in the portal configuration.
        //
        //*********************************************************************

        private void PopulateTabView(int tabIndex) {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            // Ensure that the visiting user has access to the current page
            if (PortalSecurity.IsInRoles(portalSettings.ActiveTab.AuthorizedRoles) == false) {
                Response.Redirect("~/Admin/MobileAccessDenied.aspx");
            }

            // Obtain reference to container mobile tab
            MobilePortalTab view = (MobilePortalTab) TabView.Panes[tabIndex];

            // Dynamically populate the view
            if (portalSettings.ActiveTab.Modules.Count > 0) {

                // Loop through each entry in the configuration system for this tab
                foreach (ModuleSettings _moduleSettings in portalSettings.ActiveTab.Modules) {

                    // Only add the module if it support Mobile devices
                    if (_moduleSettings.ShowMobile) {

                        MobilePortalModuleControl moduleControl = (MobilePortalModuleControl) Page.LoadControl(_moduleSettings.MobileSrc);
                        moduleControl.ModuleConfiguration = _moduleSettings;

                        view.Panes.Add(moduleControl);
                    }
                }
            }
        }

        //*********************************************************************
        //
        // TabView_OnActivate Event Handler
        //
        // The TabView_OnActivate event handler executes when the user switches
        // tabs in the tab view. It calls the PopulateTabView utility
        // method to dynamically populate the newly activated view.
        //
        //*********************************************************************

        protected void TabView_OnTabActivate(Object sender, EventArgs e) {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            int tabIndex = TabView.ActivePaneIndex;
            int tabID = ((TabStripDetails) portalSettings.MobileTabs[tabIndex]).TabId;

            // Store tabindex in a hidden variable to preserve accross round trips
            if (tabIndex != 0) {
                HiddenVariables["ti"] = String.Concat(tabIndex.ToString(), ",", tabID.ToString());
            }
            else {
                HiddenVariables.Remove("ti");
            }

            // Check to see if portal settings need reloading
            LoadPortalSettings(tabIndex, tabID);

            // Populate the newly active tab.
            PopulateTabView(tabIndex);

            // Set the view to summary mode, where a summary of all the modules are shown.
            ((MobilePortalTab)TabView.ActivePane).SummaryView = true;
        }

        //*********************************************************************
        //
        // LoadPortalSettings method
        //
        // LoadPortalSettings is a helper methods that loads portal settings for
        // the selected tab.  It first verifies that the settings haven't already
        // been set within the Global.asax file -- if they are different (in the
        // case that a tab change is made) then the method reloads the appropriate
        // tab data.
        //
        //*********************************************************************

        private void LoadPortalSettings(int tabIndex, int tabId) {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            if ((portalSettings.ActiveTab.TabId != tabId) || (portalSettings.ActiveTab.TabIndex != tabIndex)) {

                HttpContext.Current.Items["PortalSettings"] = new PortalSettings(tabIndex, tabId);
            }
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
        #endregion
    }
}
