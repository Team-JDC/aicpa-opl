#region

using System;
using System.Collections;
using System.Data;
using System.Reflection;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Book
{
    /// <summary>
    ///   Summary description for BookCollection.
    /// </summary>
    public class BookCollection : DestroyerBpc, IEnumerable, IBookCollection
    {
        #region Private Properties

        private readonly string[] bookList;
        private readonly BookBuildStatus buildStatus;
        private readonly bool includeArchived;
        private readonly bool latestVersion;
        private readonly bool retrieveAllBooks;
        private readonly bool retrieveByBuildStatus;
        private readonly ISite site;

        /// <summary>
        ///   the book dalc used for database operations around books
        /// </summary>
        private BookDalc activeBookDalc;

        /// <summary>
        ///   the book dataset that we use for most of our private storage
        /// </summary>
        private BookDs activeBookDs;

        /// <summary>
        ///   the site dalc used for database operations around sites
        /// </summary>
        private SiteDalc activeSiteDalc;

        private BookDs.BookRow[] sortedBookRows;

        private SiteDalc ActiveSiteDalc
        {
            get { return activeSiteDalc ?? (activeSiteDalc = new SiteDalc()); }
        }

        private BookDalc ActiveBookDalc
        {
            get { return activeBookDalc ?? (activeBookDalc = new BookDalc()); }
        }

        private BookDs ActiveBookDs
        {
            get
            {
                if (activeBookDs == null)
                {
                    //if we were not constructed with a site...
                    if (site == null)
                    {
                        //if retrieveAllBooks we need to grab all books from the site
                        if (retrieveAllBooks)
                        {
                            activeBookDs = ActiveBookDalc.GetBooks(latestVersion, includeArchived);
                        }
                        else
                        {
                            activeBookDs = retrieveByBuildStatus ? ActiveBookDalc.GetBooks(buildStatus) : new BookDs();
                        }
                    }
                        // if we were constructed with a site, it means that we are being used for getting at the books associated with a site
                    else
                    {
                        //if we were constructed with a book list, we need to only return back the books in that list
                        if (bookList != null && bookList.Length > 0)
                        {
                            activeBookDs = ActiveBookDalc.GetSiteBooks(site.Id, TranslateBookDomain(bookList));
                        }
                            //if we were not constructed with a book list, don't do any filtering
                        else
                        {
                            activeBookDs = ActiveBookDalc.GetSiteBooks(site.Id);
                        }
                    }
                }
                //return our book dataset
                return activeBookDs;
            }
        }

        private BookDs.BookRow[] SortedBookRows
        {
            get { return sortedBookRows ?? (sortedBookRows = (BookDs.BookRow[]) ActiveBookDs.Book.Select("", SortBy)); }
        }

        private string SortBy
        {
            get { return SortField + ((Ascending) ? " asc" : " desc"); }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor for creating a collection of books that belong to a filtered site
        /// </summary>
        /// <param name = "site"></param>
        /// <param name = "bookList"></param>
        public BookCollection(ISite site, string[] bookList)
        {
            this.site = site;
            this.bookList = bookList;
        }

        /// <summary>
        ///   Constructor for creating a collection of books that belong to a site
        /// </summary>
        /// <param name = "site"></param>
        public BookCollection(ISite site)
        {
            this.site = site;
        }

        /// <summary>
        ///   Constructor used for creating a collection of all books.
        /// </summary>
        public BookCollection(bool latestVersion, bool includeArchived)
        {
            retrieveAllBooks = true;
            this.includeArchived = includeArchived;
            this.latestVersion = latestVersion;
        }

        /// <summary>
        ///   Constructor used for creation of a new book collection for creating subscriptions.
        /// </summary>
        public BookCollection()
        {
        }

        /// <summary>
        ///   Constructor used for retrieving a collection of books with the specified book build status.
        /// </summary>
        /// <param name = "buildStatus"></param>
        public BookCollection(BookBuildStatus buildStatus)
        {
            retrieveByBuildStatus = true;
            this.buildStatus = buildStatus;
        }

        #endregion

        private bool ascending = true;
        private BookSortField sortField = BookSortField.Left;

        #region IBookCollection Members

        /// <summary>
        ///   Indexer for retrieving a book by ordinal value
        /// </summary>
        public IBook this[int index]
        {
            get
            {
                //return new Book(site, ((BookDs.BookRow)this.ActiveBookDs.Book.Rows[index]));
                return new Book(site, (SortedBookRows[index]));
            }
        }

        /// <summary>
        ///   Indexer for retrieving a book by name
        /// </summary>
        public IBook this[string name]
        {
            get
            {
                IBook retBook = null;
                //query our book table for a row containing the specified book name
                if (ActiveBookDs.Book.Rows.Count > 0)
                {
                    DataRow[] drs = ActiveBookDs.Book.Select(string.Format("Name = '{0}'", name));
                    if (drs.Length > 0)
                    {
                        BookDs.BookRow bookRow = (BookDs.BookRow) drs[0];
                        retBook = new Book(site, bookRow);
                    }
                }
                return retBook;
            }
        }

        /// <summary>
        ///   Indexer for retrieving a book by id
        /// </summary>
        public IBook GetBookByBookInstanceId(int bookInstanceId)
        {
            IBook retBook = null;
            //query our book table for a row containing the specified book name
            if (ActiveBookDs.Book.Rows.Count > 0)
            {
                retBook = new Book(site, ActiveBookDs.Book.FindByBookInstanceId(bookInstanceId));
            }
            return retBook;
        }

        /// <summary>
        ///   The number of books in the collection
        /// </summary>
        public int Count
        {
            get { return ActiveBookDs.Book.Rows.Count; }
        }

        /// <summary>
        /// The booklist that represents all books the user has access to
        /// </summary>
        public string[] BookList
        {
            get
            {
                return bookList;
            }
        }

        /// <summary>
        ///   Save the books in the collection
        /// </summary>
        public void Save()
        {
            throw new Exception("The method '" + MethodBase.GetCurrentMethod().Name + "' is not yet implemented");
        }

        /// <summary>
        /// </summary>
        /// <param name = "book"></param>
        public void Add(IBook book)
        {
            //if we were not constructed with a site, then we should simply add the book to our private book dataset
            if (site == null)
            {
                ActiveBookDs.Book.AddBookRow(book.Name, book.Version, book.Title, book.Description, book.PublishDate,
                                             book.Copyright, (int) book.SourceType, book.SourceUri, book.ReferencePath,
                                             0, 0, book.Archived, book.IsEditable, false, (int) book.BuildStatus);
            }
                //if we were constructed with a site, then we should call the site's AddBook method
            else
            {
                site.AddBook(book);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "book"></param>
        public void Remove(IBook book)
        {
            //if we were not constructed with a site, then we should simply remove the book from our private book dataset
            if (site == null)
            {
                BookDs.BookRow bookRow =
                    (BookDs.BookRow) ActiveBookDs.Book.Select(string.Format("BookId={0}", book.Id))[0];
                bookRow.Delete();
            }
                //if we were constructed with a site, then we should call the site's AddBook method
            else
            {
                site.RemoveBook(book);
            }
        }

        /// <summary>
        /// Gets or sets the sort field.	
        /// </summary>
        /// <value>The sort field.</value>
        /// <remarks></remarks>
        public BookSortField SortField
        {
            get { return sortField; }
            set
            {
                sortedBookRows = null;
                sortField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ascending.	
        /// </summary>
        /// <value>The ascending.</value>
        /// <remarks></remarks>
        public bool Ascending
        {
            get { return ascending; }
            set
            {
                sortedBookRows = null;
                ascending = value;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///   return an IEnumerator object for enumerating through the book collection
        /// </summary>
        /// <returns>Object implementing IEnumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return new BookEnumerator(site, this);
        }

        #endregion

        #region Nested type: BookEnumerator

        /// <summary>
        ///   The BookEnumerator is a class that manages enumeration of a collection of books
        /// </summary>
        private class BookEnumerator : IEnumerator
        {
            private readonly BookCollection bc;
            private readonly ISite site;
            private int index;

            /// <summary>
            ///   The constructor of our book enumerator.
            /// </summary>
            /// <param name = "Site">The site under which the collection of books reside</param>
            /// <param name = "BookColl">The collection of books to enumerate</param>
            public BookEnumerator(ISite Site, BookCollection BookColl)
            {
                bc = BookColl;
                site = Site;
                Reset();
            }

            #region IEnumerator Members

            /// <summary>
            ///   Reset our index
            /// </summary>
            public void Reset()
            {
                index = -1;
            }

            /// <summary>
            ///   Return the book row at the current index
            /// </summary>
            public object Current
            {
                get
                {
                    //return new Book(site, ((BookDs.BookRow)bc.ActiveBookDs.Book.Rows[index]));
                    return new Book(site, (bc.SortedBookRows[index]));
                }
            }

            /// <summary>
            ///   Advance our index
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                index++;
                return (index < bc.ActiveBookDs.Book.Rows.Count);
            }

            #endregion
        }

        #endregion
    }
}