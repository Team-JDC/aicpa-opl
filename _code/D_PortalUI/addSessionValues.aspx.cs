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

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for addSessionValues.
	/// </summary>
	public partial class addSessionValues : System.Web.UI.Page
	{
		
		System.Web.UI.WebControls.Label ajaxValue = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			string unSubs = Request.QueryString["unSubs"];

			if(unSubs != null && unSubs != "")
			{
				Session["SeeUnsubscribed"] = unSubs == "true" ? true : false;
			}
			else
			{
				Session["SeeUnsubscribed"] = false;
			}

			ajaxValue.Text = Session["SeeUnsubscribed"].ToString();
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
