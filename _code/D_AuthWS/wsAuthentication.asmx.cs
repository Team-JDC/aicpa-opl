using System;
using System.IdentityModel.Claims;
using System.Web.Services;


namespace D_AuthWS
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string TokenCall(string samlToken)
        {
            string message = "";
            try
            {
                string xmlToken;
                xmlToken = samlToken;
                if (xmlToken == null || xmlToken.Equals(""))
                {
                    message = "Token presented was null";
                }
                else
                {
                    
                    Token token = new Token(xmlToken);
                    message = message + token.Claims[ClaimTypes.GivenName];
                    message = message + token.Claims[ClaimTypes.Surname];
                    message = message + token.Claims[ClaimTypes.Email];
                    message = message + token.UniqueID;
                }
                
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }
    }
}
