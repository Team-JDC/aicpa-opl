using System;
using System.Collections.Generic;

namespace AICPA.Destroyer.User.UserPreferences
{
    public interface IUserPreferenceDalc
    {
        UserPreferencesDS.D_UserPreferencesRow GetPreference(int preferenceId);

        UserPreferencesDS.D_UserPreferencesRow GetPreferenceForUser(Guid userId, string key);

        IEnumerable<UserPreferencesDS.D_UserPreferencesRow> GetPreferencesForUser(Guid UserId);

        int Save(int preferenceId, Guid userId, string key, string value);

        void DeleteUserPreference(int preferenceId);
    }
}
