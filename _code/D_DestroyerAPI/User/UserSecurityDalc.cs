#region

using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace AICPA.Destroyer.User
{
    ///<summary>
    ///  The data-access layer for UserSecurity information and settings.
    ///</summary>
    public class UserSecurityDalc : DestroyerDalc, IUserSecurityDalc
    {
        #region Constants

        #region Stored Procedures

        private const string SP_INSERTCURRENTFIRMUSER = "D_InsertCurrentFirmUser";
        private const string SP_ISUSERSESSIONVALID = "D_IsUserSessionValid";
        private const string SP_CREATEUSERSESSION = "D_CreateUserSession";
        private const string SP_GETSUBSCRIPTIONBOOKNAMES = "D_GetSubscriptionBookNames";
        private const string SP_ENDUSERSESSION = "D_EndUserSession";

        #endregion Stored Procedures

        #region Dalc Errors

        private const string ERROR_ENDUSERSESSION = "Error ending a user session.";
        private const string ERROR_ISSESSIONIDVALID = "Error checking sessionId.";
        private const string ERROR_CREATEUSERSESSION = "Error creating a user session.";
        private const string ERROR_GETSUBSCRIPTIONBOOKNAMES = "Error getting subscription book names.";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_USERSECURITYDALC = "UserSecurityDalc";
        private const string METHOD_ENDUSERSESSION = "EndUserSession";
        private const string METHOD_ISSESSIONIDVALID = "IsSessionIdValid";
        private const string METHOD_CREATEUSERSESSION = "CreateUserSession";
        private const string METHOD_GETSUBSCRIPTIONBOOKNAMES = "GetSubscriptionBookNames";

        #endregion Module and Method Names		

        #endregion Constants

        #region Constructors

        // only constructed internally
        internal UserSecurityDalc()
        {
            moduleName = MODULE_USERSECURITYDALC;
        }

        #endregion Constructors

        #region Methods

        #region IUserSecurityDalc Methods

        ///<summary>
        ///  End the user session by setting the sessionid to -1 and deleting current firms information.
        ///</summary>
        ///<param name = "userDs">The dataset of the user.</param>
        public void EndUserSession(UserDs userDs)
        {
            foreach (UserDs.UsersRow usersRow in userDs.Users)
            {
                ExecuteNonQuery(METHOD_ENDUSERSESSION, ERROR_ENDUSERSESSION, SP_ENDUSERSESSION, usersRow.UserId,
                                usersRow.CurrentSessionId);
            }
        }

        ///<summary>
        ///  Check if the user session is still valid.
        ///</summary>
        ///<param name = "userId">The user's id</param>
        ///<param name = "sessionId">The user's sessionId</param>
        ///<returns>A bool that indicates if the session is valid or not.</returns>
        public bool IsSessionIdValid(Guid userId, string sessionId)
        {
            return
                (bool)
                ExecuteScalar(METHOD_ISSESSIONIDVALID, ERROR_ISSESSIONIDVALID, SP_ISUSERSESSIONVALID, userId, sessionId);
        }

        ///<summary>
        ///  Creates a new user session and overrides previous sessions.
        ///</summary>
        ///<param name = "userDs">A strongly-typed dataset for a user.</param>
        public void CreateUserSession(UserDs userDs)
        {
            SqlCommand insertUserSqlCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                      SP_CREATEUSERSESSION,
                                                                      userDs.Users.UserIdColumn.ColumnName,
                                                                      userDs.Users.LastLoginTimeColumn.ColumnName,
                                                                      userDs.Users.CurrentSessionIdColumn.ColumnName);
            UpdateDataset(METHOD_CREATEUSERSESSION, ERROR_CREATEUSERSESSION, insertUserSqlCommand, null, null, userDs,
                          userDs.Users.TableName);

            SqlCommand insertFirmSqlCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                      SP_INSERTCURRENTFIRMUSER,
                                                                      userDs.CurrentFirmUsers.ACAColumn.ColumnName,
                                                                      userDs.CurrentFirmUsers.CodeColumn.ColumnName,
                                                                      userDs.CurrentFirmUsers.UserIdColumn.ColumnName);
            UpdateDataset(METHOD_CREATEUSERSESSION, ERROR_CREATEUSERSESSION, insertFirmSqlCommand, null, null, userDs,
                          userDs.CurrentFirmUsers.TableName);
        }

        ///<summary>
        ///  Gets the names of the books that the domainString has access to.
        ///</summary>
        ///<param name = "domainString"></param>
        ///<param name = "domainStringDelimiter"></param>
        ///<returns></returns>
        public string[] GetSubscriptionBookNames(string domainString, char domainStringDelimiter)
        {
            // postpend the string with the delimiter since the stored procedure wants it that way anyway.
            if (!domainString.EndsWith(domainStringDelimiter.ToString()))
            {
                domainString += domainStringDelimiter;
            }
            // count the number of times the delimiter is in the string.  The stored procedure uses this value
            // as a check to know when it should break out of the loop in case of an error.
            int subscriptionCountCode = domainString.ToCharArray().Count(c => c == domainStringDelimiter);
            ArrayList bookNames = new ArrayList();
            using (
                SqlDataReader sqlDataReader = ExecuteReader(METHOD_GETSUBSCRIPTIONBOOKNAMES,
                                                            ERROR_GETSUBSCRIPTIONBOOKNAMES, SP_GETSUBSCRIPTIONBOOKNAMES,
                                                            domainString, domainStringDelimiter, subscriptionCountCode))
            {
                while (sqlDataReader.Read())
                {
                    bookNames.Add(sqlDataReader.GetString(0));
                }
            }
            return (string[]) bookNames.ToArray(Type.GetType("System.String"));
            ;
        }

        #endregion IUserSecurityDalc Methods

        #endregion Methods
    }
}