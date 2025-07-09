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
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_DownloadDocument.
	/// </summary>
	public partial class D_DownloadDocument : DestroyerUi
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = GetCurrentSite(this.Page);

			//get the book name, doc name, and formats parameters
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			IBook book = (doc != null)?doc.Book:null;
			IDocumentFormat docFormat = DestroyerUi.GetCurrentDocumentFormat(this.Page);
            
			Response.AddHeader("Content-Disposition", "attachment; filename=" + doc.Name);
			Response.AddHeader("Content-Length", docFormat.ContentLength.ToString());
			Response.ContentType = docFormat.Description;
			Response.BinaryWrite(docFormat.Content);
			Response.Flush();
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
