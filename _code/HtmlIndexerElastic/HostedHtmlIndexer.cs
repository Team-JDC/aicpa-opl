using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic
{
    public sealed class HostedHtmlIndexer : HtmlIndexerBase
    {
        public HostedHtmlIndexer(string endpoint, string indexName, string username, string password) : base(endpoint, indexName)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
            }
        }
    }
}
