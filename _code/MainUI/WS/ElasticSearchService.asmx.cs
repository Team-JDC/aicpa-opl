using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AICPA.Destroyer.Shared;
using MainUI.Shared;
using Nest;
using Endeca.Data;
namespace MainUI.WS
{
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class ElasticSearchService : WebService
    {
       
        private static readonly string elasticUrl = ConfigurationManager.AppSettings["ElasticSearchEndpoint"];
        private static readonly string indexName = ConfigurationManager.AppSettings["ElasticSearchIndex"];
        private static readonly string apiKey = ConfigurationManager.AppSettings["ElasticSearchApiKey"];
        [WebMethod(Description = "This method returns Elasticsearch-based search results")]
        public MainUI.Shared.Elastic.SearchResultResponse Search(string keywords, int maxHits, int searchMode,int pageSize, int pageOffset, int showExcerpts, int filterUnsubscribed)
        {
            var elastic = new MainUI.Shared.Elastic.ElasticSearchService(elasticUrl, indexName, apiKey);
            return elastic.SearchAsync(keywords, maxHits,  searchMode, pageSize, pageOffset, showExcerpts == 1, filterUnsubscribed == 1).Result;
        }
         
    }
 
}
