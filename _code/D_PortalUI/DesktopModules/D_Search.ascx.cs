#region Imported Namespaces

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;

#endregion

namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
    ///<summary>
    ///  Summary description for D_Search.
    ///</summary>
    public partial class D_Search : PortalModuleControl
    {
        protected const string SEARCH_MODULE = "Search";
        public const string SESSPARAM_CURRENTUSER = "CurrentUser";
        protected ImageButton ImageButton1;
        protected Button SearchButton;
        protected HtmlTableCell SearchResultsSummaryCurrentRange;
        protected HtmlTable SearchResultsSummaryTable;
        protected HtmlTableRow SearchResultsSummaryTr;
        protected HtmlTable SearchTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            //hide the search Unsubscribed for exam users
            //initialize to null
            IUser retUser = null;
            //try to get the user from the session
            retUser = DestroyerUi.GetCurrentUser(Page);
            //SearchUnsubscribedCheckBox.Checked = false;
            //string unSubs = Request.QueryString["noSubs"];

            //added by dam 11/16/05 to maintain the unSubscribed content state
            SearchUnsubscribedCheckBox.Attributes.Add("onclick", "setUnsubscribed(this);");
            SearchUnsubscribedCheckBox.Checked = Session["SeeUnsubscribed"] != null
                                                     ? Convert.ToBoolean(Session["SeeUnsubscribed"])
                                                     : SearchUnsubscribedCheckBox.Checked;

            if (retUser.ReferringSiteValue == ReferringSite.Exams)
            {
                SearchUnsubscribedCheckBox.Checked = false;
                SearchUnsubscribedCheckBox.Visible = false;
            }

            //setting up initial refinement table visibility
            Session["expandedRefinementSearch"] = Session["expandedRefinementSearch"] == null
                                                      ? "true"
                                                      : Session["expandedRefinementSearch"];

            //hide the search results tables
            SelectedSearchDimensionsTable.Visible = false;
            RefinementSearchDimensionsTable.Visible = false;
            SearchSummaryTable.Visible = false;
            SearchResultsTable.Visible = false;
            RefinementSearchDimensionsTableHolder.Visible = false;
            searchSubmit.Attributes.Add("onclick", "javascript:showWaiting(false)");
            if (!Page.IsPostBack)
            {
                string topSearchTxt = Convert.ToString(Request.QueryString["Search"]);
                if (topSearchTxt != null && topSearchTxt != string.Empty && topSearchTxt != "")
                {
                    topSearch_Submitted(topSearchTxt);
                }
            }
        }

        /// <summary>
        ///   Renders the search results from a search results object in the session
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        protected void D_Search_PreRender(object sender, EventArgs e)
        {
            //see if we have a search results in our session...
            ISearchResults currentSearchResults = (ISearchResults) Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];

            //...if we do have a search results object in our session, populate the search results table
            if (currentSearchResults != null)
            {
                //pre-populate our inputs with our search results object
                PopulateInputs(currentSearchResults.SearchCriteria);

                //load the dimensions xml
                XmlDocument dimensionsXmlDoc = new XmlDocument();
                dimensionsXmlDoc.LoadXml(currentSearchResults.DimensionsXml);

                //render the selected dimensions info
                XmlNodeList selectedDimensionNodes =
                    dimensionsXmlDoc.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSSELECTEDDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSION);
                foreach (XmlNode selectedDimensionNode in selectedDimensionNodes)
                {
                    //the html we will insert for the selected dimension
                    string dimensionHtml = DestroyerUi.EMPTY_STRING;

                    //get the selected dimension name and id
                    XmlNode dimensionNameNode =
                        selectedDimensionNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONNAME);
                    XmlNode dimensionIdNode =
                        selectedDimensionNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONID);

                    //go through each of the ancestors of the dimension and build a path in our HTML
                    XmlNodeList selectedDimensionAncestorNodes =
                        selectedDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORS + "/" +
                                                          DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEM);
                    foreach (XmlNode selectedDimensionAncestorNode in selectedDimensionAncestorNodes)
                    {
                        XmlNode selectedDimensionAncestorNameNode =
                            selectedDimensionAncestorNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMNAME);
                        XmlNode selectedDimensionAncestorIdNode =
                            selectedDimensionAncestorNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMID);
                        dimensionHtml += "<A href='D_Search.aspx?" + DestroyerUi.REQPARAM_SEARCHDIMENSIONVALUEIDS + "=" +
                                         selectedDimensionAncestorIdNode.InnerText + "'>" +
                                         selectedDimensionAncestorNameNode.InnerText + "</A>" +
                                         "<img src='images/portal/blackArrow.gif'>"; //+ DestroyerUi.DISPLAYPATH_SEP;
                    }

                    //create the html objects, add the generated html, and add it to our table
                    HtmlTableRow dimensionRow = new HtmlTableRow();
                    HtmlTableCell dimensionCell = new HtmlTableCell();
                    dimensionHtml += dimensionNameNode.InnerText;
                    dimensionCell.InnerHtml = dimensionHtml;
                    dimensionRow.Controls.Add(dimensionCell);
                    HtmlTableCell dimensionRemoveCell = new HtmlTableCell();
                    dimensionRemoveCell.Align = "right";
                    dimensionRemoveCell.InnerHtml = "<a href='D_Search.aspx?" +
                                                    DestroyerUi.REQPARAM_SEARCHDIMENSIONVALUEIDS + "=" +
                                                    DestroyerBpc.ENDECA_DIMENSIONID_INITIAL + "'>remove</a>";
                    dimensionRow.Controls.Add(dimensionRemoveCell);
                    SelectedSearchDimensionsTable.Controls.Add(dimensionRow);
                }

                //render the refinement dimensions info
                XmlNodeList refinementDimensionNodes =
                    dimensionsXmlDoc.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSREFINEMENTDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSION);
                foreach (XmlNode refinementDimensionNode in refinementDimensionNodes)
                {
                    //go through each of the complete path nodes of the dimension
                    XmlNodeList selectedDimensionCompletePathNodes =
                        refinementDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATH +
                                                            "/" +
                                                            DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEM);
                    int indentCount = 0;
                    int totalSelectableRefinementSearches = 0;
                    int countSelectableRefinementSearches = 0;
                    string indentString = "";
                    string dimensionValueHtml = "";

                    foreach (XmlNode selectedDimensionCompletePathNode in selectedDimensionCompletePathNodes)
                    {
                        XmlNode selectedDimensionCompletePathNameNode =
                            selectedDimensionCompletePathNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMNAME);
                        XmlNode selectedDimensionCompletePathIdNode =
                            selectedDimensionCompletePathNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMID);
                        HtmlTableRow dimensionCompletePathRow = new HtmlTableRow();
                        for (int i = 0; i < indentCount; i++)
                        {
                            indentString += "&nbsp;&nbsp;";
                        }
                        HtmlTableCell dimensionCompletePathCell = new HtmlTableCell();
                        dimensionCompletePathCell.ColSpan = 99;
                        dimensionCompletePathCell.InnerHtml = indentString +
                                                              CleanDimensionName(
                                                                  selectedDimensionCompletePathNameNode.InnerText);
                        dimensionCompletePathRow.Controls.Add(dimensionCompletePathCell);
                        RefinementSearchDimensionsTable.Controls.Add(dimensionCompletePathRow);
                        indentCount++;
                    }

                    //indent one more time
                    indentString += "&nbsp;&nbsp;";

                    //go through each of the current refinement dimensions
                    XmlNodeList dimensionValueNodes =
                        refinementDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE);
                    foreach (XmlNode dimensionValueNode in dimensionValueNodes)
                    {
                        //create the dimension value row
                        HtmlTableRow dimensionValueRow = new HtmlTableRow();

                        //create and insert the first cell with the dimension value name and link
                        HtmlTableCell dimensionValueCell1 = new HtmlTableCell();
                        XmlNode dimensionValueNameNode =
                            dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUENAME);
                        XmlNode dimensionValueIdNode =
                            dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUEID);

                        dimensionValueHtml = "<span style='background-color:white;'>" + indentString +
                                             "</span><a style='background-color:white;' href='D_Search.aspx?" +
                                             DestroyerUi.REQPARAM_SEARCHDIMENSIONVALUEIDS + "=" +
                                             dimensionValueIdNode.InnerText + "'>" + dimensionValueNameNode.InnerText +
                                             "</a>";
                        dimensionValueCell1.InnerHtml = dimensionValueHtml;
                        dimensionValueCell1.Attributes.Add("style",
                                                           "background-image:url(images/portal/dots.gif);background-repeat:repeat-x;");
                        dimensionValueRow.Controls.Add(dimensionValueCell1);

                        //create and insert the second cell with the dimension value record count
                        HtmlTableCell dimensionValueCell2 = new HtmlTableCell();
                        XmlNode dimensionValueRecCountNode =
                            dimensionValueNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUERECORDCOUNT);
                        dimensionValueHtml = dimensionValueRecCountNode.InnerText;
                        dimensionValueCell2.InnerHtml = dimensionValueHtml;
                        dimensionValueCell2.Align = "right";
                        dimensionValueCell2.Width = "2%";
                        dimensionValueCell2.NoWrap = true;
                        dimensionValueRow.Controls.Add(dimensionValueCell2);

                        //add the row to the table
                        RefinementSearchDimensionsTable.Controls.Add(dimensionValueRow);

                        //typing track of total/count refinement searches
                        totalSelectableRefinementSearches = totalSelectableRefinementSearches +
                                                            Convert.ToInt16(dimensionValueHtml);
                        countSelectableRefinementSearches++;
                    }

                    //hide RefinementSearchDimensionsTable if all search = 1
                    /*if(totalSelectableRefinementSearches == countSelectableRefinementSearches)
					{
						Session["expandedRefinementSearch"] = "false";
						int gg = Session["expandedRefinementSearchForced"] == null ? 0 : (int)Session["expandedRefinementSearchForced"];
						Session["expandedRefinementSearchForced"] = gg + 1;
					}*/
                }

                //render the summary info
                int pageStartNum = (int) currentSearchResults.PageOffset + 1;
                int pageEndNum = (int) currentSearchResults.PageOffset + (int) currentSearchResults.PageHitCount;
                PageFirstHitLabel.Text = pageStartNum.ToString();
                PageLastHitLabel.Text = pageEndNum.ToString();
                TotalHitsLabel.Text = currentSearchResults.TotalHitCount.ToString();
                PageFirstHitLabelBtm.Text = pageStartNum.ToString();
                PageLastHitLabelBtm.Text = pageEndNum.ToString();
                TotalHitsLabelBtm.Text = currentSearchResults.TotalHitCount.ToString();

                //set the visible state of the prev/next links
                PrevHitsLinkButton.Enabled = !(pageStartNum == 1);
                NextHitsLinkButton.Enabled = !(pageEndNum == currentSearchResults.TotalHitCount);

                PrevHitsLinkButtomBtn.Enabled = !(pageStartNum == 1);
                NextHitsLinkButtomBtn.Enabled = !(pageEndNum == currentSearchResults.TotalHitCount);

                //show the search results tables
                SelectedSearchDimensionsTable.Visible = SelectedSearchDimensionsTable.Controls.Count > 1 ? true : false;

                //if(RefinementSearchDimensionsTableExpand.Value == "" || RefinementSearchDimensionsTableExpand.Value == "false")
                if (Session["expandedRefinementSearch"] == null ||
                    (string) Session["expandedRefinementSearch"] == "false")
                {
                    showRefinementSearchDimensions.ImageUrl = "~/images/portal/topicbar_down.gif";
                    RefinementSearchDimensionsTable.Visible = false;
                    Session["expandedRefinementSearch"] = "false";
                    //We are not goint to do the forced refinement toggle.
                    /*int checker = Session["expandedRefinementSearchForced"] == null ? 0 : (int)Session["expandedRefinementSearchForced"];
					if(checker > 1)
					{
						showRefinementSearchDimensions.ImageUrl = "~/images/portal/topicbar_up.gif";
						RefinementSearchDimensionsTable.Visible = true;
						Session["expandedRefinementSearch"] = "true";
					}*/
                }
                else
                {
                    showRefinementSearchDimensions.ImageUrl = "~/images/portal/topicbar_up.gif";
                    RefinementSearchDimensionsTable.Visible = true;
                    Session["expandedRefinementSearch"] = "true";
                }

                //RefinementSearchDimensionsTable.Visible = true;
                if (currentSearchResults.TotalHitCount > 0)
                {
                    RefinementSearchDimensionsTableHolder.Visible = true;
                    SearchSummaryTable.Visible = true;
                    SearchResultsTable.Visible = true;
                }
                else
                {
                    searchNotFound.Visible = true;
                    whatToDo.Visible = true;
                }
                SearchResultsTable.Border = 0;

                //log Search
                Event eventError = new Event(EventType.Error, DateTime.Now, 5, SEARCH_MODULE, SearchTextBox.Text,
                                             TotalHitsLabel.Text, "User Search");
                eventError.Save(false);
                //SearchTextBox

                //render the document hits
                for (int i = 0; i < currentSearchResults.Documents.Length; i++)
                {
                    int currentDoc = pageStartNum + i;
                    IDocument doc = currentSearchResults.Documents[i];
                    HtmlTableRow[] rows = CreateSearchResultsDocRows(doc, currentDoc);
                    foreach (HtmlTableRow row in rows)
                    {
                        SearchResultsTable.Controls.Add(row);
                    }
                }
            }
        }

        /// <summary>
        ///   Helper method for repopulating the search inputs from the search criteria. Don't use viewstate, since we want to
        ///   repopulate within the session scope, not just the page scope
        /// </summary>
        /// <param name = "searchCriteria"></param>
        private void PopulateInputs(ISearchCriteria searchCriteria)
        {
            //set the search text
            SearchTextBox.Text = searchCriteria.Keywords;

            //set the search type
            switch (searchCriteria.SearchType)
            {
                case SearchType.AllWords:
                    AllWordsRadioButton.Checked = true;
                    break;
                case SearchType.AnyWords:
                    AnyWordsRadioButton.Checked = true;
                    break;
                case SearchType.Boolean:
                    BooleanRadioButton.Checked = true;
                    break;
                case SearchType.ExactPhrase:
                    PhraseRadioButton.Checked = true;
                    break;
            }

            //set the "show excerpts" box
            ShowExcerptsCheckBox.Checked = searchCriteria.Excerpts;

            //set the "search unsubscribed" box
            //HERE DAVID
            //SearchUnsubscribedCheckBox.Checked = !searchCriteria.FilterUnsubscribed;
        }

        /// <summary>
        ///   Helper method for adjusting dimension names to more user-friendly alternatives
        /// </summary>
        /// <param name = "dimensionName"></param>
        /// <returns></returns>
        private string CleanDimensionName(string dimensionName)
        {
            string retName = dimensionName;
            switch (dimensionName)
            {
                case DestroyerBpc.ENDECA_DESTROYERSITEHIERARCHY_DIMENSION:
                    retName = "Content Hierarchy";
                    break;
            }
            return retName;
        }


        /// <summary>
        ///   Helper method for inserting a set of hit document table rows
        /// </summary>
        /// <param name = "doc"></param>
        /// <param name = "hitCount"></param>
        /// <returns></returns>
        private HtmlTableRow[] CreateSearchResultsDocRows(IDocument doc, int hitCount)
        {
            //set the css class to use for these rows
            string linkClassName = (doc != null && doc.InSubscription) ? "authorizedContent" : "nonAuthorizedContent";
            bool showLegend = (doc != null && doc.InSubscription) ? false : true;

            //display legend if at least one of the documents is not in subscription
            if (unsubscribedLegend.Visible == false && showLegend)
            {
                unsubscribedLegend.Text = "<img src='images/portal/main-lock_small.gif'> Unsubscribed document.";
                unsubscribedLegend.CssClass = "legend";
                unsubscribedLegend.Visible = showLegend;
            }

            //an array list to manage our rows
            ArrayList rows = new ArrayList();

            //the document reference row
            HtmlTableRow docRefRow = new HtmlTableRow();
            docRefRow.Attributes["class"] = linkClassName;

            //the cell holding the hit number
            HtmlTableCell refCell1 = new HtmlTableCell();
            refCell1.Attributes["class"] = "searchResultHeadernumber"; //linkClassName;
            refCell1.InnerHtml = (doc != null && doc.InSubscription)
                                     ? hitCount + "."
                                     : hitCount + ".&nbsp;<img src='images/portal/main-lock_small.gif'>";

            //the cell holding the reference line
            HtmlTableCell refCell2 = new HtmlTableCell();
            refCell2.Attributes["class"] = "searchResultHeaderText"; //linkClassName;

            if (doc != null)
            {
                refCell2.InnerHtml = CreateReferencePathHtml(doc);
            }
            else
            {
                refCell2.InnerHtml = DestroyerUi.ERROR_SEARCHINDEXOUTOFDATE;
            }

            //add the cells to the row
            docRefRow.Controls.Add(refCell1);
            docRefRow.Controls.Add(refCell2);

            //add the row to our array
            rows.Add(docRefRow);

            if (doc != null && doc.KeyWordsInContext != null && doc.KeyWordsInContext != DestroyerBpc.EMPTY_STRING)
            {
                //the document kwic row
                HtmlTableRow docKwicRow = new HtmlTableRow();
                docKwicRow.Attributes["class"] = linkClassName;

                //the cell holding the hit number
                HtmlTableCell kwicCell1 = new HtmlTableCell();
                kwicCell1.Attributes["class"] = "kwic";
                kwicCell1.ColSpan = 99;
                kwicCell1.InnerHtml = doc.KeyWordsInContext;
                docKwicRow.Controls.Add(kwicCell1);

                //add the row to our array
                rows.Add(docKwicRow);
            }

            //return the rows
            return (HtmlTableRow[]) rows.ToArray(typeof (HtmlTableRow));
        }

        /// <summary>
        ///   Helper method for creating html out of refpath xml
        /// </summary>
        /// <param name = "siteRefPathXml"></param>
        /// <returns></returns>
        private string CreateReferencePathHtml(IDocument doc)
        {
            //set our site ref path xml string and our in subscription flag
            string siteRefPathXml = doc.SiteReferencePath;

            //default to an empty string
            string retHtml = DestroyerUi.EMPTY_STRING;

            //use an xml document ot load our refpath xml string
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(siteRefPathXml);

            //get the book node (for later...)
            XmlNode bookNode = xmlDoc.SelectSingleNode("*/" + DestroyerBpc.XML_ELE_BOOK);

            //go through each refpath node and add to the html
            XmlNodeList nodes = xmlDoc.SelectNodes("*/*");
            int nodeCount = nodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode node = nodes[i];
                string title = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_TITLE]);

                //if we are at the last hit and we are a subscription document, insert a document link around the title
                if (i == nodeCount - 1)
                {
                    string bookName = DestroyerBpc.GetAttributeValue(bookNode.Attributes[DestroyerBpc.XML_ATT_NAME]);
                    string docName = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_NAME]);
                    retHtml += string.Format("<a href='D_Link.aspx?{0}={1}&{2}={3}&{4}={5}'>",
                                             DestroyerUi.REQPARAM_TARGETDOC, bookName, DestroyerUi.REQPARAM_TARGETPTR,
                                             docName, DestroyerUi.REQPARAM_LINKREFERRER,
                                             DestroyerUi.LinkReferrer.SearchResults);
                }

                //send out the title
                retHtml += title;

                //if we are at the last hit, end the document link
                if (i == nodeCount - 1)
                {
                    retHtml += "</a>";
                }

                //insert a separator character between refpath nodes
                if (i < nodeCount - 2)
                {
                    retHtml += "<img src='images/portal/header.gif'>";
                }

                //right before the last node insert a line break
                if (i == nodeCount - 2)
                {
                    retHtml += "<br>";
                }
            }

            //return our html
            return retHtml;
        }

        /// <summary>
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        protected void HelpImageButton_Click(object sender, ImageClickEventArgs e)
        {
            DestroyerUi.ShowHelp(Page, DestroyerUi.HelpTopic.Search);
        }

        /*private void SearchButton_Click(object sender, System.EventArgs e)
		{
			//get the site
			ISite site = DestroyerUi.GetCurrentSite(this.Page);

			//get our search keywords
			string keywords = SearchTextBox.Text;

			//get the selected search type
			SearchType searchType = (SearchType)Enum.Parse(typeof(SearchType), SearchTypeDropDownList.SelectedValue, true);

			//get the show state for the excerpts
			bool showExcerpts = bool.Parse(ShowExcerptsDropDownList.SelectedValue);			

			//create a search criteria object and perform the search
			ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, 100, 10, 0, "", showExcerpts, null);
			
			//perform the search
			ISearchResults searchResults = site.SiteIndex.Search(searchCriteria);
            
			//store the results in our session
			Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = searchResults;
		}*/
        /// <summary>
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        /// <summary>
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        protected void NextLinkButton_Click(object sender, EventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get our current search results and search criteria
            ISearchResults searchResults = (ISearchResults) Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];
            ISearchCriteria searchCriteria = searchResults.SearchCriteria;

            //adjust the page offset of the search criteria
            searchCriteria.PageOffset = searchCriteria.PageOffset + searchCriteria.PageSize;

            //perform the search
            searchResults = site.SiteIndex.Search(searchCriteria);

            //store the results back to our session
            Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = searchResults;
        }

        /// <summary>
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        protected void PrevHitsLinkButton_Click(object sender, EventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get our current search results and search criteria
            ISearchResults searchResults = (ISearchResults) Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];
            ISearchCriteria searchCriteria = searchResults.SearchCriteria;

            //adjust the page offset of the search criteria
            searchCriteria.PageOffset = searchCriteria.PageOffset - searchCriteria.PageSize;

            //perform the search
            searchResults = site.SiteIndex.Search(searchCriteria);

            //store the results back to our session
            Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = searchResults;
        }

        protected void searchSubmit_Click(object sender, ImageClickEventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get our search keywords
            string keywords = SearchTextBox.Text;
            searchNotFound.Text = "<p class='SearchSuggestions'><b>Your Search for - <font color='darkred'>" + keywords +
                                  "</font> - did not match any document</b></p>";

            //get the selected search type
            SearchType searchType = SearchType.AnyWords;
            if (AllWordsRadioButton.Checked)
            {
                searchType = SearchType.AllWords;
            }
            else if (AnyWordsRadioButton.Checked)
            {
                searchType = SearchType.AnyWords;
            }
            else if (PhraseRadioButton.Checked)
            {
                searchType = SearchType.ExactPhrase;
            }
            else if (BooleanRadioButton.Checked)
            {
                searchType = SearchType.Boolean;
            }

            //get the show state for the excerpts
            bool showExcerpts = ShowExcerptsCheckBox.Checked;

            //get the filter unsubscribed state
            bool searchUnsubscribed = SearchUnsubscribedCheckBox.Checked;

            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, 100, 10, 0, "", showExcerpts,
                                                                !searchUnsubscribed, null);

            //perform the search
            ISearchResults searchResults = site.SiteIndex.Search(searchCriteria);

            //store the results in our session
            Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = searchResults;
        }

        protected void expandeCollapseRefinement(object sender, ImageClickEventArgs e)
        {
            if (Session["expandedRefinementSearch"] == null || Session["expandedRefinementSearch"].ToString() == "false")
            {
                Session["expandedRefinementSearch"] = "true";
                showRefinementSearchDimensions.ImageUrl = "~/images/portal/topicbar_up.gif";
            }
            else
            {
                Session["expandedRefinementSearch"] = "false";
                showRefinementSearchDimensions.ImageUrl = "~/images/portal/topicbar_down.gif";
            }
        }


        private void topSearch_Submitted(string searchTxt)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get our search keywords
            string keywords = searchTxt;
            SearchTextBox.Text = searchTxt;

            //get the selected search type
            SearchType searchType = (SearchType) Enum.Parse(typeof (SearchType), "AllWords", true);

            //get the show state for the excerpts
            bool showExcerpts = true;

            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, 100, 10, 0, "", showExcerpts,
                                                                true, null);
            //searchCriteria.FilterUnsubscribed = true;
            searchCriteria.FilterUnsubscribed = !SearchUnsubscribedCheckBox.Checked;

            //perform the search
            ISearchResults searchResults = site.SiteIndex.Search(searchCriteria);

            //store the results in our session
            Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = searchResults;

            searchNotFound.Text = "<p class='SearchSuggestions'><b>Your Search for - <font color='darkred'>" + searchTxt +
                                  "</font> - did not match any document</b></p>";
        }

        protected void showRefinementSearchDimensionsLink_Click(object sender, EventArgs e)
        {
            expandeCollapseRefinement(sender, null);
        }

        protected void cleanSearch_Click(object sender, EventArgs e)
        {
            Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS] = null;
            jsLabel.Text = "<script>clearSearch();</script>";
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        ///<summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        ///</summary>
        private void InitializeComponent()
        {
            this.PreRender += new System.EventHandler(this.D_Search_PreRender);
        }

        #endregion
    }
}