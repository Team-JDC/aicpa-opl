using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using AICPA.Destroyer.User;
using MainUI.Shared;

namespace MainUI
{
    public partial class EthicsDevLogin : System.Web.UI.Page
    {
        private string WEBCONFIG_ALLOW_DEV_LOGIN = "Security_AllowDevLogin";
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


        protected void Page_Load(object sender, EventArgs e)
        {
            string allowDevLoginFlag = ConfigurationManager.AppSettings[WEBCONFIG_ALLOW_DEV_LOGIN];

            Guid userId = Guid.Empty;
            ReferringSite userReferringSite = ReferringSite.Ethics;
            GetCookieValues(ref userId, ref userReferringSite);
            //btnLoginCookie.Visible = (userId != Guid.Empty);
            btnLoginCookie.Visible = false;
            btnLogin.Visible = false;
            User user = new User();
            /*if (userId != Guid.Empty)
            {
                btnLoginCookie.Text = "Login (Cookie: " + user.ActiveUserDalc.GetEmailAddress(userId) + ")";
                btnLogin.Text = "Login (New User)";
            }*/


            if (string.IsNullOrEmpty(allowDevLoginFlag) || allowDevLoginFlag.ToLower() != "true")
            {
                // Development Login is not allowed (as per the web config)
                Response.Redirect("~/SessionExpired.aspx");
            }

            // Now do the login

            string domainList;

            //Guid userId = Guid.Empty;
            //ReferringSite userReferringSite = ReferringSite.Ethics;
            ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);

            domainList = "et-cod";
            Session["D_RETURNURL"] = ConfigurationManager.AppSettings["EthicsLoginUrl"];
            string Url = "~/Ethics.aspx";

            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;

            //IUser user;
            if (userId != Guid.Empty)
            {
                user = new User(userId, userReferringSite);
            }
            else
            {
                user = new User(Guid.NewGuid(), userReferringSite);
            }


            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;
            Session["UserId"] = user.UserId.ToString();

            //get the URL
            Uri MyUrl = Request.Url;
            string URI = MyUrl.AbsoluteUri;
            int pos = URI.LastIndexOf("/");
            Url = URI.Substring(0, pos + 1) + "Ethics.aspx";
            


            //Response.Redirect(Url);
            int seconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["SplashDisplaySeconds"], out seconds))
                seconds = 5;
            Response.AppendHeader("Refresh", string.Format("{0}; URL={1}", seconds, Url));  

           
            
        }

        private void GetCookieValues(ref Guid userId, ref ReferringSite referringSite)
        {
            HttpCookie myCookie = ContextManager.GetSiteCookie(ContextManager.COOKIE_ETHICS);
            if (myCookie != null)
            {
                string ud = myCookie.Values.Get(ContextManager.COOKIE_KEY_USERID);
                string rs = myCookie.Values.Get(ContextManager.COOKIE_KEY_REFERRINGSITE);
                if (!string.IsNullOrEmpty(ud))
                {
                    userId = new Guid(ud);
                }
                if (!Enum.TryParse(rs, out referringSite))
                {
                    //Remove cookie if the userId is set but the referring site is bad
                    if (!string.IsNullOrEmpty(ud))
                        ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);
                    referringSite = ReferringSite.Ethics;
                }
            }

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {            
            string domainList;           
            
            Guid userId = Guid.Empty;
            ReferringSite userReferringSite = ReferringSite.Ethics;
            ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);

            domainList = "et-cod";
            Session["D_RETURNURL"] = ConfigurationManager.AppSettings["EthicsLoginUrl"];            
            string Url = "~/Ethics.aspx";
            
            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;
            
            IUser user;
            if (userId != Guid.Empty)
            {
                user = new User(userId, userReferringSite);
            }
            else
            {
                user = new User(Guid.NewGuid(), userReferringSite);            
            }
            
            
            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;
            Session["UserId"] = user.UserId.ToString();

            //Response.Redirect(Url);
            int seconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["SplashDisplaySeconds"], out seconds))
                seconds = 5;
            //Response.AppendHeader("Refresh", string.Format("{0}; URL={1}", seconds, Url));   
        }

        protected void btnLoginCookie_Click(object sender, EventArgs e)
        {
            string domainList;

            Guid userId = Guid.Empty;
            ReferringSite userReferringSite = ReferringSite.Ethics;
            HttpCookie myCookie = ContextManager.GetSiteCookie(ContextManager.COOKIE_ETHICS);
            //GetCookieValues(ref userId, ref userReferringSite);
            if (myCookie != null)
            {
                string ud = myCookie.Values.Get(ContextManager.COOKIE_KEY_USERID);
                string rs = myCookie.Values.Get(ContextManager.COOKIE_KEY_REFERRINGSITE);
                if (!string.IsNullOrEmpty(ud))
                {
                    userId = new Guid(ud);
                }
                if (!Enum.TryParse(rs, out userReferringSite))
                {
                    //Remove cookie if the userId is set but the referring site is bad
                    if (!string.IsNullOrEmpty(ud))
                        ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);
                    userReferringSite = ReferringSite.Ethics;
                }
            }
            domainList = "et-cod";
            Session["D_RETURNURL"] = ConfigurationManager.AppSettings["EthicsLoginUrl"];            
            string Url = "~/Ethics.aspx";

            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;

            IUser user;
            if (userId != Guid.Empty)
            {
                user = new User(userId, userReferringSite);
            }
            else
            {
                user = new User(Guid.NewGuid(), userReferringSite);
            }


            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;
            Session["UserId"] = user.UserId.ToString();

            //Response.Redirect(Url);
            int seconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["SplashDisplaySeconds"], out seconds))
                seconds = 5;
            Response.AppendHeader("Refresh", string.Format("{0}; URL={1}", seconds, Url));   
        }

    }
}