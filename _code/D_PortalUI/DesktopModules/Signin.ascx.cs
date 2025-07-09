using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

namespace AICPA.Destroyer.UI.Portal {

    public partial  class Signin : AICPA.Destroyer.UI.Portal.PortalModuleControl {


        protected void LoginBtn_Click(Object sender, ImageClickEventArgs e) {

            // Attempt to Validate User Credentials using UsersDB
            UsersDB accountSystem = new UsersDB();
            String userId = accountSystem.Login(email.Text, PortalSecurity.Encrypt(password.Text));

            if ((userId != null) && (userId != "")) {

                // Use security system to set the UserID within a client-side Cookie
                FormsAuthentication.SetAuthCookie(email.Text, RememberCheckbox.Checked);

                // Redirect browser back to originating page
                Response.Redirect(Request.ApplicationPath);
            }
            else {
                Message.Text = "<" + "br" + ">Login Failed!" + "<" + "br" + ">";
            }
        }

        public Signin() {
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
