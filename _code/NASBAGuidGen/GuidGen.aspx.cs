using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;

namespace NASBAGUIDGEN
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class WebForm1 : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		
		protected void Button1_Click(object sender, System.EventArgs e)
		{
			//Create the Text File for the user to Download
			
			DateTime today = new DateTime();
			today = DateTime.Now;
			//string filepath = ConfigurationSettings.AppSettings["TextFilePath"];
			string tempfilename = "NasbaGuids" + today.ToShortDateString()+ ".txt";
			string filename= tempfilename.Replace("/","_");
			//string textfile = filepath + filename;
			//StreamWriter GuidFile = new StreamWriter(textfile, false);
			string Guidfile ="";

			
			//Decide how many Guids to generate
			int numOfGuids  = System.Convert.ToInt32(DropDownList1.SelectedValue);
			string connectionString = ConfigurationSettings.AppSettings["Connection String"];
			//Create a new connection object
			SqlConnection examConnection = new SqlConnection(connectionString);	
			//	
			string commandHeader = ConfigurationSettings.AppSettings["CommandString Header"];
			string commandFooter = ConfigurationSettings.AppSettings["CommandString Footer"];
			
			for (int i = 0; i <= numOfGuids; i++)
			{
				/// get the new Guid to insert
				Guid NewGuidID = Guid.NewGuid();
				//Add the Guid to the string to download
				Guidfile += NewGuidID.ToString() + "\r\n";

				//Insert it into the database
				string commandString = commandHeader + "'" + NewGuidID.ToString() + "'" + commandFooter;

				//Create a SQL Command Object
				SqlCommand insertGuid = new SqlCommand(commandString,examConnection);

				//Open the connection
				insertGuid.Connection.Open();
				//insert the values into the database
				insertGuid.ExecuteNonQuery();
				//Close the connection
				insertGuid.Connection.Close();
				//Get rid of the object so .NET can clean in up
				insertGuid.Dispose();

			}
			// Set the URL to the test file
			Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
			Response.AddHeader("Content-Length", Guidfile.Length.ToString());
			Response.ContentType = "text/plain";
			Response.Write(Guidfile);
			Response.Flush();
			Response.End();

		}

		private void GUIDOPT_CheckedChanged(object sender, System.EventArgs e)
		{
			int i = 1;
		}
	}
}
