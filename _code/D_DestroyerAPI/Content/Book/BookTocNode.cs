#region

using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Book
{
    /// <summary>
    ///   Summary description for BookTocNode.
    /// </summary>
    public class BookTocNode : IBookTocNode
    {
        #region Private

        private readonly BookTocNodeDs.BookTocNodeRow activeBookTocNodeRow;

        #endregion

        #region Public

        #region Constructors

        /// <summary>
        ///   Creates a new book toc node object using the provided book toc node row.
        /// </summary>
        /// <param name = "bookTocNodeRow">A book toc node row to use as the underlying data for this object</param>
        public BookTocNode(BookTocNodeDs.BookTocNodeRow bookTocNodeRow)
        {
            activeBookTocNodeRow = bookTocNodeRow;
        }

        #endregion

        /// <summary>
        ///   The id of this toc node. The NodeType property must be used to determine what type of id is being refered to here.
        /// </summary>
        public int NodeId
        {
            get { return activeBookTocNodeRow.NodeId; }
        }

        /// <summary>
        ///   The name of the node.
        /// </summary>
        public string Name
        {
            get { return activeBookTocNodeRow.Name; }
        }

        /// <summary>
        ///   The title of the node to use for visual display of the node in a table of contents.
        /// </summary>
        public string Title
        {
            get { return activeBookTocNodeRow.Title; }
        }

        /// <summary>
        ///   The type of the node.
        /// </summary>
        public NodeType NodeType
        {
            get { return (NodeType) activeBookTocNodeRow.NodeTypeId; }
        }

        /// <summary>
        ///   The left value for the node.
        /// </summary>
        public int Left
        {
            get { return activeBookTocNodeRow.Left; }
        }

        /// <summary>
        ///   The right value for the node.
        /// </summary>
        public int Right
        {
            get { return activeBookTocNodeRow.Right; }
        }

        /// <summary>
        ///   Indicates whether or not the toc node has child nodes
        /// </summary>
        public bool HasChildren
        {
            get { return activeBookTocNodeRow.HasChildren; }
        }

        /// <summary>
        ///   Indicates whether or not the node should be hidden in the toc
        /// </summary>
        public bool Hidden
        {
            get { return activeBookTocNodeRow.Hidden; }
        }

        /// <summary>
        ///   Returns the uri of the node
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        #endregion
    }
}