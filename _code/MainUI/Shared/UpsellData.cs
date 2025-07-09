using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared
{
    public class UpsellData
    {
        public SiteNode Document { get; set; }
        public SiteNode Book { get; set; }

        public bool ShowStoreLink { get; set; }
        public bool ShowPhoneContact { get; set; }

        // public string StoreUrl { get; set; }
        public string MissingLibraryName { get; set; }
        public string StorePurchaseDomain { get; set; }

        public string UserGuid { get; set; }
    }
}