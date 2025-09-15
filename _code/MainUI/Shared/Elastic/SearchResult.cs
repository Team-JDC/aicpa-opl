using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared.Elastic
{
    public class SearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public string ReferencePath { get; set; }
        public string SitePath { get; set; }   
        public int ResultEnumeration { get; set; }
        public bool InSubscription { get; set; }
    }
}