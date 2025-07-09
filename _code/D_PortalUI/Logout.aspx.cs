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
using AICPA.Destroyer.User;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for Logout.
	/// </summary>
	public partial class Logout : System.Web.UI.Page
	{
		public const string SESSPARAM_CURRENTUSER = "CurrentUser";
		public const string SESSPARAM_CURRENTSITE = "CurrentSite";
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//initialize to null
			IUser retUser = null;

			//try to get the user from the session
			retUser = (IUser)Session[SESSPARAM_CURRENTUSER];
			retUser.LogOff();
			Session.Abandon();
			//Session.Remove(SESSPARAM_CURRENTUSER);

			//ISite site = null;

			//site = (ISite)Session[SESSPARAM_CURRENTSITE];
			//Session.Remove(SESSPARAM_CURRENTSITE);


			string closeWinScript = "<script language='javascript'>window.parent.close()</script>";

            //Obsolete function djf 1/19/10
            //http://msdn.microsoft.com/en-us/library/aa479390.aspx
			//Page.RegisterStartupScript("closeWinScript", closeWinScript);
            ClientScript.RegisterStartupScript(this.GetType(), "closeWinScript", closeWinScript);
			

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
