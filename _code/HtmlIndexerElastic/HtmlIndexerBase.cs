using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic
{
    /// <summary>
    /// Base indexer with shared logic for ensuring index and indexing docs.
    /// Derived classes only handle authentication differences.
    /// </summary>
    /// <summary>
    /// Base indexer with shared logic for ensuring index and indexing docs.
    /// Derived classes only handle authentication differences.
    /// </summary>
    public abstract class HtmlIndexerBase : IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly string _endpoint;
        protected readonly string _indexName;

        protected HtmlIndexerBase(string endpoint, string indexName)
        {
            _endpoint = endpoint?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(endpoint));
            _indexName = string.IsNullOrWhiteSpace(indexName) ? "html_opl_documents" : indexName.Trim();
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(100) };
        }

        public void Dispose() => _client?.Dispose();

        public async Task EnsureIndexAsync()
        {
            // Check existence
            var head = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{_endpoint}/{_indexName}"));
            if (head.IsSuccessStatusCode)
            {
                Console.WriteLine($"Index '{_indexName}' already exists.");
                return;
            }

            // Create with mapping
            var createBody = new
            {
                mappings = new
                {
                    properties = new
                    {
                        title = new { type = "text" },
                        content = new { type = "text" },
                        subscription_codes = new { type = "keyword" },
                        book_id = new { type = "keyword" },
                        document_id = new { type = "keyword" },
                        url = new { type = "keyword" },
                        indexed_at = new { type = "date" }
                    }
                }
            };

            var resp = await _client.PutAsync(
                $"{_endpoint}/{_indexName}",
                new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json")
            );

            if (!resp.IsSuccessStatusCode)
            {
                var txt = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create index '{_indexName}': {resp.StatusCode} - {txt}");
            }

            Console.WriteLine($"Created index '{_indexName}'.");
        }
        public  ElasticDocument ParseHtml(string filePath)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
            var bodyText = htmlDoc.DocumentNode.SelectSingleNode("//body")?.InnerText ?? "";

            string GetMeta(string name) =>
                metaTags?.FirstOrDefault(m => m.GetAttributeValue("name", "") == name)
                         ?.GetAttributeValue("content", "") ?? "";

            var subs = metaTags?
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_subscription_code")
                .Select(m => m.GetAttributeValue("content", ""))
                .Distinct()
                .ToList() ?? new List<string>();

            var hierarchy = metaTags?
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_site_hierarchy")
                .Select(m =>
                {
                    var parts = m.GetAttributeValue("content", "").Split(':');
                    return new SiteHierarchyNode { Type = parts[0], Id = parts[1] };
                })
                .ToList() ?? new List<SiteHierarchyNode>();

            var doc = new ElasticDocument
            {
                Id = int.Parse(GetMeta("destroyer_document_id")),
                Name = GetMeta("destroyer_document_name"),
                Title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "Untitled",
                Content = bodyText,
                ReferencePath = GenerateReferencePath(hierarchy),
                SubscriptionCodes = subs,
                BookId = GetMeta("destroyer_book_id"),
                BookName = GetMeta("destroyer_book_name"),
                SiteHierarchy = hierarchy,
                InSubscription = true, // adjust as needed
                DimensionXml = BuildDimensionXml(hierarchy, subs),
                SitePath = BuildReferencePathXml(hierarchy)
            };

            return doc;
        }
        public async Task IndexHtmlAsync(string filePath)
        {
            //var html = await File.ReadAllTextAsync(filePath);

            //var doc = new HtmlDocument();
            //doc.LoadHtml(html);

            //var metaTags = doc.DocumentNode.SelectNodes("//meta") ?? new HtmlNodeCollection(null);

            //var subscriptionCodes = metaTags
            //    .Where(m => m.GetAttributeValue("name", "") == "destroyer_subscription_code")
            //    .Select(m => m.GetAttributeValue("content", ""))
            //    .Where(v => !string.IsNullOrWhiteSpace(v))
            //    .Distinct()
            //    .ToList();

            //var bookId = metaTags.FirstOrDefault(m => m.GetAttributeValue("name", "") == "destroyer_book_id")?.GetAttributeValue("content", "");
            //var documentId = metaTags.FirstOrDefault(m => m.GetAttributeValue("name", "") == "destroyer_document_id")?.GetAttributeValue("content", "");
            //var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim() ?? string.Empty;

            //var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText?.Trim() ?? string.Empty;

            //if (string.IsNullOrWhiteSpace(bookId) || string.IsNullOrWhiteSpace(documentId))
            //{
            //    Console.Error.WriteLine($"Skipping {filePath} due to missing bookId or documentId.");
            //    return;
            //}

            //var indexDoc = new
            //{
            //    title,
            //    content = bodyText,
            //    subscription_codes = subscriptionCodes,
            //    book_id = bookId,
            //    document_id = documentId,
            //    url = string.Empty, // we can set repo URL here
            //    indexed_at = DateTime.UtcNow
            //};
            var indexDoc = ParseHtml(filePath);
            var json = JsonConvert.SerializeObject(indexDoc);
            var docId = ($"{indexDoc.Id}_{indexDoc.BookId}")
                .Replace('/', '_')
                .Replace('\\', '_')
                .Replace(' ', '_');

            var response = await _client.PutAsync(
                $"{_endpoint}/{_indexName}/_doc/{docId}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var respText = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Failed to index {filePath}: {response.StatusCode} - {respText}");
            }
            else
            {
                Console.WriteLine($"Indexed: {docId}");
            }
        }

        #region Private Methods

        private static string GenerateReferencePath(List<SiteHierarchyNode> hierarchy)
        {
            return string.Join(" > ", hierarchy.Select(h => $"{h.Type}:{h.Id}"));
        }

        private static string BuildReferencePathXml(List<SiteHierarchyNode> hierarchy)
        {
            var sb = new StringBuilder();
            sb.Append("<ReferencePath>");
            foreach (var node in hierarchy)
            {
                sb.AppendFormat(@"<Site{0} Id=""{1}"" Name=""{0}_{1}"" Title=""{0} {1}"" />", node.Type, node.Id);
            }
            sb.Append("</ReferencePath>");
            return sb.ToString();
        }

        private static string BuildDimensionXml(List<SiteHierarchyNode> hierarchy, List<string> subscriptionCodes)
        {
            var sb = new StringBuilder();
            sb.Append("<Dimensions><SelectedDimensions>");

            var selected = hierarchy.LastOrDefault(h => h.Type == "Site") ?? hierarchy.FirstOrDefault();
            if (selected != null)
            {
                sb.AppendFormat("<Dimension><Name>{0}</Name><Id>{1}</Id><DimensionAncestors /><DimensionCompletePath>", selected.Type, selected.Id);
                foreach (var h in hierarchy)
                {
                    sb.AppendFormat("<Dimension><Name>{0}</Name><Id>{1}</Id></Dimension>", h.Type, h.Id);
                }
                sb.Append("</DimensionCompletePath></Dimension>");
            }

            sb.Append("</SelectedDimensions><RefinementDimensions>");

            foreach (var code in subscriptionCodes.Distinct())
            {
                sb.AppendFormat(@"<Dimension>
                <Name>destroyer_site_hierarchy</Name>
                <Id>{0}</Id>
                <DimensionValue>
                    <Name>{0}</Name>
                    <Id>{0}</Id>
                    <RecordCount>1</RecordCount>
                </DimensionValue>
                <DimensionAncestors />
                <DimensionCompletePath>
                    <Dimension><Name>destroyer_site_hierarchy</Name><Id>{0}</Id></Dimension>
                </DimensionCompletePath>
            </Dimension>", code);
            }

            sb.Append("</RefinementDimensions></Dimensions>");
            return sb.ToString();
        }
        #endregion
    }

}
