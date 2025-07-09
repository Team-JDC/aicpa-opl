#region

using System;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Book
{
    ///<summary>
    ///  The IBook interface defines properties and methods for creating and saving 
    ///  an IBook.  It also exposes methods for resolving links to IDocuments within
    ///  an IBook and for retrieving documents within an IBook.
    ///</summary>
    public interface IBook : ITocNode, IPrimaryContentContainer
    {
        #region Properties

        /// <summary>
        ///   The site under which the book resides. Will be null if the book was retrieved outside of the context of a site.
        /// </summary>
        ISite Site { get; set; }

        /// <summary>
        ///   The sites under which the book resides.
        /// </summary>
        ISiteCollection Sites { get; }

        /// <summary>
        ///   The book's internal Id. This value is set by the system and is read-only.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///   The book's version. This property is set by the system and is read-only.
        /// </summary>
        int Version { get; }

        /// <summary>
        ///   The book's name. This is a short identifier (e.g. 'ps', 'tpa', 'aag-air') for the book.
        /// </summary>
        new string Name { get; set; }

        /// <summary>
        ///   The book's title. This is a descriptive name (e.g. 'Professional Standards', 'Technical Practice Aids') for the book.
        /// </summary>
        new string Title { get; set; }

        /// <summary>
        ///   The book's description. This is a paragraph-length description of the book.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///   A collection of documents contained in the book. The documents are contained in this collection in content order (the order in which they are represented in the book table of contents).
        /// </summary>
        IDocumentCollection Documents { get; set; }

        /// <summary>
        ///   The copyright text for the book (e.g. "Copyright 2005, American Institute of Certified Public Accountants. All Rights Reserved.').
        /// </summary>
        string Copyright { get; set; }

        /// <summary>
        ///   The date the book was built.
        /// </summary>
        DateTime PublishDate { get; set; }

        /// <summary>
        ///   The title path of the book as it is contained in the site. If the book was not retrieved through the context of a site, this property will be null.
        /// </summary>
        string ReferencePath { get; }

        /// <summary>
        ///   The source type for the book. The value of this property determines how the book is constructed from the SourceUri property.
        ///   SourceType = BookSourceType.Makefile: The book will be built using a file system makefile referenced in the SourceUri property.
        ///   SourceType = BookSourceType.Cms: The book will be built using a Proflit XML document specified as a Documentum locator referenced in the SourceUri property.
        /// </summary>
        BookSourceType SourceType { get; set; }

        /// <summary>
        ///   The source URI for the book. The value of this property depends on the SourceType property.
        ///   SourceType = BookSourceType.Makefile: The book will be built using a file system makefile referenced in the SourceUri property.
        ///   SourceType = BookSourceType.Cms: The book will be built using a Proflit XML document specified as a Documentum locator referenced in the SourceUri property.
        /// </summary>
        string SourceUri { get; set; }

        /// <summary>
        ///   Xml string describing the structure of the book.
        /// </summary>
        string BookXml { get; }

        /// <summary>
        ///   Indicates whether or not the book has pending changes that need to be committed
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        ///   Indicates whether or not the book should be edited in its current state
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        ///   Indicates whether or not the book has been archived.
        /// </summary>
        bool Archived { get; set; }

        /// <summary>
        ///   Indicates build state for a book
        /// </summary>
        BookBuildStatus BuildStatus { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Retrieves the document that contains the specified document anchor.
        /// </summary>
        /// <param name = "documentAnchorName">The name of the document anchor for which you wish to retrieve the containing document.</param>
        /// <returns>An interface representing the document containing the specified document anchor.</returns>
        IDocument GetDocumentByAnchorName(string documentAnchorName);

        /// <summary>
        ///   Saves the current book.
        /// </summary>
        void Save();

        /// <summary>
        ///   Retrieves the document that follows the specified document.
        /// </summary>
        /// <param name = "contextDocument">The context document.</param>
        /// <returns>An interface representing the next document in the book. If the context document is the last document in the book, a null value is returned.</returns>
        IDocument GetNextDocument(IDocument contextDocument);

        /// <summary>
        ///   Retrieves the document that precedes the specified document.
        /// </summary>
        /// <param name = "contextDocument">The context document.</param>
        /// <returns>An interface representing the preceding document in the book. If the context document is the first document in the book, a null value is returned.</returns>
        IDocument GetPreviousDocument(IDocument contextDocument);

        /// <summary>
        ///   Builds the book using the values contained in the SourceType and SourceUri properties.
        /// </summary>
        void Build();

        /// <summary>
        ///   Retrieves XML representing the toc nodes that are immediate children to the context node.
        /// </summary>
        /// <param name = "nodeId">The id of the context node</param>
        /// <param name = "nodeType">The type of the context node</param>
        /// <returns>An interface representing the table of contents structure for the book.</returns>
        string GetTocXml(int nodeId, NodeType nodeType);

        #endregion Methods
    }
}