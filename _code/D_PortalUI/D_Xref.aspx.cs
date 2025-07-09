using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;

namespace AICPA.Destroyer.UI.Portal
{
	/// <summary>
	/// Summary description for D_Xref.
	/// </summary>
	public partial class D_Xref : System.Web.UI.Page
	{
		private AICPA.Destroyer.Content.Site.ISite site;
		private ListItem emptyItem;
		//private XRefComparer comparer;

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Get the Site object for querying database through DALC
			site = DestroyerUi.GetCurrentSite(this.Page);
			emptyItem = new ListItem(string.Empty);
			//comparer = new XRefComparer();
			
			// Only do the following on a first time load; don't want to lose values on post back
			if (!IsPostBack)
			{
				ReinitializeDropDown(StandardTypesDropDown, true);
				ReinitializeDropDown(StandardNumbersDropDown, true);
				ReinitializeDropDown(DropdownTopic , true);
				ReinitializeDropDown(DropdownStopic , true);
				ReinitializeDropDown(DropdownSect , true);

				//Set this to the 0 pageindex if not a postback
				ResultsDataGrid.CurrentPageIndex = 0;

				// Get the unique collection of Standard Types from the database
				string[] standards = site.GetXRefStandardTypes();

				for (int i = 0; i < standards.Length; i++)
				{
					StandardTypesDropDown.Items.Add(standards[i]);
				}
				//Get the unique collection of Topics 
				string[] topics = site.GetXRefTopics();

				for (int i = 0; i < topics.Length; i++)
				{
					DropdownTopic.Items.Add(topics[i]);
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

		protected void StandardTypesDropDown_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string newType = StandardTypesDropDown.SelectedValue;

			ReinitializeDropDown(StandardNumbersDropDown, false);

			// If a new value was chosen besides the default empty one
			if (newType != string.Empty)
			{
				// Query the database for the Standard Numbers of the newly selected Standard Type
				string[] numbers = site.GetXRefStandardNumbersForStandardType(newType);

				for (int i = 0; i < numbers.Length; i++)
				{
					StandardNumbersDropDown.Items.Add(numbers[i]);
				}
			}

			//Clear all of the Codification Selections
			DropdownTopic.SelectedIndex = 0;
			ReinitializeDropDown(DropdownStopic , true);
			ReinitializeDropDown(DropdownSect , true);
		}

		protected void SubmitButton_Click(object sender, System.EventArgs e)
		{
			string standardType = StandardTypesDropDown.SelectedValue;
			string standardNumber = StandardNumbersDropDown.SelectedValue;
			string topic = DropdownTopic.SelectedValue;
			string subtopic = DropdownStopic.SelectedValue;
			string section = DropdownSect.SelectedValue;
			
			if (((standardType == string.Empty) || (standardNumber == string.Empty))&&((topic == string.Empty) || (subtopic == string.Empty) || (section == string.Empty)))
			{
				// User needs to select stuff for search
				return;
			}

			if (standardType != string.Empty)
			{
				SiteDs.XRefRow[] searchResults = site.GetXRefCodByStandard(standardType, standardNumber);
				//Array.Sort(searchResults, comparer);
				// Fill in Data Set with search results
				ResultsDataGrid.AutoGenerateColumns = false;
				ResultsDataGrid.DataSource = searchResults;
				ResultsDataGrid.DataBind();
			}
			else
			{
				SiteDs.XRefRow[] searchResults = site.GetXRefStandardByCod(topic,subtopic,section);
				//Array.Sort(searchResults, comparer);
				// Fill in Data Set with search results
				ResultsDataGrid.AutoGenerateColumns = false;
				ResultsDataGrid.DataSource = searchResults;
				ResultsDataGrid.DataBind();
			}
		}

		protected void Clear_Click(object sender, System.EventArgs e)
		{
			StandardTypesDropDown.SelectedIndex=0;
			ReinitializeDropDown(StandardNumbersDropDown, true);

			DropdownTopic.SelectedIndex=0;
			ReinitializeDropDown(DropdownStopic , true);
			ReinitializeDropDown(DropdownSect , true);

			ResultsDataGrid.DataSource = null;
			ResultsDataGrid.DataBind();
		}
	
		private void ReinitializeDropDown(DropDownList dropDown, bool addDefaultEmpty)
		{
			dropDown.Items.Clear();

			if (addDefaultEmpty)
			{
				dropDown.Items.Add(emptyItem);
			}
		}
		
		protected void DropdownTopic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Set the values in the Subtopic dropdown
			string newTopic = DropdownTopic.SelectedValue;

			ReinitializeDropDown(DropdownStopic, false);

			// If a new value was chosen besides the default empty one
			if (newTopic != string.Empty)
			{
				// Query the database for the Standard Numbers of the newly selected Standard Type
				string[] Stopics = site.GetXRefSubtopicsByTopic(newTopic);
				string firstSubTopic = Stopics[0];

				for (int i = 0; i < Stopics.Length; i++)
				{
					DropdownStopic.Items.Add(Stopics[i]);
				}

				ReinitializeDropDown(DropdownSect, false);

				// If a new value was chosen besides the default empty one
				if (firstSubTopic != string.Empty)
				{
					// Query the database for the Standard Numbers of the newly selected Standard Type
					string[] Sections = site.GetXRefSectionsByTopicSubtopic(newTopic,firstSubTopic);

					for (int i = 0; i < Sections.Length; i++)
					{
						DropdownSect.Items.Add(Sections[i]);
					}
				}
			}

			//Clear all of the Standards Selections
			StandardTypesDropDown.SelectedIndex=0;
			ReinitializeDropDown(StandardNumbersDropDown, true);
		}

		protected void DropdownStopic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//Set the values in the Subtopic dropdown
			string newTopic = DropdownTopic.SelectedValue;
			string newStopic = DropdownStopic.SelectedValue;

			ReinitializeDropDown(DropdownSect, false);

			// If a new value was chosen besides the default empty one
			if (newStopic != string.Empty)
			{
				// Query the database for the Standard Numbers of the newly selected Standard Type
				string[] Sections = site.GetXRefSectionsByTopicSubtopic(newTopic,newStopic);

				for (int i = 0; i < Sections.Length; i++)
				{
					DropdownSect.Items.Add(Sections[i]);
				}
			}
		}
	}

//	public class XRefComparer : IComparer
//	{
//		NumberFormatInfo numberFormatInfo;
//		NumberStyles numberStyles;
//
//		public XRefComparer()
//		{
//			numberFormatInfo = new CultureInfo("en-US", true).NumberFormat;
//			numberStyles = NumberStyles.Any;
//		}
//
//		public int Compare(object x, object y)
//		{
//			SiteDs.XRefRow first = (SiteDs.XRefRow)x;
//			SiteDs.XRefRow second = (SiteDs.XRefRow)y;
//			
//			// Sort Order: StandardType,StandardID,Para_Label,Sequence,Topic,Subtopic,Section,CodPara,term
//			int standardTypeCompare = first.StandardType.CompareTo(second.StandardType);
//			if (standardTypeCompare == 0)
//			{
//				int standardIDCompare = first.StandardID.CompareTo(second.StandardID);
//				if (standardIDCompare == 0)
//				{
//					// Custom sort logic for the paraLabels
//					int paraLabelCompare = CompareParaLabels(first.Para_Label, second.Para_Label);
//					if (paraLabelCompare == 0)
//					{
//						int sequenceCompare = first.Sequence.CompareTo(second.Sequence);
//						if (sequenceCompare == 0)
//						{
//							int topicCompare = first.Topic.CompareTo(second.Topic);
//							if (topicCompare == 0)
//							{
//								int subtopicCompare = first.Subtopic.CompareTo(second.Subtopic);
//								if (subtopicCompare == 0)
//								{
//									int sectionCompare = first.Section.CompareTo(second.Section);
//									if (sectionCompare == 0)
//									{
//										int codParaCompare = first.CodPara.CompareTo(second.CodPara);
//										if (codParaCompare == 0)
//										{
//											return first.term.CompareTo(second.term);
//										}
//
//										return codParaCompare;
//									}
//
//									return sectionCompare;
//								}
//
//								return subtopicCompare;
//							}
//
//							return topicCompare;
//						}
//
//						return sequenceCompare;
//					}
//
//					return paraLabelCompare;
//				}
//
//				return standardIDCompare;
//			}
//
//			return standardTypeCompare;
//		}
//
//		private int CompareParaLabels(string first, string second)
//		{
//			double firstNum;
//			double secondNum;
//
//			// Try to turn each string into a number
//			bool FirstIsANum = double.TryParse(first, numberStyles, numberFormatInfo, out firstNum);
//			bool SecondIsANum = double.TryParse(second, numberStyles, numberFormatInfo, out secondNum);
//
//			if (FirstIsANum)
//			{
//				if (SecondIsANum)
//				{
//					// Return number compare
//					return firstNum.CompareTo(secondNum);
//				}
//				else // Second is not a num
//				{
//					// Numbers sort less than a string
//					return -1;
//				}
//			}
//			else // First is not a num
//			{
//				if (SecondIsANum)
//				{
//					// Strings sort greater than a number
//					return 1;
//				}
//				else // Second is not a num
//				{
//					// Return string compare
//					return first.CompareTo(second);
//				}
//			}
//		}
//	}
}
