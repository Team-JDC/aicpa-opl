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

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for storeRedirect.
	/// </summary>
	public partial class D_Copyright : DestroyerUi
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//logic to determine which copyrights to display

			IUser user = DestroyerUi.GetCurrentUser(this.Page);

			if(ContainsBookWithPrefix(user.UserSecurity.BookName, BOOK_PREFIX_FAF))
			{
				TrFasbCopyright.Visible = true;
			}

			if(ContainsBookWithPrefix(user.UserSecurity.BookName, BOOK_PREFIX_GASB))
			{
				TrGasbCopyright.Visible = true;
			}
			
			if(user.ReferringSiteValue == ReferringSite.Exams)
			{
				TrFasbCopyright.Visible = true;
			}

		}

		private bool ContainsBookWithPrefix(string[] bookNameList, string bookPrefix)
		{
			bool foundIt = false;
			foreach(string bookName in bookNameList)
			{
				if(bookName.StartsWith(bookPrefix))
				{
					foundIt = true;
					break;
				}
			}
			return foundIt;
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
