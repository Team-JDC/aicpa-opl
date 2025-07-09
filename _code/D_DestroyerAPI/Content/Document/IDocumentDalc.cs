namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for IDocumentDALC.
    /// </summary>
    public interface IDocumentDalc
    {
        ///<summary>
        ///  Get a single document dataset.
        ///</summary>
        ///<param name = "documentId">The Id of the document to be returned.</param>
        ///<returns>A strongly-typed datatable containing a single document.</returns>
        DocumentDs GetDocument(int documentId);

        ///<summary>
        ///  Get a single document dataset from within the context of a book.
        ///</summary>
        ///<param name = "bookId">The Id of the book containing the desired document</param>
        ///<param name = "documentId">The Id of the document being requested</param>
        ///<returns>A strongly-typed dataset containing a single document.</returns>
        DocumentDs GetBookDocument(int bookId, int documentId);

        /// <summary>
        ///   Get a single document dataset from within the context of a book, using an ordinal index value.
        /// </summary>
        /// <param name = "bookId">The Id of the book containing the desired document</param>
        /// <param name = "index">The index of the document to be retrieved (order is determined by toc placement or the document node).</param>
        /// <returns></returns>
        DocumentDs GetBookDocumentByIndex(int bookId, int index);

        ///<summary>
        ///  Get a document dataset based on the namedAnchor and book information
        ///</summary>
        ///<param name = "bookId">The book instance id of the book containing the document whose id is to be returned</param>
        ///<param name = "documentAnchorName">The name of the document anchor for which you wish to retrieve the containing document.</param>
        ///<returns>The document's id</returns>
        DocumentDs GetBookDocumentByDocumentAnchor(int bookId, string documentAnchorName);

        /// <summary>
        ///   Get the documents associated with the specified book.
        /// </summary>
        /// <param name = "bookId">The id of the book for which documents are to be retrieved</param>
        /// <returns>A strongly typed dataset for holding documents.</returns>
        DocumentDs GetBookDocuments(int bookId);

        /// <summary>
        ///   Gets a named achor row for the specified named anchor id
        /// </summary>
        /// <param name = "namedAnchorId">The id of the named anchor to be retrieved</param>
        /// <returns></returns>
        DocumentDs.NamedAnchorRow GetNamedAnchor(int namedAnchorId);

        /// <summary>
        ///   Gets a named achor row for the specified named anchor name
        /// </summary>
        /// <param name = "namedAnchorName">The name of the named anchor to be retrieved</param>
        /// <param name = "documentInstanceId">The id of the document containing the named anchor</param>
        /// <returns></returns>
        DocumentDs.NamedAnchorRow GetNamedAnchor(int documentInstanceId, string namedAnchorName);

        /// <summary>
        ///   Gets the named achor rows associated with the given document id
        /// </summary>
        /// <param name = "documentId">The id of the document for which named anchors are needed.</param>
        /// <returns></returns>
        DocumentDs.NamedAnchorRow[] GetNamedAnchors(int documentId);

        ///<summary>
        ///  Insert, Update, or Delete a document or documents based on the dataset provided.
        ///</summary>
        ///<param name = "documentDs">A strongly-typed dataset containing one or more document rows.</param>
        void Save(DocumentDs documentDs);
    }
}