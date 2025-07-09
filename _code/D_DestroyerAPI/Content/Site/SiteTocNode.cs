#region

using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Summary description for SiteTocNode.
    /// </summary>
    public class SiteTocNode : ISiteTocNode, ITocNode
    {
        #region Private

        private readonly SiteTocNodeDs.SiteTocNodeRow activeSiteTocNodeRow;

        #endregion

        #region Public

        #region Constructors

        /// <summary>
        ///   Creates a new Site toc node object using the provided Site toc node row.
        /// </summary>
        /// <param name = "SiteTocNodeRow">A Site toc node row to use as the underlying data for this object</param>
        public SiteTocNode(SiteTocNodeDs.SiteTocNodeRow SiteTocNodeRow)
        {
            activeSiteTocNodeRow = SiteTocNodeRow;
        }

        #endregion

        /// <summary>
        ///   The id of this toc node. The NodeType property must be used to determine what type of id is being refered to here.
        /// </summary>
        public int NodeId
        {
            get { return activeSiteTocNodeRow.NodeId; }
        }

        /// <summary>
        ///   The name of the node.
        /// </summary>
        public string Name
        {
            get { return activeSiteTocNodeRow.Name; }
        }

        /// <summary>
        ///   The title of the node to use for visual display of the node in a table of contents.
        /// </summary>
        public string Title
        {
            get { return activeSiteTocNodeRow.Title; }
        }

        /// <summary>
        ///   The type of the node.
        /// </summary>
        public NodeType NodeType
        {
            get { return (NodeType) activeSiteTocNodeRow.NodeTypeId; }
        }

        /// <summary>
        ///   The left value for the node.
        /// </summary>
        public int Left
        {
            get { return activeSiteTocNodeRow.Left; }
        }

        /// <summary>
        ///   The right value for the node.
        /// </summary>
        public int Right
        {
            get { return activeSiteTocNodeRow.Right; }
        }

        /// <summary>
        ///   ITocNode interface property indicating whether or not the toc node has children nodes
        /// </summary>
        public bool HasChildren
        {
            get { return activeSiteTocNodeRow.HasChildren; }
        }

        /// <summary>
        ///   ITocNode interface property indicates whether or not the node should be hidden in the toc
        /// </summary>
        public bool Hidden
        {
            get { return false; }
        }

        /// <summary>
        ///   ITocNode interface property that returns the uri of the node
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        #endregion
    }
}