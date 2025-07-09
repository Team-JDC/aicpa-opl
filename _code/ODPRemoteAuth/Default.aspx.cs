using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using ASPENCRYPTLib;
using System.Text;


namespace ODPRemoteAuth
{
    public partial class Default : System.Web.UI.Page
    {
        public string strAction = "";
        bool updateReferring = false;
        bool updateDomain = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
            strAction = ConfigurationManager.AppSettings["SeamlessLoginPage"];
            UpdateControls();

        }

        private void UpdateControls()
        {
            btnLogin.Enabled = !string.IsNullOrEmpty(hidSecurityToken.Text);
        }

        protected void hidReferringSite_Change(object sender, EventArgs e)
        {
            if (updateDomain) return;
            updateReferring = true;
            try
            {
                hidDomain.SelectedIndex = hidReferringSite.SelectedIndex;
            }
            finally
            {
                updateReferring = false;
            }
        }

        protected void hidDomain_Change(object sender, EventArgs e)
        {
            if (updateReferring) return;
            updateDomain = true;
            try
            {
                hidReferringSite.SelectedIndex = hidDomain.SelectedIndex;
            }
            finally
            {
                updateDomain = false;
            }
        }



        protected void btnRequestToken_Click(object sender, EventArgs e)
        {
            string userId = hidUserId.Text;
            ServiceReference2.AuthenticationClient aClient = new ServiceReference2.AuthenticationClient();
            string value = aClient.GetAuthorizationInformationForUser(userId, hidReferringSite.Text);
            hidSecurityToken.Text = value;
            hidUserId.Text = userId;
            strAction = ConfigurationManager.AppSettings["SeamlessLoginPage"];
            UpdateControls();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.PostBackUrl = strAction;
         //   ClientScript.RegisterStartupScript(this.GetType(), "autopostback", ClientScript.GetPostBackEventReference(btnLogin, ""));
            //StringBuilder sbScript = new StringBuilder();
            //sbScript.Append("<script language='JavaScript' type='text/javascript'>\n");
            //sbScript.Append("<!--\n");
            //sbScript.Append(this.GetPostBackEventReference(this, "PBArg") + ";\n");
            //sbScript.Append("// -->\n");
            //sbScript.Append("</script>\n");

            //ScriptManager.RegisterStartupScript(this.GetType(),"AutoPostBackScript", sbScript.ToString());
            //this.RegisterStartupScript("AutoPostBackScript", sbScript.ToString());


        }
    }
}