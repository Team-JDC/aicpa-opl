namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
	using System;
	using System.IO;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.Configuration;

	using AICPA.Destroyer.Shared;
	using AICPA.Destroyer.User;
	using AICPA.Destroyer.Content.Subscription;
	using AICPA.Destroyer.Content;
	using AICPA.Destroyer.Content.Site;
	using AICPA.Destroyer.Content.Book;
	using AICPA.Destroyer.Content.Document;

	/// <summary>
	///		Summary description for D_MySubscriptions.
	/// </summary>
	public partial class D_MySubscriptions : AICPA.Destroyer.UI.Portal.PortalModuleControl
	{

		private HtmlTable eMapTable = new HtmlTable();
		private HtmlTable aicpaTable = new HtmlTable();
		private Hashtable bookSubscription = new Hashtable();

		private const string RESOURCE = "resource";
		private const string WHATS_NEW = "whatsnew_";
		private const string NO_DESCRIPTION_AVAILABLE = "No Description Available";
        // warning indicates unused djf 1/15/10

		//private bool bnonTrialAbm = false;
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ISite site = DestroyerUi.GetCurrentSite(this.Page);
			IBookCollection books = site.Books;
			IUser user = DestroyerUi.GetCurrentUser(this.Page);
			string nonTrialAbm = user.UserSecurity.Domain.ToString();

            //Unused code djf 1/15/10
            //if (nonTrialAbm.IndexOf(DestroyerUi.BOOK_PREFIX_ABM) > -1)
            //{
            //    bnonTrialAbm = true;
            //}

			//Arrays used for Subscriptions
			ArrayList eMapBooksArray = new ArrayList();
			ArrayList reSourceBooksArray = new ArrayList();
			ArrayList AbmBooksArray = new ArrayList();
			//Arrays for rolling up groups of books
            ArrayList FasbRollupArray = new ArrayList();
			ArrayList GasbRollupArray = new ArrayList();
			ArrayList ArchRollupArray = new ArrayList();
			ArrayList AagRollupArray = new ArrayList();
			ArrayList ChkRollupArray = new ArrayList();

 
			string[] subscriptionCodes = user.UserSecurity.Domain.Split(DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR);
			int detailId = Request.QueryString["bookId"] != "" && Request.QueryString["bookId"] != null ? Convert.ToInt32(Request.QueryString["bookId"]) : 0;
			string detailType = Request.QueryString["type"] != "" && Request.QueryString["type"] != null ? Request.QueryString["type"] : "";

			if(subscriptionCodes.Length == 1)
			{
				if(subscriptionCodes[0] == DestroyerUi.BOOK_PREFIX_EMAP)
				{
					copyright.Visible = false;
				}
			}

			//set the copyright text
			copyright.Text = "<span style='cursor:hand;cursor:pointer;' onClick='copyRightGoTo();'>Copyrights and other Notices</span>";

			//loading Hastable with book subscriptions
			foreach(string subscriptionCode in subscriptionCodes)
			{
				ISubscription subscription = new Subscription(subscriptionCode);
				foreach(string bookName in subscription.BookNames)
				{
					bookSubscription[bookName] = bookSubscription[bookName] == null ? subscription.Title.ToString() : bookSubscription[bookName] + "," + subscription.Title.ToString();
				}
			}

			//loading eMap and reSource collections
			foreach(Book book in books)
			{
				if(book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_EMAP) > -1)
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1)
					{
						eMapBooksArray.Add(book);
					}
				}
				else if ((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FASB) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FAF) > -1 ) || (book.Name.IndexOf("asc") > -1 ) )
				{
					//if(book.Name.IndexOf(WHATS_NEW) == -1)  we want to add the FAF whats new to the home page. 
					//{
						//FasbRollupArray.Add(book);
						reSourceBooksArray.Add(book);
					//}
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_GASB) > -1)
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1)
					{
						//GasbRollupArray.Add(book);
						reSourceBooksArray.Add(book);
					}
				}
				else if ((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_AAG) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ARA) > -1))
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1  && book.Name != "aag")
					{
						//AagRollupArray.Add(book);
						reSourceBooksArray.Add(book);
					}
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_CHK) > -1)
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1 && book.Name != "chk-intro")
					{
						//ChkRollupArray.Add(book);
						reSourceBooksArray.Add(book);
					}
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ABM) > -1)
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1)
					{
						AbmBooksArray.Add(book);
					}
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ATT) > -1)
				{
					if((book.Name.IndexOf(WHATS_NEW) == -1) && (book.Name.Length < 4))
					{
						reSourceBooksArray.Add(book);
					}
				}
				else
				{
					if(book.Name.IndexOf(WHATS_NEW) == -1 && book.Name != "aag" && book.Name != "chk-intro")
					{
						reSourceBooksArray.Add(book);
					}
					
				}
			}
			
			if(AbmBooksArray.Count > 0)
			{
				HtmlTableRow AbmTR = new HtmlTableRow();
				HtmlTableCell AbmTD = new HtmlTableCell();
				AbmTD.Align = "center";
				AbmTD.Controls.Add(this.groupTable(AbmBooksArray,2,0));
				AbmTR.Controls.Add(AbmTD);
				MySubscriptionsTable.Controls.Add(AbmTR);
			}
			if(eMapBooksArray.Count > 0)
			{
				HtmlTableRow eMapTR = new HtmlTableRow();
				HtmlTableCell eMapTD = new HtmlTableCell();
				eMapTD.Align = "center";
				eMapTD.Controls.Add(this.groupTable(eMapBooksArray,1,0));
				eMapTR.Controls.Add(eMapTD);
				MySubscriptionsTable.Controls.Add(eMapTR);
			}


			if(reSourceBooksArray.Count > 0)
			{
				HtmlTableRow reSourceTR = new HtmlTableRow();
				HtmlTableCell reSourceTD = new HtmlTableCell();
				reSourceTD.Align = "center";
				reSourceTD.Controls.Add(this.groupTable(reSourceBooksArray,0,0));
				reSourceTR.Controls.Add(reSourceTD);
				MySubscriptionsTable.Controls.Add(reSourceTR);
			}

			
		}

		//Creats the header wrap on ths subscription header
		private HtmlTableRow imgTableHeader()
		{
			HtmlTableRow imgHeaderTR = new HtmlTableRow();
			HtmlTableCell imgHeaderTD1 = new HtmlTableCell();
			HtmlTableCell imgHeaderTD2 = new HtmlTableCell();
			HtmlTableCell imgHeaderTD3 = new HtmlTableCell();

			imgHeaderTD1.Attributes.Add("style","background-image:url(images/portal/backgrounds/cornerLeft.gif);background-repeat:repeat-y;");
			imgHeaderTD1.Width = "22px";

			imgHeaderTD2.Attributes.Add("style","background-image:url(images/portal/backgrounds/topBg.gif);background-repeat:repeat-x;background-position: right top;");

			imgHeaderTD3.Attributes.Add("style","background-image:url(images/portal/backgrounds/cornerRight.gif);background-repeat:repeat-y;background-position: right top;");
			imgHeaderTD3.Width = "22px";
			imgHeaderTD3.Height = "22px";
			imgHeaderTD3.VAlign = "top";

			imgHeaderTR.Controls.Add(imgHeaderTD1);
			imgHeaderTR.Controls.Add(imgHeaderTD2);
			imgHeaderTR.Controls.Add(imgHeaderTD3);

			return imgHeaderTR;
		}

		//Creates the Footer for the subscriptions box
		private HtmlTableRow imgTableFooter()
		{
			HtmlTableRow imgFooterTR = new HtmlTableRow();
			HtmlTableCell imgFooterTD1 = new HtmlTableCell();
			HtmlTableCell imgFooterTD2 = new HtmlTableCell();
			HtmlTableCell imgFooterTD3 = new HtmlTableCell();
			
			imgFooterTD1.Attributes.Add("style","background-image:url(images/portal/backgrounds/cornerBottomLeft.gif);background-repeat:repeat-y;");
			imgFooterTD1.Width = "22px";

			imgFooterTD2.Attributes.Add("style","background-image:url(images/portal/backgrounds/bottomBg.gif);background-repeat:repeat-x;background-position: right bottom;");

			imgFooterTD3.Attributes.Add("style","background-image:url(images/portal/backgrounds/cornerBottomRight.gif);background-repeat:repeat;");
			imgFooterTD3.Width = "22px";
			imgFooterTD3.Height = "22px";
			imgFooterTD3.VAlign = "top";

			imgFooterTR.Controls.Add(imgFooterTD1);
			imgFooterTR.Controls.Add(imgFooterTD2);
			imgFooterTR.Controls.Add(imgFooterTD3);

			return imgFooterTR;
		}

		private HtmlTable groupTable(ArrayList books, int group, int expandedBookId)
		{
			HtmlTable imgTb = new HtmlTable();
			HtmlTableRow imgDataTR = new HtmlTableRow();
			HtmlTableCell imgDataTD1 = new HtmlTableCell();
			HtmlTableCell imgDataTD2 = new HtmlTableCell();
			HtmlTableCell imgDataTD3 = new HtmlTableCell();
			
			HtmlTable groupTb = new HtmlTable();
			HtmlTableRow groupHeader = new HtmlTableRow();
			HtmlTableRow groupData = new HtmlTableRow();
			HtmlTableCell headerTD1 = new HtmlTableCell();
			HtmlTableCell headerTD2 = new HtmlTableCell();
			HtmlTableCell dataTD1 = new HtmlTableCell();

			    
			string imgSrc = "";
			string titleStr = "";
			
			if (group == 0)
			{
				imgSrc = "images/ReSOURCE_logo.gif";
				titleStr = "AICPA Resource Library";
			}
			else if (group == 1)
			{
				imgSrc = "images/emap.gif";
				titleStr = "eMap Library";

			}
			else 
			{
				imgSrc = "images/ABM.gif";
				titleStr = "Accountant's Business Manual";
			}	
			
			groupTb.Border = 0;
			groupTb.Width = "100%";
			groupTb.CellSpacing = 0;
			groupTb.CellPadding = 0;

			headerTD1.InnerHtml = "<font color='#5e5e5f' size=+1><b>" + titleStr + "</b></font><hr style='color:darkred;height:1pt;'>";
			headerTD1.VAlign = "top";
			headerTD2.VAlign = "top";
			headerTD2.Align = "right";
			headerTD2.Width = "2%";
			headerTD2.NoWrap = true;
			headerTD2.Attributes.Add("style","border-bottom:solid 1pt darkred");
			headerTD2.InnerHtml = "<img src='" + imgSrc + "'>";

			groupHeader.Controls.Add(headerTD1);
			groupHeader.Controls.Add(headerTD2);

			dataTD1.ColSpan = 2;
			dataTD1.Controls.Add(booksTable(books,bookSubscription,expandedBookId));				
					
			groupData.Controls.Add(dataTD1);

			groupTb.Controls.Add(groupHeader);
			groupTb.Controls.Add(groupData);

			imgDataTD1.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgLeft.gif);background-repeat:repeat;");
			imgDataTD1.Width = "16px";
			imgDataTD3.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgRight.gif);background-repeat:repeat;");
			imgDataTD3.Width = "22px";
			imgDataTD2.Controls.Add(groupTb);

			imgDataTR.Controls.Add(imgDataTD1);
			imgDataTR.Controls.Add(imgDataTD2);
			imgDataTR.Controls.Add(imgDataTD3);

			imgTb.Border = 0;
			imgTb.CellSpacing = 0;
			imgTb.CellPadding = 0;
			imgTb.Width = "95%";
			imgTb.Controls.Add(this.imgTableHeader());
			imgTb.Controls.Add(imgDataTR);
			imgTb.Controls.Add(this.imgTableFooter());
			
			return imgTb;
			
		}


		private HtmlTable booksTable(ArrayList books,Hashtable subscriptions, int expandedBookId)
		{
			//Create Tables
			HtmlTable booksTable = new HtmlTable();
			HtmlTable AAGTable = new HtmlTable();
			HtmlTable INAAGTable = new HtmlTable();
			HtmlTable CHKTable = new HtmlTable();
			HtmlTable EMAPTable = new HtmlTable();
			HtmlTable FASBTable = new HtmlTable();
			HtmlTable ARCHTable = new HtmlTable();
			HtmlTable GASBTable = new HtmlTable();
			//Rows
			HtmlTableRow AAGRow  = new ExtTableRow();
			HtmlTableRow CHKRow  = new ExtTableRow();
			HtmlTableRow EMAPRow  = new ExtTableRow();
			HtmlTableRow FASBRow  = new ExtTableRow();
			HtmlTableRow ARCHRow  = new ExtTableRow();
			HtmlTableRow GASBRow  = new ExtTableRow();
			//Bool Switchs
			bool bAAG = false;
			bool bINAAG  = false;
			bool bCHK = false;
			bool bEMAP = false;
			bool bFASB = false;
			bool bGASB = false;
			bool bARCH = false;

			//bool Switches for adding commands one time 
			//ADD FAF CROSS REF
			bool bFAFXREF = true; 
			bool bFAFGOTO = true;

			//int Switchs
			int iAAGExpand = 0;
			int iCHKExpand = 0;
			int iExpanded = 0;

			//string  sExpanded = System.Configuration.ConfigurationSettings.AppSettings[DestroyerUi.ROLLUP_COUNT];
			//string sInactive = System.Configuration.ConfigurationSettings.AppSettings["InactiveAAGs"];
            //modified 1/20/10 djf
            string sExpanded = ConfigurationManager.AppSettings[DestroyerUi.ROLLUP_COUNT];
            string sInactive = ConfigurationManager.AppSettings["InactiveAAGs"];
	   
			if (sExpanded == null)
			{
				iExpanded = 2;
			}
			else 
			{
				iExpanded = Convert.ToInt32(sExpanded);
			}


			//Set Props and Book Table
			booksTable.Border = 0;
			booksTable.CellSpacing = 0;
			booksTable.CellPadding = 3;
			booksTable.Width = "100%";
			

			foreach(Book book in books)
			{
				if(book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_EMAP) > -1)
				{
					bEMAP = true;
					EMAPTable.Width = "100%";
					EMAPTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}
					//else if ((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FASB) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FAF) > -1))
				
				
				else if (book.Name.IndexOf("asc") > -1)
				{
					FASBTable.Controls.AddAt(0,this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FAF) > -1)
				{
					bFASB = true;
					FASBTable.Width = "100%";
					//We want to add a command at the top of the table
					
					if(bFAFXREF)
					{
						FASBTable.Controls.Add(this.commandLinkLaunchTable(book,"13%", "images/crossRef.gif", "4","","Cross Reference"));
						bFAFXREF = false;
					}
					if(bFAFGOTO)
					{
						FASBTable.Controls.Add(this.commandLinkLaunchTable(book,"13%", "images/goto.jpg", "1","","Go To"));
						bFAFGOTO = false;
					}
					FASBTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
					
				}
				else if ((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_FASB) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ARCH) > -1)) 
				{
					bARCH = true;
					ARCHTable.Width = "100%";
					ARCHTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_GASB) > -1)
				{
					bGASB = true;
					GASBTable.Width = "100%";
					GASBTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}	
				else if ((sInactive.IndexOf(book.Name) > -1 )&&((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_AAG) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ARA) > -1)))
				{
					
					INAAGTable.Width = "100%";
					//add an information row
					if (!bINAAG)
					{	HtmlTableRow inactiveTR = new HtmlTableRow();
						HtmlTableCell  titleInfoImageTD  =  new HtmlTableCell();
						titleInfoImageTD.Width ="15%";;
						titleInfoImageTD.NoWrap = true;
						titleInfoImageTD.Align = "right";
						titleInfoImageTD.VAlign = "top";
						titleInfoImageTD.InnerHtml = "<a name='inactive'><img src='images/books.jpg'></a>&nbsp;&nbsp;";
						
						HtmlTableCell  infoTD  =  new HtmlTableCell();
						infoTD.Width ="85%";;
						infoTD.NoWrap = true;
						infoTD.Align = "left";
						infoTD.VAlign = "top";
						infoTD.InnerHtml = "<a name='inactivetitle' class='booksTitle'>Inactive Audit and Accounting Guides</a>&nbsp;&nbsp;";
		
						inactiveTR.Controls.Add(titleInfoImageTD);
						inactiveTR.Controls.Add(infoTD);

						INAAGTable.Controls.Add(inactiveTR);
					}
					bINAAG  = true;
					INAAGTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));	
				}
				else if ((book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_AAG) > -1) || (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ARA) > -1))
				{
					iAAGExpand++;
					bAAG = true;
					AAGTable.Width = "100%";
					AAGTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_CHK) > -1)
				{
					iCHKExpand++;
					bCHK = true;
					CHKTable.Width = "100%";
					CHKTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"15%"));
				}
				else if (book.Name.IndexOf(DestroyerUi.BOOK_PREFIX_ABM) > -1)
				{
					booksTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"2%"));
				}
				else
				{
					booksTable.Controls.Add(this.bookTitleDetailTable(book,subscriptions[book.Name].ToString(),"2%"));
					
				}
				
			}
			if (bEMAP)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_EMAP + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_EMAP;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				rollupTR.Attributes.Add("class","hideBookRollup");
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				//add the Emap Table to the Cell
				rollupTC.Controls.Add(EMAPTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);

			}
			if (bAAG)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_AAG + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_AAG;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				//
				if (iAAGExpand > iExpanded)
				{
					rollupTR.Attributes.Add("class","hideBookRollup");
				}
				else
				{
					rollupTR.Attributes.Add("class","showBookRollup");
				}
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				//add the AAG Table to the Cell
				rollupTC.Controls.Add(AAGTable);
				//If there are inactive titles then addd the inactive table to the cell
				rollupTC.Controls.Add(INAAGTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);
			}
			if (bCHK)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_CHK + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_CHK;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				if (iCHKExpand > iExpanded)
				{
					rollupTR.Attributes.Add("class","hideBookRollup");
				}
				else
				{
					rollupTR.Attributes.Add("class","showBookRollup");
				}
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				//add the Emap Table to the Cell
				rollupTC.Controls.Add(CHKTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);
			}
			if (bFASB)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_FASB + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_FASB;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				rollupTR.Attributes.Add("class","hideBookRollup");
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				//add the Emap Table to the Cell
				rollupTC.Controls.Add(FASBTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);
			}
			if (bGASB)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_GASB + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_GASB;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				rollupTR.Attributes.Add("class","hideBookRollup");
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				//add the Emap Table to the Cell
				rollupTC.Controls.Add(GASBTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);
			}
			if (bARCH)
			{
				//Create The Summary Row and add it to the booktable
				string rowid = DestroyerUi.BOOK_PREFIX_ARCH + "_rollup";
				string Title = DestroyerUi.ROLLUP_TITLE_ARCH;
				booksTable.Controls.Add(this.bookRollupTable(Title,rowid));
				//Create a row and and cell
				HtmlTableRow rollupTR = new ExtTableRow();
				rollupTR.ID = rowid;
				rollupTR.Attributes.Add("class","hideBookRollup");
				HtmlTableCell rollupTC = new HtmlTableCell();	
				rollupTC.ColSpan = 3;
				rollupTC.Controls.Add(ARCHTable);
				//add the cell to the row
				rollupTR.Controls.Add(rollupTC);
				//Add the Row to the Table
				booksTable.Controls.Add(rollupTR);
			}

			return booksTable;
		}

		private HtmlTableRow bookRollupTable(string Title, string row)
		{
			HtmlTableRow rollupTR = new HtmlTableRow();
			HtmlTableCell rollupImageTD = new HtmlTableCell();
			HtmlTableCell rollupTitleTD = new HtmlTableCell();
			HtmlTableCell rollupLinkTD = new HtmlTableCell();
			
			
			rollupImageTD.Width = "2%";
			rollupImageTD.NoWrap = true;
			rollupImageTD.Align = "right";
			rollupImageTD.VAlign = "top";
			rollupImageTD.InnerHtml = "<img src='images/books.jpg'>&nbsp;&nbsp;";
			
			rollupTitleTD.Attributes.Add("style","color:darkblue;font-weight:bold;");
			rollupTitleTD.InnerHtml = "<span onClick=\"showPubs('" + row +"');\" class='rollupTitle' title='Click to view all publications'>" +  Title + "</span>";
			rollupLinkTD.VAlign = "top";

			rollupLinkTD.Align = "right";
			rollupLinkTD.Width = "2%";
			rollupLinkTD.NoWrap = true;
			rollupLinkTD.VAlign = "top";
			rollupLinkTD.InnerHtml = "<span onClick=\"showPubs('" + row +"');\" class='viewAll' title='Click to view all publications'>&nbsp;View All &nbsp;<img id='img_"+ row +"' src='images/topicbar_down.gif'>&nbsp;</span>";

			rollupTR.Controls.Add(rollupImageTD);
			rollupTR.Controls.Add(rollupTitleTD);
			rollupTR.Controls.Add(rollupLinkTD);
			
			return rollupTR;

		}



		private HtmlTableRow bookSummaryTable(string group, string groupTitle)
		{
			HtmlTableRow titleTR = new HtmlTableRow();
			HtmlTableCell titleBookImageTD = new HtmlTableCell();
			HtmlTableCell titleTD1 = new HtmlTableCell();
			HtmlTableCell titleTD2 = new HtmlTableCell();
			
			
			//string scrollAnchor = book.Id+"_"+book.Name;
			string groupId = group + "_row";
			string scrollAnchor = group;
			string spanId = "span_" + group;

			
			titleBookImageTD.Width = "2%";
			titleBookImageTD.NoWrap = true;
			titleBookImageTD.Align = "left";
			titleBookImageTD.VAlign = "top";
			titleBookImageTD.InnerHtml = "<a name='"+scrollAnchor+"'><img src='images/books.gif'></a>&nbsp;&nbsp;";

			
			titleTD1.Attributes.Add("style","color:darkblue;font-weight:bold;");
			titleTD1.InnerHtml = groupTitle;
			titleTD1.VAlign = "top";

			titleTD2.Align = "right";
			titleTD2.Width = "2%";
			titleTD2.NoWrap = true;
			titleTD2.VAlign = "top";
			titleTD2.InnerHtml = "<span onClick=\"showPubs('" + groupId +"');\" class='viewAll' title='Click to view all publications' id='" + spanId + "'>&nbsp;View All &nbsp;<img src='images/topicbar_down.gif'>&nbsp;</span>";

			titleTR.Controls.Add(titleBookImageTD);
			titleTR.Controls.Add(titleTD1);
			titleTR.Controls.Add(titleTD2);
			
			return titleTR;

		}
		
		
		
		private HtmlTableRow commandLinkLaunchTable(Book book, string indent,string cmdImage, string cmdTab,string cmdName,string cmdLinkText)
		{
			HtmlTable titleDescTb = new HtmlTable();
			HtmlTableRow titleTR = new HtmlTableRow();
			HtmlTableCell cmdImageTD = new HtmlTableCell();
			HtmlTableCell titleTD1 = new HtmlTableCell();
			HtmlTableCell titleTD2 = new HtmlTableCell();
			string 	bookAnchor = "";
			string scrollAnchor = book.Id+"_"+book.Name;

			cmdImageTD.Width = indent;
			cmdImageTD.NoWrap = true;
			cmdImageTD.Align = "right";
			cmdImageTD.VAlign = "top";
			//WGD 06/09
			//TODO LATER NECESSARY - GENERALIZE HOW COMMAND IMAGES ARE DEFINED
			// Create a image link to put in a TD
			cmdImageTD.InnerHtml = "<a name='"+scrollAnchor+"'><img src='" + cmdImage + "'></a>&nbsp;&nbsp;";
			if(book.Documents.Count > 0)
			{
				IDocument firstDoc = book.Documents[0];
				if(firstDoc != null)
				{
					//TODO LATER IF NECESSARY - GENERALIZE THE D_LINK QUERY STRING PARAMETERS
					bookAnchor = "<A onclick='showWaiting(false);' href='D_Link.aspx?targetdoc=" + book.Name + "&targetptr=" + book.Documents[0].Name + "&targettab=" + cmdTab + "&targetcmd=" + cmdName + "' class='booksTitle'>" + cmdLinkText + "</A>";
				}
			}

			titleDescTb.Width = "100%";
			titleDescTb.CellSpacing = 0;
			titleDescTb.CellPadding = 0;
			titleDescTb.Border = 0;
			
            //Cread the information for the book text and link in a TD 
			titleTD1.Attributes.Add("style","color:darkblue;font-weight:bold;");
			titleTD1.ColSpan = 2;
			titleTD1.InnerHtml = bookAnchor;
			
			
			//TO DO LATER IF YOU WOULD LIKE TO ADD A DISCRIPTION TO THE COMMAND, NOTE PASS IN THE DESCRIPTION
			/* titleTD2.Align = "right";
			titleTD2.Width = "2%";
			titleTD2.NoWrap = true;
			titleTD2.InnerHtml = "<span onClick=\"showDesc('" +book.Id +"');\" class='booksDetail' title='Click here to hide the book details'>&nbsp;Details &nbsp;<img src='images/topicbar_down.gif' id='img_"+ book.Id + "' >&nbsp;</span>";*/

			//Add the image TD
			titleTR.Controls.Add(cmdImageTD);
			//Add the CMD info TD
			titleTR.Controls.Add(titleTD1);

			return titleTR;
		}
		
		
		private HtmlTableRow bookTitleDetailTable(Book book, string subscriptions, string indent)
		{
			HtmlTableRow returnTR = new HtmlTableRow();
			HtmlTableCell returnTD = new HtmlTableCell();
			HtmlTable titleDescTb = new HtmlTable();
			HtmlTableRow titleTR = new HtmlTableRow();
			HtmlTableRow descTR = new ExtTableRow();
			HtmlTableCell titleBookImageTD = new HtmlTableCell();
			HtmlTableCell titleTD1 = new HtmlTableCell();
			HtmlTableCell titleTD2 = new HtmlTableCell();
			HtmlTableCell descTD1 = new HtmlTableCell();
			LinkButton detailsLink = new LinkButton();
			ImageButton detailImg = new ImageButton();
			string 	bookAnchor = "";
			string scrollAnchor = book.Id+"_"+book.Name;

			/*detailsLink.Text = "&nbsp;Details&nbsp;";
			detailsLink.Attributes.Add("onclick","showDetails('','');");
			detailsLink.Visible = true;
			detailsLink.CssClass = "booksDetail";
			detailsLink.ToolTip = "Click here to display the Book details";

			detailImg.ImageUrl = "~/images/topicbar_up.gif";
			detailImg.ID = "detailImg"+book.Id+"_"+book.Name;
			detailImg.Attributes.Add("onclick","showDetails('','');");
			detailImg.Visible = true;
			detailImg.ToolTip = "Click here to display the Book details";*/

			titleBookImageTD.Width = indent;
			titleBookImageTD.NoWrap = true;
			titleBookImageTD.Align = "right";
			titleBookImageTD.VAlign = "top";
			titleBookImageTD.InnerHtml = "<a name='"+scrollAnchor+"'><img src='images/book.gif'></a>&nbsp;&nbsp;";

			if(book.Documents.Count > 0)
			{
				IDocument firstDoc = book.Documents[0];
				if(firstDoc != null)
				{
					bookAnchor = "<A onclick='showWaiting(false);' href='D_Link.aspx?targetdoc=" + book.Name + "&targetptr=" + book.Documents[0].Name + "'  class='booksTitle'>" + book.Title + "</A>";
				}
			}


			titleDescTb.Width = "100%";
			titleDescTb.CellSpacing = 0;
			titleDescTb.CellPadding = 0;
			titleDescTb.Border = 0;

			titleTD1.Attributes.Add("style","color:darkblue;font-weight:bold;");
			titleTD1.InnerHtml = bookAnchor;

			//titleTD2.BgColor = "#f6f6f6";
			titleTD2.Align = "right";
			titleTD2.Width = "2%";
			titleTD2.NoWrap = true;
			//titleTD2.Attributes.Add("style","border-top:solid 1px #dedede;border-left:solid 1px #dedede;border-right:solid 1px #dedede;");
			//titleTD2.Controls.Add(detailsLink);
			//titleTD2.Controls.Add(detailImg);
			titleTD2.InnerHtml = "<span onClick=\"showDesc('" +book.Id +"');\" class='booksDetail' title='Click here to hide the book details'>&nbsp;Details &nbsp;<img src='images/topicbar_down.gif' id='img_"+ book.Id + "' >&nbsp;</span>";

			titleTR.Controls.Add(titleBookImageTD);
			titleTR.Controls.Add(titleTD1);
			titleTR.Controls.Add(titleTD2);

			descTD1.ColSpan = 3;
			descTD1.Controls.Add(this.bookDetailTable(book,subscriptions));

			//give this row an id
			//WGD
			descTR.ID =  "desc_" + book.Id;
			//descTR.Attributes.Add("name","desc_" + book.Id);
			//string temp = descTR.ClientID.ToString();
			descTR.Attributes.Add("class","hideBookDesc");
			//WGD
			descTR.Controls.Add(descTD1);

			titleDescTb.Controls.Add(titleTR);
			titleDescTb.Controls.Add(descTR);

			returnTD.ColSpan = 3;
			returnTD.Controls.Add(titleDescTb);
			returnTR.Controls.Add(returnTD);

			return returnTR;
		}


		private HtmlTable bookDetailTable(Book book, string subscriptions)
		{
			HtmlTable mainTb = new HtmlTable();
			HtmlTable subTb = new HtmlTable();
			int totalSubRows = 6;

			mainTb.CellSpacing = 0;
			mainTb.CellPadding = 0;
			mainTb.Border = 0;
			mainTb.Width = "100%";

			//loading sub table
			for(int i=0; i < totalSubRows; i++)
			{
				HtmlTableRow subTR = new HtmlTableRow();
				HtmlTableCell td1 = new HtmlTableCell();
				HtmlTableCell td2 = new HtmlTableCell();

				mainTb.Controls.Add(subTR);
				subTR.Controls.Add(td1);
				td1.BgColor = "#f6f6f6";
				td2.BgColor = "#f6f6f6";
				string bookDescription = book.Description.ToString().Length == 0 ? NO_DESCRIPTION_AVAILABLE : book.Description.ToString();
				switch(i)
				{

					case 0:
						if(bookDescription != NO_DESCRIPTION_AVAILABLE)
						{
							td1.InnerHtml = "&nbsp;<font color='#5e5e5f' face='verdana' size=-1><b>Book Description:</b></font>";
							td1.Attributes.Add("style","border-top:solid 1px #dedede;border-left:solid 1px #dedede;");
							td2.InnerHtml = "&nbsp;";
							td2.BgColor = "#f6f6f6";
							td2.Attributes.Add("style","border-right:solid 1px #dedede;");
							subTR.Controls.Add(td2);
						}
						break;
					case 1:
						if(bookDescription != NO_DESCRIPTION_AVAILABLE)
						{
							td1.InnerHtml = "&nbsp;&nbsp;<font color='#5e5e5f' face='verdana' size=-1>"+bookDescription+"</font>";
							td1.Attributes.Add("style","border-right:solid 1px #dedede;border-left:solid 1px #dedede;");
							td1.ColSpan = 2;
						}
						break;
					case 2:
						td1.InnerHtml = "&nbsp;<font color='#5e5e5f' face='verdana' size=-1><b>Publish Date:</b></font>";
						td1.Attributes.Add("style","border-right:solid 1px #dedede;border-left:solid 1px #dedede;");
						td1.ColSpan = 2;
						break;
					case 3:
						td1.InnerHtml = "&nbsp;&nbsp;<font color='#5e5e5f' face='verdana' size=-1>"+book.PublishDate.ToString()+"</font>";
						td1.Attributes.Add("style","border-right:solid 1px #dedede;border-left:solid 1px #dedede;");
						td1.ColSpan = 2;
						break;
					case 4:
						td1.InnerHtml = "&nbsp;<font color='#5e5e5f' face='verdana' size=-1><b>Subscription Name:</b></font>";
						td1.Attributes.Add("style","border-right:solid 1px #dedede;border-left:solid 1px #dedede;");
						td1.ColSpan = 2;
						break;
					case 5:
						td1.InnerHtml = "&nbsp;&nbsp;<font color='#5e5e5f' face='verdana' size=-1>"+subscriptions+"</font>";
						td1.Attributes.Add("style","border-right:solid 1px #dedede;border-left:solid 1px #dedede;border-bottom:solid 1px #dedede;");
						td1.ColSpan = 2;
						break;
				}
			}

			return mainTb;
		}


		protected void D_MySubscriptions_PreRender(object sender, System.EventArgs e)
		{
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
			this.PreRender += new System.EventHandler(this.D_MySubscriptions_PreRender);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void HelpImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			DestroyerUi.ShowHelp(this.Page, DestroyerUi.HelpTopic.MySubscriptions);
		}


		private void SubscriptionImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			ImageButton imageButton = (ImageButton)sender;
			if(imageButton.CommandName == DestroyerUi.COMMAND_SUBSCRIPTIONCLICK)
			{
				ISubscription subscription = new Subscription(imageButton.CommandArgument);
				ISite site = DestroyerUi.GetCurrentSite(this.Page);

				string firstBookName = subscription.BookNames[0];
				IBook firstBook = site.Books[firstBookName];
				IDocument firstDoc = firstBook.Documents[0];

				Session[DestroyerUi.SESSPARAM_CURRENTDOCUMENT] = firstDoc;
				
				DestroyerUi.ShowTab(this.Page, DestroyerUi.PortalTab.TocDoc, null);

			}
		}


		private void oldHomePage()
		{
			IUser user = DestroyerUi.GetCurrentUser(this.Page);
			string[] subscriptionCodes = user.UserSecurity.Domain.Split(DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR);

			ArrayList subscriptionTables = new ArrayList();
			foreach(string subscriptionCode in subscriptionCodes)
			{
				//get the subscription from the database
				ISubscription subscription = new Subscription(subscriptionCode);

				//				//create the image cell and add it to the row
				//				string imageRef = "~/images/subscription_" + subscription.Code + ".jpg";
				//				if(!File.Exists(Server.MapPath(imageRef)))
				//				{
				//					imageRef = "~/images/subscription_default.jpg";
				//				}
				//				HtmlTableCell subscriptionTableImageCell = new HtmlTableCell();
				//				subscriptionTableImageCell.VAlign = "top";
				//				subscriptionTableImageCell.Width = "50";
				//				ImageButton subscriptionImageButton = new ImageButton();
				//				subscriptionImageButton.ImageUrl = imageRef;
				//				subscriptionImageButton.CommandName = DestroyerUi.COMMAND_SUBSCRIPTIONCLICK;
				//				subscriptionImageButton.CommandArgument = subscriptionCode;
				//				subscriptionImageButton.Click += new System.Web.UI.ImageClickEventHandler(SubscriptionImageButton_Click);
				//				subscriptionTableImageCell.Controls.Add(subscriptionImageButton);
				//				subscriptionTableRow.Controls.Add(subscriptionTableImageCell);

				//create the subscription table
				HtmlTable subscriptionTable = new HtmlTable();
				subscriptionTable.Border=0;

				//create and associate all rows and cells that have to do with the subscription description
				HtmlTableRow subscriptionTableRow = new HtmlTableRow();
				subscriptionTableRow.Attributes["runat"] = "server";
				subscriptionTableRow.ID = "subscriptionDescRow_" + subscription.Code; 
				subscriptionTableRow.Attributes["class"] = "subscriptionDesc";
				subscriptionTable.Controls.Add(subscriptionTableRow);
				HtmlTableCell subscriptionTableDescCell = new HtmlTableCell();
				subscriptionTableRow.Controls.Add(subscriptionTableDescCell);

				//create and associate all rows and cells that have to do with the subscription books
				/*HtmlTableRow bookTableRow = new HtmlTableRow();
				bookTableRow.Attributes["runat"] = "server";
				bookTableRow.ID = "subscriptionBookRow_" + subscription.Code; 
				bookTableRow.Attributes["class"] = "subscriptionBookDesc";
				subscriptionTable.Controls.Add(bookTableRow);
				HtmlTableCell bookTableCell = new HtmlTableCell();
				bookTableRow.Controls.Add(bookTableCell);*/

				//-----------------------------------------------------
				HtmlTable headerTable = new HtmlTable();
				
				HtmlTableRow headerTR = new HtmlTableRow();
				HtmlTableRow headerRow = new HtmlTableRow();
				HtmlTableRow headerRow2 = new HtmlTableRow();
				HtmlTableRow headerRow3 = new HtmlTableRow();
				HtmlTableRow headerRow4 = new HtmlTableRow();
				HtmlTableRow booksRow = new HtmlTableRow();
				
				HtmlTableCell headerTD1 = new HtmlTableCell();
				HtmlTableCell headerTD2 = new HtmlTableCell();
				HtmlTableCell headerTD3 = new HtmlTableCell();
				HtmlTableCell headerCell1 = new HtmlTableCell();
				HtmlTableCell headerCell2 = new HtmlTableCell();
				HtmlTableCell headerCell3 = new HtmlTableCell();
				HtmlTableCell headerCell4 = new HtmlTableCell();
				HtmlTableCell headerCell5 = new HtmlTableCell();
				HtmlTableCell bookTD1 = new HtmlTableCell();
				HtmlTableCell bookTD2 = new HtmlTableCell();
				HtmlTableCell bookTD3 = new HtmlTableCell();

				HtmlTableCell headerLeftBG = new HtmlTableCell();
				HtmlTableCell headerRightBG = new HtmlTableCell();
				HtmlTableCell headerLeftBG2 = new HtmlTableCell();
				HtmlTableCell headerRightBG2 = new HtmlTableCell();
				HtmlTableCell headerLeftBG3 = new HtmlTableCell();
				HtmlTableCell headerRightBG3 = new HtmlTableCell();
				HtmlTableCell headerLeftBG4 = new HtmlTableCell();
				HtmlTableCell headerRightBG4 = new HtmlTableCell();


				headerTable.Border = 0;
				headerTable.Width = "100%";
				headerTable.CellSpacing = 0;
				headerTable.CellPadding = 0;
				headerTable.BgColor = "#ffffff";

				// Top Row with images
				headerTD1.Attributes.Add("style","background-image:url(images/portal/backgrounds/cornerLeft.gif);background-repeat:repeat-y;");
				headerTD1.Width="22px";
				headerTD3.Attributes.Add("style","background-image:url(images/portal/backgrounds/topBg.gif);background-repeat:repeat-y;background-position: right top;");
				headerTD3.Width="22px";
				headerTD3.Height="22px";
				headerTD3.InnerHtml = "<img src='images/portal/backgrounds/cornerRight.gif'>";
				headerTD3.VAlign="top";
				headerTD2.Attributes.Add("style","background-image:url(images/portal/backgrounds/topBg.gif);background-repeat:repeat-x;background-position: right top;");


				headerTD2.ColSpan = 2;
				/*headerTD2.InnerHtml = "&nbsp;";
				headerTD2.Attributes["class"] = "subscriptionDesc";
				headerTD2.Attributes.Add("style","border-top:solid 1px #d2d2d2");*/


				//headerTD2.Height="19px";
				
				headerTR.Controls.Add(headerTD1);
				headerTR.Controls.Add(headerTD2);
				headerTR.Controls.Add(headerTD3);

				headerTable.Controls.Add(headerTR);
				
				//Subscription title and Image row
				headerLeftBG.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgLeft.gif);background-repeat:repeat-y;");
				headerLeftBG.Width="16px";
				headerRightBG.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgRight.gif);background-repeat:repeat-y;");
				headerRightBG.Width="16px";

				headerCell1.NoWrap = true;
				headerCell1.InnerHtml = "<li class='subscriptionTitle'>&nbsp;" + subscription.Title + "</li>";
				headerCell1.VAlign = "top";
				headerCell1.ColSpan = 2;

				headerRow.Controls.Add(headerLeftBG);
				headerRow.Controls.Add(headerCell1);
				headerRow.Controls.Add(headerRightBG);
				
				headerTable.Controls.Add(headerRow);

				//Description Row
				headerLeftBG2.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgLeft.gif);background-repeat:repeat-y;");
				headerLeftBG2.Width="22px";
				headerRightBG2.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgRight.gif);background-repeat:repeat-y;");
				headerRightBG2.Width="22px";

				headerCell3.Attributes["class"] = "subscriptionDesc";
				headerCell3.InnerHtml = subscription.Description;// + "- Description test test test test tes tes ";
				headerCell3.ColSpan = 2;
				
				headerRow2.Controls.Add(headerLeftBG2);
				headerRow2.Controls.Add(headerCell3);
				headerRow2.Controls.Add(headerRightBG2);

				headerTable.Controls.Add(headerRow2);

				//Books Row
				bookTD1.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgLeft.gif);background-repeat:repeat-y;");
				bookTD1.Width="16px";
				bookTD3.Attributes.Add("style","background-image:url(images/portal/backgrounds/bgRight.gif);background-repeat:repeat-y;");
				bookTD3.Width="16px";

				//------------ looking for Books
				HtmlTable booksTable = new HtmlTable();
				HtmlTableRow bookTableRow = new HtmlTableRow();
				bookTableRow.Attributes["runat"] = "server";
				bookTableRow.ID = "subscriptionBookRow_" + subscription.Code; 
				bookTableRow.Attributes["class"] = "subscriptionBookDesc";
				booksTable.Controls.Add(bookTableRow);
				HtmlTableCell bookTableCell = new HtmlTableCell();
				bookTableRow.Controls.Add(bookTableCell);
				
				bookTableCell.InnerHtml = "<span class='booksAvailable'><img src='images/book.gif'>&nbsp;&nbsp;Books available for subscription:</span>";
				ISite site = DestroyerUi.GetCurrentSite(this.Page);
				foreach(string bookName in subscription.BookNames)
				{	
					IBook book = site.Books[bookName];
					if(book != null)
					{
						//create the table
						HtmlTable subscriptionBookTable = new HtmlTable();

						//create a row
						HtmlTableRow subscriptionBookTableRow = new HtmlTableRow();
						HtmlTableCell subscriptionBookTableCell = new HtmlTableCell();
						subscriptionBookTableCell.VAlign = "top";
						subscriptionBookTableCell.Width = "90%";//subscriptionBookTableCellWidth.ToString();

						string bookAnchorBeg = "";
						string bookAnchorEnd = "";

						if(book.Documents.Count > 0)
						{
							IDocument firstDoc = book.Documents[0];
							if(firstDoc != null)
							{
								bookAnchorBeg = "<A href='D_Link.aspx?targetdoc=" + book.Name + "&targetptr=" + book.Documents[0].Name + "'>";
								bookAnchorEnd = "</A>";
							}
						}

						subscriptionBookTableCell.InnerHtml = "<P class='subscriptionBookTitle'><img src='images/portal/gray_arrow.gif'>&nbsp;&nbsp;" + bookAnchorBeg + book.Title + bookAnchorEnd + "</P>";
						if(book.Description != "")
						{
							subscriptionBookTableCell.InnerHtml += "<P class='subscriptionBookDesc'>" + book.Description + "</P>";
						}
						//add the subscription book cell to the subscription book row
						subscriptionBookTableRow.Controls.Add(subscriptionBookTableCell);
						
						//add the subscription book row to the subscription book table
						subscriptionBookTable.Controls.Add(subscriptionBookTableRow);

						//add the subscription book table to the book table cell
						bookTableCell.Controls.Add(subscriptionBookTable);
					}
				}

				//---------------------------------
				bookTD2.Attributes["class"] = "subscriptionDesc";
				bookTD2.Controls.Add(booksTable);
				bookTD2.ColSpan = 2;
								
				booksRow.Controls.Add(bookTD1);
				booksRow.Controls.Add(bookTD2);
				booksRow.Controls.Add(bookTD3);

				headerTable.Controls.Add(booksRow);
				
				//blank Row
				headerLeftBG4.Attributes.Add("style","background-image:url(images/portal/backgrounds/footerLeft.gif);background-repeat:repeat-y;");
				headerLeftBG4.Height="77px";
				headerRightBG4.Attributes.Add("style","background-image:url(images/portal/backgrounds/footerRight.gif);background-repeat:repeat-y;background-position:left;");
				headerRightBG4.Width="22px";

				string toggleSubscriptionBooksLinkId = "showBookLink_" + subscription.Code;
				headerCell5.Attributes["class"] = "subscriptionDesc";
				headerCell5.Attributes.Add("style","background-image:url(images/portal/backgrounds/footerBg.gif);background-repeat:repeat-x;");
				headerCell5.InnerHtml = "<span class='subscriptionShowBooks'><img src='images/portal/oSearch.gif' valign='bottom'>&nbsp;&nbsp;<A href='#' id='" + toggleSubscriptionBooksLinkId + "' onclick='toggleSubscriptionBooks(\"" + bookTableRow.ClientID + "\", \"" + toggleSubscriptionBooksLinkId + "\")'>show books</A></span>";
				headerCell5.Align = "right";

				headerCell2.NoWrap = true;
				string logoImg = subscription.Code.IndexOf("emap") > -1 ? "emap.gif" : "ReSOURCE_logo.gif";
				headerCell2.InnerHtml = "<img src='images/portal/" + logoImg + "'>";
				headerCell2.Attributes.Add("style","background-image:url(images/portal/backgrounds/footerBg.gif);background-repeat:repeat-x;");
				
				headerRow4.Controls.Add(headerLeftBG4);
				headerRow4.Controls.Add(headerCell2);
				headerRow4.Controls.Add(headerCell5);
				headerRow4.Controls.Add(headerRightBG4);

				headerTable.Controls.Add(headerRow4);

				//Tab Row
				HtmlTable showBTable = new HtmlTable();
				HtmlTableRow showBtr = new HtmlTableRow();
				HtmlTableCell showBtd1 = new HtmlTableCell();
				HtmlTableCell showBtd2 = new HtmlTableCell();

				showBtd1.Attributes.Add("style","background-image:url(images/portal/bottomTab.gif);background-repeat:repeat-x;");
				showBtd1.Width = "19px";

				/*string toggleSubscriptionBooksLinkId = "showBookLink_" + subscription.Code;
				showBtd2.InnerHtml = "<span class='subscriptionShowBooks'><img src='images/portal/oSearch.gif' valign='bottom'>&nbsp;&nbsp;<A href='#' id='" + toggleSubscriptionBooksLinkId + "' onclick='toggleSubscriptionBooks(\"" + bookTableRow.ClientID + "\", \"" + toggleSubscriptionBooksLinkId + "\")'>show books</A></span>";
				showBtd2.BgColor = "white";
				showBtd2.Attributes.Add("style","background-image:url(images/portal/tabBG.gif);background-repeat:repeat-x;");
				showBtd2.Align = "right";*/
				
				/*showBtr.Controls.Add(showBtd1);
				showBtr.Controls.Add(showBtd2);
				showBTable.Controls.Add(showBtr);*/

				/*showBTable.CellPadding = 0;
				showBTable.CellSpacing = 0;
				showBTable.Border = 0;
				showBTable.Height = "31px";
				showBTable.Width = "20%";
				
				headerCell4.Controls.Add(showBTable);
				headerCell4.Align = "right";
				//headerCell4.ColSpan = 2;
				headerCell4.BgColor = "white";

				//headerLeftBG3.Attributes.Add("style","background-image:url(images/portal/clb.gif);background-repeat:repeat-y;");
				headerLeftBG3.BgColor = "white";
				headerRightBG3.Attributes.Add("style","background-image:url(images/portal/bottomRtab.gif);background-repeat:repeat-y;");
				headerRightBG3.Height="31px";

				headerRow3.Controls.Add(headerLeftBG3);
				headerRow3.Controls.Add(headerCell4);
				headerRow3.Controls.Add(headerRightBG3);

				headerTable.Controls.Add(headerRow3);*/

				//-----------------------------------------------------
				
				//set some attributes and inner html on the subscription description cell
				subscriptionTableDescCell.VAlign = "top";
				subscriptionTableDescCell.NoWrap = true;
				subscriptionTableDescCell.Width = "600";
				//string toggleSubscriptionBooksLinkId = "showBookLink_" + subscription.Code;
				subscriptionTableDescCell.Controls.Add(headerTable);
				//subscriptionTableDescCell.InnerHtml = "<li class='subscriptionTitle'>&nbsp;" + subscription.Title + "</li><P class='subscriptionDesc'>" + subscription.Description + "</P><span class='subscriptionShowBooks'><img src='images/portal/oSearch.gif' valign='bottom'>&nbsp;&nbsp;<A href='#' id='" + toggleSubscriptionBooksLinkId + "' onclick='toggleSubscriptionBooks(\"" + bookTableRow.ClientID + "\", \"" + toggleSubscriptionBooksLinkId + "\")'>show books</A></span>";
				
				//add a table for each book in the suscription
				/*ISite site = DestroyerUi.GetCurrentSite(this.Page);
				foreach(string bookName in subscription.BookNames)
				{	
					IBook book = site.Books[bookName];
					if(book != null)
					{
						//create the table
						HtmlTable subscriptionBookTable = new HtmlTable();

						//create a row
						HtmlTableRow subscriptionBookTableRow = new HtmlTableRow();
						HtmlTableCell subscriptionBookTableCell = new HtmlTableCell();
						subscriptionBookTableCell.VAlign = "top";
						int subscriptionBookTableCellWidth = int.Parse(subscriptionTableDescCell.Width) - 10;
						subscriptionBookTableCell.Width = subscriptionBookTableCellWidth.ToString();

						string bookAnchorBeg = "";
						string bookAnchorEnd = "";

						if(book.Documents.Count > 0)
						{
							IDocument firstDoc = book.Documents[0];
							if(firstDoc != null)
							{
								bookAnchorBeg = "<A href='D_Link.aspx?targetdoc=" + book.Name + "&targetptr=" + book.Documents[0].Name + "'>";
								bookAnchorEnd = "</A>";
							}
						}

						subscriptionBookTableCell.InnerHtml = "<P class='subscriptionBookTitle'><img src='images/book.gif'>&nbsp;&nbsp;" + bookAnchorBeg + book.Title + bookAnchorEnd + "</P>";
						if(book.Description != "")
						{
							subscriptionBookTableCell.InnerHtml += "<P class='subscriptionBookDesc'>" + book.Description + "</P>";
						}
						//add the subscription book cell to the subscription book row
						subscriptionBookTableRow.Controls.Add(subscriptionBookTableCell);
						
						//add the subscription book row to the subscription book table
						subscriptionBookTable.Controls.Add(subscriptionBookTableRow);

						//add the subscription book table to the book table cell
						bookTableCell.Controls.Add(subscriptionBookTable);
					}
				}*/

				//add the table to an array
				subscriptionTables.Add(subscriptionTable);
			}
		
			//go through each subscription control, adding to table columns
			HtmlTableRow tableRow = null;
			foreach(System.Web.UI.HtmlControls.HtmlTable subscriptionTable in subscriptionTables)
			{
				tableRow = new HtmlTableRow();

				//create a new cell
				HtmlTableCell tableCell = new HtmlTableCell();
				tableCell.VAlign = "top";
				tableCell.Align = "center";
				tableCell.Controls.Add(subscriptionTable);

				//attach the table cell to the table row
				tableRow.Controls.Add(tableCell);

				//attach the row to the table
				MySubscriptionsTable.Controls.Add(tableRow);
			}	
		}


	}
}

