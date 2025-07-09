using System;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace ExamCandidate
{
	/// <summary>
	/// Summary description for common.
	/// </summary>
	public class common : System.Web.UI.Page
	{

		#region constants
		protected const string APPLICATION_SETTING_DBCONNECTIONSTRING = "DbConnectionString";

		#endregion constants

		#region storeProcedures
			private const string REGISTER_USER = "dt_setUserRegistration";
			private const string GET_PASSWORD = "dt_getUserPassword";
			private const string GET_GUID = "dt_getUserGuid";
			private const string STATUS_BY_ID = "dt_getUserStatus_byId";
			private const string STATUS_BY_DATA = "dt_getUserStatus_byNamePassword";

		#endregion storeProcedures
		
		protected static string DBConnectionString = System.Configuration.ConfigurationSettings.AppSettings[APPLICATION_SETTING_DBCONNECTIONSTRING];

		public common()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		/// <summary>
		/// get the user current status based on the user unique Id
		/// </summary>
		/// <param name="userName">userName</param>
		/// <param name="password">Password</param>
		/// <returns>string with the current user status</returns>
		protected int getCurrentStatus(System.Guid guid)
		{
			int currentStatus = 1000;
			try
			{
				currentStatus = (int)SqlHelper.ExecuteScalar(DBConnectionString,STATUS_BY_ID,guid);
			}
			catch(Exception e)
			{
				currentStatus = 1000;
			}
			return currentStatus;
		}


		/// <summary>
		/// get the user current status based on the userName and password
		/// </summary>
		/// <param name="userName">userName</param>
		/// <param name="password">Password</param>
		/// <returns>string with the current user status</returns>
		protected int getCurrentStatus(string userName, string password)
		{
			int currentStatus = 1000;
			try
			{
				currentStatus = (int)SqlHelper.ExecuteScalar(DBConnectionString,STATUS_BY_DATA,userName,password);
			}
			catch(Exception e)
			{
				currentStatus = 1000;
			}
			return currentStatus;
		}


		/// <summary>
		/// Update exam candidate records based on ID
		/// </summary>
		/// <param name="guid">guid assigned to user</param>
		/// <param name="userName">userid defined by user</param>
		/// <param name="password">new password defined by user</param>
		/// <returns>True if GUID is found.  False if GUID not found</returns>
		protected bool registerUser(System.Guid guid,string userName, string password)
		{
			bool registeredSuccessfully = false;
			try
			{
				registeredSuccessfully = (bool)SqlHelper.ExecuteScalar(DBConnectionString,REGISTER_USER,guid,userName,password);
			}
			catch(Exception e)
			{
				registeredSuccessfully = false;
			}

			return registeredSuccessfully;
		}


		/// <summary>
		/// get user password
		/// </summary>
		/// <param name="guid">User Unique ID</param>
		/// <returns>user password as string</returns>
		protected string getPassword(System.Guid guid)
		{
			string password = string.Empty;
			try
			{
				password = (string)SqlHelper.ExecuteScalar(DBConnectionString,GET_PASSWORD,guid);
			}
			catch(Exception e)
			{
				password = "ERROR: "+e.Message;
			}
			return password;
		}
		
		protected System.Guid getGuid(string userName, string password)
		{
			
			try
			{
				Guid dbGuid = (System.Guid)SqlHelper.ExecuteScalar(DBConnectionString,GET_GUID,userName,password);
				return dbGuid;
			}
			catch(Exception e)
			{
				Guid dbGuid = new Guid("11111111111111111111111111111111");
				return dbGuid;
			}
		}
	}
}
