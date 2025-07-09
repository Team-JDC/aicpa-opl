using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainUI.Shared;
using System.Configuration;

namespace MainUI
{
    public partial class Default2 : System.Web.UI.Page
    {
        //#region Context Manager
        //private ContextManager _contextManager = null;
        //private ContextManager ContextManager
        //{
        //    get
        //    {
        //        if (_contextManager == null)
        //        {
        //            _contextManager = new ContextManager(Context);
        //        }

        //        return _contextManager;
        //    }
        //}
        //#endregion

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
            //if (!ContextManager.HasCurrentUser)
            //{
            //    Response.Redirect("SessionExpired.aspx");
            //}

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultPageRedirect"]))
            //{
            //    Response.Redirect(ConfigurationManager.AppSettings["DefaultPageRedirect"]);
            //}

        }

        protected string FeedbackEmailAddress
        {
            get
            {
                throw new Exception("Not Implemented");
                return string.Empty;
                //return ContextManager.FeedbackEmailAddress;
            }
        }

    }
}