#region

using System.Collections;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Book
{
    /// <summary>
    ///   Summary description for BookTocNodeCollection.
    /// </summary>
    public class BookTocNodeCollection : IBookTocNodeCollection
    {
        private readonly int bookId = -1;
        private BookDalc activeBookDalc;
        private BookTocNodeDs.BookTocNodeDataTable activeBookTocNodeTable;
        private IBook book;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookTocNodeCollection"/> class.
        /// </summary>
        /// <param name="book">The book.</param>
        public BookTocNodeCollection(IBook book)
        {
            this.book = book;
            bookId = book.Id;
        }

        /// <summary>
        ///   Retrieves a collection of toc nodes representing the children of the specified node.
        /// </summary>
        /// <param name = "nodeId"></param>
        /// <param name = "nodeType"></param>
        public BookTocNodeCollection(int nodeId, NodeType nodeType)
        {
            activeBookTocNodeTable = ActiveBookDalc.GetChildBookTocNodes(nodeId, nodeType);
        }

        /// <summary>
        /// Gets the active book dalc.
        /// </summary>
        /// <value>The active book dalc.</value>
        public BookDalc ActiveBookDalc
        {
            get { return activeBookDalc ?? (activeBookDalc = new BookDalc()); }
        }

        private BookTocNodeDs.BookTocNodeDataTable ActiveBookTocNodeTable
        {
            get
            {
                return activeBookTocNodeTable ??
                       (activeBookTocNodeTable = ActiveBookDalc.GetBookTocNodes(bookId));
            }
        }

        #region IBookTocNodeCollection Members

        /// <summary>
        /// Gets the <see cref="ITocNode" /> at the specified index.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ITocNode this[int index]
        {
            get { return new BookTocNode((BookTocNodeDs.BookTocNodeRow) ActiveBookTocNodeTable.Rows[index]); }
        }

        /// <summary>
        /// Gets the count.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Count
        {
            get { return ActiveBookTocNodeTable.Rows.Count; }
        }

        /// <summary>
        /// Saves this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Save()
        {
            // TODO:  Add BookTocNodeCollection.Save implementation
        }

        /// <summary>
        /// Gets the enumerator.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            return new BookTocNodeEnumerator(this);
        }

        #endregion

        #region Nested type: BookTocNodeEnumerator

        private class BookTocNodeEnumerator : IEnumerator
        {
            private readonly BookTocNodeCollection btnc;
            private int index;

            public BookTocNodeEnumerator(BookTocNodeCollection BookTocNodeColl)
            {
                btnc = BookTocNodeColl;
                Reset();
            }

            #region IEnumerator Members

            public void Reset()
            {
                index = -1;
            }

            public object Current
            {
                get { return new BookTocNode((BookTocNodeDs.BookTocNodeRow) btnc.ActiveBookTocNodeTable.Rows[index]); }
            }

            public bool MoveNext()
            {
                index++;
                return (index < btnc.ActiveBookTocNodeTable.Rows.Count);
            }

            #endregion
        }

        #endregion
    }
}