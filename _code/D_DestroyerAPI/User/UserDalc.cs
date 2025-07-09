using System;

using AICPA.Destroyer.Shared;
using System.Configuration;

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  The data-access layer for User information and settings.
    ///</summary>
    public class UserDalc : DestroyerDalc, IUserDalc
    {
        #region Constants

        #region Stored Procedures

        private const string SP_INSERTUSER = "dbo.D_InsertUser";        
        private const string SP_INSERTUSER3= "dbo.D_InsertUser3";        
        private const string SP_CREATEUSER = "dbo.D_CreateUser";
        private const string SP_UPDATEUSERLICENSEFLAG = "dbo.D_UpdateUserLicenseFlag";
        private const string SP_GETUSERLICENSEFLAG = "dbo.D_GetUserLicenseFlag";
        private const string SP_GETUSEREMAILADDRESS = "dbo.D_GetUserEmailAddress";
        private const string SP_CREATEUSERMEMBERSHIP = "dbo.D_CreateUserMembership";
        
        #endregion Stored Procedures

        #region Dalc Errors

        private const string ERROR_SAVEUSER = "Error saving a user.";
        private const string ERROR_SETLICENSEFLAG = "Error setting user license flag.";
        private const string ERROR_GETLICENSEFLAG = "Error getting user license flag.";
        private const string ERROR_GETEMAILADDRESS = "Error getting user email address.";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_USERDALC = "UserDalc";
        private const string METHOD_SAVEUSER = "SaveUser";
        private const string METHOD_SETLICENSEFLAG = "SetLicenseFlag";
        private const string METHOD_GETLICENSEFLAG = "GetLicenseFlag";
        private const string METHOD_GETEMAILADDRESS = "GetEmailAddress";

        #endregion Module and Method Names

        private int _TrialPeriod = 15;//default

        #endregion Constants

        #region Constructors

        // can only be constructed internally
        internal UserDalc()
        {
            moduleName = MODULE_USERDALC;

            if (ConfigurationManager.AppSettings["TrialPeriod"] != null)
                _TrialPeriod = Int32.Parse(ConfigurationManager.AppSettings["TrialPeriod"]);
        }

        #endregion Constructors

        #region Methods

        ///<summary>
        ///  Save the user to the database.
        ///</summary>
        ///<param name = "userId">The user's id</param>
        public void SaveUser(Guid userId, string referringSite, string emailAddress)
        {
            ExecuteNonQuery(METHOD_SAVEUSER, ERROR_SAVEUSER, SP_INSERTUSER, userId, referringSite, emailAddress);
        }

        public void SaveUser(Guid userId, string referringSite, string emailAddress, string firstName, string lastName)
        {
            ExecuteNonQuery(METHOD_SAVEUSER, ERROR_SAVEUSER, SP_INSERTUSER3, userId, referringSite, emailAddress, firstName, lastName);
        }

        public void CreateUser(Guid userId, string referringSite, string emailAddress, string password, string firstName, string lastName)
        {
            string newPassword = Security.EncryptString(password, Security.GetEncryptionKey());//Security.EncodePassword(password);

            ExecuteNonQuery("CreateUser", "Error creating user.", SP_CREATEUSER, userId, referringSite, emailAddress, newPassword, firstName, lastName);
        }

        public void CreateUserMembership(string membershipId, string password, ReferringSite referingSite)
        {
            string newPassword = Security.EncryptString(password, Security.GetEncryptionKey()); //Security.EncodePassword(password);
            ExecuteNonQuery("CreateUserMembership", "Error creating user.", SP_CREATEUSERMEMBERSHIP, membershipId, newPassword, referingSite);            
        }

        /// <summary>
        /// Method to retrieve users for a specific organization
        /// </summary>
        /// <param name="organizationId">Organization ID number</param>
        /// <returns>Returns a generic list of Users</returns>
        //public List<IUser> GetUsers(int organizationId)
        //{
        //    UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetDataByOrgId(organizationId);

        //    return (from row in table
        //               select new User
        //                {
        //                    UserId = row.UserId,
        //                    EmailAddress = row.Email,
        //                    OrganizationId = row.OrganizationId,
        //                    FirstName = row.FirstName,
        //                    LastName = row.LastName,
        //                    DisplayName = row.DisplayName,
        //                    Admin = row.Admin,
        //                    Active = row.Active
        //                }).ToList();
        //}

        public Guid Login(string username, string password)
        {
            string newPassword = Security.EncryptString(password, Security.GetEncryptionKey());//Security.EncodePassword(password);

            //FormsDS.D_UserFormsDataTable table = new FormsDSTableAdapters.D_UserFormsTableAdapter().SelectAllByUserId(UserId);
            UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().Login(username, newPassword);

            if (table.Count > 0)
            {
                return ((UserDSNew.D_UserRow)table.Rows[0]).UserId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        //public string getMainSubscriptionByUserId(Guid id)
        //{
        //    UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserById(id);

        //    if(table.Count > 0)
        //    {
        //        int OrgId = ((UserDSNew.D_UserRow)table.Rows[0]).OrganizationId;

        //        UserDSNew.D_OrganizationDataTable tableOrg = new UserDSNewTableAdapters.D_OrganizationTableAdapter().GetOrganizationById(OrgId);


        //        if(tableOrg.Count > 0)
        //        {
        //            return ((UserDSNew.D_OrganizationRow)tableOrg.Rows[0]).Subscription;
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
                
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        //public string getSubscriptionByUserId(Guid id)
        //{
        //    UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserById(id);

        //    if(table.Count > 0)
        //    {
        //        int OrgId = ((UserDSNew.D_UserRow)table.Rows[0]).OrganizationId;

        //        UserDSNew.D_OrganizationDataTable tableOrg = new UserDSNewTableAdapters.D_OrganizationTableAdapter().GetOrganizationById(OrgId);


        //        if(tableOrg.Count > 0)
        //        {
        //            string subscriptions = ((UserDSNew.D_OrganizationRow)tableOrg.Rows[0]).Subscription;

        //            if (!string.IsNullOrEmpty(((UserDSNew.D_OrganizationRow)tableOrg.Rows[0]).AdditionalSubscriptions))
        //            {
        //                subscriptions += "~" + ((UserDSNew.D_OrganizationRow)tableOrg.Rows[0]).AdditionalSubscriptions;
        //            }
        //            return subscriptions;
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
                
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        /// <summary>
        /// Sets the license value.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="statusValue">The status value.</param>
        /// <remarks></remarks>
        public void SetLicenseValue(Guid userId, int statusValue)
        {
            ExecuteNonQuery(METHOD_SETLICENSEFLAG, ERROR_SETLICENSEFLAG, SP_UPDATEUSERLICENSEFLAG, userId, statusValue);
        }

        /// <summary>
        /// Gets the license value.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetLicenseValue(Guid userId)
        {
            return (int) ExecuteScalar(METHOD_GETLICENSEFLAG, ERROR_GETLICENSEFLAG, SP_GETUSERLICENSEFLAG, userId);
        }

        public int GetOrganizationId(Guid userId)
        {
            object retVal = ExecuteScalar("GetOrganizationId", "GetOrganizationId Error", "D_GetUserOrganizationId", userId);

            if (retVal is DBNull)
            {
                return 0;
            }
            else
            {
                return (int)retVal;
            }
        }

        public bool GetAdmin(Guid userId)
        {
            object retVal = ExecuteScalar("GetUserAdmin", "GetAdmin Error", "D_GetUserAdmin", userId);
            if (retVal is DBNull)
            {
                return false;
            }
            else
            {
                return (bool)retVal;
            }
        }

        public string GetDisplayName(Guid userId)
        {
            object retVal = ExecuteScalar("GetDisplayName", "GetDisplayName error", "D_GetUserDisplayName", userId);
            
            if (retVal is DBNull)
            {
                return null;
            }
            else
            {
                return (string) retVal;
            }
        }

        /// <summary>
        /// Gets the email address.	
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetEmailAddress(Guid userId)
        {
            object retVal = ExecuteScalar(METHOD_GETEMAILADDRESS, ERROR_GETEMAILADDRESS, SP_GETUSEREMAILADDRESS, userId);

            if (retVal is DBNull)
            {
                return null;
            }
            else
            {
                return (string) retVal;
            }
        }

        /// <summary>
        /// Sets the password for a specific username
        /// </summary>
        /// <param name="username">The user to reset</param>
        /// <param name="password">The password to reset to</param>
        /// <returns>Returns either an empty string or an error message.</returns>
        public string ResetPassword(string username, string password)
        {
            string retVal = "";

            string newPassword = Security.EncryptString(password, Security.GetEncryptionKey());//Security.EncodePassword(password);

            try
            {
                int whyInt = new UserDSNewTableAdapters.D_UserTableAdapter().UpdatePassword(newPassword, username);
                string ProductName = (ConfigurationManager.AppSettings["ProductName"] != null ? ConfigurationManager.AppSettings["ProductName"] :  "Ethics"); // TODO: Load from Web.Config
                UserMail.Send("Your " + ProductName + " password was reset", "Your password to " + ProductName + " was reset. Your new password is " + password + ". Please use this password when you login the next time.", username);
            }
            catch (Exception e)
            {   
                retVal = e.Message;
            }

            return retVal;
        }

        /// <summary>
        /// Sets the password for a specific username
        /// </summary>
        /// <param name="username">The user to reset</param>
        /// <param name="password">The password to reset to</param>
        /// <returns>Returns either an empty string or an error message.</returns>
        public string SendUserPassword(string username)
        {
            string retVal = "";
            string userPassword = string.Empty;
            string firstName = string.Empty;
            //string newPassword = Security.EncryptString(password, Security.GetEncryptionKey());//Security.EncodePassword(password);
            UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserByEmail(username);
            if (table.Count > 0)
            {
                userPassword = ((UserDSNew.D_UserRow)table.Rows[0]).Password;
                firstName = ((UserDSNew.D_UserRow)table.Rows[0]).FirstName;
            }

            string dUserPassword = Security.DecryptString(userPassword, Security.GetEncryptionKey());
            try
            {               
                string ProductName = (ConfigurationManager.AppSettings["ProductName"] != null ? ConfigurationManager.AppSettings["ProductName"] : "Ethics"); // TODO: Load from Web.Config
                string emailText = "Dear " + firstName + ", \r\n\r\nPer your requested, your password is being mailed to you.\r\n\r\nPassword: " + dUserPassword + "\r\n" +
                    "\r\nIf you did not request this password please ignore.\r\n\r\nThanks,\r\nAdmin";
                UserMail.Send(ProductName +" password.",emailText,username);
            }
            catch (Exception e)
            {
                retVal = e.Message;
            }

            return retVal;
        }

        //public Guid GetUserIdByMembership(string membershipId)
        //{
        //    try
        //    {
        //        UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().SelectUserByMembershipId(membershipId);
        //        if (table.Count > 0)
        //        {
        //            return ((UserDSNew.D_UserRow)table.Rows[0]).UserId;
        //        }
        //        else
        //        {
        //            return Guid.Empty;
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return Guid.Empty;
        //    }

        //}

        public bool IsTrialLapsed(Guid userId)
        {
            object retVal = ExecuteScalar("GetTrialLapsedDays", "GetAdmin Error", "D_GetTrialLapsedDays", userId);
            if (retVal is DBNull)
            {
                return false;//we can't find them in the system, so they must be a trial user
            }
            else
            {
                return ((int)retVal >= _TrialPeriod);
            }
        }

        public int GetTrialLapsedDays(Guid userId)
        {
            var trial = new UserDSNewTableAdapters.D_UserTableAdapter().GetTrialLapsedDays(userId);
            if (trial == null)
                return 0;
            return (int)trial;
        }

        /// <summary>
        /// Grab the last login value by Email Address. If the value doesn't exist it will be DateTime.MinValue
        /// </summary>
        /// <param name="email">email address of user</param>
        /// <returns>DateTime value </returns>
        public DateTime GetLastLoginByEmail(string email)
        {
            DateTime retVal = DateTime.MinValue;
            try
            {
                UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserByEmail(email);
                if (table.Count > 0)
                {
                    retVal = ((UserDSNew.D_UserRow)table.Rows[0]).LastLogin;
                }
            }
            catch (Exception e)
            {
                retVal = DateTime.MinValue;
            }

            return retVal; 
        }

        /// <summary>
        /// Get the current SessionId from the database for a given email and referringSite.
        /// </summary>
        /// <param name="email">Users email address</param>
        /// <param name="referringSite">Users referringSite</param>
        /// <returns>The session id of the user if an error occurs then a Empty Guid is returned</returns>
        public string GetSessionByEmail(string email, string referringSite)
        {
            try
            {
                UserDSNewTableAdapters.D_GetUserSessionByEmailTableAdapter tA = new UserDSNewTableAdapters.D_GetUserSessionByEmailTableAdapter();
                UserDSNew.D_GetUserSessionByEmailDataTable table = tA.GetData(email, referringSite);

                if (table.Rows.Count == 0)
                {
                    SaveUser(Guid.NewGuid(), referringSite, email);
                    table = tA.GetData(email, referringSite);
                }

                if (table.Rows.Count == 1)
                {
                    return (string)table.Rows[0][0];
                }
            }
            catch // No Error handling.  If an error occurs the default Empty Guid is returned.
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Get UserId by Email and current sessionId
        /// </summary>
        /// <param name="email">Email address to lookup</param>
        /// <param name="sessionId">Current SessionId for Email</param>
        /// <returns>UserId</returns>
        public Guid GetUserIdByEmailAndSessionId(string email, string sessionId)
        {
            Guid? value = new Guid();
            try
            {
                value = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserIdByEmailAndSessionIdNew(email, sessionId);
                if (value == null)
                    return new Guid();
            }
            catch
            {

            }

            return (Guid)value;
        }

        public Guid GetUserIdByEmail(string email)
        {
            Guid? value = Guid.Empty;
            try
            {
                UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserByEmail(email);
                if (table.Count > 0)
                {
                    value = ((UserDSNew.D_UserRow)table.Rows[0]).UserId;                    
                }
            }
            catch
            {

            }

            return (Guid)value;
        }

        public Guid GetUserIdByEmailAndReferringSite(string email, string referringSite)
        {
            Guid? value = Guid.Empty;
            try
            {
                UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().GetUserIdByEmailAndReferringSite(email, referringSite);
                if (table.Count > 0)
                {
                    value = ((UserDSNew.D_UserRow)table.Rows[0]).UserId;
                }
            }
            catch
            {

            }

            return (Guid)value;
        }



        /// <summary>
        /// Will update a specific user's sessionId.  This is to help with the cases where the sessionId = -1
        /// </summary>
        /// <param name="email">Email address of user</param>
        /// <param name="referringSite">User's Referring Site</param>
        /// <param name="newSessionId">New SessionId value</param>
        public void UpdateUserSessionId(string email, string referringSite, string newSessionId)
        {
            try
            {
                int status = new UserDSNewTableAdapters.D_UserTableAdapter().UpdateSessionId(newSessionId, email, referringSite);
            }
            catch
            {

            }

        }

        /// <summary>
        /// Will return the top 1 referring site.   This is used to help us enforce the rule of 1 referring site per email
        /// </summary>
        /// <param name="email">email address to check</param>
        /// <returns>Referring site (1st one if there are multiple...)</returns>
        public string GetReferringSiteByEmail(string email)
        {
            string referringSite = string.Empty;
            try
            {
                referringSite = new NewUserDSTableAdapters.QueriesTableAdapter().GetReferringSiteByEmail(email).ToString();
            }
            catch // No Error handling.  If an error occurs the default Empty Guid is returned.
            {
            }
            return referringSite;
        }

        public bool UserExists(string email)
        {
            return GetUserIdByEmail(email) != Guid.Empty;            
        }

        /// <summary>
        /// Search for the username in the databaes.  The 'username' is the first part of an email address without the @domain.com piece
        /// .. for instance  dwatson@knowlysis.com would be 'dwatson';
        /// </summary>
        /// <param name="username">first part of email</param>
        /// <returns>Username from the system.</returns>
        public string FindUserEmailByUsername(string username)
        {
            string value = string.Empty;
            try
            {                
                UserDSNew.D_UserDataTable table = new UserDSNewTableAdapters.D_UserTableAdapter().FindUserByUsername(username);
                if (table.Count > 0)
                {
                    value = ((UserDSNew.D_UserRow)table.Rows[0]).Email;
                }
            }
            catch
            {
                ///
            }

            return value;
        }




        #endregion Methods


    }
}