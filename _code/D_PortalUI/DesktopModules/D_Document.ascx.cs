using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Xml;
using System.Reflection;
	
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;

namespace AICPA.Destroyer.UI.Portal.DesktopModules
{	
	/// <summary>
	///		The D_Document user control is used for displaying document content in the AICPA Destroyer environment.
	///		This user control uses session data to determine the current document to display, as well as to determine
	///		which, if any, terms to highlight if a query is active.
	/// </summary>
	public partial class D_Document : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{
		#region Protected Fields
		protected System.Web.UI.HtmlControls.HtmlTable DocTable;
		protected System.Web.UI.WebControls.ImageButton jsectionsToggler;
		
		#endregion Protected Fields

		#region Event Handlers
		/// <summary>
		/// Handles the loading of the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Add javascript to ASP.NET button onclick
			NonDocPrintImageButton.Attributes.Add("onclick", "javascript:printNonDoc(); return false;");
		}

		/// <summary>
		/// This is the last event that gets fired before content is displayed. We will always render the document in this
		/// method since we do not want to use viewstate (too much overhead invovled with this control for viewstate to
		/// be effective).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_PreRender(object sender, System.EventArgs e)
		{

			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			if (DocLiteral.Text.IndexOf("RefLink") < 0 && DocLiteral.Text.IndexOf("Archive") < 0 && DocLiteral.Text.IndexOf("Xref") < 0 && 
				DocLiteral.Text.IndexOf("JSection") < 0 && DocLiteral.Text.IndexOf("ViewDocument") < 0 && DocLiteral.Text.IndexOf("Goto") < 0)
			{
				//Session[DestroyerUi.SESSPARAM_QUERYCACHE] = string.Empty;
				RenderCurrentDoc(site);
				
			}
			
		
				this.setDisplayToc();
				
		}

		/// <summary>
		/// Handles the clicking of the next document image button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void NextDocImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//get the current site
			ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get the current doc
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);

			//get the next doc
			IDocument nextDoc = site.GetNextDocument(doc);

			//make sure there is a next doc
			if(nextDoc != null)
			{
				//WGD  6/26/2009
				if (nextDoc.Name.IndexOf("~") > -1) 
				{
					//Get both the book and the document from the docuemnt name
					string book = nextDoc.Name.Substring(nextDoc.Name.IndexOf("~")+ 1,nextDoc.Name.LastIndexOf("~") - nextDoc.Name.IndexOf("~") -1);
					string docname = nextDoc.Name.Substring(nextDoc.Name.LastIndexOf("~")+1);
					string url = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, book, DestroyerUi.REQPARAM_TARGETPTR, docname);
					Response.Redirect(url, true);

				}
				else
				{
					//redirect through D_Link
					string url = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, nextDoc.Book.Name, DestroyerUi.REQPARAM_TARGETPTR, nextDoc.Name);
					Response.Redirect(url, true);
				}
			}
		}

		/// <summary>
		/// Handles the clicking of the previous document image button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void PrevDocImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//get the current site
			ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get the current doc
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);

			//get the previous doc
			IDocument prevDoc = site.GetPreviousDocument(doc);

			//make sure there is a next doc
			if(prevDoc != null)
			{
				//redirect through D_Link
				string url = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, prevDoc.Book.Name, DestroyerUi.REQPARAM_TARGETPTR, prevDoc.Name);
				Response.Redirect(url, true);
			}
		}

		/// <summary>
		/// Handles the clicking of the help image button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void HelpImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			DestroyerUi.ShowHelp(this.Page, DestroyerUi.HelpTopic.Document);
		}

		#endregion Event Handlers

		#region Helper Methods

		/// <summary>
		/// Handles rendering of the document formats
		/// </summary>
		/// <param name="formats"></param>
		private void RenderDocFormats(IDocumentFormatCollection formats, IDocumentFormat currentDocumentFormat)
		{
			foreach(IDocumentFormat format in formats)
			{
				string currentContentTypeDesc = format.Description;
				if((currentContentTypeDesc != currentDocumentFormat.Description) && (currentContentTypeDesc != "text/wlh")&& (currentContentTypeDesc != "text/arch") )
				{
					//create the link
					HyperLink hyperLink = new HyperLink();
					hyperLink.ID = "dfhl_" + format.ContentTypeId;
					hyperLink.CssClass = "DocFormatLink";
                    hyperLink.Text = "download document in " + currentContentTypeDesc + " format";
                    hyperLink.NavigateUrl = "javascript:getFormat('" + format.ContentTypeId + "')";

					//create the image
					System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
					image.ID = "dfi_" + format.ContentTypeId;
					image.CssClass = "DocFormatImage";
					image.ImageUrl = "~/images/ct_" + format.ContentTypeId + ".gif";
                 	
					//add the image to the link
					hyperLink.Controls.Add(image);

					//add the link to the placeholder
					DocumentFormatPlaceHolder.Controls.Add(hyperLink);
				}
			}
		}

		/// <summary>
		/// Handles rendering of the document reference line
		/// </summary>
		/// <param name="siteReferenceXml"></param>
		private void RenderDocRef(string siteReferenceXml, bool onlyClearControls)
		{
			DocRefPlaceHolder.Controls.Clear();

			if (onlyClearControls)
			{
				return;
			}

			//get the xml string into an xml document
			XmlDocument siteReferenceXmlDoc = new XmlDocument();
			siteReferenceXmlDoc.LoadXml(siteReferenceXml);

			//grab all of the second-level nodes and go through them
			XmlNodeList nodes = siteReferenceXmlDoc.SelectNodes("/*/*");
			for(int i = 0; i < nodes.Count; i++)
			{
				//our current node
				XmlNode node = nodes[i];

				//get the attributes
				string nodeId = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_ID]);
				string nodeName = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_NAME]);
				string nodeTitle = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_TITLE]);
				NodeType nodeType = (NodeType)Enum.Parse(typeof(NodeType), node.LocalName, true);

				string nodeTitleShort = nodeTitle.Length > 21 ? nodeTitle.Substring(0,20) + " ..." : nodeTitle;
				
				HyperLink hyperLink = new HyperLink();
				hyperLink.ID = "rp_" + nodeId;
				hyperLink.CssClass = "DocRefLink";
				hyperLink.Text = nodeTitle;
				hyperLink.ToolTip = nodeTitle;
				if(nodeType == NodeType.Document || nodeType == NodeType.DocumentAnchor)
				{
					XmlNode bookNode = siteReferenceXmlDoc.SelectSingleNode("//" + DestroyerBpc.XML_ELE_BOOK);
					string bookName = DestroyerBpc.GetAttributeValue(bookNode.Attributes[DestroyerBpc.XML_ATT_BOOKNAME]);

					hyperLink.NavigateUrl = string.Format("~/D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, bookName, DestroyerUi.REQPARAM_TARGETPTR, nodeName);
				}
				else if(nodeType == NodeType.Book)
				{
					hyperLink.NavigateUrl = string.Format("~/D_Link.aspx?{0}={1}", DestroyerUi.REQPARAM_TARGETDOC, nodeName);
				}
				else if(nodeType == NodeType.SiteFolder)
				{
					hyperLink.NavigateUrl = string.Format("~/D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_LINKTYPE, NodeType.SiteFolder, DestroyerUi.REQPARAM_SITEFOLDERID, nodeId);
				}
				else
				{
					hyperLink.NavigateUrl = "javascript:syncTocToPath('" + DestroyerUi.GetTocSyncPathFromXml(siteReferenceXml, i+1) + "')";
				}
				DocRefPlaceHolder.Controls.Add(hyperLink);

				if(i != nodes.Count-1)
				{
					Label label = new Label();
					label.CssClass = "DocRefSepLabel";
					label.Text = "<img src='images/portal/header.gif'>";
					DocRefPlaceHolder.Controls.Add(label);
				}
			}
		}
		
		/// <summary>
		/// Gets the current document and renders it
		/// </summary>
		/// <param name="site"></param>
		private void RenderCurrentDoc(ISite site)
		{
			//first see if a site folder uri is being requested...
		
			string siteFolderId = Request.Params[DestroyerUi.REQPARAM_SITEFOLDERID];

			if(siteFolderId != null && siteFolderId != string.Empty)
			{
				//get the site folder object
				ISiteFolder siteFolder = new SiteFolder(site, int.Parse(siteFolderId));

				//show the uri if there is one
				if(siteFolder != null && siteFolder.Uri != null && siteFolder.Uri != string.Empty)
				{
					//render the document reference path
					RenderDocRef(siteFolder.SiteReferencePath, false);

					//set the iframe to show the site folder's url
					DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + siteFolder.Uri + "' width='100%' frameborder='0'></IFRAME>";
				}
				gotoToggler.Visible=false;
				linkToggler.Visible=false;
				archiveToggler.Visible=false;
				xrefToggler.Visible=false;
				jsectToggler.Visible=false;
			}
			else
			{
				//get the current doc
				IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);

				//don't bother going any further unless we have a valid document
				if(doc != null)
				{
					//get the document anchor from the url
					IDocumentAnchor docAnchor = DestroyerUi.GetCurrentDocumentAnchor(this.Page);

					//set the document format the the document's primary format
					IDocumentFormat docFormat = doc.PrimaryFormat;

					if(doc.InSubscription)
					{
						//populate ui elements with the document properties
						//add the copyright
						DocCopyLabel.Text = doc.Book.Copyright;

						//render the document reference path
						// Do this later on where we know which tab we're on
						//RenderDocRef(doc.SiteReferencePath);

						//render the document formats
						RenderDocFormats(doc.Formats, docFormat);

						//hide the "clear search" button if there is not an active search and if the hilite param is not present in the url
						bool hilite = Request.Params[DestroyerUi.REQPARAM_HITHIGHLIGHTS]==null?false:bool.Parse(Request.Params[DestroyerUi.REQPARAM_HITHIGHLIGHTS]);
						ISearchResults currentSearchResults = (ISearchResults)Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];
						ClearSearchImageButton.Visible = (hilite && currentSearchResults != null);

						//supress the next/prev if they are not applicable
						IDocument prevDoc = site.GetPreviousDocument(doc);
						IDocument nextDoc = site.GetNextDocument(doc);
						PrevDocImageButton.Visible = (prevDoc != null);
						NextDocImageButton.Visible = (nextDoc != null);
						PrevDocImageButton.Attributes.Add("onclick","javascript:docNavStartTimer();");
						NextDocImageButton.Attributes.Add("onclick","javascript:docNavStartTimer();");
						PrintImageButton.Visible = true;
						PrintImageButton.Attributes.Add("onclick","javascript:getPosition(event);");
						NonDocPrintImageButton.Visible = false;
						//Added 12/23/2008 WGD  archive and what links here folder
						//get the current Documents book 
						string archiveList = site.getArchiveList();
						string curbook = doc.Book.Name;
						if (archiveList.IndexOf(curbook) >= 0)
						{
							gotoToggler.Visible=true;
							archiveToggler.Visible = true;
							//Show the showSource if the session variable is empty
							if ((string)Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] == string.Empty)
							{
								showSource.Visible = true;
								hideSource.Visible = false;
							}
							else 
							{
								showSource.Visible = false;
								hideSource.Visible = true;
							}
						}
						else
						{
							archiveToggler.Visible = false;
						}
						string linkList = site.getReferenceLinkList();
						if (linkList.IndexOf(curbook) >= 0 )
						{
							linkToggler.Visible = true;
							IsCodificationDocument = true;
						}
						else
						{
							linkToggler.Visible = false;
							IsCodificationDocument = false;
						}
						string xrefList = site.getXRefList();
						if (xrefList.IndexOf(curbook) >= 0 )
						{
							xrefToggler.Visible = true;
						}
						else
						{
							xrefToggler.Visible = false;
						}
						string jsectList = site.getJSectionList();
						if (jsectList.IndexOf(curbook) >= 0 )
						{
							jsectToggler.Visible = true;
						}
						else
						{
							jsectToggler.Visible = false;
						}
						//End WGD

						//render the actual document
						//this is where we can determine how to display (inline, iframe, etc.) based on the content type of the current document.
						//for now, we will display all within an iframe using D_ViewDocument.aspx
						
						//WGD MKM
						//Here we want to implement the new parameters of targettab
						// We are going to check if parameters are the same as the last postback and ignore if that is the case.
						DestroyerUi.DocTab tabToSelect = DestroyerUi.GetDocumentTab(this.Page);
						DestroyerUi.DocCmd cmdToExecute = DestroyerUi.GetDocumentCommand(this.Page);

						if (procCustomTag(doc.Name, tabToSelect,cmdToExecute))
						{
							string queryCache = doc.Name + tabToSelect.ToString() + cmdToExecute.ToString();
							Session[DestroyerUi.SESSPARAM_QUERYCACHE] = queryCache;

							if ((tabToSelect == DestroyerUi.DocTab.Goto)&& (gotoToggler.Visible == true))
							{
								gotoToggler_Select(((int)cmdToExecute).ToString());
							}

							if ((tabToSelect == DestroyerUi.DocTab.WhatLinksHere)&& (linkToggler.Visible == true))
							{
								linkToggler_Select(((int)cmdToExecute).ToString());
							}
							else if ((tabToSelect == DestroyerUi.DocTab.Archive)&& (archiveToggler.Visible == true))
							{
								archiveToggler_Select(((int)cmdToExecute).ToString());
							}
							else if ((tabToSelect == DestroyerUi.DocTab.CrossRef)&& (xrefToggler.Visible == true))
							{
								xrefToggler_Select(((int)cmdToExecute).ToString());
							}
							else if ((tabToSelect == DestroyerUi.DocTab.JoinSections)&& (jsectToggler.Visible == true))
							{
								jsectToggler_Select(((int)cmdToExecute).ToString());
							}
							
						}	
						else
						{
							RenderDocRef(doc.SiteReferencePath, false);
							ContentType contentType = (ContentType)docFormat.ContentTypeId;
							switch(contentType)
							{
								default:
									//default is to write out the content into an iframe
									string anchorText = DestroyerUi.EMPTY_STRING;
									if(docAnchor != null)
									{
										anchorText = "#" + docAnchor.Name;
									}

									string bookList = string.Empty;
									foreach(string bookName in site.User.UserSecurity.BookName)
									{
										bookList += bookName + ";";
									}

									string url = string.Empty;
									if(hilite)
									{
										url = string.Format("D_ViewDocument.aspx?{0}={1}&{2}={3}&{4}={5}#{6}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_HITHIGHLIGHTS, hilite.ToString(), DestroyerUi.HILITE_ANCHORNAME);
									}
									else
									{
										url = string.Format("D_ViewDocument.aspx?{0}={1}&{2}={3}&{4}={5}{6}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTANCHORNAME, (docAnchor != null)?docAnchor.Name:string.Empty, anchorText);
									}

									//add a javascript call to getFormat for any document formats in this document that are set to AutoDownload
									string onloadExtra = string.Empty;
									foreach(DocumentFormat format in doc.Formats)
									{
										if(!format.IsPrimary && format.IsAutoDownload)
										{
											onloadExtra += "getFormat(\"" + format.ContentTypeId + "\");";
										}
									}

									//create the iframe
									//Make sure that the tabs are synced up
										if (linkToggler.Visible)
										{
											if (Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] == null ||(string)Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] == string.Empty )
											{
												showSource.Visible = true;
												hideSource.Visible = false;
											}
											else 
											{
												showSource.Visible = false;
												hideSource.Visible = true;
											}
											linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
										}
										if (archiveToggler.Visible)
										{
											archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
										}
										if (xrefToggler.Visible)
										{
											xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
										}
										if (jsectToggler.Visible)
										{
											jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
										}

										docToggler.ImageUrl = "../images/portal/document_active.jpg";


									DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url + "' width='100%' frameborder='0' onload='" + onloadExtra + "return docframe_onload(docframe, \"" + bookList + "\", \";\", \"" + DestroyerUi.ALTERNATEBOOK_SUFFIX + "\");' ></IFRAME>";


									break;
							}
						}

						//get rid of the current anchor on the session now that we have rendered the document; otherwise we will keep on jumping down to that anchor, which is annoying
						if(docAnchor != null)
						{
							Session.Remove(DestroyerUi.REQPARAM_CURRENTDOCUMENTANCHORNAME);
						}
					}
					else
					{
						string url = string.Format("D_ViewDocumentNotAuthorized.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTANCHORNAME, (docAnchor != null)?docAnchor.Name:string.Empty);
						DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url + "' width='100%' frameborder='0'></IFRAME>";
					}
				}
				else
				{
					string url = "D_ViewDocumentWelcome.aspx";
					DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url + "' width='100%' frameborder='0'></IFRAME>";
				}
			}
		}

		protected bool IsCodificationDocument;

		#endregion Helper Methods

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void printPrompt(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string x = this.Xvalue.Value;
			string y = this.Yvalue.Value;
			this.documentJSLabel.Text = "<script>printPrompt("+x+","+y+");</script>";
			this.documentJSLabel.Visible = true;
		}

		protected void ClearSearchImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = null;
		}

		private void setDisplayToc()
		{
			string setTocVisibility = Request.QueryString["toc"];
			string tocVisibility = string.Empty;

			if(setTocVisibility != null && setTocVisibility != "")
			{
				Session["TocVisibility"] = setTocVisibility;
			}
			else
			{
				if(Session["TocVisibility"] == null)
				{
					Session["TocVisibility"] = "true";
				}
				
			}
		}

		protected void docToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			RenderDocRef(doc.SiteReferencePath, false);
			
			PrintImageButton.Visible = true;
			NonDocPrintImageButton.Visible = false;
			documentJSLabel.Visible = false;
						
			if (gotoToggler.Visible)
			{
				gotoToggler.ImageUrl = "../images/portal/goto_selectable.jpg";
			}
			if (linkToggler.Visible)
			{
				if (Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] == null ||(string)Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] == string.Empty )
				{
					showSource.Visible = true;
					hideSource.Visible = false;
				}
				else 
				{
					showSource.Visible = false;
					hideSource.Visible = true;
				}
				linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
			}
			if (archiveToggler.Visible)
			{
				archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
			}
			if (xrefToggler.Visible)
			{
				xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
			}
			if (jsectToggler.Visible)
			{
				jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
			}

			docToggler.ImageUrl = "../images/portal/document_active.jpg";
			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			//RenderCurrentDoc(site);
			
			string url2 = string.Format("D_ViewDocument.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
	    }

		protected void gotoToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			gotoToggler_Select(string.Empty);
		}

		private void gotoToggler_Select(string cmdName)
		{
			
			showSource.Visible = false;
			hideSource.Visible = false;
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			
			PrintImageButton.Visible = false;
			NonDocPrintImageButton.Visible = false;
			documentJSLabel.Visible = false;

			docToggler.ImageUrl = "../images/portal/document_selectable.jpg";
				
			if (linkToggler.Visible)
			{
				linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
			}
			if (archiveToggler.Visible)
			{
				archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
			}
			if (xrefToggler.Visible)
			{
				xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
			}
			if (jsectToggler.Visible)
			{
				jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
			}

			gotoToggler.ImageUrl = "../images/portal/goto_active.jpg";
			
			string url2 = string.Format("D_Goto.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_TARGETCMD, cmdName);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
		}

		
		protected void linkToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			linkToggler_Select(string.Empty);
		}

		private void linkToggler_Select(string cmdName)
		{
			showSource.Visible = false;
			hideSource.Visible = false;
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			RenderDocRef(doc.SiteReferencePath, false);

			PrintImageButton.Visible = false;
			NonDocPrintImageButton.Visible = true;
			documentJSLabel.Visible = false;

			docToggler.ImageUrl = "../images/portal/document_selectable.jpg";
			if (gotoToggler.Visible)
			{
				gotoToggler.ImageUrl = "../images/portal/goto_selectable.jpg";
			}	
			if (archiveToggler.Visible)
			{
				archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
			}
			if (xrefToggler.Visible)
			{
				xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
			}
			if (jsectToggler.Visible)
			{
				jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
			}

			linkToggler.ImageUrl = "../images/portal/links_active.jpg";
			
			string url2 = string.Format("D_RefLink.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_TARGETCMD, cmdName);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
		}
	
		
		//handle Archive
		protected void archiveToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			archiveToggler_Select(string.Empty);

		}	

		private void archiveToggler_Select(string cmdName)
		{
			showSource.Visible = false;
			hideSource.Visible = false;
			PrintImageButton.Visible = false;
			NonDocPrintImageButton.Visible = true;
			documentJSLabel.Visible = false;
			
			docToggler.ImageUrl = "../images/portal/document_selectable.jpg";
			if (gotoToggler.Visible)
			{
				gotoToggler.ImageUrl = "../images/portal/goto_selectable.jpg";
			}		
			if (linkToggler.Visible)
			{
				linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
			}
			if (xrefToggler.Visible)
			{
				xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
			}
			if (jsectToggler.Visible)
			{
				jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
			}

			archiveToggler.ImageUrl = "../images/portal/archive_active.jpg";
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			RenderDocRef(doc.SiteReferencePath, false);
			
			string url2 = string.Format("D_Archive.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name,DestroyerUi.REQPARAM_TARGETCMD, cmdName);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
		}

		//Handle XRef
		protected void xrefToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			xrefToggler_Select(string.Empty);
		}
		private void xrefToggler_Select(string cmdName)
		{
			showSource.Visible = false;
			hideSource.Visible=false;
			PrintImageButton.Visible = false;
			NonDocPrintImageButton.Visible = true;
			documentJSLabel.Visible = false;

			docToggler.ImageUrl = "../images/portal/document_selectable.jpg";
			if (gotoToggler.Visible)
			{
				gotoToggler.ImageUrl = "../images/portal/goto_selectable.jpg";
			}	
			if (linkToggler.Visible)
			{
				linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
			}
			if (archiveToggler.Visible)
			{
				archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
			}
			if (jsectToggler.Visible)
			{
				jsectToggler.ImageUrl = "../images/portal/jsections_selectable.jpg";
			}

			xrefToggler.ImageUrl = "../images/portal/xref_active.jpg";
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			RenderDocRef(doc.SiteReferencePath, true);
			
			string url2 = string.Format("D_Xref.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name,DestroyerUi.REQPARAM_TARGETCMD, cmdName);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
		}

		//Handle Join Sections
		protected void jsectToggler_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			jsectToggler_Select(string.Empty);
		}

		private void jsectToggler_Select(string cmdName)
		{
			showSource.Visible = false;
			hideSource.Visible = false;
			PrintImageButton.Visible = false;
			NonDocPrintImageButton.Visible = true;
			documentJSLabel.Visible = false;

			docToggler.ImageUrl = "../images/portal/document_selectable.jpg";
			if (gotoToggler.Visible)
			{
				gotoToggler.ImageUrl = "../images/portal/goto_selectable.jpg";
			}		
			if (linkToggler.Visible)
			{
				linkToggler.ImageUrl = "../images/portal/links_selectable.jpg";
			}
			if (archiveToggler.Visible)
			{
				archiveToggler.ImageUrl = "../images/portal/archive_selectable.jpg";
			}
			if (xrefToggler.Visible)
			{
				xrefToggler.ImageUrl = "../images/portal/xref_selectable.jpg";
			}

			jsectToggler.ImageUrl = "../images/portal/jsections_active.jpg";
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			RenderDocRef(doc.SiteReferencePath, true);
				
			string url2 = string.Format("D_JSections.aspx?{0}={1}&{2}={3}&{4}={5}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name, DestroyerUi.REQPARAM_TARGETCMD, cmdName);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
		}

		private bool procCustomTag(string document, DestroyerUi.DocTab tab, DestroyerUi.DocCmd cmd)
		{
			//Lets create a session variable
			if(tab == DestroyerUi.DocTab.Document)
			{
				return false;
			}
			string sessionQueryCache = (string)Session[DestroyerUi.SESSPARAM_QUERYCACHE];
			if((sessionQueryCache ==null) || (sessionQueryCache == string.Empty))
			{
				return true;
			}
			string queryCache = document + tab.ToString() + cmd.ToString();
			if (	sessionQueryCache != queryCache)
				return true;
			else
				return false;
				
		}

		protected void showSource_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
			Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] = "showSource";
			showSource.Visible = false;
			hideSource.Visible = true;
			
			docToggler.ImageUrl = "../images/portal/document_active.jpg";
			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			//RenderCurrentDoc(site);
			string url2 = string.Format("D_ViewDocument.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
			
			return;
		
		}

		protected void hideSource_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			
			Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] = string.Empty;
			
			IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);


			showSource.Visible = true;
			hideSource.Visible = false;

			docToggler.ImageUrl = "../images/portal/document_active.jpg";
			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			//RenderCurrentDoc(site);
			string url2 = string.Format("D_ViewDocument.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_CURRENTBOOKNAME, doc.Book.Name, DestroyerUi.REQPARAM_CURRENTDOCUMENTNAME, doc.Name);
			DocLiteral.Text = "<IFRAME id='docframe' style='height:420' name='docframe' src='" + url2 + "' width='100%' frameborder='0'></IFRAME>";
			return;

		}				

				
	}
}
