using System;
using System.Collections.Generic;

using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.User.UserPreferences;

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  An enumeration of the possible referring sites.
    ///</summary>
    public enum ReferringSite
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        C2b = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Exams = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Csc = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Ceb = 4,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Lms = 5,
        /// <summary>
        /// 
        /// </summary>
        Other = 6,
        /// <summary>
        /// 
        /// </summary>
        Mcgdy = 7,
        /// <summary>
        /// 
        /// </summary>
        Mcgdyasc = 8,
        /// <summary>
        /// Forensic Valuation Services
        /// </summary>
        Fvs = 9,
        /// <summary>
        /// Ethics Anonymous Project
        /// </summary>
        Ethics = 10,
        /// <summary>
        /// Ethics User
        /// </summary>
        EthicsUser= 11,
        /// <summary>
        /// Personal Finance User from .org
        /// </summary>
        Pfp = 12,
        /// <summary>
        /// Authenticated COSO user
        /// </summary>
        Coso = 13,
        /// <summary>
        /// User from the RAVE system which replaced the C2b store
        /// </summary>
        Rave=14,
        /// <summary>
        /// A second set of users coming through for exams
        /// </summary>
        ExamsNew=15
    }

    /// <summary>
    /// 	
    /// </summary>
    public enum LicenseAgreementStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        InitialDefault = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Agreed = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Declined = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SubscriptionStatusEnum
    {
        /// <summary>
        /// Trial users get 15 days
        /// </summary>
        Trial = 0,

        /// <summary>
        /// User has active subscription to site
        /// </summary>
        Active = 1,

        /// <summary>
        /// User subscription expired
        /// </summary>
        Expired = 2
    }


    /// <summary>
    ///   The IUser interface provides properties and methods for creating managing and saving
    ///   a user object.  It also exposes all necessary methods for managaing security by storing
    ///   an IUserSecurity interface as a property.  All personalization methods will be exposed
    ///   through this interface as all of these features occur in the context of a user.
    /// </summary>
    public interface IUser
    {
        #region Properties

        /// <summary>
        ///   An encrypted version of the user Id.
        /// </summary>
        string EncryptedUserId { get; }

        /// <summary>
        ///   Unique Userid Provided from the referring site or from our own login.
        ///   Should be verified against the AICPA data wharehouse.
        /// </summary>
        Guid UserId { get; }

        ///<summary>
        ///  The site the user came from.
        ///</summary>
        ReferringSite ReferringSiteValue { get; }

        /// <summary>
        ///   Interface that is exposed on the user to provide all necessary security
        ///   methods and properties.
        /// </summary>
        IUserSecurity UserSecurity { get; }

        IUserDalc ActiveUserDalc { get; }

        ///// <summary>
        ///// Property that stores a users subscription notifications
        ///// </summary>
        //IBookCollection NotificationSubscriptions { get; set; }

        ///// <summary>
        ///// Property that stores a users saved searches
        ///// </summary>
        //ISearchCriteriaCollection Searches	{ get; set; }

        ///// <summary>
        ///// Property that stores a collection of notes for the user
        ///// </summary>
        //INoteCollection Notes { get; set; }

        ///// <summary>
        ///// Property that stores a collection of Documents the user has request during 
        ///// the existing session
        ///// </summary>
        //IDocumentCollection DocumentHistory { get; set; }

        /// <summary>
        /// Gets the email address.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        string EmailAddress { get; set; }

        /// <summary>
        /// Gets the first name
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Gets the last name
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Gets the display name
        /// </summary>
        //string DisplayName { get; set; }

        /// <summary>
        /// gets the boolean value showing whether admin
        /// </summary>
        //bool Admin { get;}

        /// <summary>
        /// gets the boolean value showing whether active
        /// </summary>
        //bool Active { get; set; }

        /// <summary>
        /// Gets the license agreement value.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        LicenseAgreementStatus LicenseAgreementValue { get; }

        UserPreferencesDictionary Preferences { get; }

        SearchCollection SavedSearches { get; }

        /// <summary>
        /// Membership ID - ID to uniquely identify member.
        /// </summary>
        //string MembershipId { get; set; }

        /// <summary>
        /// Status of user
        /// </summary>
        //SubscriptionStatusEnum SubscriptionStatus { get; set; }

        string OktaId { get; set; }

        string RoleId { get; set; }

        #region GASB Specific
        
        /// <summary>
        /// GASB AccountID used for the concurrency
        /// initially used for GASB
        /// </summary>
        string AccountId { get; set; }

        /// <summary>
        /// Define the conccurecny for the user account
        /// initially used for GASB
        /// </summary>
        int Concurrency { get; set; }
        #endregion

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Logs a user onto destroyer.
        ///</summary>
        ///<param name = "sessionId">The session id for the login.</param>
        void LogOn(string sessionId);

        ///<summary>
        ///  Log a user on with the security context.
        ///</summary>
        ///<param name = "sessionId">The sessionId for a given logon.</param>
        ///<param name = "domain">The domain string indicating the subscriptions a user has access to.</param>
        void LogOn(string sessionId, string domain);

        ///<summary>
        ///  Logs a user off of destroyer.
        ///</summary>
        void LogOff();

        /// <summary>
        /// Sets the license agreement value.	
        /// </summary>
        /// <param name="statusValue">The status value.</param>
        /// <remarks></remarks>
        void SetLicenseAgreementValue(LicenseAgreementStatus statusValue);

        /// <summary>
        /// Return users sessionId
        /// </summary>
        /// <returns>Current sessionId</returns>
        string GetSessionId();

        /// <summary>
        /// Used to update the sessionId for a user. There are cases when the sessionId is -1.  For the 
        /// Authentication piece, we need to change it from -1 to something unique.  
        /// </summary>
        /// <param name="sessionId">New sessionId</param>
        void UpdateSessionId(string sessionId);

        /// <summary>
        /// Resets the user's password and then emails the password to the user.
        /// </summary>
        /// <param name="userName">email address for the user. Used as a user name.</param>
        /// <returns>Returns an empty string or an error message.</returns>
        string ResetPassword(string userName);

        /// <summary>
        /// Send the user's password to them in an email.
        /// </summary>
        /// <param name="userName">email address for the user. Used as a user name.</param>
        /// <returns>Returns an empty string or an error message.</returns>
        string SendUserPassword(string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="organizationId"></param>
        void CreateUser(Guid userId, string referringSite, string emailAddress, string password, string firstName, string lastName);
        

        /// <summary>
        /// Create a user with membershipId
        /// </summary>
        /// <param name="membershipId">Membership ID</param>
        /// <param name="password">User password</param>
        /// <param name="referingSite">Refering Site</param>
        /// <param name="organizationId">Assigned Organization ID</param>
        /// <param name="subscriptionStatus">Subscription Status</param>
        void CreateUserMembership(string membershipId, string password, ReferringSite referingSite);

        /// <summary>
        /// Method to edit user information
        /// </summary>
        /// <param name="userId">The unique user ID</param>
        /// <param name="emailAddress">The user's email address</param>
        /// <param name="organizationId">The ID for the organization linked record</param>
        /// <param name="password">The password. If this is empty or null then the password won't be updated.</param>
        /// <param name="firstName">The first name of the user</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="displayName">The display name for the user.</param>
        /// <param name="active">The boolean indicator whether the user is active.</param>
        void EditUser(Guid userId, string emailAddress, int organizationId, string password, string firstName, string lastName, string displayName, bool active);

        /// <summary>
        /// Method to retrieve users for a specific organization
        /// </summary>
        /// <param name="organizationId">Organization ID number</param>
        /// <returns>Returns a generic list of Users</returns>
        //List<IUser> GetUsers(int organizationId);

        Guid Login(string username, string password, string SessionId, string overrideSubscription, ReferringSite referringSiteValue);
        
        void InitUser(Guid userId, ReferringSite referringSiteValue);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="membershipId"></param>
        /// <returns></returns>
        //Guid GetUserIdByMembership(string membershipId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsTrialLapsed();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        //void UpdateSubscriptionStatus(SubscriptionStatusEnum status);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetTrialLapsedDays();

        DateTime LastLoginDateByEmail(string email);

        #endregion Methods
    }
}