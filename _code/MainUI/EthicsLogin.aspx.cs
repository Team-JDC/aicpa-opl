using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainUI.Shared;
using AICPA.Destroyer.User;
using System.Configuration;
using AICPA.Destroyer.Shared;

namespace MainUI
{
    public partial class EthicsLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["disableLogin"] == "true")
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginUrl"]);
            }            

            if (HttpRequestBaseExtensions.IsMobileClient(Request, Response))
                Response.Redirect("~/mlogin.aspx");

            //if (Request.QueryString["autologin"] == "true" && ConfigurationManager.AppSettings["EnbableAutoLogin"] == "true")
            //{
            //    DoLogin(LoadTestHelper.LoadTestHelper.GetUserName(), "1", string.Empty, theme);
            //}

            if (!string.IsNullOrEmpty(Request.QueryString["sub"]))
            {
                Session["default_subscription"] = Request.QueryString["sub"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["user"]) && !string.IsNullOrEmpty(Request.QueryString["pass"]))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["sub"]))
                {
                    DoLogin(Request.QueryString["user"], Request.QueryString["pass"], Request.QueryString["sub"]);
                }
                else
                {
                    DoLogin(Request.QueryString["user"], Request.QueryString["pass"], string.Empty);
                }
            }

            if (!string.IsNullOrEmpty((string)Session["LoginMessage"]))
            {
                LoginMessageLabel.Text = (string)Session["LoginMessage"];
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {            
            DoLogin(UserNameTextBox.Text, PasswordTextBox.Text, string.Empty);
        }

        private void DoLogin(string Username, string Password, string subscription)
        {
            string defaultTheme = System.Configuration.ConfigurationManager.AppSettings["CustomThemeLocation"];

            IUser user = new User();
            if (!string.IsNullOrEmpty(Session["default_subscription"] as string))
                subscription = (Session["default_subscription"] as string);
            Guid userId = user.Login(Username, Password, Session.SessionID, subscription, ReferringSite.EthicsUser);

            if (userId != Guid.Empty)
            {

                Session[ContextManager.SESSION_USER] = user;
                Session["UserId"] = userId.ToString();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                Session["CoreLocation"] = uri.Substring(0, uri.ToLower().IndexOf("/ethicslogin.aspx"));               

                Response.Redirect("~/ethics.aspx");
            }
            else
            {
                ErrorLabel.Text = "Login incorrect please try again.";
            }
        }

        private bool isMobileBrowser()
        {
            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;

            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
                {
                    "midp", "j2me", "avant", "docomo", 
                    "novarra", "palmos", "palmsource", 
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/", 
                    "blackberry", "mib/", "symbian", 
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio", 
                    "SIE-", "SEC-", "samsung", "HTC", 
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx", 
                    "NEC", "philips", "mmm", "xx", 
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java", 
                    "pt", "pg", "vox", "amoi", 
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo", 
                    "sgh", "gradi", "jb", "dddi", 
                    "moto", "iphone"
                };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(s.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}