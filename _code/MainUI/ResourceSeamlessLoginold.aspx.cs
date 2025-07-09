
#region Directives

using System;
using System.Configuration;
using System.Web.UI;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;
using ASPENCRYPTLib;
using MainUI.Shared;
using System.Web;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content.Site;
using System.Text.RegularExpressions;

#endregion

namespace MainUI
{
    /// <summary>
    ///   Summary description for ProflitSeamlessLogin.
    /// </summary>
    public partial class ProflitSeamlessLoginOld : Page
    {

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


        private readonly string _key = ConfigurationManager.AppSettings["KEY"];

        /// <summary>
        ///   Handles the Load event of the Page control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //for logging
            IEvent logEvent = null;
            string resourceBook = string.Empty;
            string resourceName = string.Empty;
            string targetDoc = string.Empty;
            string targetPtr = string.Empty;
            string docId = string.Empty;
            string docType = string.Empty;
            string decryptedUsername = string.Empty;

            try
            {
                string referrer = (Request.UrlReferrer == null ? string.Empty : Request.UrlReferrer.Host);
                string browser = (Request.UserAgent == null ? string.Empty : Request.UserAgent);
                //string test = Request.Headers.ToString();
                string referringSite = Request.Params["hidSourceSiteCode"];
                string Url = Request.Params["hidURL"];
                string Domain = Request.Params["hidDomain"];
                string encryptedGuid = Request.Params["hidEncPersGUID"];
                string encryptedUsername = Request.Params["hidEncUsername"];
                string securityToken = Request.Params["hidSecurityToken"];
                string Email = Request.Params["hidEmail"];                
                bool productLogin = false;
                if (!string.IsNullOrEmpty(Request.QueryString["prod"]))
                {
                   
                    Url = "default.aspx";
                    productLogin = true;
                    if (Request.QueryString["prod"] == "fvs" ||
                        Request.QueryString["prod"] == "pfp")
                    {
                        referringSite = ReferringSite.C2b.ToString();
                    } else referringSite = Request.QueryString["prod"];                    

                    //Handle Ethics issue for links from word
                   
                }
                bool viewCompleteTopic = (!string.IsNullOrEmpty(Request.QueryString["vct"]) && Request.QueryString["vct"] == "1");
                
                string incomingParams = "";
                incomingParams = incomingParams + (string.IsNullOrEmpty(encryptedGuid) ? string.Empty : " hidEncPersGUID: " + encryptedGuid);
                incomingParams = incomingParams + (string.IsNullOrEmpty(encryptedUsername) ? string.Empty : " hidEncUsername: " + encryptedUsername);
                incomingParams = incomingParams + (string.IsNullOrEmpty(securityToken) ? string.Empty : " hidSecurityToken:" + securityToken);
                incomingParams = incomingParams + (string.IsNullOrEmpty(referringSite) ? string.Empty : " hidSourceSiteCode: " + referringSite);
                incomingParams = incomingParams + (string.IsNullOrEmpty(Url) ? string.Empty : " hidURL: " + Url);
                incomingParams = incomingParams + (string.IsNullOrEmpty(Domain) ? string.Empty : " hidDomain: " + Domain);                
                incomingParams = incomingParams + (string.IsNullOrEmpty(Email) ? string.Empty : " hidEmail: " + Email);                
                incomingParams = incomingParams + (string.IsNullOrEmpty(referrer) ? string.Empty : string.Format(" referrer: {0}", referrer));                
                // They are trying to come through with the product ("prod") query string.
                if (productLogin)
                    incomingParams = incomingParams + " Product-Login: " + referringSite;

                logEvent = new Event(EventType.Info, DateTime.Now, 1, "Page_Load", "Page_Load", "Incoming Params", incomingParams.Trim());
                logEvent.Save(false);
                Guid userGuid = Guid.Empty;

                if (referringSite.ToUpper().Equals("COSO"))
                {
                    if (!string.IsNullOrEmpty(encryptedGuid))
                    {
                        userGuid = new Guid(encryptedGuid);
                    }
                    else
                    {
                        /// error out
                        /// 
                        logEvent = new Event(EventType.Error, DateTime.Now, 1, "ResourceSeemlessLogin.aspx", "Page_Load", "Missing Info", "Missing COSO User Guid.");
                        logEvent.Save(false);
                    }
                    encryptedGuid = string.Empty;
                }
                ////for now handle non-encrypted guids - for okta
                //else if (!string.IsNullOrEmpty(encryptedGuid))
                //{
                //    if (!Guid.TryParse(encryptedGuid, out userGuid))
                //    {
                //        userGuid = Guid.Empty;
                //    }
                //    else
                //    {
                //        encryptedGuid = string.Empty;                        
                //    }
                //}

                if(!string.IsNullOrEmpty(encryptedUsername))
                {
                    AICPAEncryption aicpaEnc = new AICPAEncryption();
                    try
                    {
                        decryptedUsername = aicpaEnc.Decrypt(encryptedUsername);
                    }
                    finally
                    {
                        aicpaEnc = null;
                    }
                }

                #region Old EncryptionGuid Code
                //if (!string.IsNullOrEmpty(encryptedGuid))
                //{
                //    //Decrypt the Guid
                //    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AicpaEncryptionKey"]))
                //    {

                //        //decrypt the guid
                //        Object objNew = new Object();
                //        ICryptoManager objCM = new CryptoManager();
                //        ICryptoContext objContext = objCM.OpenContextEx("Microsoft Enhanced Cryptographic Provider v1.0", "", 0,
                //                                                        objNew);
                //        ICryptoKey objKey = objContext.GenerateKeyFromPassword("$Cp8-2-bi5Z_&_RE-P!At4rm_s5yNk",
                //                                                               CryptoAlgorithms.calgSHA,
                //                                                               CryptoAlgorithms.calgRC4, 128);
     
                //        ICryptoBlob objBlob = objCM.CreateBlob();
                //        objBlob.Hex = encryptedGuid;
                //        string decryptedGuid = objKey.DecryptText(objBlob);
                //        decryptedGuid = decryptedGuid.ToLower();
                //        userGuid = new Guid(decryptedGuid);
                //    }
                //    else
                //    {
                //        AICPAEncryption oDecryptKey = new AICPAEncryption();
                //        string decryptedGuid = oDecryptKey.Decrypt(encryptedGuid);
                        
                //        //For testing purposes, if an unencrypted guid was passed in, we can use that.
                //        if (decryptedGuid=="EncryptedPasswordInputIsInvalid" && Regex.IsMatch(encryptedGuid, "[A-Z0-9]{7}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}"))
                //        {
                //            decryptedGuid = encryptedGuid;
                //        }
                //        userGuid = new Guid(decryptedGuid.ToLower());
                //    }
                //}
                #endregion



                #region Hande links back to documents and resources
                
                
                //Get the Referring Site the URL and the Domain

                if (!string.IsNullOrEmpty(Request.QueryString["tdoc"]))
                {
                    targetDoc = Request.QueryString["tdoc"];
                }
                else targetDoc = string.Empty;

                if (!string.IsNullOrEmpty(Request.QueryString["tptr"]))
                {
                    targetPtr = Request.QueryString["tptr"];
                }
                else targetPtr = string.Empty;

                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    docId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(Request.QueryString["type"]))
                    docType = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(Request.QueryString["r_bn"]))
                    resourceBook = Request.QueryString["r_bn"];
                if (!string.IsNullOrEmpty(Request.QueryString["r_rn"]))
                    resourceName = Request.QueryString["r_rn"];
                #endregion


                //get the current user from the session
                IUser retUser = (IUser) Session[ContextManager.SESSION_USER];
                //bool setNewSite = false;
                //If we already have a user, log it off and set it to null so we can login again.
                if (retUser != null && (referringSite.ToUpper() == "FVS" || referringSite.ToUpper() == "PFP"))
                {
                    retUser.LogOff();
                  //  setNewSite = true;
                    retUser = null;
                }
                
                if (retUser == null)
                {
                    //Call the constructor based on the referringSite
                    if (referringSite.Equals("C2B",StringComparison.CurrentCultureIgnoreCase))
                    {

                        if (!string.IsNullOrEmpty(encryptedGuid))
                        {
                            CheckUserGuid(userGuid);
                            retUser = new User(encryptedGuid, userGuid, ReferringSite.C2b);
                            retUser.LogOn(Session.SessionID);
                        }
                        else if (!string.IsNullOrEmpty(decryptedUsername))
                        {   
                            //Log create user record based on email                         
                            retUser = new User(string.Empty, decryptedUsername, ReferringSite.C2b.ToString());                            
                            retUser.LogOn(Session.SessionID);
                        }
                        else
                        {
                            Url = ConfigurationManager.AppSettings[ContextManager.WEBCONFIG_C2B_LOGINURL]; // goto OKTA Login
                        }                  
                    }
                    //else if (referringSite.Equals("CSC",StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    CheckUserGuid(userGuid);
                    //    retUser = new User(userGuid, ReferringSite.Csc);
                    //    retUser.LogOn(Session.SessionID, Domain);
                    //}
                    else if (referringSite.Equals("CEB", StringComparison.CurrentCultureIgnoreCase))
                    {
                        CheckUserGuid(userGuid);
                        retUser = new User(userGuid, ReferringSite.Ceb, Email);
                        retUser.LogOn(Session.SessionID, Domain);
                    }
                    #region Ethics
                    else if (referringSite.ToUpper() == "ETHICS")
                    {
                        Url = "~/ethics.aspx";
                        Guid userId = Guid.Empty;
                        ReferringSite userReferringSite = ReferringSite.Ethics;
                        HttpCookie myCookie = ContextManager.GetSiteCookie(ContextManager.COOKIE_ETHICS);
                        if (myCookie != null)
                        {
                            string ud = myCookie.Values.Get(ContextManager.COOKIE_KEY_USERID);
                            string rs = myCookie.Values.Get(ContextManager.COOKIE_KEY_REFERRINGSITE);
                            if (!string.IsNullOrEmpty(ud))
                            {
                                userId = new Guid(ud);
                            }
                            if (!Enum.TryParse(rs, out userReferringSite))
                            {
                                //Remove cookie if the userId is set but the referring site is bad
                                if (!string.IsNullOrEmpty(ud))
                                    ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);
                                userReferringSite = ReferringSite.Ethics;
                            }
                        }

                        Session["D_RETURNURL"] = ConfigurationManager.AppSettings["EthicsLoginUrl"];
                        string domainList = ConfigurationManager.AppSettings["EthicsDefaultDomains"];
                        if (string.IsNullOrEmpty(domainList))
                        {
                            logEvent = new Event(EventType.Error, DateTime.Now, 1, "ResourceSeemlessLogin.aspx", "Page_Load", "Missing Info", "Missing EthicsDefaultDomains from Web.config");
                            logEvent.Save(false);
                        }

                        if (userId != Guid.Empty)
                        {
                            retUser = new User(userId, userReferringSite);
                        }
                        else
                            retUser = new User(Guid.NewGuid(), ReferringSite.Ethics);
                        retUser.LogOn(Session.SessionID, domainList);
                        Session[ContextManager.SESSION_USER] = retUser;

                        MainUI.WS.Content content = new MainUI.WS.Content();

                        SubscriptionSiteNode ssn = null;
                        try
                        {
                            ssn = content.ResolveContentLink(targetDoc, targetPtr);
                            if (ssn != null && ssn.Restricted)
                            {
                                logEvent = new Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx.cs", "Page_Load", "restricted content", string.Format("restricted content  (targetdoc:{0}, targetptr: {1})", targetDoc, targetPtr));
                                logEvent.Save(false);
                            }
                        }
                        catch
                        {
                            ssn = null;
                        }


                        if (ssn == null || ssn.Restricted)
                        {
                            logEvent = new Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx.cs", "Page_Load", "invalid targetdoc/targetptr", string.Format("invalid targetdoc and/or targetptr  {0}, {1}", targetDoc, targetPtr));
                            logEvent.Save(false);
                        }

                        //Log Valid Access
                        logEvent = new Event(EventType.Info, DateTime.Now, 1, "Sharedoc.aspx.cs", "Page_Load", "valid share requested", string.Format(" targetdoc: {0} targetptr: {1} referrer: {2}", targetDoc, targetPtr, referrer));
                        logEvent.Save(false);

                    }
                    #endregion Ethics
                    #region COSO
                    else if (referringSite.ToUpper() == "COSO")
                    {
                        Url = "~/coso.aspx";
                        Guid userId = Guid.Empty;
                        ReferringSite userReferringSite = ReferringSite.Coso;                        
                        //HttpCookie myCookie = ContextManager.GetSiteCookie(ContextManager.COOKIE_ETHICS);
                        //if (myCookie != null)
                        //{
                        //    string ud = myCookie.Values.Get(ContextManager.COOKIE_KEY_USERID);
                        //    string rs = myCookie.Values.Get(ContextManager.COOKIE_KEY_REFERRINGSITE);
                        //    if (!string.IsNullOrEmpty(ud))
                        //    {
                        //        userId = new Guid(ud);
                        //    }
                        //    if (!Enum.TryParse(rs, out userReferringSite))
                        //    {
                        //        //Remove cookie if the userId is set but the referring site is bad
                        //        if (!string.IsNullOrEmpty(ud))
                        //            ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);
                        //        userReferringSite = ReferringSite.Ethics;
                        //    }
                        //}                      

                        
                        if (string.IsNullOrEmpty(Domain))
                        {
                            logEvent = new Event(EventType.Error, DateTime.Now, 1, "ResourceSeemlessLogin.aspx", "Page_Load", "Missing Info", "Missing Domain string from OPL");
                            logEvent.Save(false);
                        }

                        if (userGuid != Guid.Empty)
                        {
                            retUser = new User(userId, userReferringSite);
                        }

                        retUser.LogOn(Session.SessionID, Domain);
                        Session[ContextManager.SESSION_USER] = retUser;

                        MainUI.WS.Content content = new MainUI.WS.Content();

                        //SubscriptionSiteNode ssn = null;
                        //try
                        //{
                        //    ssn = content.ResolveContentLink(targetDoc, targetPtr);
                        //    if (ssn != null && ssn.Restricted)
                        //    {
                        //        logEvent = new Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx.cs", "Page_Load", "restricted content", string.Format("restricted content  (targetdoc:{0}, targetptr: {1})", targetDoc, targetPtr));
                        //        logEvent.Save(false);
                        //    }
                        //}
                        //catch
                        //{
                        //    ssn = null;
                        //}


                        //if (ssn == null || ssn.Restricted)
                        //{
                        //    logEvent = new Event(EventType.Info, DateTime.Now, 1, "ResourceSeamlessLogin.aspx.cs", "Page_Load", "invalid targetdoc/targetptr", string.Format("invalid targetdoc and/or targetptr  {0}, {1}", targetDoc, targetPtr));
                        //    logEvent.Save(false);
                        //}

                        //Log Valid Access
                        //logEvent = new Event(EventType.Info, DateTime.Now, 1, "Sharedoc.aspx.cs", "Page_Load", "valid share requested", string.Format(" targetdoc: {0} targetptr: {1} referrer: {2}", targetDoc, targetPtr, referrer));
                        //logEvent.Save(false);
                    }
                    #endregion COSO
                    #region FVS & PFP
                    //else if ((referringSite.ToUpper() == "FVS") || (referringSite.ToUpper() == "PFP"))
                    //{
                    //    string domainList = "";
                    //    string siteList = "";
                    //    //Default Domain List 
                    //    if (referringSite.ToUpper() == "FVS")
                    //    {
                    //        domainList = ConfigurationManager.AppSettings["FvsDefaultDomains"];
                    //        siteList = ConfigurationManager.AppSettings["FvsSiteList"];
                    //    }
                    //    else
                    //    {
                    //        domainList = ConfigurationManager.AppSettings["PfpDefaultDomains"];
                    //        siteList = ConfigurationManager.AppSettings["PfpSiteList"];
                    //    }

                    //    if (string.IsNullOrEmpty(domainList))
                    //    {
                    //        logEvent = new Event(EventType.Error, DateTime.Now, 1, "ResourceSeemlessLogin.aspx", "Page_Load", "Missing Info", "Missing DefaultDomains from Web.config for either FVS or PFP");
                    //        logEvent.Save(false);
                    //    }

                    //    if (string.IsNullOrEmpty(siteList))
                    //    {
                    //        logEvent = new Event(EventType.Error, DateTime.Now, 1, "ResourceSeemlessLogin.aspx", "Page_Load", "Missing Info", "Missing SiteList from Web.config for either FVS or PFP");
                    //        logEvent.Save(false);
                    //    }

                    //    string[] siteListArr = siteList.Split(',');

                    //    bool validSite = false;
                    //    foreach (string site in siteListArr)
                    //    {
                    //        if (referrer.ToLower().Contains(site.Trim().ToLower()))
                    //            validSite = true;
                    //    }

                    //    if (!validSite)
                    //    {
                    //        if (!browser.ToUpper().Contains("MSIE"))
                    //        {
                    //            throw new Exception("Error: Unauthorized referring site.");
                    //        }
                    //    }

                    //    Session["D_FEATURES"] = ConfigurationManager.AppSettings["FvsRemovedFeatureIds"];

                    //    //Create Anonymous user.
                    //    Guid newGuid = Guid.NewGuid();
                    //    if (referringSite.ToUpper() == "FVS")
                    //    {
                    //        retUser = new User(newGuid, ReferringSite.Fvs);
                    //    }
                    //    else
                    //    {
                    //        retUser = new User(newGuid, ReferringSite.Pfp);
                    //        Session["D_FEATURES"] = ConfigurationManager.AppSettings["PFPRemovedFeatureIds"];
                    //    }
                    //    retUser.LogOn(Session.SessionID, domainList);

                    //    Session[ContextManager.SESSION_USER] = retUser;

                    //    //If the user was already logged in, we need to create a new site to make sure we have the right domain string.
                    //    if (setNewSite)
                    //    {
                    //        SiteStatus status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
                    //        Session[ContextManager.SESSION_SITE] = new Site(retUser, status);
                    //    }
                    //}
                    #endregion FVS & PFP
                    #region Exams/etc
                    else
                    {
                        retUser = new User(userGuid, ReferringSite.Exams);
                        retUser.LogOn(Session.SessionID, Domain);
                    }
                    #endregion Exams/etc
                    //add the user back to the session
                    if (retUser.UserSecurity.Authenticated)
                    {
                        Session[ContextManager.SESSION_USER] = retUser;
                    }
                    else
                    {
                        Session[ContextManager.SESSION_USER] = retUser;
                        Response.Redirect(ConfigurationManager.AppSettings["PAGE_AUTHENTICATIONFAILED"], false);
                    }
                }
    
                if (referringSite.ToUpper() == "ETHICS")
                {
                    Url = "~/ethics.aspx";
                }


                bool docRedirect = false;
                if (targetDoc != string.Empty) {
                    Url = Url + "?targetdoc=" + targetDoc + "&targetptr=" + targetPtr;
                    docRedirect = true;
                }
                else if ((docId != string.Empty) && (docType != string.Empty)) {
                    Url = Url + "?id=" + docId + "&type=" + docType;
                    docRedirect = true;
                }


                if ((docRedirect) && (viewCompleteTopic))
                    Url = Url + "&vct=1";
                if (Url.StartsWith("default", StringComparison.CurrentCultureIgnoreCase))
                {
                    Url = "~/" + Url;
                }   

                if (referringSite == "ethics" && browser.Contains("ms-office"))
                {
                    Url = "OfficeFix.aspx" + Request.Url.Query;
                    Response.Redirect(Url,false);

                }
                else
                {
                    Response.Redirect(Url, false);
                }


            }
            catch (Exception ex)
            {
                string err = ex.Message;

                //log error
                logEvent = new Event(EventType.Info, DateTime.Now, 1,
                                     ConfigurationManager.AppSettings["PAGE_AUTHENTICATIONFAILED"], "Page_Load", "Error",
                                     err+":"+ex.ToString());
                logEvent.Save(false);
                Response.Redirect("~/Error.aspx?ex=" + ex.Message + "&hideReturnLink=true");
            }
        }

        private void CheckUserGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                throw new Exception("hidEncPersGUID missing");

        }

        #region Web Form Designer generated code

        /// <summary>
        ///   Raises the <see cref = "E:System.Web.UI.Control.Init" /> event to initialize the page.
        /// </summary>
        /// <param name = "e">An <see cref = "T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}