using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OKTAAuth
{
    public partial class OPLRedirect : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GotoOPL(EmailAddress, ConfigurationManager.AppSettings["ReferringSite"], RelayState);
        }
        /*
        *
function deleteAllCookies() {
var cookies = document.cookie.split(";");
        alert('test');
for (var i = 0; i < cookies.length; i++) {
var cookie = cookies[i];
var eqPos = cookie.indexOf("=");
var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
}
}
         function doStuff() {
             deleteAllCookies();
             document.forms["form"].submit();
         }
         * 
         * 
         * 
         */



        private string GetFullJavascript()
        {
            string full = "function deleteAllCookies() {" +
                            "var cookies = document.cookie.split(\";\");\r\n " +
                            
                            "for (var i = 0; i < cookies.length; i++) { \r\n " +
                            "var d = new Date();" +
                            "d.setDate(d.getDate() - 1); \r\n" +
                            "var expires = \";expires=\" + d; \r\n " +
                            "var cookie = cookies[i]; \r\n " +
                            "var eqPos = cookie.indexOf(\"=\"); \r\n " +
                            "var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie; \r\n " +
                            "var value = \"\"; \r\n"+
                //"document.cookie = name + \"=;expires=Thu, 01 Jan 1970 00:00:00 GMT\"; \r\n " +
                            "document.cookie = name + \"=\" + value + expires + \"; path=/\";"+
                            "}\r\n " +
                            "}\r\n " +
                            "function doStuff() { \r\n"+
                            "deleteAllCookies(); \r\n"+
                            "document.forms[\"form\"].submit(); \r\n"+
                            "}\r\n";
            return full;
        }

        

        private void GotoOPL(string email, string referringSite, string relayState)
        {
            string userId = email;
            string securityToken = string.Empty;
            string action = string.Empty;
            LogEvent("Default.aspx.cs", "GOTOOPL", "OKTA Info", string.Format("UserId: {0}", userId));
            AICPA.Destroyer.Shared.AICPAEncryption aicpaEnc = new AICPA.Destroyer.Shared.AICPAEncryption();
            string encryptedEmail = aicpaEnc.Encrypt(email);

            string strAction = ConfigurationManager.AppSettings["SeamlessLoginPage"];
            
            LogEvent("OPLRedirect.aspx.cs", "GotoOPL", "Redirect Information",
                string.Format("UserId: {0} EncryptedUserId: {1} ReferringSite: {2} Action: {3} RelayState:{4}", userId, encryptedEmail, referringSite, strAction, relayState));


            string[] myCookies = Request.Cookies.AllKeys;
            string cookieList = string.Empty;
            foreach (string cookie in myCookies)
            {
                //LogEvent("OPLRedirect.aspx.cs", "GotoOPL", "Removing Cookie", string.Format("Cookie: {0} ", cookie));

                //cookieList += cookie + ";";
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }



            Response.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append(@"<script>");
            sb.Append(GetFullJavascript());
            sb.Append("</script></head>");
            sb.AppendFormat(@"<body onload='doStuff();'>");
            sb.AppendFormat("<p>Redirecting to AICPA Online Professional Library</p>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", strAction);
            sb.AppendFormat("<input type='hidden' name='hidEncUsername' value='{0}'>", encryptedEmail);
            sb.AppendFormat("<input type='hidden' name='hidSourceSiteCode' value='{0}'>", referringSite);
            sb.AppendFormat("<input type='hidden' name='hidURL' value='{0}'>", relayState);

            // Other params go here
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            LogEvent("OPLRedirect.aspx.cs", "GotoOPL", "RedirectHTML",sb.ToString());
            Response.Write(sb.ToString());

            Response.End();
            Session.Abandon();



        }
    }
}