using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AICPA.Destroyer.User;
using MainUI.Shared;
using System.Configuration;

namespace MainUI
{
    public partial class aratlogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private const string BOOKMARK_USERID = "28BBC090-47CA-4D9D-82C8-7D79FE368538";
        protected void btnLogin_Click(object sender, EventArgs e)
        {

            ReferringSite referringSite = ReferringSite.Csc;
            string domainList="arat";


            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;


            IUser user;


            user = new User(Guid.NewGuid(), referringSite);
            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;

            Response.Redirect("~/Default.aspx");
        }
    }
}