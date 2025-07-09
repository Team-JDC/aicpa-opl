#region

using System.Collections;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for SiteTocNodeCollection.
    /// </summary>
    public class SiteTocNodeCollection : DestroyerBpc, ISiteTocNodeCollection, IEnumerable
    {
        private readonly int siteId = -1;
        private ISiteDalc activeSiteDalc;
        private SiteTocNodeDs.SiteTocNodeDataTable activeSiteTocNodeTable;
        private ISite site;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteTocNodeCollection" /> class.	
        /// </summary>
        /// <param name="site">The site.</param>
        /// <remarks></remarks>
        public SiteTocNodeCollection(ISite site)
        {
            this.site = site;
            siteId = site.Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteTocNodeCollection" /> class.	
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="nodeId">The node id.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <remarks></remarks>
        public SiteTocNodeCollection(int siteId, int nodeId, NodeType nodeType)
        {
            this.siteId = siteId;
            activeSiteTocNodeTable = ActiveSiteDalc.GetChildSiteTocNodes(nodeId, nodeType);
        }

        /// <summary>
        /// Gets the active site dalc.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ISiteDalc ActiveSiteDalc
        {
            get { return activeSiteDalc ?? (activeSiteDalc = new SiteDalc()); }
        }

        private SiteTocNodeDs.SiteTocNodeDataTable ActiveSiteTocNodeTable
        {
            get { return activeSiteTocNodeTable ?? (activeSiteTocNodeTable = ActiveSiteDalc.GetSiteTocNodes(siteId)); }
        }

        #region ISiteTocNodeCollection Members

        /// <summary>
        /// Gets the <see cref="ITocNode" /> at the specified index.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ITocNode this[int index]
        {
            get { return new SiteTocNode((SiteTocNodeDs.SiteTocNodeRow) ActiveSiteTocNodeTable.Rows[index]); }
        }

        /// <summary>
        /// Gets the count.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Count
        {
            get { return ActiveSiteTocNodeTable.Rows.Count; }
        }

        /// <summary>
        /// Saves this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Save()
        {
            // TODO:  Add SiteTocNodeCollection.Save implementation
        }

        /// <summary>
        /// Gets the enumerator.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            return new SiteTocNodeEnumerator(this);
        }

        #endregion

        #region Nested type: SiteTocNodeEnumerator

        private class SiteTocNodeEnumerator : IEnumerator
        {
            private readonly SiteTocNodeCollection btnc;
            private int index;

            public SiteTocNodeEnumerator(SiteTocNodeCollection SiteTocNodeColl)
            {
                btnc = SiteTocNodeColl;
                Reset();
            }

            #region IEnumerator Members

            public void Reset()
            {
                index = -1;
            }

            public object Current
            {
                get { return new SiteTocNode((SiteTocNodeDs.SiteTocNodeRow) btnc.ActiveSiteTocNodeTable.Rows[index]); }
            }

            public bool MoveNext()
            {
                index++;
                return (index < btnc.ActiveSiteTocNodeTable.Rows.Count);
            }

            #endregion
        }

        #endregion
    }
}