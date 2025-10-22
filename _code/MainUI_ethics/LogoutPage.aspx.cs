using System;
using System.Web;
using System.Web.UI;

namespace MainUI
{
    public partial class LogoutPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                if (System.Web.HttpContext.Current != null)
                {

                    var contextManager = new MainUI.Shared.ContextManager(System.Web.HttpContext.Current);

                    AICPA.Destroyer.User.Event.IEvent logEvent;
                    logEvent = new AICPA.Destroyer.User.Event.Event(AICPA.Destroyer.User.Event.EventType.Info
                        , DateTime.Now
                        , 1
                        , "LogoutPage.aspx.cs"
                        , "Page_Load"
                        , "Logging Out"
                        , "Clearing Session.."
                        , contextManager.CurrentUser);
                    logEvent.Save(false);
                }
                else
                {


                    AICPA.Destroyer.User.Event.IEvent logEvent;
                    logEvent = new AICPA.Destroyer.User.Event.Event(AICPA.Destroyer.User.Event.EventType.Info
                        , DateTime.Now
                        , 1
                        , "LogoutPage.aspx.cs"
                        , "Page_Load"
                        , "Logging Out"
                        , "Clearing Session .. Unknown User");
                    logEvent.Save(false);
                }
            }
            catch
            {
                //
            }
            EndUserSession();

            //TempLiteral.Text = cookieList;
        }

        private void EndUserSession()
        {
            Session.Abandon();
            string[] myCookies = Request.Cookies.AllKeys;
            string cookieList = string.Empty;
            foreach (string cookie in myCookies)
            {
                //cookieList += cookie + ";";
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
        }
    }
}