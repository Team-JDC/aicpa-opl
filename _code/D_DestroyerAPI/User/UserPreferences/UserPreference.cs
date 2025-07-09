using System;

using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.User.UserPreferences
{
    public class UserPreference: DestroyerBpc, IUserPreference
    {
        #region Private Fields

        private IUserPreferenceDalc _activeUserPreferenceDalc;

        private IUserPreferenceDalc ActiveUserPreferenceDalc
        {
            get { return _activeUserPreferenceDalc ?? (_activeUserPreferenceDalc = new UserPreferenceDalc()); }
        }

        #endregion Private Fields

        #region Properties

        public int PreferenceId { get; private set; }

        public Guid UserId { get; private set; }

        public string Key { get; private set; }

        public string Value { get; set; }

        #endregion Properties

        #region Constructors

        public UserPreference(Guid userId, string key, string value)
        {
            UserId = userId;
            Key = key;
            Value = value;
        }

        public UserPreference(UserPreferencesDS.D_UserPreferencesRow preferenceRow)
        {
            PreferenceId = preferenceRow.UserPreferenceId;
            UserId = preferenceRow.UserId;
            Key = preferenceRow.Key;
            Value = preferenceRow.Value;
        }

        #endregion Constructors

        #region Methods

        public void Save()
        {
            PreferenceId = ActiveUserPreferenceDalc.Save(PreferenceId, UserId, Key, Value);
        }

        public static UserPreferencesDictionary GetPreferencesForUser(Guid userId)
        {
            return new UserPreferencesDictionary(userId, new UserPreferenceDalc().GetPreferencesForUser(userId));
        }

        public static void DeletePreferenceById(int preferenceId)
        {
            new UserPreferenceDalc().DeleteUserPreference(preferenceId);
        }

        #endregion
    }
}
