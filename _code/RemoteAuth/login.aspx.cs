using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

namespace RemoteAuth
{
    public partial class login : System.Web.UI.Page
    {
        public string strAction = "";
        bool updateReferring = false;
        bool updateDomain = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //btnLogin.Click += new EventHandler(btnLogin_Click);   
                
            }
            strAction = ConfigurationManager.AppSettings["ResourceSeamlessLoginPage"];
            //btnLogin.PostBackUrl = strAction;
            
        }

        protected void btnRequestToken_Click(object sender, EventArgs e)
        {
            
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            
            
        }

        protected void btnLogin_Click1(object sender, EventArgs e)
        {
            string userId = hidUserId.Text;
            string referringSite = ConfigurationManager.AppSettings["DefaultReferringSite"];
            string domain = ConfigurationManager.AppSettings["DefaultDomain"];

            ServiceReference1.AuthenticationClient aClient = new ServiceReference1.AuthenticationClient();
            string securityToken = aClient.GetAuthorizationInformationForUser(userId, referringSite);
            
            hidUserId.Text = userId;
            btnLogin.Text = "Login";
            strAction = ConfigurationManager.AppSettings["SeamlessLoginPage"];
            btnLogin.PostBackUrl = strAction;

           
            Response.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", strAction);
            sb.AppendFormat("<input type='hidden' name='hidUserId' value='{0}'>", hidUserId.Text);
            sb.AppendFormat("<input type='hidden' name='hidSecurityToken' value='{0}'>", securityToken);
            sb.AppendFormat("<input type='hidden' name='hidReferringSite' value='{0}'>", referringSite);
            sb.AppendFormat("<input type='hidden' name='hidDomain' value='{0}'>", domain);
            


            // Other params go here
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");

            Response.Write(sb.ToString());

            Response.End();

        }
    }
}