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

using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.UI.Portal {

    public partial class DesktopDefault : DestroyerUi {
    
        public DesktopDefault() {
            Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Init(object sender, EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();

            //*********************************************************************
            //
            // Page_Init Event Handler
            //
            // The Page_Init event handler executes at the very beginning of each page
            // request (immediately before Page_Load).
            //
            // The Page_Init event handler below determines the tab index of the currently
            // requested portal view, and then calls the PopulatePortalSection utility
            // method to dynamically populate the left, center and right hand sections
            // of the portal tab.
            //
            //*********************************************************************

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
        
            // Ensure that the visiting user has access to the current page
            if (PortalSecurity.IsInRoles(portalSettings.ActiveTab.AuthorizedRoles) == false) {
                Response.Redirect("~/Admin/AccessDenied.aspx");
            }

            // Dynamically inject a signin login module into the top left-hand corner
            // of the home page if the client is not yet authenticated
            if ((Request.IsAuthenticated == false) && (portalSettings.ActiveTab.TabIndex == 0)) {
                LeftPane.Controls.Add(Page.LoadControl("~/DesktopModules/SignIn.ascx"));
                LeftPane.Visible = true;             
            }

            // Dynamically Populate the Left, Center and Right pane sections of the portal page
            if (portalSettings.ActiveTab.Modules.Count > 0) {

                // Loop through each entry in the configuration system for this tab
				int moduleIndex = 0;
                foreach (ModuleSettings _moduleSettings in portalSettings.ActiveTab.Modules) {

					moduleIndex++;
                
                    Control parent = Page.FindControl(_moduleSettings.PaneName);

                    // If no caching is specified, create the user control instance and dynamically
                    // inject it into the page.  Otherwise, create a cached module instance that
                    // may or may not optionally inject the module into the tree

                    if ((_moduleSettings.CacheTime) == 0) {

                        PortalModuleControl portalModule = (PortalModuleControl) Page.LoadControl(_moduleSettings.DesktopSrc);
                   
                        portalModule.PortalId = portalSettings.PortalId;                                  
                        portalModule.ModuleConfiguration = _moduleSettings;
                   
                        parent.Controls.Add(portalModule);
                    }
                    else {

                        CachedPortalModuleControl portalModule = new CachedPortalModuleControl();
                   
                        portalModule.PortalId = portalSettings.PortalId;                                 
                        portalModule.ModuleConfiguration = _moduleSettings;
 
                        parent.Controls.Add(portalModule);
                    }

                    // Dynamically inject separator break between portal modules
					if(moduleIndex != portalSettings.ActiveTab.Modules.Count)
					{
						parent.Controls.Add(new LiteralControl("<" + "br" + ">"));
					}
                    parent.Visible = true;
                }
            }
        }

		#region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
			//this.Error +=new EventHandler(this.Page_Error);
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
		
		}

		/*private void Page_Error(Object sender, System.EventArgs e )
		{
			HttpContext ctx = HttpContext.Current;
			string eStk = string.Empty;

			Exception exception = ctx.Server.GetLastError ();
			eStk = exception.StackTrace;
		}*/
    }    
}

