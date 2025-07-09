namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;

	using AICPA.Destroyer.Shared;
	using AICPA.Destroyer.User;
	using AICPA.Destroyer.Content;
	using AICPA.Destroyer.Content.Site;
	using AICPA.Destroyer.Content.Book;
	using AICPA.Destroyer.Content.Document;

	/// <summary>
	///		Summary description for D_QuickFind.
	/// </summary>
	public partial class D_QuickFindNav : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{
		protected Telerik.WebControls.RadPanelbar RadPanelbar1;
		protected System.Web.UI.HtmlControls.HtmlTableCell QuickFindTd;
		protected System.Web.UI.HtmlControls.HtmlTableCell content;

		protected string TAXONOMY_XMLFILE = HttpContext.Current.Server.MapPath("xmlFiles/Taxonomy.xml");
		private int counter = 0;
		private int uniqueIdentifier = 0;
		private string div_Ids = string.Empty;
		private string userAccess = string.Empty;

		protected const string LEVEL_1 = "&nbsp;";
		protected const string DIV_ID = "div_";
		protected const string TAXONOMY = "Taxonomy";
		protected const string TAXONOMY_VALUE = "TaxonomyValue";
		protected const string EMAP = "emap";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			userAccess = this.setUserAccess();
			this.buildQuickFindTable();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		private string setUserAccess()
		{
			IUser user = DestroyerUi.GetCurrentUser(this.Page);
			int counter = 0;
			bool emapFlag = false;
			string accessStr = string.Empty;
			
			foreach(string bookName in user.UserSecurity.BookName)
			{
				counter++;
				if(bookName.IndexOf(EMAP) > -1)
				{
					emapFlag = true;
				}
			}
			
			if(counter == 1 && emapFlag)
			{
				accessStr = "~"+EMAP+"~";
			}
			else if(counter > 1 && emapFlag)
			{
				accessStr = "~smre~";
			}
			else if(counter > 0 && !emapFlag)
			{
				accessStr = "~sm~";
			}

			return accessStr;
		}

		private void buildQuickFindTable()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(TAXONOMY_XMLFILE);
			XmlNode root = xmlDoc.SelectSingleNode("/Taxonomies");
			HtmlTableRow taxonomyTableRow = new HtmlTableRow();
			HtmlTable subTable = new HtmlTable();

			if(root.HasChildNodes)
			{
				foreach(XmlNode node in root.ChildNodes)
				{
					if(node.LocalName == TAXONOMY)
					{
						taxonomyTableRow = this.addSection(node.Attributes.GetNamedItem("title").Value);
						this.QFTable.Controls.Add(taxonomyTableRow);

						taxonomyTableRow = this.genericBuild(node,"",true,false,node.Attributes.GetNamedItem("title").Value);
						this.QFTable.Controls.Add(taxonomyTableRow);
					}
				}
			}
			divIds.Value = div_Ids;
		}

		private HtmlTableRow genericBuild(XmlNode parentNode,string spacer,bool colSpanding, bool nestedParent,string divName)
		{
			HtmlTableRow returnRow = new HtmlTableRow();
			HtmlTableCell cell = new HtmlTableCell();
			HtmlTable table = new HtmlTable();
			Panel div = new Panel();
			string xmlAccess = string.Empty;
			table.Border = 0;
			table.CellSpacing = 0;
			if(colSpanding)
			{
				cell.ColSpan = 2;
			}
			HtmlTableRow tr = new HtmlTableRow();

			foreach(XmlNode child in parentNode)
			{
				if(child.LocalName == TAXONOMY_VALUE)
				{
					xmlAccess = child.Attributes.GetNamedItem("access").Value;

					if(lowestLevelTaxonomyValue(child))
					{
						tr = this.addSection(child.Attributes.GetNamedItem("title").Value,child.Attributes.GetNamedItem("id").Value,spacer+LEVEL_1,"images/portal/header.gif",false);
					}
					else
					{
						tr = this.addSection(child.Attributes.GetNamedItem("title").Value,child.Attributes.GetNamedItem("id").Value,spacer+LEVEL_1,"images/minus.gif",true);
						if(xmlAccess.IndexOf(this.userAccess) > -1)
						{
							table.Controls.Add(tr);
						}						
						tr = this.genericBuild(child,LEVEL_1+LEVEL_1,true,true,child.Attributes.GetNamedItem("title").Value);
					}
				}

				if(xmlAccess.IndexOf(this.userAccess) > -1)
				{
					table.Controls.Add(tr);
				}
				//table.Controls.Add(tr);
			}

			counter++;
			divName = divName.Replace(" ","_");
			div.ID = "div_"+divName;
			div.Attributes["class"] = "divHiddenClass";
			div.Controls.Add(table);
			cell.Controls.Add(div);
			div_Ids = div_Ids + div.ID +",";

			returnRow.Controls.Add(cell);

			return returnRow;
		}


		private HtmlTableRow addSection(string Title)
		{
			HtmlTableRow mainTitleTableRow = new HtmlTableRow();
			HtmlTableCell titleTableCell = new HtmlTableCell();
			HtmlTableCell titleTableCell2 = new HtmlTableCell();
			titleTableCell.Align = "left";
			titleTableCell.InnerHtml = "<b>"+Title+"</b>";
			titleTableCell.Attributes["class"] = "taxonomy";
			mainTitleTableRow.Controls.Add(titleTableCell);
			Title = Title.Replace(" ","_");
			titleTableCell2.InnerHtml = "<img id='img_" + Title + "' src='images/portal/topicbar_up.gif' style='cursor:hand;cursor:pointer' onClick=\"getDiv('" + Title + "');\">";
			titleTableCell2.Attributes["class"] = "taxonomy_image";
			titleTableCell2.Width="2%";
			mainTitleTableRow.Controls.Add(titleTableCell2);

			return mainTitleTableRow;
		}


		private HtmlTableRow addSection(string Title,string Id, string spacer,string image,bool nestedParent)
		{
			string expandStr = nestedParent ? " style='cursor:hand;cursor:pointer' id='img_"+Title.Replace(" ","_") + "' onClick=\"getDiv('" + Title.Replace(" ","_") + "');\" " : string.Empty;
			string uniqueId = Id.Replace(" ","_");
			
			HtmlTableRow mainTitleTableRow = new HtmlTableRow();
			HtmlTableCell titleTableCell = new HtmlTableCell();
			HtmlTableCell titleTableCell2 = new HtmlTableCell();
			titleTableCell.Align = "left";
			titleTableCell.VAlign = "top";
			titleTableCell.Width = "3%";
			titleTableCell.NoWrap = true;
			titleTableCell.InnerHtml = spacer+"<img "+ expandStr + " src='"+image+"'>&nbsp;";
			uniqueId = spacer == "&nbsp;&nbsp;&nbsp;" ? "child_"+uniqueId : uniqueId;
			uniqueIdentifier++;
			uniqueId = uniqueIdentifier + "_" + uniqueId;
			mainTitleTableRow.Controls.Add(titleTableCell);
			titleTableCell2.Align = "left";
			
			if(nestedParent)
			{
				titleTableCell2.InnerHtml = "<span class='subElement' " + expandStr + " >"+Title+"</span>";
			}
			else
			{
				titleTableCell2.InnerHtml = "<span id='"+uniqueId+"' class='subElement' onClick=\"getLink('"+Id+"','"+uniqueId+"');\">"+Title+"</span>";
			}
			
			mainTitleTableRow.Controls.Add(titleTableCell2);
			return mainTitleTableRow;
		}


		private bool lowestLevelTaxonomyValue(XmlNode evaluateNode)
		{
            bool returnValue = true;

			foreach(XmlNode node in evaluateNode.ChildNodes)
			{
				if(node.LocalName == TAXONOMY_VALUE)
				{
					returnValue = false;
				}
			}
			return returnValue;
		}
	}
}
