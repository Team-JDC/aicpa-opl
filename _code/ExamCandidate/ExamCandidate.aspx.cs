using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using ExamsCandidate.Shared;
using System.Configuration;

namespace ExamCandidate
{
    public partial class ExamCandidate : System.Web.UI.Page
    {
        #region Constants

        
        public const string APPSETTINGS_RESOURCESEAMLESSLOGIN_URL = "ResourceSeamlessLoginURL";

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            

            AICPAEncryption enc = new AICPAEncryption();
            string encGuid = enc.Encrypt(Request.Params["Guid"]);
            string hidSourceSiteCode = "Exam";
            string url = "/default.aspx";
            string domain = "exam";


            string action = ConfigurationManager.AppSettings[APPSETTINGS_RESOURCESEAMLESSLOGIN_URL];
            
            Response.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<p>Redirecting to AICPA Online Professional Library</p>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", action);
            sb.AppendFormat("<input type='hidden' name='hidEncPersGUID' value='{0}'>", encGuid);
            sb.AppendFormat("<input type='hidden' name='hidSourceSiteCode' value='{0}'>", hidSourceSiteCode);
            sb.AppendFormat("<input type='hidden' name='hidURL' value='{0}'>", url);
            sb.AppendFormat("<input type='hidden' name='hidDomain' value='{0}'>", domain);

            // Other params go here
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            //LogEvent("OPLRedirect.aspx.cs", "GotoOPL", "RedirectHTML",sb.ToString());
            Response.Write(sb.ToString());

            Response.End();


        }

    }
}