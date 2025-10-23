using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared
{
    public class BreadcrumbNode
    {
        public SiteNode SiteNode { get; set; }
        public List<BreadcrumbNode> Children { get; set; }
        public bool HasChilren { get; set; }
        public bool Expanded { get; set; }
    }
}