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
using System.Xml.XPath;

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.User;
using AICPA.Destroyer.Content.Subscription;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for quickFind_links.
	/// </summary>
	public partial class quickFind_links : DestroyerUi
	{

		protected string TAXONOMY_XMLFILE = HttpContext.Current.Server.MapPath("xmlFiles/Taxonomy.xml");
		protected const string HOME = "quickFindHome";
		protected const string COLUMN = "Column";
		protected const string DOCUMENT = "Document";
		protected const string EMAP = "emap";
		protected ImageButton helpBtn = new ImageButton();



		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			if(!this.Page.IsPostBack)
			{
				this.displayIntro();
			}

			string NodeId = Request.QueryString["nodeId"];			
			if(NodeId != HOME)
			{
				this.emapOnly.Visible = false;
				this.resourceAndEmap.Visible = false;
				this.resourceOnly.Visible = false;
				this.buildTable(NodeId);
			}
			else
			{
				HtmlTable tableHeader = new HtmlTable();
				HtmlTableRow tableHeaderTR = new HtmlTableRow();

				HtmlTableCell tableHeaderImgLeftCell = new HtmlTableCell();
				tableHeaderImgLeftCell.InnerHtml = "<img src='images/portal/titleBarLeft.gif'>";
				tableHeaderImgLeftCell.Width="1%";
				tableHeaderImgLeftCell.VAlign="top";
				tableHeaderImgLeftCell.Align="right";

				HtmlTableCell tableHeaderImgRightCell = new HtmlTableCell();
				tableHeaderImgRightCell.InnerHtml = "<img src='images/portal/titleBarRight.gif'>";
				tableHeaderImgRightCell.Width="1%";
				tableHeaderImgRightCell.VAlign="top";

				//ImageButton helpBtn = new ImageButton();
				helpBtn.ImageUrl = "images/portal/icon_help_16.gif";
				helpBtn.ID = "HelpImageButton";
				helpBtn.Attributes.Add("title","help");
	
				HtmlTableCell tableHeaderHelpCell = new HtmlTableCell();
				tableHeaderHelpCell.InnerHtml = "";
				tableHeaderHelpCell.Attributes["class"] = "taxonomyValueTitle";
				tableHeaderHelpCell.Align = "right";
				tableHeaderHelpCell.Width = "5%";
				tableHeaderHelpCell.VAlign = "top";
				tableHeaderHelpCell.Controls.Add(helpBtn);

				HtmlTableCell tableHeaderMidCell = new HtmlTableCell();
				tableHeaderMidCell.InnerHtml = "<b>Quick Find</b>";
				tableHeaderMidCell.Attributes["class"] = "taxonomyValueTitle";

				tableHeaderTR.Controls.Add(tableHeaderImgLeftCell);
				tableHeaderTR.Controls.Add(tableHeaderMidCell);
				tableHeaderTR.Controls.Add(tableHeaderHelpCell);
				tableHeaderTR.Controls.Add(tableHeaderImgRightCell);
				tableHeader.Controls.Add(tableHeaderTR);
				tableHeader.Width = "100%";
				tableHeader.Border=0;
				tableHeader.CellSpacing=0;
				tableHeader.CellPadding=0;

				HtmlTableRow tableHeaderRow = new HtmlTableRow();
				HtmlTableCell tableHeaderCell = new HtmlTableCell();
				tableHeaderCell.ColSpan = 4;

				tableHeaderCell.Controls.Add(tableHeader);
				tableHeaderRow.Controls.Add(tableHeaderCell);
				quickFindLinks.Controls.Add(tableHeaderRow);
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
			this.helpBtn.Click += new System.Web.UI.ImageClickEventHandler(this.HelpImageButton_Click);
		}
		#endregion

		private void displayIntro()
		{
			IUser user = DestroyerUi.GetCurrentUser(this.Page);
			string[] subscriptionCodes = user.UserSecurity.Domain.Split(DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR);
			int counter = 0;
			bool emapFlag = false;


			foreach(string subscriptionCode in subscriptionCodes)
			{
				counter++;
				if(subscriptionCode.IndexOf(EMAP) > -1)
				{
					emapFlag = true;
				}
			}

			if(counter > 0 && !emapFlag)
			{
				this.resourceOnly.Visible = true;
			}
			else if(counter == 1 && emapFlag)
			{
				this.emapOnly.Visible = true;
			}
			else if(counter > 1 && emapFlag)
			{
				this.resourceAndEmap.Visible = true;
			}

		}


		private void buildTable(string nodeId)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(TAXONOMY_XMLFILE);
			string XPathExpression = "//Taxonomy/TaxonomyValue[@id='"+nodeId+"']";
			string nestedXPathExpression = "//Taxonomy/TaxonomyValue/TaxonomyValue[@id='"+nodeId+"']";
			XmlNode mainNode = xmlDoc.SelectSingleNode(XPathExpression);
			HtmlTableRow tr = new HtmlTableRow();
			
			if(mainNode == null)
			{
				mainNode = xmlDoc.SelectSingleNode(nestedXPathExpression);
			}

			HtmlTable tableHeader = new HtmlTable();
			HtmlTableRow tableHeaderTR = new HtmlTableRow();

			HtmlTableCell tableHeaderImgLeftCell = new HtmlTableCell();
			tableHeaderImgLeftCell.InnerHtml = "<img src='images/portal/titleBarLeft.gif'>";
			tableHeaderImgLeftCell.Width="1%";
			tableHeaderImgLeftCell.VAlign="top";
			tableHeaderImgLeftCell.Align="right";

			HtmlTableCell tableHeaderImgRightCell = new HtmlTableCell();
			tableHeaderImgRightCell.InnerHtml = "<img src='images/portal/titleBarRight.gif'>";
			tableHeaderImgRightCell.Width="1%";
			tableHeaderImgRightCell.VAlign="top";

			HtmlTableCell tableHeaderMidCell = new HtmlTableCell();
			tableHeaderMidCell.InnerHtml = "<b>"+mainNode.Attributes.GetNamedItem("title").Value+"</b>";
			tableHeaderMidCell.Attributes["class"] = "taxonomyValueTitle";

			//ImageButton helpBtn = new ImageButton();
			helpBtn.ImageUrl = "images/portal/icon_help_16.gif";
			helpBtn.ID = "HelpImageButton";
			helpBtn.Attributes.Add("title","help");
	
			HtmlTableCell tableHeaderHelpCell = new HtmlTableCell();
			tableHeaderHelpCell.InnerHtml = "";
			tableHeaderHelpCell.Attributes["class"] = "taxonomyValueTitle";
			tableHeaderHelpCell.Align = "right";
			tableHeaderHelpCell.Width = "5%";
			tableHeaderHelpCell.VAlign = "top";
			tableHeaderHelpCell.Controls.Add(helpBtn);

			tableHeaderTR.Controls.Add(tableHeaderImgLeftCell);
			tableHeaderTR.Controls.Add(tableHeaderMidCell);
			tableHeaderTR.Controls.Add(tableHeaderHelpCell);
			tableHeaderTR.Controls.Add(tableHeaderImgRightCell);
			tableHeader.Controls.Add(tableHeaderTR);
			tableHeader.Width = "100%";
			tableHeader.Border=0;
			tableHeader.CellSpacing=0;
			tableHeader.CellPadding=0;

			HtmlTableRow tableHeaderRow = new HtmlTableRow();
			HtmlTableCell tableHeaderCell = new HtmlTableCell();
			tableHeaderCell.ColSpan = 4;

			tableHeaderCell.Controls.Add(tableHeader);
			tableHeaderRow.Controls.Add(tableHeaderCell);
			quickFindLinks.Controls.Add(tableHeaderRow);

			HtmlTableCell blankTdTop = new HtmlTableCell();
			blankTdTop.InnerHtml = "";
			tr.Controls.Add(blankTdTop);
			quickFindLinks.Controls.Add(tr);

			if(mainNode.HasChildNodes)
			{
				foreach(XmlNode node in mainNode)
				{
					if(node.LocalName == DOCUMENT)
					{
						tr = this.buildRow(node,node.Attributes.GetNamedItem("targetdoc").Value,node.Attributes.GetNamedItem("targetptr").Value);
					}
					quickFindLinks.Controls.Add(tr);
				}
			}
			else
			{
				HtmlTableCell td = new HtmlTableCell();
				td.InnerHtml = "The link has no sub-links";
				tr.Controls.Add(td);
				quickFindLinks.Controls.Add(tr);
			}

			HtmlTableCell blankTdBottom = new HtmlTableCell();
			blankTdBottom.InnerHtml = "";
			tr.Controls.Add(blankTdBottom);
			quickFindLinks.Controls.Add(tr);
			
		}

		private HtmlTableRow buildRow(XmlNode docNode,string targetdoc,string targetptr)
		{
			HtmlTableRow tr = new HtmlTableRow();
			string tmp = string.Empty;
			AICPA.Destroyer.Content.Site.ISite site = GetCurrentSite(this.Page);
			
			HtmlTableCell imgTd = new HtmlTableCell();

			imgTd.InnerHtml = site.Books[targetdoc] != null ? "<img src='images/portal/gray_arrow.gif'>" : "<img src='images/portal/main-lock_small.gif'>";
			imgTd.VAlign = "top";
			tr.Controls.Add(imgTd);

			foreach(XmlNode node in docNode)
			{
				if(node.LocalName == COLUMN)
				{

					HtmlTableCell td = new HtmlTableCell();
					if(node.Attributes.GetNamedItem("isLink") != null)
					{
						tmp = node.Attributes.GetNamedItem("isLink").Value == "true" ? "<a target='_parent' href='D_Link.aspx?targetdoc=" + targetdoc + "&targetptr=" + targetptr + "'>"+node.InnerText+"</a>" : node.InnerText;
						td.Attributes["class"] = "taxonomyLink";
					}
					else
					{
						tmp = node.InnerText;
						td.Width = "20%";
						td.NoWrap = false;
						td.Attributes["class"] = "taxonamyValueColumn1";
						td.VAlign="top";
					}
					td.InnerHtml = tmp + "&nbsp;&nbsp;&nbsp;&nbsp;";
					tr.Controls.Add(td);
				}
			}
			
			return tr;
		}

		private void HelpImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			jsLabel.Text = "<script>goTo();</script>";
			jsLabel.Visible = true;
		}
	}
}
