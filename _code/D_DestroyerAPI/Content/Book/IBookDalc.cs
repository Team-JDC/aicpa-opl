#region

using AICPA.Destroyer.Shared;

#endregion
namespace AICPA.Destroyer.Content.Book
{
	/// <summary>
	/// This interface describes properties and methods for storage and retrieval of book data.
	/// </summary>
	public interface IBookDalc
	{
		/// <summary>
		/// Returns a book dataset with the first row of the book table populated with information for the specified book.
		/// </summary>
		/// <param name="bookId">The id of the book you wish to retrieve.</param>
		/// <returns>A book dataset.</returns>
		BookDs GetBook (int bookId);

		/// <summary>
		/// Returns a book dataset with the first row of the book table populated with information from the specified book. Used
		/// when retrieving a book from within the context of a site (extra site information is available with this retrieval).
		/// </summary>
		/// <param name="siteId">The id of the context site.</param>
		/// <param name="bookId">The id of the book you wish to retrieve.</param>
		/// <returns></returns>
		BookDs GetBook(int siteId, int bookId);

		/// <summary>
		/// Returns a book dataset containing the books associated with the specified site.
		/// </summary>
		/// <param name="siteId">The siteId of the sitebook to be returned.</param>
		/// <returns>A book dataset</returns>
		BookDs GetSiteBooks (int siteId);

        /// <summary>
        /// Returns a list of all books in the datastore.
        /// </summary>
        /// <param name="latestVersion">Determines whether all versions of all books should be returned or only the current version of all books.</param>
        /// <param name="includeArchived">if set to <c>true</c> [include archived].</param>
        /// <returns>A book dataset</returns>
		BookDs GetBooks (bool latestVersion, bool includeArchived);

		/// <summary>
		/// Returns a book dataset containing the books associated with the specified site, filtered by the specified bookDomain.
		/// </summary>
		/// <param name="siteId">The siteId of the sitebook to be returned.</param>
		/// /// <param name="bookDomain">The list of books allowed to be returned. Must be formatted for a SQL WHERE clause as follows:
		///		'book1','book2','book3'</param>
		/// <returns>A book dataset.</returns>
		BookDs GetSiteBooks (int siteId, string bookDomain);

		/// <summary>
		/// Returns a book toc node dataset containing the book toc node children underneath the context node.
		/// </summary>
		/// <param name="nodeId">The id of the context node for which you want to retrieve children.</param>
		/// <param name="nodeType">The node type of the context node.</param>
		/// <returns>A book toc node dataset.</returns>
		BookTocNodeDs.BookTocNodeDataTable GetChildBookTocNodes(int nodeId, NodeType nodeType);

		/// <summary>
		/// Returns a book toc node dataset containing all book toc nodes for the specified book.
		/// </summary>
		/// <param name="bookId">The id of the book for which you want to retrieve children.</param>
		/// <returns>A book toc node dataset.</returns>
		BookTocNodeDs.BookTocNodeDataTable GetBookTocNodes(int bookId);

		/// <summary>
		/// Returns a book toc node dataset containing the document book toc node that follows the specified document.
		/// </summary>
		/// <param name="bookId">The id of the book containing the context document.</param>
		/// <param name="documentId">The id of the context document.</param>
		/// <returns></returns>
		BookTocNodeDs.BookTocNodeRow GetNextDocument(int bookId, int documentId);

		/// <summary>
		/// Returns a book toc node dataset containing the document book toc node that precedes the specified document.
		/// </summary>
		/// <param name="bookId">The id of the book containing the context document.</param>
		/// <param name="documentId">The id of the context document.</param>
		/// <returns></returns>
		BookTocNodeDs.BookTocNodeRow GetPreviousDocument(int bookId, int documentId);

		/// <summary>
		/// Clears all documents and related content associated with the specified book instance id.
		/// </summary>
		/// <param name="bookId">The book instance id for which you want to retrieve a document count.</param>
		/// <returns></returns>	
		void ClearBookContent(int bookId);

		/// <summary>
		/// Returns the number of documents in the specified book instance.
		/// </summary>
		/// <param name="bookId">The book instance id for which you want to retrieve a document count.</param>
		/// <returns></returns>
		int GetDocumentCount(int bookId);	

		/// <summary>
		/// Saves the book dataset.
		/// </summary>
		/// <param name="bookDs">The book dataset to save.</param>
		void Save (BookDs bookDs);

		/// <summary>
		/// Saves the book toc node dataset.
		/// </summary>
		/// <param name="bookTocNodeDs">The book toc node dataset to save.</param>
		void Save (BookTocNodeDs bookTocNodeDs);

		/// <summary>
		/// Saves the book folder dataset.
		/// </summary>
		/// <param name="bookFolderDs">The book folder dataset to save.</param>
		void Save (BookFolderDs bookFolderDs);
	}
}
