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
using AICPA.Destroyer.Content.Site;
using Telerik.WebControls;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;

namespace Rainbow.Portals._Destroyer
{
	/// <summary>
	/// Summary description for ManageSite.
	/// </summary>
	//public class ManageSite : System.Web.UI.Page
	public partial class ManageSite : D_AdminUI.PageBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (!Page.IsPostBack)
			{
				this.siteDataBind();
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
			this.SiteGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridSite_itemCommand);
			this.SiteGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridSite_onCancel);
			this.SiteGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridSite_onEdit);
			this.SiteGrid.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGridSite_Sort);
			this.SiteGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridSite_onUpdate);
			this.SiteGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.itemDataBound);

		}
		#endregion

		#region Properties

		//Controlers
		protected System.Web.UI.WebControls.Image titleSortImage;
		protected System.Web.UI.WebControls.Image nameSortImage;
		protected System.Web.UI.WebControls.Image versionSortImage;
		protected System.Web.UI.WebControls.Image statusSortImage;
		protected System.Web.UI.WebControls.PlaceHolder treeHolder;
		protected System.Web.UI.HtmlControls.HtmlInputHidden indexStatusMonitor;
		protected System.Web.UI.WebControls.Label Label4;

		//Global Variables
		private string statusStr;
		private int startIndex;
		private int pageCount = 0;
		private int pageIndex = 0;
		private int PAGE_SIZE = 15;
		public bool showAllFlag = false;
		
		//Global Constants
		protected const string COMMAND_ARCHIVE = "Archive";
		protected const string COMMAND_UNARCHIVE = "Unarchive";
		protected const string COMMAND_SHOWARCHIVE = "showArchive";
		protected const string COMMAND_CANNOTARCHIVE = "cannotArchive";
		protected const string COMMAND_TOCDISPLAY = "TOC_Display";
		protected const string COMMAND_ADDSITE = "addSite";
		protected const string LABEL_TEXT_SHOWARCHIVED = "Show Archived";
		protected const string LABEL_TEXT_HIDEARCHIVED = "Hide Archived";
		protected const string LABEL_TEXT_UNASSIGNED = "Unassigned";
		protected const string LABEL_TEXT_STAGING = "Staging";
		protected const string LABEL_TEXT_PRODUCTION = "Production";
		protected const string LABEL_TEXT_SHOW_ALL_SITES = "Show All Sites";
		protected const string LABEL_TEXT_SHOW_SITES_IN_PAGE = "Show Sites in Pages";
		protected const string IMAGES_DIR = "images/";
		protected const string ICON_ARCHIVE = "action_project_archive.gif";
		protected const string ICON_UNARCHIVE = "action_project_unarchive.gif";
		protected const string ICON_EXPAND_TREE = "toc_expand.gif";
		protected const string ICON_COLLAPSE_TREE = "toc_collapse.gif";
		protected const string SHOW_ERROR_DIV = "ERROR_Display";
		protected const string LABEL_REBUILD_INDEX = "ReBuildIndex";
		protected const string EXE_SITEMODULE = "Site";
		protected const string EXE_SITEBUILDMETHOD = "SiteBuild";
		protected const string EXE_SITEINDEXMODULE = "SiteIndexModule";
		protected const string EXE_SITEINDEXBUILDMETHOD = "SiteIndexMethod";

		private ICollection CreateDataSource(int maxRows)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("Id",typeof(string)));
			dt.Columns.Add(new DataColumn("Title",typeof(string)));
			dt.Columns.Add(new DataColumn("Name",typeof(string)));
			dt.Columns.Add(new DataColumn("Version",typeof(string)));
			dt.Columns.Add(new DataColumn("Description",typeof(string)));
			dt.Columns.Add(new DataColumn("StageStatus",typeof(string)));
			dt.Columns.Add(new DataColumn("IndexStatus",typeof(string)));
			dt.Columns.Add(new DataColumn("SiteStatus",typeof(string)));
			
			for(int i = startIndex; i < maxRows + startIndex; i++)
			{
				dr = dt.NewRow();
				dr[0] = this.ActiveSiteCollection[i].Id;
				dr[1] = this.ActiveSiteCollection[i].Title;
				dr[2] = this.ActiveSiteCollection[i].Name;
				dr[3] = this.ActiveSiteCollection[i].Version;
				dr[4] = this.ActiveSiteCollection[i].Description;
				dr[5] = this.ActiveSiteCollection[i].Status;
				dr[6] = this.ActiveSiteCollection[i].IndexBuildStatus.ToString();
				dr[7] = this.ActiveSiteCollection[i].BuildStatus;

				dt.Rows.Add(dr);
				if(this.ActiveSiteCollection.Count - 1 == i)
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


		private SiteCollection activeSiteCollection = null;
		private SiteCollection ActiveSiteCollection
		{
			get
			{
				if (activeSiteCollection == null)
				{
					activeSiteCollection = new SiteCollection(this.latestSiteVersions,IncludeArchived);
				}
				return activeSiteCollection;
			}
		}


		private bool latestSiteVersions = false;
		private bool LatestSiteVersions
		{
			get
			{
				return latestSiteVersions;
			}
		}


		SiteSortField SortField 
		{
			get 
			{
				object o = ViewState["SortField"];
				if (o == null) 
				{
					return SiteSortField.Name;
				}
				return (SiteSortField)o;
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


		private void sortImage()
		{

			switch (this.SortField)
			{
				case SiteSortField.Title:
					titleSortImage.Visible = true;
					titleSortImage.ImageUrl = this.SortAscending ? "images/up.gif" : "images/dn.gif";
					break;
			}

		}
		#endregion Properties

		#region siteManagement

		private void siteDataBind()
		{
			this.ActiveSiteCollection.SortField = this.SortField;
			this.ActiveSiteCollection.Ascending = this.SortAscending;
			//this.SiteGrid.DataSource = this.ActiveSiteCollection;
			
			if(showAllFlag)
			{
				this.stats.Visible = false;
				this.nextPage.Visible = false;
				this.prevPage.Visible = false;
				this.SiteGrid.DataSource = this.CreateDataSource(this.ActiveSiteCollection.Count);
			}
			else
			{
				if(this.ActiveSiteCollection.Count > 0)
				{
					this.SiteGrid.DataSource = this.CreateDataSource(PAGE_SIZE);
				}
				else
				{
					this.SiteGrid.DataSource = this.ActiveSiteCollection;
				}
				
			}
			
			this.SiteGrid.DataBind();

			if(!showAllFlag)
			{
				this.DataGrid_ShowStats();
			}
		}


		private void DataGrid_ShowStats()
		{
			
			int maxRow = this.ActiveSiteCollection.Count - 1 < startIndex + PAGE_SIZE ? this.ActiveSiteCollection.Count : startIndex + PAGE_SIZE;
			pageCount = (int)this.ActiveSiteCollection.Count % PAGE_SIZE != 0 ? ((int)this.ActiveSiteCollection.Count / PAGE_SIZE) + 1 : (int)this.ActiveSiteCollection.Count / PAGE_SIZE;
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			this.stats.Text = string.Format("Page {0} of {1}, [{2} - {3}] of {4} sites"
								,pageIndex + 1
								,pageCount
								,startIndex + 1
								,maxRow
								,this.ActiveSiteCollection.Count);
			this.stats.Visible = (pageIndex + 1) < pageCount || pageIndex > 0 ? true : false;
			
			this.prevPage.Visible = pageIndex > 0 ? true : false;
			this.nextPage.Visible = (pageIndex + 1) < pageCount ? true : false;
			this.showAll.Visible = (pageIndex + 1) < pageCount || pageIndex > 0 ? true : false;
			if(this.ActiveSiteCollection.Count < PAGE_SIZE)
			{
				this.stats.Visible = false;
				this.nextPage.Visible = false;
				this.prevPage.Visible = false;
			}
			ViewState["START_INDEX"] = startIndex;
		}


		private void DataGridSite_onEdit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			statusStr = ((Label)e.Item.FindControl("StatusLabel")).Text;
			SiteGrid.EditItemIndex = e.Item.ItemIndex;
			this.setStartIndex();
			this.siteDataBind();

			jsLabel.Text = "<script>";
			jsLabel.Text += "showButton('";
			jsLabel.Text += (string)e.Item.Cells[0].UniqueID;
			jsLabel.Text += "','";
			jsLabel.Text += e.Item.ItemIndex;
			jsLabel.Text += "','SiteGrid')";
			jsLabel.Text += "</script>";
			jsLabel.Visible = true;
		}


		private void itemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			SiteGrid.Columns[0].Visible = false;
			
			if (e.Item.ItemType == ListItemType.EditItem)
			{
				DropDownList statusSelect = (DropDownList)e.Item.FindControl("StatusDDL");
				string selectId = string.Format("getTransferIndex('{0}')",statusSelect.ClientID);
				statusSelect.Attributes.Add("onchange",selectId);

				if(statusStr != SiteStatus.Unassigned.ToString() && statusStr != null)
				{
					DropDownList statusAdd = (DropDownList)e.Item.FindControl("StatusDDL");
					ListItem productionStatus = new ListItem(SiteStatus.Production.ToString(), ((int)SiteStatus.Production).ToString());
					statusAdd.Items.Add(productionStatus);
					statusSelect.SelectedIndex = statusSelect.Items.IndexOf(statusSelect.Items.FindByText(statusStr));
					if(statusStr == SiteStatus.Production.ToString())
					{
						TextBox title = (TextBox)e.Item.FindControl("EntryTitle");
						TextBox name = (TextBox)e.Item.FindControl("EntryName");
						TextBox desc = (TextBox)e.Item.FindControl("EntryDescription");
						title.Attributes.Add("disabled","true");
						name.Attributes.Add("disabled","true");
						desc.Attributes.Add("disabled","true");
					}
				}
				else if(statusStr == null)
				{
					DropDownList statusAdd = (DropDownList)e.Item.FindControl("StatusDDL");
					statusAdd.Items.RemoveAt(1);
				}
			}

			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				int SiteId = Convert.ToInt16(((Label)e.Item.FindControl("siteDbId")).Text);

				if(this.ActiveSiteCollection.GetSiteById(SiteId).Archived)
				{
					ImageButton editBtn = (ImageButton)e.Item.FindControl("IconModify");
					editBtn.Attributes.Add("onclick","editArchive()");
				}

				ImageButton archiveBtn = (ImageButton)e.Item.FindControl("IconDelete");
				archiveBtn.ImageUrl = this.ActiveSiteCollection.GetSiteById(SiteId).Archived ? string.Format("{0}{1}",IMAGES_DIR,ICON_UNARCHIVE) : string.Format("{0}{1}",IMAGES_DIR,ICON_ARCHIVE);
				archiveBtn.AlternateText = this.ActiveSiteCollection.GetSiteById(SiteId).Archived ? COMMAND_UNARCHIVE : COMMAND_ARCHIVE;
				archiveBtn.CommandName = this.ActiveSiteCollection.GetSiteById(SiteId).Archived ? COMMAND_UNARCHIVE : this.ActiveSiteCollection.GetSiteById(SiteId).Status.ToString() == LABEL_TEXT_UNASSIGNED ? COMMAND_ARCHIVE : COMMAND_CANNOTARCHIVE;
				e.Item.BackColor = this.ActiveSiteCollection.GetSiteById(SiteId).Archived  ? System.Drawing.ColorTranslator.FromHtml("#fdfdfd") : System.Drawing.Color.White;
				e.Item.ForeColor = this.ActiveSiteCollection.GetSiteById(SiteId).Archived ? System.Drawing.ColorTranslator.FromHtml("gray") : System.Drawing.Color.Black;

				if(this.ActiveSiteCollection.GetSiteById(SiteId).IndexBuildStatus == SiteIndexBuildStatus.Error || this.ActiveSiteCollection.GetSiteById(SiteId).BuildStatus == SiteBuildStatus.Error)
				{
					Label siteIndxStatus = (Label)e.Item.FindControl("SiteIndexStatus");
					siteIndxStatus.ForeColor = System.Drawing.ColorTranslator.FromHtml("darkred");
					ImageButton extraInfo = (ImageButton)e.Item.FindControl("imgHolder");
					ImageButton rebuildBtn = (ImageButton)e.Item.FindControl("reBuildIndex");

					extraInfo.Attributes.Add("onclick","getXYvalues();");
					extraInfo.Visible = true;
					e.Item.ForeColor = System.Drawing.ColorTranslator.FromHtml("darkred");
					rebuildBtn.Visible = true;
					
				}

				if(this.ActiveSiteCollection.GetSiteById(SiteId).BuildStatus == SiteBuildStatus.BuildRequested)
				{
					e.Item.ForeColor = System.Drawing.ColorTranslator.FromHtml("darkblue");
				}
			}

		}


		private void DataGridSite_onCancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			SiteGrid.EditItemIndex = -1;
			this.setStartIndex();
			this.siteDataBind();
			jsLabel.Visible = false;
		}


		private void DataGridSite_onUpdate(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGridItem item = SiteGrid.Items[SiteGrid.EditItemIndex];
			this.setStartIndex();

			RequiredFieldValidator requiredTitle = (RequiredFieldValidator) item.FindControl("RequiredfieldvalidatorTitle"); 
			RequiredFieldValidator requiredName = (RequiredFieldValidator) item.FindControl("RequiredfieldvalidatorName");

			requiredTitle.Validate();
			requiredName.Validate();

			jsLabel.Text = string.Empty;

			if(requiredTitle.IsValid && requiredName.IsValid)
			{
				int SiteId = Convert.ToInt16(((Label)item.FindControl("SiteId")).Text);
				if(SiteId != 0 )
				{
					Site site = (Site)this.ActiveSiteCollection.GetSiteById(SiteId);
					site.Title = ((TextBox)item.FindControl("EntryTitle")).Text;
					site.Name = ((TextBox)item.FindControl("EntryName")).Text;
					site.Description = ((TextBox)item.FindControl("EntryDescription")).Text;
					site.RequestedStatus = (SiteStatus)Convert.ToInt16(((DropDownList) e.Item.FindControl("StatusDDL")).SelectedItem.Value);
					site.Archived = this.ActiveSiteCollection.GetSiteById(SiteId).Archived ? false : this.ActiveSiteCollection.GetSiteById(SiteId).Archived;

					if(site.RequestedStatus != site.Status)
					{
						site.BuildStatus = SiteBuildStatus.BuildRequested;
						//site.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
					}
					site.Save();


//					if(site.Status != SiteStatus.Unassigned)
//					{
//						if(site.Status == SiteStatus.PreProduction)
//						{
//							//site.SiteIndex.Build(site.Status,this.indexTransfer.Checked);
//							site.BuildStatus = SiteBuildStatus.BuildRequested;
//						}
//						else
//						{
//							//site.SiteIndex.Build(site.Status,false);
//							site.BuildStatus = SiteBuildStatus.BuildRequested;
//						}
//						jsLabel.Text = string.Format("<script>refresh();</script>",SiteId,item.ItemIndex);
//					}
//					site.Save();
				}
				else
				{
					Site site = new Site();
					site.Title = ((TextBox)item.FindControl("EntryTitle")).Text;
					site.Name = ((TextBox)item.FindControl("EntryName")).Text;
					site.Description = ((TextBox)item.FindControl("EntryDescription")).Text;
					site.Status = (SiteStatus)Convert.ToInt16(((DropDownList) e.Item.FindControl("StatusDDL")).SelectedItem.Value);
					site.Save();
				}
					

				SiteGrid.EditItemIndex = -1;
				this.siteDataBind();
				jsLabel.Visible = true;
			}
			
		}


		private void DataGridSite_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			SiteGrid.EditItemIndex = -1;
			this.SortField = (SiteSortField)(Convert.ToInt16(e.SortExpression));
			this.siteDataBind();
			jsLabel.Visible = false;
		}


		private void DataGridSite_itemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			
			this.jsLabel.Text = string.Empty;
			

			switch(e.CommandName)
			{
				case COMMAND_ADDSITE:
						this.SiteGrid_addRow();
					break;

				case COMMAND_TOCDISPLAY:
						int GridRow = e.Item.ItemIndex;
						DataGridItem item = SiteGrid.Items[e.Item.ItemIndex];

						ImageButton openTOC = (ImageButton)item.FindControl("editTreeBtnDN");
						openTOC.Attributes.Add("onclick","resizeNice('close','"+openTOC.UniqueID+"');return false;monitoringValues();");
						int siteId = Convert.ToInt16(((Label)item.FindControl("siteDbId")).Text);

						this.SiteGrid_addTOC(GridRow,siteId,openTOC.UniqueID);
						//this.siteDataBind();
						
					break;

				case COMMAND_SHOWARCHIVE:
						IncludeArchived = IncludeArchived ? false : true;
						this.activeSiteCollection = null;
						this.SiteGrid.EditItemIndex = -1;
						this.siteDataBind();
					break;

				case COMMAND_UNARCHIVE:
						int UnarchiveSiteId = Convert.ToInt16(((Label)SiteGrid.Items[e.Item.ItemIndex].FindControl("siteDbId")).Text);
						this.toggleArchiveSite(false,UnarchiveSiteId);
					break;

				case COMMAND_ARCHIVE:
						int ArchiveSiteId = Convert.ToInt16(((Label)SiteGrid.Items[e.Item.ItemIndex].FindControl("siteDbId")).Text);
						this.toggleArchiveSite(true,ArchiveSiteId);
					break;

				case COMMAND_CANNOTARCHIVE:
						jsLabel.Text = "<script>cannotArchive();</script>";
						jsLabel.Visible = true;
					break;

				case SHOW_ERROR_DIV:
						DataGridItem item2 = SiteGrid.Items[e.Item.ItemIndex];
						int siteId2 = Convert.ToInt16(((Label)item2.FindControl("siteDbId")).Text);
						Event eventError = new Event(siteId2.ToString());
						if(this.ActiveSiteCollection.GetSiteById(siteId2).BuildStatus == SiteBuildStatus.Error)
						{
							//errorGrid.DataSource = eventError.getEventLogged(EXE_SITEMODULE,EXE_SITEBUILDMETHOD);
							errorLabel.Text = eventError.getEventLogged(EXE_SITEMODULE,EXE_SITEBUILDMETHOD);
							errorLabel.Visible = true;
						}
						else if(this.ActiveSiteCollection.GetSiteById(siteId2).IndexBuildStatus == SiteIndexBuildStatus.Error)
						{
							errorGrid.DataSource = eventError.getEventLogged(EXE_SITEINDEXMODULE,EXE_SITEINDEXBUILDMETHOD);
							errorGrid.DataBind();
							errorGrid.CssClass = "error_grid";
							errorGrid.ItemStyle.Wrap = false;
							errorGrid.Visible = true;
						}
						//errorGrid.DataSource = this.ActiveSiteCollection.GetSiteById(siteId2).SiteIndex.BuildErrors;
						jsLabel.Text = string.Format("<script>pageLoadedFlag=true;showmenu({0},{1});</script>",xVal.Value,yVal.Value);
						jsLabel.Visible = true;
					break;

				case LABEL_REBUILD_INDEX:
						DataGridItem item3 = SiteGrid.Items[e.Item.ItemIndex];
						int siteId3 = Convert.ToInt16(((Label)item3.FindControl("siteDbId")).Text);
						//may need to change flag (ask Jared)
						Site site = (Site)this.ActiveSiteCollection.GetSiteById(siteId3);
						//site.SiteIndex.Build(site.Status,false);
						//ask Jared if we also need to build site
						//site.BuildStatus = SiteBuildStatus.BuildRequested;
						site.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
						site.RequestedStatus = site.Status;
						site.Save();
						jsLabel.Text = "<script>refresh();</script>";//string.Format("<script>setMonitor('{0}|{1}');</script>",siteId3,e.Item.ItemIndex);
					break;

			}
		}

		
		private void toggleArchiveSite(bool archived, int siteId)
		{
			Site site = (Site)this.ActiveSiteCollection.GetSiteById(siteId);
			site.Archived = archived;
			site.Save();
			this.activeSiteCollection = null;
			this.SiteGrid.EditItemIndex = -1;
			this.setStartIndex();
			
			if(startIndex == this.ActiveSiteCollection.Count)
			{
				this.DataGrid_prevPage();
			}
			else
			{
				this.siteDataBind();
			}
		}

		
		private void SiteGrid_addRow()
		{
			Site site = new Site("","","","");
			this.ActiveSiteCollection.Add(site);
			SiteGrid.EditItemIndex = 0;
			this.siteDataBind();
			jsLabel.Text = "<script>";
			jsLabel.Text += "showButton('','0','SiteGrid')";
			jsLabel.Text += "</script>";
			jsLabel.Visible = true;
		}


		private void SiteGrid_addTOC(int row,int siteId,string uniqueID)
		{
			jsLabel.Text = "<div id='treeContainer' name='treeContainer' style='visibility:visible'>";
			jsLabel.Text += "</div>";
			jsLabel.Text += "<script>";
			jsLabel.Text += "showTree(";
			jsLabel.Text += row;
			jsLabel.Text += ",";
            jsLabel.Text += siteId;
			jsLabel.Text += ",1);";
			jsLabel.Text += "var oImage = document.getElementById('"+uniqueID+"');";
			jsLabel.Text += "oImage.src = 'images/toc_collapse.gif';";
			jsLabel.Text += "</script>";
			jsLabel.Visible = true;
		}	


		protected string GetToggleArchiveText()
		{
			string retVal = IncludeArchived ? LABEL_TEXT_HIDEARCHIVED : LABEL_TEXT_SHOWARCHIVED;
			return retVal;
		}


		protected void DataGrid_nextPage(object sender, System.EventArgs e)
		{
			this.setStartIndex();
			pageCount = (int)this.ActiveSiteCollection.Count % PAGE_SIZE != 0 ? ((int)this.ActiveSiteCollection.Count / PAGE_SIZE) + 1 : (int)this.ActiveSiteCollection.Count / PAGE_SIZE;
			pageIndex = startIndex == 0 ? 0 : startIndex/PAGE_SIZE;

			if(pageIndex < pageCount)
			{
				pageIndex++;
			}
			startIndex = pageIndex * PAGE_SIZE;
			this.jsLabel.Visible = false;
			this.siteDataBind();
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
			this.siteDataBind();		
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
			this.siteDataBind();		
		}


		protected void DataGrid_setShowAllFlag(object sender, System.EventArgs e)
		{
			
			if(this.showAll.Text == LABEL_TEXT_SHOW_ALL_SITES)
			{
				this.showAll.Text = LABEL_TEXT_SHOW_SITES_IN_PAGE;
				showAllFlag = true;
			}
			else
			{
				this.showAll.Text = LABEL_TEXT_SHOW_ALL_SITES;
				showAllFlag = false;
			}
			this.siteDataBind();
		}


		private void setStartIndex()
		{
			startIndex = (int)ViewState["START_INDEX"];
		}


		#endregion siteManagement

	}
}
