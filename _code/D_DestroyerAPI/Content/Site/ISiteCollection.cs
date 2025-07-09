#region

using System.Collections;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Exposes properties and methods for working with collections of ISite objects.
    /// </summary>
    public interface ISiteCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of sites in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Indexer for retrieving a site by ordinal value
        /// </summary>
        ISite this[int index] { get; }

        /// <summary>
        ///   Indexer for retrieving a site by name
        /// </summary>
        ISite this[string name] { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   The order to sort the collection.
        /// </summary>
        SiteSortField SortField { get; set; }

        /// <summary>
        ///   The order to sort the collection.
        /// </summary>
        bool Ascending { get; set; }

        ///<summary>
        ///  Save changes to the ISiteCollection.
        ///</summary>
        void Save();

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <returns></returns>
        ISite GetSiteById(int siteId);

        /// <summary>
        /// </summary>
        /// <param name = "site"></param>
        void Add(ISite site);

        #endregion Methods
    }
}