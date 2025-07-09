using System;
using System.Xml;
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
	/// This page is used for accessing a specific document and format outside of the context of the portal.
	/// The page is useful for opening up content in an iframe.
	/// </summary>
	public partial class D_ViewDocument : DestroyerUi
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get the book name, doc name, and formats parameters
			IDocument doc = GetCurrentDocument(this.Page);
			IBook book = (doc != null)?doc.Book:null;
			IDocumentFormat format = doc.PrimaryFormat;
			bool hilite = Request.Params[REQPARAM_HITHIGHLIGHTS]==null?false:bool.Parse(Request.Params[REQPARAM_HITHIGHLIGHTS]);

			//make sure the document is in our subscription
			if(doc.InSubscription)
			{
				//see if we have a current search result object
				ISearchResults searchResults = (ISearchResults)Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];

				if(format != null)
				{
					// Serve up the file by name
					Response.AppendHeader("content-disposition","filename=" + doc.Name);
	            
					// set the content type for the Response to that of the 
					// document to display.  For example. "application/msword"
					Response.ContentType = format.Description;
					
					if (format.Description == "application/pdf")
					{
						Response.WriteFile(format.Uri);
						Response.End();
						return;
					}

					//set our bytes to be highlighted or not
					byte[] contentBytes = null;
					if(hilite && searchResults != null && format.ContentTypeId ==  (int)AICPA.Destroyer.Shared.ContentType.TextHtml && searchResults.WordInterpretations != null && searchResults.WordInterpretations.Length > 0)
					{
						contentBytes = format.GetHighlightedContent(searchResults.WordInterpretations, HILITE_BEGINTAG, HILITE_ENDTAG);
					}
					else
					{
						contentBytes = format.Content;
					}

					string contentText = DestroyerBpc.ByteArrayToStr(contentBytes);
					
					// If we're suppossed to show sources, add that stylesheet in the content
					if ((string)Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] != string.Empty)
					{
						contentText = D_PrintDocument.IncludeCodificationStyleSheets(contentText, true);
					}
					
					// If it's an old FASB or archived book, then add the "this document is not current" notice
					if (D_PrintDocument.IsDocumentNotCurrent(book.Name))
					{
						contentText = D_PrintDocument.IncludeDocumentNotCurrentNotice(contentText, true);
					}

					// Write to response stream and end the response
					Response.Write(contentText);
					Response.End();
				}
			}
			else
			{
				Response.Redirect("D_ViewDocumentNotAuthorized.aspx", true);
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
