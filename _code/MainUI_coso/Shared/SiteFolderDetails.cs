using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AICPA.Destroyer.Content.Site;

namespace MainUI.Shared
{
    public class SiteFolderDetails : SiteNode
    {
        public SiteFolderDetails()
            : base()
        {
        }

        public SiteFolderDetails(ISiteFolder siteFolder)
            : base(siteFolder)
        {
        }

        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<BreadcrumbNode> Children { get; set; }
    }
}