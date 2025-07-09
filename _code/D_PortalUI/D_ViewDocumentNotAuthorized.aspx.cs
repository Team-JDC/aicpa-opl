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
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.User;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_ViewDocumentNotAuthorized.
	/// </summary>
	public partial class D_ViewDocumentNotAuthorized : DestroyerUi
	{
		protected string requestedBookTitle = string.Empty;
		protected string requestedBookName = string.Empty;
		protected string requestedDocTitle = string.Empty;
		protected string requestedDocName = string.Empty;
		protected string previousBookName = string.Empty;
		protected string previousDocName = string.Empty;
		protected string UserGuid = string.Empty;
		protected string fasbGasbLibraryName = string.Empty;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			fasbGasbMsg.Visible = false;
			
			//get the last document and book that was successfully retrieved
			IDocument previousDoc = (IDocument)Session[SESSPARAM_CURRENTDOCUMENT];
			IBook previousBook = (previousDoc != null)?previousDoc.Book:null;

			//get the document and book referenced in the url
			IDocument requestedDoc = GetCurrentDocument(this.Page);
			IBook requestedBook = (requestedDoc != null)?requestedDoc.Book:null;

			//set the names for the previous document and previous book
			if(previousDoc != null && previousBook != null)
			{
				previousDocName = previousDoc.Name;
				previousBookName = previousBook.Name;
			}

            //set the names and titles for the requested document and book
			if(requestedDoc != null && requestedBook != null)
			{
				requestedDocTitle = requestedDoc.Title;
				requestedDocName = requestedDoc.Name;
				requestedBookTitle = requestedBook.Title;
				requestedBookName = requestedBook.Name;
			}

			//set the user guid and referring site
			IUser user = GetCurrentUser(this.Page);
			ReferringSite referringSite = user.ReferringSiteValue;
			UserGuid = user.UserId.ToString();

			//Hidding the instructions from Exam Users
			if(user.ReferringSiteValue == ReferringSite.Exams)
			{
				jsLabel.Visible = true;
				jsLabel.Text = "<script>document.getElementById('purchaseLinkMsg').style.visibility = 'hidden';document.getElementById('instructions').style.visibility = 'hidden';</script>";
			}

			//set up our link to the c2b catalog
			string c2bLink = string.Empty;
			//string port = (Request.Url.Port!=80)&&((Request.Url.Port!=443)?":"+Request.Url.Port:string.Empty;
			string destroyerRootUri = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + Request.ApplicationPath + "/";
			string tempUrl = Server.UrlEncode((previousDoc!=null)?destroyerRootUri + "D_Link.aspx?targetdoc=" + previousBookName + "&targetptr=" + previousDocName:destroyerRootUri);
			string fromUrl = tempUrl.Replace(".","%2E");
					
			tempUrl = Server.UrlEncode((requestedDoc!=null)?destroyerRootUri + "D_Link.aspx?targetdoc=" + requestedBookName + "&targetptr=" + requestedDocName:destroyerRootUri);
			string toUrl = tempUrl.Replace(".","%2E");
			purchaseLink.NavigateUrl = "#";
			purchaseLink.Attributes["onclick"] = "sendToCatalog('" + fromUrl + "', '" + toUrl + "', '" + requestedBookName + "', '" + referringSite.ToString() + "','" + UserGuid + "')";
			purchaseLink.Controls.Add(new System.Web.UI.LiteralControl(requestedBookTitle));

			if (requestedBookName.IndexOf(BOOK_PREFIX_FASB) != -1 || requestedBookName.IndexOf(BOOK_PREFIX_GASB) != -1)
			{
				//set the string used in the rebuff message to either FASB or GASB
				fasbGasbLibraryName = (requestedBookName.IndexOf(BOOK_PREFIX_FASB) != -1)?"FASB":"GASB";

				//show the fasb/gasb purchase message
				if(user.ReferringSiteValue != ReferringSite.Exams)
				{
					fasbGasbMsg.Visible = true;
				}
				//hide the regular purchase message
				purchaseRow1.Visible = false;
				purchaseRow2.Visible = false;
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
