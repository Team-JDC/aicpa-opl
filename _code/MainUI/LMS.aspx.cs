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
    public partial class LmsLogin : System.Web.UI.Page
    {
      
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            lmsLogin();
            //Response.Redirect("~/LmsDoc.aspx?targetdoc=aag-air&targetptr=aag-air_preface&context=false");
        }

      
        protected void lmsLogin()
        {
            ReferringSite referringSite = ReferringSite.Lms;

            string domainList = "proflit";


            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;


            IUser user= new User(new Guid("357838b2-f783-4ed9-a7ea-889a3c21c3a4"), referringSite);
            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;
       
        }
    }
}