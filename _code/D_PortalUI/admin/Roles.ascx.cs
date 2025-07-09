using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AICPA.Destroyer.UI.Portal {
    public partial  class Roles : AICPA.Destroyer.UI.Portal.PortalModuleControl {

        int tabIndex = 0;
        int tabId = 0;

        //*******************************************************
        //
        // The Page_Load server event handler on this user control is used
        // to populate the current roles settings from the configuration system
        //
        //*******************************************************

        protected void Page_Load(object sender, System.EventArgs e) {

            // Verify that the current user has access to access this page
            if (PortalSecurity.IsInRoles("Admins") == false) {
                Response.Redirect("~/Admin/EditAccessDenied.aspx");
            }

            if (Request.Params["tabid"] != null) {
                tabId = Int32.Parse(Request.Params["tabid"]);
            }
            if (Request.Params["tabindex"] != null) {
                tabIndex = Int32.Parse(Request.Params["tabindex"]);
            }

            // If this is the first visit to the page, bind the role data to the datalist
            if (Page.IsPostBack == false) {

                BindData();
            }
        }

        //*******************************************************
        //
        // The AddRole_Click server event handler is used to add
        // a new security role for this portal
        //
        //*******************************************************

        protected void AddRole_Click(Object Sender, EventArgs e) {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) Context.Items["PortalSettings"];

            // Add a new role to the database
			RolesDB roles = new RolesDB();
            roles.AddRole(portalSettings.PortalId, "New Role");
        
            // set the edit item index to the last item
            rolesList.EditItemIndex = rolesList.Items.Count;

            // Rebind list
            BindData();
        }

        //*******************************************************
        //
        // The RolesList_ItemCommand server event handler on this page 
        // is used to handle the user editing and deleting roles
        // from the RolesList asp:datalist control
        //
        //*******************************************************

        private void RolesList_ItemCommand(object sender, DataListCommandEventArgs e) {

            RolesDB roles = new RolesDB();
            int roleId = (int) rolesList.DataKeys[e.Item.ItemIndex];
       
            if (e.CommandName == "edit") {

                // Set editable list item index if "edit" button clicked next to the item
                rolesList.EditItemIndex = e.Item.ItemIndex;

                // Repopulate the datalist control
                BindData();
            }
            else if (e.CommandName == "apply") {

                // Apply changes
                String _roleName = ((TextBox) e.Item.FindControl("roleName")).Text;
            
                // update database
                roles.UpdateRole(roleId, _roleName);
            
                // Disable editable list item access
                rolesList.EditItemIndex = -1;

                // Repopulate the datalist control
                BindData();
            }
            else if (e.CommandName == "delete") {

                // update database
                roles.DeleteRole(roleId);

                // Ensure that item is not editable
                rolesList.EditItemIndex = -1;

                // Repopulate list
                BindData();
            }
            else if (e.CommandName == "members") {
        
                // Save role name changes first
                String _roleName = ((TextBox) e.Item.FindControl("roleName")).Text;
                roles.UpdateRole(roleId, _roleName);

                // redirect to edit page
                Response.Redirect("~/Admin/SecurityRoles.aspx?roleId=" + roleId + "&rolename=" + _roleName + "&tabindex=" + tabIndex + "&tabid=" + tabId);
            }        
        }
    
        //*******************************************************
        //
        // The BindData helper method is used to bind the list of 
        // security roles for this portal to an asp:datalist server control
        //
        //*******************************************************

        private void BindData() {

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings) Context.Items["PortalSettings"];
		
            // Get the portal's roles from the database
            RolesDB roles = new RolesDB();
        
            rolesList.DataSource = roles.GetPortalRoles(portalSettings.PortalId);
            rolesList.DataBind();
        }
    
        public Roles() {
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
            this.rolesList.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.RolesList_ItemCommand);

        }
		#endregion
    }
}
