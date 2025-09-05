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
            _indexName = string.IsNullOrWhiteSpace(indexName) ? "html_pages" : indexName.Trim();
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

        public async Task IndexHtmlAsync(string filePath)
        {
            var html = await File.ReadAllTextAsync(filePath);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var metaTags = doc.DocumentNode.SelectNodes("//meta") ?? new HtmlNodeCollection(null);

            var subscriptionCodes = metaTags
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_subscription_code")
                .Select(m => m.GetAttributeValue("content", ""))
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct()
                .ToList();

            var bookId = metaTags.FirstOrDefault(m => m.GetAttributeValue("name", "") == "destroyer_book_id")?.GetAttributeValue("content", "");
            var documentId = metaTags.FirstOrDefault(m => m.GetAttributeValue("name", "") == "destroyer_document_id")?.GetAttributeValue("content", "");
            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim() ?? string.Empty;

            var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(bookId) || string.IsNullOrWhiteSpace(documentId))
            {
                Console.Error.WriteLine($"Skipping {filePath} due to missing bookId or documentId.");
                return;
            }

            var indexDoc = new
            {
                title,
                content = bodyText,
                subscription_codes = subscriptionCodes,
                book_id = bookId,
                document_id = documentId,
                url = string.Empty, // we can set repo URL here
                indexed_at = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(indexDoc);
            var docId = ($"{bookId}_{documentId}")
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
    }


}
