using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_Resource.
	/// </summary>
	public partial class D_Resource : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get some config values
			//string bookContentFolder = System.Configuration.ConfigurationSettings.AppSettings[Book.BOOK_CONTENTFOLDER_KEY];
			//string bookResourceFolder = System.Configuration.ConfigurationSettings.AppSettings[Book.BOOK_RESOURCEFOLDER_KEY];
            //modified 1/20/10 djf
            string bookContentFolder = ConfigurationManager.AppSettings[Book.BOOK_CONTENTFOLDER_KEY];
            string bookResourceFolder = ConfigurationManager.AppSettings[Book.BOOK_RESOURCEFOLDER_KEY];
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get some request values
			string resourceBookName = Request["r_bn"];
			string resourceName = Request["r_rn"];

			//get the book mentioned in the request
			IBook book = site.Books[resourceBookName];

			if(book == null)
			{
				string alternateBook = DestroyerUi.GetAlternateBookName(site.Books, resourceBookName);
				if(alternateBook != null && alternateBook != string.Empty)
				{
                    resourceBookName = alternateBook;
					book = site.Books[resourceBookName];
				}
			}

			if(book == null)
			{
				throw new Exception(string.Format(DestroyerUi.ERROR_RESOURCEBOOKACCESSDENIED, resourceBookName, resourceName));
			}

			//throw an error if there are path characters in the resource name (prevent payload attacks)
			if(resourceName.IndexOf(Path.DirectorySeparatorChar) >=0 || resourceName.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
			{
				throw new Exception(string.Format(DestroyerUi.ERROR_RESOURCENAMEINVALID, resourceBookName, resourceName));
			}

			//construct a path to our resources and throw an error if the directory was not found
			string resourceDirectory = Path.Combine(Path.Combine(bookContentFolder, book.Name + "." + book.Version), bookResourceFolder);
			if(!Directory.Exists(resourceDirectory))
			{
				throw new DirectoryNotFoundException(string.Format(DestroyerUi.ERROR_RESOURCEFOLDERNOTFOUND, resourceBookName, resourceName));
			}

			//create our full path to the resource
			string resourcePath = Path.Combine(resourceDirectory, resourceName);

			if(!File.Exists(resourcePath))
			{
				throw new FileNotFoundException(string.Format(DestroyerUi.ERROR_RESOURCENOTFOUND, resourceBookName, resourceName));
			}

			// Serve up the file
			string mimeType = AICPA.Destroyer.UI.Portal.Shared.MimeTypeUtil.CheckType(resourcePath);
			Response.ContentType = mimeType;
			Response.WriteFile(resourcePath);
			
			//force a download prompt for all non-html content, making sure that the resource
			//filename is passed in the header to allow the download prompt to display the correct
			//filename
			if(mimeType != "text/html")
			{
				Response.AddHeader("Content-Disposition", "attachment; filename=" + resourceName);
			}

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
