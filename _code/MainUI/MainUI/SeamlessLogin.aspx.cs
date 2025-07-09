using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AICPA.Destroyer.User.Event;
using System.Configuration;
using AICPA.Destroyer.User;
using MainUI.Shared;
using System.Globalization;

namespace MainUI
{
    public partial class SeamlessLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IEvent logEvent = null;
            try
            {
                
                string sessionID = Request.Params["hidSecurityToken"];
                string email = Request.Params["hidUserId"];
                string referringSite = Request.Params["hidReferringSite"].ToUpper();
                string domain = Request.Params["hidDomain"];
                string relayState = Request.Params["hidRelayState"];
                //fix referring site to be 
                referringSite = referringSite[0].ToString() + referringSite.ToLower().Substring(1);

                string incomingParams = string.Format("hidSecurityToken: {0} hidUserId: {1} hidReferringSite: {2} hidDomain: {3} hidRelayState: {4}", sessionID, email, referringSite, domain, relayState);
                logEvent = new Event(EventType.Info, DateTime.Now, 1, "SeemlessLogin.aspx", "Page_Load", "Incoming Params", incomingParams);
                logEvent.Save(false);
                if (string.IsNullOrEmpty(sessionID.Trim()))
                    throw new Exception("SessionID required.");
                if (string.IsNullOrEmpty(email.Trim()))
                    throw new Exception("Email address required.");


                IUser retUser = (IUser)Session[ContextManager.SESSION_USER];
                if (retUser == null)
                {

                    if (referringSite.ToUpper() == "MCGDY" ||
                         referringSite.ToUpper() == "MCGDYASC")
                    {
                        //retUser = new User(email);
                        //if (retUser.ReferringSiteValue.ToString().Equals(referringSite))
                       // {
                            retUser = new User(sessionID, email, referringSite);

                            //if (retUser.GetSessionId().Equals(sessionID))
                            //{
                                retUser.LogOn(Session.SessionID, domain);
                            //}
                            //else
                            //{
                            //    throw new Exception("Invalid Security Token.");
                           // }
                        //}
                        //else
                        //{
                        //    throw new Exception("Referring Site Mismatch");
                        //}
                    }
                    else if (referringSite.ToUpper() == "CEB")
                    {
                        retUser = new User(email);
                        if (retUser.ReferringSiteValue.ToString().Equals(referringSite))
                        {
                            retUser = new User(sessionID, email, referringSite);                       

                            if (retUser.GetSessionId().Equals(sessionID))
                            {
                                retUser.LogOn(Session.SessionID, domain);
                            }
                        }
                        else
                        {
                            throw new Exception("Referring Site Mismatch");
                        }
                    }
                    else if (referringSite.ToUpper() == "CSC")
                    {
                        retUser = new User(email);
                        if (retUser.ReferringSiteValue.ToString().Equals(referringSite))
                        {
                            retUser = new User(sessionID, email, referringSite);

                            if (retUser.GetSessionId().Equals(sessionID))
                            {
                                retUser.LogOn(Session.SessionID, domain);
                            }
                        }
                        else
                        {
                            throw new Exception("Referring Site Mismatch");
                        }
                    }

                    //    domain = (domain == "aag;aag-ara;gasb";
                    //} else if (referringSite.ToUpper() == "MCGDYASC")
                    //{
                    //    domain = "aag;aag-ara;gasb";                    
                    //}
                    //add the user back to the session
                    if (retUser.UserSecurity.Authenticated)
                    {
                        Session[ContextManager.SESSION_USER] = retUser;
                    }
                    else
                    {
                        Session[ContextManager.SESSION_USER] = retUser;
                        Response.Redirect(ConfigurationManager.AppSettings["PAGE_AUTHENTICATIONFAILED"], false);
                    }
                }


                //if (Url.StartsWith("default.aspx", StringComparison.CurrentCultureIgnoreCase))
                //{
                    //Url = "~/" + Url;
                //}
                if (!string.IsNullOrEmpty(relayState))
                {
                    Response.Redirect(relayState, false);
                }
                else
                {
                    Response.Redirect("/default", false);
                }
                

            }
            catch (Exception ex)
            {
                string err = ex.Message;

                //log error
                logEvent = new Event(EventType.Info, DateTime.Now, 1,
                                     ConfigurationManager.AppSettings["PAGE_AUTHENTICATIONFAILED"], "Page_Load", "Error",
                                     err);
                logEvent.Save(false);
                Response.Redirect("/Error?ex=" + ex.Message+"&hideReturnLink=true");
            }


        }
    }
}