using HtmlAgilityPack;
using HtmlIndexerElastic.Util;
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
         IndexerHelper _indexerHelper =new IndexerHelper();
        protected HtmlIndexerBase(string endpoint, string indexName)
        {
            _endpoint = endpoint?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(endpoint));
            _indexName = string.IsNullOrWhiteSpace(indexName) ? "html_opl_documents" : indexName.Trim();
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(100) };
        }

        public void Dispose() => _client?.Dispose();

        
        public async Task IndexHtmlAsync(string filePath)
        { 
            var indexDoc = _indexerHelper.ParseHtml(filePath);
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
        
    }

}
