#region

using AICPA.Destroyer.Content.Document;
using System.Collections.Generic;

#endregion

namespace AICPA.Destroyer.Content.Search
{
    ///<summary>
    ///  Exposes properties and methods for working with the results of a site index search.
    ///</summary>
    public interface ISearchResults
    {
        #region Properties

        /// <summary>
        ///   The search criteria used to generate the search results.
        /// </summary>
        ISearchCriteria SearchCriteria { get; }

        /// <summary>
        ///   The total number of hits resulting from the query.
        /// </summary>
        long TotalHitCount { get; }

        /// <summary>
        ///   The total number of hits on the current page (for the last page of search hits this value could be less than ISearchCriteria.PageSize).
        /// </summary>
        long PageHitCount { get; }

        /// <summary>
        ///   The starting hit for the page
        /// </summary>
        long PageOffset { get; }

        /// <summary>
        ///   The words that were ultimately used to perform the query (used for hit highlighting).
        /// </summary>
        string[] WordInterpretations { get; }

        /// <summary>
        ///   XML describing the dimensions returned for the query
        /// </summary>
        string DimensionsXml { get; }

/*
		/// <summary>
		/// XML describing the search results returned for the query
		/// </summary>
		string SearchResultsXml { get; }
*/

        /// <summary>
        ///   The document objects from the current page of search results
        /// </summary>
        IDocument[] Documents { get; }

        /// <summary>
        /// Hold list of Document ID's in the order they were returned
        /// </summary>
        List<int> DocumentIDCache { get; set; }

        //ENEQueryResults EndecaQueryResults { get; }

        #endregion Properties

        #region Methods

        #endregion Methods
    }
}