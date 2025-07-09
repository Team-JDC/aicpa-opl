using System;
using System.Collections.Generic;

namespace AICPA.Destroyer.Content.Search
{
    /// <summary>
    ///   Summary description for ISearchDALC.
    /// </summary>
    public interface ISearchDalc
    {
        ///<summary>
        ///  Get a single search based on its searchId.
        ///</summary>
        ///<param name = "searchId">The Id of the search to be returned.</param>
        ///<returns>A strongly-typed datatable containing search rows.</returns>
        SearchDs.SearchRow GetSearch(int searchId);

        ///<summary>
        ///  Get one or more searches based on the userId.
        ///</summary>
        ///<param name = "UserId">The userId of the user whose searches are to be returned.</param>
        ///<returns>A strongly-typed datatable containing search rows.</returns>
        IEnumerable<SearchDs.SearchRow> GetSearches(Guid UserId);

        ///<summary>
        ///  Insert, Update, or Delete a search or searches based on the dataset provided.
        ///</summary>
        ///<param name = "searchDataTable">A strongly-typed datatable containing search rows.</param>
        int Save(int searchId, string name, Guid userId, string searchCriteriaXml, DateTime lastModified);

        void DeleteSearch(int searchId);
    }
}