using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

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
	/// Summary description for JSections.
	/// </summary>
	public partial class JSections : DestroyerUi
	{
		private AICPA.Destroyer.Content.Site.ISite site;
		protected IBook book = null;
		protected IDocument doc = null;
		protected int totalDocCount = 0;

		private ListItem emptyListItem;
		private string padding;
		
	
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

			// Only do the following on a first time load; don't want to lose values on post back
			if (!IsPostBack)
			{
				ReinitializeDropDown(jsTopic, true);
				ReinitializeDropDown(jsSection, true);
				
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

						jsTopic.Items.Add(new ListItem(sBook, j.ToString()));
					}
					
					// Add each distinct topic to DropDownList.Items (with indenting padding)
					jsTopic.Items.Add(new ListItem(padding + sValue + " - " + sTitle, sValue));
				}
			}

			// See if there is a command
			switch (DestroyerUi.GetDocumentCommand(this.Page))
			{
				case DestroyerUi.DocCmd.JoinSectionsRecurse:
					JoinDocsAndSubDocs();

					break;

				default:
					break;
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

		protected void jsTopic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string topic = jsTopic.SelectedValue;

			if (topic != string.Empty && int.Parse(topic) < 100)
			{
				jsTopic.SelectedIndex = int.Parse(jsTopic.SelectedValue);
				topic = jsTopic.SelectedValue;
			}
			
			ReinitializeDropDown(jsSection, false);

			// If a new value was chosen besides the default empty one
			if (topic != string.Empty)
			{
                //Get intersection info
				int isect = Intersection.Checked ? 1 : 0;
				int sec = 0;

				switch (rbSec.SelectedValue)
				{
					case "SEC": 
						sec = 1;
						break;
					case "ALL":
						sec = 2;
						break;
					default:
						sec = 0;
						break;
				}				
				
				SiteDs.Cod_MetaRow[] sections = site.GetSJoinSectionsByTopic(topic, isect, sec);

				for (int i = 0; i < sections.Length; i++)
				{
					string sTitle = sections[i].SectionTitle;
					string sValue = sections[i].SectionNum;
					
					// Add each distinct section across all subtopics for given topic (with indenting padding)
					jsSection.Items.Add(new ListItem(padding + sValue + " - " + sTitle, sValue));
				}
			}
			
			SubmitButton.Enabled = jsSection.SelectedValue == string.Empty ? false : true; 
		}

		private void ReinitializeDropDown(DropDownList dropDown, bool addDefaultEmpty)
		{
			dropDown.Items.Clear();

			if (addDefaultEmpty)
			{
				dropDown.Items.Add(emptyListItem);
			}
		}

		protected void Intersection_CheckedChanged(object sender, System.EventArgs e)
		{
			jsTopic_SelectedIndexChanged(sender,e);
		}

		protected void rbSec_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			jsTopic_SelectedIndexChanged(sender,e);
		}

		protected void SubmitButton_Click(object sender, System.EventArgs e)
		{
			string topicNum = jsTopic.SelectedValue;
			string sectionNum = jsSection.SelectedValue;
			int isect = Intersection.Checked ? 1 : 0;
			
			SiteDs.Cod_MetaRow[] searchResults = site.GetSJoinDocsByTopicSection(topicNum, sectionNum, isect);
			// Fill in Data Set with search results
			Documents.AutoGenerateColumns = false;
			Documents.DataSource = searchResults;
			Documents.DataBind();
			JoinSubmit.Visible = true;
			JoinWithSourcesSubmit.Visible = true;
		}

		protected void JoinSubmit_Click(object sender, System.EventArgs e)
		{
			JoinDocuments(false);
		}

		protected void JoinWithSourcesSubmit_Click(object sender, System.EventArgs e)
		{
			JoinDocuments(true);
		}

		private void JoinDocuments(bool includeSources)
		{
			// Didn't use DocumentCollection because it didn't appear to have dynamic length ability
			ArrayList docs = new ArrayList();

			foreach (DataGridItem di in Documents.Items)
			{
				HtmlInputCheckBox checkBox = (HtmlInputCheckBox)di.FindControl("joinCheck");

				if (checkBox != null && checkBox.Checked)
				{
					string topic = ((Label)di.FindControl("lblTopicNum")).Text;
					string subtopic = ((Label)di.FindControl("lblSubtopicNum")).Text;
					string section = ((Label)di.FindControl("lblSectionNum")).Text;
					string docName = string.Format("{0}-{1}-{2}", topic, subtopic, section);

					string bookName = ((Label)di.FindControl("lblBookTitle")).Text;
					// Lower the casing of the first character and replace any spaces; also add "faf-" at the beginning
					bookName = "faf-" + bookName.Substring(0, 1).ToLower() + bookName.Substring(1).Replace(" ", "");
					
					IDocument document = site.Books[bookName].Documents[docName];
					docs.Add(document);
				}
			}

			WriteDocsToStream(docs, includeSources);
		}

		private void JoinDocsAndSubDocs()
		{
			//get the xml that represents this top node and its immediate children
			// doc and site were populated in OnLoad event
			string tocXml = site.GetTocXml(doc.Id, NodeType.Document);

			// Didn't use DocumentCollection because it didn't appear to have dynamic length ability
			ArrayList docs = GetSubDocuments(tocXml);

			WriteDocsToStream(docs, false);
		}

		private ArrayList GetSubDocuments(string tocXml)
		{
			// Didn't use DocumentCollection because it didn't appear to have dynamic length ability
			ArrayList docs = new ArrayList();			

			//load the xml string into an xml document
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(tocXml);
		
			//get all the child document nodes that are represented in the xml
			XmlNodeList docNodes = xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENT);
			int docCount = 0;
			foreach(XmlNode docNode in docNodes)
			{
				//get the document name and "has children" status
				string docName = DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_DOCUMENTNAME]);
				int docId = int.Parse(DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_DOCUMENTID]));
				bool hasChildren = bool.Parse(DestroyerBpc.GetAttributeValue(docNode.Attributes[DestroyerBpc.XML_ATT_HASCHILDREN]));

				//if we are the first document in the xml, only print this out if we are also the top document 
				if(docCount > 0 | totalDocCount == 0)
				{
					//instantiate the document
					IDocument subDoc = book.Documents[docName];
					docs.Add(subDoc);

					totalDocCount++;
				}

				//recurse if there are children of this document node and if we are not the very first document node
				if(hasChildren && docCount > 0)
				{
					string subdocTocXml = site.GetTocXml(docId, NodeType.Document);
					docs.AddRange(GetSubDocuments(subdocTocXml).ToArray());
				}
				docCount++;
			}

			//return the ArrayList
			return docs;
		}

		private void WriteDocsToStream(ArrayList docs, bool includeSources)
		{
			bool firstDoc = true;
			string docString = string.Empty;

			// Start putting together the content
			foreach (object obj in docs)
			{
				IDocument document = (IDocument)obj;

				if(document.PrimaryFormat.Description == "text/html")
				{
					string content = DestroyerBpc.ByteArrayToStr(document.PrimaryFormat.Content);

					// strip out codification headers from all codification content, except the first one
					if (firstDoc)
						firstDoc = false;
					else
						content = D_PrintDocument.RemoveCodificationHeaders(content);

					// remove all codification footers
					// sburton: we may want to alter this to leave the footers of the last doc...
					content = D_PrintDocument.RemoveCodificationFooters(content);

					// add necessary codification stylesheets
					content = D_PrintDocument.IncludeCodificationStyleSheets(content, includeSources);

					// remove noPrint spans
					content = D_PrintDocument.RemoveNoPrintSpans(content);

					docString += content;
				}
			}

			if (docString != string.Empty)
			{
				// output the actual document contents to the response output stream
				Response.Write(docString);

				// end the response
				Response.End();
			}
		}
	}
}
