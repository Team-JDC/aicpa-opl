#region

using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Exposes properties and methods for indexing and searching documents on a site.
    /// </summary>
    public interface ISiteIndex
    {
        #region Properties

        ///<summary>
        ///  The ISite object that is indexed by this ISiteIndex.
        ///</summary>
        ISite Site { get; }

        /// <summary>
        ///   Gets or sets the online status of the site index.
        /// </summary>
        bool Online { get; set; }

        /// <summary>
        ///   Gets the status of the site index
        /// </summary>
        SiteIndexStatus Status { get; }

        /// <summary>
        ///   Gets a list of site indexing error strings
        /// </summary>
        string[] BuildErrors { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Initiates indexing of the site.
        /// </summary>
        void Build();

        /// <summary>
        ///   Performs a search on the site using the specified search criteria and returns the search results.
        /// </summary>
        /// <param name = "searchCriteria">ISearchCriteria object that specifies the search criteria.</param>
        /// <returns></returns>
        ISearchResults Search(ISearchCriteria searchCriteria);

        #endregion Methods
    }
}