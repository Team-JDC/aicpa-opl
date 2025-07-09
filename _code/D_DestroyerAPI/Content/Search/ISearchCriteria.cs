#region

using System;
using System.Collections.Specialized;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Search
{
    /// <summary>
    ///   Exposes properties and methods used for specifying the type of search to be performed on a site index.
    /// </summary>
    public interface ISearchCriteria : IEquatable<ISearchCriteria>
    {
        #region Properties

        /// <summary>
        ///   The search text.
        /// </summary>
        string Keywords { get; set; }

        /// <summary>
        ///   The prefered type of search.
        /// </summary>
        SearchType SearchType { get; set; }

        /// <summary>
        ///   The maximum number of hits to return. Send 0 to request the maximum number of hits supported by the search engine.
        /// </summary>
        int MaxHits { get; set; }

        /// <summary>
        ///   The desired number of hits per page.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        ///   The desired sort order.
        /// </summary>
        string SortOrder { get; set; }

        /// <summary>
        ///   The offset at which to start the hit page, relative to the first overal search hit. A value of zero (0) will start the hit page at the first overall hit.
        /// </summary>
        int PageOffset { get; set; }

        /// <summary>
        ///   The list of dimension ids to use for narrowing the search results
        /// </summary>
        string[] DimensionIds { get; set; }

        /// <summary>
        ///   Determines whether or not document excerpts are returned
        /// </summary>
        bool Excerpts { get; set; }

        /// <summary>
        ///   Determines whether or not the search results should include unsubscribed information.
        /// </summary>
        bool FilterUnsubscribed { get; set; }

        /// <summary>
        ///   A list of name/value pairs used for passing custom search parameters to the search engine.
        /// </summary>
        NameValueCollection SearchParameters { get; set; }

        /// <summary>
        /// Gets the user id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        Guid UserId { get; }

        /// <summary>
        /// Gets the name.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        string Name { get; }

        /// <summary>
        /// Gets the last modified.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        DateTime LastModified { get; }

        /// <summary>
        /// Gets the save id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        int SaveId { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// This version of Save is only for first-time save to the database
        /// </summary>
        void Save(string name, Guid userId);

        /// <summary>
        /// This version of Save is only for renaming an already persisted search; it also saves any other pending changes to the Search Criteria
        /// </summary>
        void Save(string name);

        /// <summary>
        /// This version of Save is only for saving pending changes to already persisted searches
        /// </summary>
        void Save();

        /// <summary>
        ///   Determines if the ISearchCriteria object is ready to be passed into a search.
        /// </summary>
        /// <returns>A true value if the ISearchCriteria is valid, false if ISearchCriteria is not valid</returns>
        bool Validate();

        #endregion Methods
    }
}