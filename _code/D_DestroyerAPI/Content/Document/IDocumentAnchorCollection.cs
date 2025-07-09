#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for IDocumentAnchorCollection.
    /// </summary>
    public interface IDocumentAnchorCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of DocumentAnchor objects in this collection
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Retrieves the document anchor by ordinal
        /// </summary>
        IDocumentAnchor this[int index] { get; }

        /// <summary>
        ///   Retrieves the document anchor by name
        /// </summary>
        IDocumentAnchor this[string name] { get; }

        #endregion Properties

        #region Methods

        #endregion Methods
    }
}