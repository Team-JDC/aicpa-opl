using System;
using System.Collections.Generic;

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  An Interface that represents the datalayer for a user object.
    ///</summary>
    public interface IUserDalc
    {
        #region Methods

        ///<summary>
        ///  Insert a user into the D_User table.
        ///</summary>
        ///<param name = "userId">The id of the user to be inserted.</param>

        void SaveUser(Guid userId, string referringSite, string emailAddress);

        void SaveUser(Guid userId, string referringSite, string emailAddress, string firstName, string lastName);

        //void CreateUser(string emailAddress, string password);
        void CreateUser(Guid userId, string referringSite, string emailAddress, string password, string firstName, string lastName);

        /// <summary>
        ///  
        /// </summary>
        /// <param name="membershipId"></param>
        /// <param name="password"></param>
        /// <param name="referingSite"></param>
        /// <param name="organizationId"></param>
        /// <param name="subscriptionStatus"></param>        
        void CreateUserMembership(string membershipId, string password, ReferringSite referingSite);

        /// <summary>
        /// Method to retrieve users for a specific organization
        /// </summary>
        /// <param name="organizationId">Organization ID number</param>
        /// <returns>Returns a generic list of Users</returns>
        //List<IUser> GetUsers(int organizationId);

        Guid Login(string username, string password);

        /// <summary>
        /// Gets the license value.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        int GetLicenseValue(Guid userId);

        /// <summary>
        /// Sets the license value.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="statusValue">The status value.</param>
        /// <remarks></remarks>
        void SetLicenseValue(Guid userId, int statusValue);

        /// <summary>
        /// Gets the email address.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string GetEmailAddress(Guid userId);

        /// <summary>
        /// Gets the user's display name.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string GetDisplayName(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetOrganizationId(Guid userId);

        bool GetAdmin(Guid userId);

        /// <summary>
        /// Get user subscription by userId
        /// </summary>
        /// <param name="id">UserId as a guid</param>
        /// <returns>the subscription as a string</returns>
        //string getSubscriptionByUserId(Guid id);

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //string getMainSubscriptionByUserId(Guid id);

        ///
        ///<summary>
        ///</summary>
        ///<param name="userName"></param>
        ///<returns>string message</returns>
        string ResetPassword(string userName, string password);

        string SendUserPassword(string userName);

        //Guid GetUserIdByMembership(string membershipId);

        bool IsTrialLapsed(Guid userId);

        /// <summary>
        /// Get the Subscription Status
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        //int GetSubscriptionStatus(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //string UpdateSubscriptionStatus(SubscriptionStatusEnum status, Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetTrialLapsedDays(Guid userId);

        /// <summary>
        /// Grab the last login value by Email Address. If the value doesn't exist it will be DateTime.MinValue
        /// </summary>
        /// <param name="email">email address of user</param>
        /// <returns>DateTime value </returns>
        DateTime GetLastLoginByEmail(string email);

        /// <summary>
        /// Get the current SessionId from the database for a given email and referringSite.
        /// </summary>
        /// <param name="email">Users email address</param>
        /// <param name="referringSite">Users referringSite</param>
        /// <returns>The session id of the user if an error occurs then a Empty Guid is returned</returns>
        string GetSessionByEmail(string email, string referringSite);

        /// <summary>
        /// Get UserId by Email and current sessionId
        /// </summary>
        /// <param name="email">Email address to lookup</param>
        /// <param name="sessionId">Current SessionId for Email</param>
        /// <returns>UserId</returns>
        Guid GetUserIdByEmailAndSessionId(string email, string sessionId);

        /// <summary>
        /// Will update a specific user's sessionId.  This is to help with the cases where the sessionId = -1
        /// </summary>
        /// <param name="email">Email address of user</param>
        /// <param name="referringSite">User's Referring Site</param>
        /// <param name="newSessionId">New SessionId value</param>
        void UpdateUserSessionId(string email, string referringSite, string newSessionId);

        string GetReferringSiteByEmail(string email);
        /// <summary>
        /// Check to see if a users exists by Email
        /// </summary>
        /// <param name="email">email/username</param>
        /// <returns></returns>
        bool UserExists(string email);

        /// <summary>
        /// Get a userId given an email address
        /// </summary>
        /// <param name="email">Email/username</param>
        /// <returns></returns>
        Guid GetUserIdByEmail(string email);

        /// <summary>
        /// Get a userId given an email address
        /// </summary>
        /// <param name="email">Email/username</param>
        /// <returns></returns>
        Guid GetUserIdByEmailAndReferringSite(string email, string referringSite);


        string FindUserEmailByUsername(string username);

         #endregion Methods
    }
}