#region

using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content
{
    /// <summary>
    ///   The ITocNode interface provides properties for accessing a generic table of contentents node.
    /// </summary>
    public interface ITocNode
    {
        /// <summary>
        ///   The id of this toc node. The NodeType property must be used to determine what type of id is being refered to here.
        /// </summary>
        int NodeId { get; }

        /// <summary>
        ///   The name of the node.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   The title of the node to use for visual display of the node in a table of contents.
        /// </summary>
        string Title { get; }

        /// <summary>
        ///   The type of the node.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        ///   The left value for the node.
        /// </summary>
        int Left { get; }

        /// <summary>
        ///   The right value for the node.
        /// </summary>
        int Right { get; }

        /// <summary>
        ///   Indicates whether or not there are children for this toc node
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        ///   Indicates whether or not the node should be hidden in the toc
        /// </summary>
        bool Hidden { get; }

        /// <summary>
        ///   Indicates the uri value for the node.
        /// </summary>
        string Uri { get; }
    }
}