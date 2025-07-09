using System;
using System.Collections;
using NUnit.Framework;
using AICPA.Destroyer.User;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Subscription;
using System.Threading;

namespace AICPA.Destroyer.User
{
	#region UserSecurityBpcTest
	/// <summary>
	/// Summary description for UserSecurityBpcTest.
	/// </summary>
	public class UserSecurityBpcTest : BaseTest
	{
		/// <summary>
		///		A helper method to check if the passed users are logged on.
		/// </summary>
		/// <param name="users"></param>
		public static void CheckUsersLoggedOn(params User[] users)
		{
			foreach(User user in users)
			{
				CheckUserLoggedOn(user);
			}
		}
		/// <summary>
		///		A helper method to check if the passed users are logged off.
		/// </summary>
		/// <param name="users"></param>
		public static void CheckUsersLoggedOff(params User[] users)
		{
			foreach(User user in users)
			{
				CheckUserLoggedOff(user);
			}
		}
		/// <summary>
		///		A helper method to check if the passed user is logged on.
		/// </summary>
		/// <param name="user"></param>
		public static void CheckUserLoggedOn(User user)
		{
			if(!IsUserLoggedOn(user))
			{
				throw new SecurityException("User was not logged on as expected.");	
			}
		}
		/// <summary>
		///		A helper method to check if the passed user is logged off.
		/// </summary>
		/// <param name="user"></param>
		public static void CheckUserLoggedOff(User user)
		{
			if(IsUserLoggedOn(user))
			{
				throw new SecurityException("User was not logged off as expected.");	
			}
		}
		/// <summary>
		///		Determines if a user is logged in or not.
		/// </summary>
		/// <param name="user">A user object.</param>
		/// <returns>A true or false value </returns>
		public static bool IsUserLoggedOn(User user)
		{
			bool loggedOn = false;
			try
			{
				if(0<(int)(SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT COUNT(*) FROM D_User WHERE CurrentSessionId = '{0}' AND UserId='{1}'", user.UserSecurity.SessionId, user.UserId.ToString()))))
				{
					loggedOn = true;
					foreach(Firm.Firm firm in user.UserSecurity.FirmCollection)
					{				
						if(0==(int)(SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT COUNT(*) FROM D_CurrentFirmUsers WHERE Aca = '{0}' AND Code='{1}' AND UserId='{2}'", firm.Aca, firm.Code, user.UserId.ToString()))))
						{
							throw new SecurityException("User is logged on, but firm subscriptions were not properly saved to the database.");
						}
					}
				}
			}
			catch(SecurityException e)
			{
				if (e.Message == User.ERROR_USERNOTLOGGEDIN)
				{
					loggedOn = false;
					if(0!=(int)(SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT COUNT(*) FROM D_CurrentFirmUsers WHERE UserId='{0}'", user.UserId.ToString()))))
					{
						throw new SecurityException("User is logged off, but firm subscriptions were not properly deleted from the database.");
					}
				}
				else
				{
					throw;
				}
			}
			return loggedOn;
		}
	}
	#endregion UserSecurityBpcTest

	#region UserSecurityValidation
	[TestFixture]
	public class UserSecurityValidation : UserSecurityBpcTest
	{
		/// <summary>
		///		Make sure that a Csc user can log onto the system if they have
		///		c2b information.
		/// </summary>
		[Test]
		public void CscUserWithC2bUserId()
		{
			Guid userId = Guid.NewGuid();
			string myDomain = "test;test1;test2;";
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, myDomain, "Test-KnowlysisThing", "2965415", 145);
			string sessionId = Guid.NewGuid().ToString();			
			User cscUser = new User(userId.ToString(), userId, ReferringSite.Csc);
			try
			{
				cscUser.LogOn(sessionId);
				Assert.IsTrue(myDomain == cscUser.UserSecurity.Domain, "The user did not have the expected domain.");
				Assert.IsTrue(sessionId == cscUser.UserSecurity.SessionId, "The user did not have the expected sessionID");
				UserSecurityValidation.CheckDBRecordExists("D_User", string.Format("UserId='{0}' and CurrentSessionId='{1}'", userId, sessionId));
			}
			finally
			{
				UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}

		/// <summary>
		///		Make sure that a Csc user can log onto the system if they don't have
		///		c2b information.
		/// </summary>
		[Test]
		public void CscUserWithoutC2bUserId()
		{
			string myDomain = "test;test1;test2;";
			string sessionId = Guid.NewGuid().ToString();
			Guid userId = Guid.NewGuid();
			User cscUser = new User(userId, ReferringSite.Csc);
			try
			{
				cscUser.LogOn(sessionId, myDomain);
				Assert.IsTrue(myDomain == cscUser.UserSecurity.Domain, "The user did not have the expected domain.");
				Assert.IsTrue(sessionId == cscUser.UserSecurity.SessionId, "The user did not have the expected sessionID");
				UserSecurityValidation.CheckDBRecordExists("D_User", string.Format("UserId='{0}' and CurrentSessionId='{1}'", userId, sessionId));
			}
			finally
			{
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}

		/// <summary>
		///		Make sure that an Exams user can log onto the system 
		/// </summary>
		[Test]
		public void ExamsUser()
		{
			string myDomain = "test;test1;test2;";
			string sessionId = Guid.NewGuid().ToString();
			Guid userId = Guid.NewGuid();
			User cscUser = new User(userId, ReferringSite.Exams);
			try
			{
				cscUser.LogOn(sessionId, myDomain);
				Assert.IsTrue(myDomain == cscUser.UserSecurity.Domain, "The user did not have the expected domain.");
				Assert.IsTrue(sessionId == cscUser.UserSecurity.SessionId, "The user did not have the expected sessionID");
				UserSecurityValidation.CheckDBRecordExists("D_User", string.Format("UserId='{0}' and CurrentSessionId='{1}'", userId, sessionId));
			}
			finally
			{
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}

		/// <summary>
		///		The user should have the expected domain string.
		/// </summary>
		[Test]
		public void UserDomainString()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				Assert.IsTrue(this.BasicC2bUser.UserSecurity.Domain == UserBpcTest.BASIC_C2B_USER_DOMAIN);
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		///		The user should have the expected firm information
		/// </summary>
		[Test]
		public void UserFirm()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				Assert.IsTrue(this.BasicC2bUser.UserSecurity.FirmCollection[0].Aca == UserBpcTest.BASIC_C2B_USER_ACA, "Unexpected Aca value in the User.UserSecurity.FirmCollection.Firm object");
				Assert.IsTrue(this.BasicC2bUser.UserSecurity.FirmCollection[0].Code == UserBpcTest.BASIC_C2B_USER_CODE, "Unexpected Code value in the User.UserSecurity.FirmCollection.Firm object");
				BaseTest.CheckDBRecordExists("D_EventLog", string.Format("Name='Firm Logon' AND Description='ACA={0};Code={1};' AND UserId='{2}'", UserBpcTest.BASIC_C2B_USER_ACA, UserBpcTest.BASIC_C2B_USER_CODE, this.BasicC2bUser.UserId.ToString()));	
				BaseTest.CheckDBRecordExists("D_EventLog", string.Format("Name='User Logon' AND UserId='{0}'", this.BasicC2bUser.UserId.ToString()));	
				this.BasicC2bUser.LogOff();				
				BaseTest.CheckDBRecordExists("D_EventLog", string.Format("Name='User Logoff' AND UserId='{0}'", this.BasicC2bUser.UserId.ToString()));	
			}
			finally
			{
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		///		The user object and the reference to the user object on the off the UserSecurity object
		///		should be the exact same object.  This will test that they are.
		/// </summary>
		[Test]
		public void UserSecurityUser()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				Assert.AreSame(this.BasicC2bUser.UserSecurity.User, this.BasicC2bUser, "User objects are not the same");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

        [Test]
        public void WebServiceTest()
        {
            //localhost.Service2 service2 = new AICPA.Destroyer.localhost.Service2();
            //service2.Url = "http://localhost.:19681/Service2.asmx";
            //string h = service2.HelloWorld();

            cpa2biz.AuthService.com.SubscriptionService c2bService = new cpa2biz.AuthService.com.SubscriptionService();

            c2bService.Url = System.Configuration.ConfigurationSettings.AppSettings[UserSecurity.CONFIG_WEB_REFERENCE_URL];
            string c2bSubscriptionXml = c2bService.GetOnlinePlatformAuthInfo("123abc456");

            Assert.IsNotNullOrEmpty(c2bSubscriptionXml);
        }

		/// <summary>
		///		If a user logs on twice, the original user will be able to access their information
		///		until the database is polled again.  The database should not be polled under this test
		///		case.  If it is, the test will fail.
		/// </summary>
		[Test]
		public void UserLoginTwiceWithinSessionPollTime()
		{
			Guid userId = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
			
			AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			// same userId, just a different user object
			AICPA.Destroyer.User.User user1 = new User(userId.ToString(), userId, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				// should be available to log on since poll time has not expired (unless there was a delay for some unexpected reason such as DB down)...
				string testDomain = user.UserSecurity.Domain;
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
					UserBpcTest.DeleteDestroyerUsers(userId);
				}
			}
		}

		/// <summary>
		///		Multiple users should be able to log on as long as the firm count is not exceeded.  This
		///		test case will confirm that multiple users are able to log on under a firm with 6 firm count.
		/// </summary>
		[Test]
		public void MultipleUsers_FirmCountNotExceeded()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);
			C2bUser c2bUser1 =UserBpcTest.CreateC2bWebServiceUser(userId1, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);
			C2bUser c2bUser2 =UserBpcTest.CreateC2bWebServiceUser(userId2, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);

			AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
			AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
					user2.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2);
				}
			}
		}
	}
	#endregion UserSecurityValidation

	#region UserSecurityExceptions
	[TestFixture]
	public class UserSecurityExceptions : UserSecurityBpcTest
	{
		/// <summary>
		///		If the firm count is exceeded then the user exceeding the firm count should not
		///		be able to log on. This test will check that the user exceedingt the count will not
		///		be able to log on.
		/// </summary>
		[Test]
		public void EXCEPTION_FirmCountExceeded()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
			C2bUser c2bUser1 =UserBpcTest.CreateC2bWebServiceUser(userId1, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
			C2bUser c2bUser2 =UserBpcTest.CreateC2bWebServiceUser(userId2, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);

            AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
            AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
            AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
				Assert.Fail("LogOn succeeded, but should have failed due to firm count being exceeded");
			}
			catch(SecurityException e)
			{
				Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED) > -1, "LogOn should have failed due to firm count being exceeded");
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2);
				}
			}
		}

		/// <summary>
		///		A user sessionId will become invalid if the same user logs in under a different
		///		session.  The original session will become invalid only if the user tries to
		///		do something after a session poll time has passed.  This test case logs a user
		///		in twice, waits the session poll time, then tries to do something with the original 
		///		logon.  An exception is expected.
		/// </summary>
		[Test]
		public void EXCEPTION_InvalidSessionId_HasBuiltInDelay()
		{
			Guid userId = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);

            AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			// same userId, just a different user object
            AICPA.Destroyer.User.User user1 = new User(userId.ToString(), userId, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				int waitTime = Convert.ToInt16(System.Configuration.ConfigurationSettings.AppSettings[UserSecurity.CONFIG_SESSION_POLL_INTERVAL]);
				Thread.Sleep((waitTime * 1000) + 1000);
				
				// should not be available now that poll time expired....
				string testDomain = user.UserSecurity.Domain;
				Assert.Fail("UserSecurity call succeeded, but should have failed due to invalid sessionId");
			}
			catch(UserNotAuthenticated e)
			{
				Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_INVALID_SESSIONID) > -1, "LogOn should have failed due to the session being expired.");
				Assert.IsTrue(!user.UserSecurity.Authenticated, "User Authentication should have failed since the session was expired.");
				Assert.AreSame(user.UserSecurity.AuthenticationError, UserSecurity.AUTHENTICATION_ERROR_INVALID_SESSIONID, "Unexpected authentication error.  Expected: " + UserSecurity.AUTHENTICATION_ERROR_INVALID_SESSIONID + ", but got: " + user.UserSecurity.AuthenticationError);
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
					UserBpcTest.DeleteDestroyerUsers(userId);
				}
			}
		}
		
		/// <summary>
		///		A user must log off before they can logon again.  This test case will make sure that 
		///		the API will not let a user log on twice without logging off.
		/// </summary>
		[Test]
		public void EXCEPTION_LogOnTwiceNoLogoff()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				try
				{

					this.BasicC2bUser.LogOn(Guid.NewGuid().ToString());
				}
				catch(SecurityException e)
				{
					BaseTest.CheckExceptionMessage(User.ERROR_USERALREADYLOGGEDON, e.Message);
				}
				
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		///		When logging on, a valid sessionId must be provided (sessionid cannot be null).
		///		This test will make sure that an exception is thrown when a null sessionid is passed.
		/// </summary>
		[Test]
		public void EXCEPTION_LogOnSessionIdNull()
		{
			try
			{
				string newSession = null;				
				try
				{
					this.BasicC2bUser.LogOn(newSession);
				}
				catch(ArgumentNullException e)
				{
					BaseTest.CheckExceptionMessage(User.ERROR_INVALIDSESSIONID, e.Message);
				}				
			}
			finally
			{				
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		///		If User.LogOff is called without ever having called User.LogOn, an exception should
		///		be thrown.  This test case confirms that the exception is thrown as expected.
		/// </summary>
		[Test]
		public void EXCEPTION_LogoffNoLogon()
		{
			try
			{
				this.BasicC2bUser.LogOff();				
			}
			catch(SecurityException e)
			{	
				BaseTest.CheckExceptionMessage(User.ERROR_LOGOFFWITHOULOGON, e.Message);
			}
			finally
			{
				this.DeleteBasicC2bUser();
			}
		}


		/// <summary>
		///		A user coming from the C2B site should logon without passing a domain string.  This test 
		///		will confirm that an exception is thrown when a this scenario happens.
		/// </summary>
		[Test]
		public void EXCEPTION_C2bUserWrongLogon()
		{
			try
			{
				this.BasicC2bUser.LogOn(Guid.NewGuid().ToString(), "bob;singer;telephone;");				
			}
			catch(SecurityException e)
			{	
				BaseTest.CheckExceptionMessage(User.ERROR_DONOTPASSDOMAINSTRING, e.Message);
			}
			finally
			{
				this.DeleteBasicC2bUser();
			}
		}
		
		/// <summary>
		///		If the c2b web service fails, then the user authentication should fail as well
		///		and encase the error. This test case validates that the web service failure
		///		will produce the expected results.
		/// </summary>
		[Test]
		public void EXCEPTION_C2bWebServiceFailure()
		{
			// This userId will produce a failure in the c2b web service....
			Guid userId = new Guid("77EBEF60-5767-4E4E-A669-318C7741C08F");
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
            AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				Assert.Fail("UserSecurity call succeeded, but should have failed due to web service failure");
			}
			catch(SecurityException e)
			{
                // sburton 2010-01-21: Changed to check for some specific messages, rather than exact string match.
                //                     The new version (of .net and/or web service) gives a slightly differen message.
                //Assert.IsTrue(e.Message == "The user failed to authenticate.  Reason for authentication failure: Server was unable to process request. --> Internal Web Server Error", "Unexpected error message");

                Assert.IsTrue(e.Message.Contains("The user failed to authenticate"), "Unexpected error message");
                Assert.IsTrue(e.Message.Contains("Server was unable to process request."), "Unexpected error message");
                Assert.IsTrue(e.Message.Contains("Internal Web Server Error"), "Unexpected error message");


                UserSecurityBpcTest.CheckDBRecordExists("D_EventLog", string.Format("[Module]='{1}' AND Method='{2}' AND Name='{3}'", user.UserId, UserSecurity.MODULE_USERSECURITY, "SetC2bSubscriptionContext", "C2B Web Service Failure"));			
			}
			finally
			{
				UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}
	}
	#endregion UserSecurityExceptions

	#region DatabaseCheckTests
	[TestFixture]
	public class DatabaseCheckTests : UserSecurityBpcTest
	{
		/// <summary>
		///		A time lapse will require the program to poll the database 
		///		for the session.  the LastSessionPoll time should be updated 
		///		due to the time Lapse
		/// </summary>
		[Test]
		public void LastSessionPollTimeIsUpdated__HasBuiltInDelay()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				DateTime firstSessionPollTime = (DateTime)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT LastSessionPoll FROM D_User WHERE UserId = '{0}' AND CurrentSessionID = '{1}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.SessionId));
				int waitTime = Convert.ToInt16(System.Configuration.ConfigurationSettings.AppSettings[UserSecurity.CONFIG_SESSION_POLL_INTERVAL]);
				Thread.Sleep((waitTime * 1000) + 1000);	// wait the poll time lapse period (from config file) plus one second.
				string myFavoriteDomain = this.BasicC2bUser.UserSecurity.Domain; // Call something that should poll the sessionId
				DateTime secondSessionPollTime = (DateTime)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT LastSessionPoll FROM D_User WHERE UserId = '{0}' AND CurrentSessionID = '{1}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.SessionId));
				Assert.IsTrue(firstSessionPollTime != secondSessionPollTime, "The LastSessionPoll field in the database was not updated as expected.");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		A time lapse will require the program to poll the database 
		///		for the session.  If the TimeLapse is not exceeded, then there should be 
		///		no update to the datbase.  This test case may fail if the code is stepped through
		///		since that would create a time lapse as well, thus invalidating the test.
		/// </summary>
		[Test]
		public void LastSessionPollTimeIsNotUpdated()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				DateTime firstSessionPollTime = (DateTime)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT LastSessionPoll FROM D_User WHERE UserId = '{0}' AND CurrentSessionID = '{1}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.SessionId));
				string myFavoriteDomain = this.BasicC2bUser.UserSecurity.Domain; // Call something that should poll the sessionId
				DateTime secondSessionPollTime = (DateTime)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT LastSessionPoll FROM D_User WHERE UserId = '{0}' AND CurrentSessionID = '{1}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.SessionId));
				Assert.IsTrue(firstSessionPollTime == secondSessionPollTime, "The LastSessionPoll field in the database was not updated as expected.");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		A simple User LogOn should insert values into the D_User and D_CurrentFirmUsers table.
		///		This test confirms that this is the case.
		/// </summary>
		[Test]
		public void UserAndUserFirmInDB()
		{
			try
			{
				string newSession = Guid.NewGuid().ToString();
				this.BasicC2bUser.LogOn(newSession);
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_User WHERE UserId = '{0}' AND CurrentSessionID = '{1}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.SessionId)), "An unexpected rowcount was found in D_CurrentFirmUsers for user " + this.BasicC2bUser.UserId.ToString() + ".  Expected exactly 1 (one) row.");
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE UserId = '{0}' AND Aca='{1}' AND Code='{2}'", this.BasicC2bUser.UserId, this.BasicC2bUser.UserSecurity.FirmCollection[0].Aca, this.BasicC2bUser.UserSecurity.FirmCollection[0].Code)), "An unexpected rowcount was found in D_CurrentFirmUsers for user " + this.BasicC2bUser.UserId.ToString() + ".  Expected exactly 1 (one) row.");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		///		If multiple users from the same firm log on, then there should exist a record in d_currentFirmUsers
		///		for each user
		/// </summary>
		[Test]
		public void MultipleUsers_SingleFirmPersisted()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);
			C2bUser c2bUser1 =UserBpcTest.CreateC2bWebServiceUser(userId1, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);
			C2bUser c2bUser2 =UserBpcTest.CreateC2bWebServiceUser(userId2, "test;aag-air;proflit;", "Test-Knowlysis", "555", 6);

			AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
			AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);

			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
				Assert.IsTrue(3==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}'", "Test-Knowlysis", "555")), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 3 (three) rows.");
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
					user2.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2);
				}
			}
		}
		/// <summary>
		///		If a single user from the multiple firms log on, then there should exist a record in d_currentFirmUsers
		///		for each firm
		/// </summary>
		[Test]
		public void SingleUser_MultipleFirmsPersisted()
		{
			Guid userId = Guid.NewGuid();

			C2bUser c2bUser = new C2bUser();
			c2bUser.User.AddUserRow(userId, "test;aag-air;proflit;");
			c2bUser.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser.Firm.AddFirmRow("Test-Knowlysis", "777", 2);
			c2bUser.Firm.AddFirmRow("Test-AICPA", "888", 2);

			UserBpcTest.CreateC2bWebServiceUsers(c2bUser);

			AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}' AND UserId='{2}'", "Test-Knowlysis", "555", user.UserId)), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 1 (one) row.");
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}' AND UserId='{2}'", "Test-Knowlysis", "777", user.UserId)), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 1 (one) row.");
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}' AND UserId='{2}'", "Test-AICPA", "888", user.UserId)), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 1 (one) row.");
			}
			finally
			{
				try
				{
					user.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
					UserBpcTest.DeleteDestroyerUsers(userId);
				}
			}
		}

		/// <summary>
		///		If a user logs on, the allowed firm users count for all that user's firms becomes the
		///		maximum allowed users out of all that user's firms.
		/// </summary>
		[Test]
		public void MutipleUsers_MultipleFirmsExceedingFirmCount()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			Guid userId3 = Guid.NewGuid();
			Guid userId4 = Guid.NewGuid();
			Guid userId5 = Guid.NewGuid();

			// can log on
			C2bUser c2bUser = new C2bUser();
			c2bUser.User.AddUserRow(userId, "test;aag-air;proflit;");
			c2bUser.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser);

			// can log on
			C2bUser c2bUser1 = new C2bUser();
			c2bUser1.User.AddUserRow(userId1, "test;aag-air;proflit;");
			c2bUser1.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser1.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser1);

			// can log on
			C2bUser c2bUser2 = new C2bUser();
			c2bUser2.User.AddUserRow(userId2, "test;aag-air;proflit;");
			c2bUser2.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser2.Firm.AddFirmRow("Test-Knowlysis", "777", 1);
			c2bUser2.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser2);

			// can log on
			C2bUser c2bUser3 = new C2bUser();
			c2bUser3.User.AddUserRow(userId3, "test;aag-air;proflit;");
			c2bUser3.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser3.Firm.AddFirmRow("Test-Knowlysis", "777", 1);
			c2bUser3.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			c2bUser3.Firm.AddFirmRow("Test-MegaFirm", "222", 15);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser3);

			// cannot log on
			C2bUser c2bUser4 = new C2bUser();
			c2bUser4.User.AddUserRow(userId4, "test;aag-air;proflit;");
			c2bUser4.Firm.AddFirmRow("Test-Knowlysis", "555", 2);
			c2bUser4.Firm.AddFirmRow("Test-Knowlysis", "777", 1);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser4);

			// cannot log on
			C2bUser c2bUser5 = new C2bUser();
			c2bUser5.User.AddUserRow(userId5, "test;aag-air;proflit;");
			c2bUser5.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser5);

			AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
            AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
            AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);
            AICPA.Destroyer.User.User user3 = new User(userId3.ToString(), userId3, ReferringSite.C2b);
            AICPA.Destroyer.User.User user4 = new User(userId4.ToString(), userId4, ReferringSite.C2b);
            AICPA.Destroyer.User.User user5 = new User(userId5.ToString(), userId5, ReferringSite.C2b);
			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
				user3.LogOn(Guid.NewGuid().ToString());
				try
				{
					user4.LogOn(Guid.NewGuid().ToString());
				}
				catch(SecurityException e)
				{
					Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED) > -1, "LogOn should have failed due to firm count being exceeded");					
				}
				try
				{
					user5.LogOn(Guid.NewGuid().ToString());
				}
				catch(SecurityException e)
				{
					Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED) > -1, "LogOn should have failed due to firm count being exceeded");
				}
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
					user2.LogOff();
					user3.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2, c2bUser3, c2bUser4, c2bUser5);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2, userId3, userId4, userId5);
				}
			}
		}
		/// <summary>
		///		If a user firm limit is exceeded, then an additional user cannot log on.  If one of the original
		///		user's session times out, then an additional user should be able to now log on.  The original user
		///		whose session timed out should now be invalid.
		/// </summary>
		[Test]
		public void FirmCountExceededThenDecremented_LargeBuiltInDelay()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
			C2bUser c2bUser1 =UserBpcTest.CreateC2bWebServiceUser(userId1, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);
			C2bUser c2bUser2 =UserBpcTest.CreateC2bWebServiceUser(userId2, "test;aag-air;proflit;", "Test-Knowlysis", "555", 2);

            AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
            AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
            AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);

			try
			{
				int SessionTimeoutPeriod = Convert.ToInt16(System.Configuration.ConfigurationSettings.AppSettings[UserSecurity.CONFIG_SESSION_TIMEOUT_VALUE]);
				int waitTime = ((int)(System.Math.Round((decimal)SessionTimeoutPeriod/2)))*1000;
				user.LogOn(Guid.NewGuid().ToString());	// session that will eventually timeout.			
				Thread.Sleep(waitTime);						// wait for half the timeout period...
				user1.LogOn(Guid.NewGuid().ToString());		// logon another user to exceed the firm count.
				try
				{
					user2.LogOn(Guid.NewGuid().ToString());		// user logon should fail because "user" user has not timed out.
					Assert.Fail("User logon should have failed.");
				}
				catch(SecurityException e)
				{
					Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED) > -1, "Unexpected error message.  Expected: " + UserSecurity.AUTHENTICATION_ERROR_FIRM_SUBSCRIPTION_EXCEEDED);
					Thread.Sleep(waitTime + 2000);				// wait the other half of the timeout period plus 2 seconds.
				}
				// original user should now be expired.
				user2.LogOn(Guid.NewGuid().ToString());			// now the user2 can log on with a new sessionId because original user it timed out.
				// two users will now be in the D_CurrentFirmUsers table.
				Assert.IsTrue(2==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}'", "Test-Knowlysis", "555")), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 2 (two) rows.");
				// one of the users should be user2
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}' AND UserId='{2}'", "Test-Knowlysis", "555", user2.UserId)), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 1 (one) row.");
				Assert.IsTrue(1==(int)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, string.Format("SELECT Count(*) FROM D_CurrentFirmUsers WHERE Aca='{0}' AND Code='{1}' AND UserId='{2}'", "Test-Knowlysis", "555", user1.UserId)), "An unexpected rowcount was found in D_CurrentFirmUsers for the firm. Expected exactly 1 (one) row.");
				try
				{
					// the original user should now have an invalid session.
					string domain = user.UserSecurity.Domain;
					Assert.Fail("User should no longer have access to domain since session is no longer valid.");
				}
				catch(UserNotAuthenticated e)
				{
					Assert.IsTrue(e.Message.IndexOf(UserSecurity.AUTHENTICATION_ERROR_INVALID_SESSIONID) > -1, "Unexpected error message.  Expected: " + UserSecurity.AUTHENTICATION_ERROR_INVALID_SESSIONID);					
				}
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
					user2.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2);
				}
			}
		}
		/// <summary>
		///		If a user logs on, the allowed firm users count for all that user's firms becomes the
		///		maximum allowed users out of all that user's firms.
		/// </summary>
		[Test]
		public void MutipleUsers_DifferentFirms()
		{
			Guid userId = Guid.NewGuid();
			Guid userId1 = Guid.NewGuid();
			Guid userId2 = Guid.NewGuid();
			Guid userId3 = Guid.NewGuid();
			Guid userId4 = Guid.NewGuid();
			Guid userId5 = Guid.NewGuid();

			// can log on
			C2bUser c2bUser = new C2bUser();
			c2bUser.User.AddUserRow(userId, "test;aag-air;proflit;");
			c2bUser.Firm.AddFirmRow("Test-Knowlysis", "756", 2);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser);

			// can log on
			C2bUser c2bUser1 = new C2bUser();
			c2bUser1.User.AddUserRow(userId1, "test;aag-air;proflit;");
			c2bUser1.Firm.AddFirmRow("Test-ACIPA", "474", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser1);

			// can log on
			C2bUser c2bUser2 = new C2bUser();
			c2bUser2.User.AddUserRow(userId2, "test;aag-air;proflit;");
			c2bUser2.Firm.AddFirmRow("Test-KnowlysisSouthCampus", "221", 2);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser2);

			// can log on
			C2bUser c2bUser3 = new C2bUser();
			c2bUser3.User.AddUserRow(userId3, "test;aag-air;proflit;");
			c2bUser3.Firm.AddFirmRow("Test-KnowlysisNorthCampus", "546", 2);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser3);

			// cannot log on
			C2bUser c2bUser4 = new C2bUser();
			c2bUser4.User.AddUserRow(userId4, "test;aag-air;proflit;");
			c2bUser4.Firm.AddFirmRow("Test-KnowlysisChicago", "365", 2);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser4);

			// cannot log on
			C2bUser c2bUser5 = new C2bUser();
			c2bUser5.User.AddUserRow(userId5, "test;aag-air;proflit;");
			c2bUser5.Firm.AddFirmRow("Test-ACIPAJersey", "154", 3);
			UserBpcTest.CreateC2bWebServiceUsers(c2bUser5);

            AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
            AICPA.Destroyer.User.User user1 = new User(userId1.ToString(), userId1, ReferringSite.C2b);
            AICPA.Destroyer.User.User user2 = new User(userId2.ToString(), userId2, ReferringSite.C2b);
            AICPA.Destroyer.User.User user3 = new User(userId3.ToString(), userId3, ReferringSite.C2b);
            AICPA.Destroyer.User.User user4 = new User(userId4.ToString(), userId4, ReferringSite.C2b);
            AICPA.Destroyer.User.User user5 = new User(userId5.ToString(), userId5, ReferringSite.C2b);
			try
			{
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
				user3.LogOn(Guid.NewGuid().ToString());
				user4.LogOn(Guid.NewGuid().ToString());
				user5.LogOn(Guid.NewGuid().ToString());
				UserSecurityBpcTest.CheckUsersLoggedOn(user, user1, user2, user3, user4, user5);
				user.LogOff();
				user1.LogOff();
				UserSecurityBpcTest.CheckUsersLoggedOff(user, user1);
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				UserSecurityBpcTest.CheckUsersLoggedOn(user, user1);
				user4.LogOff();
				user5.LogOff();
				UserSecurityBpcTest.CheckUsersLoggedOff(user4, user5);
				user4.LogOn(Guid.NewGuid().ToString());
				user5.LogOn(Guid.NewGuid().ToString());
				UserSecurityBpcTest.CheckUsersLoggedOn(user4, user5);
				user2.LogOff();
				user3.LogOff();
				UserSecurityBpcTest.CheckUsersLoggedOff(user2, user3);
				user2.LogOn(Guid.NewGuid().ToString());
				user3.LogOn(Guid.NewGuid().ToString());
				UserSecurityBpcTest.CheckUsersLoggedOn(user, user1, user2, user3, user4, user5);
				user.LogOff();
				user1.LogOff();
				user2.LogOff();
				user3.LogOff();
				user4.LogOff();
				user5.LogOff();
				UserSecurityBpcTest.CheckUsersLoggedOff(user, user1, user2, user3, user4, user5);
				user.LogOn(Guid.NewGuid().ToString());
				user1.LogOn(Guid.NewGuid().ToString());
				user2.LogOn(Guid.NewGuid().ToString());
				user3.LogOn(Guid.NewGuid().ToString());
				user4.LogOn(Guid.NewGuid().ToString());
				user5.LogOn(Guid.NewGuid().ToString());
				UserSecurityBpcTest.CheckUsersLoggedOn(user, user1, user2, user3, user4, user5);
			}
			finally
			{
				try
				{
					user.LogOff();
					user1.LogOff();
					user2.LogOff();
					user3.LogOff();
					user4.LogOff();
					user5.LogOff();
				}
				finally
				{
					UserBpcTest.DeleteC2bWebServiceUsers(c2bUser, c2bUser1, c2bUser2, c2bUser3, c2bUser4, c2bUser5);
					UserBpcTest.DeleteDestroyerUsers(userId, userId1, userId2, userId3, userId4, userId5);
				}
			}
		}
		
	}
	#endregion DatabaseCheckTests

	#region BookSecurityTests
	[TestFixture]
	public class BookSecurityTests : UserSecurityBpcTest
	{
		/// <summary>
		///		A simple test that checks that a c2b user can see the expected books on their security context.
		/// </summary>
		[Test]
		public void C2bUserBooksTest()
		{
			
			string testSubscriptionCode = "test-Subscription";
			string testSubscriptionCode1 = "test-Subscription1";
			string testSubscriptionCode2 = "test-Subscription2";
			string testBook = Guid.NewGuid().ToString();
			string testBook1 = Guid.NewGuid().ToString();
			string testBook2 = Guid.NewGuid().ToString();
			string testBook3 = Guid.NewGuid().ToString();
			string testBook4 = Guid.NewGuid().ToString();
			string testBook5 = Guid.NewGuid().ToString();
			Book book = new Book(testBook, "Test-Book", "Test Book", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book1 = new Book(testBook1, "Test-Book1", "Test Book1", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book2 = new Book(testBook2, "Test-Book2", "Test Book2", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book3 = new Book(testBook3, "Test-Book3", "Test Book3", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book4 = new Book(testBook4, "Test-Book4", "Test Book4", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book5 = new Book(testBook5, "Test-Book5", "Test Book5", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			book.Save();
			book1.Save();
			book2.Save();
			book3.Save();
			book4.Save();
			book5.Save();
			ArrayList expectedBookNames = new ArrayList();
			ArrayList unexpectedBookNames = new ArrayList();
			// books the user will have access to
			expectedBookNames.Add(book.Name);
			expectedBookNames.Add(book1.Name);
			expectedBookNames.Add(book2.Name);
			expectedBookNames.Add(book3.Name);
			// books user will not have access to
			unexpectedBookNames.Add(book4.Name);
			unexpectedBookNames.Add(book5.Name);
			Guid userId = Guid.NewGuid();
			// build a domain string to give the user access to two subscriptions (books 0, 1, 2, 3)
			string userSubscriptionCode = testSubscriptionCode + "~" + testSubscriptionCode1 + "~";
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, userSubscriptionCode, "Test-KnowlysisThing", "2965415", 145);
				
			try
			{
				// insert the subscription information.
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode1));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode2));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode, book.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode, book1.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode1, book2.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode1, book3.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode2, book4.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode2, book5.Id));
							
				AICPA.Destroyer.User.User user = new User(userId.ToString(), userId, ReferringSite.C2b);
			
				string newSession = Guid.NewGuid().ToString();
				user.LogOn(newSession);

				// put book names into an arraylist
				ArrayList UserBookNames = new ArrayList();
				foreach(string bookName in user.UserSecurity.BookName)
				{
					UserBookNames.Add(bookName);	
				}

				// check the user's books for the expected books.
				foreach(string expectedBookname in expectedBookNames)
				{
					if(!UserBookNames.Contains(expectedBookname))
					{
						throw new SecurityException(string.Format("Expected the user to have book name {0}, but did not.", expectedBookname));
					}
					
				}

				// check the user's books for the unexpected books.
				foreach(string unexpectedBookname in unexpectedBookNames)
				{
					if(UserBookNames.Contains(unexpectedBookname))
					{
						throw new SecurityException(string.Format("Did not expect the user to have book name {0}, but did.", unexpectedBookname));
					}
				}
				
			}
			finally
			{
				// clean up the test data.
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode));
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode1));
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode2));

                ContentShared.DeleteNamedBook(testBook);
				ContentShared.DeleteNamedBook(testBook1);
				ContentShared.DeleteNamedBook(testBook2);
				ContentShared.DeleteNamedBook(testBook3);
				ContentShared.DeleteNamedBook(testBook4);
				ContentShared.DeleteNamedBook(testBook5);

				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode1));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode2));
			
				UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}

		/// <summary>
		///		A simple test that checks that an exams user can see the expected books on their security context.
		/// </summary>
		[Test]
		public void ExamUserBooksTest()
		{
			
			string testSubscriptionCode = "test-Subscription";
			string testSubscriptionCode1 = "test-Subscription1";
			string testSubscriptionCode2 = "test-Subscription2";
			string testBook = Guid.NewGuid().ToString();
			string testBook1 = Guid.NewGuid().ToString();
			string testBook2 = Guid.NewGuid().ToString();
			string testBook3 = Guid.NewGuid().ToString();
			string testBook4 = Guid.NewGuid().ToString();
			string testBook5 = Guid.NewGuid().ToString();
			Book book = new Book(testBook, "Test-Book", "Test Book", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book1 = new Book(testBook1, "Test-Book1", "Test Book1", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book2 = new Book(testBook2, "Test-Book2", "Test Book2", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book3 = new Book(testBook3, "Test-Book3", "Test Book3", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book4 = new Book(testBook4, "Test-Book4", "Test Book4", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			Book book5 = new Book(testBook5, "Test-Book5", "Test Book5", "no copyright", BookSourceType.Makefile, "www.testbook.com");
			book.Save();
			book1.Save();
			book2.Save();
			book3.Save();
			book4.Save();
			book5.Save();
			ArrayList expectedBookNames = new ArrayList();
			ArrayList unexpectedBookNames = new ArrayList();
			// books the user will have access to
			expectedBookNames.Add(book.Name);
			expectedBookNames.Add(book1.Name);
			expectedBookNames.Add(book2.Name);
			expectedBookNames.Add(book3.Name);
			// books user will not have access to
			unexpectedBookNames.Add(book4.Name);
			unexpectedBookNames.Add(book5.Name);
			Guid userId = Guid.NewGuid();
			// build a domain string to give the user access to two subscriptions (books 0, 1, 2, 3)
			string userSubscriptionCode = testSubscriptionCode + "~" + testSubscriptionCode1 + "~";
				
			try
			{
				// insert the subscription information.
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode1));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_Subscription (SubscriptionCode) VALUES ('{0}');", testSubscriptionCode2));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode, book.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode, book1.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode1, book2.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode1, book3.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode2, book4.Id));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("INSERT INTO D_SubscriptionBook (SubscriptionCode, BookId) SELECT '{0}', BookId FROM D_BookInstance WHERE BookInstanceId = {1};", testSubscriptionCode2, book5.Id));
							
				AICPA.Destroyer.User.User user = new User(userId, ReferringSite.Exams);
			
				string newSession = Guid.NewGuid().ToString();
				user.LogOn(newSession, userSubscriptionCode);

				// put book names into an arraylist
				ArrayList UserBookNames = new ArrayList();
				foreach(string bookName in user.UserSecurity.BookName)
				{
					UserBookNames.Add(bookName);	
				}

				// check the user's books for the expected books.
				foreach(string expectedBookname in expectedBookNames)
				{
					if(!UserBookNames.Contains(expectedBookname))
					{
						throw new SecurityException(string.Format("Expected the user to have book name {0}, but did not.", expectedBookname));
					}
					
				}

				// check the user's books for the unexpected books.
				foreach(string unexpectedBookname in unexpectedBookNames)
				{
					if(UserBookNames.Contains(unexpectedBookname))
					{
						throw new SecurityException(string.Format("Did not expect the user to have book name {0}, but did.", unexpectedBookname));
					}
				}
				
			}
			finally
			{
				// clean up the test data.
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode));
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode1));
                SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_SubscriptionBook WHERE SubscriptionCode='{0}';", testSubscriptionCode2));
                ContentShared.DeleteNamedBook(testBook);
				ContentShared.DeleteNamedBook(testBook1);
				ContentShared.DeleteNamedBook(testBook2);
				ContentShared.DeleteNamedBook(testBook3);
				ContentShared.DeleteNamedBook(testBook4);
				ContentShared.DeleteNamedBook(testBook5);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode1));
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, string.Format("DELETE FROM  D_Subscription WHERE SubscriptionCode='{0}';", testSubscriptionCode2));
			
				UserBpcTest.DeleteDestroyerUsers(userId);
			}
		}
	}
	#endregion BookSecurityTests
}
