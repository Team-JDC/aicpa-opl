using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AICPA.Destroyer.User.UserPreferences
{
    public class UserPreferencesDictionary : IDictionary<string, string>
    {
        private readonly Dictionary<string, UserPreference> preferences = new Dictionary<string, UserPreference>();
        private Guid UserId { get; set; }

        #region Constructors

        ///<summary>
        /// For new UserPreferenceCollections with no existing key/value pairs
        ///</summary>
        ///<param name="userId"></param>
        public UserPreferencesDictionary(Guid userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// For creating a UserPreferenceCollection based on existing key/value pairs stored in the database
        /// </summary>
        /// <param name="prefRows"></param>
        public UserPreferencesDictionary(Guid userId, IEnumerable<UserPreferencesDS.D_UserPreferencesRow> prefRows)
        {
            UserId = userId;

            foreach (var prefRow in prefRows)
            {
                preferences.Add(prefRow.Key, new UserPreference(prefRow));
            }
        }

        #endregion

        #region IDictionary<string,string> Members

        public void Add(string key, string value)
        {
            UserPreference pref = new UserPreference(UserId, key, value);

            preferences[key] = pref;
            pref.Save();
        }

        public bool ContainsKey(string key)
        {
            return preferences.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return preferences.Keys; }
        }

        public bool Remove(string key)
        {
            UserPreference pref;

            if (preferences.TryGetValue(key, out pref))
            {
                UserPreference.DeletePreferenceById(pref.PreferenceId);
            }

            return preferences.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            UserPreference pref;
            bool success = false;
            value = null;

            if (preferences.TryGetValue(key, out pref))
            {
                value = pref.Value;
                success = true;
            }

            return success;
        }

        public ICollection<string> Values
        {
            get { return preferences.Values.Select(userPref => userPref.Value).ToList(); }
        }

        public string this[string key]
        {
            get
            {
                string returnVal = null;

                TryGetValue(key, out returnVal);
                return returnVal;
            }
            set
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    UserPreference pref = preferences[key];

                    pref.Value = value;
                    pref.Save();
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            foreach (UserPreference pref in preferences.Values)
            {
                UserPreference.DeletePreferenceById(pref.PreferenceId);
            }

            preferences.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return preferences.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
