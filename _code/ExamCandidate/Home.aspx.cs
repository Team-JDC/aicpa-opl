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

namespace ExamCandidate
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class WebForm1 : common
	{
	
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// see if there is a GUID on the URL
			string examGuid  = Request.Params["id"];
			
			//if there is a GUID in the URL then we need to see if they are registered.
			if ((examGuid != null) && (examGuid.Length > 1))
			{
				//put the guid in a cookie so we can get it later if we need it
				HttpCookie c_Guid = new HttpCookie("Guid");
				c_Guid.Value=examGuid;
				Response.Cookies.Add(c_Guid);
				
				Guid userGuid = new Guid(examGuid);
				
				//Get current status
				int status = getCurrentStatus(userGuid);

				// If the user is not registered then send them to the registration page
				if (status == 0)
				{
					//write the Guid to a cookie so that we can get back to it.
					jslabel.Text = "<script>changeContent(2);</script>";
					jslabel.Visible = true;
				}
				//The Users session has expired send them to the proper error page
				if (status == 2)
				{
					jslabel.Text = "<script>changeContent(5);</script>";
					jslabel.Visible = true;
	
				}
				if (status > 2)
				{
					jslabel.Text = "<script>changeContent(6);</script>";
					jslabel.Visible = true;
				}
			}	
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
