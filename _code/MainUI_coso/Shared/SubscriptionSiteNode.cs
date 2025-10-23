using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AICPA.Destroyer.Content.Document;

namespace MainUI.Shared
{
    public class SubscriptionSiteNode : SiteNode
    {
        public SubscriptionSiteNode()
        {}

        public SubscriptionSiteNode(IDocument doc, bool restricted)
            : base(doc)
        {
            Restricted = restricted;
        }

        public bool Restricted { get; set; }
    }
}