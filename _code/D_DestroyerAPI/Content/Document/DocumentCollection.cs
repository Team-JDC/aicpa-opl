#region

using System;
using System.Collections;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for DocumentCollection.
    /// </summary>
    public class DocumentCollection : DestroyerBpc, IDocumentCollection, IEnumerable
    {
        #region Constants

        #endregion Constants

        #region Private

        //private fields used for object construction
        private readonly IBook book;

        /// <summary>
        ///   The active book dalc
        /// </summary>
        private BookDalc activeBookDalc;

        /// <summary>
        ///   The active number of documents in the book
        /// </summary>
        private int activeDocumentCount = EMPTY_INT;

        /// <summary>
        ///   The active document dalc
        /// </summary>
        private DocumentDalc activeDocumentDalc;

        private int ActiveDocumentCount
        {
            get
            {
                if (activeDocumentCount == EMPTY_INT)
                {
                    activeDocumentCount = ActiveBookDalc.GetDocumentCount(book.Id);
                }
                return activeDocumentCount;
            }
        }

        private DocumentDalc ActiveDocumentDalc
        {
            get { return activeDocumentDalc ?? (activeDocumentDalc = new DocumentDalc()); }
        }

        private BookDalc ActiveBookDalc
        {
            get { return activeBookDalc ?? (activeBookDalc = new BookDalc()); }
        }

        #endregion Private

        #region Constructors

        /// <summary>
        ///   Documents to be retrieved from the specified book
        /// </summary>
        /// <param name = "book"></param>
        public DocumentCollection(IBook book)
        {
            this.book = book;
        }

        /// <summary>
        ///   Creation of an empty document collection that may be added to with the Add method
        /// </summary>
        public DocumentCollection()
        {
        }

        #endregion Constructors

        #region IDocumentCollection Members

        /// <summary>
        ///   Indexer to retrieve by ordinal
        /// </summary>
        public IDocument this[int index]
        {
            get
            {
                try
                {
                    DocumentDs documentDs = ActiveDocumentDalc.GetBookDocumentByIndex(book.Id, index);
                    return new Document(book, (DocumentDs.DocumentRow)documentDs.Document.Rows[0]);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///   Indexer to retrieve by document name
        /// </summary>
        public IDocument this[string name]
        {
            get
            {
                IDocument retDoc = null;
                DocumentDs documentDs = ActiveDocumentDalc.GetBookDocument(book.Id, name);
                if (documentDs.Document.Rows.Count > 0)
                {
                    retDoc = new Document(book, (DocumentDs.DocumentRow) documentDs.Document.Rows[0]);
                }
                return retDoc;
            }
        }

        /// <summary>
        /// </summary>
        public int Count
        {
            get { return ActiveDocumentCount; }
        }

        /// <summary>
        /// </summary>
        public void Save()
        {
            throw new Exception(ERROR_NOTIMPLEMENTED);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new DocumentEnumerator(book, this);
        }

        #endregion

        #region Nested type: DocumentEnumerator

        /// <summary>
        /// </summary>
        private class DocumentEnumerator : IEnumerator
        {
            private readonly IBook book;
            private readonly DocumentCollection dc;
            private int index;

            public DocumentEnumerator(IBook Book, DocumentCollection DocumentColl)
            {
                book = Book;
                dc = DocumentColl;
                Reset();
            }

            #region IEnumerator Members

            /// <summary>
            /// </summary>
            public void Reset()
            {
                index = -1;
            }

            /// <summary>
            /// </summary>
            public object Current
            {
                get
                {
                    DocumentDs documentDs = dc.ActiveDocumentDalc.GetBookDocumentByIndex(book.Id, index);
                    return new Document(book, (DocumentDs.DocumentRow) documentDs.Document.Rows[0]);
                }
            }

            /// <summary>
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                index++;
                return (index < dc.Count);
            }

            #endregion
        }

        #endregion
    }
}