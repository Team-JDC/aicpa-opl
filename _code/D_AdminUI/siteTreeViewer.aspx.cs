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
using System.Xml;
using Telerik.WebControls;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

namespace D_AdminUI
{
	/// <summary>
	/// Summary description for siteTreeViewer.
	/// </summary>
	public partial class siteTreeViewer : System.Web.UI.Page
	{

		int globalSiteId;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			treeType = Convert.ToInt16(Request.QueryString["treeType"]);

			if(!Page.IsPostBack)
			{
				globalSiteId = Convert.ToInt16(Request.QueryString["Id"]);
				if(treeType == 1)
				{
					this.getSiteTree(globalSiteId);
				}
				else
				{
					this.getBookTree(globalSiteId);
				}
				
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
			this.TreeView.NodeDrop += new Telerik.WebControls.RadTreeView.RadTreeViewEventHandler(this.TreeView_NodeDragDrop);
			this.TreeView.NodeContextClick += new Telerik.WebControls.RadTreeView.RadTreeViewEventHandler(this.TreeView_ContextClick);

		}
		#endregion

		#region Properties
		//private SiteCollection activeSiteCollection = null;

		private int treeType;
		private string removedBooks;

		#region constants
		private const string CONTENTMENU_ADD_FOLDER = "Add Folder";
		private const string CONTENTMENU_ADD_BOOK = "Add Book";
		private const string CONTENTMENU_RENAME_FOLDER = "Rename Folder";
		private const string CONTENTMENU_REMOVE_FOLDER = "Remove Folder";
		private const string CONTENTMENU_REMOVE_BOOK = "Remove Book";
		private const string CONTENTMENU_REMOVE_BOOK_FROM_SITE = "Remove Book from Site";
		private const string CONTENTMENU_MOVE_UP = "Move Up";
		private const string CONTENTMENU_MOVE_DN = "Move Down";
		private const string CONTENTMENU_REPLACE_BOOK = "Replace Book";
		private const string ROOT_MENU = "RootMenu";
		private const string FOLDER_MENU = "FolderMenu";
		private const string BOOK_MENU = "BookMenu";
		private const string SITE = "Site";
		private const string SITE_FOLDER = "SiteFolder";
		private const string BOOK = "Book";
		private const string DOCUMENT = "Document";

		#endregion constants
	
		#endregion Properties

		#region BuildTreeXML

		private string buildTreeXML(XmlNode elementNode, 
			string img, 
			string contentMenu,
			string nodeId,
			bool editFlag,
			string title,
			string nodeLongDescArg)
		{
			string nodeLongDesc = string.Empty;
			string childStr = string.Empty;
			string nodeDataId = string.Empty;
			string elementLongDesc = nodeLongDescArg;//elementNode.Attributes.GetNamedItem("Name").Value;
			elementLongDesc = elementLongDesc.Replace("'","`");
			elementLongDesc = elementLongDesc.Replace("&","&amp;");
			title  = title.Replace("'","`");
			title = title.Replace("&","&amp;");

			string parentStr = string.Format("<Node LongDesc=\"{0}\" Image=\"{1}\" Expanded=\"True\" ContextMenuName=\"{2}\" ID=\"{3}\" AllowNodeEditing=\"{4}\" Text=\"{5}\" ToolTip=\"{6}\">",
				elementLongDesc,img,contentMenu,nodeId,editFlag,title,elementLongDesc);
			
			if(elementNode.HasChildNodes)
			{
				XmlNodeList nodeList = elementNode.ChildNodes;
				foreach(XmlNode nodeData in nodeList)
				{
					nodeLongDesc = nodeData.Attributes.GetNamedItem("Name").Value;

					switch(nodeData.Name)
					{
						case SITE:
							img = "images/TreeIcons/Icons/oInboxF.gif";
							contentMenu = "FolderMenu";
							editFlag = true;
							
							break;
						case SITE_FOLDER:
							img = "images/TreeIcons/Icons/Folder.gif";
							contentMenu = "FolderMenu";
							if(nodeData.Attributes.GetNamedItem("Uri") != null)
							{
								nodeLongDesc = nodeData.Attributes.GetNamedItem("Uri").Value;
							}
							else if(nodeData.Attributes.GetNamedItem("Id") != null)
							{
								string folderId = nodeData.Attributes.GetNamedItem("Id").Value;
								SiteFolder folder = new SiteFolder(new Site(globalSiteId), Convert.ToInt32(folderId));
								nodeLongDesc = folder.Uri;
							}
							else
							{
								nodeLongDesc = "NO URI";
							}
							 
							editFlag = true;
							//string folderId = nodeData.Attributes.GetNamedItem("").Value;
							break;
						case BOOK:
							img = "images/TreeIcons/Icons/book.gif";
							contentMenu = "BookMenu";
							editFlag = false;
							break;
						case DOCUMENT:
							img = "images/TreeIcons/Icons/doc.gif";
							break;
					}
					
					
					nodeLongDesc = nodeLongDesc.Replace("'","`");
					nodeDataId = nodeData.Attributes.GetNamedItem("Id") == null ? Guid.NewGuid().ToString() : nodeData.Attributes.GetNamedItem("Id").Value;
					nodeDataId = nodeData.LocalName + "_" + nodeDataId;
					childStr += nodeData.HasChildNodes ? 
						buildTreeXML(nodeData,img,contentMenu,nodeDataId,editFlag,nodeData.Attributes.GetNamedItem("Title").Value,nodeLongDesc) :
						string.Format("<Node LongDesc=\"{0}\" Image=\"{1}\" Expanded=\"False\" ContextMenuName=\"{2}\" ID=\"{3}\" AllowNodeEditing=\"{4}\" Text=\"{5}\" ToolTip=\"{6}\"/>",
						nodeLongDesc
						,img
						,contentMenu
						,nodeDataId
						,editFlag
						,nodeData.Attributes.GetNamedItem("Title").Value
						,nodeLongDesc);
				}
			}
			
			parentStr += childStr;
			parentStr += "</Node>";
			return parentStr;
		}
		#endregion

		#region siteTreeView

		private string getTreeXML(string strXML)
		{
			XmlTextReader reader = new XmlTextReader(strXML,XmlNodeType.Element,null);
			string treeXML = "<Tree>";
			string elementNode = string.Empty;
			string nodeTitle = string.Empty;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(strXML);

			if(xmlDoc.HasChildNodes)
			{
				XmlNode siteElement = xmlDoc.SelectSingleNode("/Site");
				nodeTitle = siteElement.Attributes.GetNamedItem("Title").Value;
				nodeTitle = nodeTitle.Replace("'","");
				elementNode = this.buildTreeXML(siteElement
					,"images/TreeIcons/Icons/oInboxF.gif"
					,"RootMenu"
					,siteElement.Attributes.GetNamedItem("Id").Value
					,true
					,nodeTitle
					,siteElement.Attributes.GetNamedItem("Name").Value);
			}
			
			treeXML += elementNode;
			treeXML += "</Tree>";
			return treeXML;
		}


		private void getSiteTree(int siteId)
		{
			siteId = siteId == 0 ? 285 : siteId;
			Site site = new Site(siteId);

			try
			{
				string siteXML = (string)site.SiteXml;
				string siteTemplateXML = (string) site.SiteTemplateXml;
				string treeXML = this.getTreeXML(siteXML);
				
				TreeView.LoadXmlString(treeXML);
				TreeView.ExpandAllNodes();
				TreeView.Visible = true;
				TreeView.ID = "RadTree1";
				//TreeView.Sort = RadTreeViewSort.Ascending;

				if((site.Status == SiteStatus.Unassigned && site.BuildStatus != SiteBuildStatus.BuildRequested) || (site.Status == SiteStatus.Staging && site.IndexBuildStatus.ToString() != "Building" && site.BuildStatus != SiteBuildStatus.BuildRequested))
				{
					TreeView.DragAndDrop = true;
					TreeView.MultipleSelect = true;
					TreeView.ContextMenuContentFile = "xmlFiles/treeContentMenu.xml";
					TreeView.BeforeClientContextClick = "setPos();";
					this.siteIndexLb.Visible = false;
					this.siteStatusLb.Visible = false;
				}
				else
				{
					this.siteStatusLb.Text = "Site nonEditable";
					this.siteStatusLb.Attributes.Add("style","color:darkred");
					this.SaveBtn.Visible = true;
					this.newSiteVersion.Visible = true;
					this.newSiteVersion.Checked = true;
					//this.newSiteVersion.Attributes.Add("disabled","true");

					try
					{
						this.siteIndexLb.Text = string.Format("Site Index Status: {0}",site.IndexBuildStatus);
					}
					catch
					{
						this.siteIndexLb.Text = "Site Index Status: unavailable";
					}
				
					this.siteIndexLb.Visible = true;
					this.siteStatusLb.Visible = true;
				}
			
				this.SiteIdLabel.Text = "" + siteId;
				this.SiteIdLabel.Attributes.Add("style","visibility:hidden");

			}
			catch(Exception e)
			{
				this.siteStatusLb.Text = string.Format("ERROR while building the TOC.<br> <b>Error Description</b>:<br> {0}",e.Message);
				TreeView.Visible=false;
			}
			

			
		}


		protected void TreeView_AddSiteFolder(object sender, System.EventArgs e)
		{
			string index = this.IdHolder.Text;
			int newId = Convert.ToInt16(this.NewNodeId.Text);
			newId++;
			this.NewNodeId.Text = newId.ToString();//DestroyerBpc.XML_ELE_SITEFOLDER + "_" + newId;
			
			RadTreeNode clickedNode = TreeView.FindNodeById(index);
			RadTreeNode newNode = new RadTreeNode();
			newNode.Text = this.NameBox.Text;
			newNode.ToolTip = this.folderURI.Text;
			newNode.Image = "images/TreeIcons/Icons/Folder.gif";
			newNode.ImageExpanded = "";
			newNode.ContextMenuName = "FolderMenu";
			newNode.EditEnabled = true;
			newNode.ID = string.Format("addedNode_{0}",this.NewNodeId.Text);
			newNode.Value = "BookFolder";
			clickedNode.AddNode(newNode);
			clickedNode.Expanded = true;

			this.HideComponents();
			this.SaveBtn.Visible = true;
			this.newSiteVersion.Visible = true;

			this.jsLabel.Text = "<script>clearhidemenu();</script>";
			this.jsLabel.Visible = true;
		}


		protected void TreeView_editFolder(object sender, System.EventArgs e)
		{

			string index = this.IdHolder.Text;
			RadTreeNode nodeEdited = TreeView.FindNodeById(index);
			string newText = this.NameBox.Text;
			string newURI = this.folderURI.Text;
			nodeEdited.Text = newText;
			nodeEdited.ToolTip = newURI;
			
			this.HideComponents();
			this.SaveBtn.Visible = true;
			this.newSiteVersion.Visible = true;

			this.jsLabel.Text = "<script>clearhidemenu();</script>";
			this.jsLabel.Visible = true;
		}


		private void HideComponents()
		{
			this.addButton.Visible = false;
			this.NewNodeId.Visible = false;
			this.InputText.Visible = false;
			this.IdHolder.Visible = false;
			this.reNameNodeBtn.Visible = false;
			this.NameBox.Visible = false;
			this.bookAddBtn.Visible = false;
			this.BookDB.Visible = false;
			this.cancelBtn.Visible = false;
			this.siteStatusLb.Visible = false;
			this.folderURI.Visible = false;
			this.folderURIRowTitle.Visible = false;
			this.folderURIRowInput.Visible = false;
			this.folderURIDivision.Visible = false;
			this.replaceButton.Visible = false;
			
		}


		private void bindBook()
		{
			BookDB.DataTextField = "BookTitleVersion";
			BookDB.DataValueField = "Id";
			BookDB.DataSource = CreateBookDataSource();
			BookDB.DataBind();
			BookDB.Visible=true;
		}


		private ICollection CreateBookDataSource()
		{
			BookCollection books = new BookCollection(BookBuildStatus.Built);
			DataTable dt = new DataTable();
			DataRow dr;
            books.SortField = BookSortField.Name;
            books.Ascending = true;
            string bookTitle = string.Empty;
			string bookID = string.Empty;
			RadTreeNode doesBookExist = new RadTreeNode();

			dt.Columns.Add(new DataColumn("BookTitleVersion",typeof(string)));
			dt.Columns.Add(new DataColumn("Id",typeof(string)));
			
			foreach(Book book in books)
			{
				dr = dt.NewRow();
				bookTitle = book.Title;
				bookTitle = bookTitle.Length > 30 ? bookTitle.Substring(0,27) + "..." : bookTitle;
				dr[0] = string.Format("{0} ({1}) v{2}",bookTitle,book.Name,book.Version);
				dr[1] = "Book_"+book.Id;
				
				doesBookExist = TreeView.FindNodeById(dr[1].ToString());
				if(doesBookExist == null)
				{
					dt.Rows.Add(dr);
				}
			}

			DataView dv = new DataView(dt);
			return dv;
		}

		protected void TreeView_ReplaceBook(object sender, System.EventArgs e)
		{
			string index = this.IdHolder.Text;
			string newId = this.BookDB.SelectedItem.Value;

			RadTreeNode doesBookExist = TreeView.FindNodeById(newId);
			try
			{
				string bookName = doesBookExist.Text;
				this.siteStatusLb.Text = string.Format("The book:'{0}' already exist on the site",bookName);
				this.siteStatusLb.Attributes.Add("style","color:darkred");
				this.siteStatusLb.Visible = true;
				doesBookExist.Selected = true;
				doesBookExist.Expanded = true;
			}
			catch
			{
				RadTreeNode previoulsyClickedNode = TreeView.FindNodeById(index);
				RadTreeNode replaceNode = new RadTreeNode();
				RadTreeNode parentNode = previoulsyClickedNode.Parent;
				
				replaceNode.Text = this.BookDB.SelectedItem.Text;
				replaceNode.Image = "images/TreeIcons/Icons/book.gif";
				replaceNode.ImageExpanded = "";
				replaceNode.ContextMenuName = "BookMenu";
				replaceNode.EditEnabled = true;
				replaceNode.ID = newId;
				replaceNode.Value = "Book";

				parentNode.AddNode(replaceNode);
				this.TreeView_MoveNode(replaceNode,previoulsyClickedNode);
				previoulsyClickedNode.Remove();

				this.HideComponents();
				this.SaveBtn.Visible = true;
				this.newSiteVersion.Visible = true;
			}

			this.jsLabel.Text = "<script>clearhidemenu();</script>";
			this.jsLabel.Visible = true;
		}

		protected void TreeView_addBook(object sender, System.EventArgs e)
		{
			string index = this.IdHolder.Text;
			string newId = this.BookDB.SelectedItem.Value;

			RadTreeNode doesBookExist = TreeView.FindNodeById(newId);
			try
			{
				string bookName = doesBookExist.Text;
				this.siteStatusLb.Text = string.Format("The book:'{0}' already exist on the site",bookName);
				this.siteStatusLb.Attributes.Add("style","color:darkred");
				this.siteStatusLb.Visible = true;
				doesBookExist.Selected = true;
				doesBookExist.Expanded = true;
			}
			catch
			{
				RadTreeNode clickedNode = TreeView.FindNodeById(index);
				RadTreeNode newNode = new RadTreeNode();
				newNode.Text = this.BookDB.SelectedItem.Text;
				newNode.Image = "images/TreeIcons/Icons/book.gif";
				newNode.ImageExpanded = "";
				newNode.ContextMenuName = "BookMenu";
				newNode.EditEnabled = true;
				newNode.ID = newId;
				newNode.Value = "Book";
				clickedNode.AddNode(newNode);
				clickedNode.Expanded = true;

				this.HideComponents();
				this.SaveBtn.Visible = true;
				this.newSiteVersion.Visible = true;
			}

			this.jsLabel.Text = "<script>clearhidemenu();</script>";
			this.jsLabel.Visible = true;
		}


		protected void TreeView_Save(object sender, System.Web.UI.ImageClickEventArgs e)
		{
//			if(treeType == 0)
//			{
//				this.parseBooksRemoved();
//				return;
//			}

			string treeXML = TreeView.GetXml();
            
			//Checking if needed new version
			if(newSiteVersion.Checked)
			{
				Site site = new Site("","","","");
				Site oldSite = new Site(Convert.ToInt16(this.SiteIdLabel.Text));
				site.Name = oldSite.Name;
				site.Title = oldSite.Title;
				site.Description = oldSite.Description;
				//site.Save();
				string makeFile = CreateMakeFile(treeXML,site);
                makeFile = fixSiteXml(makeFile);
				site.SiteTemplateXml = makeFile;
				site.BuildStatus = SiteBuildStatus.BuildRequested;
				//site.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
				site.RequestedStatus = site.Status;
				site.Save();

				//site.Build();
			
				jsLabel.Text = "<script>window.parent.refresh();</script>";
				jsLabel.Visible = true;
			}
			else //update version
			{
				Site site = new Site(Convert.ToInt16(this.SiteIdLabel.Text));
//				foreach(Book book in site.Books)
//				{
//					site.RemoveBook(book);	//removing each node off of site
//				}
				string makeFile = CreateMakeFile(treeXML,site);
				site.SiteTemplateXml = makeFile;
				site.BuildStatus = SiteBuildStatus.BuildRequested;
				//site.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
				site.Save();
				//site.Build();

				//now building site index
				/*if(site.Status != SiteStatus.Unassigned)
				{
					site.SiteIndex.Build(site.Status,false);
				}*/

				jsLabel.Text = "<script>window.parent.refresh();</script>";
				jsLabel.Visible = true;
				
			}

			this.SaveBtn.Visible = false;
			this.newSiteVersion.Visible = false;

		}
        private string fixSiteXml(string treeXML)
        {
            treeXML = treeXML.Replace("&amp;", "&");
            treeXML = treeXML.Replace("&", "&amp;");
            treeXML = treeXML.Replace("`", "&apos;");

            return treeXML;
        }


		private string CreateMakeFile(string oXML,Site xmlSite)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(oXML);
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";

			if(xmlDoc.HasChildNodes)
			{
				XmlNode treeNode = xmlDoc.SelectSingleNode("/Tree");
				if(treeNode.HasChildNodes)
				{
					XmlNode firstChild = treeNode.FirstChild;
					makefileContent += SiteFolderXML(firstChild,xmlSite);
				}

			}
			makefileContent += "</MakeFile>";
			return makefileContent;
		}


		private string SiteFolderXML(XmlNode elementNode, Site xmlSite)
		{
			string xmlContent = string.Empty;

			switch(elementNode.Attributes.GetNamedItem("ContextMenuName").Value)
			{
				case ROOT_MENU:
					xmlContent += "<Site>";
					if(elementNode.HasChildNodes)
					{
						XmlNodeList nodeList = elementNode.ChildNodes;
						foreach(XmlNode nodeData in nodeList)
						{
							xmlContent += SiteFolderXML(nodeData,xmlSite);
						}
					}
					xmlContent += "</Site>";
					break;

				case FOLDER_MENU:
					string folderURI = string.Empty;
					string uri = elementNode.Attributes["ToolTip"] == null ? "" : elementNode.Attributes.GetNamedItem("ToolTip").Value;
					xmlContent += string.Format("<SiteFolder Name='{0}' Title='{1}' Uri='{2}'>"
						,elementNode.Attributes.GetNamedItem("Text").Value
						,elementNode.Attributes.GetNamedItem("Text").Value
						,uri);
					
					if(elementNode.HasChildNodes)
					{
						XmlNodeList nodeList = elementNode.ChildNodes;
						foreach(XmlNode nodeData in nodeList)
						{
							xmlContent += SiteFolderXML(nodeData,xmlSite);
						}
					}
					xmlContent += "</SiteFolder>";
					break;

				case BOOK_MENU:
					string bookId = elementNode.Attributes.GetNamedItem("ID").Value;
					bookId = bookId.Replace("Book_","");
					xmlContent += string.Format("<Book Id='{0}'/>",bookId);
					//Book book = new Book(Convert.ToInt16(bookId));
					//xmlSite.AddBook(book);
					break;

			}

			return xmlContent;
		}


		private void TreeView_MoveNode(RadTreeNode currNode, RadTreeNode goToNode)
		{

			RadTreeNode parentNode = currNode.Parent;
			int gotoIndx = goToNode.Index;
			int currIndx = currNode.Index;

			parentNode.Nodes[gotoIndx] = currNode;
			parentNode.Nodes[currIndx] = goToNode;
			TreeView.ClearSelectedNodes();

		}

		private void TreeView_NodeDragDrop(object o, Telerik.WebControls.RadTreeNodeEventArgs e)
		{
			RadTreeNode sourceNode = e.SourceDragNode;
			RadTreeNode destNode = e.DestDragNode;         
			this.HideComponents();
                           
			if (TreeView.SelectedNodes.Count > 1)
			{
				foreach (RadTreeNode node in TreeView.SelectedNodes)
				{
					if (node != destNode.Parent)
					{
						RadTreeNodeCollection nodeCollection = (node.Parent != null) ? node.Parent.Nodes : node.TreeViewParent.Nodes;
						nodeCollection.Remove(node);
						destNode.AddNode(node);
					}
					node.Selected = false;
				}
			}
			else 
			{
				if (sourceNode != destNode.Parent && destNode.ContextMenuName != "BookMenu")
				{
					RadTreeNodeCollection nodeCollection = (sourceNode.Parent != null) ? sourceNode.Parent.Nodes : sourceNode.TreeViewParent.Nodes;
					nodeCollection.Remove(sourceNode);
					destNode.AddNode(sourceNode);
				}
				else
				{
					InputText.Text = "Books cannot have children";
					InputText.Visible = true;
				}

				sourceNode.Selected = false;
			}
			destNode.Expanded = true;
			TreeView.ClearSelectedNodes();
      
			this.SaveBtn.Visible = true;
			this.newSiteVersion.Visible = true;
		}


		protected void TreeView_cancel(object sender, System.EventArgs e)
		{
			this.jsLabel.Text = "<script>clearhidemenu();</script>";
			this.jsLabel.Visible = true;
			HideComponents();
		}


		#endregion siteTreeView

		# region bookTreeView

		private void getBookTree(int bookId)
		{
			string siteTitleToDisplay = string.Empty;
			Book book = new Book(bookId);
			SiteCollection sites = new SiteCollection(book);
			Telerik.WebControls.RadTreeNode rootNode = this.CreateNode(book.Title.ToString(),true,book.Id.ToString());
			rootNode.Image = "images/TreeIcons/Icons/book.gif";
			TreeView.AddNode(rootNode);
			
			//Populating all the sites for given book
			foreach(Site site in sites)
			{
				siteTitleToDisplay = string.Format("{0} Version:{1}",site.Title,site.Version);
				Telerik.WebControls.RadTreeNode childNode = this.CreateNode(siteTitleToDisplay,false,site.Id.ToString());
				childNode.Image = "images/TreeIcons/Icons/oInboxF.gif";
				childNode.ContextMenuName = "BookSiteMenu";
				if(site.Status != SiteStatus.Unassigned)
				{
					childNode.Enabled = false;
				}
				rootNode.AddNode(childNode);
			}

			this.TreeView.ID = "RadTree1";
			TreeView.ContextMenuContentFile = "xmlFiles/treeContentMenu.xml";
			TreeView.Visible = true;

		}


		private RadTreeNode CreateNode(string text, bool expanded, string id)
		{
			RadTreeNode node = new RadTreeNode(text);
			node.Expanded = expanded;
			node.ID = id;

			return node;
		}


//		private void removeBookFromSite(int siteId, int bookId)
//		{
//			Site site = new Site(siteId);
//			Book book = new Book(bookId);
//			if(site.Status == SiteStatus.Unassigned)
//			{
//				site.RemoveBook(book);
//			}
//		}


//		private void parseBooksRemoved()
//		{
//			string booksRemoved = (string)ViewState["removedBooks"];
//			booksRemoved = booksRemoved.Substring(0,booksRemoved.Length-1);
//			string[] booksArray = booksRemoved.Split(new char[1]{','});
//			
//			foreach(string element in booksArray)
//			{
//				string[] elementValues = element.Split(new char[1]{'|'});
//				this.removeBookFromSite(Convert.ToInt16(elementValues[0]),Convert.ToInt16(elementValues[1]));
//			}
//			this.SaveBtn.Visible = false;
//		}


		#endregion bookTreeView

		#region TreeHandler

		private void TreeView_ContextClick(object o, Telerik.WebControls.RadTreeNodeEventArgs e)
		{
			RadTreeNode clickedNode = e.NodeClicked;

			switch(e.ContextMenuItemText)
			{
				case CONTENTMENU_ADD_FOLDER :

					this.HideComponents();
					this.addButton.Visible = true;
					this.InputText.Visible = true;
					this.NameBox.Visible = true;
					this.NewNodeId.Visible = true;
					this.IdHolder.Visible = true;
					this.cancelBtn.Visible = true;
					this.folderURI.Visible = true;
					this.folderURIRowTitle.Visible = true;
					this.folderURIRowInput.Visible = true;
					this.folderURIDivision.Visible = true;

					this.InputText.Text = "New Folder Name:";
					this.IdHolder.Text = "";
					this.NameBox.Text = "";
					this.IdHolder.Text = "";
					this.IdHolder.Text += clickedNode.ID;

					this.IdHolder.Attributes.Add("style","visibility:hidden");
					this.NewNodeId.Attributes.Add("style","visibility:hidden");
					
					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;
					
					break;

				case CONTENTMENU_ADD_BOOK :
					this.HideComponents();
					
					this.InputText.Visible = true;
					this.bookAddBtn.Visible = true;
					this.IdHolder.Visible = true;
					this.cancelBtn.Visible = true;
					this.IdHolder.Text = "";
					this.IdHolder.Text += clickedNode.ID;
					this.IdHolder.Attributes.Add("style","visibility:hidden");
					this.InputText.Text = "Book :";
					this.bindBook();

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;

					break;

				case CONTENTMENU_RENAME_FOLDER:
					
					this.HideComponents();
					this.InputText.Visible = true;
					this.IdHolder.Visible = false;
					this.reNameNodeBtn.Visible = true;
					this.NameBox.Visible = true;
					this.cancelBtn.Visible = true;

					this.folderURIRowTitle.Visible = true;
					this.folderURIRowInput.Visible = true;
					this.folderURI.Visible = true;
					this.folderURIDivision.Visible = true;

					this.InputText.Text = "New Folder Name:";
					this.IdHolder.Text = "";
					this.IdHolder.Text += clickedNode.ID;
					this.NameBox.Text = clickedNode.Text;
					this.folderURI.Text = clickedNode.ToolTip;

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;
					
					break;
				
				case CONTENTMENU_REMOVE_FOLDER:
					clickedNode.Remove();
					this.SaveBtn.Visible = true;
					this.newSiteVersion.Visible = true;

					this.jsLabel.Text = "<script>clearhidemenu();</script>";
					this.jsLabel.Visible = true;

					break;

				case CONTENTMENU_REMOVE_BOOK:
					clickedNode.Remove();
					this.SaveBtn.Visible = true;
					this.newSiteVersion.Visible = true;

					this.jsLabel.Text = "<script>clearhidemenu();</script>";
					this.jsLabel.Visible = true;

					break;

				case CONTENTMENU_REMOVE_BOOK_FROM_SITE:
					clickedNode.Remove();
					this.SaveBtn.Visible = true;
					removedBooks = string.Format("{0}{1}|{2},",(string)ViewState["removedBooks"],clickedNode.ID,clickedNode.Parent.ID);
					ViewState["removedBooks"] = removedBooks;
					break;
				
				case CONTENTMENU_MOVE_UP:
					if(clickedNode.Parent != null && clickedNode.Prev != null)
					{
						this.TreeView_MoveNode(clickedNode,clickedNode.Prev);
						this.SaveBtn.Visible = true;
						this.newSiteVersion.Visible = true;
					}
					break;

				case CONTENTMENU_MOVE_DN:
					if(clickedNode.Parent != null && clickedNode.Next != null)
					{
						this.TreeView_MoveNode(clickedNode,clickedNode.Next);
						this.SaveBtn.Visible = true;
						this.newSiteVersion.Visible = true;
					}
					break;

				case CONTENTMENU_REPLACE_BOOK:
					//ViewState["replaceNode"] = clickedNode;
					this.HideComponents();

					this.InputText.Visible = true;
					this.cancelBtn.Visible = true;
					this.replaceButton.Visible = true;
					this.IdHolder.Visible = true;
					this.IdHolder.Text = string.Empty;
					this.IdHolder.Text += clickedNode.ID;
					this.IdHolder.Attributes.Add("style","visibility:hidden");
					this.InputText.Text = "Replace with this Book :";
					this.bindBook();

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;

					break;
			}

		}


		#endregion TreeHandler
	}
}
