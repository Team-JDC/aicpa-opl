using System;
using System.Linq;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Shared;
using NUnit.Framework;

namespace AICPA.Destroyer.User.UserPreferences
{
    public class UserPreferencesBpcTest : ContentShared
    {
    }

    [TestFixture]
    public class UserPreferenceGeneral : UserPreferencesBpcTest
    {
        [Test]
        public void UserPreferenceGeneral_Save_and_Retrieve_Preferences()
        {
            string testKey = "MyTestKey";
            string testValue = "My Test Saved Value 1";
            UserPreferenceDalc dalc = new UserPreferenceDalc();
            Guid userId = Guid.NewGuid();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);
            string sessionId = Guid.NewGuid().ToString();
            var cscUser = new User(userId, ReferringSite.Csc);

            try
            {
                //user login
                cscUser.LogOn(sessionId, myDomain);

                // Create a dummy user preference and save it
                UserPreference userPreference = new UserPreference(userId, testKey, testValue);
                userPreference.Save();

                // Make sure PreferenceId comes back populated after Save call and check other properties on user preference object
                Assert.AreNotEqual(0, userPreference.PreferenceId, "Calling Save method on a UserPreference was expected to bring back a valid PreferenceId value from DB.");
                Assert.AreEqual(userId, userPreference.UserId, "Expected UserId to be set on UserPreference object.");
                Assert.AreEqual(testKey, userPreference.Key, "Expected Key to be set on UserPreference object.");
                Assert.AreEqual(testValue, userPreference.Value, "Expected Value to be set on UserPreference object.");

                // Change the value and see if it comes through correctly
                int savedId = userPreference.PreferenceId;
                string newTestValue = "My Test Saved Value 2";
                userPreference.Value = newTestValue;
                userPreference.Save();

                Assert.AreEqual(savedId, userPreference.PreferenceId, "Expected preferenceId in DB not to change on update.");
                Assert.AreEqual(newTestValue, userPreference.Value, "Expected new saved value.");

                // Now retrieve all saved preferences from the DB for the dummy user; assert that there is only 1 row and try out [] indexing syntax
                UserPreferencesDictionary retrievedPreferences = UserPreference.GetPreferencesForUser(userId);

                Assert.AreEqual(1, retrievedPreferences.Count, "Expected exactly one user preference to be retrieved for test user from DB.");
                Assert.AreEqual(newTestValue, retrievedPreferences[testKey], "Expected new saved value.");
                
                // Test Clear method
                retrievedPreferences.Clear();
                Assert.AreEqual(0, retrievedPreferences.Count, "Clear method should remove all preferences from the collection.");
                Assert.IsNull(dalc.GetPreference(userPreference.PreferenceId), "User Preference should have been deleted from the database and null returned.");

                // Test getting User Preferences collection from user object; it should be empty and TryGetValue should fail
                string tryValueString;
                retrievedPreferences = cscUser.Preferences;
                Assert.AreEqual(0, retrievedPreferences.Count, "Preferences collection should still be empty because of the previous call to Clear.");
                Assert.False(retrievedPreferences.TryGetValue(testKey, out tryValueString), "Collection is empty so TryGetValue should return false.");
                Assert.IsNull(tryValueString, "tryValueString should not have been set because attempt was unsuccessful in finding key/value pair in empty collection.");

                // Add key back in and make some asserts in regard to collection count and TryGetValue method
                retrievedPreferences.Add(testKey, newTestValue);
                Assert.AreEqual(1, retrievedPreferences.Count, "Expected exactly one user preference to be retrieved for test user from DB.");
                Assert.True(retrievedPreferences.TryGetValue(testKey, out tryValueString), "TryGetValue should have been successful.");
                Assert.AreEqual(newTestValue, tryValueString, "Expected new saved value.");

                // Test retrieving preference object for user/key combination
                UserPreference retrievedPref = new UserPreference(dalc.GetPreferenceForUser(userId, testKey));
                Assert.AreEqual(newTestValue, retrievedPref.Value, "Expected new saved value.");
                Assert.AreNotEqual(savedId, retrievedPref.PreferenceId, "Expected new PreferenceId to have been incremented (and different than previous one) when key/value were added back in again.");
                savedId = retrievedPref.PreferenceId;

                // Test Keys and Values collections
                Assert.AreEqual(1, retrievedPreferences.Keys.Count, "Expected Keys collection of exactly 1 key.");
                Assert.AreEqual(testKey, retrievedPreferences.Keys.Single(), "One and only key in Keys collection didn't match key persisted.");
                Assert.AreEqual(1, retrievedPreferences.Values.Count, "Expected Values collection of exactly 1 value.");
                Assert.AreEqual(newTestValue, retrievedPreferences.Values.Single(), "One and only value in Values collection didn't match value persisted.");

                // Test ContainsKey method
                Assert.True(retrievedPreferences.ContainsKey(testKey), "Preferences collection should have contained key.");
                Assert.False(retrievedPreferences.ContainsKey("NonsenseKeyValue"), "Preferences collection should not contain random string as a key value.");

                // Test Remove method
                retrievedPreferences.Remove(retrievedPref.Key);
                Assert.IsNull(dalc.GetPreference(retrievedPref.PreferenceId), "User Preference should have been deleted from the database and null returned.");
                Assert.AreEqual(0, retrievedPreferences.Count, "Preferences collection should still be empty because of the previous call to Remove.");

                // Test [] indexing syntax for update and insert scenarios
                retrievedPreferences[testKey] = testValue;
                Assert.AreEqual(1, retrievedPreferences.Count, "Expected exactly one user preference to be retrieved for test user from DB.");

                retrievedPref = new UserPreference(dalc.GetPreferenceForUser(userId, testKey));
                Assert.AreEqual(testValue, retrievedPref.Value, "Expected original saved value.");
                Assert.AreNotEqual(savedId, retrievedPref.PreferenceId, "Expected new PreferenceId to have been incremented (and different than previous one) when key/value were added back in again.");
                savedId = retrievedPref.PreferenceId;

                retrievedPreferences[testKey] = newTestValue;
                Assert.AreEqual(1, retrievedPreferences.Count, "Expected exactly one user preference to be retrieved for test user from DB.");
                Assert.AreEqual(newTestValue, retrievedPreferences[testKey], "Expected new saved value.");

                // Test UserPreferenceDalc Delete method (this doesn't update Preferences collection on User object; so don't use normally)
                dalc.DeleteUserPreference(savedId);
                Assert.IsNull(dalc.GetPreference(retrievedPref.PreferenceId), "User Preference should have been deleted from the database and null returned.");
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }
    }
}
