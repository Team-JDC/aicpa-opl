#region

using System.Data;
using System.Data.SqlClient;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;
using System;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    ///<summary>
    ///  Methods for getting sites and creating and updating sites and their related information.
    ///</summary>
    public class SiteDalc : DestroyerDalc, ISiteDalc
    {
        #region Constants

        #region Stored Procedures

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITE = "D_GetSite";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITES = "D_GetSites";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITESALL = "D_GetSitesAll";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITESBYBOOK = "D_GetSitesByBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITESBYBUILDSTATUS = "D_GetSitesByBuildStatus";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITESBYINDEXBUILDSTATUS = "D_GetSitesByIndexBuildStatus";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITESBYSITESTATUS = "D_GetSitesByStatus";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITEBYNAME = "D_GetSiteByName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITEBYSTATUS = "D_GetSiteByStatus";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITEBYNAMEANDVERSION = "D_GetSiteByNameAndVersion";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSITE = "D_InsertSite";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNEXTBOOKSITETOCNODE = "D_GetNextBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETPREVIOUSBOOKSITETOCNODE = "D_GetPreviousBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNEXTBOOKSITETOCNODEFILTERED = "D_GetNextBookFiltered";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETPREVIOUSBOOKSITETOCNODEFILTERED = "D_GetPreviousBookFiltered";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETIMMEDIATEDESCENDANTNODESSITETOC = "D_GetImmediateDescendantNodesSiteToc";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITETOCNODES = "D_GetSiteTocNodes";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSITEBOOK = "D_InsertSiteBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_REMOVESITEBOOK = "D_RemoveSiteBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_UPDATESITE = "D_UpdateSite";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSITEFOLDER = "D_InsertSiteFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSITETOCNODE = "D_InsertSiteTocNode";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSITEFOLDER = "D_GetSiteFolder";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETALTERNATEBOOK = "D_GetAlternateBookForTargetPtr";
        //dam added to retrieve alternate book for reused content

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETSTANDARDTYPES = "D_XRef_GetStandardTypes";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETSTANDARDNUMBERSFORSTANDARDTYPE = "D_XRef_GetStandardNumbersForStandardType";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETCODBYSTANDARD = "D_XRef_GetCodByStandard";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETSTANDARDBYCOD = "D_XRef_GetStandardByCod";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETTOPICS = "D_XRef_GetTopics";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETSUBTOPICSBYTOPIC = "D_XRef_GetSubtopicByTopic";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_XREF_GETSECTIONSBYTOPICSUBTOPIC = "D_XRef_GetSectionByTopicSubTopic";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_SJOIN_GETDOCSBYTOPICSECTION = "D_SJoin_getDocsByTopicSection";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_SJOIN_GETSECTIONSBYTOPIC = "D_SJoin_GetSectionsByTopic";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_SJOIN_GETTOPICS = "D_SJoin_GetTopics";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSUBTOPICSBYTOPIC = "D_Goto_GetSubTopicByTopic";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSECTIONSBYTOPICSUBTOPIC = "D_Goto_GetSectionByTopicSubtopic";

        #endregion Stored Procedures

        #region Dalc Errors

        private const string ERROR_GETSITE = "Error getting a site.";
        private const string ERROR_GETSITES = "Error getting sites.";
        private const string ERROR_GETCHILDSITETOCNODES = "Error getting a child site toc node.";
        private const string ERROR_GETSITETOCNODES = "Error getting a site toc node.";
        private const string ERROR_SAVE = "Error saving site information.";
        private const string ERROR_GETPREVIOUSBOOK = "Error getting previous book.";
        private const string ERROR_GETNEXTBOOK = "Error getting next book.";
        private const string ERROR_INSERTSITEBOOKS = "Error inserting site books.";
        private const string ERROR_REMOVESITEBOOK = "Error removing site book.";
        private const string ERROR_GETSITEFOLDER = "Error getting a site folder.";

        private const string ERROR_GETALTERNATEBOOK = "Error getting the alternate book for the site-book convination.";
        //dam added for alternate book

        private const string ERROR_GETXREFSTANDARDTYPES =
            "Error getting the unique collection of the original Standard Types.";

        private const string ERROR_GETXREFSTANDARDNUMBERSFORSTANDARDTYPE =
            "Error getting the Standard Numbers collection for specified Standard Type from the original Standards.";

        private const string ERROR_GETXREFCODBYSTANDARD =
            "Error getting the Cross Reference Codification search results for specified Standard Type and Standard Number from the original Standards.";

        private const string ERROR_GETXREFTOPICS = "Error getting the Cross Reference Codification Topics";
        private const string ERROR_GETSJOINTOPICS = "Error getting database information for the Join Section Tab";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_SITEDALC = "SiteDalc";
        private const string METHOD_GETSITE = "GetSite";
        private const string METHOD_GETSITES = "GetSites";
        private const string METHOD_GETCHILDSITETOCNODES = "GetChildSiteTocNodes";
        private const string METHOD_GETSITETOCNODES = "GetSiteTocNodes";
        private const string METHOD_SAVE = "Save";
        private const string METHOD_GETPREVIOUSBOOK = "GetPreviousBook";
        private const string METHOD_GETNEXTBOOK = "GetNextBook";
        private const string METHOD_INSERTSITEBOOKS = "InsertSiteBooks";
        private const string METHOD_REMOVESITEBOOK = "RemoveSiteBook";
        private const string METHOD_GETSITEFOLDER = "GetSiteFolder";

        private const string METHOD_GETALTERNATEBOOK = "GetAlternateBook";
        //dam to get alternate book for a given site/book

        private const string METHOD_GETXREFSTANDARDTYPES = "GetXRefStandardTypes";
        private const string METHOD_GETXREFTOPICS = "GetXRefTopics";
        private const string METHOD_GETXREFSUBTOPICSBYTOPIC = "GetXRefSubtopicsByTopic";
        private const string METHOD_GETXREFSECTIONSBYTOPICSUBTOPIC = "GetXRefSectionsByTopicSubTopic";
        private const string METHOD_GETSJOINDOCSBYTOPICSECTION = "GetSJoinDocsByTopicSection";
        private const string METHOD_GETSJOINSECTIONSBYTOPIC = "GetSJoinSectionsByTopic";
        private const string METHOD_GETSJOINTOPICS = "GetSJoinTopics";
        private const string METHOD_GETSUBTOPICSBYTOPIC = "GetSubtopicByTopic";
        private const string METHOD_GETSECTIONBYTOPICSUBTOPIC = "GetSectionByTopicSubtopic";
        private const string METHOD_GETXREFSTANDARDNUMBERSFORSTANDARDTYPE = "GetXRefStandardNumbersForStandardType";
        private const string METHOD_GETXREFCODBYSTANDARD = "GetXRefCodByStandard";
        private const string METHOD_GETXREFSTANDARDBYCOD = "GetXRefStandardByCod";

        #endregion Module and Method Names

        #endregion Constants

        #region Constructors

        internal SiteDalc()
        {
            moduleName = MODULE_SITEDALC;
        }

        #endregion Constructors

        #region Methods

        #region Private Methods

        private SiteDs GetSiteDataSet(string methodName, string errorName, string storedProcedureName,
                                      params object[] parameterValues)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs,
                        new string[2] {siteDs.Site.TableName, siteDs.SiteBook.TableName}, parameterValues);
            return siteDs;
        }

        private SiteFolderDs GetSiteFolderDataSet(string methodName, string errorName, string storedProcedureName,
                                                  params object[] parameterValues)
        {
            SiteFolderDs siteFolderDs = new SiteFolderDs();
            FillDataset(methodName, errorName, storedProcedureName, siteFolderDs,
                        new[] {siteFolderDs.SiteFolder.TableName}, parameterValues);
            return siteFolderDs;
        }

        private SiteTocNodeDs.SiteTocNodeDataTable GetSiteTocNodeDataTable(string methodName, string errorName,
                                                                           string storedProcedureName,
                                                                           params object[] parameterValues)
        {
            SiteTocNodeDs siteTocNodeDs = new SiteTocNodeDs();
            FillDataset(methodName, errorName, storedProcedureName, siteTocNodeDs,
                        new[] {siteTocNodeDs.SiteTocNode.TableName}, parameterValues);
            return siteTocNodeDs.SiteTocNode;
        }

        private SiteTocNodeDs.SiteTocNodeRow GetSiteTocNodeRow(string methodName, string errorName,
                                                               string storedProcedureName,
                                                               params object[] parameterValues)
        {
            SiteTocNodeDs.SiteTocNodeRow retRow = null;
            SiteTocNodeDs siteTocNodeDs = new SiteTocNodeDs();
            FillDataset(methodName, errorName, storedProcedureName, siteTocNodeDs,
                        new[] {siteTocNodeDs.SiteTocNode.TableName}, parameterValues);
            if (siteTocNodeDs.SiteTocNode.Rows.Count > 0)
            {
                retRow = (SiteTocNodeDs.SiteTocNodeRow) siteTocNodeDs.SiteTocNode.Rows[0];
            }
            return retRow;
        }

        private void Save(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet,
                          string tableName)
        {
            UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertCommand, deleteCommand, updateCommand, dataSet, tableName);
        }

        //dam 
        // This method will only return 1 book as alternate book. If a list of alternate books is needed the method will have to change
        private string GetAlternateBook(string methodName, string errorName, string storedProcedureName,
                                        params object[] parameterValues)
        {
            SqlDataReader sqlDataReader;
            string alternateBook = string.Empty;

            try
            {
                sqlDataReader = ExecuteReader(methodName, errorName, storedProcedureName, parameterValues);
                if (sqlDataReader.Read())
                {
                    alternateBook = sqlDataReader.GetString(1);
                }
            }
            catch (Exception ex)
            {
                string parameters="";
                foreach (object obj in parameterValues)
                {
                    parameters += obj.ToString()+" ";
                }
                IEvent logEvent = new Event(EventType.Error, DateTime.Now, DestroyerDalc.ERROR_SEVERITY_DB_ERROR,
                                                moduleName, "GetAlternateBook", "Error getting alternate book",
                                                "Stored Procedure: "+storedProcedureName+", Paramater Values: "+parameters);
                logEvent.Save(false);
            }
            return alternateBook;
        }

        private string[] GetXRefTopics(string methodName, string errorName, string storedProcedureName)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName});
            DataRowCollection rows = siteDs.XRef.Rows;

            string[] topics = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                topics[i] = rows[i]["Topic"].ToString();
            }

            return topics;
        }

        private string[] GetXRefSubtopicsByTopic(string methodName, string errorName, string storedProcedureName,
                                                 string Topic)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName}, Topic);
            DataRowCollection rows = siteDs.XRef.Rows;

            string[] Subtopics = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                Subtopics[i] = rows[i]["SubTopic"].ToString();
            }

            return Subtopics;
        }

        private string[] GetXRefSectionsByTopicSubtopic(string methodName, string errorName, string storedProcedureName,
                                                        string Topic, string Subtopic)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName}, Topic,
                        Subtopic);
            DataRowCollection rows = siteDs.XRef.Rows;

            string[] Subtopics = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                Subtopics[i] = rows[i]["Section"].ToString();
            }

            return Subtopics;
        }

        private string[] GetXRefStandardTypes(string methodName, string errorName, string storedProcedureName)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName});
            DataRowCollection rows = siteDs.XRef.Rows;

            string[] standards = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                standards[i] = (string) rows[i]["StandardType"];
            }

            return standards;
        }

        private string[] GetXRefStandardNumbersForStandardType(string methodName, string errorName,
                                                               string storedProcedureName, string standardType)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName},
                        standardType);
            DataRowCollection rows = siteDs.XRef.Rows;

            string[] numbers = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                numbers[i] = (string) rows[i]["StandardID"];
            }

            return numbers;
        }

        private SiteDs.XRefRow[] GetXRefCodByStandard(string methodName, string errorName, string storedProcedureName,
                                                      string standardType, string standardNum)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName},
                        standardType, standardNum);

            return (SiteDs.XRefRow[]) siteDs.XRef.Select();
        }

        private SiteDs.XRefRow[] GetXRefStandardByCod(string methodName, string errorName, string storedProcedureName,
                                                      string topic, string subtopic, string section)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.XRef.TableName}, topic,
                        subtopic, section);

            return (SiteDs.XRefRow[]) siteDs.XRef.Select();
        }

        private SiteDs.Cod_MetaRow[] GetSJoinDocsByTopicSection(string methodName, string errorName,
                                                                string storedProcedureName, string topicnum,
                                                                string sectionnum, int intersection)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.Cod_Meta.TableName},
                        topicnum, sectionnum, intersection);

            return (SiteDs.Cod_MetaRow[]) siteDs.Cod_Meta.Select();
        }

        private SiteDs.Cod_MetaRow[] GetSJoinSectionsByTopic(string methodName, string errorName,
                                                             string storedProcedureName, string topicnum,
                                                             int intersection, int sec)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.Cod_Meta.TableName},
                        topicnum, intersection, sec);

            return (SiteDs.Cod_MetaRow[]) siteDs.Cod_Meta.Select();
        }

        private SiteDs.Cod_MetaRow[] GetSJoinTopics(string methodName, string errorName, string storedProcedureName)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.Cod_Meta.TableName});

            return (SiteDs.Cod_MetaRow[]) siteDs.Cod_Meta.Select();
        }

        private SiteDs.Cod_MetaRow[] GetSubtopicByTopic(string methodName, string errorName, string storedProcedureName,
                                                        string Topicnum)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.Cod_Meta.TableName},
                        Topicnum);

            return (SiteDs.Cod_MetaRow[]) siteDs.Cod_Meta.Select();
        }

        private SiteDs.Cod_MetaRow[] GetSectionByTopicSubtopic(string methodName, string errorName,
                                                               string storedProcedureName, string Topicnum,
                                                               string Subtopicnum)
        {
            SiteDs siteDs = new SiteDs();
            FillDataset(methodName, errorName, storedProcedureName, siteDs, new[] {siteDs.Cod_Meta.TableName},
                        Topicnum, Subtopicnum);

            return (SiteDs.Cod_MetaRow[]) siteDs.Cod_Meta.Select();
        }

        #endregion Private Methods

        #region ISiteDalc Methods

        #region GetSite

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <returns></returns>
        public SiteDs GetSite(int siteId)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITE, siteId);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteName"></param>
        /// <returns></returns>
        public SiteDs GetSite(string siteName)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITEBYNAME, siteName);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteStatusCode"></param>
        /// <returns></returns>
        public SiteDs GetSite(SiteStatus siteStatusCode)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITEBYSTATUS, (int) siteStatusCode);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteName"></param>
        /// <param name = "siteVersion"></param>
        /// <returns></returns>
        public SiteDs GetSite(string siteName, int siteVersion)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITEBYNAMEANDVERSION, siteName, siteVersion);
        }

        /// <summary>
        /// </summary>
        /// <param name = "latestVersion"></param>
        /// <returns></returns>
        public SiteDs GetSites(bool latestVersion, bool includeArchived)
        {
            return GetSiteDataSet(METHOD_GETSITES, ERROR_GETSITES, includeArchived ? SP_GETSITESALL : SP_GETSITES,
                                  latestVersion);
        }

        /// <summary>
        /// </summary>
        /// <param name = "bookInstanceId"></param>
        /// <returns></returns>
        public SiteDs GetSites(int bookInstanceId)
        {
            return GetSiteDataSet(METHOD_GETSITES, ERROR_GETSITES, SP_GETSITESBYBOOK, bookInstanceId);
        }

        /// <summary>
        /// </summary>
        /// <param name = "buildStatus"></param>
        /// <returns></returns>
        public SiteDs GetSites(SiteBuildStatus buildStatus)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITESBYBUILDSTATUS, (int) buildStatus);
        }

        /// <summary>
        /// </summary>
        /// <param name = "indexBuildStatus"></param>
        /// <returns></returns>
        public SiteDs GetSites(SiteIndexBuildStatus indexBuildStatus)
        {
            return GetSiteDataSet(METHOD_GETSITE, ERROR_GETSITE, SP_GETSITESBYINDEXBUILDSTATUS, (int) indexBuildStatus);
        }

        /// <summary>
        ///   Get all sites matching the specified site status
        /// </summary>
        /// <param name = "siteStatus">The site status to match when retrieving sites</param>
        /// <returns>A strongly-typed dataset containing site information</returns>
        public SiteDs GetSites(SiteStatus siteStatus)
        {
            return GetSiteDataSet(METHOD_GETSITES, ERROR_GETSITES, SP_GETSITESBYSITESTATUS, siteStatus);
        }

        #endregion GetSite

        #region GetSiteFolder

        /// <summary>
        ///   Gets the site folder with the specified id
        /// </summary>
        /// <param name = "siteId">The id of the site containing the site folder you would like to retrieve</param>
        /// <param name = "siteFolderId">The id for the folder you would like to retrieve</param>
        /// <returns>A site folder dataset containing the site folder</returns>
        public SiteFolderDs GetSiteFolder(int siteId, int siteFolderId)
        {
            return GetSiteFolderDataSet(METHOD_GETSITEFOLDER, ERROR_GETSITEFOLDER, SP_GETSITEFOLDER, siteId,
                                        siteFolderId);
        }

        #endregion GetSiteFolder

        #region GetSiteTocNodeDataTable

        /// <summary>
        ///   Returns a Site toc node dataset containing the Site toc node children underneath the context node
        /// </summary>
        /// <param name = "nodeId">The id of the context node for which you want to retrieve children</param>
        /// <param name = "nodeType">The node type of the context node</param>
        /// <returns>A Site toc node dataset</returns>
        public SiteTocNodeDs.SiteTocNodeDataTable GetChildSiteTocNodes(int nodeId, NodeType nodeType)
        {
            return GetSiteTocNodeDataTable(METHOD_GETCHILDSITETOCNODES, ERROR_GETCHILDSITETOCNODES,
                                           SP_GETIMMEDIATEDESCENDANTNODESSITETOC, nodeId, (int) nodeType);
        }

        /// <summary>
        ///   Returns a Site toc node dataset containing all Site toc nodes for the specified Site
        /// </summary>
        /// <param name = "siteId">The id of the Site for which you want to retrieve children</param>
        /// <returns>A Site toc node dataset</returns>
        public SiteTocNodeDs.SiteTocNodeDataTable GetSiteTocNodes(int siteId)
        {
            return GetSiteTocNodeDataTable(METHOD_GETSITETOCNODES, ERROR_GETSITETOCNODES, SP_GETSITETOCNODES, siteId);
        }

        #endregion GetSiteTocNodeDataTable

        #region Save

        /// <summary>
        /// </summary>
        /// <param name = "siteDs"></param>
        public void Save(SiteDs siteDs)
        {
            SiteDs.SiteDataTable siteDt = siteDs.Site;
            SqlCommand insertSiteCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString), SP_INSERTSITE,
                                                                   siteDt.SiteIdColumn.ColumnName,
                                                                   siteDt.NameColumn.ColumnName,
                                                                   siteDt.TitleColumn.ColumnName,
                                                                   siteDt.DescriptionColumn.ColumnName,
                                                                   siteDt.RequestedSiteStatusCodeColumn.ColumnName,
                                                                   siteDt.SearchUriColumn.ColumnName,
                                                                   siteDt.OnlineColumn.ColumnName,
                                                                   siteDt.ArchivedColumn.ColumnName,
                                                                   siteDt.BuildStatusCodeColumn.ColumnName,
                                                                   siteDt.IndexBuildStatusCodeColumn.ColumnName,
                                                                   siteDt.SiteTemplateXmlColumn.ColumnName);
            SqlCommand updateSiteCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString), SP_UPDATESITE,
                                                                   siteDt.SiteIdColumn.ColumnName,
                                                                   siteDt.NameColumn.ColumnName,
                                                                   siteDt.TitleColumn.ColumnName,
                                                                   siteDt.RequestedSiteStatusCodeColumn.ColumnName,
                                                                   siteDt.SiteStatusCodeColumn.ColumnName,
                                                                   siteDt.DescriptionColumn.ColumnName,
                                                                   siteDt.SearchUriColumn.ColumnName,
                                                                   siteDt.OnlineColumn.ColumnName,
                                                                   siteDt.ArchivedColumn.ColumnName,
                                                                   siteDt.BuildStatusCodeColumn.ColumnName,
                                                                   siteDt.IndexBuildStatusCodeColumn.ColumnName,
                                                                   siteDt.SiteTemplateXmlColumn.ColumnName);
            Save(insertSiteCommand, null, updateSiteCommand, siteDs, siteDs.Site.TableName);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteFolderDs"></param>
        public void Save(SiteFolderDs siteFolderDs)
        {
            SiteFolderDs.SiteFolderDataTable siteFolderDt = siteFolderDs.SiteFolder;
            SqlCommand insertSiteFolderCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                         SP_INSERTSITEFOLDER,
                                                                         siteFolderDt.FolderIdColumn.ColumnName,
                                                                         siteFolderDt.NameColumn.ColumnName,
                                                                         siteFolderDt.TitleColumn.ColumnName,
                                                                         siteFolderDt.UriColumn.ColumnName);
            Save(insertSiteFolderCommand, null, null, siteFolderDs, siteFolderDs.SiteFolder.TableName);
        }

        /// <summary>
        /// </summary>
        /// <param name = "sitetocNodeDs"></param>
        public void Save(SiteTocNodeDs sitetocNodeDs)
        {
            SiteTocNodeDs.SiteTocNodeDataTable siteTocNodeDt = sitetocNodeDs.SiteTocNode;
            SqlCommand insertSiteTocNodeCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                          SP_INSERTSITETOCNODE,
                                                                          siteTocNodeDt.SiteTocIdColumn.ColumnName,
                                                                          siteTocNodeDt.SiteIdColumn.ColumnName,
                                                                          siteTocNodeDt.NodeIdColumn.ColumnName,
                                                                          siteTocNodeDt.NodeTypeIdColumn.ColumnName,
                                                                          siteTocNodeDt.LeftColumn.ColumnName,
                                                                          siteTocNodeDt.RightColumn.ColumnName);
            Save(insertSiteTocNodeCommand, null, null, sitetocNodeDs, sitetocNodeDs.SiteTocNode.TableName);
        }

        #endregion Save

        #region GetSiteTocNodeRow

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <returns></returns>
        public SiteTocNodeDs.SiteTocNodeRow GetPreviousBook(int siteId, int bookId)
        {
            return GetSiteTocNodeRow(METHOD_GETPREVIOUSBOOK, ERROR_GETPREVIOUSBOOK, SP_GETPREVIOUSBOOKSITETOCNODE,
                                     siteId, bookId);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <param name = "bookDomain"></param>
        /// <returns></returns>
        public SiteTocNodeDs.SiteTocNodeRow GetPreviousBook(int siteId, int bookId, string bookDomain)
        {
            return GetSiteTocNodeRow(METHOD_GETPREVIOUSBOOK, ERROR_GETPREVIOUSBOOK,
                                     SP_GETPREVIOUSBOOKSITETOCNODEFILTERED, siteId, bookId, bookDomain);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <returns></returns>
        public SiteTocNodeDs.SiteTocNodeRow GetNextBook(int siteId, int bookId)
        {
            return GetSiteTocNodeRow(METHOD_GETNEXTBOOK, ERROR_GETNEXTBOOK, SP_GETNEXTBOOKSITETOCNODE, siteId, bookId);
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <param name = "bookDomain"></param>
        /// <returns></returns>
        public SiteTocNodeDs.SiteTocNodeRow GetNextBook(int siteId, int bookId, string bookDomain)
        {
            return GetSiteTocNodeRow(METHOD_GETNEXTBOOK, ERROR_GETNEXTBOOK, SP_GETNEXTBOOKSITETOCNODEFILTERED, siteId,
                                     bookId, bookDomain);
        }

        #endregion GetSiteTocNodeRow

        /// <summary>
        ///   Adds a book to a site. The book is added to the bottom of the site toc
        /// </summary>
        /// <param name = "siteBookTable">A SiteBookDataTable containing the books you wish to add to a site.</param>
        public void InsertSiteBooks(SiteDs.SiteBookDataTable siteBookTable)
        {
            SqlCommand insertSiteBookCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                       SP_INSERTSITEBOOK,
                                                                       siteBookTable.SiteIdColumn.ColumnName,
                                                                       siteBookTable.BookInstanceIdColumn.ColumnName);
            UpdateDataset(METHOD_INSERTSITEBOOKS, ERROR_INSERTSITEBOOKS, insertSiteBookCommand, null, null,
                          siteBookTable.DataSet, siteBookTable.TableName);
        }

        /// <summary>
        ///   Removes the association between a site and the specified books.
        /// </summary>
        /// <param name = "siteId">the id of the site from which to remove the books</param>
        /// <param name = "bookId">The id of the book you wish to remove from the site</param>
        public void RemoveSiteBook(int siteId, int bookId)
        {
            ExecuteNonQuery(METHOD_REMOVESITEBOOK, ERROR_REMOVESITEBOOK, SP_REMOVESITEBOOK, siteId, bookId);
        }

        /// <summary>
        ///   DAM
        ///   Get alternate book for site/book relation
        /// </summary>
        /// <param name = "siteId">the id of the site from which to get alternate book</param>
        /// <param name = "targetPtr">The targetPtr inside of the document</param>
        /// <param name = "targetDoc">Document to use (book)</param>
        /// <param name = "siteDomain">Comma delimited string with a list of all books for given site (user level)</param>
        public string AlternateBook(int siteId, string targetPtr, string targetDoc, string siteDomain)
        {
            return GetAlternateBook(METHOD_GETALTERNATEBOOK, ERROR_GETALTERNATEBOOK, SP_GETALTERNATEBOOK, targetPtr,
                                    targetDoc, siteDomain, siteId);
        }

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array contaianing the names of the original Standard Types.</returns>
        public string[] GetXRefStandardTypes()
        {
            return GetXRefStandardTypes(METHOD_GETXREFSTANDARDTYPES, ERROR_GETXREFSTANDARDTYPES,
                                        SP_XREF_GETSTANDARDTYPES);
        }

        /// <summary>
        ///   Gets the collection of unique Standard Numbers for a specified Standard Type from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter Standard Numbers on.</param>
        /// <returns>String array containing the Standard Numbers for the specified Standard Type.</returns>
        public string[] GetXRefStandardNumbersForStandardType(string standardType)
        {
            return GetXRefStandardNumbersForStandardType(METHOD_GETXREFSTANDARDNUMBERSFORSTANDARDTYPE,
                                                         ERROR_GETXREFSTANDARDNUMBERSFORSTANDARDTYPE,
                                                         SP_XREF_GETSTANDARDNUMBERSFORSTANDARDTYPE, standardType);
        }

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Standard Type and Standard Number from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter on in searching for cross-referenced Codification entires.</param>
        /// <param name = "standardNum">Standard Number in which to filter on in searching for cross-referenced Codification entries.</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        public SiteDs.XRefRow[] GetXRefCodByStandard(string standardType, string standardNum)
        {
            return GetXRefCodByStandard(METHOD_GETXREFCODBYSTANDARD, ERROR_GETXREFCODBYSTANDARD,
                                        SP_XREF_GETCODBYSTANDARD, standardType, standardNum);
        }

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Topic,Subtopic and section
        /// </summary>
        /// <param name = "topic">Codification Topic</param>
        /// <param name = "subtopic">Codification SubTopic</param>
        /// <param name = "section">Codification Section</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        public SiteDs.XRefRow[] GetXRefStandardByCod(string topic, string subtopic, string section)
        {
            return GetXRefStandardByCod(METHOD_GETXREFSTANDARDBYCOD, ERROR_GETXREFCODBYSTANDARD,
                                        SP_XREF_GETSTANDARDBYCOD, topic, subtopic, section);
        }

        ///<summary>
        ///  Get the collection of unique Codification Topics for use in Xref
        ///</summary>
        ///<returns>Strung array containing the topic numbers</returns>
        public string[] GetXRefTopics()
        {
            return GetXRefTopics(METHOD_GETXREFTOPICS, ERROR_GETXREFTOPICS, SP_XREF_GETTOPICS);
        }

        /// <summary>
        /// Gets the X ref subtopics by topic.	
        /// </summary>
        /// <param name="Topic">The topic.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] GetXRefSubtopicsByTopic(string Topic)
        {
            return GetXRefSubtopicsByTopic(METHOD_GETXREFSUBTOPICSBYTOPIC, ERROR_GETXREFTOPICS,
                                           SP_XREF_GETSUBTOPICSBYTOPIC, Topic);
        }

        /// <summary>
        /// Gets the X ref sections by topic subtopic.	
        /// </summary>
        /// <param name="Topic">The topic.</param>
        /// <param name="Subtopic">The subtopic.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] GetXRefSectionsByTopicSubtopic(string Topic, string Subtopic)
        {
            return GetXRefSectionsByTopicSubtopic(METHOD_GETXREFSECTIONSBYTOPICSUBTOPIC, ERROR_GETXREFTOPICS,
                                                  SP_XREF_GETSECTIONSBYTOPICSUBTOPIC, Topic, Subtopic);
        }

        /// <summary>
        /// Gets the S join docs by topic section.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="Sectionnum">The sectionnum.</param>
        /// <param name="Intersection">The intersection.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinDocsByTopicSection(string Topicnum, string Sectionnum, int Intersection)
        {
            return GetSJoinDocsByTopicSection(METHOD_GETSJOINDOCSBYTOPICSECTION, ERROR_GETSJOINTOPICS,
                                              SP_SJOIN_GETDOCSBYTOPICSECTION, Topicnum, Sectionnum, Intersection);
        }

        /// <summary>
        /// Gets the S join sections by topic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="intersection">The intersection.</param>
        /// <param name="sec">The sec.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinSectionsByTopic(string Topicnum, int intersection, int sec)
        {
            return GetSJoinSectionsByTopic(METHOD_GETSJOINSECTIONSBYTOPIC, ERROR_GETSJOINTOPICS,
                                           SP_SJOIN_GETSECTIONSBYTOPIC, Topicnum, intersection, sec);
        }

        /// <summary>
        /// Gets the S join topics.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSJoinTopics()
        {
            return GetSJoinTopics(METHOD_GETSJOINTOPICS, ERROR_GETSJOINTOPICS, SP_SJOIN_GETTOPICS);
        }

        /// <summary>
        /// Gets the subtopic by topic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSubtopicByTopic(string Topicnum)
        {
            return GetSubtopicByTopic(METHOD_GETSUBTOPICSBYTOPIC, ERROR_GETSJOINTOPICS, SP_GETSUBTOPICSBYTOPIC, Topicnum);
        }

        /// <summary>
        /// Gets the section by topic subtopic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="Subtopicnum">The subtopicnum.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SiteDs.Cod_MetaRow[] GetSectionByTopicSubtopic(string Topicnum, string Subtopicnum)
        {
            return GetSectionByTopicSubtopic(METHOD_GETSECTIONBYTOPICSUBTOPIC, ERROR_GETSJOINTOPICS,
                                             SP_GETSECTIONSBYTOPICSUBTOPIC, Topicnum, Subtopicnum);
        }

        #endregion ISiteDalc Methods

        #endregion Methods
    }
}