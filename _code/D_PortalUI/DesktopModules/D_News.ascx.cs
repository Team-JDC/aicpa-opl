namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;
	using System.Text.RegularExpressions;

	using AICPA.Destroyer.Shared;
	using AICPA.Destroyer.User;
	using AICPA.Destroyer.Content.Subscription;
	using AICPA.Destroyer.Content;
	using AICPA.Destroyer.Content.Site;
	using AICPA.Destroyer.Content.Book;
	using AICPA.Destroyer.Content.Document;


	/// <summary>
	///		Summary description for D_news.
	/// </summary>
	public partial class D_news : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{
		protected Telerik.WebControls.RadRotator RadRotator2;
		protected Telerik.WebControls.RadRotator rotator0;
		protected Telerik.WebControls.RadRotator RadRotator1;
		protected System.Web.UI.HtmlControls.HtmlTable storeLinksTable;
		protected System.Web.UI.WebControls.Label spacer2;

		private string WHATS_NEW_FILE = HttpContext.Current.Server.MapPath("xmlFiles/homePage/whatsNew.xml");
		private string TandT_FILE = HttpContext.Current.Server.MapPath("xmlFiles/homePage/tipsAndTechniques.xml");
		private string STORE_FILE = HttpContext.Current.Server.MapPath("xmlFiles/homePage/storeInfo.xml");

		private const string NEWS_ROOT = "/news";
		private const string NEWS_ITEM = "NewsItem";
		private const string NEWS_DATE = "NewsDate";
		private const string NEWS_TEXT = "NewsText";
		private const string SUBSCRIPTION = "subscription";
		private const string POSITION = "position";
		private const string TOP_ITEM = "top";
		private const string TT_ROOT = "/tipsAndTechniques";
		private const string TT_ITEM = "ttValue";
		private const string TT_TEXT = "ttText";
		private const string TT_ANCHOR = "anchor";
		private const string TT_IMG_TOP = "ttImgTop";
		private const string TT_IMG_BTM = "ttImgBtm";
		private const string STORE_ROOT = "/store";
		private const string PRODUCT = "Product";
		private const string PRODUCT_GROUP = "ProductGroup";
		private const string WHATS_NEW = "whatsnew_";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			IUser user = DestroyerUi.GetCurrentUser(this.Page);
			
			string[] subscriptionCodes = user.UserSecurity.Domain.Split(DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR);
			this.load_whatsNew(subscriptionCodes);
			this.load_tipsAndTechniques();

			if(user.ReferringSiteValue == ReferringSite.Exams)
			{
				removeStore.Visible = false;
				removeWhatsNew.Visible = false;
			}
			
			//this.load_storeLinks();
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
			this.storeMain.ItemClicked += new Telerik.WebControls.RadRotator.RadRotatorClickEventHandler(storeMain_ItemClicked);
		}
		#endregion

		private void load_whatsNew(string[] subscritionsAccess)
		{
			
			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			IBookCollection books = site.Books;
/* jjs 10/16: why is this here? seems to cause gaps in the whats new background image
			bool topFlag = true;
*/
			//variable unused, removed from logic djf 1/19/10
            //bool wn_visible = false;
			string bookAnchor = string.Empty;
			string REMOVE_WHATS_NEW_STRING = "What.s New in ";
            //variable unused, removed from logic djf 1/19/10
            //string REMOVE_ATT_WN = "whatsnew_att";

			/*if(books.Count < 1)
			{
				wn_visible = false;
			}
			else
			{*/
			foreach(Book book in books)
			{

				if(book.Name.IndexOf(WHATS_NEW) > -1)
				{
					//wn_visible = true;
					bookAnchor = string.Empty;
					HtmlTableRow tr = new HtmlTableRow();
					HtmlTableCell td = new HtmlTableCell();
					td.Attributes.Add("style", "padding-top:5px;");

					if(book.Documents.Count > 0)
					{
						IDocument firstDoc = book.Documents[0];
						string bookTitle = Regex.Replace(book.Title,REMOVE_WHATS_NEW_STRING,string.Empty,RegexOptions.IgnoreCase);

						if(firstDoc != null/* && book.Name != REMOVE_ATT_WN */) // jjs 12/13/06: lois wants att back in the whatsnew list
						{
							bookAnchor = "<img src='images/portal/gray_arrow.gif'><A onclick='showWaiting(false);' href='D_Link.aspx?targetdoc=" + book.Name + "&targetptr=" + book.Documents[0].Name + "' class='news'>" + bookTitle + "</A>";
						}
					}
			
					if(bookAnchor != string.Empty)
					{
						td.InnerHtml = bookAnchor;
						tr.Controls.Add(td);
						whatsNewTable.Controls.Add(tr);
					}
				}
			}
			//}

			/*if(!whatsNewTable.HasControls())
			{
				wn_visible = whatsNewTable.HasControls();
			}*/
			this.removeWhatsNew.Visible = whatsNewTable.HasControls();
		}


		private void load_tipsAndTechniques()
		{
			//Data tables
			DataTable tableTop = new DataTable();
			DataTable tableBottom = new DataTable();
			DataRow topRow;
			DataRow bottomRow;
			string anchor = string.Empty;

			tableTop.Columns.Add(new DataColumn("ttImgTop",typeof(string)));
			tableTop.Columns.Add(new DataColumn("ttItem",typeof(string)));
			tableTop.Columns.Add(new DataColumn("ttImgBtn",typeof(string)));

			tableBottom.Columns.Add(new DataColumn("ttImgTop",typeof(string)));
			tableBottom.Columns.Add(new DataColumn("ttItem",typeof(string)));
			tableBottom.Columns.Add(new DataColumn("ttImgBtn",typeof(string)));
			
			//xml file handler
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(TandT_FILE);
			XmlNode root = xmlDoc.SelectSingleNode(TT_ROOT);

			if(root.HasChildNodes)
			{
				XmlNodeList ttNodes = root.ChildNodes;
				foreach(XmlNode ttNode in ttNodes)
				{
					anchor = ttNode.Attributes.GetNamedItem(TT_ANCHOR).Value;
					if(ttNode.LocalName == TT_ITEM)
					{
						if(ttNode.Attributes.GetNamedItem(POSITION).Value == TOP_ITEM)
						{
							topRow = tableTop.NewRow();
							topRow[0] = ttNode[TT_IMG_TOP].InnerText != "" ? "<img src='" + ttNode[TT_IMG_TOP].InnerText + "' style='border:solid 1px #cecece'>" : "";
							topRow[1] = anchor == "" ? ttNode[TT_TEXT].InnerText : "<span style='cursor:hand;cursor:pointer' onClick='tipsAndTech(\""+anchor+"\");'>"+ttNode[TT_TEXT].InnerText+"</span>";
							topRow[2] = ttNode[TT_IMG_BTM].InnerText != "" ? "<img src=' " + ttNode[TT_IMG_BTM].InnerText + "' style='border:solid 1px #cecece'>" : "";
							tableTop.Rows.Add(topRow);
						}
						else
						{
							bottomRow = tableBottom.NewRow();
							bottomRow[0] = ttNode[TT_IMG_TOP].InnerText != "" ? "<img src='" + ttNode[TT_IMG_TOP].InnerText + "' style='border:solid 1px #cecece'>" : "";
							bottomRow[1] = anchor == "" ? ttNode[TT_TEXT].InnerText : "<span style='cursor:hand;cursor:pointer' onClick='tipsAndTech(\""+anchor+"\");'>"+ttNode[TT_TEXT].InnerText+"</span>";
							bottomRow[2] = ttNode[TT_IMG_BTM].InnerText != "" ? "<img src=' " + ttNode[TT_IMG_BTM].InnerText + "' style='border:solid 1px #cecece'>" : "";
							tableBottom.Rows.Add(bottomRow);
						}
					}
				}
			}
			RadRotator3.DataSource = tableTop;
			RadRotator3.DataBind();
			RadRotator4.DataSource = tableBottom;
			RadRotator4.DataBind();
		}


		private void load_storeLinks()
		{
			//xml file handler
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(STORE_FILE);
			XmlNode root = xmlDoc.SelectSingleNode(STORE_ROOT);

			if(root.HasChildNodes)
			{
				XmlNodeList storeNodeList = root.ChildNodes;
				foreach(XmlNode storeNode in storeNodeList)
				{
					HtmlTableRow storeTR = new HtmlTableRow();
					HtmlTableCell storeTD = new HtmlTableCell();

					if(storeNode.LocalName == PRODUCT)
					{
						storeTD.InnerHtml = "<li class='storeTxt'><span onClick='openStore(\""+storeNode.Attributes.GetNamedItem("url").Value+"\");'>"+storeNode.Attributes.GetNamedItem("linktext").Value+"</span></li>";
					}

					if(storeNode.LocalName == PRODUCT_GROUP)
					{
						storeTD.Controls.Add(this.storeNewFeatures(storeNode));
					}
					storeTR.Controls.Add(storeTD);
					storeLinksTable.Controls.Add(storeTR);
				}
			}
		}


		private HtmlTable storeNewFeatures(XmlNode storeNode)
		{
			HtmlTable newFeatures = new HtmlTable();
			HtmlTableRow headerTR = new HtmlTableRow();
			HtmlTableCell headerTD = new HtmlTableCell();

			headerTD.Attributes["class"] = "storeFeature";
			headerTD.InnerHtml = "<img src='images/portal/new.gif'>&nbsp;&nbsp;<b>"+storeNode.Attributes.GetNamedItem("title").Value+"</b><img src='images/portal/new.gif'>";
			headerTD.Align = "left";
			headerTR.Controls.Add(headerTD);
			newFeatures.Controls.Add(headerTR);

			if(storeNode.HasChildNodes)
			{
				XmlNodeList nodes = storeNode.ChildNodes;
				foreach(XmlNode node in nodes)
				{
					HtmlTableRow storeTR = new HtmlTableRow();
					HtmlTableCell storeTD = new HtmlTableCell();

					if(node.LocalName == "Product")
					{
						storeTD.InnerHtml = "<li class='storeTxt'><span onClick='openStore(\""+node.Attributes.GetNamedItem("url").Value+"\");'>"+node.Attributes.GetNamedItem("linktext").Value+"</span></li>";
					}

					storeTR.Controls.Add(storeTD);
					newFeatures.Controls.Add(storeTR);
				}
			}

			return newFeatures;
		}

		private void storeMain_ItemClicked(object o, Telerik.WebControls.RadRotatorClickEventArgs e)
		{
			Telerik.WebControls.RadRotatorFrame frame = e.Item;

			//xml file handler
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(STORE_FILE);
			XmlNode root = xmlDoc.SelectSingleNode(STORE_ROOT);

			int xmlProductNode = frame.ItemIndex + 1;
			string xPath = "child::product["+xmlProductNode+"]/child::url";
			string storeUrl = root.SelectSingleNode(xPath).InnerText;
			this.jsLabel.Text = "<script>openStore('" + storeUrl + "');</script>";
			this.jsLabel.Visible = true;
		}

	}
}
