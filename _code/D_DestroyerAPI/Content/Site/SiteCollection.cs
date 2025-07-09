#region

using System;
using System.Collections;
using System.Data;
using System.Reflection;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for SiteCollection.
    /// </summary>
    public class SiteCollection : DestroyerBpc, IEnumerable, ISiteCollection
    {
        #region Private Properties

        //these are used for construction
        private readonly IBook book;
        private readonly SiteBuildStatus buildStatus;
        private readonly bool includeArchived;
        private readonly SiteIndexBuildStatus indexBuildStatus;
        private readonly bool latestVersion;
        private readonly bool retrieveByBuildStatus;
        private readonly bool retrieveByIndexBuildStatus;
        private readonly bool retrieveBySiteStatus;
        private readonly SiteStatus siteStatus;

        /// <summary>
        ///   the site dalc used for database operations around sites
        /// </summary>
        private SiteDalc activeSiteDalc;

        /// <summary>
        ///   the site dataset that we use for most of our private storage
        /// </summary>
        private SiteDs activeSiteDs;

        private SiteDs.SiteRow[] sortedSiteRows;

        private SiteDalc ActiveSiteDalc
        {
            get { return activeSiteDalc ?? (activeSiteDalc = new SiteDalc()); }
        }

        private SiteDs ActiveSiteDs
        {
            get
            {
                if (activeSiteDs == null)
                {
                    if (book != null)
                    {
                        activeSiteDs = ActiveSiteDalc.GetSites(book.Id);
                    }
                    else if (retrieveBySiteStatus)
                    {
                        activeSiteDs = ActiveSiteDalc.GetSites(siteStatus);
                    }
                    else if (retrieveByBuildStatus)
                    {
                        activeSiteDs = ActiveSiteDalc.GetSites(buildStatus);
                    }
                    else if (retrieveByIndexBuildStatus)
                    {
                        activeSiteDs = ActiveSiteDalc.GetSites(indexBuildStatus);
                    }
                    else
                    {
                        activeSiteDs = ActiveSiteDalc.GetSites(latestVersion, includeArchived);
                    }
                }
                return activeSiteDs;
            }
        }

        private SiteDs.SiteRow[] SortedSiteRows
        {
            get { return sortedSiteRows ?? (sortedSiteRows = (SiteDs.SiteRow[]) ActiveSiteDs.Site.Select("", SortBy)); }
        }

        private string SortBy
        {
            get { return SortField + ((Ascending) ? " asc" : " desc"); }
        }

        #endregion Private Properties

        #region Constructors

        /// <summary>
        ///   Constructs a new site collection containing all sites that exist in the datastore
        /// </summary>
        /// <param name = "latestVersion"></param>
        /// <param name = "includeArchived"></param>
        public SiteCollection(bool latestVersion, bool includeArchived)
        {
            this.latestVersion = latestVersion;
            this.includeArchived = includeArchived;
        }

        /// <summary>
        ///   Constructs a new site collection containing all the sites that a book is associated with
        /// </summary>
        /// <param name = "book">The book object for which you wish to retrieve associated sites.</param>
        public SiteCollection(IBook book)
        {
            this.book = book;
        }


        /// <summary>
        ///   Constructs a new site collection containing all sites that have the specified site status
        /// </summary>
        /// <param name = "siteStatus">The site status to use for retrieving the site collection</param>
        public SiteCollection(SiteStatus siteStatus)
        {
            retrieveBySiteStatus = true;
            this.siteStatus = siteStatus;
        }

        /// <summary>
        ///   Constructs a new site collection containing all sites that have the specified site build status
        /// </summary>
        /// <param name = "siteBuildStatus">The site build status to use for retrieving the site collection</param>
        public SiteCollection(SiteBuildStatus siteBuildStatus)
        {
            retrieveByBuildStatus = true;
            buildStatus = siteBuildStatus;
        }

        /// <summary>
        ///   Constructs a new site collection containing all sites that have the specified site index build status
        /// </summary>
        /// <param name = "siteIndexBuildStatus">The site index build status to use for retrieving the site collection</param>
        public SiteCollection(SiteIndexBuildStatus siteIndexBuildStatus)
        {
            retrieveByIndexBuildStatus = true;
            indexBuildStatus = siteIndexBuildStatus;
        }

        #endregion Constructors

        private bool ascending = true;
        private SiteSortField sortField = SiteSortField.Title;

        #region IEnumerable Members

        /// <summary>
        ///   return an IEnumerator object for enumerating through the site collection
        /// </summary>
        /// <returns>Object implementing IEnumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return new SiteEnumerator(this);
        }

        #endregion

        #region ISiteCollection Members

        /// <summary>
        /// Gets or sets the sort field.	
        /// </summary>
        /// <value>The sort field.</value>
        /// <remarks></remarks>
        public SiteSortField SortField
        {
            get { return sortField; }
            set
            {
                sortedSiteRows = null;
                sortField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ascending.	
        /// </summary>
        /// <value>The ascending.</value>
        /// <remarks></remarks>
        public bool Ascending
        {
            get { return ascending; }
            set
            {
                sortedSiteRows = null;
                ascending = value;
            }
        }

        /// <summary>
        ///   Indexer for retrieving a site by ordinal value
        /// </summary>
        public ISite this[int index]
        {
            get
            {
                //return new Site(((SiteDs.SiteRow)this.ActiveSiteDs.Site.Rows[index]));
                return new Site((SortedSiteRows[index]));
            }
        }

        /// <summary>
        ///   Indexer for retrieving a book by name
        /// </summary>
        public ISite this[string name]
        {
            get
            {
                ISite retSite = null;
                //query our site table for a row containing the specified site name
                if (ActiveSiteDs.Site.Rows.Count > 0)
                {
                    DataRow[] drs = ActiveSiteDs.Site.Select(string.Format("Name = '{0}'", name));
                    if (drs.Length > 0)
                    {
                        SiteDs.SiteRow siteRow = (SiteDs.SiteRow) drs[0];
                        retSite = new Site(siteRow);
                    }
                }
                return retSite;
            }
        }

        ///<summary>
        ///  Get a site by the siteId
        ///</summary>
        public ISite GetSiteById(int siteId)
        {
            ISite retSite = null;
            //query our site table for a row containing the specified site id
            if (ActiveSiteDs.Site.Rows.Count > 0)
            {
                retSite = new Site(ActiveSiteDs.Site.FindBySiteId(siteId));
            }
            return retSite;
        }

        /// <summary>
        ///   The number of sites in the collection
        /// </summary>
        public int Count
        {
            get { return ActiveSiteDs.Site.Rows.Count; }
        }

        /// <summary>
        ///   Save the sites in the collection
        /// </summary>
        public void Save()
        {
            throw new Exception("The method '" + MethodBase.GetCurrentMethod().Name + "' is not yet implemented");
        }

        /// <summary>
        /// </summary>
        /// <param name = "site"></param>
        public void Add(ISite site)
        {
            //if we were not constructed with a site, then we should simply add the site to our private site dataset
            ActiveSiteDs.Site.AddSiteRow(site.Version, site.Title, site.Description, (int) site.RequestedStatus,
                                         (int) site.Status, site.SearchUri, site.Online, site.Name, site.Archived,
                                         (int) site.BuildStatus, (int) site.IndexBuildStatus, site.SiteXml);
        }

        #endregion

        #region Nested type: SiteEnumerator

        /// <summary>
        ///   The BookEnumerator is a class that manages enumeration of a collection of books
        /// </summary>
        private class SiteEnumerator : IEnumerator
        {
            private readonly SiteCollection sc;
            private int index;

            /// <summary>
            ///   The constructor of our site enumerator.
            /// </summary>
            /// <param name = "SiteColl">The collection of sites to enumerate</param>
            public SiteEnumerator(SiteCollection SiteColl)
            {
                sc = SiteColl;
                Reset();
            }

            #region IEnumerator Members

            /// <summary>
            ///   Reset our index
            /// </summary>
            public void Reset()
            {
                index = -1;
            }

            /// <summary>
            ///   Return the book row at the current index
            /// </summary>
            public object Current
            {
                get
                {
                    //return new Site(((SiteDs.SiteRow)sc.ActiveSiteDs.Site.Rows[index]));
                    return new Site((sc.SortedSiteRows[index]));
                }
            }

            /// <summary>
            ///   Advance our index
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                index++;
                return (index < sc.ActiveSiteDs.Site.Rows.Count);
            }

            #endregion
        }

        #endregion
    }
}