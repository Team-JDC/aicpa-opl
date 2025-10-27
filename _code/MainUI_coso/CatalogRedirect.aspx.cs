#region Directives

using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using ASPENCRYPTLib;

#endregion

namespace MainUI
{
    public partial class CatalogRedirect : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Object objNew = new Object();
            ICryptoManager objCM = new CryptoManager();
            ICryptoContext objContext = objCM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 0,
                                                            objNew);
            ICryptoKey objKey = objContext.GenerateKeyFromPassword("$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk",
                                                                   CryptoAlgorithms.calgSHA,
                                                                   CryptoAlgorithms.calgRC4, 128);
            string userGuid = "{" + Request.QueryString["Guid"] + "}";
            ICryptoBlob objBlob = objKey.EncryptText(userGuid);
            string encryptedGuid = objBlob.Hex;
            string siteCode = "C2B"; //Request.QueryString["SiteCode"];
           
            string domain = Request.QueryString["Domain"];
           
            string fullUrl = ConfigurationManager.AppSettings["c2bAutoLogin"] + "?hidEncPersGUID=" + encryptedGuid +
            "&hidSourceSiteCode=" + siteCode + "&hidURL=Default.aspx";
            string Url = ConfigurationManager.AppSettings["c2bRedirector"] + "?Domain=" + domain +
                                      "&returnurl=" + fullUrl + "&linkurl=" + fullUrl;


            Response.Redirect(Url);
        }
    }
}