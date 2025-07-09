#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Exposes properties and methods for working with collections of IDocument objects.
    /// </summary>
    public interface IDocumentCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of Document objects in this collection
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Indexer to retrieve an IDocument interface by document index
        /// </summary>
        IDocument this[int index] { get; }

        /// <summary>
        ///   Indexer to retrieve an IDocument interface by document name
        /// </summary>
        IDocument this[string name] { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Save the IDocument objects in the collection.
        ///</summary>
        void Save();

        #endregion Methods
    }
}