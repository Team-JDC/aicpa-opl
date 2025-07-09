using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Web.Script.Services;
using System.Web.Services;

using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;

namespace MainUI.WS
{
    /// <summary>
    ///   Summary description for SearchServices
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class SearchServices : AicpaService
    {
        /// <summary>
        ///   Gets the saved searches.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "This service returns a list of saved user searches.")]
        public SavedSearches GetSavedSearches()
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            SearchCollection searchCollection = CurrentUser.SavedSearches;
            return GetUserSavedSearchCollection(searchCollection);
        }

        /// <summary>
        /// Saves the user search.
        /// </summary>
        /// <param name="searchName">Name of the search.</param>
        /// <param name="dimensionId">The dimension id.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="maxHits">The max hits.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageOffset">The page offset.</param>
        /// <param name="showExcerpts">The show excerpts.</param>
        /// <param name="filterUnsubscribed">The filter unsubscribed.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "This saves a user search.")]
        public SavedSearches SaveUserSearch(string searchName, string dimensionId, string keywords, int searchMode,
                                                         int maxHits, int pageSize,
                                                         int pageOffset, int showExcerpts, int filterUnsubscribed)
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);

            string[] dimensionIds = { "" };
            bool showExcerptsBool = showExcerpts == 1;
            bool filterUnsubscribedBool = filterUnsubscribed == 1;
            SearchType searchType;

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
                    searchType = SearchType.ExactPhrase;
                    break;
            }

            SearchCollection searchCollection = CurrentUser.SavedSearches;

            if (searchCollection.Contains(searchName))
            {
                ISearchCriteria existingSearch = searchCollection[searchName];

                existingSearch.DimensionIds = dimensionIds;
                existingSearch.Keywords = keywords;
                existingSearch.SearchType = searchType;
                existingSearch.MaxHits = maxHits;
                existingSearch.PageSize = pageSize;
                existingSearch.PageOffset = pageOffset;
                existingSearch.Excerpts = showExcerptsBool;
                existingSearch.FilterUnsubscribed = filterUnsubscribedBool;

                existingSearch.Save();
            }
            else
            {
                ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, keywords, searchType, maxHits, pageSize,
                                                                pageOffset, "", showExcerptsBool, filterUnsubscribedBool,
                                                                null);
                searchCollection.Add(searchCriteria, searchName, CurrentUser.UserId);
            }

            return GetUserSavedSearchCollection(searchCollection);
        }

        /// <summary>
        /// Deletes the user saved search.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "This deletes a user search matching the name given.")]
        public SavedSearches DeleteUserSavedSearch(string name)
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            CurrentUser.SavedSearches.Remove(name);
            SearchCollection searchCollection = CurrentUser.SavedSearches;
            return GetUserSavedSearchCollection(searchCollection);
        }

        /// <summary>
        /// Renames the user saved search.
        /// </summary>
        /// <param name="name">The old name.</param>
        /// <param name="newName">The new name to give the search.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "This renames a user search matching the name given.")]
        public SavedSearches RenameUserSavedSearch(string name, string newName)
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            CurrentUser.SavedSearches[name].Save(newName);
            SearchCollection searchCollection = CurrentUser.SavedSearches;
            return GetUserSavedSearchCollection(searchCollection);
        }

        private SavedSearches GetUserSavedSearchCollection(SearchCollection searchCollection)
        {
            SavedSearches savedSearches = new SavedSearches {SavedUserSearches = new List<SavedSearch>()};

            foreach (SearchCriteria search in searchCollection)
            {
                SavedSearch savedSearch = new SavedSearch
                                              {
                                                  Keywords = search.Keywords,
                                                  MaxHits = search.MaxHits,
                                                  PageOffset = search.PageOffset,
                                                  PageSize = search.PageSize,
                                                  SearchId = search.SaveId,
                                                  SearchName = search.Name,
                                                  ShowExcerpts = search.Excerpts ? 1 : 0,
                                                  FilterUnsubscribed = search.FilterUnsubscribed ? 1 : 0
                                              };

                switch (search.SearchType)
                {
                    case SearchType.AllWords:
                        savedSearch.SearchMode = 1;
                        break;
                    case SearchType.AnyWords:
                        savedSearch.SearchMode = 2;
                        break;
                    case SearchType.ExactPhrase:
                        savedSearch.SearchMode = 3;
                        break;
                    case SearchType.Boolean:
                        savedSearch.SearchMode = 4;
                        break;
                    default:
                        savedSearch.SearchMode = 3;
                        break;
                }

                savedSearches.SavedUserSearches.Add(savedSearch);
            }

            return savedSearches;
        }

        [WebMethod(true, CacheDuration = 31536000, Description = "This method returns the whole list of search terms.")]
        public string[] GetSuggestedSearchTerms()
        {
            // Just fetch the table from the database and return the list, nothing fancy
            return new SearchDalc().GetSuggestedSearchTerms();
        }

        [WebMethod(true, Description = "Add a use to a suggested search term.")]
        public void IncrementSuggestedSearchTerm(string term)
        {
            // Non-query to increment use column of db.
            new SearchDalc().IncrementSuggestedTerm(term);
        }
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public struct SavedSearch
    {
        public string DimensionIds;
        public int FilterUnsubscribed;
        public string Keywords;
        public int MaxHits;
        public int PageOffset;
        public int PageSize;
        public int SearchId;
        public int SearchMode;
        public string SearchName;
        public int ShowExcerpts;
    }

    [Serializable]
    public struct SavedSearches
    {
        public List<SavedSearch> SavedUserSearches;
    }
}