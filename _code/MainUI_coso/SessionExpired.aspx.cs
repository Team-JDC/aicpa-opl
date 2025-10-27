using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainUI.WS;

namespace MainUI
{
    public partial class SessionExpired : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (HttpContext.Current != null) {

                    var contextManager = new MainUI.Shared.ContextManager(HttpContext.Current);

                    AICPA.Destroyer.User.Event.IEvent logEvent;
                    logEvent = new AICPA.Destroyer.User.Event.Event(AICPA.Destroyer.User.Event.EventType.Info
                        , DateTime.Now
                        , 1
                        , "SessionExpired.cs"
                        , "Page_Load"
                        , "Clearing Session"
                        , "Clearing Session.."
                        ,contextManager.CurrentUser);
                        logEvent.Save(false);
                }
                else
                {

                    
                    AICPA.Destroyer.User.Event.IEvent logEvent;
                    logEvent = new AICPA.Destroyer.User.Event.Event(AICPA.Destroyer.User.Event.EventType.Info
                        , DateTime.Now
                        , 1
                        , "SessionExpired.cs"
                        , "Page_Load"
                        , "Clearing Session"
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