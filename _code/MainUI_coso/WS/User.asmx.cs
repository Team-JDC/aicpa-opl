using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MainUI.Shared;
using AICPA.Destroyer.User;

namespace MainUI.WS
{
    /// <summary>
    /// Summary description for User
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]    
    public class User : System.Web.Services.WebService
    {
         
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "")]
        public LoginResult LogUserIn(string Username, string Password, string Subscription)        
        {
            LoginResult result = new LoginResult() { message = string.Empty, referringSite = string.Empty, userId = string.Empty };
            AICPA.Destroyer.User.IUser user = new AICPA.Destroyer.User.User();
            if (!string.IsNullOrEmpty(Session["default_subscription"] as string))
                Subscription = (Session["default_subscription"] as string);
            Guid userId = user.Login(Username, Password, Session.SessionID, Subscription, ReferringSite.EthicsUser);
            
            if (userId != Guid.Empty)
            {
                ContextManager cm = new ContextManager(Context);
                cm.AddSiteCookie(ContextManager.COOKIE_ETHICS, new Dictionary<string, string>() {
                    {ContextManager.COOKIE_KEY_USERID, userId.ToString()},
                    {ContextManager.COOKIE_KEY_REFERRINGSITE, user.ReferringSiteValue.ToString()}}, 30);

                Session[ContextManager.SESSION_USER] = user;
                Session["UserId"] = userId.ToString();                
                string uri = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                Session["CoreLocation"] = uri;                
                result.message = string.Empty;
                result.userId = userId.ToString();
                result.referringSite = user.ReferringSiteValue.ToString();
            }
            else
            {
                result.message = "Login incorrect. Please try again.";
            }

            return result;
        }

        [WebMethod(true, Description = "")]
        public LoginResult RegisterUser(string Username, string Password, string FirstName, string LastName)
        {
            LoginResult result = new LoginResult() { message = string.Empty, referringSite = string.Empty, userId = string.Empty };
            //IUserDalc userDalc = new UserDalc();
            string errorMessage = string.Empty;
            AICPA.Destroyer.User.User user = new AICPA.Destroyer.User.User();
            Guid userGuid = Guid.NewGuid();
            try
            {
                user.CreateUser(userGuid, ReferringSite.EthicsUser.ToString(), Username, Password, FirstName, LastName);
                user = new AICPA.Destroyer.User.User(userGuid, ReferringSite.EthicsUser);
                user.LogOn(Session.SessionID, "et-cod");
                Session["UserId"] = user.UserId.ToString();
                Session[ContextManager.SESSION_USER] = user;
                result.message = string.Empty;
                result.userId = user.UserId.ToString();
                result.referringSite = user.ReferringSiteValue.ToString();
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }
            return result;
        }

        [WebMethod(true, Description = "")]
        public LoginResult SendUserPassword(string Username)
        {
            LoginResult result = new LoginResult() { message = string.Empty, referringSite = string.Empty, userId = string.Empty };
            IUser user = new AICPA.Destroyer.User.User();

            string returnMessage = user.SendUserPassword(Username);

            if (returnMessage != "")
            {
                result.message = returnMessage;
            }
            else
            {
                result.message = string.Empty;
            }
            return result;
        }
    }

    [Serializable]
    public struct LoginResult
    {
        public string message;
        public string userId;
        public string referringSite;
    }

   
 
}
