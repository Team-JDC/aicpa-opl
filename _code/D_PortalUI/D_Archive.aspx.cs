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
using AICPA.Destroyer.Content.Search;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_Archive.
	/// </summary>
	public partial class D_Archive : DestroyerUi
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get the book name, doc name, and formats parameters
			IDocument doc = GetCurrentDocument(this.Page);
			IBook book = (doc != null)?doc.Book:null;
			bool archiveExist = false;

			// Serve up the file by name
			Response.AppendHeader("content-disposition","filename=" + doc.Name);
	            
			// set the content type for the Response to that of the 
			// document to display.  For example. "application/msword"
			Response.ContentType = "text/html";

			//set our bytes to be highlighted or not
			byte[] contentBytes = null;
			
			
			
			foreach(DocumentFormat format in doc.Formats)
			{
				if (format.Description == "text/arch")
				{
					contentBytes = format.Content;
					archiveExist = true;
				}
			}
			if (!archiveExist)
			{
				System.Text.ASCIIEncoding  encoding=new System.Text.ASCIIEncoding();
				string message = "<html><head><link rel=\"stylesheet\" href=\"cssFiles/arch.css\" type=\"text/css\"></head><body class=\"noStrikethrough\"><h1>Archive</h1><p>The Archive page provides links to current and all superseded Subsections that comprise the current Section. The date that precedes each Subsection is both the date it was issued and the date on which its previous version was archived (i.e., superseded).</p></body></html>";
				contentBytes = encoding.GetBytes(message);

			}

			// output the actual document contents to the response output stream
			Response.OutputStream.Write(contentBytes, 0, contentBytes.Length);
			// end the response
			Response.End();
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
