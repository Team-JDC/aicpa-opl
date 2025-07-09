using System;
using System.Collections.Generic;
using System.Linq;

using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

namespace AICPA.Destroyer.User.UserPreferences
{
    public class UserPreferenceDalc : DestroyerDalc, IUserPreferenceDalc
    {
        #region Constants

        #region Dalc Errors

        private const string ERROR_GETPREFERENCE = "Error getting a saved user preference by ID.";
        private const string ERROR_GETPREFERENCEFORUSER = "Error getting a saved user preference by UserId and Key.";
        private const string ERROR_GETPREFERENCESFORUSER = "Error getting all of a user's preferences.";

        #endregion

        #region Module and Method Names

        private const string MODULE_USERPREFERENCEDALC = "UserPreferenceDalc";
        private const string METHOD_GETPREFERENCE = "GetPreference";
        private const string METHOD_GETPREFERENCEFORUSER = "GetPreferenceForUser";
        private const string METHOD_GETPREFERENCESFORUSER = "GetPreferencesForUser";

        #endregion Module and Method Names

        #region Stored Procedures

        private const string SP_INSERTUSERPREFERENCE = "dbo.D_InsertUserPreference";
        private const string SP_UPDATEUSERPREFERENCE = "dbo.D_UpdateUserPreference";
        private const string SP_DELETEUSERPREFERENCE = "dbo.D_DeleteUserPreference";
        private const string SP_GETUSERPREFERENCE = "dbo.D_GetUserPreference";
        private const string SP_GETUSERPREFERENCES = "dbo.D_GetUserPreferences";
        private const string SP_GETUSERPREFERENCEBYUSERIDANDKEY = "dbo.D_GetUserPreferenceByUserIdAndKey";

        #endregion Stored Procedures

        #endregion Constants

        #region Constructors

        public UserPreferenceDalc()
        {
            moduleName = MODULE_USERPREFERENCEDALC;
        }

        #endregion Constructors

        #region IUserPreferenceDalc Members

        public UserPreferencesDS.D_UserPreferencesRow GetPreference(int preferenceId)
        {
            UserPreferencesDS dataSet = new UserPreferencesDS();
            FillDataset(METHOD_GETPREFERENCE, ERROR_GETPREFERENCE, SP_GETUSERPREFERENCE, dataSet, 
                        new [] { dataSet.D_UserPreferences.TableName }, preferenceId);

            return dataSet.D_UserPreferences.SingleOrDefault();
        }

        public UserPreferencesDS.D_UserPreferencesRow GetPreferenceForUser(Guid userId, string key)
        {
            UserPreferencesDS dataSet = new UserPreferencesDS();
            FillDataset(METHOD_GETPREFERENCEFORUSER, ERROR_GETPREFERENCEFORUSER, SP_GETUSERPREFERENCEBYUSERIDANDKEY,
                        dataSet, new[] { dataSet.D_UserPreferences.TableName }, userId, key);

            return dataSet.D_UserPreferences.SingleOrDefault();
        }

        public IEnumerable<UserPreferencesDS.D_UserPreferencesRow> GetPreferencesForUser(Guid UserId)
        {
            UserPreferencesDS dataSet = new UserPreferencesDS();
            FillDataset(METHOD_GETPREFERENCESFORUSER, ERROR_GETPREFERENCESFORUSER, SP_GETUSERPREFERENCES,
                        dataSet, new[] { dataSet.D_UserPreferences.TableName }, UserId);

            return (UserPreferencesDS.D_UserPreferencesRow[])dataSet.D_UserPreferences.Select();
        }

        public int Save(int preferenceId, Guid userId, string key, string value)
        {
            int returnId;

            if (preferenceId == 0)
            {
                var noClueWhyThisIsDecimal = SqlHelper.ExecuteScalar(DBConnectionString, SP_INSERTUSERPREFERENCE,
                                                                     userId, key, value);

                returnId = Convert.ToInt32(noClueWhyThisIsDecimal);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_UPDATEUSERPREFERENCE,
                                          preferenceId, userId, key, value);

                returnId = preferenceId;
            }

            return returnId;
        }

        public void DeleteUserPreference(int preferenceId)
        {
            SqlHelper.ExecuteNonQuery(DBConnectionString, SP_DELETEUSERPREFERENCE, preferenceId);
        }

        #endregion
    }
}
