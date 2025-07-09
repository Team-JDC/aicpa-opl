using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Text;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;

namespace D_AdminUI.Reporting
{
	/// <summary>
	/// Summary description for reporting_db.
	/// </summary>
	public class reporting_db : System.Web.UI.Page
	{
		
		#region Constants
		protected const string APPLICATION_SETTING_DBCONNECTIONSTRING = "DbConnectionString";

		#endregion constants

		#region StoreProcedures
		private const string GET_USERS_LIST = "dbo.D_AdminReports_getTopUsers";
		private const string GET_DOCUMENTS_REQUESTED = "dbo.D_AdminReports_getTopBooks";
		private const string GET_USERS_FOR_BOOK = "dbo.D_AdminReports_BookUserLevel";
		private const string GET_USER_BOOK_ACCESSED = "dbo.D_AdminReports_UserBookLevel";
		private const string GET_USER_LOGIN_HISTORY = "dbo.D_AdminReports_getUserLoginHist";
		private const string GET_KEYWORDS_SEARCHED = "dbo.D_AdminReports_getSearchedKeys";
		private const string GET_ERROR_TYPE_SUMMARY = "dbo.D_AdminReports_getErrorCount";
		private const string GET_ERRORS_BREAKUP_BY_TYPE = "dbo.D_AdminReports_listErrors";
		private const string GET_BOOK_DOCUMENT_VISITS = "dbo.D_AdminReports_BookDocumentLevel";

		#endregion StoreProcedures

		#region Global Variable
		protected static string DBConnectionString = System.Configuration.ConfigurationSettings.AppSettings[APPLICATION_SETTING_DBCONNECTIONSTRING];
		int divId = 1;
		#endregion Global Variable

		public reporting_db()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected ICollection getTopUsersList(int daysToGoBack,int userType)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("userId",typeof(string)));
			dt.Columns.Add(new DataColumn("userType",typeof(string)));
			dt.Columns.Add(new DataColumn("accessCount",typeof(int)));
			dt.Columns.Add(new DataColumn("userName",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_USERS_LIST,daysToGoBack,userType);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetValue(1).ToString();
					dr[3] = reader.GetInt32(2);
					dr[4] = userType == 1 ? "extra" : reader.GetValue(3).ToString();
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getTopBooksList(int daysToGoBack)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("bookTitle",typeof(string)));
			dt.Columns.Add(new DataColumn("bookId",typeof(string)));
			dt.Columns.Add(new DataColumn("totalHits",typeof(int)));
			dt.Columns.Add(new DataColumn("uniqueUsers",typeof(int)));
			dt.Columns.Add(new DataColumn("bookName",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_DOCUMENTS_REQUESTED,daysToGoBack);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(2).ToString();
					dr[2] = reader.GetInt32(1);
					dr[3] = reader.GetInt32(3);
					dr[4] = reader.GetInt32(4);
					dr[5] = reader.GetValue(0).ToString();
					dt.Rows.Add(dr);
					i++;	
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;

		}

		protected ICollection getUserAccessedBooks(int daysToGoBack,string userId)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("bookName",typeof(string)));
			dt.Columns.Add(new DataColumn("hitCount",typeof(string)));
			dt.Columns.Add(new DataColumn("loggedInfo",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_USER_BOOK_ACCESSED,daysToGoBack,userId);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetInt32(2);
					dr[3] = reader.GetValue(3).ToString();
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getUserAccessHistory(int daysToGoBack,string userId)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("userType",typeof(string)));
			dt.Columns.Add(new DataColumn("accessedDate",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_USER_LOGIN_HISTORY,daysToGoBack,userId);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetValue(1).ToString();
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getBookUserList(int daysToGoBack,string description)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("userId",typeof(string)));
			dt.Columns.Add(new DataColumn("totalHits",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_USERS_FOR_BOOK,daysToGoBack,description);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetInt32(1);
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getSearchKeys(int daysToGoBack,bool found)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 1;
			int j = found ? 0 : 1;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("keyword",typeof(string)));
			dt.Columns.Add(new DataColumn("searchedCount",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_KEYWORDS_SEARCHED,daysToGoBack,j);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetInt32(1);
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getErrorSummary(int daysToGoBack)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;

			dt.Columns.Add(new DataColumn("errorTypeDescription",typeof(string)));
			dt.Columns.Add(new DataColumn("totalHits",typeof(string)));
			dt.Columns.Add(new DataColumn("errorType",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_ERROR_TYPE_SUMMARY,daysToGoBack);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = reader.GetValue(0).ToString();
					dr[1] = reader.GetInt32(1);
					dr[2] = reader.GetValue(2).ToString();
					dt.Rows.Add(dr);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getErrorDetail(int daysToGoBack,string errorType)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;

			dt.Columns.Add(new DataColumn("errorTypeDescription",typeof(string)));
			dt.Columns.Add(new DataColumn("description",typeof(string)));
			dt.Columns.Add(new DataColumn("totalHits",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_ERRORS_BREAKUP_BY_TYPE,daysToGoBack,errorType);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = reader.GetValue(0).ToString();
					dr[1] = this.errorParser(reader.GetValue(1).ToString());
					dr[2] = reader.GetInt32(2);
					dt.Rows.Add(dr);
				}
			}
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected ICollection getBookDocumentVisits(int daysToGoBack,string bookName)
		{
			DataTable dt = new DataTable();
			SqlDataReader reader;
			DataRow dr;
			int i = 0;

			dt.Columns.Add(new DataColumn("rowId",typeof(int)));
			dt.Columns.Add(new DataColumn("DocumentTitle",typeof(string)));
			dt.Columns.Add(new DataColumn("DocumentHits",typeof(string)));
			dt.Columns.Add(new DataColumn("UniqueUsers",typeof(string)));

			try
			{
				reader = SqlHelper.ExecuteReader(DBConnectionString,GET_BOOK_DOCUMENT_VISITS,daysToGoBack,bookName);
				while(reader.Read())
				{
					dr = dt.NewRow();
					dr[0] = i;
					dr[1] = reader.GetValue(0).ToString();
					dr[2] = reader.GetInt32(1);
					dr[3] = reader.GetInt32(2);
					dt.Rows.Add(dr);
					i++;
				}
			}
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		private string errorParser(string errorDesc)
		{
			if(errorDesc.IndexOf("|^|") > -1)
			{
				if(errorDesc.IndexOf("|^|") == 0)
				{
					errorDesc = "ERROR DESCRIPTION NOT AVAILABLE"+errorDesc;
				}
				string divUniqueId = "div_"+divId;
				string replacer = string.Format("</span><br><div id='{0}' style='display:none;'>",divUniqueId);
				errorDesc = errorDesc.Replace("|^|",replacer);
				errorDesc = string.Format("<span title='Click for Details' onclick=\"showDiv('{0}');\" style='cursor:hand;'>{1}</div>",divUniqueId,errorDesc);
				divId++;
			}
			return errorDesc;
		}
	}
}
