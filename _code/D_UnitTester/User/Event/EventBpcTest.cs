using System;
using NUnit.Framework;
using AICPA.Destroyer.User;
using Microsoft.ApplicationBlocks.Data;
using System.Data;

namespace AICPA.Destroyer.User.Event
{
	/// <summary>
	/// Summary description for EventBpcTest.
	/// </summary>
	public class EventBpcTest : BaseTest
	{
		
	}

	[TestFixture]
	public class CreateEvent : EventBpcTest
	{
		#region Overloaded EventLog Tests
		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_CheckToBeLoggedFirst()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{				
				if (Event.IsEventToBeLogged(EventType.Info, 5, "CreateEvent", "LogEvent_CheckToBeLoggedFirst"))
				{
					Event testEvent = new Event(EventType.Info, DateTime.Now, 5, "CreateEvent", "LogEvent_CheckToBeLoggedFirst", eventName, "This is a test of the logging class");
					testEvent.Save(false);
				}
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the log
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_CheckToBeLoggedFirst()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				if (Event.IsEventToBeLogged(EventType.Info, 6, "CreateEvent", "DO_NOT_LogEvent_CheckToBeLoggedFirst"))
				{
					Event testEvent = new Event(EventType.Info, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_CheckToBeLoggedFirst", eventName, "This is a test of the logging class");
					testEvent.Save(false);
				}
				CheckDBRecordNotExists("D_EventLog", "Name='" + eventName +"'");
			}
			finally
			{
				// delete the log if it was inserted, even though it wasn't supposed to be!
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_CheckToBeLoggedFirstWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{		
				if (Event.IsEventToBeLogged(EventType.Info, 5, "CreateEvent", "LogEvent_CheckToBeLoggedFirstWithUser"))
				{
					Event testEvent = new Event(EventType.Info, DateTime.Now, 5, "CreateEvent", "LogEvent_CheckToBeLoggedFirstWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save(false);
				}
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_CheckToBeLoggedFirstWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{			
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{					
					if (Event.IsEventToBeLogged(EventType.Info, 6, "CreateEvent", "DO_NOT_LogEvent_CheckToBeLoggedFirstWithUser"))
					{
						Event testEvent = new Event(EventType.Info, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_CheckToBeLoggedFirstWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
						testEvent.Save(false);
					}
				}
				CheckDBRecordNotExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		#endregion Overloaded Save EventLog Tests

		#region Basic EventType EventLog Tests
		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificEventTypeSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Info, DateTime.Now, 5, "CreateEvent", "LogEvent_SpecificEventTypeSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the log
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificEventTypeSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Info, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_SpecificEventTypeSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordNotExists("D_EventLog", "Name='" + eventName +"'");
			}
			finally
			{
				// delete the log if it was inserted, even though it wasn't supposed to be!
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificEventTypeWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Info, DateTime.Now, 5, "CreateEvent", "LogEvent_SpecificEventTypeWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificEventTypeSettingWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Info, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_SpecificEventTypeSettingWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordNotExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		#endregion Basic EventType EventLog Tests

		#region Basic Module EventLog Tests
		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificModuleSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Usage, DateTime.Now, 5, "CreateEvent", "LogEvent_SpecificModuleSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the log
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificModuleSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Usage, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_SpecificModuleSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordNotExists("D_EventLog", "Name='" + eventName +"'");
			}
			finally
			{
				// delete the log if it was inserted, even though it wasn't supposed to be!
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificModuleSettingWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Usage, DateTime.Now, 5, "CreateEvent", "LogEvent_SpecificModuleSettingWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificModuleSettingWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Usage, DateTime.Now, 6, "CreateEvent", "DO_NOT_LogEvent_SpecificModuleSettingWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordNotExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				// delete the user and log if it was created.
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		#endregion Basic Module EventLog Tests

		#region Basic Method EventLog Tests
		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificMethodSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Usage, DateTime.Now, 9, "CreateEvent", "LogEvent_SpecificMethodSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				// delete the log
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificMethodSetting()
		{
			string eventName = Guid.NewGuid().ToString();
			try
			{
				Event testEvent = new Event(EventType.Usage, DateTime.Now, 2, "CreateEvent", "DO_NOT_LogEvent_SpecificMethodSetting", eventName, "This is a test of the logging class");
				testEvent.Save();
				CheckDBRecordNotExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				// delete the log if it was inserted, even though it wasn't supposed to be!
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE name = '" + eventName + "';");
			}
		}

		/// <summary>
		/// Create an eventLog that will commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void LogEvent_SpecificMethodSettingWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Usage, DateTime.Now, 9, "CreateEvent", "LogEvent_SpecificMethodSettingWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordExists("D_EventLog", "name='" + eventName + "'");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}

		/// <summary>
		/// Create an eventLog that will NOT commit because of a matching method/module specific log setting in the config file
		/// </summary>
		[Test]
		public void DO_NOT_LogEvent_SpecificMethodSettingWithUser()
		{
			string eventName = Guid.NewGuid().ToString();
			this.BasicC2bUser.LogOn("8DA11D3A-0B52-4334-9A42-7150F489E95D");
			try
			{
				if(this.BasicC2bUser.UserSecurity.Authenticated)
				{
					Event testEvent = new Event(EventType.Usage, DateTime.Now, 2, "CreateEvent", "DO_NOT_LogEvent_SpecificMethodSettingWithUser", eventName, "This is a test of the logging class", this.BasicC2bUser);
					testEvent.Save();
				}
				CheckDBRecordNotExists("D_EventLog", "Name='" + eventName + "'");
			}
			finally
			{
				this.BasicC2bUser.LogOff();
				this.DeleteBasicC2bUser();
			}
		}
		#endregion Basic Method EventLog Tests
				
	}
}
