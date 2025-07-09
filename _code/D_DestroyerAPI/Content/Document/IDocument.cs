#region

using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.User.Note;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Exposes properties and methods for working with individual documents of a book.
    /// </summary>
    public interface IDocument : ITocNode, IPrimaryContentContainer
    {
        #region Properties

        /// <summary>
        ///   The book under which the document resides. Will be null if the document was retrieved outside of the context of a book.
        /// </summary>
        IBook Book { get; }

        /// <summary>
        ///   The document's internal Id. This value is set by the system and is read-only.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///   The document's name. This is a short identifier (e.g. 'pc_170', 'personal_financial_planning', 'aag-air_app_d') for the document.
        /// </summary>
        new string Name { get; }

        /// <summary>
        ///   The document's title. This is a descriptive name (e.g. 'Conforming Amendments to PCAOB Interim Standards...', 'Personal Financial Planning', 'Appendix D') for the book.
        /// </summary>
        new string Title { get; }

        /// <summary>
        ///   The usernote associated with the document.
        /// </summary>
        INote Note { get; set; }

        /// <summary>
        ///   The title path of the document down to its containing book, including both the book title and the document title. If the document is
        ///   not retrieved within the context of a book, the value of this property is null.
        /// </summary>
        string BookReferencePath { get; }

        /// <summary>
        ///   The title path of the document down to its containing site, including both the site title and the document title. If the document is
        ///   not retrieved within the context of a site, the value of this property is null.
        /// </summary>
        string SiteReferencePath { get; }

        /// <summary>
        ///   A collection of document anchors associated with the document.
        /// </summary>
        IDocumentAnchorCollection Anchors { get; set; }

        /// <summary>
        ///   The document formats associated with the document.
        /// </summary>
        IDocumentFormatCollection Formats { get; set; }

        /// <summary>
        ///   The IDocumentFormat for the document's primary format.
        /// </summary>
        IDocumentFormat PrimaryFormat { get; }

        ///<summary>
        ///  A string that represents a snippet of text from the document that shows the hit keywords in context. This property is used within the context of search results.
        ///</summary>
        string KeyWordsInContext { get; set; }

        ///<summary>
        ///  A flag that is used to indicate whether or not the book was pulled from the user's subscription. This property is used within the context of search results.
        ///</summary>
        bool InSubscription { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Quick retrieval for document anchor by name
        /// </summary>
        /// <param name = "documentAnchorName">Anchor name to retrieve</param>
        /// <returns></returns>
        IDocumentAnchor GetDocumentAnchor(string documentAnchorName);

        /// <summary>
        ///   Save the document.
        /// </summary>
        void Save();

        bool hasPreviousDocument();
        bool hasNextDocument();

        #endregion Methods
    }
}