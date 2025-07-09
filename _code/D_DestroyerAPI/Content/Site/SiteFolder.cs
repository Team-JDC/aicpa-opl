#region

using System;
using System.Xml;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for SiteFolder.
    /// </summary>
    public class SiteFolder : DestroyerBpc, ISiteFolder
    {
        #region Private Fields

        private readonly int folderId = -1;
        private readonly ISite site;

        #endregion Private Fields

        private SiteDalc activeSiteDalc;
        private SiteFolderDs activeSiteFolderDs;

        private SiteFolderDs.SiteFolderRow activeSiteFolderRow;

        private SiteFolderDs ActiveSiteFolderDs
        {
            get { return activeSiteFolderDs ?? (activeSiteFolderDs = ActiveSiteDalc.GetSiteFolder(site.Id, folderId)); }
        }

        private SiteFolderDs.SiteFolderRow ActiveSiteFolderRow
        {
            get
            {
                if (activeSiteFolderRow == null && ActiveSiteFolderDs != null &&
                    ActiveSiteFolderDs.SiteFolder.Rows.Count > 0)
                {
                    activeSiteFolderRow = (SiteFolderDs.SiteFolderRow) ActiveSiteFolderDs.SiteFolder.Rows[0];
                }
                return activeSiteFolderRow;
            }
        }

        private SiteDalc ActiveSiteDalc
        {
            get { return activeSiteDalc ?? (activeSiteDalc = new SiteDalc()); }
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteFolder" /> class.	
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="folderId">The folder id.</param>
        /// <remarks></remarks>
        public SiteFolder(ISite site, int folderId)
        {
            this.site = site;
            this.folderId = folderId;
        }

        #endregion Constructors

        #region ISiteFolder Members

        /// <summary>
        /// Gets the id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Id
        {
            get { return ActiveSiteFolderRow.FolderId; }
        }

        /// <summary>
        /// Gets the name.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Name
        {
            get { return ActiveSiteFolderRow.Name; }
        }

        /// <summary>
        /// Gets the title.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Title
        {
            get { return ActiveSiteFolderRow.Title; }
        }

        /// <summary>
        /// Gets the URI.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Uri
        {
            get { return ActiveSiteFolderRow.Uri; }
        }

        /// <summary>
        /// Gets the site reference path.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string SiteReferencePath
        {
            get { return ReferencePathToXml(ActiveSiteFolderRow.SiteTitlePath); }
        }

        /// <summary>
        /// Gets the node id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int NodeId
        {
            get { return Id; }
        }

        /// <summary>
        /// Gets the type of the node.	
        /// </summary>
        /// <value>The type of the node.</value>
        /// <remarks></remarks>
        public NodeType NodeType
        {
            get { return NodeType.SiteFolder; }
        }

        /// <summary>
        /// Gets the left.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Left
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        /// Gets the right.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Right
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        /// Gets the has children.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool HasChildren
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        /// Gets the hidden.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool Hidden
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        #endregion

        #region IPrimaryContentContainer Properties

        /// <summary>
        /// Gets the content of the primary.	
        /// </summary>
        /// <value>The content of the primary.</value>
        /// <remarks></remarks>
        public ContentWrapper PrimaryContent
        {
            get
            {
                ContentWrapper wrapper;

                if (string.IsNullOrEmpty(Uri))
                {
                    //find the first child of the folder by using the toc xml
                    string folderXml = site.GetTocXml(Id, NodeType);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(folderXml);

                    XmlNode folderChildNode = xmlDoc.SelectSingleNode("*/*");

                    if (folderChildNode == null)
                    {
                        throw new Exception("SiteFolder was expected to either have a Uri or child node");
                    }

                    int id = Convert.ToInt32(GetAttributeValue(folderChildNode.Attributes[XML_ATT_ID]));

                    IPrimaryContentContainer container = PrimaryContentContainer.ConstructContainer(site, id,
                                                                                                    folderChildNode.
                                                                                                        LocalName);
                    wrapper = container.PrimaryContent;
                }
                else
                {
                    wrapper = new ContentWrapper(Uri);
                }

                return wrapper;
            }
        }

        #endregion
    }
}