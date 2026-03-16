using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using MainUI.Shared;
using MainUI.Shared.Elastic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
namespace MainUI.WS
{
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class ElasticSearchService : AicpaService
    {
        protected const string SEARCH_MODULE = "ElasticSearchService";
        private static readonly string elasticUrl = ConfigurationManager.AppSettings["ElasticSearchEndpoint"];
        private static readonly string indexName = ConfigurationManager.AppSettings["ElasticSearchIndex"];
        private static readonly string apiKey = ConfigurationManager.AppSettings["ElasticSearchApiKey"];
        private const string SESSION_CRITERIA = "ES_SearchCriteria";
        private const string SESSION_DOCIDS = "ES_LastDocIds";
        [WebMethod(true, Description = "This method returns Elasticsearch-based search results")]
        public MainUI.Shared.Elastic.SearchResultResponse ElasticSearch(string keywords, int maxHits, int searchMode, int pageSize, int pageOffset, int showExcerpts, int filterUnsubscribed)
        {
            EnsureConfigured();

            var elastic = new ElasticServices(elasticUrl, indexName, apiKey);
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
            ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, keywords, searchType, maxHits, pageSize,
                                                       pageOffset, "", showExcerptsBool, filterUnsubscribedBool,
                                                       null);
            ContextManager.SearchCriteria = searchCriteria;
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            //pass userSubscriptionCodes
            string[] domainItems = null;
            if (searchCriteria.FilterUnsubscribed)
            {
                domainItems = ContextManager.CurrentUser.UserSecurity.Domain
                    .Split(new[] { "~" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Replace(";", ""))
                    .ToArray();
            }
            var res = elastic.SearchAsync(searchCriteria, domainItems).GetAwaiter().GetResult();

            // Post-process to match legacy Endeca response semantics
            AfterSearchPostProcess(res, keywords, searchMode, showExcerpts, filterUnsubscribed, pageOffset);

            // Cache for Next/Prev
            CacheLastSearch(res, new EsSearchCriteria
            {
                Keywords = keywords,
                SearchMode = searchMode,
                MaxHits = maxHits,
                PageSize = pageSize,
                PageOffset = pageOffset,
                ShowExcerpts = showExcerpts,
                FilterUnsubscribed = filterUnsubscribed,
                DimensionId = null
            });

            try
            {
                //log Search
                IEvent searchEvent = new Event(EventType.Usage, DateTime.Now, 5, SEARCH_MODULE, "ElasticSearch", res.HitCount.ToString(), keywords,
                                             ContextManager.CurrentUser.UserId, ContextManager.CurrentUser.UserSecurity.SessionId);

                searchEvent.Save(false);
            }
            catch
            {
                // hmm... we couldn't log the event.  We really should try to log the fact that we couldn't log, but you know...
            }

            //string jsonString = JsonConvert.SerializeObject(res);
            return res;
        }
        #region Public WebMethods


        [WebMethod(true, Description = "Elasticsearch: advanced search with optional dimension (maps to subscription filter)")]
        public MainUI.Shared.Elastic.SearchResultResponse ElasticAdvancedSearch(
            string dimensionId, string keywords, int searchMode, int maxHits, int pageSize,
            int pageOffset, int showExcerpts, int filterUnsubscribed, int nonauthoritative)
        {
            EnsureConfigured();


            var elastic = new ElasticServices(elasticUrl, indexName, apiKey);

            string[] dimensionIds = { dimensionId };

            if (dimensionId == null || dimensionId == "")
            {
                dimensionIds = elastic.BlankDimensions();
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
                    searchType = SearchType.ExactPhrase;
                    break;
            }
            SearchResultResponse searchResultResponse = new SearchResultResponse();


            //create a search criteria object and perform the search
            ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, CleanInput(keywords).Trim(), searchType, maxHits, pageSize,
                                                                pageOffset, "", showExcerptsBool, filterUnsubscribedBool,
                                                                opts);


            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);

            //pass userSubscriptionCodes
            string[] domainItems = null;
            if (searchCriteria.FilterUnsubscribed)
            {
                domainItems = ContextManager.CurrentUser.UserSecurity.Domain
                    .Split(new[] { "~" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Replace(";", ""))
                    .ToArray();
            }
            var res = elastic.SearchAsync(searchCriteria, domainItems).GetAwaiter().GetResult();

            var svc = new ElasticServices(elasticUrl, indexName, apiKey);

            // Round-trip the requested selections for the UI (Endeca parity)
            res.DimensionId = dimensionId;

            AfterSearchPostProcess(res, keywords, searchMode, showExcerpts, filterUnsubscribed, pageOffset);
            res.nonauthoritative = nonauthoritative; // echo flag

            CacheLastSearch(res, new EsSearchCriteria
            {
                Keywords = keywords,
                SearchMode = searchMode,
                MaxHits = maxHits,
                PageSize = pageSize,
                PageOffset = pageOffset,
                ShowExcerpts = showExcerpts,
                FilterUnsubscribed = filterUnsubscribed,
                DimensionId = dimensionId
            });

            try
            {
                //log Search
                IEvent searchEvent = new Event(EventType.Error, DateTime.Now, 5, SEARCH_MODULE, "ElasticAdvancedSearch", res.HitCount.ToString(), keywords,
                                             ContextManager.CurrentUser.UserId, ContextManager.CurrentUser.UserSecurity.SessionId);

                searchEvent.Save(false);
            }
            catch
            {
                // hmm... we couldn't log the event.  We really should try to log the fact that we couldn't log, but you know...
            }

            return res;
        }

        [WebMethod(true, Description = "Elasticsearch: blank search (returns dimensions/facets; minimal docs)")]
        public MainUI.Shared.Elastic.SearchResultResponse DoBlankSearch()
        {
            EnsureConfigured();

            var elastic = new ElasticServices(elasticUrl, indexName, apiKey);

            string[] dimensionIds = elastic.BlankDimensions();

            ISearchCriteria searchCriteria = new SearchCriteria(dimensionIds, "", SearchType.AnyWords, 10, 10,
                                                       0, "", false, true,
                                                       null);
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            //pass userSubscriptionCodes
            string[] domainItems = null;
            if (searchCriteria.FilterUnsubscribed)
            {
                domainItems = ContextManager.CurrentUser.UserSecurity.Domain
                    .Split(new[] { "~" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Replace(";", ""))
                    .ToArray();
            }
            var res = elastic.SearchAsync(searchCriteria, domainItems).GetAwaiter().GetResult();
             

            // Trim docs; keep only dimensions
            if (res.SearchResults != null) res.SearchResults = res.SearchResults.Take(0).ToList();
            res.SearchTerm = string.Empty;
            res.DisplayOffset = 0;
            res.DisplayResults = 0;
            res.Excerpts = 0;
            res.Unsubscribed = 1;
            res.SearchMode = 2;
            return res;
        }

        [WebMethod(true, Description = "Elasticsearch: search with last stored criteria (session)")]
        public MainUI.Shared.Elastic.SearchResultResponse ElasticSearchWithCurrentCriteria()
        {
            EnsureConfigured();
            var crit = HttpContext.Current.Session[SESSION_CRITERIA] as EsSearchCriteria;
            if (crit == null)
            {
                var empty = new MainUI.Shared.Elastic.SearchResultResponse();
                empty.DimensionId = "-1";
                return empty;
            }
            return ElasticAdvancedSearch(crit.DimensionId, crit.Keywords, crit.SearchMode, crit.MaxHits,
                                         crit.PageSize, crit.PageOffset, crit.ShowExcerpts, crit.FilterUnsubscribed, 0);
        }

        [WebMethod(true, Description = "Elasticsearch: next hit doc (based on last search cache)")]
        public HitDocResult ElasticNextHitDoc(int id, string type)
        {
            var cache = HttpContext.Current.Session[SESSION_DOCIDS] as List<int>;
            var crit = HttpContext.Current.Session[SESSION_CRITERIA] as EsSearchCriteria;
            var notFound = new HitDocResult { Id = -1, Type = string.Empty };

            if (cache == null || crit == null || cache.Count == 0) return notFound;

            int index = cache.IndexOf(id);
            if (index >= 0 && index + 1 < cache.Count)
                return new HitDocResult { Id = cache[index + 1], Type = "Document" };

            // need to fetch next page
            var nextOffset = crit.PageOffset + crit.PageSize;
            if (nextOffset >= int.MaxValue) return notFound;

            var res = ElasticAdvancedSearch(crit.DimensionId, crit.Keywords, crit.SearchMode, crit.MaxHits,
                                            crit.PageSize, nextOffset, crit.ShowExcerpts, crit.FilterUnsubscribed, 0);
            var newCache = HttpContext.Current.Session[SESSION_DOCIDS] as List<int>;
            if (newCache != null && newCache.Count > 0)
                return new HitDocResult { Id = newCache[0], Type = "Document" };

            return notFound;
        }

        [WebMethod(true, Description = "Elasticsearch: previous hit doc (based on last search cache)")]
        public HitDocResult ElasticPrevHitDoc(int id, string type)
        {
            var cache = HttpContext.Current.Session[SESSION_DOCIDS] as List<int>;
            var crit = HttpContext.Current.Session[SESSION_CRITERIA] as EsSearchCriteria;
            var notFound = new HitDocResult { Id = -1, Type = string.Empty };

            if (cache == null || crit == null || cache.Count == 0) return notFound;

            int index = cache.IndexOf(id);
            if (index > 0)
                return new HitDocResult { Id = cache[index - 1], Type = "Document" };

            if (crit.PageOffset == 0) return notFound;

            var prevOffset = Math.Max(crit.PageOffset - crit.PageSize, 0);
            var res = ElasticAdvancedSearch(crit.DimensionId, crit.Keywords, crit.SearchMode, crit.MaxHits,
                                            crit.PageSize, prevOffset, crit.ShowExcerpts, crit.FilterUnsubscribed, 0);
            var newCache = HttpContext.Current.Session[SESSION_DOCIDS] as List<int>;
            if (newCache != null && newCache.Count > 0)
                return new HitDocResult { Id = newCache.Last(), Type = "Document" };

            return notFound;
        }

        [WebMethod(true, Description = "Elasticsearch: set search criteria in session (Endeca-compatible)")]
        public bool SetSearchCriteria(string[] dimensionIds, string keywords, SearchType searchType, int maxHits,
                                      int pageSize, int pageOffset, bool showExcerpts, bool searchUnsubscribed)
        {
            try
            {
                var mode = 3; // ExactPhrase default
                if (searchType == SearchType.AllWords) mode = 1;
                else if (searchType == SearchType.AnyWords) mode = 2;

                var dimId = (dimensionIds != null && dimensionIds.Length > 0) ? string.Join(",", dimensionIds) : null;

                HttpContext.Current.Session[SESSION_CRITERIA] = new EsSearchCriteria
                {
                    DimensionId = dimId,
                    Keywords = keywords,
                    SearchMode = mode,
                    MaxHits = maxHits,
                    PageSize = pageSize,
                    PageOffset = pageOffset,
                    ShowExcerpts = showExcerpts ? 1 : 0,
                    FilterUnsubscribed = searchUnsubscribed ? 0 : 1 // note: legacy param meaning
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Helpers

        private static void EnsureConfigured()
        {
            if (string.IsNullOrWhiteSpace(elasticUrl))
                throw new InvalidOperationException("ElasticSearchEndpoint is not configured.");
        }

        private static void AfterSearchPostProcess(MainUI.Shared.Elastic.SearchResultResponse res, string keywords,
                                                    int searchMode, int showExcerpts, int filterUnsubscribed, int pageOffset)
        {
            if (res == null) return;

            // echo original flags
            res.SearchTerm = string.IsNullOrEmpty(res.SearchTerm) ? (keywords ?? string.Empty) : res.SearchTerm;
            res.SearchMode = searchMode;
            res.Excerpts = showExcerpts;
            res.Unsubscribed = filterUnsubscribed;
            res.DisplayOffset = pageOffset;

            // Word interpretations if missing
            if (string.IsNullOrEmpty(res.WordIntepretations) && !string.IsNullOrWhiteSpace(keywords))
            {
                var parts = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                res.WordIntepretations = string.Join(", ", parts);
            }

            // Enumerate results and compute DisplayResults like Endeca
            if (res.SearchResults != null)
            {
                int x = pageOffset;
                for (int i = 0; i < res.SearchResults.Count; i++)
                {
                    res.SearchResults[i].ResultEnumeration = x++;
                    // If snippet lacks highlight, emulate Endeca bolding on query terms
                    if (!string.IsNullOrEmpty(res.SearchResults[i].Snippet) && res.SearchResults[i].Snippet.IndexOf("endeca_term", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        foreach (var w in (res.WordIntepretations ?? string.Empty).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var token = w.Trim();
                            if (token.Length == 0) continue;
                            res.SearchResults[i].Snippet = System.Text.RegularExpressions.Regex.Replace(
                                res.SearchResults[i].Snippet,
                                "\b" + System.Text.RegularExpressions.Regex.Escape(token) + "\b",
                                "<b class='endeca_term'>" + token + "</b>",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                    }
                }
                res.DisplayResults = pageOffset + res.SearchResults.Count;
            }
            else
            {
                res.DisplayResults = pageOffset;
            }
        }

        private static void CacheLastSearch(MainUI.Shared.Elastic.SearchResultResponse res, EsSearchCriteria crit)
        {
            // Cache criteria
            HttpContext.Current.Session[SESSION_CRITERIA] = crit;

            // Cache numeric doc ids if possible (for Next/Prev)
            var list = new List<int>();
            if (res != null && res.SearchResults != null)
            {
                foreach (var r in res.SearchResults)
                {
                    if (r != null && r.Id > 0)
                        list.Add(r.Id);
                }
            }
            HttpContext.Current.Session[SESSION_DOCIDS] = list;
        }


        private sealed class EsSearchCriteria
        {
            public string DimensionId { get; set; }
            public string Keywords { get; set; }
            public int SearchMode { get; set; }
            public int MaxHits { get; set; }
            public int PageSize { get; set; }
            public int PageOffset { get; set; }
            public int ShowExcerpts { get; set; }
            public int FilterUnsubscribed { get; set; }
        }


        #endregion

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            //In addition to alphanumeric characters The following characters are supported by the index _-?\&*$!@#%()'

            string strOut = Regex.Replace(strIn, @"[^\w\.@',\\$\\*&_\\?!%\\(\\)-]", " ");
            return strOut;
        }
    }

}
