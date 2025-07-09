using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.IdentityModel.Claims;
//using System.IdentityModel;
//using System.IdentityModel.Configuration;
//using Microsoft.IdentityModel.Claims;
using System.Security.Claims;
using System.Threading;
using System.Configuration;
using System.Text;

namespace OKTAAuth
{
    public partial class Default : BasePage
    {
        const string RELAYSTATE_PARAMSTR = "relayState";

        protected void Page_Load(object sender, EventArgs e)
        {

            ClaimsPrincipal principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            
            if (principal != null)
            {
                EmailAddress = principal.Identity.Name;                
            }

            //else
            //{
            //    this.name.Value = "Empty";
            //}
            //try
            //{
            //    this.ptest.InnerHtml += "<br/>Identity.Name:" + principal.Identity.Name;
            //}
            //catch
            //{
            //    this.ptest.InnerHtml += "<br/>No IDentity Name<br/>";
            //}
            //try
            //{
            //    this.ptest.InnerHtml += "<br/>Is Authenticated:" + principal.Identity.IsAuthenticated;
            //}
            //catch
            //{
            //    this.ptest.InnerHtml += "<br/>NotAuthenticated<br/>";
            //}

            
            try
            {
                
                StringBuilder sb = new StringBuilder();
                sb.Append("Claims:\r\n");
                foreach (Claim claim in principal.Claims)
                {

                    sb.Append(string.Format("Claim Type:{0} - Claim Value Type: {1} - Claim Value: {2}\r\n",
                        claim.Type, claim.ValueType, claim.Value));

                }

                sb.Append("Request.Params..\r\n");
                if ((Request.Params != null) && Request.Params.AllKeys.Length > 0)
                {                    
                    foreach (string key in Request.Params.AllKeys)
                    {
                        sb.Append(string.Format("Key={0} Value={1} \r\n", key, Request.Params[key]));
                    }
                }
                else
                {
                    sb.Append("None\r\n");
                }

                if (Request.Params.AllKeys.Contains(RELAYSTATE_PARAMSTR))
                {
                    RelayState = Request.Params[RELAYSTATE_PARAMSTR];
                    LogEvent("Default.aspx.cs", "Page_Load", "OKTA Incoming Info", "RelayState: "+RelayState);
                }

                LogEvent("Default.aspx.cs", "Page_Load", "OKTA Incoming Info", sb.ToString());
            }
            catch { }
            Response.Redirect("OPLRedirect.aspx", true);

        }


    }
}