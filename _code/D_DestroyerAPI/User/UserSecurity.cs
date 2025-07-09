#region Directives

using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using AICPA.Destroyer.cpa2biz.AuthService.com;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.User.Firm;
using AICPA.Destroyer.netForumServices;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  The UserSecurity class holds the security context of a user.  It indicates if a 
    ///  user is authenticated and what access a user has.
    ///</summary>
    public class UserSecurity : DestroyerBpc, IUserSecurity
    {
        #region Constants

        #region Private Constants

        #endregion Private Constants

        #region Public Constants

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string CONFIG_SESSION_POLL_INTERVAL = "SessionIdCheckTimeInSeconds";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string CONFIG_SESSION_TIMEOUT_VALUE = "ApiSessionTimeoutInSeconds";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string CONFIG_DEFAULTACTIONPERMISSION = "DefaultActionPermission";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string CONFIG_WEB_REFERENCE_URL = "c2bWebReferenceUrl";

        /// <summary>
        /// Mask for Configuration setting for a specific Referring Site Maximum Users
        /// </summary>
        public const string CONFIG_MAX_USERS_MASK = "{0}MaxUsers";


        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_ERROR_INVALID_SESSIONID =
            "DUPLICATE USER - Your session has been terminated due to multiple logins with your username and password. Please try again later.  If you believe this message was received in error, please contact support at service@aicpa.org.";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_ERROR_NO_C2B_USERID =
            "ACCESS DENIED - We are unable to verify your username and password. Please close your browser and try to login again. If you receive this message again, please contact support at service@aicpa.org.";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_ERROR_INVALID_USERGUID =
            "ACCESS DENIED - We are unable to verify your username and password. Please close your browser and try to login again. If you receive this message again, please contact support at service@aicpa.org.";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_ERROR_VALIDATION_FAILURE =
            "System exception encountered. Description – {0}.  Please contact support at service@aicpa.org.";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_C2B_SUBSCRIPTION_SERVICE_FAILURE =
            "ACCESS DENIED - We are unable to verify your username and password. [Reason {0}]. Please close your browser and try to login again. If you receive this message again, please contact support at service@aicpa.org.";

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED =
            "FIRM LIMIT EXCEEDED - You have exceeded the maximum number of users for your subscription please try again later.";

        public const string AUTHENTICATION_ERROR_NO_C2B_DOMAIN_PASSED = "The subscription domain string passed from C2B web service call is empty.";



        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string AUTHENTICATION_MCGDY_SUBSCRIPTION_SERVICE_FAILURE =
            "ACCESS DENIED - We are unable to verify your username and password. [Reason {0}]. Please close your browser and try to login again. If you receive this message again, please contact support at service@aicpa.org.";

        /// <summary>
        /// 
        /// </summary>
        public const string AUTHENTICATION_ERROR_NO_DOMAIN_PASSED = "The subscription domain string passed from web service call is empty.";


        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string MODULE_USERSECURITY = "UserSecurity";

        #endregion Public Constants

        #endregion Constants

        #region Constructors

        #region Internal Constructors

        ///<summary>
        ///  Create a user security object by providing a reference to a user and a session.  
        ///  The user object can only be accessed internally.
        ///</summary>
        ///<param name = "user">The user who will own the security context.</param>
        ///<param name = "sessionId">The unique sessionId of the user.</param>
        ///<param name = "domain">The domain string of the user.  The domain may be null.</param>
        internal UserSecurity(IUser user, string sessionId, string domain)
        {
            this.domain = domain;
            this.user = user;
            this.sessionId = sessionId;
        }

        ///<summary>
        ///  Create a user security object by providing a reference to a user and a session.  
        ///  The user object can only be accessed internally.
        ///</summary>
        ///<param name = "user">The user who will own the security context.</param>
        ///<param name = "sessionId">The unique sessionId of the user.</param>
        internal UserSecurity(IUser user, string sessionId)
        {
            this.user = user;
            this.sessionId = sessionId;
        }

        #endregion Internal Constructors

        #endregion Constructors

        #region Properties

        #region IUserSecurity Properties

        private readonly string sessionId;
        private readonly IUser user;
        private int actionPermission;

        private object authenticated;

        private string authenticationError;

        private string[] bookName;
        private string domain;
        private string email;

        private IFirmCollection firmCollection;

        ///<summary>
        ///  The user that the UserSecurity pertains to.
        ///</summary>
        public IUser User
        {
            get { return user; }
        }

        ///<summary>
        ///  Attempts to authenticate the user if it has not been previously attempted, otherwise will
        ///  only check if the user's session is still valid.
        ///</summary>
        public bool Authenticated
        {
            get
            {
                if (authenticated == null)
                {
                    try
                    {
                        AuthenticateUser();
                    }
                    catch (Exception e)
                    {
                        authenticated = false;
                        authenticationError = e.Message;
                    }
                }
                if (authenticated != null && (bool) authenticated && IsSessionIdCheckTimeExpired() &&
                    !IsSessionIdValid())
                {
                    authenticated = false;
                    authenticationError = AUTHENTICATION_ERROR_INVALID_SESSIONID;
                }
                return (bool) authenticated;
            }
        }

        ///<summary>
        ///  If a user failed to authenticate, the reason will be stored in this property.
        ///</summary>
        public string AuthenticationError
        {
            get { return authenticationError; }
        }

        ///<summary>
        ///  An array of book names that the user has access to. The User must be authenticated to access this property or an error will be thrown.
        ///</summary>
        public string[] BookName
        {
            get
            {
                if (!Authenticated)
                {
                    throw new UserNotAuthenticated(AuthenticationError);
                }

                if (string.IsNullOrWhiteSpace(Domain))
                {
                    throw new SecurityException(AUTHENTICATION_ERROR_NO_C2B_DOMAIN_PASSED);
                }

                if (bookName == null)
                {
                    bookName = ActiveUserSecurityDalc.GetSubscriptionBookNames(Domain, DOMAIN_SUBSCRIPTIONCODESEPCHAR);
                }

                return bookName;
            }
        }

        ///<summary>
        ///  A collection of firms that the user belongs to. The User must be authenticated to access this property or an error will be thrown.
        ///</summary>
        public IFirmCollection FirmCollection
        {
            get
            {
                if (!Authenticated)
                {
                    throw new UserNotAuthenticated(AuthenticationError);
                }
                return firmCollection ?? (firmCollection = new FirmCollection());
            }
        }

        ///<summary>
        ///  The domain string provided by the c2b web service or other referring site
        ///  indicating the subscriptions that a user has access to. The User must be authenticated to access this property or an error will be thrown.
        ///</summary>
        public string Domain
        {
            get
            {
                if (!Authenticated)
                {
                    throw new UserNotAuthenticated(AuthenticationError);
                }
                return domain;
            }
        }

        ///<summary>
        ///  The current session id of the user.
        ///</summary>
        public string SessionId
        {
            get { return sessionId; }
        }

        ///<summary>
        ///  An integer representing the actions that a user has access to. The User must be authenticated to access this property or an error will be thrown.
        ///</summary>
        public int ActionPermission
        {
            get
            {
                if (!Authenticated)
                {
                    throw new UserNotAuthenticated(AuthenticationError);
                }
                return actionPermission;
            }
        }

        ///<summary>
        ///  The email address of the user. The User must be authenticated to access this property or an error will be thrown.
        ///</summary>
        public string Email
        {
            get
            {
                if (!Authenticated)
                {
                    throw new UserNotAuthenticated(AuthenticationError);
                }
                if (email == null)
                {
                    SetDataWarehouseInformation();
                }
                return email;
            }
        }

        #endregion IUserSecurity Properties

        #region Private Properties

        ///<summary>
        ///  Returns the UserDs for the UserSecurity object.
        ///</summary>
        private readonly UserDs activeUserDs = new UserDs();

        private UserSecurityDalc activeUserSecurityDalc;

        ///<summary>
        ///  Returns the UsersDataTable for the UserSecurity Object.
        ///</summary>
        private UserDs.UsersDataTable activeUsersDataTable;

        ///<summary>
        ///  Returns the UsersRow of the current UserSecurity object.
        ///</summary>
        private UserDs.UsersRow activeUsersRow;

        private int maximumAllowedConcurrentUsers;
        private DateTime sessionIdCheckTime = DateTime.MinValue;
        private int sessionTimeout = -1;

        ///<summary>
        ///  If an instance of the UserSecurityDalc has not been created, then 
        ///  this property creates the instance and returns it, otherwise it just
        ///  returns and instance.
        ///</summary>
        private UserSecurityDalc ActiveUserSecurityDalc
        {
            get
            {
                if (activeUserSecurityDalc == null)
                {
                    activeUserSecurityDalc = new UserSecurityDalc();
                }
                return activeUserSecurityDalc;
            }
        }

        ///<summary>
        ///  Returns the maximum number of allowed concurrent users in any
        ///  given firm for this user.
        ///</summary>
        private int MaximumAllowedConcurrentUsers
        {
            get { return maximumAllowedConcurrentUsers; }
        }

        /// <summary>
        /// </summary>
        private UserDs ActiveUserDs
        {
            get { return activeUserDs; }
        }

        /// <summary>
        /// </summary>
        private UserDs.UsersDataTable ActiveUsersDataTable
        {
            get { return activeUsersDataTable; }
        }

        /// <summary>
        /// </summary>
        private UserDs.UsersRow ActiveUsersRow
        {
            get { return activeUsersRow; }
        }

        ///<summary>
        ///  The session timeout value in seconds.  This is how long before a user
        ///  will go inactive if they haven't checked the Authenticated property 
        ///  recently.  The value comes from the configuration file.
        ///</summary>
        private int SessionTimeout
        {
            get
            {
                if (sessionTimeout == -1)
                {
                    sessionTimeout = Convert.ToInt16(ConfigurationSettings.AppSettings[CONFIG_SESSION_TIMEOUT_VALUE]);
                }
                return sessionTimeout;
            }
        }

        #endregion Private Properties

        #region Internal Properties

        private string c2bEncryptedGuid = string.Empty;
        private Guid c2bUserGuid = Guid.Empty;

        ///<summary>
        ///  Returns the user's C2bUserGuid if they have one, otherwise it
        ///  will throw an error.
        ///</summary>
        internal Guid C2bUserGuid
        {
            get
            {
                if (c2bUserGuid == Guid.Empty)
                {
                    c2bUserGuid = GetC2bUserGuid();
                }
                return c2bUserGuid;
            }
        }
                
        internal string C2bEncryptedGuid
        {
            get
            {
                if (c2bEncryptedGuid == string.Empty)
                {
                    c2bEncryptedGuid = User.EncryptedUserId;
                }
                return c2bEncryptedGuid;
            }
        }
                
        internal string C2bEncryptedUsername
        {
            get
            {
                return new AICPAEncryption().Encrypt(User.EmailAddress);                
            }
        }

        

        ///<summary>
        ///  A method that indicates if a user has a C2B User Id.  This can be called 
        ///  instead of the C2bUserGuid property because this will not throw an error 
        ///  if they don't have a C2bUserGuid.
        ///</summary>
        ///<returns></returns>
        internal bool HasC2bUserGuid()
        {
            bool retVal = true;
            try
            {
                Guid temp = C2bUserGuid;
            }
            catch (SecurityException e)
            {
                if (e.Message == AUTHENTICATION_ERROR_NO_C2B_USERID)
                {
                    retVal = false;
                }
                else
                {
                    throw;
                }
            }
            return retVal;
        }

        #endregion Internal Properties

        #endregion Properties

        #region Methods

        #region IUserSecurity Methods

        ///<summary>
        ///  Releases the user's session in the database and sets all local 
        ///  properties to their original values before authentication.
        ///</summary>
        public void EndUserSession()
        {
            ActiveUserSecurityDalc.EndUserSession(ActiveUserDs);
            authenticated = null;
            authenticationError = null;
            domain = null;
            actionPermission = 0;
            email = null;
            bookName = null;
        }

        ///<summary>
        ///  Determines if the user has permission to perform the provided action.
        ///</summary>
        ///<param name = "action">the integer representation of the action in question.</param>
        ///<returns>A bool that indicate if the user has permisstion to perform the provided action.</returns>
        public bool AuthorizeAction(int action)
        {
            return (action & ActionPermission) == action;
        }

        #endregion IUserSecurity Methods

        #region Private Methods

        ///<summary>
        ///  Returns the user's c2b userId or throws an error if they don't have 
        ///  a c2b userid.
        ///</summary>
        ///<returns>A Guid that is the c2b userid</returns>
        private Guid GetC2bUserGuid()
        {
            Guid retval;
            try
            {
                if (User.ReferringSiteValue == ReferringSite.C2b ||
                    (domain == null && User.ReferringSiteValue == ReferringSite.Csc))
                {
                    retval = User.UserId;
                }
                else
                {
                    // TODO: If the service returns no Guid, then throw this error:
                    throw new SecurityException(AUTHENTICATION_ERROR_NO_C2B_USERID);
                }
            }
            catch (Exception e)
            {
                // if try fails, log the error
                if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_EXTERNAL_SERVICE, MODULE_USERSECURITY,
                                                  "GetC2bUserGuid"))
                {
                    IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                      MODULE_USERSECURITY, "GetC2bUserGuid",
                                                      "Datawarehouse Call Failure", e.Message, user);
                    logEvent.Save(false);
                }
                throw;
            }
            return retval;
        }

        ///<summary>
        ///  Set the subscription context of the c2b user by accessing the c2b web services and
        ///  parsing the returned XML and setting the properties of the object accordingly.
        ///</summary>
        private void SetC2bSubscriptionContext()
        {
            // TODO: Replace this code with the call to the real c2bService.
            // TODO: Change our test service to have the same WSDL as the C2B web service.
            string c2bSubscriptionXml = "";
            try
            {
               ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CertificateValidationCallBack);

                // Netforum VRS. C2B
                //IF C2B
                //SubscriptionService c2bService = new SubscriptionService();
                //c2bService.Url = ConfigurationSettings.AppSettings[CONFIG_WEB_REFERENCE_URL];
               C2BSubscriptionWebService.SubscriptionService ss = new C2BSubscriptionWebService.SubscriptionService();
               ss.Url = ConfigurationManager.AppSettings[CONFIG_WEB_REFERENCE_URL];
                
                

                
                string encUsername = C2bEncryptedUsername;
                c2bSubscriptionXml = ss.GetOnlinePlatformAuthInfo(encUsername);            
                //c2bSubscriptionXml = c2bService.GetOnlinePlatformAuthInfo(encUsername);            
               //else
               //{
               //    netForumServices.AICPAOPLWSSoapClient nfservice = new AICPAOPLWSSoapClient();
               //    netForumServices.AuthorizationToken authToken = new AuthorizationToken();
               //    authToken.Token = "OPLxWebUser;XY280t4p7X796QVK009hF";
               //    XmlNode authinfo = nfservice.AICPAGetOPLSubscriptions(ref authToken, C2bEncryptedGuid);
               //    c2bSubscriptionXml = authinfo.OuterXml.ToString();
               //}
                

                //log the xml response for debugging purposes
                IEvent logEvent = new Event.Event(EventType.Info, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                  MODULE_USERSECURITY, "SetC2bSubscriptionContext",
                                                  "C2B Web Service Response", c2bSubscriptionXml, user);
                logEvent.Save(false);
            }
            catch (Exception e)
            {
                if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_EXTERNAL_SERVICE, MODULE_USERSECURITY,
                                                  "SetC2bSubscriptionContext"))
                {
                    IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                      MODULE_USERSECURITY, "SetC2bSubscriptionContext",
                                                      "C2B Web Service Failure", e.Message, user);
                    logEvent.Save(false);
                }
                throw;
            }
            // Handle XML with an error message in it.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(c2bSubscriptionXml);
            XmlNode errorNode = xmlDocument.SelectSingleNode("userinfo/error/description");
            if (errorNode != null)
            {
                authenticated = false;
                authenticationError = errorNode.InnerText;
            }

            if (authenticated != null) return;

            string username = xmlDocument.SelectSingleNode("userinfo/credentials").Attributes["username"].Value;
            if (username != C2bEncryptedUsername)
            {
                throw new UserNotAuthenticated("Authorization service email doesn't match.");                
            }
            
            // do check here?

            domain = xmlDocument.SelectSingleNode("userinfo/permissions").Attributes["domains"].Value;

            //WGD 8/4/2014  If the Web Service passes an empty domain back to use throw an exception.
            if (domain.Length < 2)
            {
                throw new UserNotAuthenticated("An Empty Domain string was returned by the authorization service");

            }            

            XmlNodeList firmNodes = xmlDocument.SelectNodes("userinfo/firms/firm");
            foreach (XmlNode firmNode in firmNodes)
            {
                // for each firm node in the xml tree, check to see if the concurrent user count
                // is greater than the current firm's concurrent user count.  Use the greater 
                // value as the firm's concurrent user count.  Also, add the firm to the 
                // UserSecurity objects firm collection.
                if (Convert.ToInt16(firmNode.Attributes["concurrentusers"].Value) > maximumAllowedConcurrentUsers)
                {
                    maximumAllowedConcurrentUsers = Convert.ToInt16(firmNode.Attributes["concurrentusers"].Value);
                }
                ActiveUserDs.CurrentFirmUsers.AddCurrentFirmUsersRow(firmNode.Attributes["aca"].Value,
                                                                     firmNode.Attributes["code"].Value, C2bUserGuid);
            }
            firmCollection = new FirmCollection(ActiveUserDs.CurrentFirmUsers);
        }

        /// <summary>
        /// Will set the subscription context.
        /// 1. Add User to the Firm Table
        /// 2. Creates new Firm Collection
        /// 3. Grabs Concurrent Setting from web.config
        ///    3.a. The key is < referringsite >MaxUsers ie. "CebMaxUsers"
        /// 4. Calculates the Max usage if there is more than one firm. 
        /// </summary>
        private void SetSubscriptionContext(string referringSite)
        {
            //Check Concurrent
            ActiveUserDs.CurrentFirmUsers.AddCurrentFirmUsersRow(referringSite,
                                                                 referringSite, user.UserId);
            firmCollection = new FirmCollection(ActiveUserDs.CurrentFirmUsers);
            //Setting in Web.Confg is <ReferringSite>MaxUsers
            string webConfigKey = string.Format(CONFIG_MAX_USERS_MASK, referringSite);

            int definedMax = 0;//?
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[webConfigKey]))
            {
                IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                    MODULE_USERSECURITY, "SetSubscriptionContext", "Missing/Invalid Concurrency setting",
                                                    "Missing concurrency count. Please add key:" + webConfigKey, user);
                logEvent.Save(false);
                throw new Exception("Missing concurrency count. Add web.config key: " + webConfigKey);
            }
            else
            {
                definedMax = int.Parse(ConfigurationManager.AppSettings[webConfigKey]);
            }


            foreach (IFirm firm in firmCollection)
            {
                maximumAllowedConcurrentUsers = Math.Max(firm.GetCurrentFirmUserCount(SessionTimeout), definedMax);
            }
        
        }

        private string reconcileDomainStrings(string domain1, string domain2)
        {
            List<string> domainList = new List<string>();

            string[] firstList = domain1.Split('~');
            string[] secondList = domain2.Split('~');

            foreach (string domain in firstList)
            {
                if (!domainList.Contains(domain))
                {
                    domainList.Add(domain);
                }
            }
            foreach (string domain in secondList)
            {
                if (!domainList.Contains(domain))
                {
                    domainList.Add(domain);
                }
            }

            if (domainList.Count == 1)
            {
                return domainList[0];
            }
            if (domainList.Count == 0)
            {
                return "";
            }

            string returnString = "";
            foreach (string domain in domainList)
            {
                if (!string.IsNullOrEmpty(domain))
                {
                    returnString = returnString + domain + "~";
                }
            }
            returnString = returnString.TrimEnd('~');
            return returnString;
        }

        private string getC2bDomainString()
        {
            string c2bSubscriptionXml = "";
            string c2bDomain = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                     new RemoteCertificateValidationCallback(CertificateValidationCallBack);

                // Netforum VRS. C2B
                //IF C2B
                C2BSubscriptionWebService.SubscriptionService ss = new C2BSubscriptionWebService.SubscriptionService();
                ss.Url = ConfigurationManager.AppSettings[CONFIG_WEB_REFERENCE_URL];




                string encUsername = C2bEncryptedUsername;
                c2bSubscriptionXml = ss.GetOnlinePlatformAuthInfo(encUsername);

                //log the xml response for debugging purposes
                IEvent logEvent = new Event.Event(EventType.Info, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                  MODULE_USERSECURITY, "getC2bDomainString",
                                                  "C2B Web Service Response", c2bSubscriptionXml, user);
                logEvent.Save(false);
            }
            catch (Exception e)
            {
                if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_EXTERNAL_SERVICE, MODULE_USERSECURITY,
                                                  "getC2bDomainString"))
                {
                    IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                      MODULE_USERSECURITY, "getC2bDomainString",
                                                      "C2B Web Service Failure", e.Message, user);
                    logEvent.Save(false);
                }
                return "";
            }
            // Handle XML with an error message in it.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(c2bSubscriptionXml);
            XmlNode errorNode = xmlDocument.SelectSingleNode("userinfo/error/description");
            if (errorNode != null)
            {
                return "";
            }
            // do check here?

            try
            {
                c2bDomain = xmlDocument.SelectSingleNode("userinfo/permissions").Attributes["domains"].Value;
            }
            catch (Exception ex)
            {
                return "";
            }


            return c2bDomain;
        }

        /// <summary>
        /// Will set the subscription context.
        /// 
        /// </summary>
        private void SetRaveSubscriptionContext(string oktaid, string roleId)
        {
            string c2bDomain = "";
            if (ConfigurationManager.AppSettings["RaveShouldGetC2bDomain"].ToString().ToLower() == "true")
            { 
                c2bDomain = getC2bDomainString();
            }
            
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls |SecurityProtocolType.Ssl3; 
                string raveSubscriptionUrl = ConfigurationManager.AppSettings["RaveSubscriptionServiceUrl"].ToString();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(raveSubscriptionUrl);
                request.Method = "POST";
                request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["RaveUsername"].ToString(), ConfigurationManager.AppSettings["RavePassword"].ToString());
                request.PreAuthenticate = true;
                request.Accept = "application/json;";

                request.Headers.Add("Accept-Charset", "utf-8;");
                //UAT hardcoded header for debugging
                //request.Headers.Add("Authorization", "Basic c2VydmljZS5vcGwuYXBpdXNlcjpPVEZKVmtndk1WTlBVWEIyWTNWU1NYWkZXbVJpZHowOUNnPT0=");
                
                //Production hardcoded header for debugging
                //request.Headers.Add("Authorization", "Basic c2VydmljZS5vcGwudXNlcjpiRE5aVVdaa1pqVkRLMFpsY0VSa1JUVjJhVE0zUVQwOUNnPT0=");
                
                request.ContentType = "application/json; charset=utf-8";

                string oktaIdProperty = "\"oktaid\":\"" + oktaid + "\"";
                string oktaRoleIdProperty = "";
                if (roleId != null)
                {
                    string[] roleList = roleId.Split(',');
                    oktaRoleIdProperty = ",\"oktaroleid\":[";
                    int roleCount = 0;
                    foreach (string role in roleList)
                    {
                        if (roleCount > 0)
                        {
                            oktaRoleIdProperty = oktaRoleIdProperty + ",";
                        }
                        oktaRoleIdProperty = oktaRoleIdProperty + "\""+role+"\"";
                        roleCount++;
                    }
                    oktaRoleIdProperty = oktaRoleIdProperty + "]";
                }

                string json = "{"+oktaIdProperty+oktaRoleIdProperty+"}";

                IEvent sendlogEvent = new Event.Event(EventType.Info, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                  MODULE_USERSECURITY, "SetRaveSubscriptionContext",
                                  "Sending request to Rave",
                                  "json: " + json + " username: " + ConfigurationManager.AppSettings["RaveUsername"].ToString()+" password: "+ConfigurationManager.AppSettings["RavePassword"].ToString()+" url: "+raveSubscriptionUrl, user);
                sendlogEvent.Save(false);


                byte[] body = Encoding.ASCII.GetBytes(json);
                request.ContentLength = body.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();

                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(),
                                      ASCIIEncoding.UTF8);
                string responsejson = reader.ReadToEnd();
                reader.Close();


                //dynamic result = System.Web.Helpers.Json.Decode(responsejson);
                dynamic result = JObject.Parse(responsejson);


                //log the xml response for debugging purposes
                IEvent logEvent = new Event.Event(EventType.Info, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                  MODULE_USERSECURITY, "SetRaveSubscriptionContext",
                                                  "Rave Web Service Response", responsejson, user);
                
                logEvent.Save(false);

                string raveDomain = result.domainStringList.ToString();
                domain = reconcileDomainStrings(c2bDomain, raveDomain);
            }
            catch (System.Exception ex)
            {
                IEvent logEvent = new Event.Event(EventType.Info, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                      MODULE_USERSECURITY, "SetRaveSubscriptionContext", "Error retrieving domain from Rave web service", ex.Message, user);
                logEvent.Save(false);
            }
        }
        
        /// <summary>
        /// Set the Subscription Context for Mcgdy/Mcgdyasc
        /// Will read the configuration setting that indicates how many concurrent users are allowed on the system.
        /// </summary>
        private void SetMcgdySubscriptionContext()
        {
            //Check Concurrent
            ActiveUserDs.CurrentFirmUsers.AddCurrentFirmUsersRow(User.ReferringSiteValue.ToString(),
                                                                 User.ReferringSiteValue.ToString(), user.UserId);
            firmCollection = new FirmCollection(ActiveUserDs.CurrentFirmUsers);
            string webConfigKey = string.Format(CONFIG_MAX_USERS_MASK, User.ReferringSiteValue.ToString());

            int definedMax = 0;//?
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[webConfigKey]))
            {
                IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                    MODULE_USERSECURITY, "SetMcgdySubscriptionContext", "Missing/Invalid Concurrency setting",
                                                    "Missing concurrency count. Please add key:" + webConfigKey, user);
                logEvent.Save(false);
                throw new Exception("Missing concurrency count. Add web.config key: " + webConfigKey);
            }
            else
            {
                definedMax = int.Parse(ConfigurationManager.AppSettings[webConfigKey]);
            }
            
            
            foreach (IFirm firm in firmCollection)
            {
                maximumAllowedConcurrentUsers = Math.Max(firm.GetCurrentFirmUserCount(SessionTimeout), definedMax);
            } 
        }


        ///<summary>
        ///  Set the subscription context of a non-c2b user.
        ///</summary>
        private void SetSubscriptionContext()
        {
            actionPermission = Convert.ToInt16(ConfigurationSettings.AppSettings[CONFIG_DEFAULTACTIONPERMISSION]);
        }

        ///<summary>
        ///  Get any information from the data warehouse (including email address).
        ///</summary>
        private void SetDataWarehouseInformation()
        {
            try
            {
                // TODO: Call data warehouse and set pertinent userinformation.
                email = "";
            }
            catch (Exception e)
            {
                if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_EXTERNAL_SERVICE, MODULE_USERSECURITY,
                                                  "SetDataWarehouseInformation"))
                {
                    IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                      MODULE_USERSECURITY, "SetDataWarehouseInformation",
                                                      "Datawarehouse Call Failure.", e.Message, user);
                    logEvent.Save(false);
                }
                throw;
            }
        }

        ///<summary>
        ///  Calls the AICPA's datawarehouse and confirms if the user's userId is indeed 
        ///  a valid AICPA userId Guid.
        ///</summary>
        ///<returns></returns>
        private bool ValidateAicpaGuid()
        {
            bool retVal = false;
            try
            {
                // TODO: Implement call to datawarehouse to validate user.
                retVal = true;
            }
            catch (Exception e)
            {
                if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_EXTERNAL_SERVICE, MODULE_USERSECURITY,
                                                  "ValidateAicpaGuid"))
                {
                    IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_EXTERNAL_SERVICE,
                                                      MODULE_USERSECURITY, "ValidateAicpaGuid",
                                                      "Datawarehouse Call Failure.", e.Message, user);
                    logEvent.Save(false);
                }
                throw;
            }
            return retVal;
        }

        ///<summary>
        ///  Determines if the UserId is valid, checks if the user is to get the c2b subscription information 
        ///  and gets and sets it accordingly, persists a user session to the database, and sets the authentication
        ///  error if the user failed to authenticate.
        ///</summary>
        private void AuthenticateUser()
        {
            // try to validate the user, but if it fails, then return the
            // user as not validated.
            bool validAicpaGuid = false;
            try
            {
                validAicpaGuid = ValidateAicpaGuid();
            }
            catch (Exception e)
            {
                validAicpaGuid = false;
                authenticated = false;
                authenticationError = string.Format(AUTHENTICATION_ERROR_VALIDATION_FAILURE, e.Message);
                throw;
            }

            // if the user has a valid Aicpa userId, then try to get the subscription context.
            if (validAicpaGuid)
            {
                //Set the subscription context of the user.
                if (User.ReferringSiteValue == ReferringSite.C2b ||
                    (User.ReferringSiteValue == ReferringSite.Csc && HasC2bUserGuid()))
                {
                    // can set authenticated to false if there is an error string in the return xml from
                    // the c2b web service.
                    try
                    {
                        SetC2bSubscriptionContext();
                    }
                    catch (Exception e)
                    {
                        authenticated = false;
                        authenticationError = string.Format(AUTHENTICATION_C2B_SUBSCRIPTION_SERVICE_FAILURE, e.Message);
                        throw;
                    }
                    
                    // if the user is in a firm, check the number of concurrent users.
                    if (authenticated == null && IsFirmCountExceeded())
                    {
                        authenticated = false;
                        authenticationError = AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED;
                        Log.LogEvent(EventType.Usage, 1, "UserSecurity", "AuthenticateUser", "Firm Exceeded", string.Format("Firm Count Exceeded for {0}", User.ReferringSiteValue.ToString()), User);
                    }
                }
                else if (User.ReferringSiteValue == ReferringSite.Rave)
                {
                    SetRaveSubscriptionContext(User.OktaId, User.RoleId);
                }
                else if (User.ReferringSiteValue == ReferringSite.Ceb)
                {
                    // can set authenticated to false if there is an error string in the return xml from
                    // the c2b web service.
                    try
                    {
                        SetSubscriptionContext(User.ReferringSiteValue.ToString());
                    }
                    catch (Exception e)
                    {
                        authenticated = false;
                        authenticationError = string.Format(AUTHENTICATION_C2B_SUBSCRIPTION_SERVICE_FAILURE, e.Message);
                        throw;
                    }

                    // if the user is in a firm, check the number of concurrent users.
                    if (authenticated == null && IsFirmCountExceeded())
                    {
                        authenticated = false;
                        authenticationError = AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED;
                        Log.LogEvent(EventType.Usage, 1, "UserSecurity", "AuthenticateUser", "Firm Exceeded", string.Format("Firm Count Exceeded for {0}", User.ReferringSiteValue.ToString()), User);
                    }
                }
                else if (User.ReferringSiteValue == ReferringSite.Mcgdy ||
                         User.ReferringSiteValue == ReferringSite.Mcgdyasc)
                {
                    // can set authenticated to false if there is an error string in the return xml from
                    // the c2b web service.
                    try
                    {
                        SetMcgdySubscriptionContext();
                    }
                    catch (Exception e)
                    {
                        authenticated = false;
                        authenticationError = string.Format(AUTHENTICATION_MCGDY_SUBSCRIPTION_SERVICE_FAILURE, e.Message);
                        throw;
                    }
                    bool userLoggedIn = false;
                    foreach (IFirm firm in firmCollection)
                    {
                        userLoggedIn |= firm.UserLoggedIn(user.UserId);
                    }

                    // if the user is in a firm, check the number of concurrent users.
                    if (authenticated == null && IsFirmCountExceeded() && !userLoggedIn)
                    {
                        authenticated = false;
                        authenticationError = AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED;
                        Log.LogEvent(EventType.Usage, 1, "UserSecurity", "AuthenticateUser", "Firm Exceeded", string.Format("Firm Count Exceeded for {0}", User.ReferringSiteValue.ToString()), User);
                    }

                }
                else
                {
                    SetSubscriptionContext();
                }
            }
            else
            {
                authenticated = false;
                authenticationError = AUTHENTICATION_ERROR_INVALID_USERGUID;
            }

            // The user should only have an authenticated==false or authenticated==null.
            if (authenticated != null)
            {
            }
            else
            {
                ActiveUserDs.Users.AddUsersRow(User.UserId, DateTime.Now, SessionId);
                try
                {
                    // this will insert the user and the currentFirmUsers (if any).
                    ActiveUserSecurityDalc.CreateUserSession(ActiveUserDs);
                    authenticated = true;
                }
                catch (Exception e)
                {
                    authenticated = false;
                    authenticationError = e.Message;
                    if (Event.Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_DATALAYER_ERROR,
                                                      MODULE_USERSECURITY,
                                                      "AuthenticateUser"))
                    {
                        IEvent logEvent = new Event.Event(EventType.Error, DateTime.Now,
                                                          ERROR_SEVERITY_DATALAYER_ERROR,
                                                          MODULE_USERSECURITY, "AuthenticateUser",
                                                          "Error Creating a user session.",
                                                          e.Message, user);
                        logEvent.Save(false);
                    }
                    throw;
                }
            }
        }

        ///<summary>
        ///  If a user is a firm user, then they have restrictions on how many concurrent
        ///  users can be logged on under the firm.  This method determines if the firm
        ///  count is exceeded for a given users.
        ///</summary>
        ///<returns>
        ///  A bool indicating if the firm count for the user is exceeded.
        ///</returns>
        ///<remarks>
        ///  If a user is in multiple firms, then the maximum allowed users for the 
        ///  all the user's firms is the maximum allowed users from the firm with 
        ///  the highest amount of maximum allowed users.
        ///		
        ///  For example, if a user has three firms--firms A, B, and C--and A has 5 maximum users, 
        ///  B has 2, and C has 4, then the user's maximum allowed users on all three firms is 5.  So
        ///  the user would be able to log on so long as firms A, B, or C have not exceeded 5 users.  As
        ///  soon as one firm exceeds 5 concurrent users, then the user cannot log on.
        ///</remarks>
        private bool IsFirmCountExceeded()
        {
            bool retVal = false;
            if (firmCollection.Count > 0)
            {
                foreach (IFirm firm in
                    firmCollection.Cast<IFirm>().Where(
                        firm => firm.GetCurrentFirmUserCount(SessionTimeout) >= MaximumAllowedConcurrentUsers))
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        ///<summary>
        ///  Checks if the enough time has lapsed to go back to the database and check if the sessionId
        ///  is valid.
        ///</summary>
        ///<returns>A bool indicating if the sessionId check time has lapsed.</returns>
        ///<remarks>
        ///  A user's sessionId may become invalid (for instance the user may log in on another machine,
        ///  thus making his or her new session the current session). When this happens, the invalid session
        ///  will not be flagged as invalid until it checks the database and sees that there is a difference
        ///  in the sessionIds.  The SessionId check only happens at a given interval as set in the config
        ///  file.
        ///</remarks>
        private bool IsSessionIdCheckTimeExpired()
        {
            if (sessionIdCheckTime == DateTime.MinValue)
            {
                sessionIdCheckTime = DateTime.Now;
            }
            bool retVal = false;
            TimeSpan allowableTimeLapse = new TimeSpan(0, 0, 0,
                                                       Convert.ToInt16(
                                                           ConfigurationSettings.AppSettings[
                                                               CONFIG_SESSION_POLL_INTERVAL]), 0);
            if ((DateTime.Now - sessionIdCheckTime) > allowableTimeLapse)
            {
                retVal = true;
            }
            return retVal;
        }

        ///<summary>
        ///  A method that hits the database table D_User and determines if the value in the field CurrentSessionId
        ///  is equal to the value of this object's session.
        ///</summary>
        ///<returns>A bool indicating if the session is valid.</returns>
        private bool IsSessionIdValid()
        {
            return ActiveUserSecurityDalc.IsSessionIdValid(User.UserId, SessionId);
        }

        #endregion Private Methods

        #endregion Methods

        public bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain,
                                                  SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}