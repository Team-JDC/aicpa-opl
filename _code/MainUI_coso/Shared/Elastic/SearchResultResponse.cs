using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared.Elastic
{
    [Serializable]
    public class SearchResultResponse
    {
        public string DimensionId { get; set; }
        public List<DimensionNavigationResult> DimensionResults { get; set; }
        public List<DimensionNavigationResult> SelectedDimensionResults { get; set; }
        public string DimensionXml { get; set; }
        public int DisplayOffset { get; set; }
        public int DisplayResults { get; set; }
        public int Excerpts { get; set; }
        public int HitCount { get; set; }
        public int SearchMode { get; set; }
        public List<SearchResult> SearchResults { get; set; }
        public string SearchTerm { get; set; }
        public int Unsubscribed { get; set; }
        public string WordIntepretations { get; set; }
        public int nonauthoritative { get; set; }
    }
}