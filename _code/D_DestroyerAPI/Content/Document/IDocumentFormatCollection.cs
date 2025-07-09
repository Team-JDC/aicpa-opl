#region

using System.Collections;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for IDocumentFormatCollection.
    /// </summary>
    public interface IDocumentFormatCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of DocumentFormat objects in this collection
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Indexer to retrieve by ordinal
        /// </summary>
        IDocumentFormat this[int index] { get; }

        /// <summary>
        ///   Indexer to retrieve by content type description
        /// </summary>
        IDocumentFormat this[string contentTypeDesc] { get; }

        /// <summary>
        ///   Indexer to retrieve by content type enum
        /// </summary>
        IDocumentFormat this[ContentType contentType] { get; }

        #endregion Properties

        #region Methods

        #endregion Methods
    }
}