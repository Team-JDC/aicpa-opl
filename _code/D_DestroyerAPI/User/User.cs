using System;
using System.Collections.Generic;
using System.Linq;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.User.Firm;
using AICPA.Destroyer.User.UserPreferences;
using AICPA.Destroyer.User.Organization;
using System.Web;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  Represents a subscription user on the destroyer system and contains
    ///  all the security context about the user.
    ///</summary>
    public class User : DestroyerBpc, IUser
    {
        #region Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_USERNOTLOGGEDIN =
            "The user is not logged on.  Please Log on before accessing the User's security context.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_USERLOGONFAILED =
            "The user failed to authenticate.  Reason for authentication failure: {0}";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_USERALREADYLOGGEDON =
            "The user is already logged on under session {0}.  Please logoff before logging on again.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_INVALIDSESSIONID = "Please provide a valid sessionId.  The sessionId cannot be null.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_LOGOFFWITHOULOGON =
            "The user {0} is not logged on.  LogOn before attempting to LogOff.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_DONOTPASSDOMAINSTRING =
            "Please do not pass the domain string if authenticating a user with referring site {0}";

        private const string METHOD_LOGON = "LogOn";
        private const string USAGE_EVENT_LOGON = "User Logon";
        private const string USAGE_EVENT_FIRM_LOGON = "Firm Logon";
        private const string MESSAGE_LOGON = "User Successfully Logged On.";
        private const string MESSAGE_FIRM_LOGON = "ACA={0};Code={1};";

        private const string METHOD_LOGOFF = "LogOff";
        private const string USAGE_EVENT_LOGOFF = "User Logoff";
        private const string USAGE_EVENT_FIRM_LOGOFF = "Firm Logoff";
        private const string MESSAGE_LOGOFF = "User Successfully Logged Off.";
        private const string MESSAGE_FIRM_LOGOFF = "ACA={0};Code={1};";

        #endregion Constants

        #region Properties

        #region Private Properties

        private IUserDalc activeUserDalc;

        ///<summary>
        ///  If an active UserDalc doesn't exist it creates it and returns it, otherwise
        ///  it just returns the active UserDalc.
        ///</summary>
        public IUserDalc ActiveUserDalc
        {
            get { return activeUserDalc ?? (activeUserDalc = new UserDalc()); }
        }

        private string _organization { get; set; }

        #endregion Private Properties

        #region Phase II Properties

        //public AICPA.Destroyer.Content.Book.IBookCollection NotificationSubscriptions
        //{
        //    get
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.NotificationSubscriptions getter implementation
        //        //return null;
        //    }
        //    set
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.NotificationSubscriptions setter implementation
        //    }
        //}

        //public AICPA.Destroyer.Content.Search.ISearchCriteriaCollection Searches
        //{
        //    get
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.Searches getter implementation
        //        //return null;
        //    }
        //    set
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.Searches setter implementation
        //    }
        //}

        //public INoteCollection Notes
        //{
        //    get
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.Notes getter implementation
        //        //return null;
        //    }
        //    set
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.Notes setter implementation
        //    }
        //}

        //public AICPA.Destroyer.Content.Document.IDocumentCollection DocumentHistory
        //{
        //    get
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.DocumentHistory getter implementation
        //        //return null;
        //    }
        //    set
        //    {
        //        throw new Exception("This feature will not be implemented until phase 2");
        //        // TODO:  Add User.DocumentHistory setter implementation
        //    }
        //}

        #endregion Phase II Properties

        #region IUser Properties
        
        private string emailAddress;
        private string firstName;
        private string lastName;
        //private string displayName;
        //private bool admin;
        //private bool active;
        //private int organizationId;
        //private string membershipId;
        //private SubscriptionStatusEnum subscriptionStatus;

        private readonly string encryptedUserId = string.Empty;

        private ReferringSite referringSiteValue;
        private Guid userId = Guid.Empty;

        private string domain;
        private LicenseAgreementStatus licenseAgreementValue;
        private string sessionId;
        private IUserSecurity userSecurity;
        private UserPreferencesDictionary preferences;
        private SearchCollection savedSearches;
        
        /// <summary>
        /// GASB AccountID used for the concurrency
        /// initially used for GASB
        /// </summary>
        private string accountId;
        /// <summary>
        /// Define the conccurecny for the user account
        /// initially used for GASB
        /// </summary>
        private int concurrency;


        /// <summary>
        ///   The encrypted Id of the user.
        /// </summary>
        public string EncryptedUserId
        {
            get { return encryptedUserId; }
        }

        ///<summary>
        ///  The Id of the user.
        ///</summary>
        public Guid UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        ///<summary>
        ///  The site that the user came from.
        ///</summary>
        public ReferringSite ReferringSiteValue
        {
            get { return referringSiteValue; }
        }

        public string OktaId { get; set; }

        public string RoleId { get; set; }

        ///<summary>
        ///  A reference to the security context of a user.
        ///</summary>
        public IUserSecurity UserSecurity
        {
            get
            {
                if (sessionId == null)
                {
                    throw new SecurityException(ERROR_USERNOTLOGGEDIN);
                }
                if (userSecurity == null || sessionId != userSecurity.SessionId)
                {
                    userSecurity = domain == null
                                       ? new UserSecurity(this, sessionId)
                                       : new UserSecurity(this, sessionId, domain);
                }
                return userSecurity;
            }
        }

        /// <summary>
        /// Gets the email address.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        /// <summary>
        /// Gets the license agreement value.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public LicenseAgreementStatus LicenseAgreementValue
        {
            get { return licenseAgreementValue; }
        }

        public UserPreferencesDictionary Preferences
        {
            get { return preferences ?? (preferences = UserPreference.GetPreferencesForUser(UserId)); }
        }

        public SearchCollection SavedSearches
        {
            get { return savedSearches ?? (savedSearches = new SearchCollection(SearchCriteria.GetSavedSearchesForUser(UserId))); }
        }

        public string Organization 
        {
            get { return _organization; }
        }

        /// <summary>
        /// Gets the organization ID
        /// </summary>
        //public int OrganizationId
        //{
        //    get { return organizationId; }
        //    set { organizationId = value; }
        //}

        /// <summary>
        /// Gets the First Name
        /// </summary>
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        /// <summary>
        /// Gets the last name
        /// </summary>
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        /// <summary>
        /// Gets the display name
        /// </summary>
        //public string DisplayName
        //{
        //    //get { return displayName; }
        //    get { return displayName; }
        //    set { displayName = value; }
        //}

        /// <summary>
        /// Returns a boolean value to indicate whether user is admin
        /// </summary>
        //public bool Admin
        //{
        //    get { return admin; }
        //    set { admin = value; }

        //}

        /// <summary>
        /// Returns a boolean value to indicate whether user is active
        /// </summary>
        //public bool Active
        //{
        //    get { return active; }
        //    set { active = value; }

        //}

        //public string MembershipId
        //{
        //    get { return membershipId; }
        //    set { membershipId = value; }
        //}

        //public SubscriptionStatusEnum SubscriptionStatus
        //{
        //    get { return subscriptionStatus;  }
        //    set { subscriptionStatus = value; }
        //}

        /// <summary>
        /// GASB AccountID used for the concurrency
        /// initially used for GASB
        /// </summary>
        public string AccountId
        {
            get { return accountId; }
            set { accountId = value; }
        }        

        /// <summary>
        /// Define the conccurecny for the user account
        /// initially used for GASB
        /// </summary>
        public int Concurrency
        {
            get { return concurrency; }
            set { concurrency = value; }
        }        
        
        #endregion IUser Properties

        #endregion Properties

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <remarks>This constructor was added because we need an empty user to create a user.</remarks>
        public User()
        {
            
        }

        ///<summary>
        ///  This constructor is used for creating a subscription-based user.
        ///</summary>
        ///<param name = "userId">The user Guid for the given user.</param>
        ///<param name = "referringSiteValue">The referring site.</param>
        public User(Guid userId, ReferringSite referringSiteValue)
        {
            InitUser(userId, referringSiteValue); 
            //this.userId = userId;
            //this.referringSiteValue = referringSiteValue;
            //moduleName = "User";
            //licenseAgreementValue = (LicenseAgreementStatus) ActiveUserDalc.GetLicenseValue(userId);
            //emailAddress = ActiveUserDalc.GetEmailAddress(userId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.	
        /// </summary>
        /// <param name="encryptedUserId">The encrypted user id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="referringSiteValue">The referring site value.</param>
        /// <remarks></remarks>
        public User(string encryptedUserId, Guid userId, ReferringSite referringSiteValue)
        {
            this.encryptedUserId = encryptedUserId;
            this.userId = userId;
            this.referringSiteValue = referringSiteValue;
            moduleName = "User";
            licenseAgreementValue = (LicenseAgreementStatus) ActiveUserDalc.GetLicenseValue(userId);
            emailAddress = ActiveUserDalc.GetEmailAddress(userId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="referringSiteValue">The referring site value.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <remarks></remarks>
        public User(Guid userId, ReferringSite referringSiteValue, string emailAddress)
        {
            this.userId = userId;
            this.referringSiteValue = referringSiteValue;
            this.emailAddress = emailAddress;
            moduleName = "User";
            licenseAgreementValue = (LicenseAgreementStatus) ActiveUserDalc.GetLicenseValue(userId);
        }

        /// <summary>
        /// This Constructor is only used when calling GetSessionByEmail
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="email"></param>
        /// <param name="referringSiteValue"></param>
        public User(string sessionId, string email, string referringSiteValue)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                this.userId = ActiveUserDalc.GetUserIdByEmailAndSessionId(email, sessionId);
                this.emailAddress = email;
            }
            else
            {
                //Talked to Hari today (5/8/2015) and he said that he is hardcoding <username>@aicpa.com,
                // any user that comes in with @aicpa.com, we should remove the @aicpa.com piece and use the 
                // username as is.  Any other email domain (@gmail.com, @knowlysis.com, etc) will be un touched.
                var checkUser = Regex.Replace(email, @"@aicpa.com", "", RegexOptions.IgnoreCase);

                this.userId = ActiveUserDalc.GetUserIdByEmailAndReferringSite(checkUser, referringSiteValue);
                

                //If we can't find the user then we create a new one.
                if (this.userId == Guid.Empty)
                {
                    this.userId = Guid.NewGuid();                
                }

                this.emailAddress = checkUser;
                
                IEvent logEvent = new Event.Event(EventType.Info
                    , DateTime.Now
                    , 1
                    , "User.cs"
                    , "User"
                    , "User Login"
                    , string.Format("Logging user in with username/email: {0} ", this.emailAddress));
                logEvent.Save(false);
            }            
            
            //else if (ActiveUserDalc.UserExists(email))
            //{
            //    this.userId = ActiveUserDalc.GetUserIdByEmail(email);
            //}
            //else
            //{
            //    this.userId = Guid.NewGuid();
            //}
            //We can't set the sessionId, the system will puke 
            //this.sessionId = sessionId;
            
            ReferringSite parsedReferringSite;
            moduleName = "User";
            licenseAgreementValue = (LicenseAgreementStatus)ActiveUserDalc.GetLicenseValue(userId);
            if (Enum.TryParse(referringSiteValue, out parsedReferringSite))
                this.referringSiteValue = parsedReferringSite;
            else
                this.referringSiteValue = ReferringSite.Other;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.	
        /// </summary>
        /// <param name="encryptedUserId">The encrypted user id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="referringSiteValue">The referring site value.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <remarks></remarks>
        public User(string encryptedUserId, Guid userId, ReferringSite referringSiteValue, string emailAddress)
        {
            this.encryptedUserId = encryptedUserId;
            this.userId = userId;
            this.referringSiteValue = referringSiteValue;
            this.emailAddress = emailAddress;
            moduleName = "User";
            licenseAgreementValue = (LicenseAgreementStatus) ActiveUserDalc.GetLicenseValue(userId);
        }

        /// <summary>
        /// This Constructor is only used when calling GetSessionByEmail
        /// </summary>
        /// <param name="email"></param>
        /// <param name="referringSiteValue"></param>
        public User(string email, string referringSiteValue)
        {
            ReferringSite parsedReferringSite;
            this.emailAddress = email;

            if(Enum.TryParse(referringSiteValue, out parsedReferringSite))
                this.referringSiteValue = parsedReferringSite;
            else
                this.referringSiteValue = ReferringSite.Other;
        }


        /// <summary>
        /// This constructor is only used when internally and uses the current referring site if available
        /// </summary>
        /// <param name="email">Email address to check</param>
        public User(string email)
        {
            ReferringSite parsedReferringSite;
            this.emailAddress = email;

            string referringSiteValue = ActiveUserDalc.GetReferringSiteByEmail(email);
            if (Enum.TryParse(referringSiteValue, out parsedReferringSite))
                this.referringSiteValue = parsedReferringSite;
            else
                this.referringSiteValue = ReferringSite.Other;            
        }

        

        #endregion Public Constructors

        #endregion Constructors

        #region Methods

        #region IUser Methods

        public Guid Login(string username, string password, string SessionId, string overrideSubscription, ReferringSite referringSiteValue)
        {
            try
            {
                Guid guid = ActiveUserDalc.Login(username, password);
                if (guid != Guid.Empty)
                {
                    //removed for ethics.  Are they going to have different content?
                    
                    string subscription = "et-cod";//ActiveUserDalc.getSubscriptionByUserId(guid);

                    InitUser(guid, referringSiteValue);

                    if (string.IsNullOrEmpty(overrideSubscription))
                    {
                        LogOn(SessionId, subscription);
                    }
                    else
                    {
                        LogOn(SessionId, overrideSubscription);
                    }

                    _organization = ReferringSiteValue.ToString();
                    //_organization = ActiveUserDalc.getMainSubscriptionByUserId(guid);
                }

                return guid;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }



        ///<summary>
        ///  Log a user on by calling this method.
        ///</summary>
        ///<param name = "sessionId">The sessionId for a given logon.</param>
        public void LogOn(string sessionId)
        {
            LogOn(sessionId, null);
        }

        ///<summary>
        ///  Log a user on with the security context.
        ///</summary>
        ///<param name = "sessionId">The sessionId for a given logon.</param>
        ///<param name = "domain">The domain string indicating the subscriptions a user has access to.</param>
        public void LogOn(string sessionId, string domain)
        {
            ActiveUserDalc.SaveUser(UserId, ReferringSiteValue.ToString(), EmailAddress);
            if (domain != null && referringSiteValue == ReferringSite.C2b)
            {
                throw new SecurityException(string.Format(ERROR_DONOTPASSDOMAINSTRING, ReferringSite.C2b));
            }
            if (this.sessionId != null)
            {
                throw new SecurityException(string.Format(ERROR_USERALREADYLOGGEDON, this.sessionId));
            }
            if (sessionId == null)
            {
                throw new ArgumentNullException(ERROR_INVALIDSESSIONID);
            }
            // don't set the sessionId until after the two previous checks.
            this.sessionId = sessionId;
            this.domain = domain;
            if (UserSecurity.Authenticated == false)
            {
                string authenticationError = UserSecurity.AuthenticationError;
                this.sessionId = null;
                this.domain = null;
                throw new SecurityException(string.Format(ERROR_USERLOGONFAILED, authenticationError));
            }
            if (
                !Event.Event.IsEventToBeLogged(EventType.Usage, USAGE_SEVERITY_SUCCESSFUL_LOGON, ModuleName,
                                               METHOD_LOGON))
                return;
            DateTime now = DateTime.Now;
            Event.Event logOnEvent = new Event.Event(EventType.Usage, now, USAGE_SEVERITY_SUCCESSFUL_LOGON, ModuleName,
                                                     METHOD_LOGON, USAGE_EVENT_LOGON, MESSAGE_LOGON, UserId,
                                                     this.sessionId);
            logOnEvent.Save(false);
            foreach (Event.Event firmLogOnEvent in from IFirm firm in UserSecurity.FirmCollection
                                                   select
                                                       new Event.Event(EventType.Usage, now,
                                                                       USAGE_SEVERITY_SUCCESSFUL_LOGON, ModuleName,
                                                                       METHOD_LOGON, USAGE_EVENT_FIRM_LOGON,
                                                                       string.Format(MESSAGE_FIRM_LOGON, firm.Aca,
                                                                                     firm.Code), UserId, this.sessionId)
                )
            {
                firmLogOnEvent.Save(false);
            }
        }

        ///<summary>
        ///  For each session a user should be logged off.  Call this method 
        ///  to log a user off.
        ///</summary>
        public void LogOff()
        {
            // A user cannot logoff if they have no firm id.
            if (sessionId == null)
            {
                throw new SecurityException(string.Format(ERROR_LOGOFFWITHOULOGON, UserId));
            }

            // Store user information for logging off the user
            string logOffUserId = UserId.ToString();
            string logOffSessionId = sessionId;

            // Log off the user...
            UserSecurity.EndUserSession();
            userSecurity = null;
            sessionId = null;
            domain = null;

            // Log to the event log that the user logged off and the firms that the user had.
            if (
                !Event.Event.IsEventToBeLogged(EventType.Usage, USAGE_SEVERITY_SUCCESSFUL_LOGOFF, ModuleName,
                                               METHOD_LOGOFF)) return;
            DateTime now = DateTime.Now;
            Event.Event logOffEvent = new Event.Event(EventType.Usage, now, USAGE_SEVERITY_SUCCESSFUL_LOGOFF, ModuleName,
                                                      METHOD_LOGOFF, USAGE_EVENT_LOGOFF, MESSAGE_LOGOFF, UserId,
                                                      sessionId);
            logOffEvent.Save(false);
        }

        /// <summary>
        /// Sets the license agreement value.	
        /// </summary>
        /// <param name="statusValue">The status value.</param>
        /// <remarks></remarks>
        public void SetLicenseAgreementValue(LicenseAgreementStatus statusValue)
        {
            ActiveUserDalc.SetLicenseValue(UserId, (int) statusValue);
            licenseAgreementValue = statusValue;
        }

        public string GetSessionId()
        {
            //ActiveUserDalc.SaveUser(Guid.NewGuid(), this.referringSiteValue.ToString(), this.emailAddress);

            return ActiveUserDalc.GetSessionByEmail(this.emailAddress, this.referringSiteValue.ToString());
        }

        /// <summary>
        /// This is internal use only.  Used for SingleSignOnHelper
        /// </summary>
        /// <param name="sessionId">new SessionId</param>
        public void UpdateSessionId(string sessionId)
        {
            this.sessionId = sessionId;
            this.activeUserDalc.UpdateUserSessionId(this.EmailAddress, this.referringSiteValue.ToString(), sessionId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="organizationId"></param>        
        public void CreateUser(Guid userId, string referringSite, string emailAddress, string password, string firstName, string lastName)
        {
            ActiveUserDalc.CreateUser(userId, referringSite, emailAddress, password, firstName, lastName);
        }

        /// <summary>
        /// Create a user with membershipId
        /// </summary>
        /// <param name="membershipId">Membership ID</param>
        /// <param name="password">User password</param>
        /// <param name="referingSite">Refering Site</param>
        /// <param name="organizationId">Assigned Organization ID</param>
        /// <param name="subscriptionStatus">Subscription Status</param>
        public void CreateUserMembership(string membershipId, string password, ReferringSite referingSite)
        {
            ActiveUserDalc.CreateUserMembership(membershipId, password, referingSite);
        }

        public void EditUser(Guid userId, string emailAddress, int organizationId, string password, string firstName, string lastName, string displayName, bool active)
        { 

        
        }

        /// <summary>
        /// Method to retrieve users for a specific organization
        /// </summary>
        /// <param name="organizationId">Organization ID number</param>
        /// <returns>Returns a generic list of Users</returns>
        //public List<IUser> GetUsers(int organizationId)
        //{
        //    List<IUser> retList = new List<User>();
        //    retList = ActiveUserDalc.GetUsers(organizationId);
        //    return retList;
        //}


        /// <summary>
        /// resets the password to a generated password and emails user's address
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks></remarks>
        public string ResetPassword(string userName)
        {
            string password = GenerateRandomString(10);
            return ActiveUserDalc.ResetPassword(userName, password);
        }

        public string SendUserPassword(string userName)
        {
            return ActiveUserDalc.SendUserPassword(userName);
        }

        /// <summary>
        /// Generates a random string used for passwords.
        /// </summary>
        /// <param name="length">Length, integer, of the number of password characters.</param>
        /// <returns>Random string of parameter length.</returns>
        public static string GenerateRandomString(int length)
        {
            //Removed O, o, 0, l, 1 
            string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            string allowedNumberChars = "23456789";
            char[] chars = new char[length];
            Random rd = new Random();

            bool useLetter = true;
            for (int i = 0; i < length; i++)
            {
                if (useLetter)
                {
                    chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                    useLetter = false;
                }
                else
                {
                    chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                    useLetter = true;
                }

            }

            return new string(chars);
        }

        public void InitUser(Guid userId, ReferringSite referringSiteValue)
        {
            this.userId = userId;
            this.referringSiteValue = referringSiteValue;
            moduleName = "User";
            licenseAgreementValue = (LicenseAgreementStatus)ActiveUserDalc.GetLicenseValue(userId);
            emailAddress = ActiveUserDalc.GetEmailAddress(userId);
            //organizationId = ActiveUserDalc.GetOrganizationId(userId);
            //admin = ActiveUserDalc.GetAdmin(userId);
            //displayName = ActiveUserDalc.GetDisplayName(userId);
           // subscriptionStatus = (SubscriptionStatusEnum)ActiveUserDalc.GetSubscriptionStatus(userId);
            //Organazation org = new Organazation();
            //_organization = org.GetSubscriptionById(organizationId);
        }

      
        //public Guid GetUserIdByMembership(string membershipId)
        //{
        //    return ActiveUserDalc.GetUserIdByMembership(membershipId);
        //}

        public bool IsTrialLapsed()
        {
            return ActiveUserDalc.IsTrialLapsed(UserId);
        }

        //public void UpdateSubscriptionStatus(SubscriptionStatusEnum status)
        //{
        //    string retval = ActiveUserDalc.UpdateSubscriptionStatus(status, UserId);
        //    if (retval.Length > 0)
        //    {
        //        //TODO Throw error
        //    }
            
        //}

        public int GetTrialLapsedDays()
        {
            return ActiveUserDalc.GetTrialLapsedDays(UserId);
        }


        public DateTime LastLoginDateByEmail(string email)
        {
            return ActiveUserDalc.GetLastLoginByEmail(email);
        }


        //
        #endregion IUser Methods		


        #endregion Methods
    }
}