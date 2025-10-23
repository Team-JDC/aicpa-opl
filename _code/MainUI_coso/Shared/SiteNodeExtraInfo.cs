using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared
{
    public class SiteNodeExtraInfo : SubscriptionSiteNode
    {
        public SiteNodeExtraInfo()
        {}

        public SiteNodeExtraInfo(SiteNode siteNode,bool restricted, string targetDoc, string TargetPtr)
        {
            this.Id = siteNode.Id;
            this.Name = siteNode.Name;
            this.Title = siteNode.Title;
            this.Type = siteNode.Type;
            this.Anchor = siteNode.Anchor;
            this.hasPrevious = siteNode.hasPrevious;
            this.hasNext = siteNode.hasNext;
            this.Restricted = restricted;
            this.TargetDoc = targetDoc;
            this.TargetPtr = TargetPtr;
            NextDocument = null;
            PreviousDocument = null;
        }

        public SiteNodeExtraInfo(AICPA.Destroyer.Content.Document.IDocument doc, 
            bool restricted,
            string targetDoc,
            string targetPtr,
            SiteNodeExtraInfo PreviousDocument,
            SiteNodeExtraInfo NextDocument)
            : base(doc,restricted)
        {
            this.Restricted = restricted;
            this.PreviousDocument = PreviousDocument;
            this.NextDocument = NextDocument;
            this.TargetDoc = targetDoc;
            this.TargetPtr = targetPtr;
            this.Formats = new List<FormatOption>();
        }

        public string TargetDoc { get; set; }
        public string TargetPtr { get; set; }

        public SiteNode NextDocument { get; set; }
        public SiteNode PreviousDocument { get; set; }

        public List<FormatOption> Formats { get; set; }
    }
}