using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AICPA.Destroyer.User;
using System.ServiceModel.Description;

namespace SingleSignOnHelper
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Authentication : IAuthentication
    {
        const string ERROR_MASK = "Error: {0}";
        public string GetAuthorizationInformationForUser(string userId, string referringSite)
        {
            try{
                userId = userId.Trim();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("email address required.  Invalid email address");
                //create user, so we can grab the current referring site
                User user = new User(userId, referringSite);                
                // if the referring site is other, then the user wasn't created previously, so continue.
                //If they don't equal, throw up error
                //if ((user.ReferringSiteValue == ReferringSite.Other) || (user.ReferringSiteValue.ToString().Equals(referringSite)))
                //{
                //user = new User(userId, referringSite);                
                string sessionId = user.GetSessionId();

                if (sessionId == "-1")
                {
                    sessionId = Guid.NewGuid().ToString();
                    user.UpdateSessionId(sessionId);
                }

                return sessionId;
                //}
                //else
                //{
                //    string msg = string.Format("referring site mismatch.  Current referring site: {0} requested site: {1}.",user.ReferringSiteValue.ToString(), referringSite);
                //    return string.Format(ERROR_MASK, msg);
                //}
            }
            catch(Exception exp)
            {
                return string.Format(ERROR_MASK, exp.Message);
            }
        }
    }
}
