#region

using System;
using System.Web.UI;

#endregion

namespace MainUI
{
    public partial class Logout : Page
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
            Session.Abandon();
        }
    }
}