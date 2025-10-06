using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using HtmlAgilityPack;
using HtmlIndexerElastic.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
        IndexerHelper _indexerHelper = new IndexerHelper();
        protected HtmlIndexerBase(string endpoint, string indexName)
        {
            _endpoint = endpoint?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(endpoint));
            _indexName = string.IsNullOrWhiteSpace(indexName) ? "html_opl_documents" : indexName.Trim();
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(100) };

        }

        public void Dispose() => _client?.Dispose();


        public async Task IndexHtmlAsync(string filePath)
        {
            var indexDoc = await _indexerHelper.BuildDocumentAsync(filePath,"");//_indexerHelper.ParseHtml(filePath);
            var json = JsonConvert.SerializeObject(indexDoc);
            //var docId = ($"{indexDoc.BookId}_{indexDoc.Id}")
            //    .Replace('/', '_')
            //    .Replace('\\', '_')
            //    .Replace(' ', '_');
            var docId = $"{indexDoc.BookId}_{indexDoc.Id}";
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



        public async Task DeleteFromElasticAsync(string docId)
        {
            var response = await _client.DeleteAsync($"{_endpoint}/{_indexName}/_doc/{docId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"❌ DELETE failed for {docId}: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine($"🗑 Deleted document {docId} from index.");
            }
        }
        public async Task DeleteAllFromElasticAsync()
        {
            var payload = @"{
                ""query"": {
                    ""match_all"": {}
                }
            }";

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/{_indexName}/_delete_by_query")
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"❌ Failed to delete all documents: {response.StatusCode}\n{content}");
            }
            else
            {
                Console.WriteLine($"🗑️ All documents deleted from index '{_indexName}'.");
            }
        }
        public async Task<bool> IndexHasDocumentsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}/{_indexName}/_count");

            using var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"❌ Failed to check document count: {response.StatusCode}\n{error}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var countObj = JsonDocument.Parse(json);
            var count = countObj.RootElement.GetProperty("count").GetInt32();

            Console.WriteLine($"📊 Index '{_indexName}' contains {count} document(s).");
            return count > 0;
        }
        public async Task BulkIndexHtmlAsync(IEnumerable<string> filePaths,string connectionString, int batchSize=500 )
        {
            var batch = new List<string>();
            int count = 0;


            foreach (var path in filePaths)
            {
                var doc = await _indexerHelper.BuildDocumentAsync(path, connectionString); 
                string docId = MakeDocId(doc);
                string meta = $"{{ \"index\": {{ \"_index\": \"{_indexName}\", \"_id\": \"{docId}\" }} }}";
                string json = System.Text.Json.JsonSerializer.Serialize(doc);
                //Console.WriteLine($"meta: {meta}\n");
                 
                batch.Add(meta);
                batch.Add(json);
                count++;


                if (count % batchSize == 0)
                {
                    await SendBulkAsync(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await SendBulkAsync(batch);
            }
        }

        private async Task SendBulkAsync(List<string> batch)
        {
            string ndjson = string.Join("\n", batch) + "\n";
            var content = new StringContent(ndjson, Encoding.UTF8, "application/x-ndjson");
            var response = await _client.PostAsync($"{_endpoint}/_bulk?refresh=false", content);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                Console.WriteLine($"✅ Bulk index success: {batch.Count / 2} docs.");
            else
                Console.WriteLine($"❌ Bulk index error: {result}");
        }
 
          

        private static string MakeDocId(ElasticDocument d)
        {
            // canonical: BookId + "_" + DocumentId (Id)
            var book = (d.BookId.Trim() ?? "").Trim();
            var doc = d.Id.ToString(System.Globalization.CultureInfo.InvariantCulture).Trim();
            var raw = $"{doc}_{book}";

            // normalize
            return raw
                .Trim()
                .ToLowerInvariant()
                .Replace('/', '_')
                .Replace('\\', '_')
                .Replace(':', '_')
                .Replace(' ', '_');
        }

       
    }

}
