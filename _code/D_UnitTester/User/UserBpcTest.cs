using System;

using AICPA.Destroyer.Shared;
using NUnit.Framework;

namespace AICPA.Destroyer.User
{
	/// <summary>
	/// Summary description for UserBpcTest.
	/// </summary>
	[TestFixture]
	public class UserBpcTest : BaseTest
	{
        [Test]
		public void TestLicenseAgreementFlag()
        {
            Guid userId = Guid.NewGuid();
            string sessionId = Guid.NewGuid().ToString();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);
            
            try
            {
                User user = new User(userId, ReferringSite.Csc);

                Assert.AreEqual(LicenseAgreementStatus.InitialDefault, user.LicenseAgreementValue, "Expected default license agreement status value of 0 in DB and InitialDefault in enumeration in memory.");

                user.LogOn(sessionId, myDomain);

                Assert.AreEqual(LicenseAgreementStatus.InitialDefault, user.LicenseAgreementValue, "User log on shouldn't affect license agreement status value.");

                user.SetLicenseAgreementValue(LicenseAgreementStatus.Agreed);

                Assert.AreEqual(LicenseAgreementStatus.Agreed, user.LicenseAgreementValue, "Explicitly setting the license agreement status value to Agreed (for in memory enumeration; value of 1 in DB) should change it's value.");

                // Let's create one more user with same Guid to make sure license agreement status value is being retrieved from DB
                User sameUser = new User(userId, ReferringSite.Csc);

                Assert.AreEqual(LicenseAgreementStatus.Agreed, sameUser.LicenseAgreementValue, "When constructing a user object based on existing Guid, it should retrieve saved license agreement status value from DB.");
            }
	        finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }

        [Test]
        public void TestGettingAndSettingUserEmailAddress()
        {
            string email = "dummy@domain.com";
            Guid userId = Guid.NewGuid();
            string sessionId = Guid.NewGuid().ToString();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);

            try
            {
                User user = new User(userId, ReferringSite.Ceb, email);

                Assert.AreEqual(email, user.EmailAddress, "User object should have email populated with value passed in at construction.");

                user.LogOn(sessionId, myDomain);

                Assert.AreEqual(email, user.EmailAddress, "Logging on should not change the email address.");

                // Let's create one more user with same Guid to make sure email is being retrieved from DB
                User sameUser = new User(userId, ReferringSite.Ceb);

                Assert.AreEqual(email, sameUser.EmailAddress, "When constructing a user object based on existing Guid, it should retrieve saved email address value from DB.");
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }

        [Test]
        public void TestNoUserEmailAddress()
        {
            Guid userId = Guid.NewGuid();
            string sessionId = Guid.NewGuid().ToString();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);

            try
            {
                User user = new User(userId, ReferringSite.Csc);

                Assert.AreEqual(null, user.EmailAddress, "User object should have null email address populated when no value passed in at construction.");

                user.LogOn(sessionId, myDomain);

                Assert.AreEqual(null, user.EmailAddress, "Logging on should not change the email address from being null.");
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }
	}
	
	//[TestFixture]
	public class CreateC2bUser : UserBpcTest
	{
		
	}

}
