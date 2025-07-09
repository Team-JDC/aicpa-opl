using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AICPA.Destroyer.User.Event;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for error.
	/// </summary>
	public partial class error : System.Web.UI.Page
	{
		private const string ERROR_MODULE = "ERROR";
		private const string DELIMITER = "|^|";
		protected System.Web.UI.WebControls.Label jsLabel;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			string errorType = Request.QueryString["errorType"] == null || Request.QueryString["errorType"] == "" ? "100" : Request.QueryString["errorType"];
			string errorMsg = string.Empty;
			string errorTitle = string.Empty;
			string errorMsgDisplay = string.Empty;
			string errorDesc = string.Empty;

			HttpContext ctx = HttpContext.Current;
			//string test = Session["Error_Msg"].ToString();
			switch(errorType)
			{
				case "100":
					errorTitle = "Generic Error";
					errorMsgDisplay = "<p>The item you requested does not exist on this server or cannot be served at this moment. The error has been logged.</p>";
					errorMsg = "Generic Error";
					break;
				case "403":
					errorTitle = "Forbidden";
					errorMsgDisplay = "<p>You do not have access to the item requested. The error has been logged.</p>";
					errorMsg = "Forbidden Access";
					break;
				case "404":
					errorTitle = "Page not found";
					errorMsgDisplay = "<p>The item you requested does not exist on this server. The error has been logged.</p>";
					errorMsg = "Page not Found";
					break;
				case "500":
					errorTitle = "Server Error";
					errorMsgDisplay = "<p>The item you requested cannot be served at this moment. The error has been logged.</p>";
					string errorStackTxt = "<b>Stack:</b>"+Session["Error_Desc"].ToString();
					string errorPageLoc = Session["Error_Page"].ToString();
					errorMsg = Session["Error_Msg"].ToString() + DELIMITER + errorStackTxt;
					errorTitle = errorTitle + DELIMITER + errorPageLoc;
					errorStackTxt = errorStackTxt.Replace("at ","<br>at ");
					errorStackTxt = errorStackTxt + "<br><b>Location:</b><br> "+errorPageLoc;
					errorStack.Text = errorStackTxt;
					errorStack.Visible = true;
					ShowErrorInfo.Text = "Technical Information: <br><script>displayInfo();</script>";
					ShowErrorInfo.Visible = true;
					ErrorDescription.Text = Session["Error_Msg"].ToString();
					break;
				default:
					errorTitle = "UnHandled Error";
					errorMsgDisplay = "<p>The item you requested does not exist on this server or cannot be served at this moment. The error has been logged.</p>";
					string errorStackTxt2 = "<b>Stack:</b>"+Session["Error_Desc"].ToString();
					string errorPageLoc2 = Session["Error_Page"].ToString();
					errorMsg = Session["Error_Msg"].ToString() + DELIMITER + errorStackTxt2;
					errorTitle = errorTitle + DELIMITER + errorPageLoc2;
					errorStackTxt2 = errorStackTxt2.Replace("at ","<br>at ");
					errorStackTxt2 = errorStackTxt2 + "<br><b>Location:</b><br> "+errorPageLoc2;
					errorStack.Text = errorStackTxt2;
					errorStack.Visible = true;
					ShowErrorInfo.Text = "Technical Information: <br><script>displayInfo();</script>";
					ShowErrorInfo.Visible = true;
					ErrorDescription.Text = Session["Error_Msg"].ToString();
					break;
			}
			
			Event eventError = new Event(EventType.Error,DateTime.Now,5,ERROR_MODULE,errorType,errorTitle,errorMsg);
			eventError.Save(false);
			errorMsgLb.Text = errorMsgDisplay;
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
	}
}
