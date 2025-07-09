#region

using System.Collections;
using System.Linq;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for DocumentFormatCollection.
    /// </summary>
    public class DocumentFormatCollection : DestroyerBpc, IDocumentFormatCollection, IEnumerable
    {
        private readonly IDocument document;
        private readonly DocumentDs.DocumentFormatRow[] documentFormatRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentFormatCollection" /> class.	
        /// </summary>
        /// <param name="documentFormatRows">The document format rows.</param>
        /// <param name="document">The document.</param>
        /// <remarks></remarks>
        public DocumentFormatCollection(DocumentDs.DocumentFormatRow[] documentFormatRows, IDocument document)
        {
            this.documentFormatRows = documentFormatRows;
            this.document = document;
        }

        #region IDocumentFormatCollection Members

        /// <summary>
        ///   The number of document formats in this collection.
        /// </summary>
        public int Count
        {
            get { return documentFormatRows.Length; }
        }

        /// <summary>
        ///   Indexer to retrieve a document format by ordinal
        /// </summary>
        public IDocumentFormat this[int index]
        {
            get { return new DocumentFormat(documentFormatRows[index], document); }
        }

        /// <summary>
        ///   Indexer to retrieve a document format by content type description
        /// </summary>
        public IDocumentFormat this[string contentTypeDesc]
        {
            get
            {
                DocumentFormat retDf = null;
                //should probably use a Hashtable with content type as key so we can perform lookup more quickly
                //need to retain order, however, so still need array if hashtable is used
                DocumentDs.DocumentFormatRow targetDfr =
                    documentFormatRows.FirstOrDefault(dfr => dfr.Description == contentTypeDesc);
                //don't try to construct a DocumentFormat unless we have a valid DocumentFormatRow
                if (targetDfr != null)
                {
                    retDf = new DocumentFormat(targetDfr, document);
                }
                return retDf;
            }
        }

        /// <summary>
        ///   Indexer to retrieve by content type enum
        /// </summary>
        public IDocumentFormat this[ContentType contentType]
        {
            get
            {
                DocumentFormat retDf = null;
                //should probably use a Hashtable with content type as key so we can perform lookup more quickly
                //need to retain order, however, so still need array if hashtable is used
                DocumentDs.DocumentFormatRow targetDfr =
                    documentFormatRows.FirstOrDefault(dfr => dfr.ContentTypeId == (int) contentType);
                //don't try to construct a DocumentFormat unless we have a valid DocumentFormatRow
                if (targetDfr != null)
                {
                    retDf = new DocumentFormat(targetDfr, document);
                }
                return retDf;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new DocumentFormatEnumerator(this);
        }

        #endregion

        #region Nested type: DocumentFormatEnumerator

        /// <summary>
        /// </summary>
        private class DocumentFormatEnumerator : IEnumerator
        {
            private readonly DocumentFormatCollection dfc;
            private int index;

            public DocumentFormatEnumerator(DocumentFormatCollection DocumentFormatColl)
            {
                dfc = DocumentFormatColl;
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
                get { return new DocumentFormat(dfc.documentFormatRows[index], dfc.document); }
            }

            /// <summary>
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                index++;
                return (index < dfc.documentFormatRows.Length);
            }

            #endregion
        }

        #endregion
    }
}