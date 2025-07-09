#region

using System;
using AICPA.Destroyer.Shared;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Site;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for DocumentAnchor.
    /// </summary>
    public class DocumentAnchor : DestroyerBpc, IDocumentAnchor
    {
        #region Private

        private readonly int namedAnchorId;
        private DocumentDalc activeDocumentDalc;
        private DocumentDs.NamedAnchorRow activeNamedAnchorRow;

        private DocumentDalc ActiveDocumentDalc
        {
            get { return activeDocumentDalc ?? (activeDocumentDalc = new DocumentDalc()); }
        }

        private DocumentDs.NamedAnchorRow ActiveNamedAnchorRow
        {
            get { return activeNamedAnchorRow ?? (activeNamedAnchorRow = ActiveDocumentDalc.GetNamedAnchor(namedAnchorId)); }
        }

        private IDocument ResolveDocumentByDocumentAchorId(ISite site, int id)
        {
            IDocument document = null;
            IBook book = null;

            int bookId = -1;
            int docId = -1;
            string xml = BookReferencePath;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            bookId = Int32.Parse(xmlDoc.SelectSingleNode("//Book").Attributes.GetNamedItem("Id").Value);

            XmlNode xmlNode = xmlDoc.SelectSingleNode("//DocumentAnchor[@Id='" + id + "']");

            while (xmlNode.LocalName != "Document")
            {
                xmlNode = xmlNode.PreviousSibling;
            }

            docId = Int32.Parse(xmlNode.Attributes.GetNamedItem("Id").Value);

            if (site == null)
            {
                book = new Book.Book(bookId);
            }
            else
            {
                book = new Book.Book(site, bookId);
            }

            document = new Document(book, docId);

            return document;
        }

        #endregion

        /// <summary>
        /// </summary>
        private readonly IDocument document;

        /// <summary>
        /// Instantiate a DocumentAnchor using only the anchor id. This constructor is used from within the context of a Toc.
        /// </summary>
        /// <param name="namedAnchorId">The id of the named anchor</param>
        public DocumentAnchor(int namedAnchorId)
        {
            this.namedAnchorId = namedAnchorId;
            this.document = ResolveDocumentByDocumentAchorId(null, namedAnchorId);
        }

        /// <summary>
        /// Instantiate a DocumentAnchor using only the anchor id. This constructor is used from within the context of a Toc.
        /// </summary>
        /// <param name="namedAnchorId">The id of the named anchor</param>
        public DocumentAnchor(ISite site, int namedAnchorId)
        {
            this.namedAnchorId = namedAnchorId;
            this.document = ResolveDocumentByDocumentAchorId(site, namedAnchorId);
        }

        /// <summary>
        ///   Instantiate a DocumentAnchor using only the anchor id. This constructor is used from within the context of a Toc.
        /// </summary>
        /// <param name = "namedAnchorId">The id of the named anchor</param>
        public DocumentAnchor(int namedAnchorId, IDocument document)
        {
            this.document = document;
            this.namedAnchorId = namedAnchorId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnchor" /> class.	
        /// </summary>
        /// <param name="namedAnchorRow">The named anchor row.</param>
        /// <param name="document">The document.</param>
        /// <remarks></remarks>
        public DocumentAnchor(DocumentDs.NamedAnchorRow namedAnchorRow, IDocument document)
        {
            this.document = document;
            activeNamedAnchorRow = namedAnchorRow;
        }

        #region IDocumentAnchor Members

        /// <summary>
        /// Gets the document.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IDocument Document
        {
            get { return document; }
        }

        /// <summary>
        /// </summary>
        public int Id
        {
            get { return ActiveNamedAnchorRow.NamedAnchorId; }
        }

        /// <summary>
        /// </summary>
        public string Name
        {
            get { return ActiveNamedAnchorRow.Name; }
        }

        /// <summary>
        /// </summary>
        public string Title
        {
            get { return ActiveNamedAnchorRow.Title; }
        }

        /// <summary>
        /// </summary>
        public string BookReferencePath
        {
            get { return ReferencePathToXml(ActiveNamedAnchorRow.BookTitlePath); }
        }

        /// <summary>
        /// </summary>
        public string SiteReferencePath
        {
            get
            {
                string siteRefPath = string.Empty;
                if (Document.Book != null)
                {
                    siteRefPath = CombineReferencePaths(Document.Book.ReferencePath, BookReferencePath);
                }
                return siteRefPath;
            }
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
            get { return NodeType.DocumentAnchor; }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node right value
        /// </summary>
        public int Right
        {
            get { throw new Exception("Not implemented."); }
        }

        /// <summary>
        ///   ITocNode interface property indicating the objects node left value
        /// </summary>
        public int Left
        {
            get { throw new Exception("Not implemented."); }
        }

        /// <summary>
        ///   ITocNode interface property indicating whether or not the toc node has children nodes
        /// </summary>
        public bool HasChildren
        {
            get { throw new Exception("Not implemented."); }
        }

        /// <summary>
        ///   ITocNode interface property indicates whether or not the node should be hidden in the toc
        /// </summary>
        public bool Hidden
        {
            get { throw new Exception("Not implemented."); }
        }

        /// <summary>
        ///   ITocNode interface property indicates whether or not the node should be hidden in the toc
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        #endregion

        #region IPrimaryContentContainer Properties

        /// <summary>
        /// Gets the content of the primary.	
        /// </summary>
        /// <value>The content of the primary.</value>
        /// <remarks></remarks>
        public ContentWrapper PrimaryContent
        {
            get { return new ContentWrapper(this.document, this.Name); }
        }

        #endregion
    }
}