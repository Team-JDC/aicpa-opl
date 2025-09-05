using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic
{
    public sealed class ServerlessHtmlIndexer : HtmlIndexerBase
    {
        public ServerlessHtmlIndexer(string endpoint, string indexName, string apiKey) : base(endpoint, indexName)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("ApiKey is required for serverless mode.", nameof(apiKey));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", apiKey);
        }
    }
}
