using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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


        public async Task<SearchResultResponse> SearchAsyncOld(ISearchCriteria sc, string[] userSubscriptionCodes = null)
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
                                    
                                    return aa
                                        // (A) child sitefolders
                                        .Filter("child_folders", f => f
                                            .Filter(q => q.Bool(b => b.Must(
                                                m => m.Term(t => t.Field("SiteHierarchy.Type").Value("SiteFolder")),
                                                m => m.Term(t => t.Field("SiteHierarchy.Level").Value(sel.level + 1))
                                            )))
                                        .Aggregations(aaa => aaa.Composite("by_child_folder", c => c
                                            .Size(1000)
                                            .Sources(ss => ss
                                                .Terms("fid", t => t.Field("SiteHierarchy.Id").Order(SortOrder.Ascending))
                                                .Terms("fname", t => t.Field("SiteHierarchy.Title").Order(SortOrder.Ascending))
                                            ))))
                                        // (B) books under this branch (fallback)
                                        .Filter("books_scope", f => f
                                            .Filter(q => q.Term(t => t.Field("SiteHierarchy.Type").Value("Book")))
                                        .Aggregations(aaa => aaa.Composite("by_book", c => c
                                            .Size(1000)
                                            .Sources(ss => ss
                                                .Terms("bid", t => t.Field("SiteHierarchy.Id").Order(SortOrder.Ascending))
                                                .Terms("btitle", t => t.Field("SiteHierarchy.Title").Order(SortOrder.Ascending))
                                            ))));

                  
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
                        // First try CHILD SITEFOLDERS (next level)
                        var childComp = response.Aggregations
                            .Nested("site_h")?
                            .Filter("child_folders")?
                            .Composite("by_child_folder");

                        var childBuckets = childComp?.Buckets?.ToList() ?? new List<CompositeBucket>();

                        if (childBuckets.Count > 0)
                        {
                            // We found deeper sitefolders → facet those
                            axis = "site_folders";
                            var ordered = childBuckets
                                .OrderByDescending(b => b.DocCount ?? 0)
                                .ThenBy(b => b.Key.TryGetValue("fname", out string nm) ? nm?.ToString() : null)
                                .ToList();

                            foreach (var b in ordered)
                            {
                                b.Key.TryGetValue("fid", out object idObj);
                                b.Key.TryGetValue("fname", out object titleObj);
                                var fid = idObj?.ToString();
                                var fname = titleObj?.ToString();
                                if (string.IsNullOrEmpty(fid)) continue;

                                var label = !string.IsNullOrWhiteSpace(fname) ? fname : $"SiteFolder {fid}";
                                var cnt = b.DocCount ?? 0;

                                dimResults.Add(new DimensionNavigationResult
                                {
                                    DimensionId = fid,
                                    DimensionName = label,
                                    DimensionValue = cnt.ToString()
                                });
                                refinementItems.Add(Tuple.Create(fid, label, (long)cnt));
                            }
                        }
                        else
                        {
                            // No more sitefolders under this branch → facet BOOKS
                            axis = "books";
                            var comp = response.Aggregations
                                .Nested("site_h")?
                                .Filter("books_scope")?
                                .Composite("by_book");

                            var ordered = comp?.Buckets?
                                .OrderByDescending(b => b.DocCount ?? 0)
                                .ThenBy(b => b.Key.TryGetValue("btitle", out string nm) ? nm?.ToString() : null)
                                .ToList() ?? new List<CompositeBucket>();

                            foreach (var b in ordered)
                            {
                                b.Key.TryGetValue("bid", out object idObj);
                                b.Key.TryGetValue("btitle", out object titleObj);
                                var bid = idObj?.ToString();
                                var btitle = titleObj?.ToString();
                                if (string.IsNullOrEmpty(bid)) continue;

                                var label = !string.IsNullOrWhiteSpace(btitle) ? btitle : $"Book {bid}";
                                var cnt = b.DocCount ?? 0;

                                dimResults.Add(new DimensionNavigationResult
                                {
                                    DimensionId = bid,
                                    DimensionName = label,
                                    DimensionValue = cnt.ToString()
                                });
                                refinementItems.Add(Tuple.Create(bid, label, (long)cnt));
                            }
                        }

                        break;
                    }

                case SearchHelper.NodeType.Site:
                default:
                    {
                        axis = "site_folders";
                        var comp = response.Aggregations
                            .Nested("site_h")?
                            .Filter("level1_folders")?
                            .Composite("by_site_folder");

                        var ordered = comp?.Buckets?
                            .OrderByDescending(b => b.DocCount ?? 0)
                            .ThenBy(b => b.Key.TryGetValue("fname", out string nm) ? nm?.ToString() : null)
                            .ToList() ?? new List<CompositeBucket>();

                        foreach (var b in ordered)
                        {
                            b.Key.TryGetValue("fid", out object idObj);
                            b.Key.TryGetValue("fname", out object titleObj);
                            var fid = idObj?.ToString();
                            var fname = titleObj?.ToString();
                            if (string.IsNullOrEmpty(fid)) continue;

                            var label = !string.IsNullOrWhiteSpace(fname) ? fname : $"SiteFolder {fid}";
                            var cnt = b.DocCount ?? 0;

                            dimResults.Add(new DimensionNavigationResult
                            {
                                DimensionId = fid,
                                DimensionName = label,
                                DimensionValue = cnt.ToString()
                            });
                            refinementItems.Add(Tuple.Create(fid, label, (long)cnt));
                        }
                        break;
                    }
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
                InSubscription = hit.Source.InSubscription,
                Type = NodeType.Document.ToString()
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
        public async Task<SearchResultResponse> SearchAsync(ISearchCriteria sc, string[] userSubscriptionCodes = null)
        {
            // 0) content query
            QueryContainer contentQuery;
            var keywords = sc.Keywords ?? string.Empty;

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

            // 1) resolve selection (Site / SiteFolder / Book) when a plain numeric dimensionId is supplied
            var incomingDimId = (sc.DimensionIds != null && sc.DimensionIds.Length > 0) ? sc.DimensionIds[0] : null;
            var sel = await SearchHelper.ResolveNodeAsync(_client, incomingDimId); // returns Id, Type, Level, Title

            // 2) build filters
            var filters = new List<QueryContainer>();
            if (sc.FilterUnsubscribed && userSubscriptionCodes != null && userSubscriptionCodes.Length > 0)
                filters.Add(new TermsQuery { Field = "SubscriptionCodes", Terms = userSubscriptionCodes });

            filters.Add(new TermQuery { Field = "DocumentStatus", Value = "success" });

       
            if (!string.IsNullOrEmpty(sel.Id))
            {
                filters.Add(new NestedQuery
                {
                    Path = "SiteHierarchy",
                    Query = new TermQuery { Field = "SiteHierarchy.Id", Value = sel.Id }
                });
            }
             

            Func<AggregationContainerDescriptor<ElasticDocument>, IAggregationContainer> aggs = a =>
            {
                var baseAggs = a.Nested("site_h", n => n
                    .Path(p => p.SiteHierarchy)
                    .Aggregations(aa =>
                    {
                        switch (sel.Type)
                        {
                            case SearchHelper.NodeType.Book:
                                // First-level documents ONLY (immediate children of the selected book)
                                return aa.Filter("first_level_docs", f => f
                                        .Filter(q => q.Bool(b => b.Must(
                                            m => m.Term(t => t.Field("SiteHierarchy.Type.keyword").Value("Document")),
                                            m => m.Term(t => t.Field("SiteHierarchy.Level").Value(sel.level + 1))
                                        )))
                                    .Aggregations(aaa => aaa.Composite("by_doc", c => c
                                        .Size(1000)
                                        .Sources(ss => ss
                                            .Terms("did", t => t.Field("SiteHierarchy.Id.keyword").MissingBucket(false))
                                            .Terms("dtitle", t => t.Field("SiteHierarchy.Title.keyword").MissingBucket(true))
                                        ))));

                            case SearchHelper.NodeType.Site:
                            default:
                                // From site → list books (first level after site)
                                return aa.Filter("books_under_site", f => f
                                        .Filter(q => q.Bool(b => b.Must(
                                            m => m.Term(t => t.Field("SiteHierarchy.Type.keyword").Value("Book")),
                                            m => m.Term(t => t.Field("SiteHierarchy.Level").Value(1)) // Site=0 → Book=1
                                        )))
                                    .Aggregations(aaa => aaa.Composite("by_book", c => c
                                        .Size(1000)
                                        .Sources(ss => ss
                                            .Terms("bid", t => t.Field("SiteHierarchy.Id.keyword").MissingBucket(false))
                                            .Terms("btitle", t => t.Field("SiteHierarchy.Title.keyword").MissingBucket(true))
                                        ))));
                        }
                    })
                );

                // Keep the chain helper so SelectedDimensionResults can be built correctly
                if (!string.IsNullOrWhiteSpace(sel.Id))
                {
                    baseAggs = baseAggs.Nested("sel_h", n => n
                        .Path(p => p.SiteHierarchy)
                        .Aggregations(aa => aa
                            .Filter("sel_id", f => f
                                .Filter(q => q.Term(t => t.Field("SiteHierarchy.Id.keyword").Value(sel.Id)))
                                .Aggregations(aaa => aaa.TopHits("one", th => th
                                    .Size(1)
                                    .Source(s => s.Includes(i => i.Field(d => d.SiteHierarchy))))
                            ))));
                }

                return baseAggs;
            };


            // 4) highlight
            Func<HighlightDescriptor<ElasticDocument>, IHighlight> hi = h => sc.Excerpts
                ? h.Fields(f => f.Field("Content").FragmentSize(160).NumberOfFragments(1)
                                .PreTags("<b class='endeca_term'>").PostTags("</b>"))
                : null;

            // 5) execute search
            var response = await _client.SearchAsync<ElasticDocument>(s => s
                .From(sc.PageOffset)
                .Size(sc.PageSize)
                .TrackTotalHits(true)
                .Query(q => q.Bool(b => b.Must(contentQuery).Filter(filters.ToArray())))
                .Aggregations(aggs)
                .Highlight(hi)
            );

            // 6) build DimensionResults (choose next level intelligently)
            var dimResults = new List<DimensionNavigationResult>();
            //var axis = "books";
            var refinementItems = new List<Tuple<string, string, long>>();
            string axisId =
            sel.Type == SearchHelper.NodeType.Site ? "books" :
            sel.Type == SearchHelper.NodeType.Book ? "documents" :
            "child_documents";
            switch (sel.Type)
            {
                case SearchHelper.NodeType.Site:
                    {
                        // Site → facet immediate Books in the hits
                        axisId = "books";
                        var childBooks = SearchHelper.BuildImmediateChildrenFromHits(response.Hits, sel.Id, "Book");

                        foreach (var (id, title, cnt) in childBooks)
                        {
                            dimResults.Add(new DimensionNavigationResult
                            {
                                DimensionId = id,
                                DimensionName = title,
                                DimensionValue = cnt.ToString()
                            });
                            refinementItems.Add(Tuple.Create(id, title, cnt));
                        }
                        break;
                    }

                case SearchHelper.NodeType.Book:
                    {
                        // Book → facet immediate Documents in the hits (first-level docs only)
                        axisId = "documents";
                        var firstLevelDocs = SearchHelper.BuildImmediateChildrenFromHits(response.Hits, sel.Id, "Document");

                        foreach (var (id, title, cnt) in firstLevelDocs)
                        {
                            dimResults.Add(new DimensionNavigationResult
                            {
                                DimensionId = id,
                                DimensionName = title,
                                DimensionValue = cnt.ToString()
                            });
                            refinementItems.Add(Tuple.Create(id, title, cnt));
                        }
                        break;
                    }

                case SearchHelper.NodeType.Document:
                    {
                        // Document → facet its immediate child Documents (next level only)
                        axisId = "documents"; 

                        // 1) Add the SELECTED (parent) document as the first refinement item
                        //    - label from any hit’s SiteHierarchy where Id == sel.Id
                        //    - count = number of hits that include the selected doc in their chain
                        string parentLabel = null;
                        long parentCount = 0;

                        foreach (var hit in response.Hits)
                        {
                            var chain = hit.Source?.SiteHierarchy;
                            if (chain == null || chain.Count == 0) continue;

                            var parentNode = chain.FirstOrDefault(n => n.Id == sel.Id &&
                                                                       n.Type.Equals("Document", StringComparison.OrdinalIgnoreCase));
                             
                            if (parentLabel == null)
                            {
                                parentLabel = !string.IsNullOrWhiteSpace(parentNode.Title)
                                    ? parentNode.Title
                                    : ("Document " + parentNode.Id);

                                parentCount++;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(sel.Id) && !string.IsNullOrWhiteSpace(parentLabel))
                        {
                            // Insert the parent at the top
                            dimResults.Add(new DimensionNavigationResult
                            {
                                DimensionId = sel.Id,
                                DimensionName = parentLabel,
                                DimensionValue = parentCount.ToString()
                            });
                            refinementItems.Add(Tuple.Create(sel.Id, parentLabel, parentCount));
                        }

                        // 2) Now add the IMMEDIATE CHILD documents (first-level only)
                        var childDocs = SearchHelper.BuildImmediateChildrenFromHits(response.Hits, sel.Id, "Document");
                        foreach (var (id, title, cnt) in childDocs)
                        {
                            dimResults.Add(new DimensionNavigationResult
                            {
                                DimensionId = id,
                                DimensionName = title,
                                DimensionValue = cnt.ToString()
                            });
                            refinementItems.Add(Tuple.Create(id, title, cnt));
                        }

                        break;
                    }

                default:
                    {
                        // No selection → treat like Site (pick site from first hit)
                        var firstSite1 = response.Hits
                            .SelectMany(h => h.Source?.SiteHierarchy ?? new List<SiteHierarchyNode>())
                            .Where(n => n.Type.Equals("Site", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(n => n.Level)
                            .FirstOrDefault();

                        if (firstSite1 != null)
                        {
                            axisId = "books";
                            var childBooks = SearchHelper.BuildImmediateChildrenFromHits(response.Hits, firstSite1.Id, "Book");
                            foreach (var (id, title, cnt) in childBooks)
                            {
                                dimResults.Add(new DimensionNavigationResult
                                {
                                    DimensionId = id,
                                    DimensionName = title,
                                    DimensionValue = cnt.ToString()
                                });
                                refinementItems.Add(Tuple.Create(id, title, cnt));
                            }
                        }
                        else
                        {
                            axisId = "books";
                        }
                        break;
                    }
            }
             

            // 7) SelectedDimensionResults (site → folder → book chain)
            var selected = SearchHelper.BuildSelectedChain(response, sel.Id);

            // 8) DimensionXml
            var firstSite = response.Hits
                .SelectMany(h => (h.Source.SiteHierarchy ?? new List<SiteHierarchyNode>())
                    .Where(n => string.Equals(n.Type, "Site", StringComparison.OrdinalIgnoreCase)))
                .OrderBy(n => n.Level)
                .FirstOrDefault();

            var siteTitle = firstSite?.Title;
            var siteId = firstSite?.Id;

            var folderTitle = sel.Type == SearchHelper.NodeType.SiteFolder ? sel.Title : null;
            var folderId = sel.Type == SearchHelper.NodeType.SiteFolder ? sel.Id : null;

 

            var dimensionXml = SearchHelper.BuildDimensionXmlFromChain(
            selected,                               // Site → Book → Document chain
            axisId,                                 // which axis to show next
            refinementItems.Select(t => (t.Item1, t.Item2, t.Item3))   // (id, name, count)
        );

            string Decode(string s) => string.IsNullOrEmpty(s) ? s : WebUtility.HtmlDecode(s).Replace("&nbsp;", " ");
            // 9) results
            var results = response.Hits.Select((hit, i) => new SearchResult
            {
                Id = hit.Source.Id,
                Name = Decode(hit.Source.Name),
                Title = Decode(hit.Source.Title),
                Snippet = WebUtility.HtmlDecode(sc.Excerpts
                    ? (hit.Highlight != null && hit.Highlight.ContainsKey("Content")
                        ? hit.Highlight["Content"].FirstOrDefault()
                        : "")
                    : ""),
                ReferencePath = Decode(hit.Source.ReferencePath),
                SitePath = Decode(hit.Source.SitePath),
                ResultEnumeration = i + sc.PageOffset,
                InSubscription = hit.Source.InSubscription,
                Type = NodeType.Document.ToString()
            }).ToList();
            // 9) final response
            return new SearchResultResponse
            {
                DimensionId = incomingDimId ?? string.Empty,
                DimensionResults = dimResults,
                SelectedDimensionResults = selected,
                DimensionXml = dimensionXml,
                DisplayOffset = sc.PageOffset,
                DisplayResults = sc.PageOffset + results.Count,
                Excerpts = sc.Excerpts ? 1 : 0,
                HitCount = (int)response.Total,
                SearchMode = (int)sc.SearchType,
                SearchResults = results,
                SearchTerm = keywords,
                Unsubscribed = sc.FilterUnsubscribed ? 1 : 0,
                WordIntepretations = string.Join(", ",
                    (keywords ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)),
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