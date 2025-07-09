#region

using System;
using System.Collections;
using System.Data.SqlClient;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    /// <summary>
    ///   Summary description for FirmDalc.
    /// </summary>
    public class FirmDalc : DestroyerDalc, IFirmDalc
    {
        #region Constants

        #region Stored Procedures

        private const string SP_GETFIRMUSERCOUNT = "dbo.D_GetFirmUserCount";
        private const string SP_GETFIRMCURRENTUSERS = "dbo.D_GetFirmCurrentUsers";

        #endregion Stored Procedures

        #region Dalc Errors

        private const string ERROR_GETCURRENTFIRMUSERCOUNT = "Error getting the current firm user count.";
        private const string ERROR_GETFIRMCURRENTUSERIDS = "Error getting the firm current userids.";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_FIRMDALC = "FirmDalc";
        private const string METHOD_GETCURRENTFIRMUSERCOUNT = "GetCurrentFirmUserCount";
        private const string METHOD_GETFIRMCURRENTUSERIDS = "GetFirmCurrentUserIds";

        #endregion Module and Method Names

        #endregion Constants

        #region Constructors

        internal FirmDalc()
        {
            moduleName = MODULE_FIRMDALC;
        }

        #endregion Constructors		

        #region IFirmDalc Members

        ///<summary>
        ///  Gets the firm's current user count.
        ///</summary>
        ///<param name = "aca">The Aca of the firm</param>
        ///<param name = "code">The Code of the firm</param>
        ///<param name = "sessionTimeout">
        ///  The standard timeout value in seconds for a user.  This is used 
        ///  to determine if there is a stale user and then not include that user in the count.
        ///</param>
        ///<returns>The count of current users with valid sessions.</returns>
        public int GetCurrentFirmUserCount(string aca, string code, int sessionTimeout)
        {
            return
                (int)
                ExecuteScalar(METHOD_GETCURRENTFIRMUSERCOUNT, ERROR_GETCURRENTFIRMUSERCOUNT, SP_GETFIRMUSERCOUNT, aca,
                              code, sessionTimeout);
        }

        ///<summary>
        ///  Gets the userids of the users for the firm.
        ///</summary>
        ///<param name = "aca">The Aca of the firm</param>
        ///<param name = "code">The Code of the firm</param>
        ///<returns>An array of user's ids</returns>
        public Guid[] GetFirmCurrentUserIds(string aca, string code)
        {
            ArrayList userIds = new ArrayList();
            using (
                SqlDataReader sqlDataReader = ExecuteReader(METHOD_GETFIRMCURRENTUSERIDS, ERROR_GETFIRMCURRENTUSERIDS,
                                                            SP_GETFIRMCURRENTUSERS, aca, code))
            {
                while (sqlDataReader.Read())
                {
                    userIds.Add(sqlDataReader.GetGuid(0));
                }
            }
            return (Guid[]) userIds.ToArray(Type.GetType("System.Guid"));
        }

        #endregion
    }
}