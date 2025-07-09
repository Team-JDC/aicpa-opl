using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AICPA.Destroyer.UI.Portal {

    public partial  class QuickLinks : AICPA.Destroyer.UI.Portal.PortalModuleControl {


        protected String linkImage = "";

        //*******************************************************
        //
        // The Page_Load event handler on this User Control is used to
        // obtain a DataReader of link information from the Links
        // table, and then databind the results to a templated DataList
        // server control.  It uses the AICPA.Destroyer.UI.Portal.LinkDB()
        // data component to encapsulate all data functionality.
        //
        //*******************************************************

        protected void Page_Load(object sender, System.EventArgs e) {

            // Set the link image type
            if (IsEditable) {
                linkImage = "~/images/edit.gif";
            }
            else {
                linkImage = "~/images/navlink.gif";
            }

            // Obtain links information from the Links table
            // and bind to the list control
            AICPA.Destroyer.UI.Portal.LinkDB links = new AICPA.Destroyer.UI.Portal.LinkDB();

            myDataList.DataSource = links.GetLinks(ModuleId);
            myDataList.DataBind();
        
            // Ensure that only users in role may add links
            if (PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedEditRoles)) {

                EditButton.Text="Add Link";
                EditButton.NavigateUrl = "~/DesktopModules/EditLinks.aspx?mid=" + ModuleId.ToString();
            }
        }
        
		protected string ChooseURL(string itemID, string modID, string URL)
		{
			if(IsEditable)
				return "~/DesktopModules/EditLinks.aspx?ItemID=" + itemID.ToString() + "&mid=" + modID;
			else
				return URL;
		}

        public QuickLinks() {
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
    }
}
