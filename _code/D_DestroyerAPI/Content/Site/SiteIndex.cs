#region

using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Content.Subscription;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using Endeca.Navigation;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   The SiteIndex object is used to build and search indexes of a site.
    /// </summary>
    public class SiteIndex : DestroyerBpc, ISiteIndex
    {
        #region Constants

        //errors
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_SITEINDEXSEARCHUNAVAILABLEOFFLINE =
            "The index for site '{0}' ('{1}') is not online. You must set the site index online before performing a search.";

        public static string ERROR_SITEINDEXSEARCHUNAVAILABLEINCOMPATIBLESTATUS =
            "The index for site '{0}' ('{1}') is not available because of the site's current status of '{2}' does not support searching. You must change the site status before accessing this index through a search.";

        public static string ERROR_SITEINDEXSEARCHUNAVAILABLEINDEXING =
            "The search for this site is currently unavailable, we apologize for the inconvenience, please try again later. Site status is '{2}'.";

        public static string ERROR_SITEINDEXSEARCHNODATAINDEXINGSERVICEURL =
            "The index for site '{0}' ('{1}') does not have a data indexing service URL specified for the site's current status of '{2}'.";

        //public static string ERROR_ENDECAPROJECTFOLDERNOTFOUND = "The Endeca project folder '{0}', as specified in the application configuration file as the site index project folder for '{1}', was not found.";
        public static string ERROR_ENDECAPROJECTNOTVALID =
            "The Endeca project folder '{0}' does not contain a valid Endeca project.";

        public static string ERROR_SITEBUILDSITENOTSAVED =
            "Error while building Site Index for site '{0}' ('{1}'). You must save a site before building its index.";

        public static string ERROR_SITEBUILDSOURCESITESTATUSNOTSUPPORTED =
            "Error while building Site Index for site '{0}' ('{1}'). The source site status '{2}' is not supported.";

        public static string ERROR_SITEBUILDDESTSITESTATUSNOTSUPPORTED =
            "Error while building Site Index for site '{0}' ('{1}'). The destination site status '{2}' is not supported (you may only build a site to Staging or Production).";

        public static string ERROR_SITEBUILDPIPELINENOTFOUND =
            "Error while building Site Index for site '{0}' ('{1}'). The Endeca pipeline file '{2}' was not found. Please check your application configuration file.";

        public static string ERROR_SITEBUILDINCORRECTSTATUS =
            "Error while building Site Index for site '{0}' ('{1}'). You may not build and index when the site has a status of '{0}'.";

        public static string ERROR_SITEINDEXDATAINDEXINGSERVICEFAILURE =
            "Error while building Site Index for site '{0}' ('{1}'). There was an error performing the site index update. The following error was returned:\n{2}";

        public static string ERROR_ADDINDEXINGMETADATAHEADNOTFOUND =
            "Head tag not found for file '{0}'. Metadata cannot be added.";

        public static string ERROR_SITEINDEXBUILDUNSUPPORTEDSTATUSCHANGE =
            "Error building site index for site '{0}' (id={1}). The site status change of '{2}' to '{3}' is not supported.";

        public static string ERROR_SITEINDEXBUILDUEXPECTEDSTATUSCHANGE =
            "Error building site index for site '{0}' (id={1}). The site status change of '{2}' to '{3}' is not expected.";

        //config keys
        public static string SITEINDEX_SITEXMLFILENAME_KEY = "SiteIndex_SiteXmlFilename";
        public static string SITEINDEX_ENDECATEMPLATEPROJECTFOLDER_KEY = "SiteIndex_EndecaTemplateProjectFolder";
        public static string SITEINDEX_ENDECASTAGINGPROJECTFOLDER_KEY = "SiteIndex_EndecaStagingProjectFolder";
        public static string SITEINDEX_ENDECAPRODUCTIONPROJECTFOLDER_KEY = "SiteIndex_EndecaProductionProjectFolder";
        public static string SITEINDEX_ENDECASTAGINGHOSTNAME_KEY = "SiteIndex_EndecaStagingHostname";
        public static string SITEINDEX_ENDECASTAGINGPORT_KEY = "SiteIndex_EndecaStagingPort";
        public static string SITEINDEX_ENDECAPRODUCTIONHOSTNAME_KEY = "SiteIndex_EndecaProductionHostname";
        public static string SITEINDEX_ENDECAPRODUCTIONPORT_KEY = "SiteIndex_EndecaProductionPort";

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEURL_KEY =
            "SiteIndex_EndecaStagingDataIndexingServiceUrl";

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEURL_KEY =
            "SiteIndex_EndecaProductionDataIndexingServiceUrl";

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEUSERNAME_KEY =
            "SiteIndex_EndecaProductionDataIndexingServiceUsername";

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEUSERNAME_KEY =
            "SiteIndex_EndecaStagingDataIndexingServiceUsername";

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEPASSWORD_KEY =
            "SiteIndex_EndecaProductionDataIndexingServicePassword";

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEPASSWORD_KEY =
            "SiteIndex_EndecaStagingDataIndexingServicePassword";

        public static string SITEINDEX_ENDECAPIPELINERELATIVEPATH_KEY = "SiteIndex_EndecaPipelineRelativePath";

        //config values
        public static string SITEINDEX_SITEXMLFILENAME =
            ConfigurationSettings.AppSettings[SITEINDEX_SITEXMLFILENAME_KEY];

        public static string SITEINDEX_ENDECATEMPLATEPROJECTFOLDER =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECATEMPLATEPROJECTFOLDER_KEY];

        public static string SITEINDEX_ENDECASTAGINGPROJECTFOLDER =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGPROJECTFOLDER_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONPROJECTFOLDER =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONPROJECTFOLDER_KEY];

        public static string SITEINDEX_ENDECASTAGINGHOSTNAME =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGHOSTNAME_KEY];

        public static string SITEINDEX_ENDECASTAGINGPORT =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGPORT_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONHOSTNAME =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONHOSTNAME_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONPORT =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONPORT_KEY];

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEURL =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEURL_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEURL =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEURL_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEUSERNAME =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEUSERNAME_KEY];

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEUSERNAME =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEUSERNAME_KEY];

        public static string SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEPASSWORD =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEPASSWORD_KEY];

        public static string SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEPASSWORD =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEPASSWORD_KEY];

        public static string SITEINDEX_ENDECAPIPELINERELATIVEPATH =
            ConfigurationSettings.AppSettings[SITEINDEX_ENDECAPIPELINERELATIVEPATH_KEY];

        //other site index specific values
        public static string SITEINDEX_CONTENTSUBFOLDERNAME = "content";
        public static string SITEINDEX_ENDECADIMENSION_SITEHIERARCHY = "destroyer_site_hierarchy";
        public static string SITEINDEX_ENDECAPROPERTY_BOOKID = "destroyer_book_id";
        public static string SITEINDEX_ENDECAPROPERTY_BOOKNAME = "destroyer_book_name";
        public static string SITEINDEX_ENDECAPROPERTY_DOCUMENTID = "destroyer_document_id";
        public static string SITEINDEX_ENDECAPROPERTY_DOCUMENTNAME = "destroyer_document_name";
        public static string SITEINDEX_ENDECAPROPERTY_SUBSCRIPTIONCODE = "destroyer_subscription_code";
        public static string SITEINDEX_BACKUPSUFFIX = "_autobackup";

        #endregion Constants

        #region Private Fields

        private readonly ISite site;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        ///   Construct a site index for the specified site
        /// </summary>
        /// <param name = "site">The site for which the index is intended.</param>
        public SiteIndex(ISite site)
        {
            // store a reference to the site object to which this belongs
            this.site = site;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   The site that is using this SiteIndex
        /// </summary>
        public ISite Site
        {
            get { return site; }
        }

        /// <summary>
        ///   The online status of the index
        /// </summary>
        public bool Online
        {
            get { return true; }
            set { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   Gets the status of the site index
        /// </summary>
        public SiteIndexStatus Status
        {
            get
            {
                //default to error
                SiteIndexStatus retStatus = SiteIndexStatus.Error;

                //get the site index build status from endeca
                SiteStatus siteStatus = (Site.BuildStatus == SiteBuildStatus.Built &&
                                         Site.IndexBuildStatus == SiteIndexBuildStatus.Built)
                                            ? Site.Status
                                            : Site.RequestedStatus;
                string endecaIndexStatus = GetEndecaDataIndexingService(siteStatus).getStatus().systemState;

                //...if endeca says updating...
                switch (endecaIndexStatus)
                {
                    case ENDECA_UPDATING_SYSTEMSTATE:
                        retStatus = SiteIndexStatus.Updating;
                        break;
                    case ENDECA_IDLE_SYSTEMSTATE:
                        retStatus = SiteIndexStatus.Ready;
                        break;
                    case ENDECA_ERROR_SYSTEMSTATE:
                        retStatus = SiteIndexStatus.Error;
                        break;
                }

                return retStatus;
            }
        }

        /// <summary>
        ///   Gets a list of site indexing error string
        /// </summary>
        public string[] BuildErrors
        {
            get
            {
                string[] retBuildErrors = null;
                if (Status == SiteIndexStatus.Error)
                {
                    ArrayList buildErrors = new ArrayList();

                    Status dataIndexStatus = GetEndecaDataIndexingService(Site.RequestedStatus).getStatus();
                    SystemError[] indexingErrors = dataIndexStatus.systemErrors;

                    foreach (string indexingErrorString in
                        indexingErrors.Select(
                            indexingError =>
                            string.Format("Severity: {0}; Component: {1}; RecordSpec: {2}; Error Message: {3}",
                                          indexingError.severity, indexingError.component, indexingError.recordSpec,
                                          indexingError.errorMsg)))
                    {
                        buildErrors.Add(indexingErrorString);
                    }
                    retBuildErrors = (string[]) buildErrors.ToArray(typeof (string));
                }
                return retBuildErrors;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///   Initiates indexing of the site. The site indexing process prepares the site into an indexable format and then initiates endeca indexing.
        /// </summary>
        public void Build()
        {
            try
            {
                //don't let a site build occur unless the site has been saved
                if (Site.HasChanges)
                {
                    throw new Exception(string.Format(ERROR_SITEBUILDSITENOTSAVED, Site.Name, Site.Title));
                }

                //get our statuses, both current and requested
                SiteStatus currentStatus = site.Status;
                SiteStatus requestedStatus = site.RequestedStatus;

                //handle each permutation of site status changes
                if (currentStatus == SiteStatus.Unassigned && requestedStatus == SiteStatus.Production)
                {
                    throw new Exception(string.Format(ERROR_SITEINDEXBUILDUNSUPPORTEDSTATUSCHANGE, site.Name, site.Id,
                                                      currentStatus, requestedStatus));
                }
                else if (currentStatus == SiteStatus.Unassigned && requestedStatus == SiteStatus.Unassigned)
                {
                    //there is nothing we do here since unassigned has no index
                }
                else if (currentStatus == SiteStatus.Staging && requestedStatus == SiteStatus.Unassigned)
                {
                    //there is nothing we do here since unassigned has no index
                }
                else if (currentStatus == SiteStatus.Production && requestedStatus == SiteStatus.Unassigned)
                {
                    //there is nothing we do here since unassigned has no index
                }
                else if (currentStatus == SiteStatus.Production && requestedStatus == SiteStatus.Staging)
                {
                    //copy template project over
                    PromoteEndecaProject(SiteStatus.Unassigned, SiteStatus.Staging);

                    //copy in content to staging
                    PopulateEndecaProject(SiteStatus.Staging);

                    //do baseline update
                    //DoEndecaBaselineUpdate(SiteStatus.Staging);
                }
                else if (currentStatus == SiteStatus.Unassigned && requestedStatus == SiteStatus.Staging)
                {
                    //copy template project over
                    PromoteEndecaProject(SiteStatus.Unassigned, SiteStatus.Staging);

                    //copy in content
                    PopulateEndecaProject(SiteStatus.Staging);

                    //do baseline update
                    //DoEndecaBaselineUpdate(SiteStatus.Staging);
                }
                else if (currentStatus == SiteStatus.Staging && requestedStatus == SiteStatus.Staging)
                {
                    //copy template project over
                    PromoteEndecaProject(SiteStatus.Unassigned, SiteStatus.Staging);

                    //copy in content
                    PopulateEndecaProject(SiteStatus.Staging);

                    //do baseline update
                    //DoEndecaBaselineUpdate(SiteStatus.Staging);
                }
                else if (currentStatus == SiteStatus.Staging && requestedStatus == SiteStatus.Production)
                {
                    //copy staging project over production project
                    PromoteEndecaProject(SiteStatus.Staging, SiteStatus.Production);

                    //copy in content
                    PopulateEndecaProject(SiteStatus.Production);

                    //do baseline update
                    //DoEndecaBaselineUpdate(SiteStatus.Production);
                }
                else if (currentStatus == SiteStatus.Production && requestedStatus == SiteStatus.Production)
                {
                    //copy staging project over production project
                    PromoteEndecaProject(currentStatus, requestedStatus);

                    //copy in content
                    PopulateEndecaProject(SiteStatus.Production);

                    //do baseline update
                    //DoEndecaBaselineUpdate(requestedStatus);
                }
                else
                {
                    throw new Exception(string.Format(ERROR_SITEINDEXBUILDUEXPECTEDSTATUSCHANGE, site.Name, site.Id,
                                                      currentStatus, requestedStatus));
                }

                //this is the last step -- getting here means that no exceptions were thrown
                if (Site.IndexBuildStatus != SiteIndexBuildStatus.BuiltWithWarnings)
                {
                    Site.IndexBuildStatus = SiteIndexBuildStatus.Built;
                }
                Site.Status = Site.RequestedStatus;
                Site.RequestedStatus = SiteStatus.Unassigned;
                Site.Save();
            }
            catch (Exception e)
            {
                //If we get here, something went wrong with the build. Log the problem and set the BuildStatus to Error.
                //log the error
                Event siteIndexEvent = new Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_SITEINDEXBUILD_ERROR,
                                                 MODULE_SITEINDEX, METHOD_SITEINDEXBUILD, Site.Id.ToString(), e.Message);
                siteIndexEvent.Save(false);
                //set our Error status and save the site
                Site.IndexBuildStatus = SiteIndexBuildStatus.Error;
                Site.Save();
            }
        }

        /// <summary>
        ///   Searches the site index and returns the results.
        /// </summary>
        /// <param name = "searchCriteria">An object representing the search criteria</param>
        /// <returns>An object from which you can retrieve search results</returns>
        public ISearchResults Search(ISearchCriteria searchCriteria)
        {
            //make sure our index is online
            if (!Online)
            {
                throw new Exception(string.Format(ERROR_SITEINDEXSEARCHUNAVAILABLEOFFLINE, Site.Name, Site.Title));
            }

            //make sure our site status supports search
            if (!(Site.Status == SiteStatus.Staging || Site.Status == SiteStatus.Production))
            {
                throw new Exception(string.Format(ERROR_SITEINDEXSEARCHUNAVAILABLEINCOMPATIBLESTATUS, Site.Name,
                                                  Site.Title, Site.Status));
            }

            /********* MKM 04/15/10: Can't (and don't need to) call Endeca indexing web service for status *********/

            ////make sure that our site index is not in an update or an error state
            //SiteIndexStatus siteIndexStatus = this.Status;
            //if(siteIndexStatus != SiteIndexStatus.Ready)
            //{
            //    throw new Exception(string.Format(ERROR_SITEINDEXSEARCHUNAVAILABLEINDEXING, this.Site.Name, this.Site.Title, siteIndexStatus));
            //}

            /*******************************************************************************************************/

            string endecaHostname = GetSearchUrlHostname(Site.Status);
            string endecaPort = GetSearchUrlPort(Site.Status);

            //create an endeca connection
            HttpENEConnection endecaConnection = new HttpENEConnection(endecaHostname, endecaPort);

            //construct an endeca query based on the specified search criteria object
            ENEQuery endecaQuery = new ENEQuery();

            //must set this in order for dimension values to come thru without a separate request
            endecaQuery.NavAllRefinements = true;

            //we always want to activate DYM functionality
            endecaQuery.NavERecSearchDidYouMean = true;

            //add the dimensions to the query object
            string dimensionIdList = ENDECA_DIMENSIONID_INITIAL;
            for (int i = 0; i < searchCriteria.DimensionIds.Length; i++)
            {
                dimensionIdList += searchCriteria.DimensionIds[i];
                if (i != searchCriteria.DimensionIds.Length - 1)
                {
                    dimensionIdList += ENDECA_DIMENSIONIDSEPCHAR;
                }
            }
            endecaQuery.NavDescriptors = new DimValIdList(dimensionIdList);

            //set our record search option string
            string opts = EMPTY_STRING;
            switch (searchCriteria.SearchType)
            {
                case SearchType.AnyWords:
                    opts = ENDECA_SEARCHOPT_MATCHANY;
                    break;
                case SearchType.Boolean:
                    opts = ENDECA_SEARCHOPT_MATCHBOOLEAN;
                    break;
                default:
                    opts = ENDECA_SEARCHOPT_MATCHALL;
                    break;
            }

            //set our keywords string (put quotes around the keywords ExactPhrase and if they are not already there)
            string keywords = searchCriteria.Keywords;
            if (searchCriteria.SearchType == SearchType.ExactPhrase &&
                (keywords.IndexOf("\"") != 0 && keywords.LastIndexOf("\"") != keywords.Length))
            {
                keywords = "\"" + keywords + "\"";
            }

            //request snippeting from endeca if requested in the search criteria
            if (searchCriteria.Excerpts)
            {
                opts += " snip " + ENDECA_FULLTEXT_FIELD + ":30";
            }

            //add our keywords and options to the record search list
            ERecSearchList erecSearchList = new ERecSearchList();

            //a variable for keeping track of how many search criteria we have added to the search list
            int searchListIndex = 0;

            //if selected, filter out documents belonging to books to which the user is not subscribed
            if (Site.User != null)
            {
                string subscriptionList = string.Empty;
                if (searchCriteria.FilterUnsubscribed)
                {
                    string[] domainItems = Site.User.UserSecurity.Domain.Split(DOMAIN_SUBSCRIPTIONCODESEPCHAR);
                    subscriptionList = domainItems.Aggregate(subscriptionList,
                                                             (current, domainItem) =>
                                                             current + (NormalizeSearchFilterValue(domainItem) + " "));
                    erecSearchList.Add(searchListIndex++,
                                       new ERecSearch(ENDECA_SUBSCRIPTIONCODE_FIELD, subscriptionList,
                                                      ENDECA_SEARCHOPT_MATCHANY));
                }
            }

            //filter out documents for which the crawl did not succeed (get rid of, for example, document references)
            erecSearchList.Add(searchListIndex++,
                               new ERecSearch(ENDECA_DOCUMENTSTATUS_PROPERTY, ENDECA_DOCUMENTSTATUSSUCCESS_KEYWORD,
                                              ENDECA_SEARCHOPT_MATCHALL));

            //add the keywords
            if (keywords != EMPTY_STRING)
            {
                erecSearchList.Add(searchListIndex++, new ERecSearch(ENDECA_FULLTEXT_FIELD, keywords, opts));
            }

            //Add additional properties
            foreach (string key in searchCriteria.SearchParameters.Keys)
            {
                string val = searchCriteria.SearchParameters[key];
                erecSearchList.Add(searchListIndex++,new ERecSearch(key, val));
            }

            //set the search list to the endeca query object
            endecaQuery.NavERecSearches = erecSearchList;

            //add our offset
            endecaQuery.NavERecsOffset = searchCriteria.PageOffset;

            //perform the query
            ENEQueryResults endecaQueryResults = endecaConnection.Query(endecaQuery);

            //create a search results object based on our endeca search results
            SearchResults searchResults = new SearchResults(Site, searchCriteria, endecaQueryResults);

            //return our search results
            return searchResults;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name = "currentStatus"></param>
        /// <param name = "requestedStatus"></param>
        private void PromoteEndecaProject(SiteStatus currentStatus, SiteStatus requestedStatus)
        {
            //initialize our project source and destination locations
            string sourceProjectFolder = GetSourceIndexingProjectFolder(currentStatus);
            string destProjectFolder = GetDestinationIndexingProjectFolder(requestedStatus);

            //verify that our source folder contains a valid project
            if (!Directory.Exists(sourceProjectFolder) || !EndecaProjectIsValid(sourceProjectFolder))
            {
                throw new Exception(string.Format(ERROR_ENDECAPROJECTNOTVALID, sourceProjectFolder));
            }

            //verify that our destination folder exists and is empty
            if (Directory.Exists(destProjectFolder))
            {
                Directory.Delete(destProjectFolder, true);
            }
            Directory.CreateDirectory(destProjectFolder);

            //copy the source project folder to the destination project folder
            CopyDirectory(sourceProjectFolder, destProjectFolder);

            //make sure the endeca project reflects the new location
            string contentIndexingFolder = Path.Combine(destProjectFolder, SITEINDEX_CONTENTSUBFOLDERNAME);
            string endecaPipelinePath = Path.Combine(destProjectFolder, SITEINDEX_ENDECAPIPELINERELATIVEPATH);
            ModifyProjectPipeline(endecaPipelinePath, contentIndexingFolder);
        }

        /// <summary>
        /// </summary>
        /// <param name = "requestedStatus"></param>
        private void PopulateEndecaProject(SiteStatus requestedStatus)
        {
            //initialize our destination location
            string destProjectFolder = GetDestinationIndexingProjectFolder(requestedStatus);

            //grab our site xml and write it out to a file
            string siteXmlFile = Path.Combine(destProjectFolder, SITEINDEX_SITEXMLFILENAME);
            XmlDocument siteXml = new XmlDocument();
            siteXml.LoadXml(Site.SiteBookXml);
            siteXml.Save(siteXmlFile);

            //	iterate through all site books
            IBookCollection books = Site.Books;
            foreach (IBook book in books)
            {
                try
                {
                    //	build a folder for each book corresponding to its name
                    string contentIndexingFolder = Path.Combine(destProjectFolder, SITEINDEX_CONTENTSUBFOLDERNAME);
                    string bookFolder = Path.Combine(contentIndexingFolder, book.Name);
                    if (Directory.Exists(bookFolder))
                    {
                        Directory.Delete(bookFolder, true);
                    }
                    Directory.CreateDirectory(bookFolder);

                    //	iterate through all book documents
                    IDocumentCollection docs = book.Documents;
                    foreach (IDocument doc in docs)
                    {
                        try
                        {
                            //	copy the primary format for the document (if it exists) into our book folder
                            IDocumentFormat primaryDocFormat = doc.PrimaryFormat;
                            if (primaryDocFormat != null)
                            {
                                string primaryDocFormatUri = primaryDocFormat.Uri;
                                string primaryDocFormatFilename = new FileInfo(primaryDocFormatUri).Name;
                                string primaryDocFormatContentTypeDesc = primaryDocFormat.Description;
                                ContentType primaryDocFormatContentType =
                                    GetContentTypeFromDescription(primaryDocFormatContentTypeDesc);
                                string destFilename = Path.Combine(bookFolder, primaryDocFormatFilename);

                                //be sure that the uri points to an existing resource before continuing
                                if (!File.Exists(primaryDocFormatUri))
                                {
                                    throw new FileNotFoundException(
                                        "Document Format file not found while building site. The file '" +
                                        primaryDocFormatUri + "', referenced in document '" + doc.Name + "' of book '" +
                                        book.Name + "', does not exist.");
                                }

                                //add endeca metadata to the file and copy it into the proper location
                                string destFileWithMetadata = AddIndexingMetadata(primaryDocFormatUri,
                                                                                  primaryDocFormatContentType, book.Id,
                                                                                  book.Name, doc.Id, doc.Name, siteXml);
                                if (destFileWithMetadata != EMPTY_STRING)
                                {
                                    File.Copy(destFileWithMetadata, destFilename);
                                    File.Delete(destFileWithMetadata);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Event siteIndexEvent = new Event(EventType.Error, DateTime.Now,
                                                             ERROR_SEVERITY_SITEINDEXBUILD_ERROR, MODULE_SITEINDEX,
                                                             METHOD_SITEINDEXBUILD, Site.Id.ToString(), e.Message);
                            siteIndexEvent.Save(false);
                            //set our Error status and save the site
                            Site.IndexBuildStatus = SiteIndexBuildStatus.BuiltWithWarnings;
                        }
                    }
                } //End Try
                catch (Exception e)
                {
                    Event siteIndexEvent = new Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_SITEINDEXBUILD_ERROR,
                                                     MODULE_SITEINDEX, METHOD_SITEINDEXBUILD, Site.Id.ToString(),
                                                     e.Message);
                    siteIndexEvent.Save(false);
                    //set our Error status and save the site
                    Site.IndexBuildStatus = SiteIndexBuildStatus.BuiltWithWarnings;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "siteStatus"></param>
        /// <returns></returns>
        [Obsolete("Calls to the Endeca DataIndexing web service are no longer supported!")]
        private DataIndexingService GetEndecaDataIndexingService(SiteStatus siteStatus)
        {
            throw new NotSupportedException("Calls to the Endeca DataIndexing web service are no longer supported!");

            //DataIndexingService dis = new DataIndexingService();
            //string dataIndexingUrl = GetDataIndexingServiceUrl(siteStatus);
            //if(dataIndexingUrl != EMPTY_STRING)
            //{
            //    dis.Url = dataIndexingUrl;
            //    dis.PreAuthenticate = true;
            //    dis.Credentials = new System.Net.NetworkCredential (GetDataIndexingServiceUsername(siteStatus), GetDataIndexingServicePassword(siteStatus));
            //}
            //else
            //{
            //    throw new Exception(string.Format(ERROR_SITEINDEXSEARCHNODATAINDEXINGSERVICEURL, this.Site.Name, this.Site.Title, siteStatus));
            //}
            //return dis;
        }

        /// <summary>
        ///   Helper method for performing endeca baseline update
        /// </summary>
        private void DoEndecaBaselineUpdate(SiteStatus siteStatus)
        {
            //perform an Endeca baseline update
            try
            {
                GetEndecaDataIndexingService(siteStatus).startBaselineUpdate();
                while (Status == SiteIndexStatus.Updating)
                {
                    //create a 15. sec sleep time between status checks;
                    //we don't want to make this too short because it will slow down the building and
                    //not give enough time for the service to return a response
                    Thread.Sleep(15000);
                }
            }
            catch (SoapException e)
            {
                throw new Exception(string.Format(ERROR_SITEINDEXDATAINDEXINGSERVICEFAILURE, Site.Name, Site.Title,
                                                  e.Detail.InnerXml));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "sourceProjectFolder"></param>
        /// <returns></returns>
        private bool EndecaProjectIsValid(string sourceProjectFolder)
        {
            //init to invalid project
            bool retVal = false;

            //make sure the project file exists
            retVal = File.Exists(Path.Combine(sourceProjectFolder, "data\\forge_input\\Pipeline.epx"));

            //return the status of the project
            return retVal;
        }

        /// <summary>
        ///   Adjusts an endeca project pipeline according to the specified parameters
        /// </summary>
        /// <param name = "endecaPipelinePath">The endeca pipeline file to modify</param>
        /// <param name = "endecaContentIndexingFolder">The location of the content indexing folder the pipeline is to spider</param>
        private void ModifyProjectPipeline(string endecaPipelinePath, string endecaContentIndexingFolder)
        {
            if (!File.Exists(endecaPipelinePath))
            {
                throw new Exception(string.Format(ERROR_SITEBUILDPIPELINENOTFOUND, Site.Name, Site.Title,
                                                  endecaPipelinePath));
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(endecaPipelinePath);

            if (endecaContentIndexingFolder != null && endecaContentIndexingFolder != EMPTY_STRING)
            {
                XmlNode rootUrlNode = xmlDoc.SelectSingleNode(ENDECA_PIPELINESPIDERURLXPATH);
                rootUrlNode.InnerText = URI_FILEPROTOCOLPREFIX + endecaContentIndexingFolder;
            }
            xmlDoc.Save(endecaPipelinePath);
        }

        /// <summary>
        ///   Returns the appropriate destination indexing project folder based on the specified site status.
        /// </summary>
        /// <param name = "siteStatus">The site status to use for determining the destination indexing project folder.</param>
        /// <returns>A string indicating the appropriate destination project folder.</returns>
        private string GetDestinationIndexingProjectFolder(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGPROJECTFOLDER;
                    break;
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONPROJECTFOLDER;
                    break;
                default:
                    throw new Exception(string.Format(ERROR_SITEBUILDDESTSITESTATUSNOTSUPPORTED, Site.Name, Site.Title,
                                                      siteStatus));
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the appropriate source indexing project folder based on the specified site status.
        /// </summary>
        /// <param name = "siteStatus">The site status to use for determining the source indexing project folder.</param>
        /// <returns>A string indicating the appropriate source project folder.</returns>
        private string GetSourceIndexingProjectFolder(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Unassigned:
                    retVal = SITEINDEX_ENDECATEMPLATEPROJECTFOLDER;
                    break;
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGPROJECTFOLDER;
                    break;
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONPROJECTFOLDER;
                    break;
                default:
                    throw new Exception(string.Format(ERROR_SITEBUILDSOURCESITESTATUSNOTSUPPORTED, Site.Name, Site.Title,
                                                      siteStatus));
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the endeca hostname for the site
        /// </summary>
        /// <returns>A string indicating the hostname for the search index</returns>
        private string GetSearchUrlHostname(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGHOSTNAME;
                    break;
                case SiteStatus.PreProduction:
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONHOSTNAME;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the endeca port for the site
        /// </summary>
        /// <returns>A string indicating the port for the search index</returns>
        private string GetSearchUrlPort(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGPORT;
                    break;
                case SiteStatus.PreProduction:
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONPORT;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the appropriate url to the endeca dataindexing web service
        /// </summary>
        /// <returns>A string indicating the url to the endeca data indexing web service</returns>
        private string GetDataIndexingServiceUrl(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEURL;
                    break;
                case SiteStatus.PreProduction:
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEURL;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the appropriate url to the endeca dataindexing web service
        /// </summary>
        /// <returns>A string indicating the url to the endeca data indexing web service</returns>
        private string GetDataIndexingServiceUsername(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEUSERNAME;
                    break;
                case SiteStatus.PreProduction:
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEUSERNAME;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Returns the appropriate url to the endeca dataindexing web service
        /// </summary>
        /// <returns>A string indicating the url to the endeca data indexing web service</returns>
        private string GetDataIndexingServicePassword(SiteStatus siteStatus)
        {
            string retVal = EMPTY_STRING;
            switch (siteStatus)
            {
                case SiteStatus.Staging:
                    retVal = SITEINDEX_ENDECASTAGINGDATAINDEXINGSERVICEPASSWORD;
                    break;
                case SiteStatus.PreProduction:
                case SiteStatus.Production:
                    retVal = SITEINDEX_ENDECAPRODUCTIONDATAINDEXINGSERVICEPASSWORD;
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///   Adds metadata to the specified file so it can be indexed properly by the indexing engine
        /// </summary>
        /// <param name = "filename">The file to which metadata is to be added</param>
        /// <param name = "contentType">The content type of the file</param>
        private string AddIndexingMetadata(string filename, ContentType contentType, int bookId, string bookName,
                                           int docId, string docName, XmlDocument siteXml)
        {
            string retFilename = EMPTY_STRING;
            switch (contentType)
            {
                case ContentType.TextHtml:
                    //open up our source for reading
                    StreamReader sr = new StreamReader(filename, Encoding.UTF8);

                    //create a temp file to hold our modified html
                    retFilename = Path.GetTempFileName();
                    StreamWriter sw = new StreamWriter(retFilename, false, Encoding.UTF8);

                    //read through our source html
                    bool foundHead = false;
                    string line = EMPTY_STRING;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //add the metadata under the head tag
                        string newLine = line;
                        Match m = Regex.Match(newLine, "^.*<head[^>]*>", RegexOptions.IgnoreCase);
                        if (m.Success && !foundHead)
                        {
                            //use this flag to see if the file ever had a head tag; if the head was not found there is a big problem because the file will not get indexed properly
                            foundHead = true;

                            //add the head tag to the string
                            newLine = m.Groups[0].Value;

                            //write out our book id
                            newLine += "\r\n<meta content=\"" + bookId + "\" name=\"" + SITEINDEX_ENDECAPROPERTY_BOOKID +
                                       "\">";

                            //write out our book name
                            newLine += "\r\n<meta content=\"" + bookName + "\" name=\"" +
                                       SITEINDEX_ENDECAPROPERTY_BOOKNAME + "\">";

                            //write out our document id
                            newLine += "\r\n<meta content=\"" + docId + "\" name=\"" +
                                       SITEINDEX_ENDECAPROPERTY_DOCUMENTID + "\">";

                            //write out our document name
                            newLine += "\r\n<meta content=\"" + docName + "\" name=\"" +
                                       SITEINDEX_ENDECAPROPERTY_DOCUMENTNAME + "\">";

                            //write out the subscriptions to which we belong
                            ISubscriptionCollection subscriptions = new SubscriptionCollection(bookName);
                            newLine = subscriptions.Cast<ISubscription>().Aggregate(newLine,
                                                                                    (current, subscription) =>
                                                                                    current +
                                                                                    ("\r\n<meta content=\"" +
                                                                                     NormalizeSearchFilterValue(
                                                                                         subscription.Code) +
                                                                                     "\" name=\"" +
                                                                                     SITEINDEX_ENDECAPROPERTY_SUBSCRIPTIONCODE +
                                                                                     "\">"));

                            //find the ancestor nodes to the current document
                            XmlNodeList nodes =
                                siteXml.SelectNodes("//" + XML_ELE_DOCUMENT + "[@" + XML_ATT_DOCUMENTID + "='" + docId +
                                                    "']/ancestor-or-self::*[local-name(.)='" + XML_ELE_SITE +
                                                    "' or local-name(.)='" + XML_ELE_SITEFOLDER + "' or local-name(.)='" +
                                                    XML_ELE_BOOK + "' or local-name(.)='" + XML_ELE_BOOKFOLDER +
                                                    "' or local-name(.)='" + XML_ELE_DOCUMENT + "']");
                            newLine = (from XmlNode node in nodes
                                       select node.LocalName + ":" + node.Attributes[XML_ATT_ID].Value).Aggregate(
                                           newLine,
                                           (current, nodeId) =>
                                           current +
                                           ("\r\n<meta content=\"" + nodeId + "\" name=\"" +
                                            SITEINDEX_ENDECADIMENSION_SITEHIERARCHY + "\">"));
                        }

                        //write to our temp file
                        sw.WriteLine(newLine);
                    }

                    //throw an exception if the head tag was never found
                    if (!foundHead)
                    {
                        throw new Exception(string.Format(ERROR_ADDINDEXINGMETADATAHEADNOTFOUND, filename));
                    }

                    //close our files
                    sw.Close();
                    sr.Close();

                    break;
                default:
                    //some content types are not supported for indexing, so dont do anything
                    break;
            }

            //return the resulting filename
            return retFilename;
        }

        /// <summary>
        ///   Cleans characters out of a string that will be used for filtering. Certain characters, like
        ///   hyphens and semicolons, should be replaced in the fielded text with a character that
        ///   does not split the text into multiple terms
        /// </summary>
        /// <param name = "text">The text to normalize</param>
        /// <returns>A normalized version of the string that is suitable for indexing and searching.</returns>
        private string NormalizeSearchFilterValue(string text)
        {
            string retText = text;
            retText = Regex.Replace(retText, "[^A-Za-z]", "");
            return retText;
        }

        #endregion Private Methods
    }
}