using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic
{
    public class ElasticDocument
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ReferencePath { get; set; }
        public string[] SubscriptionCodes { get; set; }
        public string BookId { get; set; }
        public string BookName { get; set; }
        public List<SiteHierarchyNode> SiteHierarchy { get; set; }
        public bool InSubscription { get; set; } = true;
       // public string DimensionXml { get; set; }
        public string SitePath { get; set; } // XML or breadcrumb-style
        public string DocumentStatus { get; set; }
    }
    public class SiteHierarchyNode
    {
     
        public string Type { get; set; }   // "Site" | "SiteFolder" | "Book" | "Document"
        public string Id { get; set; }     // e.g. "344", "28027", ...
        public int Level { get; set; }  // 0=Site, 1=top-level folder, 2=child folder, Book=last+1, Document=last+2
        public string Name { get; set; }   // optional (if you have machine names)
        public string Title { get; set; }  // human label resolved from SQL or HTML
    }
}
