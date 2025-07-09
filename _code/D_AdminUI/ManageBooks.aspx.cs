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
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;

namespace Rainbow.Portals._Destroyer
{
	/// <summary>
	///		An aspx page designed to allow users to manage all the books on the site.
	/// </summary>
	public partial class ManageBooks : System.Web.UI.Page
	{
		protected const string COMMAND_CLONE = "Clone";
		protected const string COMMAND_ARCHIVE = "Archive";
		protected const string COMMAND_UNARCHIVE = "Unarchive";
		protected const string COMMAND_BUILD = "Build";
		protected const string COMMAND_TOGGLEARCHIVED = "ToggleArchived";
		protected const string COMMAND_ADDBOOK = "AddBook";
		protected const string COMMAND_TOCDISPLAY = "DisplayToc";
		protected const string LABEL_HIDEARCHIVED = "Hide Archived";
		protected const string LABEL_SHOWARCHIVED = "Show Archived";
		protected const string LABEL_TEXT_SHOW_ALL_BOOKS = "Show All Books";
		protected const string LABEL_TEXT_SHOW_BOOKS_IN_PAGE = "Show Books in Pages";
		protected const string IMAGES_DIR = "images/";
		protected const string ICON_ARCHIVE = "action_project_archive.gif";
		protected const string ICON_UNARCHIVE = "action_project_unarchive.gif";
		protected const string SHOW_ERROR_DIV = "ERROR_Display";
		protected const string ERROR_MODULE = "Book";
		protected const string ERROR_METHOD = "BookBuild";


		private int startIndex;
		private int pageCount = 0;
		private int pageIndex = 0;
		private int PAGE_SIZE = 15;
		public bool showAllFlag = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
				if (!this.IsPostBack)
				{
					this.BindDataGridBooks();
					startIndex = 0;
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
			this.DataGridBooks.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridBooks_OnCommand);
			this.DataGridBooks.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridBooks_OnCancel);
			this.DataGridBooks.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridBooks_OnEdit);
			this.DataGridBooks.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGridBooks_Sort);
			this.DataGridBooks.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridBooks_OnUpdate);
			this.DataGridBooks.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridBooks_OnItemBound);

		}
		#endregion

		#region Properties

		private ICollection CreateDataSource(int maxRows)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("Id",typeof(string)));
			dt.Columns.Add(new DataColumn("Title",typeof(string)));
			dt.Columns.Add(new DataColumn("Name",typeof(string)));
			dt.Columns.Add(new DataColumn("Version",typeof(string)));
			dt.Columns.Add(new DataColumn("Description",typeof(string)));
			dt.Columns.Add(new DataColumn("IsEditable",typeof(bool)));
			dt.Columns.Add(new DataColumn("SourceType",typeof(string)));
			dt.Columns.Add(new DataColumn("SourceUri",typeof(string)));
			dt.Columns.Add(new DataColumn("PublishDate",typeof(string)));
			dt.Columns.Add(new DataColumn("BookStatus",typeof(string)));
			dt.Columns.Add(new DataColumn("Copyright",typeof(string)));
			
			for(int i = startIndex; i < maxRows + startIndex; i++)
			{
				dr = dt.NewRow();
				dr[0] = this.ActiveBookCollection[i].Id;
				dr[1] = this.ActiveBookCollection[i].Title;
				dr[2] = this.ActiveBookCollection[i].Name;
				dr[3] = this.ActiveBookCollection[i].Version;
				dr[4] = this.ActiveBookCollection[i].Description;
				dr[5] = this.ActiveBookCollection[i].IsEditable;
				dr[6] = this.ActiveBookCollection[i].SourceType;
				dr[7] = this.ActiveBookCollection[i].SourceUri;
				dr[8] = this.ActiveBookCollection[i].PublishDate;
				dr[9] = this.ActiveBookCollection[i].BuildStatus;
				dr[10] = this.ActiveBookCollection[i].Copyright;

				dt.Rows.Add(dr);
				if(this.ActiveBookCollection.Count - 1 == i)
				{
					i = maxRows + startIndex;
				}
			}

			DataView dv = new DataView(dt);
			return dv;
		}


		protected bool IncludeArchived
		{
			get 
			{
				object o = ViewState["IncludeArchived"];
				if (o == null) 
				{
					return false;
				}
				return (bool)o;
			}

			set 
			{
				ViewState["IncludeArchived"] = value;
			}
		}

		private BookCollection activeBookCollection = null;	
		private BookCollection ActiveBookCollection
		{
			get
			{
				if (activeBookCollection == null)
				{
					activeBookCollection = new BookCollection(this.LatestBookVersions, this.IncludeArchived);
				}
				return activeBookCollection;
			}
		}

		private bool latestBookVersions = false;
		private bool LatestBookVersions
		{
			get
			{
				return latestBookVersions;
			}
		}

		BookSortField SortField 
		{
			get 
			{
				object o = ViewState["SortField"];
				if (o == null) 
				{
					return BookSortField.Name;
				}
				return (BookSortField)o;
			}

			set 
			{
				if (value == SortField) 
				{
					// same as current sort file, toggle sort direction
					SortAscending = !SortAscending;
				}
				ViewState["SortField"] = value;
			}
		}

		//*********************************************************************
		//
		// SortAscending property is tracked in ViewState
		//
		//*********************************************************************

		bool SortAscending 
		{
			get 
			{
				object o = ViewState["SortAscending"];
				if (o == null) 
				{
					return true;
				}
				return (bool)o;
			}

			set 
			{
				ViewState["SortAscending"] = value;
			}
		}
		#endregion Properties

		private void BindDataGridBooks()
		{
			this.ActiveBookCollection.SortField = this.SortField;
			this.ActiveBookCollection.Ascending = this.SortAscending;
			//DataGridBooks.DataSource = this.ActiveBookCollection;
			//DataGridBooks.DataBind();

			if(showAllFlag)
			{
				this.stats.Visible = false;
				this.nextPage.Visible = false;
				this.prevPage.Visible = false;
				this.DataGridBooks.DataSource = this.CreateDataSource(this.ActiveBookCollection.Count);
			}
			else
			{
				if(this.activeBookCollection.Count > 0)
				{
					this.DataGridBooks.DataSource = this.CreateDataSource(PAGE_SIZE);
				}
				else
				{
					this.DataGridBooks.DataSource = this.activeBookCollection;
				}
				
			}
			
			this.DataGridBooks.DataBind();

			if(!showAllFlag)
			{
				this.DataGrid_ShowStats();
			}
		}

		private void DataGridBooks_OnEdit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{		
			DataGridBooks.EditItemIndex = e.Item.ItemIndex;
			this.setStartIndex();
			this.BindDataGridBooks();			
			jsLabel.Text = "<script>";
			jsLabel.Text += "showButton('";
			jsLabel.Text += (string)e.Item.Cells[0].UniqueID;
			jsLabel.Text += "','";
			jsLabel.Text += e.Item.ItemIndex;
			jsLabel.Text += "','DataGridBooks')";
			jsLabel.Text += "</script>";
			jsLabel.Visible = true;
		}

		private void DataGridBooks_OnUpdate(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int bookInstanceID = Convert.ToInt16(((Label)e.Item.FindControl("BookInstanceID")).Text);
			Book book;
			if(bookInstanceID==0)
			{
				book = new Book("", "", "", "", BookSourceType.Makefile, "");
			}
			else
			{
				book = (Book)this.ActiveBookCollection.GetBookByBookInstanceId(bookInstanceID);
			}			
			book.Title = ((TextBox)e.Item.FindControl("Title")).Text;
			book.SourceUri	= ((TextBox)e.Item.FindControl("Location")).Text;
			book.Description = ((TextBox)e.Item.FindControl("Description")).Text;
			if(book.IsEditable)
			{
				book.Name = ((TextBox)e.Item.FindControl("Name")).Text;
			}
			//book.SourceType = (BookSourceType)(Convert.ToInt16(((DropDownList)e.Item.FindControl("SourceType")).SelectedValue));
			book.Copyright = ((TextBox)e.Item.FindControl("Copyright")).Text;
			book.Save();
			this.activeBookCollection = null;
			// go to non-edit mode
			DataGridBooks.EditItemIndex = -1;
			this.setStartIndex();
			this.BindDataGridBooks();
		}

		private void DataGridBooks_OnCancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGridBooks.EditItemIndex = -1;
			this.setStartIndex();
			this.BindDataGridBooks();
			jsLabel.Visible = false;
		}

		private void DataGridBooks_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			DataGridBooks.EditItemIndex = -1;
			this.SortField = (BookSortField)(Convert.ToInt16(e.SortExpression));
			this.BindDataGridBooks();
			jsLabel.Visible = false;
		}

		private void DataGridBooks_OnItemBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{			
			DataGridBooks.Columns[0].Visible = false;

			/*if (e.Item.ItemType == ListItemType.EditItem)
			{	
				//string sourceTypeValue = ((Label)e.Item.FindControl("Source")).Text;
				//DropDownList currentCbo = (DropDownList) e.Item.FindControl("SourceType");
				//currentCbo.SelectedIndex = currentCbo.Items.IndexOf(currentCbo.Items.FindByText(sourceTypeValue));
			}*/
				
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				ImageButton cloneButton = (ImageButton)e.Item.FindControl("cloneButton");
				cloneButton.CommandName = COMMAND_CLONE;

				ImageButton archiveButton = (ImageButton)e.Item.FindControl("archiveButton");
				int BookInstanceId = Convert.ToInt16(((Label)e.Item.FindControl("BookInstanceDbID")).Text);
				bool archived = this.ActiveBookCollection.GetBookByBookInstanceId(BookInstanceId).Archived;

				archiveButton.ImageUrl = archived ? string.Format("{0}{1}",IMAGES_DIR,ICON_UNARCHIVE) : string.Format("{0}{1}",IMAGES_DIR,ICON_ARCHIVE);
				archiveButton.AlternateText = archived ? COMMAND_UNARCHIVE : COMMAND_ARCHIVE;
				archiveButton.CommandName = archived ? COMMAND_UNARCHIVE : COMMAND_ARCHIVE;
				
				//if book archived change bgcolor and font color
				e.Item.BackColor = archived ? System.Drawing.ColorTranslator.FromHtml("#fdfdfd") : System.Drawing.Color.White;
				e.Item.ForeColor = archived ? System.Drawing.ColorTranslator.FromHtml("gray") : this.ActiveBookCollection.GetBookByBookInstanceId(BookInstanceId).BuildStatus == BookBuildStatus.BuildRequested ? System.Drawing.ColorTranslator.FromHtml("darkblue") : System.Drawing.Color.Black;
				
				if(this.ActiveBookCollection.GetBookByBookInstanceId(BookInstanceId).BuildStatus == BookBuildStatus.Error)
				{
					Event bookEvent = new Event(BookInstanceId.ToString());
					string errorMsg = bookEvent.getEventLogged(ERROR_MODULE,ERROR_METHOD);
					e.Item.ForeColor = System.Drawing.ColorTranslator.FromHtml("darkred");

					if(errorMsg != null)
					{
						ImageButton extraInfo = (ImageButton)e.Item.FindControl("imgHolder");
						extraInfo.Attributes.Add("onclick","getXYvalues();");
						extraInfo.Visible = true;
					}
				}

			}
		}

		private void DataGridBooks_OnCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{			
			this.jsLabel.Text = "";
			Book book;
			switch(e.CommandName)
			{
				case COMMAND_CLONE:
					book = (Book)this.ActiveBookCollection.GetBookByBookInstanceId(Convert.ToInt16(e.CommandArgument));
					Book newBook = new Book(book.Name, book.Title, book.Description, book.Copyright, book.SourceType, book.SourceUri);
					this.ActiveBookCollection.Add(newBook);
					this.ActiveBookCollection.SortField = BookSortField.PublishDate;
					this.SortField = BookSortField.PublishDate;
					this.SortAscending = false;
					DataGridBooks.EditItemIndex = 0;
					this.BindDataGridBooks();
					jsLabel.Text = "<script>";
					jsLabel.Text += "showButton('','0','DataGridBooks')";
					jsLabel.Text += "</script>";
					jsLabel.Visible = true;
					break;

				case COMMAND_TOCDISPLAY:
					int gridRow = e.Item.ItemIndex;
					DataGridItem item = DataGridBooks.Items[e.Item.ItemIndex];
					int bookInstanceId = Convert.ToInt16(((Label)item.FindControl("BookInstanceDbID")).Text);
					
					ImageButton showBooks = (ImageButton)item.FindControl("displayBookTOC");
					showBooks.Attributes.Add("onclick","resizeNice('close','"+showBooks.UniqueID+"');return false;");

					this.DataGridBooksAddToc(gridRow,bookInstanceId,showBooks.UniqueID);
					break;

				case COMMAND_ARCHIVE:
					book = (Book)this.ActiveBookCollection.GetBookByBookInstanceId(Convert.ToInt16(e.CommandArgument));
					book.Archived = true;
					book.Save();
					activeBookCollection = null;
					DataGridBooks.EditItemIndex = -1;
					this.setStartIndex();
					//this.BindDataGridBooks();
					if(startIndex == this.ActiveBookCollection.Count)
					{
						this.DataGrid_prevPage();
					}
					else
					{
						this.BindDataGridBooks();
					}
					break;

				case COMMAND_UNARCHIVE:
					book = (Book)this.ActiveBookCollection.GetBookByBookInstanceId(Convert.ToInt16(e.CommandArgument));
					book.Archived = false;
					book.Save();
					activeBookCollection = null;
					DataGridBooks.EditItemIndex = -1;
					this.setStartIndex();
					this.BindDataGridBooks();
					break;

				case COMMAND_BUILD:
					book = (Book)this.ActiveBookCollection.GetBookByBookInstanceId(Convert.ToInt16(e.CommandArgument));
					book.BuildStatus = BookBuildStatus.BuildRequested;
					book.Save();
					activeBookCollection = null;
					DataGridBooks.EditItemIndex = -1;
					this.setStartIndex();
					this.BindDataGridBooks();
					break;

				case COMMAND_TOGGLEARCHIVED:
					if(IncludeArchived)
					{
						IncludeArchived = false;					
					}
					else
					{
						IncludeArchived = true;
					}
					activeBookCollection = null;
					DataGridBooks.EditItemIndex = -1;
					this.BindDataGridBooks();
					break;

				case COMMAND_ADDBOOK:
					book = new Book("", "", "", "", BookSourceType.Makefile, "");				
					this.ActiveBookCollection.Add(book);
					this.ActiveBookCollection.SortField = BookSortField.BookVersion;
					DataGridBooks.EditItemIndex = 0;
					this.BindDataGridBooks();	
					jsLabel.Text = "<script>";
					jsLabel.Text += "showButton('','0','DataGridBooks')";
					jsLabel.Text += "</script>";
					jsLabel.Visible = true;
					break;

				case SHOW_ERROR_DIV:
					DataGridItem item2 = DataGridBooks.Items[e.Item.ItemIndex];
					string bookId = ((Label)item2.FindControl("BookInstanceDbID")).Text;
					Event bookEvent = new Event(bookId);
					string bookErrorMsg = bookEvent.getEventLogged(ERROR_MODULE,ERROR_METHOD);
					errorMsg.Text = bookErrorMsg;
					errorMsg.Visible = true;
					jsLabel.Text = string.Format("<script>pageLoadedFlag=true;showmenu({0},{1});</script>",xVal.Value,yVal.Value);
					jsLabel.Visible = true;
					break;


			}
		}

		protected string GetToggleArchiveText()
		{
			string retVal = LABEL_SHOWARCHIVED;
			if(IncludeArchived)
			{
				retVal = LABEL_HIDEARCHIVED;
			}
			return retVal;
		}
		private void DataGridBooksAddToc(int row,int bookInstanceId, string uniqueID)
		{
			jsLabel.Text = "<div id='treeContainer' name='treeContainer' style='visibility:visible'>";
			jsLabel.Text += "</div>";
			jsLabel.Text += "<script>";
			jsLabel.Text += "showTree(";
			jsLabel.Text += row;
			jsLabel.Text += ",";
			jsLabel.Text += bookInstanceId;
			jsLabel.Text += ",0);";
			jsLabel.Text += "var oImage = document.getElementById('"+uniqueID+"');";
			jsLabel.Text += "oImage.src = 'images/action_check_in.gif';";
			jsLabel.Text += "</script>";
			jsLabel.Visible = true;
		}

		protected void DataGrid_nextPage(object sender, System.EventArgs e)
		{
			this.setStartIndex();
			pageCount = (int)this.ActiveBookCollection.Count % PAGE_SIZE != 0 ? ((int)this.ActiveBookCollection.Count / PAGE_SIZE) + 1 : (int)this.ActiveBookCollection.Count / PAGE_SIZE;
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			if(pageIndex < pageCount)
			{
				pageIndex++;
			}
			startIndex = pageIndex * PAGE_SIZE;
			this.jsLabel.Visible = false;
			this.BindDataGridBooks();
		}


		protected void DataGrid_prevPage(object sender, System.EventArgs e)
		{
			this.setStartIndex();
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			if(pageIndex > 0)
			{
				pageIndex--;
			}
			startIndex = pageIndex * PAGE_SIZE;
			this.jsLabel.Visible = false;
			this.BindDataGridBooks();		
		}

		protected void DataGrid_prevPage()
		{
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			if(pageIndex > 0)
			{
				pageIndex--;
			}
			startIndex = pageIndex * PAGE_SIZE;
			this.jsLabel.Visible = false;
			this.BindDataGridBooks();		
		}


		protected void DataGrid_setShowAllFlag(object sender, System.EventArgs e)
		{
			
			if(this.showAll.Text == LABEL_TEXT_SHOW_ALL_BOOKS)
			{
				this.showAll.Text = LABEL_TEXT_SHOW_BOOKS_IN_PAGE;
				showAllFlag = true;
			}
			else
			{
				this.showAll.Text = LABEL_TEXT_SHOW_ALL_BOOKS;
				showAllFlag = false;
			}
			this.BindDataGridBooks();
		}


		private void setStartIndex()
		{
			startIndex = (int)ViewState["START_INDEX"];
		}


		private void DataGrid_ShowStats()
		{
			
			int maxRow = this.ActiveBookCollection.Count - 1 < startIndex + PAGE_SIZE ? this.ActiveBookCollection.Count : startIndex + PAGE_SIZE;
			pageCount = (int)this.ActiveBookCollection.Count % PAGE_SIZE != 0 ? ((int)this.ActiveBookCollection.Count / PAGE_SIZE) + 1 : (int)this.ActiveBookCollection.Count / PAGE_SIZE;
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			this.stats.Text = string.Format("Page {0} of {1}, [{2} - {3}] of {4} books"
				,pageIndex + 1
				,pageCount
				,startIndex + 1
				,maxRow
				,this.ActiveBookCollection.Count);
			this.stats.Visible = (pageIndex + 1) < pageCount || pageIndex > 0 ? true : false;
			
			this.prevPage.Visible = pageIndex > 0 ? true : false;
			this.nextPage.Visible = (pageIndex + 1) < pageCount ? true : false;
			this.showAll.Visible = (pageIndex + 1) < pageCount || pageIndex > 0 ? true : false;
			if(this.ActiveBookCollection.Count < PAGE_SIZE)
			{
				this.stats.Visible = false;
				this.nextPage.Visible = false;
				this.prevPage.Visible = false;
			}
			ViewState["START_INDEX"] = startIndex;
		}


//		protected string GetArchiveText(bool isArchived)
//		{
//			string retVal = COMMAND_ARCHIVE;
//			if (isArchived)
//			{
//				retVal = COMMAND_UNARCHIVE;
//			}
//			return retVal;
//		}
	}
}
