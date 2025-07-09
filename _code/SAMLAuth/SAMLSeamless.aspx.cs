using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using ASPENCRYPTLib;
using dk.nita.saml20.config;
using dk.nita.saml20.identity;
using dk.nita.saml20.Logging;
using ExamsCandidate.Shared;
using System.Text.RegularExpressions;

namespace WebsiteDemo
{
    public partial class SAMLSeamless : Page
    {
        public string hidDomain = "";
        public string hidEmail = "";
        public string hidEncPersGUID = "";
        public string hidSourceSiteCode = "";
        public string hidURL = "";
        public string strAction = "";

        protected void Page_PreInit(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            string strEmail = "";
            string strGuid = "";
            string encryptedGuid = "";
            if (ConfigurationManager.AppSettings["IsTesting"] == "1")
            {
                strEmail = ConfigurationManager.AppSettings["TestEmail"];
            }

            else
            {
                Title = "SAML " + ConfigurationInstance<SAML20FederationConfig>.GetConfig().ServiceProvider.ID;
                //Currently all that is coming into this page is below
                strEmail = Saml20Identity.Current.Name +
                           (Saml20Identity.Current.PersistentPseudonym != null
                                ? " (Pseudonym is " + Saml20Identity.Current.PersistentPseudonym + ")"
                                : String.Empty);
            }

            // create and open a connection object
            conn = new SqlConnection(ConfigurationManager.AppSettings["connDB"]);
            conn.Open();

            var cmd = new SqlCommand("D_SAMLGetUserByEmail", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Email", strEmail));

            // execute the command
            rdr = cmd.ExecuteReader();

            // iterate through results, printing each to console
            if (rdr != null)
            {
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        strGuid = rdr["UserId"].ToString();
                    }
                }
                else
                {
                    strGuid = Guid.NewGuid().ToString();
                }
            }   

            //Encrypt the GUI that is returned from the database.
            try
            {
                AICPAEncryption oDecryptKey = new AICPAEncryption();
                string dbGuid = "{" + strGuid + "}";
                encryptedGuid = oDecryptKey.Encrypt(dbGuid);                
                //For testing purposes, if an unencrypted guid was passed in, we can use that.
                //if (decryptedGuid == "EncryptedPasswordInputIsInvalid" && Regex.IsMatch(encryptedGuid, "[A-Z0-9]{7}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}"))
                //{
                //    decryptedGuid = encryptedGuid;
                //}
                //userGuid = new Guid(decryptedGuid.ToLower());
                //var objNew = new object();
                //ICryptoManager CM = new CryptoManager();
                //ICryptoContext context = CM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 0,objNew);
                //ICryptoKey key = context.GenerateKeyFromPassword("$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk", CryptoAlgorithms.calgSHA, CryptoAlgorithms.calgRC4, 128);
                //string dbGuid = "{" + strGuid + "}";
                //encryptedGuid = oDecryptKey.Encrypt(dbGuid)
                //ICryptoBlob blob = key.EncryptText(dbGuid);
                //encryptedGuid = blob.Hex;
            }
            catch (Exception ex)
            {
                AuditLogging.logEntry(Direction.IN, Operation.ACCESS, "ENCRYPT: " + ex.Message, ex.InnerException.ToString());
              
            }

            //Do an HTTP Post to the site
            try
            {
                strAction = ConfigurationManager.AppSettings["SeamlessLoginPage"];
                hidDomain = ConfigurationManager.AppSettings["Domain"];
                hidEmail = strEmail;
                hidEncPersGUID = encryptedGuid;
                hidSourceSiteCode = ConfigurationManager.AppSettings["SourceSiteCode"];
                hidURL = ConfigurationManager.AppSettings["PostURL"];
            }
            catch (Exception ex)
            {
                AuditLogging.logEntry(Direction.IN, Operation.ACCESS, "POST: " + ex.Message, ex.InnerException.ToString());
                AuditLogging.logEntry(Direction.IN, Operation.ACCESS, "Values(Action: " + strAction + " hidEmail:" + hidEmail + " hidDomain:" + hidDomain + " hidURL:" + hidURL + " hidEncPersGUID:" + hidEncPersGUID + " hidSourceSiteCode:" + hidSourceSiteCode, ex.InnerException.ToString());
                
            }
            AuditLogging.logEntry(Direction.IN, Operation.ACCESS, "Values(Action: " + strAction + " hidEmail:" + hidEmail + " hidDomain:" + hidDomain + " hidURL:" + hidURL + " hidEncPersGUID:" + hidEncPersGUID + " hidSourceSiteCode:" + hidSourceSiteCode + ")", "Completed.");
        }
    }
}