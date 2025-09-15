using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared.Elastic
{
    public class ElasticDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ReferencePath { get; set; }
        public List<string> SubscriptionCodes { get; set; }
        public string BookId { get; set; }
        public string BookName { get; set; }
        public List<SiteHierarchyNode> SiteHierarchy { get; set; }
        public bool InSubscription { get; set; } = true;
        public string DimensionXml { get; set; }
        public string SitePath { get; set; } // XML or breadcrumb-style
    }
    public class SiteHierarchyNode
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }
}