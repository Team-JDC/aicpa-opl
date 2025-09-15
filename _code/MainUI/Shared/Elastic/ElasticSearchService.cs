using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Elasticsearch.Net;
using Nest;
namespace MainUI.Shared.Elastic
{
    public class ElasticSearchService
    {
        private readonly ElasticClient _client;

        public ElasticSearchService(string elasticUri, string indexName, string apiKey = null)
        {
            var settings = new ConnectionSettings(new Uri(elasticUri))
                .DefaultIndex(indexName)
                .ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(apiKey));
                //.DisableDirectStreaming(); //optional 
            _client = new ElasticClient(settings);
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

            // Determine query type based on search mode
            QueryContainer contentQuery;

            switch (searchMode)
            {
                case 1: // AllWords → Treat as MatchPhrase for strict match
                case 3: // ExactPhrase
                default:
                    contentQuery = new MatchPhraseQuery
                    {
                        Field = "Content",
                        Query = keywords
                    };
                    break;

                case 2: // AnyWords
                    contentQuery = new MatchQuery
                    {
                        Field = "Content",
                        Query = keywords,
                        Operator = Operator.Or
                    };
                    break;
            }

            //var json = _client.RequestResponseSerializer.SerializeToString(contentQuery);
           // Console.WriteLine(json);

            // Perform Elasticsearch query
            var response = await _client.SearchAsync<ElasticDocument>(s => s
                .From(pageOffset)
                .Size(pageSize)
                .Query(q => q
                    .Bool(b => b
                        .Must(contentQuery)
                        .Filter(f => filterUnsubscribed
                            ? f.Term(t => t.Field("InSubscription").Value(true))
                            : null)
                    )
                )
                .Highlight(h => showExcerpts
                    ? h.Fields(f => f
                        .Field("Content")
                        .PreTags("<b class='endeca_term'>")
                        .PostTags("</b>")
                    )
                    : null
                )
                .Aggregations(a => a
                    .Terms("by_subscription_code", t => t
                        .Field("SubscriptionCodes.keyword")
                        .Size(20)
                    )
                )
            );

            // Extract facets
            var facets = response.Aggregations.Terms("by_subscription_code")?.Buckets
                .Select(b => new DimensionNavigationResult
                {
                    DimensionId = b.Key,
                    DimensionName = b.Key,
                    DimensionValue = b.DocCount.ToString()
                }).ToList() ?? new List<DimensionNavigationResult>();

            var firstDoc = response.Documents.FirstOrDefault();


            //Console.WriteLine(response.DebugInformation);
            return new SearchResultResponse
            {
                HitCount = (int)response.Total,
                DisplayOffset = pageOffset,
                DisplayResults = pageOffset + response.Documents.Count,
                Excerpts = showExcerpts ? 1 : 0,
                Unsubscribed = filterUnsubscribed ? 1 : 0,
                SearchResults = response.Hits.Select((hit, i) => new SearchResult
                {
                    Id = hit.Source.Id,
                    Name = hit.Source.Name,
                    Title = hit.Source.Title,
                    Snippet = hit.Highlight?.FirstOrDefault().Value?.FirstOrDefault()
                              ?? hit.Source.Content?.Substring(0, Math.Min(hit.Source.Content.Length, 300)) ?? "",
                    ReferencePath = hit.Source.ReferencePath,
                    SitePath = hit.Source.SitePath,
                    ResultEnumeration = i + pageOffset,
                    InSubscription = hit.Source.InSubscription
                }).ToList(),
                DimensionResults = facets,
                SelectedDimensionResults = new List<DimensionNavigationResult>(),
                SearchTerm = keywords,
                SearchMode = searchMode,
                DimensionXml = firstDoc?.DimensionXml ?? "",
                WordIntepretations = string.Join(", ", keywords.Split(' ')),
                nonauthoritative = 0
            };
        }
    }
}