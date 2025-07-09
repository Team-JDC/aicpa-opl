using System;
using System.Xml;
using System.Text.RegularExpressions;
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
using AICPA.Destroyer.Content.Subscription;
using AICPA.Destroyer.User.Event;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// The D_Link page handles all linking between documents.
	/// This page accepts targetdoc and targetptr parameters and redirects to the appropriate
	/// portal page, with d_bn (bookname), d_dn (documentname), and d_an (documentanchorname) split out
	/// </summary>
	public partial class D_Link : DestroyerUi
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site (which is already filtered according to user subscription)
			AICPA.Destroyer.Content.Site.ISite site = GetCurrentSite(this.Page);

			//get the link type from the request, default to Document if no link type specified
			string linkTypeString = Request.Params[DestroyerUi.REQPARAM_LINKTYPE];
			NodeType linkType = (NodeType)(linkTypeString !=null && linkTypeString != string.Empty?Enum.Parse(typeof(NodeType), linkTypeString, true):NodeType.Document);

			//get the link referrer
			LinkReferrer linkReferrer = (LinkReferrer)(Request.Params[DestroyerUi.REQPARAM_LINKREFERRER]==null?LinkReferrer.Unknown:Enum.Parse(typeof(LinkReferrer), Request.Params[DestroyerUi.REQPARAM_LINKREFERRER], true));
			Session[DestroyerUi.SESSPARAM_QUERYCACHE] = string.Empty;
			Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] = string.Empty;

			switch(linkType)
			{
				case NodeType.Document:
					DoDocumentLink(site, linkReferrer);
					break;
				case NodeType.SiteFolder:
					DoSiteFolderLink(site, linkReferrer);
					break;
				case NodeType.BookFolder:
					DoBookFolderLink(site, linkReferrer);
					break;
				default:
					throw new Exception(string.Format("Link type '{0}' is not supported.", linkType.ToString()));
			}
		}

		/// <summary>
		/// Handles site folder links
		/// </summary>
		/// <param name="site"></param>
		/// <param name="linkReferrer"></param>
		private void DoSiteFolderLink(Content.Site.ISite site, LinkReferrer linkReferrer)
		{
			//get the site folder id
            int siteFolderId = int.Parse(Request[DestroyerUi.REQPARAM_SITEFOLDERID]);

			//get the site folder object
			ISiteFolder siteFolder = new SiteFolder(site, siteFolderId);

			//redirect to the site folder
			if(siteFolder != null && siteFolder.Uri != string.Empty)
			{
				//set the toc sync path to the folder
				Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = DestroyerUi.GetTocPath(siteFolder);

				//redirect to a page that will display the site folder
				string queryParams = string.Format("&{0}={1}", DestroyerUi.REQPARAM_SITEFOLDERID, siteFolderId);
				DestroyerUi.ShowTab(this.Page, this.displayToc(), queryParams);
			}
			else
			{
				//find the first child of the folder by using the toc xml
				string siteFolderXml = site.GetTocXml(siteFolderId, NodeType.SiteFolder);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(siteFolderXml);
				XmlNode folderChildNode = xmlDoc.SelectSingleNode("*/*");
				string nodeId = DestroyerBpc.GetAttributeValue(folderChildNode.Attributes[DestroyerBpc.XML_ATT_ID]);
				string nodeName = DestroyerBpc.GetAttributeValue(folderChildNode.Attributes[DestroyerBpc.XML_ATT_NAME]);

				NodeType folderChildNodeType = (NodeType)Enum.Parse(typeof(NodeType), folderChildNode.LocalName, true);
				switch(folderChildNodeType)
				{
					case NodeType.Book:
						//redirect back trhough us as a book
						string url = string.Format("~/D_Link.aspx?{0}={1}", DestroyerUi.REQPARAM_TARGETDOC, nodeName);
						Response.Redirect(url, true);
						break;
					case NodeType.SiteFolder:
						//redirect to a page that will display the site folder
						string queryParams = string.Format("&{0}={1}", DestroyerUi.REQPARAM_SITEFOLDERID, nodeId);
						DestroyerUi.ShowTab(this.Page, this.displayToc(), queryParams);
						break;
				}
			}
		}

		/// <summary>
		/// Handles book folder links
		/// </summary>
		/// <param name="site"></param>
		/// <param name="linkReferrer"></param>
		private void DoBookFolderLink(Content.Site.ISite site, LinkReferrer linkReferrer)
		{
			//get the book folder id
			int bookFolderId = int.Parse(Request[DestroyerUi.REQPARAM_BOOKFOLDERID]);

			//find the first child of the folder by using the toc xml
			string bookFolderXml = site.GetTocXml(bookFolderId, NodeType.BookFolder);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(bookFolderXml);
			XmlNode folderChildNode = xmlDoc.SelectSingleNode("*/*");
			string nodeId = DestroyerBpc.GetAttributeValue(folderChildNode.Attributes[DestroyerBpc.XML_ATT_ID]);
			string nodeName = DestroyerBpc.GetAttributeValue(folderChildNode.Attributes[DestroyerBpc.XML_ATT_NAME]);

			NodeType folderChildNodeType = (NodeType)Enum.Parse(typeof(NodeType), folderChildNode.LocalName, true);
			switch(folderChildNodeType)
			{
				case NodeType.Document:
					//redirect back through us as a document
					IDocument doc = new AICPA.Destroyer.Content.Document.Document(int.Parse(nodeId));
					string bookName = doc.Book!=null?doc.Book.Name:string.Empty;
					if(bookName != string.Empty)
					{
						string docUrl = string.Format("~/D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, bookName, DestroyerUi.REQPARAM_TARGETPTR, nodeName);
						Response.Redirect(docUrl, true);
					}
					break;
				case NodeType.BookFolder:
					//redirect back through us as a child book folder
					string bookFolderUrl = string.Format("~/D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_LINKTYPE, NodeType.BookFolder, DestroyerUi.REQPARAM_BOOKFOLDERID, nodeId);
					Response.Redirect(bookFolderUrl, true);
					break;
			}
		}

		private string tranlateToBookDomain(Content.Site.ISite site)
		{
			IBookCollection books = site.Books;
			string bookDomain = string.Empty;
			foreach(IBook book in books)
			{
				bookDomain += book.Name + ",";
			}

			bookDomain = bookDomain.Substring(0,bookDomain.Length-1);
			return bookDomain;
		}
		/// <summary>
		/// Handles document links
		/// </summary>
		/// <param name="site"></param>
		/// <param name="linkReferrer"></param>
		private void DoDocumentLink(Content.Site.ISite site, LinkReferrer linkReferrer)
		{
			//get the targetdoc and targetptr from the request
			string targetDoc = Request.Params[DestroyerUi.REQPARAM_TARGETDOC];
			string targetPtr = Request.Params[DestroyerUi.REQPARAM_TARGETPTR];

			//try to get the bookname, document name, and anchor name represented by the targetdoc and targetptr
			string bookName = string.Empty;
			string docName = string.Empty;
			string docAnchorName = string.Empty;
			bool bookInSubscription = true;



			IBook book = null;
			IDocument doc = null;
			IDocumentAnchor docAnchor = null;

			//first try getting the book using the targetdoc
			book = site.Books[targetDoc];

			string testthis = (string)Session[SESSPARAM_CURRENTBOOK];

			IDocument curDoc = GetCurrentDocument(this.Page);
			
			//if not found, try to get a fallback alternate book
			if(book == null)
			{
				//*************************************/
				// Code added by David Morales
				// This code will retrieve the alternate book in case is needed
				// Commented code for now//
				string domain = this.tranlateToBookDomain(site);
				string alternameBookName = site.AlternateBook(targetPtr,targetDoc,domain);
				
				// Only look for an alternate for ps if the current book is ara-ras						
				//if ((targetDoc != "ps") || ((targetDoc == "ps") && (curDoc != null) && (curDoc.Book != null) && (curDoc.Book.Name =="ara-ras")))
				//{		
				//	string alternameBookName = DestroyerUi.GetAlternateBookName(site.Books, targetDoc);

				if(alternameBookName!=null && alternameBookName!=string.Empty)
				{
					targetDoc = alternameBookName;
					book = site.Books[targetDoc];
					//If the alternate book is aam we may need to change the targetptr
					/*aam_8010 = ara-gen
					aam_8015 = ara-cra
					aam_8030 = ara-hco
					aam_8040 = ara-ins
					aam_8050 = ara-dep
					aam_8060 = ara-ebp
					aam_8070 = ara-slg
					aam_8080 = ara-cir
					aam_8090 = ara-con
					aam_8100 = ara-inv
					aam_8110 = ara-brd
					aam_8120 = ara-npo
					aam_8130 = ara-ht
					aam_8140 = ara-rle
					aam_8160 = ara-aut
					aam_8220 = ara-sga
					aam_8230 = ara-mfg
					aam_8240 = ara-iet
					aam_8250 = ara-int*/
					string alternatePtr = string.Empty;
					if(targetDoc.StartsWith("aam"))
					{
						switch(targetPtr)
						{
							case "ara-gen":
								alternatePtr = "aam_8010";
								break;
							case "ara-cra":
								alternatePtr = "aam_8015";
								break;
							case "ara-hco":
								alternatePtr = "aam_8030";
								break;
							case "ara-ins":
								alternatePtr = "aam_8040";
								break;
							case "ara-dep":
								alternatePtr = "aam_8050";
								break;
							case "ara-ebp":
								alternatePtr = "aam_8060";
								break;
							case "ara-slg":
								alternatePtr = "aam_8070";
								break;
							case "ara-cir":
								alternatePtr = "aam_8080";
								break;
							case "ara-con":
								alternatePtr = "aam_8090";
								break;	
							case "ara-inv":
								alternatePtr = "aam_8100";
								break;	
							case "ara-brd":
								alternatePtr = "aam_8110";
								break;	
							case "ara-npo":
								alternatePtr = "aam_8120";
								break;	
							case "ara-ht":
								alternatePtr = "aam_8130";
								break;	
							case "ara-rle":
								alternatePtr = "aam_8140";
								break;
							case "ara-aut":
								alternatePtr = "aam_8160";
								break;	
							case "ara-sga":
								alternatePtr = "aam_8220";
								break;
							case "ara-mfg":
								alternatePtr = "aam_8230";
								break;
							case "ara-iet":
								alternatePtr = "aam_8240";
								break;
							case "ara-int":
								alternatePtr = "aam_8250";
								break;
							default:
								break;
						}
						if (alternatePtr != string.Empty)
						{
							targetPtr = alternatePtr;
						}
					}
				}
		//	}
			}
			if(book != null)
			{
				//we found the book, so store its name
				bookName = targetDoc;

				//now try getting the doc using the targetptr
				if(targetPtr != null)
				{
					doc = book.Documents[targetPtr];
					docName = targetPtr;
				}
				else
				{
					doc = book.Documents[0];
					docName = doc.Name;

				}

				if(doc == null)
				{
					//we failed to get the doc using the targetptr, so lets
					//try to get a doc that contains an anchor that matches
					//targetptr
					doc = book.GetDocumentByAnchorName(targetPtr);
					
					//try fallback link logic if we still don't have a document and we are dealing with a link to fasb or gasb content
					if(doc == null && (bookName.IndexOf(BOOK_PREFIX_FASB) != -1 || bookName.IndexOf(BOOK_PREFIX_GASB) != -1))
					{
						string fallbackTargetPtr = string.Empty;

						//if the targetptr ends with numbers within parens...
						if(Regex.Match(targetPtr, "\\([0-9]+\\)$").Success)
						{
							//strip everything from the last left paren to the end of the string
							targetPtr = targetPtr.Substring(0, targetPtr.LastIndexOf("(")-1);
							fallbackTargetPtr = targetPtr;
						}

						//if the targetptr now ends with one or two alpha characters, preceded by a number character...
						if(Regex.Match(targetPtr, "[0-9][A-Za-z][A-Za-z]?$").Success)
						{
							//strip the trailing letter characters and try the link again
							fallbackTargetPtr = Regex.Match(targetPtr, "^(.*[0-9])([A-Za-z][A-Za-z]?)$").Groups[1].Value;
						}
						else //if the targetptr now ends with a number...
							if(Regex.Match(targetPtr, "[0-9]$").Success)
						{
							//add an 'a' and try the link again
							fallbackTargetPtr = targetPtr + "a";
						}
						
						if(fallbackTargetPtr != string.Empty)
						{
							targetPtr = fallbackTargetPtr;
							doc = book.GetDocumentByAnchorName(targetPtr);
						}
					}

					//we have successfully found a document
					if(doc != null)
					{
						//we found the doc, so store its name
						docName = doc.Name;

						//we also determined that targetptr is the name of an anchor, so get the anchor and store its name
						docAnchor = doc.GetDocumentAnchor(targetPtr);
						docAnchorName = docAnchor.Name;
					}
				}
			}

			//if we did not find the book in the user's site, make sure we set a flag before we continue
			if(bookName == string.Empty)
			{
				bookInSubscription = false;
			}

			//if we could not get the book through normal means, the document either does not exist or is not in any of the user's subscriptions
			if(!bookInSubscription)
			{
				//...so we want to grab a new version of our current site that does not filter by user access
				AICPA.Destroyer.Content.Site.ISite allBookSite = new Site(site.Id);

				//...and see if this site has a book by the name of the targetdoc
				book = allBookSite.Books[targetDoc];
				if(book != null)
				{
					//...it does, so store the bookname
					bookName = targetDoc;

					//see if the targetptr exists in the book
					doc = book.Documents[targetPtr];
					if(doc != null)
					{
						//we found the doc, so store its name
						docName = targetPtr;
					}
					else
					{
						//we failed to get the doc using the targetptr, so lets
						//try to get a doc that contains an anchor that matches
						//targetptr
						doc = book.GetDocumentByAnchorName(targetPtr);
						if(doc != null)
						{
							//we found the doc, so store its name
							docName = doc.Name;

							//we also determined that targetptr is the name of an anchor, so store its name
							docAnchor = doc.GetDocumentAnchor(targetPtr);
							docAnchorName = docAnchor.Name;
						}
					}
				}
			}

			if(bookName == string.Empty || docName == string.Empty)
			{
				throw new Exception(string.Format(ERROR_DOCUMENTNOTFOUND, targetDoc, targetPtr));
			}

			//use the session to flag the toc for sync
			if(bookInSubscription && doc != null)
			{
				Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = (docAnchor != null)?Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = DestroyerUi.GetTocPath(docAnchor):Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = DestroyerUi.GetTocPath(doc);
			}

			string docTabId = Request[DestroyerUi.REQPARAM_TARGETTAB];
			string docCmd = Request[DestroyerUi.REQPARAM_TARGETCMD];

			//okay, so now we construct our query params and redirect
			string queryParams = string.Format("&{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}",
				REQPARAM_CURRENTBOOKNAME, bookName, REQPARAM_CURRENTDOCUMENTNAME, docName,
				REQPARAM_CURRENTDOCUMENTANCHORNAME, docAnchorName,
				REQPARAM_TARGETTAB, docTabId, REQPARAM_TARGETCMD, docCmd);
			if(linkReferrer == LinkReferrer.SearchResults)
			{
				queryParams += string.Format("&{0}={1}", REQPARAM_HITHIGHLIGHTS, bool.TrueString);
			}
			
			//DestroyerUi.ShowTab(this.Page, DestroyerUi.PortalTab.TocDoc, queryParams);
			DestroyerUi.ShowTab(this.Page, this.displayToc(), queryParams);
		}

		private DestroyerUi.PortalTab displayToc()
		{
			if(Session["TocVisibility"] != null)
			{
				if(Session["TocVisibility"].ToString() == "true")
				{
					return DestroyerUi.PortalTab.TocDoc;
				}
				else
				{
					return DestroyerUi.PortalTab.Document;
				}
			}
			else
			{
				return DestroyerUi.PortalTab.TocDoc;
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
