using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using AICPA.Destroyer.User;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

namespace AICPA.Destroyer
{
	/// <summary>
	/// Summary description for TestShared.
	/// </summary>
	public class TestShared
	{
		// DB connection string																			  Shared_DBConnectionString
		public static string dbConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConnectionString"];
		public static string c2bConnectionString = System.Configuration.ConfigurationSettings.AppSettings["C2bConnectionString"];

	}
	public class BaseTest
	{
		#region public members
		public const string BASIC_C2B_USER_DOMAIN = "test;aag-air;proflit;";
		public const string BASIC_C2B_USER_ACA = "TestFirm";
		public const string BASIC_C2B_USER_CODE = "888-66";
		public const int BASIC_C2B_USER_CONCURRENT_USER_COUNT = 3;

		private User.User CreateBasicC2bUser()
		{			
			this.c2bUserDs = BaseTest.CreateC2bWebServiceUser(Guid.NewGuid(), BASIC_C2B_USER_DOMAIN, BASIC_C2B_USER_ACA, BASIC_C2B_USER_CODE, BASIC_C2B_USER_CONCURRENT_USER_COUNT);
            
            // sburton 2010-01-20: added the userid toString-ed as the "encrypted guid"
            return new User.User(this.c2bUserDs.User[0].UserID.ToString(), this.c2bUserDs.User[0].UserID, ReferringSite.C2b);
		}
		public void DeleteBasicC2bUser()
		{
			// c2bImposter
			foreach (C2bUser.FirmRow firmRow in this.c2bUserDs.Firm)
			{
				BaseTest.DeleteC2bWebServiceFirm(firmRow.FirmID);
			}
			BaseTest.DeleteC2bWebServiceUser(this.c2bUserDs.User[0].UserID);

			// destroyer
			BaseTest.DeleteDestroyerUser(this.basicC2bUser.UserId);
			
			this.basicC2bUser = null;
			this.c2bUserDs = null;
		}
		private C2bUser c2bUserDs = null;
		private User.User basicC2bUser = null;
		public User.User BasicC2bUser
		{
			get
			{
				if (basicC2bUser==null)
				{
					basicC2bUser = this.CreateBasicC2bUser();
				}
				return basicC2bUser;
			}
		}
		#endregion public members
		#region static members
		
		public static void DeleteDestroyerUsers (params Guid[] userIds)
		{
			foreach(Guid userId in userIds)
			{
				BaseTest.DeleteDestroyerUser(userId);
			}
		}

		public static void DeleteDestroyerUser (Guid userId)
		{
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_FeedBack WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_Search WHERE UserId = '" + userId.ToString() + "';");
            SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_UserPreferences WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_CurrentFirmUsers WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_NotificationSubscription WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_Note WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_EventLog WHERE UserId = '" + userId.ToString() + "';");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_User WHERE UserId = '" + userId.ToString() + "';");
		}

		public static C2bUser CreateC2bWebServiceUser(Guid userId, string domainString)
		{
			return BaseTest.CreateC2bWebServiceUser(userId, domainString, null, null, 0);			
		}

		public static C2bUser CreateC2bWebServiceUser(Guid userId, string domainString, string aca, string code, int concurrentUsers)
		{
			C2bUser newUser = new C2bUser();
			newUser.User.AddUserRow(userId, domainString);
			if (aca!=null && code!=null)
			{
				newUser.Firm.AddFirmRow(aca, code, concurrentUsers);
			}
			BaseTest.CreateC2bWebServiceUsers(newUser);
			return newUser;			
		}

		public static void CreateC2bWebServiceUsers(C2bUser c2bUser)
		{
			SqlConnection c2bConnection = new SqlConnection(TestShared.c2bConnectionString);
			SqlCommand insertUserSql = SqlHelper.CreateCommand(c2bConnection, "C2B_CreateUser", c2bUser.User.UserIDColumn.ColumnName, c2bUser.User.UserSubscriptionColumn.ColumnName);
			SqlHelper.UpdateDataset(insertUserSql, null, null, c2bUser, c2bUser.User.TableName);

			if (c2bUser.Firm.Count > 0)
			{
				SqlCommand insertFirmSql = SqlHelper.CreateCommand(c2bConnection, "C2B_CreateFirm", c2bUser.Firm.ACAColumn.ColumnName, c2bUser.Firm.CodeColumn.ColumnName, c2bUser.Firm.ConcurrentUsersColumn.ColumnName, c2bUser.Firm.FirmIDColumn.ColumnName);
				SqlHelper.UpdateDataset(insertFirmSql, null, null, c2bUser, c2bUser.Firm.TableName);				
			}
			foreach (C2bUser.FirmRow firmRow in c2bUser.Firm)
			{
				foreach (C2bUser.UserRow userRow in c2bUser.User)
				{
					SqlHelper.ExecuteNonQuery(TestShared.c2bConnectionString, "C2B_CreateUserFirm", userRow.UserID, firmRow.FirmID);
				}
			}
		}

		public static void DeleteC2bWebServiceUsers(params C2bUser[] c2bUsers)
		{
			foreach(C2bUser c2bUser in c2bUsers)
			{
				DeleteC2bWebServiceUsers(c2bUser);
			}
		}
		public static void DeleteC2bWebServiceUsers(C2bUser c2bUser)
		{
			foreach (C2bUser.FirmRow firmRow in c2bUser.Firm)
			{
				BaseTest.DeleteC2bWebServiceFirm(firmRow.FirmID);
			}
			foreach (C2bUser.UserRow userRow in c2bUser.User)
			{
				BaseTest.DeleteC2bWebServiceUser(userRow.UserID);				
			}
		}

		public static void DeleteC2bWebServiceUser(Guid userId)
		{
			SqlHelper.ExecuteNonQuery(TestShared.c2bConnectionString, "C2B_DeleteUser", userId);
		}

		public static void DeleteC2bWebServiceFirm(int firmId)
		{
			SqlHelper.ExecuteNonQuery(TestShared.c2bConnectionString, "C2B_DeleteFirm", firmId);
		}
	
		/// <summary>
		/// Asserts that an exception message matches an expected message.  Accommodates exception messages
		/// that contain formatting substitution strings, e.g., {xxx}.
		/// The expected message is split into substrings that occur before and after each formatting expression.
		/// All such substrings must exist in the actual message, or an Assertion.Fail is called.
		/// </summary>
		/// <param name="expectedMessage">The expected exception message string.  This string can contain formatting expressions, e.g. {0}</param>
		/// <param name="actualMessage">The actual exception message string.</param>
		public static void CheckExceptionMessage(string expectedMessage, string actualMessage)
		{
			// Split the string into substrings that break on the "{xxx}" pattern.
			Regex r = new Regex("\\{.+?\\}");
			string[] s = r.Split(expectedMessage);
			//Console.WriteLine("\n" + expectedMessage);
			//Console.WriteLine(actualMessage);

			// Make sure every substring from the expected message is found in the actual message.
			foreach (string str in s)
			{
				//Console.WriteLine("  " + (actualMessage.IndexOf(str) > -1) + ": " + str);
				if (actualMessage.IndexOf(str) == -1)
				{
					Assert.Fail("Unexpected exception message.  Expected = " + expectedMessage + ", Actual = " + actualMessage);
				}
			}
		}


		public static void CheckStringValue(string expectedValue, string actualValue)
		{
			// Split the string into substrings that break on the "{xxx}" pattern.
			Regex r = new Regex("\\{.+?\\}");
			string[] s = r.Split(expectedValue);

			// Make sure every substring from the expected value is found in the actual value.
			foreach (string str in s)
			{
				//Console.WriteLine("  " + (actualValue.IndexOf(str) > -1) + ": " + str);
				if (actualValue.IndexOf(str) == -1)
				{
					Assert.Fail("Unexpected string value.  Expected = " + expectedValue + ", Actual = " + actualValue);
				}
			}
		}



		/// <summary>
		/// Execute an arbitrary SQL query against the database, returning no rows.
		/// </summary>
		/// <param name="query">An SQL query string</param>
		public static void ExecuteSQL(string query)
		{
			SqlConnection conn = new SqlConnection(TestShared.dbConnectionString);
			conn.Open();
			try
			{
				SqlCommand cmd = new SqlCommand(query);
				cmd.Connection = conn;
				int results = cmd.ExecuteNonQuery();
			}
			finally
			{
				conn.Close();
			}
		}


		/// <summary>
		/// Given a SELECT query, return the results of that query in a DataSet.
		/// </summary>
		/// <param name="query">a SELECT query string.</param>
		/// <returns>an untyped DataSet containing the results of the given query.</returns>
		public static DataSet GetDataSet(string query)
		{
			DataSet ds = new DataSet();
			SqlConnection conn = new SqlConnection(TestShared.dbConnectionString);
			SqlDataAdapter da = new SqlDataAdapter(query, conn);
			conn.Open();
			try
			{
				SqlCommand cmd = new SqlCommand(query);
				cmd.Connection = conn;
				da.Fill(ds);
			}
			finally
			{
				conn.Close();
			}
			return ds;
		}


		/// <summary>
		/// Given a SELECT query, return the results of that query in a DataSet.
		/// </summary>
		/// <param name="query">a SELECT query string.</param>
		/// <returns>an untyped DataSet containing the results of the given query.</returns>
		/// <param name="t">SqlTransaction object representing the transaction in which this query should take place.</param>
		public static DataSet GetDataSet(string query, SqlTransaction t)
		{
			DataSet ds = new DataSet();
			SqlConnection conn = t.Connection;
			SqlDataAdapter da = new SqlDataAdapter(query, conn);
			SqlCommand cmd = da.SelectCommand;
			cmd.Connection = conn;
			cmd.Transaction = t;
			da.Fill(ds);
			return ds;
		}


		public static Object GetDBScalarValue(string query)
		{
			Object returnValue = null;
			SqlCommand cmd = new SqlCommand(query);
			SqlConnection conn = new SqlConnection(TestShared.dbConnectionString);
			cmd.Connection = conn;
			try
			{
				conn.Open();
				returnValue = cmd.ExecuteScalar();
			}
			finally
			{
				conn.Close();
			}
			return returnValue;
		}


		/// <summary>
		/// Assert an NUnit failure if the given record does not exist in the database.
		/// </summary>
		/// <param name="table">The name of the table in which to look for the target record.</param>
		/// <param name="whereConditions">The attributes of the record (expressed as a WHERE clause) that we hope to find</param>
		/// <param name="checkExists">if TRUE, asserts the EXISTENCE of the record; otherwise, asserts the NON-existence.</param>
		private static void CheckDBRecord(string table, string whereConditions, bool checkExists, SqlTransaction t)
		{
			SqlCommand cmd = new SqlCommand(string.Format("IF {2}EXISTS (SELECT * FROM {0} WHERE {1}) RAISERROR ('Error', 16, 1)", table, whereConditions, (checkExists ? "NOT " : "")));
			SqlConnection conn;
			if (t == null)
			{
				conn = new SqlConnection(TestShared.dbConnectionString);
				conn.Open();
			}
			else
			{
				conn = t.Connection;
				cmd.Transaction = t;
			}
			try
			{
				cmd.Connection = conn;
				int results = cmd.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				if (e.Message == "Error")
				{
					Assert.Fail(string.Format("{2} in {0} WHERE {1}.", table, whereConditions, (checkExists ? "The expected record was not found" : "An unexpected record was found")));
				}
				else
				{
					throw (e);
				}
			}
			finally
			{
				if (t == null)
				{
					conn.Close();
				}
			}
		}


		/// <summary>
		/// Cause an NUnit assertion failure if the specified record does not exist in the database.
		/// </summary>
		/// <param name="table">The name of the table containing the record we are looking for</param>
		/// <param name="whereConditions">The attributes of the record to find, expressed as a WHERE clause 
		/// (but without the word "WHERE").</param>
		public static void CheckDBRecordExists(string table, string whereConditions)
		{
			CheckDBRecord(table, whereConditions, true, null);
		}


		/// <summary>
		/// Cause an NUnit assertion failure if the specified record does not exist in the database.
		/// </summary>
		/// <param name="table">The name of the table containing the record we are looking for</param>
		/// <param name="whereConditions">The attributes of the record to find, expressed as a WHERE clause 
		/// (but without the word "WHERE").</param>
		/// <param name="t">SqlTransaction object representing the transaction in which this query should take place.</param>
		public static void CheckDBRecordExists(string table, string whereConditions, SqlTransaction t)
		{
			CheckDBRecord(table, whereConditions, true, t);
		}


		/// <summary>
		/// Cause an NUnit assertion failure if the specified record exists in the database.
		/// </summary>
		/// <param name="table">The name of the table containing the record we are looking for</param>
		/// <param name="whereConditions">The attributes of the record to find, expressed as a WHERE clause 
		/// (but without the word "WHERE").</param>
		public static void CheckDBRecordNotExists(string table, string whereConditions)
		{
			CheckDBRecord(table, whereConditions, false, null);
		}


		/// <summary>
		/// Cause an NUnit assertion failure if the specified record exists in the database.
		/// </summary>
		/// <param name="table">The name of the table containing the record we are looking for</param>
		/// <param name="whereConditions">The attributes of the record to find, expressed as a WHERE clause 
		/// (but without the word "WHERE").</param>
		/// <param name="t">SqlTransaction object representing the transaction in which this query should take place.</param>
		public static void CheckDBRecordNotExists(string table, string whereConditions, SqlTransaction t)
		{
			CheckDBRecord(table, whereConditions, false, t);
		}


		/// <summary>
		/// Checks the contents of the given DataTable against values obtained from the database using the given query.
		/// Asserts an NUnit failure if any of the following do not match between the DataTable and the recordset returned by the query:
		/// 1) The number of rows
		/// 2) The number of columns (if exactColumnCount == true, number of columns must match exactly; if false, dt must have at least as many columns as the query dataset)
		/// 3) The values contained in all the fields.
		/// </summary>
		/// <param name="dt">A DataTable whose values we want to check using data straight from the database.</param>
		/// <param name="query">A query that returns the same records and columns, in the same order, that we expect to find in the given DataTable.</param>
		/// <param name="exactColumnCount">if exactColumnCount == true, number of columns must match exactly; if false, dt must have at least as many columns as the query dataset</param>
		public static void CheckDataTable(DataTable dt, string query, bool exactColumnCount)
		{
			DataSet queryDS = GetDataSet(query);
			DataTable queryDT = queryDS.Tables[0];
			Assert.AreEqual(queryDT.Rows.Count, dt.Rows.Count, "Row count");
			Assert.IsTrue(queryDT.Columns.Count <= dt.Columns.Count, "Dataset has fewer columns than the query");
			for (int i = 0; i < queryDT.Rows.Count; i++)
			{
				DataRow queryRow = queryDT.Rows[i];
				DataRow dsRow = dt.Rows[i];
				for (int j = 0; j < queryDT.Columns.Count; j++)
				{
					//Console.WriteLine(queryRow[j].ToString() + " = " + dsRow[j].ToString());
					Assert.AreEqual(queryRow[j].ToString(), dsRow[j].ToString(), string.Format("Record {0}, {1} value", i, queryDT.Columns[j].ColumnName));
				}
			}
		}


		/// <summary>
		/// Checks the contents of the given DataTable against values obtained from the database using the given query.
		/// Asserts an NUnit failure if any of the following do not match between the DataTable and the recordset returned by the query:
		/// 1) The number of rows
		/// 2) The number of columns - dt must have at least as many columns as the query dataset
		/// 3) The values contained in all the fields.
		/// </summary>
		/// <param name="dt">A DataTable whose values we want to check using data straight from the database.</param>
		/// <param name="query">A query that returns the same records and columns, in the same order, that we expect to find in the given DataTable.</param>
		public static void CheckDataTable(DataTable dt, string query)
		{
			CheckDataTable(dt, query, false);
		}
		#endregion static members
	}

}
