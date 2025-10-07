using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            // 1) query clause (SearchType)
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
                    // trust Endeca behavior: query_string supports AND/OR/NOT, quotes, etc.
                    contentQuery = new QueryStringQuery { Fields = new[] { "Content" }, Query = keywords };
                    break;
                default: // AllWords
                    contentQuery = new MatchQuery { Field = "Content", Query = keywords, Operator = Operator.And };
                    break;
            }

            // 2) filters (dimensions, subscription, status)
            var filters = new List<QueryContainer>();

            // Selected "dimensions": if caller gives Site/Folder ids, add filters here.
            // Example: if sc.DimensionIds contains siteId(s), filter SiteHierarchy nested elements matching Level=0 and that Id.
            //foreach (var dimId in sc.DimensionIds ?? Array.Empty<string>())
            //{
            //    // Example: treat numbers >= 30000 as SiteFolder ids and < 30000 as Site ids (replace with your own rule)
            //    filters.Add(new NestedQuery
            //    {
            //        Path = "SiteHierarchy",
            //        Query = new BoolQuery
            //        {
            //            Must = new QueryContainer[]
            //            {
            //        new TermQuery { Field = "SiteHierarchy.Type",  Value =  "SiteFolder" },
            //        new TermQuery { Field = "SiteHierarchy.Id",    Value = dimId }
            //            }
            //        }
            //    });
            //}
            

            foreach (var token in sc.DimensionIds ?? Array.Empty<string>())
            {
                // Supported tokens:
                //   "SiteFolder:28070" / "Site:345" / "Book:7263" / "Document:2151146"
                //   "28070"                  (Id only)
                //   "Audit & Accounting Literature" (Name only)
                //   "28070|Audit & Accounting Literature" (Id|Name)
                string type = null, id = null, name = null;

                var t = token?.Trim() ?? string.Empty;

                if (t.Contains("|"))
                {
                    var parts = t.Split(new[] { '|' }, 2);
                    id = parts[0].Trim();
                    name = parts[1].Trim();
                }
                else if (t.Contains(":"))
                {
                    var parts = t.Split(new[] { ':' }, 2);
                    type = parts[0].Trim();   // "Site", "SiteFolder", "Book", "Document"
                    id = parts[1].Trim();
                }
                else if (IsDigits(t))
                {
                    id = t;                   // numeric id, don't assume the type
                }
                else
                {
                    name = t;                 // name-only
                }

                var must = new List<QueryContainer>();

                if (!string.IsNullOrEmpty(id))
                    must.Add(new TermQuery { Field = "SiteHierarchy.Id", Value = id });

                if (!string.IsNullOrEmpty(name))
                    must.Add(new TermQuery { Field = "SiteHierarchy.Name", Value = name });

                // Only add Type if it was provided; DO NOT infer it
                if (!string.IsNullOrEmpty(type))
                    must.Add(new TermQuery { Field = "SiteHierarchy.Type", Value = type });

                // Optionally enforce only top-level folders:
                // if (string.Equals(type, "SiteFolder", StringComparison.OrdinalIgnoreCase))
                //     must.Add(new TermQuery { Field = "SiteHierarchy.Level", Value = 1 });

                filters.Add(new NestedQuery
                {
                    Path = "SiteHierarchy",
                    Query = new BoolQuery { Must = must }
                });
            }

            if (sc.FilterUnsubscribed && userSubscriptionCodes != null && userSubscriptionCodes.Length > 0)
                filters.Add(new TermsQuery { Field = "SubscriptionCodes", Terms = userSubscriptionCodes });

            // Endeca filtered out unsuccessful docs
            filters.Add(new TermQuery { Field = "DocumentStatus", Value = "success" });

            // 3) aggregations (top-level SiteFolders, with friendly Name)
            Func<AggregationContainerDescriptor<ElasticDocument>, IAggregationContainer> aggs = a => a
                .Nested("site_h", n => n
                    .Path(p => p.SiteHierarchy)
                    .Aggregations(aa => aa
                        .Filter("top_level_sitefolders", f => f
                            .Filter(q => q.Bool(b => b.Must(
                                m => m.Term(t => t.Field("SiteHierarchy.Type").Value("SiteFolder")),
                                m => m.Term(t => t.Field("SiteHierarchy.Level").Value(1))
                            )))
                            .Aggregations(aaa => aaa
                                .Composite("by_site_folder", c => c
                                    .Size(1000) // adjust if needed
                                    .Sources(ss => ss
                                        .Terms("fid", t => t.Field("SiteHierarchy.Id"))
                                        .Terms("fname", t => t.Field("SiteHierarchy.Name")) // <-- friendly
                                                                                            // If Name is TEXT with a keyword subfield, use:
                                                                                            // .Terms("fname", t => t.Field("SiteHierarchy.Name.keyword"))
                                    )
                                )
                            )
                        )
                    )
                );
            // 4) highlight & suggest (snippets, dym)
            Func<HighlightDescriptor<ElasticDocument>, IHighlight> hi = h => sc.Excerpts
                ? h.Fields(f => f.Field("Content").FragmentSize(160).NumberOfFragments(1)
                                .PreTags("<b class='endeca_term'>").PostTags("</b>"))
                : null;


            var shoulds = new List<QueryContainer>();

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                // Title exact phrase >>> strongest
                shoulds.Add(new MatchPhraseQuery { Field = "Title", Query = keywords, Boost = 5 });

                // Content exact phrase >>
                shoulds.Add(new MatchPhraseQuery { Field = "Content", Query = keywords, Boost = 2 });

                // Best-field query across Title/Content/Name >
                shoulds.Add(new MultiMatchQuery
                {
                    Query = keywords,
                    Type = TextQueryType.BestFields,
                    Fields = new[] { "Title^3", "Content^1", "Name^1.5" },
                    Operator = (sc.SearchType == AICPA.Destroyer.Shared.SearchType.AnyWords) ? Operator.Or : Operator.And,
                    TieBreaker = 0.2
                });

                // Small fuzzy net for AnyWords to catch typos
                if (sc.SearchType == AICPA.Destroyer.Shared.SearchType.AnyWords)
                    shoulds.Add(new MatchQuery { Field = "Content", Query = keywords, Fuzziness = Fuzziness.Auto, Boost = 0.3 });
            }

            var finalQuery = new BoolQuery
            {
                    Must = new List<QueryContainer> { contentQuery }, // <-- wrap single item
                    Filter = (filters != null && filters.Count > 0) ? filters : null,
                    Should = (shoulds != null && shoulds.Count > 0) ? shoulds : null,
                    MinimumShouldMatch = (shoulds != null && shoulds.Count > 0)
               ? new MinimumShouldMatch(1)   // require at least one 'should' to match
               : null
            };


            // 5) execute


            //var response = await _client.SearchAsync<ElasticDocument>(s => s
            //    .From(sc.PageOffset)
            //    .Size(sc.PageSize)
            //    .TrackTotalHits(true)
            //    .Query(q => finalQuery)
            //    .Aggregations(aggs)
            //    .Highlight(hi)
            //    .Rescore(r => r.Rescore(rr => new Rescore
            //    {
            //        WindowSize = 200,
            //        Query = new RescoreQuery
            //        {
            //            // <<< THIS is the correct property name
            //            Query = new MatchPhraseQuery
            //            {
            //                Field = new Field("Title"), // or Infer.Field<ElasticDocument>(d => d.Title)
            //                Query = keywords,
            //                Slop = 0
            //            },
            //            QueryWeight = 0.7,
            //            RescoreQueryWeight = 3.0,
            //            ScoreMode = ScoreMode.Total
            //        }
            //    }))
            //);
            var response = await _client.SearchAsync<ElasticDocument>(s =>
            {
                s = s.From(sc.PageOffset)
                     .Size(sc.PageSize)
                     .TrackTotalHits(true)
                     .Query(q => finalQuery)
                     .Highlight(hi)
                     .Aggregations(aggs);

                if (!string.IsNullOrWhiteSpace(keywords))
                {
                    s = s.Rescore(r => r.Rescore(rr => new Rescore
                    {
                        WindowSize = 200,
                        Query = new RescoreQuery
                        {
                            // <<< THIS is the correct property name
                            Query = new MatchPhraseQuery
                            {
                                Field = new Field("Title"), // or Infer.Field<ElasticDocument>(d => d.Title)
                                Query = keywords,
                                Slop = 0
                            },
                            QueryWeight = 0.7,
                            RescoreQueryWeight = 3.0,
                            ScoreMode = ScoreMode.Total
                        }
                    }));
                }
                return s;
            });


            // 6) selected “site/library” (for SelectedDimensionResults)
            // choose: provided in sc.DimensionIds or auto-pick the site from the first hit
            string selectedSiteId = null, selectedSiteTitle = null;
            var firstHitSite = response.Hits
                .SelectMany(h => (h.Source.SiteHierarchy ?? new List<SiteHierarchyNode>()).Where(x => x.Type == "Site"))
                .FirstOrDefault();
            if (firstHitSite != null)
            {
                selectedSiteId = firstHitSite.Id;
                selectedSiteTitle = !string.IsNullOrWhiteSpace(firstHitSite.Title) ? firstHitSite.Title : ("Site " + selectedSiteId);
            }

            // 7) build refinement buckets (folderId → folderName + count)
            var folderBuckets = new List<(string FolderId, string FolderName, long Count)>();

            var comp = response.Aggregations
                .Nested("site_h")?
                .Filter("top_level_sitefolders")?
                .Composite("by_site_folder");

            if (comp != null)
            {
                foreach (var b in comp.Buckets)
                {
                    var fid = b.Key.TryGetValue("fid", out string fidObj) ? fidObj?.ToString() : null;
                    var fname = b.Key.TryGetValue("fname", out string nameObj) ? nameObj?.ToString() : null;

                    if (string.IsNullOrEmpty(fid)) continue;
                    var label = !string.IsNullOrWhiteSpace(fname) ? fname : $"SiteFolder {fid}";

                    folderBuckets.Add((fid, label, b.DocCount ?? 0));
                }
            }


            // 8) Endeca-like SelectedDimensionResults (note Endeca swaps fields)
            var selectedSite = new DimensionNavigationResult
            {
                DimensionId = null,
                DimensionName = selectedSiteId ?? "",
                DimensionValue = selectedSiteTitle ?? "",
                DimensionCompletePath = null
            };

            // 9) DimensionResults (refinements list)
            var dimResults = folderBuckets
                   .OrderBy(x => x.Count)
                   .Select(x => new DimensionNavigationResult
                   {
                       DimensionId = x.FolderId,        // filter key
                       DimensionName = x.FolderName,      // <-- friendly label (e.g., "Audit & Accounting Literature")
                       DimensionValue = x.Count.ToString()
                   })
                   .ToList();


            // 10) Endeca-like DimensionXml (with &lt; &gt; encoded)
            string DimensionXmlEncoded()
            {
                string XE(string s) => System.Security.SecurityElement.Escape(s ?? "");

                var sb = new StringBuilder();
                sb.Append("<Dimensions>");
                sb.Append("<SelectedDimensions><Dimension>");
                sb.Append("<Name>").Append(XE(selectedSiteTitle)).Append("</Name><Id>")
                  .Append(XE(selectedSiteId)).Append("</Id>");
                sb.Append("<DimensionAncestors />");
                sb.Append("<DimensionCompletePath>");
                sb.Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>site_folders</Id></Dimension>");
                sb.Append("<Dimension><Name>").Append(XE(selectedSiteTitle)).Append("</Name><Id>")
                  .Append(XE(selectedSiteId)).Append("</Id></Dimension>");
                sb.Append("</DimensionCompletePath></Dimension></SelectedDimensions>");

                sb.Append("<RefinementDimensions><Dimension>");
                sb.Append("<Name>destroyer_site_hierarchy</Name><Id>site_folders</Id>");
                foreach (var (fid, fname, cnt) in folderBuckets.Where(b => b.Count > 0))
                {
                    sb.Append("<DimensionValue><Name>").Append(XE(fname)).Append("</Name><Id>")
                      .Append(XE(fid)).Append("</Id><RecordCount>").Append(cnt)
                      .Append("</RecordCount></DimensionValue>");
                }
                sb.Append("<DimensionAncestors />");
                sb.Append("<DimensionCompletePath>");
                sb.Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>site_folders</Id></Dimension>");
                sb.Append("<Dimension><Name>").Append(XE(selectedSiteTitle)).Append("</Name><Id>")
                  .Append(XE(selectedSiteId)).Append("</Id></Dimension>");
                sb.Append("</DimensionCompletePath></Dimension></RefinementDimensions>");
                sb.Append("</Dimensions>");

                return sb.ToString();
            }


            // 11) map hits (use your already-enriched paths; fallback to rebuild)
            var results = new List<SearchResult>();
            int ix = 0;
            foreach (var hit in response.Hits)
            {
                var src = hit.Source;
              
                string snippet = string.Empty;
                if (hit.Highlight != null)
                {
                    var kv = hit.Highlight.FirstOrDefault();
                    if (kv.Value != null)
                    {
                        snippet = kv.Value.FirstOrDefault();
                    }
                }
                if (string.IsNullOrEmpty(snippet) && hit.Source != null && !string.IsNullOrEmpty(hit.Source.Content))
                {
                    snippet = hit.Source.Content.Substring(0, Math.Min(hit.Source.Content.Length, 300));
                }

                // Prefer the friendly paths you stored at index time
                var sitePathEncoded = (src.SitePath ?? "")
                                      .Replace("&", "\u0026").Replace("<", "\u003c").Replace("/>", "\u003e\u003c");
                var referencePathFriendly = string.IsNullOrWhiteSpace(src.ReferencePath)
                                            ? (src.Title ?? src.Name ?? "")
                                            : src.ReferencePath;

                results.Add(new SearchResult
                {
                    Id = src.Id,
                    Name = src.Name,
                    Title = string.IsNullOrWhiteSpace(src.Title) ? src.Name : src.Title,
                    Type = Enum.GetName(typeof(NodeType), NodeType.Document),
                    Snippet = snippet,
                    ReferencePath = referencePathFriendly,
                    SitePath = sitePathEncoded,
                    ResultEnumeration = sc.PageOffset + (ix++),
                    InSubscription = src.InSubscription
                });
            }

            // 12) final response (Endeca-compatible shape)
            return new SearchResultResponse
            {
                HitCount = (int)response.Total,
                DisplayOffset = sc.PageOffset,
                DisplayResults = sc.PageOffset + results.Count,
                Excerpts = sc.Excerpts ? 1 : 0,
                Unsubscribed = sc.FilterUnsubscribed ? 1 : 0,
                SearchResults = results,
                DimensionResults = dimResults,
                SelectedDimensionResults = string.IsNullOrWhiteSpace(selectedSiteId)
                    ? new List<DimensionNavigationResult>()
                    : new List<DimensionNavigationResult> { selectedSite },
                SearchTerm = sc.Keywords,
                SearchMode = (int)sc.SearchType, // or map to 1/2/3
                DimensionXml = DimensionXmlEncoded(),
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
        static bool IsDigits(string s) => !string.IsNullOrWhiteSpace(s) && s.All(char.IsDigit);
    }
}