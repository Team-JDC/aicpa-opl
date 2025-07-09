using AICPA.Destroyer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OKTAAuth
{
    public class BasePage : System.Web.UI.Page
    {
        const string SESSION_EMAIL_ADDRESS = "emailaddress";
        const string SESSION_RELAY_STATE = "relaystate";

        public string EmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty((string)Session[SESSION_EMAIL_ADDRESS]))
                {
                    return string.Empty;
                }
                return (string)Session[SESSION_EMAIL_ADDRESS];
            }
            set
            {
                Session[SESSION_EMAIL_ADDRESS] = value;
            }
        }

        public string RelayState
        {
            get
            {
                if (string.IsNullOrEmpty((string)Session[SESSION_RELAY_STATE]))
                {
                    return string.Empty;
                }
                return (string)Session[SESSION_RELAY_STATE];
            }
            set
            {
                Session[SESSION_RELAY_STATE] = value;
            }
        }


        public void LogEvent(string module,string method, string name, string message )
        {
            Log.LogEvent(AICPA.Destroyer.User.Event.EventType.Info,1,module, method, name, message);
        }

    }
}