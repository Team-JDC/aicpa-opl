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
using System.IO;


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
	/// Summary description for D_Goto.
	/// </summary>
	public partial class D_Goto : DestroyerUi
	{
		
		private AICPA.Destroyer.Content.Site.ISite site;
		protected IBook book = null;
		protected IDocument doc = null;
		protected int totalDocCount = 0;
		private string padding = string.Empty;

		private ListItem emptyListItem;
				
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//get the site (which is already filtered according to user subscription)
			site = GetCurrentSite(this.Page);
			doc = GetCurrentDocument(this.Page);
			book = doc.Book;
			emptyListItem = new ListItem(string.Empty);

			// Do this trick to add leading spaces to DropDownList ListItems
			padding = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			using (StringWriter writer = new StringWriter())
			{
				Server.HtmlDecode(padding, writer);
				padding = writer.ToString();
			}

			if (!IsPostBack)
			{
				ReinitializeDropDown(gtTopic, true);
				ReinitializeDropDown(gtSubtopic, true);
				ReinitializeDropDown(gtSection, true);
				// Get the unique collection of Codification topics from the database
				SiteDs.Cod_MetaRow[] topics = site.GetSJoinTopics();
				
				string curBook = string.Empty;
				int numExtraListItems = 1;

				for (int i = 0; i < topics.Length; i++)
				{
					string sTitle = topics[i].TopicTitle;
					string sValue = topics[i].TopicNum;
					string sBook = topics[i].BookTitle;
					
					// If changing to different book collection,
					//	add extra ListItem entry in order to group topics together
					if (sBook != curBook)
					{
						curBook = sBook;
						++numExtraListItems;
						int j = i + numExtraListItems;

						gtTopic.Items.Add(new ListItem(sBook, j.ToString()));
					}
					
					// Add each distinct topic to DropDownList.Items (with indenting padding)
					gtTopic.Items.Add(new ListItem(padding + sValue + " - " + sTitle, sValue));
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

		}
		#endregion

		private void ReinitializeDropDown(DropDownList dropDown, bool addDefaultEmpty)
		{
			dropDown.Items.Clear();

			if (addDefaultEmpty)
			{
				dropDown.Items.Add(emptyListItem);
			}
		}

		protected void gtTopic_SelectedIndexChanged(object sender, System.EventArgs e)
		{

			string topicNum = gtTopic.SelectedValue.ToString();

			if (topicNum != string.Empty && int.Parse(topicNum) < 100)
			{
				gtTopic.SelectedIndex = int.Parse(gtTopic.SelectedValue);
				topicNum = gtTopic.SelectedValue;
			}
			
			//get the subtopic values - clear out all existing values
			ReinitializeDropDown(gtSubtopic, true);
			// Get the unique collection of Codification Subtopics from the database
			SiteDs.Cod_MetaRow[] Subtopics = site.GetSubtopicByTopic(topicNum);
			for (int i = 0; i < Subtopics.Length; i++)
			{
				string sTitle = Subtopics[i].SubtopicTitle;
				string sValue = Subtopics[i].SubtopicNum;
				// Add each distinct topic to DropDownList.Items (with indenting padding)
				gtSubtopic.Items.Add(new ListItem(padding + sValue + " - " + sTitle, sValue));
			}

			SubmitButton.Enabled = true;
			return;
		}

		protected void gtSubtopic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Get the Section values - clear our all existing values
			ReinitializeDropDown(gtSection, true);
			string topicNum = gtTopic.SelectedValue.ToString();
			string subNum = gtSubtopic.SelectedValue.ToString();
			SiteDs.Cod_MetaRow[] Sections = site.GetSectionByTopicSubtopic(topicNum,subNum);
			for (int i = 0; i < Sections.Length; i++)
			{
				string sTitle = Sections[i].SectionTitle;
				string sValue = Sections[i].SectionNum;
				// Add each distinct topic to DropDownList.Items (with indenting padding)
				gtSection.Items.Add(new ListItem(padding + sValue + " - " + sTitle, sValue));
			}
			return;
		}

		protected void gtSection_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			return;
		}

		protected void SubmitButton_Click(object sender, System.EventArgs e)
		{
			string topicNum = gtTopic.SelectedValue.ToString();
			string subNum = gtSubtopic.SelectedValue.ToString();
			string sectNum = gtSection.SelectedValue.ToString();
			string targetPtr = string.Empty;
			string targetDoc = getFafBookName(topicNum);
            
			if (subNum.Length > 1)
			{
				targetPtr =  topicNum + "-" + subNum; 
				
			}
			else
			{
				targetPtr = "topic_" + topicNum;
			}
			if (sectNum.Length > 1)
			{
				targetPtr = targetPtr + "-" + sectNum;
			}
			
			string url = string.Format("D_Link.aspx?{0}={1}&{2}={3}&", DestroyerUi.REQPARAM_TARGETDOC, targetDoc, DestroyerUi.REQPARAM_TARGETPTR, targetPtr);

			Response.Write("<Script>top.window.location.href='" + url + "';</Script>");
			//Response.Write("top.window.location.href ='" + url + "');");

			return;
		}

		private string getFafBookName(string TopicNumber)
		{
			string firstChar = TopicNumber.Substring(0,1);
			switch (firstChar)
			{		
				case "1":
					return "faf-";
				case "2":
					return "faf-presentation";
				case "3":
					return "faf-assets";
				case "4":
					return "faf-liabilities";
				case "5":
					return "faf-equity";
				case "6":
					return "faf-revenue";
				case "7":
					return "faf-expenses";
				case "8":
					return "faf-broadTransactions";
				case "9":
					return "faf-industry";
				default:
					break;
			}
			return string.Empty;
		}

	}

}
