#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using Endeca.Data;
using Endeca.Data.Provider.PresentationApi;
using MainUI.Shared;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.Content.Document;

#endregion

namespace MainUI.WS

{
    /// <summary>
    ///   Summary description for EndecaServices
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]



    public class EndecaServices : AicpaService
    {
        //Set the endeca server from the config file.
        //Server name
        private readonly string _mdexHost = ConfigurationManager.AppSettings["EndecaHostname"];
        //Port
        private readonly int _mdexPort = int.Parse(ConfigurationManager.AppSettings["EndecaPort"]);

        /// <summary>
        ///   Gets the book by id.
        /// </summary>
        /// <param name = "keywords">The keywords.</param>
        /// <param name = "searchMode">The search mode.</param>
        /// <param name = "maxHits">The max hits.</param>
        /// <param name = "pageSize">Size of the page.</param>
        /// <param name = "showExcerpts">if set to <c>true</c> [show excerpts].</param>
        /// <param name = "filterUnsubscribed">The filter unsubscribed.</param>
        /// <param name = "pageOffset">The page offset.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "This method returns the search results based on the serch terms.")]
        public SearchResultResponse EndecaSearch(string keywords, int searchMode, int maxHits, int pageSize,
                                                 int pageOffset, int showExcerpts, int filterUnsubscribed)
        {
            SearchType searchType;

            bool showExcerptsBool;
            bool filterUnsubscribedBool;

            switch (showExcerpts)
            {
                case 1:
                    showExcerptsBool = true;
                    break;
                default:
                    showExcerptsBool = false;
                    break;
            }

            switch (filterUnsubscribed)
            {
                case 1:
                    filterUnsubscribedBool = true;
                    break;
                default:
                    filterUnsubscribedBool = false;
                    break;
            }

            switch (searchMode)
            {
                case 1:
                    searchType = SearchType.AllWords;
                    break;
                case 2:
                    searchType = SearchType.AnyWords;
                    break;
                default:
                    searchType = SearchType.ExactPhrase;
                    break;
            }
            SearchResultResponse searchResultResponse = new SearchResultResponse();

            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize,
                                                                pageOffset, "", showExcerptsBool, filterUnsubscribedBool,
                                                                null);

            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            //perform the search
            ContextManager.SearchResults = CurrentSite.SiteIndex.Search(searchCriteria);
            searchResultResponse.DimensionXml = ContextManager.SearchResults.DimensionsXml;
            searchResultResponse.SearchResults =
                (from doc in ContextManager.SearchResults.Documents where doc != null select new SearchResult(doc)).
                    ToList();
            searchResultResponse.SearchTerm = keywords;
            searchResultResponse.HitCount = (int) ContextManager.SearchResults.TotalHitCount;
            searchResultResponse.DisplayOffset = pageOffset;
            DimensionResultSet dimensionResultSet = DimensionsResults(ContextManager.SearchResults);
            searchResultResponse.DimensionResults = dimensionResultSet.DimensionResults;
            searchResultResponse.SelectedDimensionResults = dimensionResultSet.SelectedDimensionResults;
            searchResultResponse.Unsubscribed = filterUnsubscribed;
            searchResultResponse.Excerpts = showExcerpts;
            searchResultResponse.SearchMode = searchMode;
            

            int y = 0;
            foreach (string word in ContextManager.SearchResults.WordInterpretations)
            {
                if (y > 0)
                {
                    searchResultResponse.WordIntepretations = searchResultResponse.WordIntepretations + ", " + word;
                    y++;
                }
                else
                {
                    searchResultResponse.WordIntepretations = word;
                    y++;
                }
            }
            ;

            int x = 0;
            foreach (SearchResult result in searchResultResponse.SearchResults)
            {
                result.ResultEnumeration = x++;
            }
            x = x + pageOffset;
            searchResultResponse.DisplayResults = x;

            return searchResultResponse;
        }

        /// <summary>
        ///   Endecas the advanced search.
        /// </summary>
        /// <param name = "dimensionId">The dimension id.</param>
        /// <param name = "keywords">The keywords.</param>
        /// <param name = "searchMode">The search mode.</param>
        /// <param name = "maxHits">The max hits.</param>
        /// <param name = "pageSize">Size of the page.</param>
        /// <param name = "pageOffset">The page offset.</param>
        /// <param name = "showExcerpts">The show excerpts.</param>
        /// <param name = "filterUnsubscribed">The filter unsubscribed.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "This method returns the search results based on the serch terms.")]
        public SearchResultResponse EndecaAdvancedSearch(string dimensionId, string keywords, int searchMode,
                                                         int maxHits, int pageSize,
                                                         int pageOffset, int showExcerpts, int filterUnsubscribed, int nonauthoritative)
        {
            
            string[] dimensionIds = {dimensionId};

            if (dimensionId == null || dimensionId == "")
            {
                 dimensionIds = BlankDimensions();
            } 
            
            SearchType searchType;

            bool showExcerptsBool;
            bool filterUnsubscribedBool;

            System.Collections.Specialized.NameValueCollection opts = new System.Collections.Specialized.NameValueCollection();
            

            switch (showExcerpts)
            {
                case 1:
                    showExcerptsBool = true;
                    break;
                default:
                    showExcerptsBool = false;
                    break;
            }

            switch (filterUnsubscribed)
            {
                case 1:
                    filterUnsubscribedBool = true;
                    break;
                default:
                    filterUnsubscribedBool = false;
                    break;
            }

            switch (nonauthoritative)
            {
                case 1:
                    opts.Add("nonauthoritative", "true");
                    string value = opts["nonauthoritative"];
                    break;
                default:
                    opts = null;
                    break;
            }

            switch (searchMode)
            {
                case 1:
                    searchType = SearchType.AllWords;
                    break;
                case 2:
                    searchType = SearchType.AnyWords;
                    break;
                case 3:
                    searchType = SearchType.ExactPhrase;
                    break;
                case 4:
                    searchType = SearchType.Boolean;
                    break;
                default:
                    searchType = SearchType.AllWords;
                    break;
            }
            SearchResultResponse searchResultResponse = new SearchResultResponse();

            //Temporarily log the search parameters getting sent to Endeca
            //IEvent logEvent = new Event(EventType.Info, DateTime.Now, 1, "EndecaService", "Search Initiated", "Search Parmeters", string.Format("Search Parameters  {0}, {1},{2}, {3},{4}, {5},{6}, {7} ", dimensionIds[0].ToString(), keywords,searchType, maxHits,pageSize,pageOffset,showExcerptsBool,filterUnsubscribedBool));
            //logEvent.Save(false);


            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, CleanInput(keywords).Trim(), searchType, maxHits, pageSize,
                                                                pageOffset, "", showExcerptsBool, filterUnsubscribedBool,
                                                                opts);


            ContextManager.SearchCriteria = searchCriteria;
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            //perform the search
            //List<DimensionNavigationResult> tempList = DimensionsResults(ContextManager.SearchResults);
            ISearchResults searchResults = ContextManager.SearchResults;
            ContextManager.SearchResults = CurrentSite.SiteIndex.Search(searchCriteria);
            searchResultResponse.DimensionXml = ContextManager.SearchResults.DimensionsXml;
            searchResultResponse.SearchResults =
                (from doc in ContextManager.SearchResults.Documents where doc != null select new SearchResult(doc)).
                    ToList();
            searchResultResponse.SearchTerm = CleanInput(keywords).Trim();
            searchResultResponse.HitCount = (int) ContextManager.SearchResults.TotalHitCount;
            searchResultResponse.DisplayOffset = pageOffset;
            searchResultResponse.DimensionId = dimensionId;
            DimensionResultSet dimensionResultSet = DimensionsResults(ContextManager.SearchResults);
            searchResultResponse.DimensionResults = dimensionResultSet.DimensionResults;
            searchResultResponse.SelectedDimensionResults = dimensionResultSet.SelectedDimensionResults;
            searchResultResponse.Unsubscribed = filterUnsubscribed;
            searchResultResponse.Excerpts = showExcerpts;
            searchResultResponse.SearchMode = searchMode;
            ContextManager.SearchResults.DocumentIDCache = new List<int>();
            ContextManager.SearchResults.DocumentIDCache.AddRange(ContextManager.SearchResults.Documents.Select(xa => xa.Id).ToList());
            searchResultResponse.nonauthoritative = nonauthoritative;

            int y = 0;
            foreach (string word in ContextManager.SearchResults.WordInterpretations)
            {
                if (y > 0)
                {
                    searchResultResponse.WordIntepretations = searchResultResponse.WordIntepretations + ", " + word;
                    y++;
                }
                else
                {
                    searchResultResponse.WordIntepretations = word;
                    y++;
                }
            }
            ;

            int x = 0;
            foreach (SearchResult result in searchResultResponse.SearchResults)
            {
                result.ResultEnumeration = x++;
                //certain words aren't having the snippets highlight properly.  This fixes that issue.
                if (!result.Snippet.Contains("endeca_term"))
                { 
                    foreach (string word in ContextManager.SearchResults.WordInterpretations)
                    {
                        string myword = word.Replace("(","\\(");
                        myword = myword.Replace(")", "\\)");
                        result.Snippet = Regex.Replace(result.Snippet, " " + myword+" ", "<b class='endeca_term'>"+word+"</b>", RegexOptions.IgnoreCase);
                    }
                }
            }
            x = x + pageOffset;
            searchResultResponse.DisplayResults = x;
            //if there are no hits the following statement gets dimensions from a blank search and populates dimension values in the original searchResults
            if (searchResultResponse.DimensionResults.Count == 0)
            {
                //dimensionResultResponse.DimensionResults = tempList;
                //searchResultResponse.DimensionResults = tempList; 
                //ContextManager.SearchResults = searchResults;
            }

            try
            {
                //log Search
                // 2010-12-28 sburton: not sure that EventType.Error is really the appropriate level for this,
                // but this is how it was in the old one, so I'm just trying to be consistent.
                IEvent eventError = new Event(EventType.Error, DateTime.Now, 5, SEARCH_MODULE, keywords,
                                             searchResultResponse.HitCount.ToString(), "User Search", ContextManager.CurrentUser.UserId, ContextManager.CurrentUser.UserSecurity.SessionId);

                // The old event didn't use the user and session ID but I figured those were helpful, and we have them
                //IEvent eventError = new Event(EventType.Error, DateTime.Now, 5, SEARCH_MODULE, keywords,
                //                             searchResultResponse.HitCount.ToString(), "User Search");
                eventError.Save(false);
            }
            catch
            {
                // hmm... we couldn't log the event.  We really should try to log the fact that we couldn't log, but you know...
            }
            
            return searchResultResponse;
        }


        /// <summary>
        /// Does the blank search.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "This method returns the search results based on the serch terms.")]
        public SearchResultResponse DoBlankSearch()
        {
            string[] dimendionIds = BlankDimensions();
            SearchResultResponse searchResultResponse = new SearchResultResponse();
            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(dimendionIds, "", SearchType.AnyWords, 10, 10, 0, "", false, true, null);
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            ISearchResults searchResults = CurrentSite.SiteIndex.Search(searchCriteria);
            searchResultResponse.DimensionXml = searchResults.DimensionsXml;
            DimensionResultSet dimensionResultSet = DimensionsResults(searchResults);
            searchResultResponse.DimensionResults = dimensionResultSet.DimensionResults;
            searchResultResponse.SelectedDimensionResults = dimensionResultSet.SelectedDimensionResults;
            return searchResultResponse;
        }

        /// <summary>
        ///   Search with current criteria.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true,
            Description = "This method returns searchresults based on the serch terms and dimensions on the session.")]
        public SearchResultResponse EndecaSearchWithCurrentCriteria()
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            if (ContextManager.SearchCriteria == null)
            {
                SearchResultResponse searchResultResponse = new SearchResultResponse();
                searchResultResponse.DimensionId = "-1";
                return searchResultResponse;
            }
            return ResultSet(ContextManager.SearchCriteria);
        }

        [WebMethod(true,
            Description = "Returns the id and type of the next document in the search results relative to the given id")]
        public HitDocResult EndecaNextHitDoc(int id, string type)
        {
            HitDocResult result = new HitDocResult();

            int next = -1;
            //extra catch
            if (ContextManager.SearchResults == null)
            {
                result.Id = -1;
                result.Type = string.Empty;
                return result;
            }

            List<int> cache = ContextManager.SearchResults.DocumentIDCache;

            if ((cache != null) && (cache.Where(x => x == id).Count() > 0))
            {
                int val = cache.Where(x => (x == id)).FirstOrDefault();
                next = cache.IndexOf(val) + 1;
                if (next >= cache.Count)
                    next = -1;
            }

            if (next == -1)
            {
                int pOffset = ContextManager.SearchCriteria.PageOffset;
                ContextManager.SearchCriteria.PageOffset = Math.Min((ContextManager.SearchCriteria.PageOffset + ContextManager.SearchCriteria.PageSize), (int)ContextManager.SearchResults.TotalHitCount);
                if (ContextManager.SearchCriteria.PageOffset == ContextManager.SearchResults.TotalHitCount)
                {
                    ContextManager.SearchCriteria.PageOffset = pOffset;
                    return new HitDocResult() { Id = -1, Type = string.Empty };
                }

                ISearchResults searchResults = CurrentSite.SiteIndex.Search(ContextManager.SearchCriteria);
                
                ContextManager.SearchResults.DocumentIDCache.AddRange(searchResults.Documents.Select(xa => xa.Id).ToList());
                result.Id = searchResults.Documents[0].Id;
                result.Type = "Document";
                ContextManager.SearchCriteria.PageOffset = pOffset;
            }
            else
            {
                result.Id = ContextManager.SearchResults.DocumentIDCache[next];
                result.Type = "Document";//ContextManager.SearchResults.Documents[index]. GetType().ToString();
            }

            return result;
        }

        [WebMethod(true,
            Description = "Returns the id and type of the previous document in the search results relative to the given id")]
        public HitDocResult EndecaPrevHitDoc(int id, string type)
        {
            HitDocResult result = new HitDocResult();

            int prev = -1;
            //extra catch
            if (ContextManager.SearchResults == null)
            {
                result.Id = -1;
                result.Type = string.Empty;
                return result;
            }
            List<int> cache = ContextManager.SearchResults.DocumentIDCache;
            if ((cache != null) && (cache.Where(x => x == id).Count() > 0))
            {
                int val = cache.Where(x => (x == id)).FirstOrDefault();
                prev = cache.IndexOf(val) - 1;
            }

            if (prev < 0)//>= ContextManager.SearchResults.PageOffset + ContextManager.SearchCriteria.PageSize)
            {
                if (ContextManager.SearchResults.PageOffset == 0)
                    return new HitDocResult() { Id = -1, Type = string.Empty };
                int pOffset = ContextManager.SearchCriteria.PageOffset;
                ContextManager.SearchCriteria.PageOffset = Math.Max((ContextManager.SearchCriteria.PageOffset - ContextManager.SearchCriteria.PageSize), 0);
                
                ISearchResults searchResults = CurrentSite.SiteIndex.Search(ContextManager.SearchCriteria);            
                ContextManager.SearchCriteria.PageOffset = pOffset;

                ContextManager.SearchResults.DocumentIDCache.InsertRange(0, searchResults.Documents.Select(x => x.Id).ToList());

                result.Id = searchResults.Documents[ContextManager.SearchResults.Documents.Length - 1].Id;
                result.Type = "Document";//ContextManager.SearchResults.Documents[0].GetType().ToString();
            }
            else
            {
                result.Id = ContextManager.SearchResults.DocumentIDCache[prev];
                result.Type = "Document";//ContextManager.SearchResults.Documents[index].GetType().ToString();
            }

            return result;
        }

        /// <summary>
        ///   Sets the search criteria.
        /// </summary>
        /// <param name = "dimensionIds">The dimension ids.</param>
        /// <param name = "keywords">The keywords.</param>
        /// <param name = "searchType">Type of the search.</param>
        /// <param name = "maxHits">The max hits.</param>
        /// <param name = "pageSize">Size of the page.</param>
        /// <param name = "pageOffset">The page offset.</param>
        /// <param name = "showExcerpts">if set to <c>true</c> [show excerpts].</param>
        /// <param name = "searchUnsubscribed">if set to <c>true</c> [search unsubscribed].</param>
        /// <returns></returns>
        [WebMethod(true, Description = "Set search criteria.")]
        public bool SetSearchCriteria(string[] dimensionIds, string keywords, SearchType searchType, int maxHits,
                                      int pageSize,
                                      int pageOffset, bool showExcerpts, bool searchUnsubscribed)
        {
            bool resultCriteria = false;
            try
            {
                //Create new SearchCriteria
                ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, keywords, searchType, maxHits,
                                                                    pageSize,
                                                                    pageOffset, "", showExcerpts, !searchUnsubscribed,
                                                                    null);
                //Set search criteria on the session.
                ContextManager.SearchCriteria = searchCriteria;
                resultCriteria = true;
            }
            catch
            {
                //TODO: Log eror.
                resultCriteria = false;
            }
            return resultCriteria;
        }


        /// <summary>
        ///   Dimensionses the results.
        /// </summary>
        /// <param name = "currentSearchResults">The current search results.</param>
        /// <returns>A list of navigational search results.</returns>
        private DimensionResultSet DimensionsResults(ISearchResults currentSearchResults)
        {
           DimensionResultSet dimensionResultSet = new DimensionResultSet();
            List<DimensionNavigationResult> dimensionResults = new List<DimensionNavigationResult>();
            List<DimensionNavigationResult> dimensionSelectedResults = new List<DimensionNavigationResult>();
            //...if we do have a search results object in our session, populate the search results table
            if (currentSearchResults != null)
            {
                //load the dimensions xml
                XmlDocument dimensionsXmlDoc = new XmlDocument();
                dimensionsXmlDoc.LoadXml(currentSearchResults.DimensionsXml);

                //render the selected dimensions info
                //render the refinement dimensions info
                XmlNodeList refinementDimensionNodes =
                    dimensionsXmlDoc.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSREFINEMENTDIMENSIONS + "/" +
                                                 DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSION);
                XmlNodeList selectedDimensionNodes =
                   dimensionsXmlDoc.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONS + "/" +
                                                DestroyerBpc.XML_ELE_SEARCHRESULTSSELECTEDDIMENSIONS + "/" +
                                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSION);

                foreach (XmlNode selectedDimensionNode in selectedDimensionNodes)
                {
                    //the html we will insert for the selected dimension
                    
                    //get the selected dimension name and id
                    XmlNode dimensionNameNode =
                        selectedDimensionNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONNAME);
                    XmlNode dimensionIdNode =
                        selectedDimensionNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONID);
                    DimensionNavigationResult dimensionResultCurrent = new DimensionNavigationResult();
                    dimensionResultCurrent.DimensionName = dimensionIdNode.InnerText;
                    dimensionResultCurrent.DimensionValue = dimensionNameNode.InnerText;
                    //go through each of the ancestors of the dimension and build a path in our HTML
                    XmlNodeList selectedDimensionAncestorNodes =
                        selectedDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORS + "/" +
                                                          DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEM);
                    foreach (XmlNode selectedDimensionAncestorNode in selectedDimensionAncestorNodes)
                    {
                        DimensionNavigationResult dimensionResult = new DimensionNavigationResult();
                        XmlNode selectedDimensionAncestorNameNode = selectedDimensionAncestorNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMNAME);
                        XmlNode selectedDimensionAncestorIdNode = selectedDimensionAncestorNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMID);
                        dimensionResult.DimensionName = selectedDimensionAncestorIdNode.InnerText;
                        dimensionResult.DimensionValue = selectedDimensionAncestorNameNode.InnerText;
                        dimensionSelectedResults.Add(dimensionResult);
 
                    }
                    dimensionSelectedResults.Add(dimensionResultCurrent);
                }
                dimensionResultSet.SelectedDimensionResults = dimensionSelectedResults;
                foreach (XmlNode refinementDimensionNode in refinementDimensionNodes)
                {
                    ////go through each of the complete path nodes of the dimension
                    //XmlNodeList selectedDimensionCompletePathNodes =
                    //    refinementDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATH +
                    //                                        "/" +
                    //                                        DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEM);

                    //foreach (XmlNode selectedDimensionCompletePathNode in selectedDimensionCompletePathNodes)
                    //{
                    //    DimensionNavigationResult dimensionResult = new DimensionNavigationResult();
                    //    dimensionResult.DimensionName =
                    //        selectedDimensionCompletePathNode.SelectSingleNode(
                    //            DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONNAME).InnerText;
                    //    dimensionResult.DimensionId =
                    //        selectedDimensionCompletePathNode.SelectSingleNode(
                    //            DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONID).InnerText;
                    //    if (
                    //        selectedDimensionCompletePathNode.SelectSingleNode(
                    //            DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE) != null)
                    //    {
                    //        dimensionResult.DimensionValue =
                    //            selectedDimensionCompletePathNode.SelectSingleNode(
                    //                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE).
                    //                InnerText;
                    //    }
                    //    else
                    //    {
                    //        dimensionResult.DimensionValue = "";
                    //    }
                    //    dimensionResults.Add(dimensionResult);
                    //}

                    //go through each of the current refinement dimensions
                    XmlNodeList dimensionValueNodes =
                        refinementDimensionNode.SelectNodes(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE);
                    foreach (XmlNode dimensionValueNode in dimensionValueNodes)
                    {
                        DimensionNavigationResult dimensionResult = new DimensionNavigationResult();
                        dimensionResult.DimensionName =
                            dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONNAME).
                                InnerText;
                        dimensionResult.DimensionId =
                            dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONID).InnerText;
                        if (dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE) !=
                            null)
                        {
                            dimensionResult.DimensionValue =
                                dimensionValueNode.SelectSingleNode(DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUE).
                                    InnerText;
                        }
                        else
                        {
                            dimensionResult.DimensionValue = "";
                        }
                        dimensionResult.DimensionValue =
                            dimensionValueNode.SelectSingleNode(
                                DestroyerBpc.XML_ELE_SEARCHRESULTSDIMENSIONVALUERECORDCOUNT).InnerText;
                        dimensionResults.Add(dimensionResult);
                    }
                }
            }
            dimensionResultSet.DimensionResults = dimensionResults;
            return dimensionResultSet;
        }

        protected const string SEARCH_MODULE = "Search";

        /// <summary>
        /// Results the set.
        /// </summary>
        /// <param name="searchCriteriaExisting">The search criteria existing.</param>
        /// <returns></returns>
        private SearchResultResponse ResultSet(ISearchCriteria searchCriteriaExisting)
        {
            int searchMode;
            int showExcerpts;
            int filterUnsubscribed;
            switch (searchCriteriaExisting.SearchType)
                {
                    case SearchType.AllWords:
                        searchMode = 1;
                        break;
                    case SearchType.AnyWords:
                        searchMode = 2;
                        break;
                    default:
                        searchMode = 3;
                        break;
                }

                 switch (searchCriteriaExisting.Excerpts)
                {
                    case true:
                        showExcerpts = 1;
                        break;
                    default:
                        showExcerpts = 0;
                        break;
                }

                switch (searchCriteriaExisting.FilterUnsubscribed)
                {
                    case true:
                        filterUnsubscribed = 1;
                        break;
                    default:
                        filterUnsubscribed = 0;
                        break;
                }
            SearchResultResponse searchResultResponse = new SearchResultResponse();
            SearchResultResponse dimensionResultResponse = new SearchResultResponse();
            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = searchCriteriaExisting;

            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            //perform the search
            //List<DimensionNavigationResult> tempList = DimensionsResults(ContextManager.SearchResults);
            ISearchResults searchResults = ContextManager.SearchResults;
            ContextManager.SearchResults = CurrentSite.SiteIndex.Search(searchCriteria);
            searchResultResponse.DimensionXml = ContextManager.SearchResults.DimensionsXml;
            searchResultResponse.SearchResults =(from doc in ContextManager.SearchResults.Documents where doc != null select new SearchResult(doc)).
                    ToList();
            searchResultResponse.SearchTerm = searchCriteriaExisting.Keywords;
            searchResultResponse.HitCount = (int)ContextManager.SearchResults.TotalHitCount;
            searchResultResponse.DisplayOffset = searchCriteriaExisting.PageOffset;

            string newString = string.Join(",", searchCriteriaExisting.DimensionIds);
            searchResultResponse.DimensionId = newString;

            //foreach(string dimensionId in searchCriteriaExisting.DimensionIds)
            //{
            //    searchResultResponse.DimensionId. = searchCriteriaExisting.DimensionIds;    
            //}

            DimensionResultSet dimensionResultSet = DimensionsResults(ContextManager.SearchResults);
            searchResultResponse.DimensionResults = dimensionResultSet.DimensionResults;
            searchResultResponse.SelectedDimensionResults = dimensionResultSet.SelectedDimensionResults;
            searchResultResponse.Unsubscribed = filterUnsubscribed;
            searchResultResponse.Excerpts = showExcerpts;
            searchResultResponse.SearchMode = searchMode;

            int y = 0;
            foreach (string word in ContextManager.SearchResults.WordInterpretations)
            {
                if (y > 0)
                {
                    searchResultResponse.WordIntepretations = searchResultResponse.WordIntepretations + ", " + word;
                    y++;
                }
                else
                {
                    searchResultResponse.WordIntepretations = word;
                    y++;
                }
            }
            ;

            int x = 0;
            foreach (SearchResult result in searchResultResponse.SearchResults)
            {
                result.ResultEnumeration = x++;
                //certain words aren't having the snippets highlight properly.  This fixes that issue.
                if (!result.Snippet.Contains("endeca_term"))
                {
                    foreach (string word in ContextManager.SearchResults.WordInterpretations)
                    {
                        string myword = word.Replace("(", "\\(");
                        myword = myword.Replace(")", "\\)");
                        result.Snippet = Regex.Replace(result.Snippet, " " + myword + " ", "<b class='endeca_term'>" + word + "</b>", RegexOptions.IgnoreCase);
                    }
                }

            }
            x = x + searchCriteriaExisting.PageOffset;
            searchResultResponse.DisplayResults = x;

            return searchResultResponse;
        }

        private string[] BlankDimensions()
        {
            PresentationApiConnection conn = new PresentationApiConnection(_mdexHost, _mdexPort);
            DimensionSearchCommand dsc = new DimensionSearchCommand(conn);

            dsc.SearchTerms = ConfigurationManager.AppSettings["ContentRootNode"];
            dsc.SearchMode = SearchMode.All;
            

            DimensionSearchResult dsr = dsc.Execute();

            ReadOnlyDictionary<string, ReadOnlyCollection<DimensionValue>> byDimension =
            dsr.DimensionValues.ByDimensionName;
            string dimensions = "";
            // You can iterate over the dictionary and print out the name of the dimension 
            // and all the results beneath it.
            foreach (KeyValuePair<string, ReadOnlyCollection<DimensionValue>> pair in byDimension)
            {
                foreach (DimensionValue dval in pair.Value)
                {
                    
                   if (dval.DisplayName.EndsWith(dsc.SearchTerms))
                   {
                         dimensions = dimensions + dval.Id;
                   }

                   // IEvent logEvent = new Event(EventType.Info, DateTime.Now, 1, "EndecaService", "Dimension Information", "", string.Format("Dimension values  {0}, {1} ", dval.Id,dval.DisplayName));
                    //logEvent.Save(false);



                }
            }

            string[] dimendionIds = { dimensions };
            return dimendionIds;
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            //In addition to alphanumeric characters The following characters are supported by the index _-?\&*$!@#%()'

            string strOut = Regex.Replace(strIn, @"[^\w\.@',\\$\\*&_\\?!%\\(\\)-]", " ");
            return strOut;
        }
        #region Nested type: SearchResultResponse

        [Serializable]
        public struct SearchResultResponse
        {
            public string DimensionId;
            public List<DimensionNavigationResult> DimensionResults;
            public List<DimensionNavigationResult> SelectedDimensionResults;
            public string DimensionXml;
            public int DisplayOffset;
            public int DisplayResults;
            public int Excerpts;
            public int HitCount;
            public int SearchMode;
            public List<SearchResult> SearchResults;
            public string SearchTerm;
            public int Unsubscribed;
            public string WordIntepretations;
            public int nonauthoritative;
        }

        public struct DimensionResultSet
        {
            public List<DimensionNavigationResult> DimensionResults;
            public List<DimensionNavigationResult> SelectedDimensionResults;
           
            
        }
        #endregion
    }
}