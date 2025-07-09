#region

using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for ISiteDALC.
    /// </summary>
    public interface ISiteDalc
    {
        ///<summary>
        ///  Get a site based on its id.
        ///</summary>
        ///<param name = "siteId">The id of the site to be returned.</param>
        ///<returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSite(int siteId);

        ///<summary>
        ///  Get a site based on its name.
        ///</summary>
        ///<param name = "siteName">The name of the site to be returned.</param>
        ///<returns>A strongly-typed dataset containing site information. The most recent (highest version) of the site by the specified name will be returned.</returns>
        SiteDs GetSite(string siteName);

        /// <summary>
        ///   Get a site based on its status.
        /// </summary>
        /// <param name = "siteStatusCode">The status of the site to be returned</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSite(SiteStatus siteStatusCode);

        /// <summary>
        ///   Get a site based on its name and version.
        /// </summary>
        /// <param name = "siteName">The name of the site to be returned.</param>
        /// <param name = "siteVersion">The version of the site to be returned</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSite(string siteName, int siteVersion);

        /// <summary>
        ///   Returns a list of all sites in the datastore.
        /// </summary>
        /// <param name = "latestVersion">Determines whether the retrieval should be for all sites or only the sites of the latest version.</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSites(bool latestVersion, bool includeArchived);

        /// <summary>
        ///   Returns a list of all sites in the datastore matching the specified build status
        /// </summary>
        /// <param name = "buildStatus">Specifies the build status of the sites you wish to retrieve.</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSites(SiteBuildStatus buildStatus);

        /// <summary>
        ///   Returns a list of all sites in the datastore matching the specified index build status
        /// </summary>
        /// <param name = "indexBuildStatus">Specifies the index build status of the sites you wish to retrieve.</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSites(SiteIndexBuildStatus indexBuildStatus);

        /// <summary>
        ///   Gets sites based on the bookInstanceId provided
        /// </summary>
        /// <param name = "bookInstanceId">Limits the sites returned to those containing this book.</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSites(int bookInstanceId);

        /// <summary>
        ///   Get all sites matching the specified site status
        /// </summary>
        /// <param name = "siteStatus">The site status to match when retrieving sites</param>
        /// <returns>A strongly-typed dataset containing site information.</returns>
        SiteDs GetSites(SiteStatus siteStatus);

        /// <summary>
        ///   Gets the site folder with the specified id
        /// </summary>
        /// <param name = "siteId">The id for the site containing the site folder you would like to retrieve</param>
        /// <param name = "siteFolderId">The id for the folder you would like to retrieve</param>
        /// <returns>A site folder dataset containing the site folder</returns>
        SiteFolderDs GetSiteFolder(int siteId, int siteFolderId);

        /// <summary>
        ///   Returns a Site toc node dataset containing the Site toc node children underneath the context node
        /// </summary>
        /// <param name = "nodeId">The id of the context node for which you want to retrieve children</param>
        /// <param name = "nodeType">The node type of the context node</param>
        /// <returns>A Site toc node dataset</returns>
        SiteTocNodeDs.SiteTocNodeDataTable GetChildSiteTocNodes(int nodeId, NodeType nodeType);

        /// <summary>
        ///   Returns a Site toc node dataset containing all Site toc nodes for the specified Site
        /// </summary>
        /// <param name = "siteId">The id of the site for which you want to retrieve children toc nodes</param>
        /// <returns>A site toc node dataset</returns>
        SiteTocNodeDs.SiteTocNodeDataTable GetSiteTocNodes(int siteId);

        ///<summary>
        ///  Insert, Update, or Delete a site or sites based on the dataset provided.
        ///</summary>
        ///<param name = "siteDs">A site dataset.</param>
        void Save(SiteDs siteDs);

        ///<summary>
        ///  Insert, Update, or Delete a site folder or site folders based on the dataset provided.
        ///</summary>
        ///<param name = "siteFolderDs">A site folder dataset.</param>
        void Save(SiteFolderDs siteFolderDs);

        ///<summary>
        ///  Insert, Update, or Delete a site toc node or site toc nodes based on the dataset provided.
        ///</summary>
        ///<param name = "sitetocNodeDs">A site toc node dataset.</param>
        void Save(SiteTocNodeDs sitetocNodeDs);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <returns></returns>
        SiteTocNodeDs.SiteTocNodeRow GetPreviousBook(int siteId, int bookId);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <returns></returns>
        SiteTocNodeDs.SiteTocNodeRow GetNextBook(int siteId, int bookId);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <param name = "bookDomain">The list of books allowed to be returned. Must be formatted for a SQL WHERE clause as follows:
        ///   'book1','book2','book3'</param>
        /// <returns></returns>
        SiteTocNodeDs.SiteTocNodeRow GetPreviousBook(int siteId, int bookId, string bookDomain);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        /// <param name = "bookDomain">The list of books allowed to be returned. Must be formatted for a SQL WHERE clause as follows:
        ///   'book1','book2','book3'</param>
        /// <returns></returns>
        SiteTocNodeDs.SiteTocNodeRow GetNextBook(int siteId, int bookId, string bookDomain);

        /// <summary>
        /// </summary>
        /// <param name = "siteBookTable"></param>
        void InsertSiteBooks(SiteDs.SiteBookDataTable siteBookTable);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "bookId"></param>
        void RemoveSiteBook(int siteId, int bookId);

        /// <summary>
        /// </summary>
        /// <param name = "siteId"></param>
        /// <param name = "targetPtr"></param>
        /// <param name = "targetDoc"></param>
        /// <param name = "siteDomain">The list of all books for the user on given site.  Comma delimited list
        ///   book,book1,book2</param>
        string AlternateBook(int siteId, string targetPtr, string targetDoc, string siteDomain);

        /// <summary>
        ///   Gets the collection of unique Standard Types from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        string[] GetXRefStandardTypes();

        /// <summary>
        ///   Gets the collection of unique Topics from the original standards before Codification.
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        string[] GetXRefTopics();

        /// <summary>
        ///   Gets the collection of unique Standard Numbers for a specified Standard Type from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter Standard Numbers on.</param>
        /// <returns>String array containing the Standard Numbers for the specified Standard Type.</returns>
        string[] GetXRefStandardNumbersForStandardType(string standardType);

        /// <summary>
        ///   Gets the collection of unique Subtopics for a topic .
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        string[] GetXRefSubtopicsByTopic(string Topic);

        /// <summary>
        ///   Gets the collection of unique section for a topic subtopic pair .
        /// </summary>
        /// <returns>String array containing the names of the original Standard Types.</returns>
        string[] GetXRefSectionsByTopicSubtopic(string Topic, string Subtopic);

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Standard Type and Standard Number from the original standards before Codification.
        /// </summary>
        /// <param name = "standardType">Standard Type in which to filter on in searching for cross-referenced Codification entires.</param>
        /// <param name = "standardNumber">Standard Number in which to filter on in searching for cross-referenced Codification entries.</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        SiteDs.XRefRow[] GetXRefCodByStandard(string standardType, string standardNumber);

        /// <summary>
        ///   Performs the search result for Codification entries for a specified Topic,Subtopic and section
        /// </summary>
        /// <param name = "topic">Codification Topic</param>
        /// <param name = "subtopic">Codification SubTopic</param>
        /// <param name = "section">Codification Section</param>
        /// <returns>Cross Reference search results in the form of XRefRow entries as declared in the SiteDS data set.</returns>
        SiteDs.XRefRow[] GetXRefStandardByCod(string topic, string subtopic, string section);

        /// <summary>
        /// Gets the S join docs by topic section.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="Sectionnum">The sectionnum.</param>
        /// <param name="intersection">The intersection.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        SiteDs.Cod_MetaRow[] GetSJoinDocsByTopicSection(string Topicnum, string Sectionnum, int intersection);
        /// <summary>
        /// Gets the S join sections by topic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="intersection">The intersection.</param>
        /// <param name="sec">The sec.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        SiteDs.Cod_MetaRow[] GetSJoinSectionsByTopic(string Topicnum, int intersection, int sec);
        /// <summary>
        /// Gets the S join topics.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        SiteDs.Cod_MetaRow[] GetSJoinTopics();
        /// <summary>
        /// Gets the subtopic by topic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        SiteDs.Cod_MetaRow[] GetSubtopicByTopic(string Topicnum);
        /// <summary>
        /// Gets the section by topic subtopic.	
        /// </summary>
        /// <param name="Topicnum">The topicnum.</param>
        /// <param name="Subtopicnum">The subtopicnum.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        SiteDs.Cod_MetaRow[] GetSectionByTopicSubtopic(string Topicnum, string Subtopicnum);
    }
}