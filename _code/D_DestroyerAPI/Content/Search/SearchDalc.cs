using System;
using System.Collections.Generic;
using System.Linq;

using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;

namespace AICPA.Destroyer.Content.Search
{
    /// <summary>
    ///   Summary description for SearchDalc.
    /// </summary>
    public class SearchDalc : DestroyerDalc, ISearchDalc
    {
        #region Constants

        #region Dalc Errors

        private const string ERROR_GETSEARCH = "Error getting a saved search by ID.";
        private const string ERROR_GETSEARCHES = "Error getting a user's saved searches.";

        #endregion

        #region Module and Method Names

        private const string MODULE_SEARCHDALC = "SearchDalc";
        private const string METHOD_GETSEARCH = "GetSearch";
        private const string METHOD_GETSEARCHES = "GetSearches";

        #endregion Module and Method Names

        #region Stored Procedures

        private const string SP_INSERTSAVEDSEARCH = "dbo.D_InsertSavedSearch";
        private const string SP_UPDATESAVEDSEARCH = "dbo.D_UpdateSavedSearch";
        private const string SP_GETSAVEDSEARCH = "dbo.D_GetSavedSearch";
        private const string SP_GETSAVEDSEARCHESFORUSER = "dbo.D_GetSavedSearchesForUser";
        private const string SP_DELETESAVEDSEARCH = "dbo.D_DeleteSavedSearch";
        private const string SP_GETALLSUGGESTEDSEARCHTERMS = "dbo.D_GetAllSuggestedTerms";
        private const string SP_GETMATCHINGSUGGESTEDSEARCHTERMS = "dbo.D_GetMatchingSuggestedTerms";
        private const string SP_INCREMENTSUGGESTEDSEARCHTERM = "dbo.D_IncrementSuggestedTerm";

        #endregion Stored Procedures

        #endregion Constants

        #region Constructors

        public SearchDalc()
        {
            moduleName = MODULE_SEARCHDALC;
        }

        #endregion Constructors

        #region Methods

        public SearchDs.SearchRow GetSearch(int searchId)
        {
            SearchDs searchDs = new SearchDs();
            FillDataset(METHOD_GETSEARCH, ERROR_GETSEARCH, SP_GETSAVEDSEARCH, searchDs,
                        new [] { searchDs.Search.TableName }, searchId);
            return searchDs.Search.SingleOrDefault();
        }

        public IEnumerable<SearchDs.SearchRow> GetSearches(Guid UserId)
        {
            SearchDs searchDs = new SearchDs();
            FillDataset(METHOD_GETSEARCHES, ERROR_GETSEARCHES, SP_GETSAVEDSEARCHESFORUSER, searchDs,
                        new [] { searchDs.Search.TableName }, UserId);
            return (SearchDs.SearchRow[])searchDs.Search.Select();
        }

        public int Save(int searchId, string name, Guid userId, string searchCriteriaXml, DateTime lastModified)
        {
            int returnId;

            if (searchId == 0)
            {
                var noClueWhyThisIsDecimal = SqlHelper.ExecuteScalar(DBConnectionString, SP_INSERTSAVEDSEARCH,
                                                                     userId, name, searchCriteriaXml, lastModified);

                returnId = Convert.ToInt32(noClueWhyThisIsDecimal);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_UPDATESAVEDSEARCH,
                                          searchId, userId, name, searchCriteriaXml, lastModified);

                returnId = searchId;
            }

            return returnId;
        }

        public void DeleteSearch(int searchId)
        {
            SqlHelper.ExecuteNonQuery(DBConnectionString, SP_DELETESAVEDSEARCH, searchId);
        }

        public string[] GetSuggestedSearchTerms()
        {
            List<string> terms = new List<string>();

            SqlDataReader results = SqlHelper.ExecuteReader(DBConnectionString, SP_GETALLSUGGESTEDSEARCHTERMS, null);
            while (results.Read())
            {
                terms.Add(results.GetString(0));
            }
            return terms.ToArray();
        }

        public string[] GetSuggestedSearchTerms(string root)
        {
            List<string> terms = new List<string>();

            SqlDataReader results = SqlHelper.ExecuteReader(DBConnectionString, SP_GETMATCHINGSUGGESTEDSEARCHTERMS, new object[] {root});
            while (results.Read())
            {
                terms.Add(results.GetString(0));
            }
            return terms.ToArray();
        }

        public void IncrementSuggestedTerm(string term)
        {
            SqlHelper.ExecuteNonQuery(DBConnectionString, SP_INCREMENTSUGGESTEDSEARCHTERM, new object[] {term});
        }

        #endregion Methods
    }
}