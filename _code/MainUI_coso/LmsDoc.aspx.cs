using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MainUI.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;
using System.Xml;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;


namespace MainUI
{
    public partial class LmsDoc : System.Web.UI.Page
    {

        public string targetDoc {get; set;}
        public string targetPtr {get; set;}
        public string context { get; set;}
        public string plain { get; set; }
        public string referrer { get; set; }
        private IEvent logEvent = null;
        
        #region Context Manager
        private ContextManager _contextManager = null;
        private ContextManager ContextManager
        {
            get
            {
                if (_contextManager == null)
                {
                    _contextManager = new ContextManager(Context);
                }

                return _contextManager;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            lmsLogin();
            
            
            if (!ContextManager.HasCurrentUser)
            {
                Response.Redirect("SessionExpired.aspx");
            }

            targetDoc =  Request.Params["targetdoc"];
            targetPtr = Request.Params["targetptr"];
            context  = Request.Params["context"];            

            string calculatedHash = CreateHash(targetDoc, targetPtr, ContextManager.LMSHashSeed);

            //referrer = (Request.UrlReferrer == null ? string.Empty : Request.UrlReferrer.Host);

            if (string.IsNullOrEmpty(targetDoc) || string.IsNullOrEmpty(targetPtr) || string.IsNullOrEmpty(context) || !context.Equals(calculatedHash))
            {
                context = (context == null ? "(Null)" : context);
                List<string> _args = new List<string>();
                //Generate a list of problems.
                if (string.IsNullOrEmpty(targetDoc))
                    _args.Add("targetdoc");
                if (string.IsNullOrEmpty(targetPtr))
                    _args.Add("targetptr");
                if (!context.Equals(calculatedHash))
                    _args.Add("context=" + context +" (calculated: "+calculatedHash+")");

                logEvent = new Event(EventType.Info, DateTime.Now, 1, "LMSDoc.aspx.cs", "Page_Load", "invalid parameters", string.Format("invalid paramters ({0})", string.Join(",", _args.ToArray())));
                logEvent.Save(false);
                Response.Redirect("LMSBadcontent.aspx");
            }
        }

        /// <summary>
        /// Get the second level domain from a given URI.
        /// This returns b.com from http://blog.b.com
        /// </summary>
        /// <param name="URL">Given URL to check</param>
        /// <returns>second level domain</returns>
        private string GetDomain(string URL)
        {
            string hostName = string.Empty;
            string[] hosts = URL.Split(new char[] { '.' });
            // If the host is 
            if (hosts.Length > 2)
                hostName = string.Join(".", hosts, hosts.Length - 2, 2);
            else hostName = URL;

            return hostName.ToLower();
        }


        

        /// <summary>
        /// Check to see if the Domain is in the authorized list.
        /// </summary>
        /// <param name="referringdomain">referring site</param>
        /// <returns>true if site is authorized</returns>
        private bool AuthorizedDomain(string referringdomain)
        {
            try
            {
                if (string.IsNullOrEmpty(referringdomain)) return false;
                string xmlFilePath = ContextManager.XmlFile_ValidSites;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                referringdomain = GetDomain(referringdomain);
                foreach (XmlNode site in xmlDoc.DocumentElement.SelectNodes("//site"))
                {
                    Uri host = new Uri(site.InnerText);
                    if (GetDomain(host.Host).Equals(referringdomain))
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                logEvent = new Event(EventType.Info, DateTime.Now, 5, "LMS Login","Authorize Referrer" , e.Message, "unable to authorize the request");
                logEvent.Save(false);
                return false;
            }
        }

        protected void lmsLogin()
        {
            ReferringSite referringSite = ReferringSite.Lms;

           //string domainList = "proflit";  // this needs to be the targetdoc value on the url.

           string domainList = Request.Params["targetdoc"];


            // clear the current site object if it existed.
            Session[ContextManager.SESSION_SITE] = null;
            IUser user = new User(new Guid("357838b2-f783-4ed9-a7ea-889a3c21c3a4"), referringSite);
            user.LogOn(Session.SessionID, domainList);
            Session[ContextManager.SESSION_USER] = user;

            logEvent = new Event(EventType.Info, DateTime.Now, 1, "LMS_LOGIN", domainList, Request.Params["targetptr"], "lms login sucessfull");
            logEvent.Save(false);
        }

        private string CreateHash(string TargetDoc, string TargetPtr, string seed)
        {

            //create a single string to be encoded
            string encstring = TargetDoc.ToLower() + TargetPtr.ToLower() + seed.ToLower();
            //Instantuate the encoder
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            //Create a byter array big enough for the string
            byte[] unicodeText = new byte[encstring.Length * 2];
            enc.GetBytes(encstring.ToCharArray(), 0, encstring.Length, unicodeText, 0, true);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            //Build the final String by converting each byte into hex and appending it to a string builder
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }       
    }
}