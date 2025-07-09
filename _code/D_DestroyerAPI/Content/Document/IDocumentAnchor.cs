namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for IDocumentAnchor.
    /// </summary>
    public interface IDocumentAnchor : ITocNode, IPrimaryContentContainer
    {
        #region Properties

        /// <summary>
        ///   The document object containing this anchor
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        ///   The id of this anchor
        /// </summary>
        int Id { get; }

        /// <summary>
        ///   The name of this anchor
        /// </summary>
        new string Name { get; }

        /// <summary>
        ///   The title of this anchor
        /// </summary>
        new string Title { get; }

        /// <summary>
        ///   The title path of the anchor down to its containing book, including both the book title and the anchor title. If the anchor is
        ///   not retrieved within the context of a book, the value of this property is null.
        /// </summary>
        string BookReferencePath { get; }

        /// <summary>
        ///   The title path of the anchor down to its containing site, including both the site title and the anchor title. If the anchor is
        ///   not retrieved within the context of a site, the value of this property is null.
        /// </summary>
        string SiteReferencePath { get; }

        #endregion Properties

        #region Methods

        #endregion Methods
    }
}