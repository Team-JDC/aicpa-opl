#region

using System;
using System.Web.UI;

#endregion

namespace MainUI
{
    public partial class Error : Page
    {
        public bool hideReturnHomeLink = false;
        public bool showLoginLink = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            string defaultErrorMessage = "<p>You are unable to access the AICPA’s Online Professional Library at this time. Please try the following two steps to gain access.</p><ul>" +
                "<li>Clear your cache. For instructions on how to do so, please visit your browser’s Help menu.</li>" +
                "<li>Try to access OPL using a different browser.</li>" +
                "</ul><p>If you are still unable to access the Online Professional Library, we may be experiencing technical difficulties. Please contact our Member Service Center at service@aicpa.org or call 888.777.7077, 9 a.m.–6 p.m. ET, Monday–Friday. We are likely aware of the issue and are working to correct it.</p> ";
            string firmLimitErrorMessage = "FIRM LIMIT EXCEEDED - You have exceeded the maximum number of users for your subscription please try again later.";
            string error = Request["ex"];
            if (error.Contains(firmLimitErrorMessage))
            {
                lblError.Text = firmLimitErrorMessage;
            }
            else if (error.Contains("An Empty Domain string was returned by the authorization service") || error.Contains("The subscription domain string passed from C2B web service call is empty"))
            {
                lblError.Text = "It appears that you do not have a current subscription to the AICPA Online Professional Library. To purchase or renew your online subscription, please visit the <a href=\"http://www.cpa2biz.com/Content/media/Producer_content/generic_template_content/aicparesource/Resource_At_Glance.jsp\">AICPA Store</a>. If you believe this message to be in error, please contact the AICPA Service Center at 1-888-777-7077.";
            }
            else
            {
                lblError.Text = defaultErrorMessage;
            }
            
            if (!IsPostBack)
            {
                if (Request.Params["hideReturnLink"] != null && Request.Params["hideReturnLink"].ToString().Equals("true"))
                    hideReturnHomeLink = true;
                if (Request.Params["showLoginLink"] != null && Request.Params["showLoginLink"].ToString().Equals("true"))
                    showLoginLink = true;
            }
            //Session.Abandon();
        }
    }
}