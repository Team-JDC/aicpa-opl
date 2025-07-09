using System;
using System.Data.SqlClient;
using System.Data;
using NUnit.Framework;
using AICPA.Destroyer.User;
using Microsoft.ApplicationBlocks.Data;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Subscription;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.OtherTests
{
	#region BaseDalcTest
	/// <summary>
	/// Summary description for BaseDalcTests.
	/// </summary>
	public class BaseDalcTest : BaseTest
	{
		
	}
	#endregion BaseDalcTest

	[TestFixture]
	public class BaseDalcExceptionTest : BaseDalcTest
	{
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithSqlCommandsLogUpdateDataset()
		{
			string originalInsertCurrentFirmUserStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o     where o.id = c.id and c.id = object_id(N'[dbo].[D_InsertCurrentFirmUser]') order by c.number, c.colid");
			originalInsertCurrentFirmUserStoredProcedure = originalInsertCurrentFirmUserStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER      PROCEDURE dbo.D_InsertCurrentFirmUser  (@Aca varchar(64), @Code varchar(64),@UserId uniqueidentifier)    AS    RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				string newSession = Guid.NewGuid().ToString();
				try
				{
					this.BasicC2bUser.LogOn(newSession);
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "Unexpected error returned.");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalInsertCurrentFirmUserStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");
				
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithSqlCommandsLogUpdateDataRow()
		{
			string spName = "D_InsertSubscriptionBook";
			string originalStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o     where o.id = c.id and c.id = object_id(N'[dbo].[" + spName + "]') order by c.number, c.colid");
			originalStoredProcedure = originalStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER   PROCEDURE dbo.D_InsertSubscriptionBook(	@SubscriptionCode varchar(32),	@BookName varchar(128))AS RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				string newSession = Guid.NewGuid().ToString();
				try
				{					
					string[] garbage = new string[1]{"asdfaf"};
					Subscription newSubscription = new Subscription("sdfsd", garbage);
					newSubscription.Save();
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "Unexpected error returned.");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");
			}
		}
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithParametersLogExecuteDataReader()
		{
			string originalStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o     where o.id = c.id and c.id = object_id(N'[dbo].[D_GetSubscriptionBookNames]') order by c.number, c.colid");
			originalStoredProcedure = originalStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER     PROCEDURE dbo.D_GetSubscriptionBookNames 	@DomainString varchar(4096), @DomainStringDelimiter char(1), @SubscriptionCodeCount int AS    RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				string newSession = Guid.NewGuid().ToString();
				try
				{
					this.BasicC2bUser.LogOn(newSession);
					string[] bookNames = this.BasicC2bUser.UserSecurity.BookName;
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "Unexpected error returned.");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");				
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithParametersLogExecuteNonQuery()
		{
			string storedProcName = "D_InsertUser";
			string originalStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o  where o.id = c.id and c.id = object_id(N'[dbo].[" + storedProcName + "]') order by c.number, c.colid");
			originalStoredProcedure = originalStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER PROCEDURE dbo.D_InsertUser (@UserId uniqueidentifier, @ReferringSite varchar(16), @Email varchar(50)) AS RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				string newSession = Guid.NewGuid().ToString();
				try
				{
					this.BasicC2bUser.LogOn(newSession);
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "This assertion assumed the Guid passed into the ALTER D_InsertUser stored proc would be returned in the error message; did you change the InsertUser stored proc, maybe the number of parameters?");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");				
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithParametersLogExecuteScalar()
		{
			string storedProcName = "D_GetFirmUserCount";
			string originalStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o  where o.id = c.id and c.id = object_id(N'[dbo].[" + storedProcName + "]') order by c.number, c.colid");
			originalStoredProcedure = originalStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER PROCEDURE dbo.D_GetFirmUserCount @Aca varchar(64), @Code varchar(64), @SessionTimeoutSeconds int AS RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				string newSession = Guid.NewGuid().ToString();
				try
				{
					this.BasicC2bUser.LogOn(newSession);
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "Unexpected error returned.");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");				
				this.DeleteBasicC2bUser();
			}
		}
		/// <summary>
		///		This test actually changes a stored procedure and then sets it back to what it was 
		///		in the end.  It does this so that the datalayer will throw an error and we
		///		can test some unexpected error error handling.
		///		
		///		Do not execute this code and break out of it without allowing the "finally" 
		///		to execute, otherwise the stored procedure will be broken for real.
		/// </summary>
		[Test]
		public void LogDalcErrorWithParametersLogFillDataset()
		{
			string storedProcName = "D_GetSite";
			string originalStoredProcedure = (string)SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text,"select c.text from dbo.syscomments c, dbo.sysobjects o  where o.id = c.id and c.id = object_id(N'[dbo].[" + storedProcName + "]') order by c.number, c.colid");
			originalStoredProcedure = originalStoredProcedure.Replace("CREATE", "ALTER");
			string errorMessage = Guid.NewGuid().ToString();
			string siteName = Guid.NewGuid().ToString();
			try
			{				
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "ALTER   PROCEDURE dbo.D_GetSite (  @SiteId int) AS RAISERROR('" + errorMessage + "', 16, 1) RETURN");
				ISite site1 = new Site(siteName, "site title", "site desc", "site search uri");
				site1.Save();

				string newSession = Guid.NewGuid().ToString();
				try
				{
					ISite site3 = new Site(site1.Id);
					int i = site3.Id;
					Assert.Fail("Should have failed with error, but did not");				
				}
				catch(Exception e)
				{
					Assert.IsTrue(e.Message.IndexOf(errorMessage) > -1, "Unexpected error returned.");
				}
				BaseDalcTest.CheckDBRecordExists("D_EventLog", "Description like '%" + errorMessage + "%'");
			}
			finally
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, originalStoredProcedure);
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE Description Like '%" + errorMessage + "%'");				
				ContentShared.DeleteNamedSite(siteName);
			}
		}
	}
}
