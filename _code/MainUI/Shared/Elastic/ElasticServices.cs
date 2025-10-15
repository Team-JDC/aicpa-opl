using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;
using Elasticsearch.Net;
using Nest;
namespace MainUI.Shared.Elastic
{
    public class ElasticServices
    {
        private readonly ElasticClient _client;

        public ElasticServices(string elasticUri, string indexName, string apiKey = null)
        {
            var pool = new SingleNodeConnectionPool(new Uri(elasticUri));

            var connectionSettings = new ConnectionSettings(pool)
                .DefaultIndex(indexName)
                .ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(apiKey))
                //.DisableDirectStreaming() //optional 
                .DefaultFieldNameInferrer(p => p); // 👈 prevents camelCase conversion
            _client = new ElasticClient(connectionSettings);
        }


        public async Task<SearchResultResponse> SearchAsync(ISearchCriteria sc, string[] userSubscriptionCodes = null)
        {
            // 0) content query (keep your existing switch)
            QueryContainer contentQuery;
            var keywords = sc.Keywords ?? "";

            switch (sc.SearchType)
            {
                case AICPA.Destroyer.Shared.SearchType.ExactPhrase:
                    contentQuery = new MatchPhraseQuery { Field = "Content", Query = keywords };
                    break;
                case AICPA.Destroyer.Shared.SearchType.AnyWords:
                    contentQuery = new MatchQuery { Field = "Content", Query = keywords, Operator = Operator.Or };
                    break;
                case AICPA.Destroyer.Shared.SearchType.Boolean:
                    contentQuery = new QueryStringQuery { Fields = new[] { "Content" }, Query = keywords };
                    break;
                default: // AllWords
                    contentQuery = new MatchQuery { Field = "Content", Query = keywords, Operator = Operator.And };
                    break;
            }

            // 1) resolve selection type (Site / SiteFolder / Book) for plain numeric dimensionId
            var sel = await SearchHelper.ResolveNodeAsync(_client, sc.DimensionIds[0]); // _client/_indexName already exist in your service
            var filters = new List<QueryContainer>();

            // subscriptions / status filters you already had
            if (sc.FilterUnsubscribed && userSubscriptionCodes != null && userSubscriptionCodes.Length > 0)
                filters.Add(new TermsQuery { Field = "SubscriptionCodes", Terms = userSubscriptionCodes });
            filters.Add(new TermQuery { Field = "DocumentStatus", Value = "success" });

            // selection filter if we resolved one
            if (!string.IsNullOrEmpty(sel.Id))
            {
                filters.Add(new NestedQuery
                {
                    Path = "SiteHierarchy",
                    Query = new TermQuery { Field = "SiteHierarchy.Id", Value = sel.Id }
                });
            }


            // 2) choose NEXT-LEVEL facet (aggregation) based on selection type
            Func<AggregationContainerDescriptor<ElasticDocument>, IAggregationContainer> aggs = a =>
            {
                var baseAggs = a.Nested("site_h", n => n
                        .Path(p => p.SiteHierarchy)
                        .Aggregations(aa =>
                        {
                            switch (sel.Type)
                            {
                                case SearchHelper.NodeType.SiteFolder:
                                    // facet books under this folder
                                    return aa.Filter("books_scope", f => f
                                            .Filter(q => q.Term("SiteHierarchy.Type", "Book"))
                                        .Aggregations(aaa => aaa.Composite("by_book", c => c
                                            .Size(1000)
                                            .Sources(ss => ss
                                                .Terms("bid", t => t.Field("SiteHierarchy.Id").Order(SortOrder.Ascending))
                                                .Terms("btitle", t => t.Field("SiteHierarchy.Title").Order(SortOrder.Ascending))
                                            ))));
                                //case SearchHelper.NodeType.Book:
                                //    // facet documents (optional)
                                //    return aa.Filter("docs_scope", f => f
                                //            .Filter(q => q.Term("SiteHierarchy.Type", "Document"))
                                //        .Aggregations(aaa => aaa.Composite("by_doc", c => c
                                //            .Size(1000)
                                //            .Sources(ss => ss
                                //                .Terms("did", t => t.Field("SiteHierarchy.Id").Order(SortOrder.Ascending))
                                //                .Terms("dtitle", t => t.Field("SiteHierarchy.Title").Order(SortOrder.Ascending))
                                //            ))));
                                case SearchHelper.NodeType.Site:
                                default:
                                    // root site (or unknown) → facet level-1 site folders
                                    return aa.Filter("level1_folders", f => f
                                            .Filter(q => q.Bool(b => b.Must(
                                                m => m.Term(t => t.Field("SiteHierarchy.Type").Value("SiteFolder")),
                                                m => m.Term(t => t.Field("SiteHierarchy.Level").Value(1))
                                            )))
                                        .Aggregations(aaa => aaa.Composite("by_site_folder", c => c
                                            .Size(1000)
                                            .Sources(ss => ss
                                                .Terms("fid", t => t.Field("SiteHierarchy.Id").Order(SortOrder.Ascending))
                                                .Terms("fname", t => t.Field("SiteHierarchy.Title").Order(SortOrder.Ascending))
                                            ))));
                            }
                        })
                    );

                // extra: only when something is selected, fetch one full hierarchy chain for that id
                if (!string.IsNullOrWhiteSpace(sel.Id))
                {
                    baseAggs = baseAggs.Nested("sel_h", n => n
                        .Path(p => p.SiteHierarchy)
                        .Aggregations(aa => aa
                            .Filter("sel_id", f => f
                                // <-- filter agg expects a QueryContainer
                                .Filter(q => q.Term(t => t
                                    .Field("SiteHierarchy.Id")
                                    .Value(sel.Id)
                                ))
                                // sub-aggregations under the filter
                                .Aggregations(aaa => aaa
                                    .TopHits("one", th => th
                                        .Size(1)
                                        // we need the full SiteHierarchy array back
                                        .Source(s => s.Includes(i => i.Field(d => d.SiteHierarchy)))
                                    )
                                )
                            )
                        )
                    );
                }

                return baseAggs;
            };


            // 3) highlight (as you had)
            Func<HighlightDescriptor<ElasticDocument>, IHighlight> hi = h => sc.Excerpts
                ? h.Fields(f => f.Field("Content").FragmentSize(160).NumberOfFragments(1)
                                .PreTags("<b class='endeca_term'>").PostTags("</b>"))
                : null;

            // 4) execute
            var response = await _client.SearchAsync<ElasticDocument>(s => s
                .From(sc.PageOffset)
                .Size(sc.PageSize)
                .TrackTotalHits(true)
                .Query(q => q.Bool(b => b.Must(contentQuery).Filter(filters.ToArray())))
                .Aggregations(aggs)
                .Highlight(hi)
            );

            // 5) build facets (DimensionResults) from aggregation buckets
            var dimResults = new List<DimensionNavigationResult>();
            var axis = "site_folders";
            var refinementItems = new List<Tuple<string, string, long>>();

            switch (sel.Type)
            {
                case SearchHelper.NodeType.SiteFolder:
                    {
                        axis = "books";
                        var comp = response.Aggregations.Nested("site_h")?
                                    .Filter("books_scope")?
                                    .Composite("by_book");
                        var ordered = comp?.Buckets
                            .OrderBy(b => b.DocCount)  // by count
                            .ThenBy(b => b.Key["btitle"]?.ToString()) // tie-breaker
                            .ToList();
                        if (comp != null)
                        {
                            foreach (var b in ordered)
                            {
                                object idObj, titleObj;
                                b.Key.TryGetValue("bid", out idObj);
                                b.Key.TryGetValue("btitle", out titleObj);
                                var bid = idObj != null ? idObj.ToString() : null;
                                var btitle = titleObj != null ? titleObj.ToString() : null;
                                if (string.IsNullOrEmpty(bid)) continue;

                                var label = !string.IsNullOrWhiteSpace(btitle) ? btitle : ("Book " + bid);
                                var cnt = b.DocCount.HasValue ? b.DocCount.Value : 0L;
                                dimResults.Add(new DimensionNavigationResult
                                {
                                    DimensionId = bid,
                                    DimensionName = label,
                                    DimensionValue = cnt.ToString()
                                });
                                refinementItems.Add(Tuple.Create(bid, label, cnt));
                            }
                        }
                        break;
                    }
                //case SearchHelper.NodeType.Book:
                //    {
                //        axis = "documents";
                //        var comp = response.Aggregations.Nested("site_h")?
                //                    .Filter("docs_scope")?
                //                    .Composite("by_doc");
                //        if (comp != null)
                //        {
                //            foreach (var b in comp.Buckets)
                //            {
                //                object idObj, titleObj;
                //                b.Key.TryGetValue("did", out idObj);
                //                b.Key.TryGetValue("dtitle", out titleObj);
                //                var did = idObj != null ? idObj.ToString() : null;
                //                var dtitle = titleObj != null ? titleObj.ToString() : null;
                //                if (string.IsNullOrEmpty(did)) continue;

                //                var label = !string.IsNullOrWhiteSpace(dtitle) ? dtitle : ("Document " + did);
                //                var cnt = b.DocCount.HasValue ? b.DocCount.Value : 0L;
                //                dimResults.Add(new DimensionNavigationResult
                //                {
                //                    DimensionId = did,
                //                    DimensionName = label,
                //                    DimensionValue = cnt.ToString()
                //                });
                //                refinementItems.Add(Tuple.Create(did, label, cnt));
                //            }
                //        }
                //        break;
                //    }
                case SearchHelper.NodeType.Site:                
                    {
                        axis = "site_folders";
                        var comp = response.Aggregations.Nested("site_h")?
                                    .Filter("level1_folders")?
                                    .Composite("by_site_folder");
                        var ordered = comp?.Buckets
                          .OrderBy(b => b.DocCount)  // by count
                          .ThenBy(b => b.Key["fname"]?.ToString()) // tie-breaker
                          .ToList();
                        if (comp != null)
                        {
                            foreach (var b in comp.Buckets)
                            {
                                object idObj, titleObj;
                                b.Key.TryGetValue("fid", out idObj);
                                b.Key.TryGetValue("fname", out titleObj);
                                var fid = idObj != null ? idObj.ToString() : null;
                                var fname = titleObj != null ? titleObj.ToString() : null;
                                if (string.IsNullOrEmpty(fid)) continue;

                                var label = !string.IsNullOrWhiteSpace(fname) ? fname : ("SiteFolder " + fid);
                                var cnt = b.DocCount.HasValue ? b.DocCount.Value : 0L;
                                dimResults.Add(new DimensionNavigationResult
                                {
                                    DimensionId = fid,
                                    DimensionName = label,
                                    DimensionValue = cnt.ToString()
                                });
                                refinementItems.Add(Tuple.Create(fid, label, cnt));
                            }
                        }
                        break;
                    }
                default:
                    //do nothing
                    break;
            }

            // 6) SelectedDimensionResults
            var selected = SearchHelper.BuildSelectedChain(response, sel.Id);
            //var selected = new List<DimensionNavigationResult>();
            // Always include the root site from the first hit (Endeca does this)
            var firstSite = response.Hits
                .SelectMany(h => (h.Source.SiteHierarchy ?? new List<SiteHierarchyNode>())
                    .Where(n => string.Equals(n.Type, "Site", StringComparison.OrdinalIgnoreCase)))
                .OrderBy(n => n.Level)
                .FirstOrDefault();

      
            // 7) DimensionXml
            var siteTitle = firstSite != null ? firstSite.Title : null;
            var siteId = firstSite != null ? firstSite.Id : null;
            var folderTitle = (sel.Type == SearchHelper.NodeType.SiteFolder) ? sel.Title : null;
            var folderId = (sel.Type == SearchHelper.NodeType.SiteFolder) ? sel.Id : null;

            var dimensionXml = SearchHelper.BuildDimensionXmlFor(
                sel.Type == SearchHelper.NodeType.Site ? SearchHelper.NodeType.Site : sel.Type,
                siteTitle, siteId,
                folderTitle, folderId,
                refinementItems,
                sel.Type == SearchHelper.NodeType.SiteFolder ? "books" :
                sel.Type == SearchHelper.NodeType.Book ? "documents" :
                                                 "site_folders"
            );

            // 8) Results (unchanged except snippet/highlight)
            var results = response.Hits.Select((hit, i) => new SearchResult
            {
                Id = hit.Source.Id,
                Name = hit.Source.Name,
                Title = hit.Source.Title,
                Snippet = sc.Excerpts
                    ? (hit.Highlight != null && hit.Highlight.ContainsKey("Content") ? hit.Highlight["Content"].FirstOrDefault() : "")
                    : "",
                ReferencePath = hit.Source.ReferencePath,
                SitePath = hit.Source.SitePath,
                ResultEnumeration = i + sc.PageOffset,
                InSubscription = hit.Source.InSubscription
            }).ToList();

            return new SearchResultResponse
            {
                DimensionId = sc.DimensionIds[0] ?? "",
                DimensionResults = dimResults,
                SelectedDimensionResults = selected,
                DimensionXml = dimensionXml,
                DisplayOffset = sc.PageOffset,
                DisplayResults = sc.PageOffset + results.Count(),
                Excerpts = sc.Excerpts ? 1 : 0,
                HitCount = (int)response.Total,
                SearchMode = (int)sc.SearchType,
                SearchResults = results,
                SearchTerm = keywords,
                Unsubscribed = sc.FilterUnsubscribed ? 1 : 0,
                WordIntepretations = string.Join(", ", (keywords ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)),
                nonauthoritative = 0
            };
        }
         
        // --- Endeca BlankDimensions() equivalent ---
        public string[] BlankDimensions()
        {
            try
            {
                return BlankDimensionsAsync().GetAwaiter().GetResult();
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<string[]> BlankDimensionsAsync()
        {
            var contentRoot = (ConfigurationManager.AppSettings["ContentRootNode"] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(contentRoot))
                return Array.Empty<string>();

            // We aggregate nested SiteHierarchy where Type == "Site"
            // Then group by Title and (as a sub-agg) by Id, and pick the title that ends with contentRoot
            var response = await _client.SearchAsync<ElasticDocument>(s => s
                .Size(0)
                .Aggregations(a => a
                    .Nested("site_h", n => n
                        .Path(p => p.SiteHierarchy)
                        .Aggregations(aa => aa
                            .Filter("only_sites", f => f
                                .Filter(q => q.Term(t => t.Field("SiteHierarchy.Type").Value("Site")))
                                .Aggregations(aaa => aaa
                                    .Terms("by_title", t => t
                                        .Field("SiteHierarchy.Title")   // keyword in mapping
                                        .Size(200)                      // adjust if you truly have >200 sites
                                        .Aggregations(aaaa => aaaa
                                            .Terms("ids", tt => tt
                                                .Field("SiteHierarchy.Id") // keyword in mapping
                                                .Size(5)
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            if (!response.IsValid) return Array.Empty<string>();

            var ids = new List<string>();
            var titlesAgg = response.Aggregations
                .Nested("site_h")?
                .Filter("only_sites")?
                .Terms("by_title");

            if (titlesAgg != null)
            {
                foreach (var bucket in titlesAgg.Buckets)
                {
                    var title = bucket.Key as string ?? bucket.Key.ToString();
                    if (!string.IsNullOrEmpty(title) &&
                        title.EndsWith(contentRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        var idAgg = bucket.Terms("ids");
                        if (idAgg != null)
                            ids.AddRange(idAgg.Buckets.Select(b => b.Key as string ?? b.Key.ToString()));
                    }
                }
            }

            // Endeca code returned a single string composed by concatenating ids and wrapped in an array.
            // Returning distinct ids as an array is more sensible; if you truly want the exact Endeca quirk:
            //   var concat = string.Concat(ids);
            //   return new[] { concat };
            return ids.Distinct(StringComparer.Ordinal).ToArray();
        }
       
    }
}