using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MainUI
{
    public partial class LoginPage : System.Web.UI.Page
    {
        public const string WEBCONFIG_C2B_LOGINURL = "C2bLoginUrl";

        protected void Page_Load(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings[WEBCONFIG_C2B_LOGINURL];
            Response.Redirect(url, false);
        }
    }
}