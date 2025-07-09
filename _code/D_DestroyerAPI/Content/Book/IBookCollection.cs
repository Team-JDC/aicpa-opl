#region

using System.Collections;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Book
{
    ///<summary>
    ///  Exposes properties and methods for working with collections of IBook objects.
    ///</summary>
    public interface IBookCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of books in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The booklist that represents all books the user has access to
        /// </summary>
        string[] BookList { get; }

        /// <summary>
        ///   The order to sort the collection.
        /// </summary>
        BookSortField SortField { get; set; }

        /// <summary>
        ///   The order to sort the collection.
        /// </summary>
        bool Ascending { get; set; }

        /// <summary>
        ///   Indexer for retrieving a book by ordinal value
        /// </summary>
        IBook this[int index] { get; }

        /// <summary>
        ///   Indexer for retrieving a book by name
        /// </summary>
        IBook this[string name] { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Save changes to the IBookCollection.
        /// </summary>
        void Save();

        /// <summary>
        ///   Adds a book to the collection
        /// </summary>
        /// <param name = "book">The book to add to the collection.</param>
        void Add(IBook book);

        /// <summary>
        ///   Removes a book from the collection
        /// </summary>
        /// <param name = "book">The book to remove from the book collection.</param>
        void Remove(IBook book);

        /// <summary>
        ///   Retrieves a book by its instance id.
        /// </summary>
        /// <param name = "bookInstanceId">The instance id of the book to retrieve.</param>
        /// <returns></returns>
        IBook GetBookByBookInstanceId(int bookInstanceId);

        #endregion Methods
    }
}