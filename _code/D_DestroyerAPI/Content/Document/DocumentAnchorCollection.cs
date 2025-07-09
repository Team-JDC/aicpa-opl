#region

using System.Collections;
using System.Linq;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for DocumentAnchorCollection.
    /// </summary>
    public class DocumentAnchorCollection : DestroyerBpc, IDocumentAnchorCollection, IEnumerable
    {
        private readonly IDocument document;
        private readonly DocumentDs.NamedAnchorRow[] namedAnchorRows;


        private IDocumentDalc activeDocumentDalc;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnchorCollection" /> class.	
        /// </summary>
        /// <param name="namedAnchorRows">The named anchor rows.</param>
        /// <param name="document">The document.</param>
        /// <remarks></remarks>
        public DocumentAnchorCollection(DocumentDs.NamedAnchorRow[] namedAnchorRows, IDocument document)
        {
            this.document = document;
            this.namedAnchorRows = namedAnchorRows;
        }

        /// <summary>
        ///   For retrieving the active document DALC. If there is no active document DALC, this accessor will instantiate a new empty one.
        /// </summary>
        private IDocumentDalc ActiveDocumentDalc
        {
            get { return activeDocumentDalc ?? (activeDocumentDalc = new DocumentDalc()); }
        }

        #region IDocumentAnchorCollection Members

        /// <summary>
        /// Gets the count.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Count
        {
            get { return namedAnchorRows.Length; }
        }

        /// <summary>
        ///   Retrieves the document anchor by ordinal.
        /// </summary>
        public IDocumentAnchor this[int index]
        {
            get { return new DocumentAnchor(namedAnchorRows[index], document); }
        }

        /// <summary>
        ///   Retrieves the document anchor by name.
        /// </summary>
        public IDocumentAnchor this[string name]
        {
            get
            {
                DocumentAnchor retDa = null;
                //todo: this lookup is terrible...need to handle this indexer in a better way
                DocumentDs.NamedAnchorRow targetDna = namedAnchorRows.FirstOrDefault(dna => dna.Name == name);
                //don't try to construct a DocumentAnchor unless we have a valid NamedAnchorRow
                if (targetDna != null)
                {
                    retDa = new DocumentAnchor(targetDna, document);
                }
                return retDa;
            }
        }

        /// <summary>
        /// Gets the enumerator.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            return new DocumentAnchorEnumerator(this, document);
        }

        #endregion

        #region Nested type: DocumentAnchorEnumerator

        private class DocumentAnchorEnumerator : IEnumerator
        {
            private readonly DocumentAnchorCollection dac;
            private readonly IDocument document;
            private int index;

            public DocumentAnchorEnumerator(DocumentAnchorCollection DocumentAnchorColl, IDocument document)
            {
                this.document = document;
                dac = DocumentAnchorColl;
                Reset();
            }

            #region IEnumerator Members

            public void Reset()
            {
                index = -1;
            }

            public object Current
            {
                get { return new DocumentAnchor(dac.namedAnchorRows[index], document); }
            }

            public bool MoveNext()
            {
                index++;
                return (index < dac.namedAnchorRows.Length);
            }

            #endregion
        }

        #endregion
    }
}