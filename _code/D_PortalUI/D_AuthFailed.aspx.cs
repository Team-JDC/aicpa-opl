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
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.User.Event;


namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_AuthFailed.
	/// </summary>
	public partial class D_AuthFailed : System.Web.UI.Page
	{
		public const string SESSPARAM_CURRENTUSER = "CurrentUser";
		System.Web.UI.WebControls.Label Autherror = null;
				
		protected void Page_Load(object sender, System.EventArgs e)
		{
			IUser retUser = null;
			retUser = (IUser)Session[SESSPARAM_CURRENTUSER];
			if (retUser != null)
			{
				  Autherror.Text = retUser.UserSecurity.AuthenticationError;		
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
