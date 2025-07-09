using System;
using System.Collections;
using System.Text.RegularExpressions;
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
using AICPA.Destroyer.Content.Subscription;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// The D_SiteCrawl page is for C2B indexing integration. We return back the specified document with metadata added to the head tag.
	/// </summary>
	public partial class D_SiteCrawl : DestroyerUi
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//determine the desired site status (default to production if staging is not set on the url)
			string isStagingString = Request[REQPARAM_INDEXINGSTAGING].ToLower();
			bool isStaging = isStagingString == "1" || isStagingString == "true";
			SiteStatus siteStatus = isStaging?SiteStatus.Staging:SiteStatus.Production;
            
			//get the site
			AICPA.Destroyer.Content.Site.ISite site = new Site(siteStatus);

			//see if we can get the requested book and document
			string bookName = Request[REQPARAM_TARGETDOC];
			string docName = Request[REQPARAM_TARGETPTR];
			IBook book = site.Books[bookName];
			if(book != null)
			{
				IDocument doc = book.Documents[docName];
				if(doc != null)
				{
					string title = doc.Title;
					byte[] docBytes = doc.PrimaryFormat.Content;
					string content = DestroyerBpc.ByteArrayToStr(docBytes);
					content = AddC2bMetadata(doc, title, content);
					Response.Write(content);
					Response.Flush();
					Response.Close();
				}
				else
				{
					throw new Exception(string.Format("The requested document was not found on the site (bookName='{0}'; docName='{1}')", bookName, docName));
				}
			}
			else
			{
				throw new Exception(string.Format("The requested book was not found on the site (bookName='{0}')", bookName));
			}
		}

		private string AddC2bMetadata(IDocument doc,string title,string content)
		{
			//the resulting string
			string retContent = content;

			//prepare the metadata
			string c2bMeta = string.Empty;
			string descriptionMeta = GetC2bDescriptionMeta(doc, content, 500);
			string categoryMeta = GetC2bCategoryMeta(doc, content);
			c2bMeta = string.Format("\n<title>{0}</title>\n<meta name='Description' content='{1}'>\n<meta name='Category' content='{2}'>\n<meta name='Vendor' content=''>\n<meta name='Author' content=''>\n", title, descriptionMeta, categoryMeta);	

			//strip out all script elements that might be in the content
			retContent = Regex.Replace(retContent, "<script[^>]*>.*</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//add the meta tags
			retContent = Regex.Replace(retContent, "(<head[^>]*>)",  "$1" + c2bMeta, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
			
			//return the content
			return retContent;
		}

		private string GetC2bDescriptionMeta(IDocument doc, string content, int length)
		{
			//the resulting string
			string strippedContent = content;

			//first strip out the head
			strippedContent = Regex.Replace(strippedContent, "<head[^>]*>.*</head>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
			
			//now strip out all script elements that might be in the body
			strippedContent = Regex.Replace(strippedContent, "<script[^>]*>.*</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//now strip out all markup
			strippedContent = Regex.Replace(strippedContent, "</?[^>]*>", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//now change all whitespace characters to a space
			strippedContent = Regex.Replace(strippedContent, "[\r\n\t]", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//replace quotes with entity references
			strippedContent = Regex.Replace(strippedContent, "\"", "&quot;", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//now remove all redundant spaces
			strippedContent = Regex.Replace(strippedContent, " +", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			//now remove all spaces at the beginning of the string
			strippedContent = Regex.Replace(strippedContent, "^ +", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

            //now truncate
			strippedContent = strippedContent.Substring(0, (length>strippedContent.Length)?strippedContent.Length:length);

			//now remove the last word of the string to avoid partial words
			strippedContent = strippedContent.Substring(0, strippedContent.LastIndexOf(" "));
            
			//return the finished string
			return strippedContent;
		}

		private string GetC2bCategoryMeta(IDocument doc, string content)
		{
			//the string to return
			string retCat = string.Empty;

            //decide how to categorize based on the book name
			string bookName = doc.Book.Name;
			if(bookName.IndexOf("ara") == 0 ||bookName.IndexOf("chk") == 0 || bookName.IndexOf("aag") == 0 || bookName == ("tpa") || bookName == ("ps") || bookName == ("ps-pcaob") || bookName == ("aam") || bookName == ("att"))
			{
				retCat = "AICPA reSOURCE";
			} 
			else if (bookName.IndexOf("emap") == 0)
			{
				retCat = "EMAP";
			}
			else if(bookName.IndexOf("fasb") == 0)
			{
				retCat = "FASB";
			}

			//return our categorization
			return retCat;
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
