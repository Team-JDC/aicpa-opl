#region

using System;
using System.Linq;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Note;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    ///<summary>
    ///  Represents a document object.
    ///</summary>
    public class Document : DestroyerBpc, IDocument
    {
        #region Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_DOCUMENTNOTFOUND = "A document with the id of '{0}' was not found in the datastore.";

        #endregion Constants

        private readonly int documentId = EMPTY_INT;

        #region Properties

        #region Private

        private IDocumentAnchorCollection activeDocumentAnchorCollection;
        private IDocumentDalc activeDocumentDalc;

        private DocumentDs activeDocumentDs;
        private IDocumentFormatCollection activeDocumentFormatCollection;

        private DocumentDs.DocumentRow activeDocumentRow;
        private bool activeInSubscription = true;

        private string activeKwic;

        /// <summary>
        ///   For retrieving the active document DALC. If there is no active document DALC, this accessor will instantiate a new empty one.
        /// </summary>
        private IDocumentDalc ActiveDocumentDalc
        {
            get { return activeDocumentDalc ?? (activeDocumentDalc = new DocumentDalc()); }
        }

        /// <summary>
        ///   For retrieving the active document dataset. If there is no active document dataset, a new one will be instantiated. If the documentId
        ///   field was set, the document dataset will be pulled from the database; otherwise a new empty dataset will be created.
        ///   Note that the document information is pulled from different DALC methods depending on whether or not a book has been associated
        ///   with the document. This is because there is extra property information available if the document is retrieved within the context of a book.
        /// </summary>
        private DocumentDs ActiveDocumentDs
        {
            get
            {
                if (activeDocumentDs == null)
                {
                    //if we have a documentId then we should pull the book from the database
                    if (documentId >= 0)
                    {
                        //the way we pull the data depends on whether or not we were constructed with an IBook
                        activeDocumentDs = book == null ? ActiveDocumentDalc.GetDocument(documentId) : ActiveDocumentDalc.GetBookDocument(Book.Id, documentId);
                        activeDocumentRow = (DocumentDs.DocumentRow) activeDocumentDs.Document.Rows[0];
                    }
                        //if the DocumentDs has not been instantiated yet, just create a new empty one
                    else
                    {
                        activeDocumentDs = new DocumentDs();
                    }
                }
                return activeDocumentDs;
            }
        }

        /// <summary>
        ///   For retrieving the active document row. If there is no active document row, a new one is instantiated. If the documentId field was set,
        ///   a document dataset will be pulled from the database and our active document row will be set to its first row; otherwise a new empty dataset will be
        ///   created and a new empty document row will be added to it and made our active document row.
        /// </summary>
        private DocumentDs.DocumentRow ActiveDocumentRow
        {
            get
            {
                if (activeDocumentRow == null)
                {
                    if (documentId > -1)
                    {
                        //the way we pull the data depends on whether or not we were constructed with an IBook
                        activeDocumentDs = book == null ? ActiveDocumentDalc.GetDocument(documentId) : ActiveDocumentDalc.GetBookDocument(Book.Id, documentId);
                        if (activeDocumentDs.Document.Rows.Count > 0)
                        {
                            activeDocumentRow = (DocumentDs.DocumentRow) activeDocumentDs.Document.Rows[0];
                        }
                        else
                        {
                            throw new Exception(string.Format(ERROR_DOCUMENTNOTFOUND, documentId));
                        }
                    }
                    else
                    {
                        //create an empty document row
                        activeDocumentRow = ActiveDocumentDs.Document.AddDocumentRow(Book.Id, EMPTY_STRING, EMPTY_STRING,
                                                                                     EMPTY_STRING, EMPTY_INT, EMPTY_INT,
                                                                                     EMPTY_BOOL, EMPTY_BOOL);
                    }
                }
                return activeDocumentRow;
            }
        }

        /// <summary>
        ///   Private accessor for key words in context
        /// </summary>
        private string ActiveKwic
        {
            get { return activeKwic; }
            set { activeKwic = value; }
        }

        /// <summary>
        ///   Private accessor for in subscription
        /// </summary>
        private bool ActiveInSubscription
        {
            get { return activeInSubscription; }
            set { activeInSubscription = value; }
        }

        /// <summary>
        ///   For retrieving a collection of document anchors associated with this document.
        /// </summary>
        private IDocumentAnchorCollection ActiveDocumentAnchorCollection
        {
            get
            {
                if (activeDocumentAnchorCollection == null)
                {
                    DocumentDs.NamedAnchorRow[] namedAnchors = ActiveDocumentDalc.GetNamedAnchors(Id);
                    ActiveDocumentDs.Merge(namedAnchors);
                    //this.activeDocumentAnchorCollection = new DocumentAnchorCollection((DocumentDs.NamedAnchorRow[])this.ActiveDocumentRow.GetChildRows(DocumentDalc.DR_DOCUMENT_NAMEDANCHOR), this);
                    activeDocumentAnchorCollection = new DocumentAnchorCollection(
                        ActiveDocumentRow.GetNamedAnchorRows(), this);
                }
                return activeDocumentAnchorCollection;
            }
        }

        /// <summary>
        ///   For retrieving a collection of document formats associated with this document.
        /// </summary>
        private IDocumentFormatCollection ActiveDocumentFormatCollection
        {
            get
            {
                return activeDocumentFormatCollection ?? (activeDocumentFormatCollection = new DocumentFormatCollection(
                                                                                               (
                                                                                               DocumentDs.
                                                                                                   DocumentFormatRow[])
                                                                                               ActiveDocumentRow.
                                                                                                   GetChildRows(
                                                                                                       DocumentDalc.
                                                                                                           DR_DOCUMENT_DOCUMENTFORMAT),
                                                                                               this));
            }
        }

        #endregion Private

        #region IDocument Public

        private readonly IBook book;
        private IDocumentFormat primaryFormat;

        /// <summary>
        ///   The book under which the document resides
        /// </summary>
        public IBook Book
        {
            get
            {
                IBook retBook = book ?? new Book.Book(ActiveDocumentRow.BookInstanceId);
                return retBook;
            }
        }

        /// <summary>
        ///   The document's internal Id. This value is set by the system and is read-only.
        /// </summary>
        public int Id
        {
            get { return ActiveDocumentRow.DocumentInstanceId; }
        }

        /// <summary>
        ///   The document's name. This is a short identifier (e.g. 'pc_170', 'personal_financial_planning', 'aag-air_app_d') for the document.
        /// </summary>
        public string Name
        {
            get { return ActiveDocumentRow.Name; }
        }

        /// <summary>
        ///   The document's title. This is a descriptive name (e.g. 'Conforming Amendments to PCAOB Interim Standards...', 'Personal Financial Planning', 'Appendix D') for the book.
        /// </summary>
        public string Title
        {
            get { return ActiveDocumentRow.Title; }
        }

        /// <summary>
        ///   The user note associated with the document.
        /// </summary>
        public INote Note
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
            set { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   The title path of the document down to its containing book, including both the book title and the document title. If the document is
        ///   not retrieved within the context of a book, the value of this property is null.
        /// </summary>
        public string BookReferencePath
        {
            get { return ReferencePathToXml(ActiveDocumentRow.BookTitlePath); }
        }

        /// <summary>
        ///   The title path of the document down to its containing site, including both the site title and the document title. If the document is
        ///   not retrieved within the context of a site, the value of this property an empty string.
        /// </summary>
        public string SiteReferencePath
        {
            get
            {
                string siteRefPath = EMPTY_STRING;
                if (Book != null)
                {
                    siteRefPath = CombineReferencePaths(Book.ReferencePath, BookReferencePath);
                }
                return siteRefPath;
            }
        }

        /// <summary>
        ///   A collection of document anchors associated with the document.
        /// </summary>
        public IDocumentAnchorCollection Anchors
        {
            get { return ActiveDocumentAnchorCollection; }
            set { activeDocumentAnchorCollection = value; }
        }

        /// <summary>
        ///   The document formats associated with the document.
        /// </summary>
        public IDocumentFormatCollection Formats
        {
            get { return ActiveDocumentFormatCollection; }
            set { activeDocumentFormatCollection = value; }
        }

        /// <summary>
        ///   The primary format of the document.
        /// </summary>
        public IDocumentFormat PrimaryFormat
        {
            get
            {
                if (primaryFormat == null)
                {
                    //grab the corresponding document format object from the document
                    foreach (IDocumentFormat format in
                        ActiveDocumentFormatCollection.Cast<IDocumentFormat>().Where(format => format.IsPrimary))
                    {
                        primaryFormat = format;
                        break;
                    }
                }

                //we just want to return a null docFormat if that is what we get back...	
                return primaryFormat;
            }
        }

        ///<summary>
        ///  A string that represents a snippet of text from the document that shows the hit keywords in context. This property is used within the context of search results.
        ///</summary>
        public string KeyWordsInContext
        {
            get { return ActiveKwic; }
            set { ActiveKwic = value; }
        }

        ///<summary>
        ///  A flag that is used to indicate whether or not the book was pulled from the user's subscription. This property is used within the context of search results.
        ///</summary>
        public bool InSubscription
        {
            get { return ActiveInSubscription; }
            set { ActiveInSubscription = value; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node id
        /// </summary>
        public int NodeId
        {
            get { return Id; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node type
        /// </summary>
        public NodeType NodeType
        {
            get { return NodeType.Document; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node right value
        /// </summary>
        public int Right
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node left value
        /// </summary>
        public int Left
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicating whether or not the toc node has children nodes
        /// </summary>
        public bool HasChildren
        {
            get { throw new Exception(ERROR_NOTIMPLEMENTED); }
        }

        /// <summary>
        ///   ITocNode interface property indicates whether or not the node should be hidden in the toc
        /// </summary>
        public bool Hidden
        {
            get { return false; }
        }

        /// <summary>
        ///   ITocNode interface property returns the uri of the node
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        #endregion IDocument Public

        #region IPrimaryContentContainer Properties

        /// <summary>
        /// Gets the content of the primary.	
        /// </summary>
        /// <value>The content of the primary.</value>
        /// <remarks></remarks>
        public ContentWrapper PrimaryContent
        {
            get {
                if (PrimaryFormat == null)
                {
                    string docName = this.Name;
                    string[] docSplit = docName.Split(DestroyerBpc.EXTERNAL_DOC_DATA_SEPERATOR);
                    if (docSplit.Length > 1 && docSplit[0].StartsWith(DestroyerBpc.EXTERNAL_DOCUMENT))
                    {
                        string targetDoc = docSplit[1];
                        string targetPointer = docSplit[2];
                        
                        return new ContentWrapper(targetDoc, targetPointer);
                    }
                }
                return new ContentWrapper(this);
            }
        }

        #endregion

        #endregion Properties

        #region Constructors

        ///<summary>
        ///  Creates a new Document object with no data.  Data can be added via
        ///  the Document properties, and then saved.
        ///</summary>
        public Document()
        {
        }

        /// <summary>
        ///   Retrieves the specified Document object from the database.
        /// </summary>
        /// <param name = "documentId">The id of the document to retrieve from the database.</param>
        public Document(int documentId)
        {
            this.documentId = documentId;
        }

        /// <summary>
        ///   Retrieves the specified Document object from the database.
        /// </summary>
        /// <param name = "book">The context book</param>
        /// <param name = "documentId">The id of the document to retrieve from the database.</param>
        public Document(IBook book, int documentId)
        {
            this.documentId = documentId;
            this.book = book;
        }

        ///<summary>
        ///  Creates a new Document object using a dataset provided by a DocumentCollection.
        ///  Note that the access visibility is "internal."  This constructor is intended for
        ///  use only by DocumentCollection; users of this API can't use this constructor 
        ///  themselves.  Note also that this object's dataset will be a reference to the
        ///  dataset in the DocumentCollection that spawned it - it doesn't "belong" to this Document
        ///  object directly in this case.
        ///</summary>
        ///<param name = "book"></param>
        ///<param name = "documentRow"></param>
        internal Document(IBook book, DocumentDs.DocumentRow documentRow)
        {
            this.book = book;
            activeDocumentRow = documentRow;
            activeDocumentDs = (DocumentDs) activeDocumentRow.Table.DataSet;
        }

        #endregion Constructors

        #region Methods

        #region IDocument (Public)

        /// <summary>
        ///   Quick retrieval for document anchor by name
        /// </summary>
        /// <param name = "documentAnchorName">Anchor name to retrieve</param>
        /// <returns></returns>
        public IDocumentAnchor GetDocumentAnchor(string documentAnchorName)
        {
            IDocumentAnchor retAnchor = null;
            DocumentDs.NamedAnchorRow namedAnchorRow = ActiveDocumentDalc.GetNamedAnchor(Id, documentAnchorName);
            if (namedAnchorRow != null)
            {
                retAnchor = new DocumentAnchor(namedAnchorRow, this);
            }
            return retAnchor;
        }

        /// <summary>
        ///   Saves the current document.
        /// </summary>
        public void Save()
        {
            throw new Exception(ERROR_NOTIMPLEMENTED);
        }

        public bool hasPreviousDocument()
        {
            IDocument prevDoc = Book.GetPreviousDocument(this);
            return prevDoc != null;
        }

        public bool hasNextDocument()
        {
            IDocument nextDoc = Book.GetNextDocument(this);
            return nextDoc != null;
        }

        #endregion IDocument (Public)

        #endregion Methods
    }
}