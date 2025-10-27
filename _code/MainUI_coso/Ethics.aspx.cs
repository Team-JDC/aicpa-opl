using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainUI.Shared;
using System.Configuration;
using AICPA.Destroyer.User;

namespace MainUI
{
    public partial class Ethics : System.Web.UI.Page
    {
        #region Context Manager
        private ContextManager _contextManager = null;
        private ContextManager ContextManager
        {
            get
            {
                if (_contextManager == null)
                {
                    _contextManager = new ContextManager(Context);
                }

                return _contextManager;
            }
        }
        #endregion

        public bool ShowSearch
        {
            get
            {
                if (ConfigurationManager.AppSettings["RemoveSearch"] == "true")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ContextManager.HasCurrentUser)
            {
                Response.Redirect("EthicsDevLogin.aspx");
            }
            
            ReferringSiteVar.Text = _contextManager.CurrentUser.ReferringSiteValue.ToString();
        }

        protected string FeedbackEmailAddress
        {
            get
            {
                return ContextManager.FeedbackEmailAddress;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DoLogin(UserNameTextBox.Text, PasswordTextBox.Text, string.Empty);
        }

        private void DoLogin(string Username, string Password, string subscription)
        {
            //string defaultTheme = System.Configuration.ConfigurationManager.AppSettings["CustomThemeLocation"];

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

                //Response.Redirect("~/ethics.aspx");
            }
            else
            {
                ErrorLabel.Text = "Login incorrect please try again.";
            }
        }
    }
}