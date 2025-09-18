using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Elasticsearch.Net;
using Nest;
namespace MainUI.Shared.Elastic
{
    public class ElasticSearchService
    {
        private readonly ElasticClient _client;

        public ElasticSearchService(string elasticUri, string indexName, string apiKey = null)
        {
            var pool = new SingleNodeConnectionPool(new Uri(elasticUri));

            var connectionSettings = new ConnectionSettings(pool)
                .DefaultIndex(indexName)
                .ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(apiKey))
                //.DisableDirectStreaming() //optional 
                .DefaultFieldNameInferrer(p => p); // 👈 prevents camelCase conversion
            _client = new ElasticClient(connectionSettings);
        }

        public async Task<SearchResultResponse> SearchAsync(
                 string keywords,
                 int maxHits,
                 int searchMode,
                 int pageSize,
                 int pageOffset,
                 bool showExcerpts,
                 bool filterUnsubscribed)
        {

            try
            {
                // ---- normalize paging ----
                int size = pageSize > 0 ? pageSize : 10;
                if (maxHits > 0 && size > maxHits) size = maxHits;
                if (pageOffset < 0) pageOffset = 0;

                // ---- query ----
                QueryContainer contentQuery = BuildContentQuery(searchMode, keywords);

                // optional filter
                Func<QueryContainerDescriptor<ElasticDocument>, QueryContainer> filterClause = null;
                if (filterUnsubscribed)
                {
                    filterClause = f => f.Term(t => t.Field("InSubscription").Value(true));
                }

                // optional highlight
                Func<HighlightDescriptor<ElasticDocument>, IHighlight> highlightClause = null;
                if (showExcerpts)
                {
                    highlightClause = h => h.Fields(fd => fd
                        .Field("Content")
                        .PreTags("<b class='endeca_term'>")
                        .PostTags("</b>")
                    );
                }

                // aggregations (faceting)
                Func<AggregationContainerDescriptor<ElasticDocument>, IAggregationContainer> aggs =
                    a => a.Terms("by_subscription_code", t => t
                        .Field("SubscriptionCodes.keyword")
                        .Size(20)
                    );

                // ---- execute ----
                var response = await _client.SearchAsync<ElasticDocument>(s => s
                    .From(pageOffset)
                    .Size(size)
                    .TrackTotalHits(true)
                    .Query(q => q.Bool(b => b.Must(contentQuery).Filter(filterClause)))
                    .Highlight(highlightClause)
                    .Aggregations(aggs)
                );

                // ---- parse dimension xml from first hit ----
                var firstHit = response.Hits.FirstOrDefault();
                var dimParse = ParseDimensionXml(firstHit != null ? firstHit.Source != null ? firstHit.Source.DimensionXml : null : null);

                // ---- project results ----
                var results = new List<SearchResult>();
                int i = 0;
                foreach (var hit in response.Hits)
                {
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

                    results.Add(new SearchResult
                    {
                        Id = hit.Source.Id,
                        Name = hit.Source != null ? hit.Source.Name : null,
                        Title = hit.Source != null ? hit.Source.Title : null,
                        Snippet = snippet,
                        ReferencePath = hit.Source != null ? hit.Source.ReferencePath : null,
                        SitePath = hit.Source != null ? hit.Source.SitePath : null,
                        ResultEnumeration = i + pageOffset,
                        InSubscription = hit.Source != null && hit.Source.InSubscription
                    });
                    i++;
                }

                // ---- build final response into a variable, then return ----
                var result = new SearchResultResponse
                {
                    HitCount = (int)(response.Total),
                    DisplayOffset = pageOffset,
                    DisplayResults = pageOffset + results.Count,
                    Excerpts = showExcerpts ? 1 : 0,
                    Unsubscribed = filterUnsubscribed ? 1 : 0,
                    SearchResults = results,
                    DimensionResults = dimParse.Refinements,
                    SelectedDimensionResults = dimParse.Selected,
                    SearchTerm = keywords,
                    SearchMode = searchMode,
                    DimensionXml = firstHit != null && firstHit.Source != null ? firstHit.Source.DimensionXml : string.Empty,
                    WordIntepretations = string.Join(", ", (keywords ?? string.Empty)
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)),
                    nonauthoritative = 0
                };

                return result;
            }
            catch (Exception)
            {
                // log if needed
                var result = new SearchResultResponse();
                return result;
            }

        }

        private static QueryContainer BuildContentQuery(int mode, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new MatchAllQuery();

            switch (mode)
            {
                case 1: // AllWords → treat as strict phrase (your original behavior)
                case 3: // ExactPhrase
                default:
                    return new MatchPhraseQuery
                    {
                        Field = "Content",
                        Query = query
                    };

                case 2: // AnyWords
                    return new MatchQuery
                    {
                        Field = "Content",
                        Query = query,
                        Operator = Operator.Or
                    };
            }
        }

        private static DimensionParseResult ParseDimensionXml(string xml)
        {
            var result = new DimensionParseResult
            {
                Refinements = new List<DimensionNavigationResult>(),
                Selected = new List<DimensionNavigationResult>()
            };

            if (string.IsNullOrWhiteSpace(xml))
                return result;

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var selectedNodes = doc.SelectNodes("//SelectedDimensions/Dimension");
            if (selectedNodes != null)
            {
                foreach (XmlNode node in selectedNodes)
                {
                    var dim = new DimensionNavigationResult
                    {
                        DimensionId = SafeInnerText(node, "Id"),
                        DimensionName = SafeInnerText(node, "Name"),
                        DimensionValue = string.Empty,
                        DimensionCompletePath = SafeInnerXml(node, "DimensionCompletePath")
                    };
                    result.Selected.Add(dim);
                }
            }

            var refinementNodes = doc.SelectNodes("//RefinementDimensions/Dimension/DimensionValue");
            if (refinementNodes != null)
            {
                foreach (XmlNode node in refinementNodes)
                {
                    var dim = new DimensionNavigationResult
                    {
                        DimensionId = SafeInnerText(node, "Id"),
                        DimensionName = SafeInnerText(node, "Name"),
                        DimensionValue = SafeInnerText(node, "RecordCount"),
                        DimensionCompletePath = SafeInnerXml(node, "DimensionCompletePath")
                    };
                    if (string.IsNullOrEmpty(dim.DimensionValue))
                        dim.DimensionValue = "0";

                    result.Refinements.Add(dim);
                }
            }

            return result;
        }

        private static string SafeInnerText(XmlNode parent, string childName)
        {
            var n = parent.SelectSingleNode(childName);
            return n != null ? n.InnerText : string.Empty;
        }

        private static string SafeInnerXml(XmlNode parent, string childName)
        {
            var n = parent.SelectSingleNode(childName);
            return n != null ? n.InnerXml : string.Empty;
        }

        private sealed class DimensionParseResult
        {
            public List<DimensionNavigationResult> Refinements { get; set; }
            public List<DimensionNavigationResult> Selected { get; set; }
        }
    }
}