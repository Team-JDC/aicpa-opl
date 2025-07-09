using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;

namespace AICPA.Destroyer.Content.Book
{
    ///<summary>
    ///  A class for creating and accessing information about a book.
    ///</summary>
    public class Book : DestroyerBpc, IBook
    {
        private int bookId = EMPTY_INT;

        #region Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKMAKEFILENOTFOUND =
            "Error building book. The makefile '{0}' associated with book '{1}' does not exist.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKMAKEFILEINVALIDXML =
            "There was a problem loading the makefile '{0}' for book '{1}'. Make sure that the makefile is well formed XML. XML parser error: {2}";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_DOCUMENTFORMATURINOTFOUND =
            "Document Format file not found while building book. The file '{0}', referenced in the makefile of book '{1}', does not exist.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKRESOURCENOTFOUND =
            "Book Resource file not found while building book. The file '{0}', referenced in the makefile of book '{1}', does not exist.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKBUILDSOURCEURINOTSPECIFIED =
            "SourceType and SourceUri properties must be provided before building a book.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKBUILDBOOKISBUILDING =
            "A build cannot be started for the book because it is already building.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKBUILDSOURCETYPENOTIMPLEMENTED =
            "The book source type of '{0}' has not yet been implemented.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKNOTEDITABLE =
            "Performing this action or modifying this property is not allowed when the book is associated with a production site.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string BOOK_CONTENTFOLDER_KEY = "Book_ContentFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string BOOK_RESOURCEFOLDER_KEY = "Book_ResourceFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string BOOK_MAKEFILESCHEMAPATH_KEY = "Book_MakefileSchemaPath";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string BOOK_CONTENTFOLDER = ConfigurationSettings.AppSettings[BOOK_CONTENTFOLDER_KEY];
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string BOOK_RESOURCEFOLDER = ConfigurationSettings.AppSettings[BOOK_RESOURCEFOLDER_KEY];
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string BOOK_MAKEFILESCHEMAPATH = ConfigurationSettings.AppSettings[BOOK_MAKEFILESCHEMAPATH_KEY];
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string BOOK_FORMATSFOLDERSUFFIX = "_Formats";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static int BOOK_INITIALVERSION;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ALTERNATEBOOK_SUFFIX = "_fallback";

        #endregion

        #region Constructors

        #region Public Constructors

        /// <summary>
        ///   Retrieves the specified book from the database.
        /// </summary>
        /// <param name = "bookId">The id of the book to retrieve from the database.</param>
        public Book(int bookId)
        {
            this.bookId = bookId;
        }

        /// <summary>
        ///   Retrieves the specified book from the database, within the context of a book.
        /// </summary>
        /// <param name = "site">The context site for the book retrieval.</param>
        /// <param name = "bookId">The id of the book to retrieve from the database.</param>
        public Book(ISite site, int bookId)
        {
            this.bookId = bookId;
            this.site = site;
        }

        /// <summary>
        ///   Creates a new book object. After instantiation you may set the properties of the book as needed.
        /// </summary>
        public Book()
        {
        }

        /// <summary>
        ///   Creates a new book object with the specified properties
        /// </summary>
        /// <param name = "bookName">The name of the book</param>
        /// <param name = "bookTitle">The title of the book</param>
        /// <param name = "bookDesc">The description of the book</param>
        /// <param name = "bookCopyright">The copyright of the book</param>
        /// <param name = "bookSourceType">The source type of the book</param>
        /// <param name = "bookSourceUri">The source URI of the book</param>
        public Book(string bookName, string bookTitle, string bookDesc, string bookCopyright,
                    BookSourceType bookSourceType, string bookSourceUri)
        {
            ActiveBookRow.Name = bookName;
            ActiveBookRow.Title = bookTitle;
            ActiveBookRow.Description = bookDesc;
            ActiveBookRow.Copyright = bookCopyright;
            ActiveBookRow.SourceTypeId = (int) bookSourceType;
            ActiveBookRow.SourceUri = bookSourceUri;
        }

        #endregion Public Constructors

        #region Internal Constructors

        ///<summary>
        ///  Creates a new Book object using a datarow provided by a BookCollection.
        ///  Note that the access visibility is "internal."  This constructor is intended for
        ///  use only by BookCollection; users of this API can't use this constructor 
        ///  themselves.  Note also that this object's dataset will be a reference to the
        ///  dataset in the BookCollection that spawned it - it doesn't "belong" to this Book
        ///  object directly in this case.
        ///</summary>
        ///<param name = "bookRow"></param>
        internal Book(ISite site, BookDs.BookRow bookRow)
        {
            this.site = site;
            activeBookRow = bookRow;
            activeBookDs = (BookDs) activeBookRow.Table.DataSet;
        }

        #endregion Internal Constructors

        #endregion

        #region Properties

        #region IBook Properties

        private ISite site;

        /// <summary>
        ///   The site under which the book resides. Will be null if the book was retrieved outside of the context of a site.
        /// </summary>
        public ISite Site
        {
            get { return site; }
            set { site = value; }
        }

        /// <summary>
        ///   The sites under which the book resides.
        /// </summary>
        public ISiteCollection Sites
        {
            get { return ActiveSiteCollection; }
        }

        /// <summary>
        ///   The book's internal Id. This value is set by the system and is read-only.
        /// </summary>
        public int Id
        {
            get { return ActiveBookRow.BookInstanceId; }
        }

        /// <summary>
        ///   The book's version. This property is set by the system and is read-only.
        /// </summary>
        public int Version
        {
            get { return ActiveBookRow.BookVersion; }
        }

        /// <summary>
        ///   The book's name. This is a short identifier (e.g. 'ps', 'tpa', 'aag-air') for the book.
        /// </summary>
        public string Name
        {
            get { return ActiveBookRow.Name; }
            set
            {
                if (IsEditable)
                {
                    ActiveBookRow.Name = value;
                }
                else
                {
                    throw new Exception(ERROR_BOOKNOTEDITABLE);
                }
            }
        }

        /// <summary>
        ///   The book's title. This is a descriptive name (e.g. 'Professional Standards', 'Technical Practice Aids') for the book.
        /// </summary>
        public string Title
        {
            get { return ActiveBookRow.Title; }
            set { ActiveBookRow.Title = value; }
        }

        /// <summary>
        ///   The book's description. This is a paragraph-length description of the book.
        /// </summary>
        public string Description
        {
            get { return ActiveBookRow.Description; }
            set { ActiveBookRow.Description = value; }
        }

        /// <summary>
        ///   A collection of documents contained in the book. The documents are contained in this collection in content order (the order in which they are represented in the book table of contents).
        /// </summary>
        public IDocumentCollection Documents
        {
            get { return ActiveDocumentCollection; }
            set { activeDocumentCollection = value; }
        }

        /// <summary>
        ///   The copyright text for the book (e.g. "Copyright 2005, American Institute of Certified Public Accountants. All Rights Reserved.').
        /// </summary>
        public string Copyright
        {
            get
            {
                return ReplaceCopyrightCurrentYear(ActiveBookRow.Copyright);
            }
            set { ActiveBookRow.Copyright = value; }
        }

        /// <summary>
        /// </summary>
        public bool Archived
        {
            get { return ActiveBookRow.Archived; }
            set { ActiveBookRow.Archived = value; }
        }

        /// <summary>
        ///   The date the book was built.
        /// </summary>
        public DateTime PublishDate
        {
            get { return ActiveBookRow.PublishDate; }
            set { ActiveBookRow.PublishDate = value; }
        }

        /// <summary>
        ///   The title path of the book as it is contained in the site. If the book was not retrieved through the context of a site, this property will be null.
        /// </summary>
        public string ReferencePath
        {
            get { return ReferencePathToXml(ActiveBookRow.SiteTitlePath); }
        }

        /// <summary>
        ///   The source type for the book. The value of this property determines how the book is constructed from the SourceUri property.
        ///   SourceType = BookSourceType.Makefile: The book will be built using a file system makefile referenced in the SourceUri property.
        ///   SourceType = BookSourceType.Cms: The book will be built using a Proflit XML document specified as a Documentum locator referenced in the SourceUri property.
        /// </summary>
        public BookSourceType SourceType
        {
            get { return (BookSourceType) ActiveBookRow.SourceTypeId; }
            set { ActiveBookRow.SourceTypeId = (int) value; }
        }

        /// <summary>
        ///   The source URI for the book. The value of this property depends on the SourceType property.
        ///   SourceType = BookSourceType.Makefile: The book will be built using a file system makefile referenced in the SourceUri property.
        ///   /// SourceType = BookSourceType.Cms: The book will be built using a Proflit XML document specified as a Documentum locator referenced in the SourceUri property.
        /// </summary>
        public string SourceUri
        {
            get { return ActiveBookRow.SourceUri; }
            set { ActiveBookRow.SourceUri = value; }
        }

        /// <summary>
        ///   Xml string describing the structure of the book.
        /// </summary>
        public string BookXml
        {
            get { return ActiveBookXml; }
        }

        /// <summary>
        ///   Indicates whether or not the book should be edited in its current state
        /// </summary>
        public bool IsEditable
        {
            get { return ActiveBookRow.IsEditable; }
        }

        /// <summary>
        ///   Indicates whether or not the book has pending changes that need to be committed
        /// </summary>
        public bool HasChanges
        {
            get { return ActiveBookDs.HasChanges(); }
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
            get { return NodeType.Book; }
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
        ///   ITocNode interface property returns the Uri for the book
        /// </summary>
        public string Uri
        {
            get { return string.Empty; }
        }

        /// <summary>
        ///   Retrieve or set the build status of a book.
        ///   A setter is available if, on the presentation level, you wish to add a request for a build but handle the build later in an offline process.
        ///   When this technique is used, the client is required to call Build() on all flagged books, as a build will not automatically take place.
        /// </summary>
        public BookBuildStatus BuildStatus
        {
            get { return ActiveBuildStatus; }
            set { ActiveBuildStatus = value; }
        }

        #endregion IBook Properties

        #region IPrimaryContentContainer Properties

        /// <summary>
        /// Gets the content of the primary.	
        /// </summary>
        /// <value>The content of the primary.</value>
        /// <remarks></remarks>
        public ContentWrapper PrimaryContent
        {
            get { return new ContentWrapper(Documents[0]); }
        }

        #endregion

        #region Private Properties

        private IBookDalc activeBookDalc;
        private BookDs activeBookDs;
        private BookDs.BookRow activeBookRow;
        private IBookTocNodeCollection activeBookTocNodeCollection;
        private string activeBookXml;
        private IDocumentCollection activeDocumentCollection;

        private IDocumentDalc activeDocumentDalc;
        private ISiteCollection activeSiteCollection;

        /// <summary>
        ///   For retrieving the active book DALC. If there is no active book DALC, this accessor will instantiate a new empty one.
        /// </summary>
        private IBookDalc ActiveBookDalc
        {
            get { return activeBookDalc ?? (activeBookDalc = new BookDalc()); }
        }

        /// <summary>
        ///   For retrieving the active document DALC. If there is no active document DALC, this accessor will instantiate a new empty one.
        /// </summary>
        private IDocumentDalc ActiveDocumentDalc
        {
            get { return activeDocumentDalc ?? (activeDocumentDalc = new DocumentDalc()); }
        }

        /// <summary>
        ///   For retrieving the active book dataset. If there is no active book dataset, a new one will be instantiated. If the bookId field was set,
        ///   the book dataset will be pulled from the database; otherwise a new empty dataset will be created.
        ///   Note that the book information is pulled from different DALC methods depending on whether or not a site has been associated with the
        ///   book. This is because there is extra property information available if the book is retrieved within the context of a site.
        /// </summary>
        private BookDs ActiveBookDs
        {
            get
            {
                if (activeBookDs == null)
                {
                    //if we have a bookid then we should pull the book from the database
                    if (bookId >= 0)
                    {
                        //the way we pull the data depends on whether or not we were constructed with an ISite
                        activeBookDs = Site == null
                                           ? ActiveBookDalc.GetBook(bookId)
                                           : ActiveBookDalc.GetBook(Site.Id, bookId);
                        activeBookRow = (BookDs.BookRow) activeBookDs.Book.Rows[0];
                    }
                        //if the BookDs has not been instantiated yet; just create a new empty one
                    else
                    {
                        activeBookDs = new BookDs();
                    }
                }
                return activeBookDs;
            }
        }

        /// <summary>
        ///   For retrieving the active book row. If there is no active book row, a new one is instantiated. If the bookId field was set,
        ///   a book dataset will be pulled from the database and our active book row will be set to its first row; otherwise a new empty dataset will be
        ///   created and a new empty book row will be added to it and made our active book row.
        /// </summary>
        private BookDs.BookRow ActiveBookRow
        {
            get
            {
                if (activeBookRow == null)
                {
                    if (bookId > -1)
                    {
                        //the way we pull the data depends on whether or not we were constructed with an ISite
                        activeBookDs = Site == null
                                           ? ActiveBookDalc.GetBook(bookId)
                                           : ActiveBookDalc.GetBook(Site.Id, bookId);
                        activeBookRow = (BookDs.BookRow) activeBookDs.Book.Rows[0];
                    }
                    else
                    {
                        //create an empty book row
                        activeBookRow = ActiveBookDs.Book.AddBookRow(string.Empty, BOOK_INITIALVERSION, string.Empty,
                                                                     string.Empty, DateTime.Now, string.Empty,
                                                                     (int) BookSourceType.Makefile, string.Empty,
                                                                     string.Empty, EMPTY_INT, EMPTY_INT, EMPTY_BOOL,
                                                                     true, EMPTY_BOOL, (int) BookBuildStatus.NotBuilt);
                    }
                }
                return activeBookRow;
            }
        }

        private ISiteCollection ActiveSiteCollection
        {
            get { return activeSiteCollection ?? (activeSiteCollection = new SiteCollection(this)); }
        }

        /// <summary>
        ///   For retrieving a collection of documents associated with this book.
        /// </summary>
        private IDocumentCollection ActiveDocumentCollection
        {
            get { return activeDocumentCollection ?? (activeDocumentCollection = new DocumentCollection(this)); }
        }

        /// <summary>
        ///   For retrieving a collection of book table of contents nodes associated with this book
        /// </summary>
        private IBookTocNodeCollection ActiveBookTocNodeCollection
        {
            get { return activeBookTocNodeCollection ?? (activeBookTocNodeCollection = new BookTocNodeCollection(this)); }
        }

        private BookBuildStatus ActiveBuildStatus
        {
            get { return (BookBuildStatus) ActiveBookRow.BuildStatusCode; }
            set { ActiveBookRow.BuildStatusCode = (int) value; }
        }

        /// <summary>
        /// </summary>
        private string ActiveBookXml
        {
            get
            {
                if (activeBookXml == null)
                {
                    //Log.LogEvent(EventType.Info, 1, "Book", "ActiveBookXml", "Debug", string.Format("1. Current Book:{0} BookId:{1}", this.Title, this.Id));
                    string tempXml = GetXmlFromTocNodes(ActiveBookTocNodeCollection, true);
                    //Log.LogEvent(EventType.Info, 1, "Book", "ActiveBookXml", "Debug", string.Format("2. Current Book:{0} BookXML:{1}", this.Title, tempXml));
                    if (tempXml != string.Empty)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(tempXml);
                        XmlNodeList nodes = xmlDoc.SelectNodes("//" + XML_ELE_DOCUMENT);
                        foreach (XmlNode node in nodes)
                        {
                            string docName = node.Attributes[XML_ATT_DOCUMENTNAME].Value;
                            IDocumentFormatCollection formats = Documents[docName].Formats;
                            foreach (IDocumentFormat format in formats)
                            {
                                //create a document format element with its attributes
                                XmlElement ele = xmlDoc.CreateElement(XML_ELE_DOCUMENTFORMAT);

                                XmlAttribute contentTypeAtt = xmlDoc.CreateAttribute(XML_ATT_DOCUMENTFORMATCONTENTTYPE);
                                contentTypeAtt.Value = format.Description;
                                ele.Attributes.Append(contentTypeAtt);

                                XmlAttribute uriAtt = xmlDoc.CreateAttribute(XML_ATT_DOCUMENTFORMATURI);
                                uriAtt.Value = format.Uri;
                                ele.Attributes.Append(uriAtt);

                                XmlAttribute priAtt = xmlDoc.CreateAttribute(XML_ATT_DOCUMENTFORMATPRI);
                                priAtt.Value = format.IsPrimary.ToString();
                                ele.Attributes.Append(priAtt);

                                //pop the new element under the current document node
                                node.AppendChild(ele);
                            }
                        }
                        activeBookXml = xmlDoc.OuterXml;
                    }
                    else
                    {
                        activeBookXml = string.Empty;
                    }
                }
                return activeBookXml;
            }
        }

        #endregion Private Properties

        #endregion Properties

        #region Methods

        #region Static Helper Methods

        /// <summary>
        /// Gets the name of the alternate book.	
        /// </summary>
        /// <param name="books">The books.</param>
        /// <param name="bookName">Name of the book.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetAlternateBookName(IBookCollection books, string bookName)
        {
            //the book name to return
            string retBookName = string.Empty;

            if (books.Count > 0)
            {
                ArrayList alternateBooks = new ArrayList();
                foreach (IBook book in
                    books.Cast<IBook>().Where(book => book.Name.StartsWith(bookName + ALTERNATEBOOK_SUFFIX)))
                {
                    alternateBooks.Add(book.Name);
                }

                if (alternateBooks.Count > 0)
                {
                    //sort in alpha order
                    alternateBooks.Sort();

                    //set our return book, the first book by alpha order
                    retBookName = (string) alternateBooks[0];
                }
            }
            //logic added to resolve ara-links to aam if the user has access to aam but not aras
            //also logic to make ras a fallback if the currentbook is 
            if (retBookName == string.Empty)
            {
                if (bookName.StartsWith("ara-"))
                {
                    if (books.Cast<IBook>().Any(book => book.Name == "aam"))
                    {
                        retBookName = "aam";
                    }
                }
                if (bookName == "ps")
                {
                    if (books.Cast<IBook>().Any(book => book.Name == "ras"))
                    {
                        retBookName = "ras";
                    }
                }
            }

            //return the book name
            return retBookName;
        }

        public static bool IsAICPABookWithLicenseAgreement(IBook book)
        {
            return book.Name.IndexOf("fasb") > -1 ||
                   book.Name.IndexOf("faf") > -1 ||
                   book.Name.IndexOf("asc") > -1;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Private helper method for building a book from a makefile
        /// </summary>
        /// <returns>Boolean value determining success or failure</returns>
        private void BuildBookFromMakefile()
        {
            string makeFile = SourceUri;

            //make sure the book is editable
            if (!IsEditable)
            {
                throw new Exception(ERROR_BOOKNOTEDITABLE);
            }

            //make sure a source uri was specified
            if (SourceUri == null)
            {
                throw new Exception(ERROR_BOOKBUILDSOURCEURINOTSPECIFIED);
            }

            //make sure the makefile exists
            if (!File.Exists(makeFile))
            {
                throw new FileNotFoundException(string.Format(ERROR_BOOKMAKEFILENOTFOUND, makeFile, Name));
            }

            //grab the directory containing the makefile so we can use it to resolve relative URIs in the makefile
            string makeFileDirectory = Path.GetDirectoryName(makeFile);

            //load the makefile into a dom document
            XmlDocument makeXml = new XmlDocument();
            try
            {
                //jjs 8/23: spent half a day trying to get this validation working with entities with no luck...so no validation for the time being
//				XmlSchemaCollection xsc = new XmlSchemaCollection();
//				xsc.Add(null,new XmlTextReader(BOOK_MAKEFILESCHEMAPATH));
//				XmlValidatingReader rdr = new XmlValidatingReader(File.OpenRead(makeFile), XmlNodeType.Document, null);
//				rdr.ValidationType = ValidationType.None;
//				rdr.XmlResolver = new XmlUrlResolver();
//				rdr.Schemas.Add(xsc);
//				rdr.ValidationType = ValidationType.Schema;
//				rdr.EntityHandling = EntityHandling.ExpandEntities;
//				makeXml.Load(rdr);
                makeXml.Load(makeFile);
            }
            catch (Exception e)
            {
                throw new XmlException(string.Format(ERROR_BOOKMAKEFILEINVALIDXML, makeFile, Name, e.Message));
            }

            //process all makefile elements
            XmlNodeList nodes = makeXml.SelectNodes("//*");

            //create a new document dataset
            DocumentDs documentDs = new DocumentDs();

            //create a new book folder dataset
            BookFolderDs bookFolderDs = new BookFolderDs();

            //create a new book toc node dataset
            BookTocNodeDs bookTocNodeDs = new BookTocNodeDs();

            //base folder for storing book content
            string bookSubFolder = Name + "." + Version;
            string bookFolder = Path.Combine(BOOK_CONTENTFOLDER, bookSubFolder);
            string bookResourceFolder = Path.Combine(bookFolder, BOOK_RESOURCEFOLDER);

            int docCount = 0;
            int externalCount = 0;
            foreach (XmlNode node in nodes)
            {
                DateTime dateAdded = DateTime.Now;
                string nodeName = node.LocalName;

                switch (nodeName)
                {
                    case XML_ELE_MAKEFILE:
                        break;
                    case XML_ELE_BOOK:
                        //create a folder to house our document content for this book
                        if (Directory.Exists(bookFolder))
                        {
                            Directory.Delete(bookFolder, true);
                        }
                        Directory.CreateDirectory(bookFolder);

                        //create a folder to house our book resources
                        if (Directory.Exists(bookResourceFolder))
                        {
                            Directory.Delete(bookResourceFolder, true);
                        }
                        Directory.CreateDirectory(bookResourceFolder);

                        //get the properties for the book dataset ready
                        int bookTocLeft = GetNodeLeftValue(node);
                        int bookTocRight = GetNodeRightValue(node, bookTocLeft);
                        bool bookHasChildren = !(bookTocLeft == bookTocRight - 1);
                        SourceType = BookSourceType.Makefile;
                        SourceUri = makeFile;

                        //save our Book dataset right away, since the Document dataset will need to be populated with the returned BookId identity value
                        ActiveBookDalc.Save(ActiveBookDs);
                        bookId = ActiveBookRow.BookInstanceId;

                        //add our book's information to the booktocnode table
                        bookTocNodeDs.BookTocNode.AddBookTocNodeRow(Id, Id, bookTocLeft, bookTocRight, (byte) NodeType,
                                                                    NodeType.ToString(), Name, Title, bookHasChildren,
                                                                    false);

                        break;
                    case XML_ELE_DOCUMENT:
                        //check to see if the document is external
                        string externalAttribValue = GetAttributeValue(node.Attributes[XML_ATT_EXTERNAL]);
                        bool isExternal = bool.Parse(externalAttribValue == "" ? "false" : externalAttribValue);

                        //get the properties for the document dataset ready
                        string docName = GetAttributeValue(node.Attributes[XML_ATT_DOCUMENTNAME]);
                        string docTitle = GetAttributeValue(node.Attributes[XML_ATT_DOCUMENTTITLE]);
                        bool docHidden = false;
                            //(GetAttributeValue(node.Attributes[XML_ATT_HIDDEN]).ToLower()==bool.TrueString.ToLower()); //we do not yet support hidden documents
                        int docTocLeft = GetNodeLeftValue(node);
                        int docTocRight = GetNodeRightValue(node, docTocLeft);
                        bool docHasChildren = docTocLeft != docTocRight - 1;


                        if (isExternal)
                        {
                            string externalBookName = GetAttributeValue(node.Attributes[XML_ATT_EXT_BOOK_NAME]);
                            string externalDocName = EXTERNAL_DOCUMENT + externalCount++ + EXTERNAL_DOC_DATA_SEPERATOR +
                                                     externalBookName + EXTERNAL_DOC_DATA_SEPERATOR + docName;
                            DocumentDs.DocumentRow documentRow = documentDs.Document.AddDocumentRow(Id, externalDocName,
                                                                                                    docTitle,
                                                                                                    string.Empty,
                                                                                                    docTocLeft,
                                                                                                    docTocRight,
                                                                                                    docHasChildren,
                                                                                                    docHidden);
                        }
                        else
                        {
                            //update documents in the active document dataset
                            DocumentDs.DocumentRow documentRow = documentDs.Document.AddDocumentRow(Id, docName,
                                                                                                    docTitle,
                                                                                                    string.Empty,
                                                                                                    docTocLeft,
                                                                                                    docTocRight,
                                                                                                    docHasChildren,
                                                                                                    docHidden);

                            //process all format elements under the document
                            XmlNodeList formatNodes = node.SelectNodes(XML_ELE_DOCUMENTFORMAT);
                            foreach (XmlNode formatNode  in formatNodes)
                            {
                                //increment our document count
                                docCount++;

                                //get the content type
                                string formatContentTypeDesc =
                                    GetAttributeValue(formatNode.Attributes[XML_ATT_DOCUMENTFORMATCONTENTTYPE]);
                                ContentType formatContentType = GetContentTypeFromDescription(formatContentTypeDesc);

                                //get the format uri
                                string formatUri = GetAttributeValue(formatNode.Attributes[XML_ATT_DOCUMENTFORMATURI]);
                                bool primaryFormat =
                                    GetAttributeValue(formatNode.Attributes[XML_ATT_DOCUMENTFORMATPRI]).ToLower() ==
                                    bool.TrueString.ToLower();
                                bool autoDownload =
                                    GetAttributeValue(formatNode.Attributes[XML_ATT_DOCUMENTFORMATAUTODOWNLOAD]).ToLower
                                        () == bool.TrueString.ToLower();

                                //if the URI is a relative reference, assume that it is relative to the makefile's location
                                if (!Path.IsPathRooted(formatUri))
                                {
                                    formatUri = Path.Combine(makeFileDirectory, formatUri);
                                }

                                //be sure that the uri points to an existing resource before continuing
                                if (!File.Exists(formatUri))
                                {
                                    throw new FileNotFoundException(string.Format(ERROR_DOCUMENTFORMATURINOTFOUND,
                                                                                  formatUri, Name));
                                }

                                //copy our primary document format to the base of the book folder...
                                string destPath = string.Empty;
                                string dbDestPath = string.Empty;
                                string baseFormatFileExt = new FileInfo(formatUri).Extension;
                                string destFilename = docCount + baseFormatFileExt;
                                if (primaryFormat)
                                {
                                    destPath = Path.Combine(bookFolder, destFilename);
                                    dbDestPath = Path.Combine(bookSubFolder, destFilename);
                                }
                                    //copy our other document formats to a sub-folder
                                else
                                {
                                    string formatFolder = Path.Combine(bookFolder, docCount + BOOK_FORMATSFOLDERSUFFIX);
                                    string dbFormatFolder = Path.Combine(bookSubFolder,
                                                                         docCount + BOOK_FORMATSFOLDERSUFFIX);
                                    Directory.CreateDirectory(formatFolder);
                                    destPath = Path.Combine(formatFolder, destFilename);
                                    dbDestPath = Path.Combine(dbFormatFolder, destFilename);
                                }
                                File.Copy(formatUri, destPath, true);

                                //add format objects into underlying dataset
                                documentDs.DocumentFormat.AddDocumentFormatRow(documentRow, (int) formatContentType,
                                                                               formatContentTypeDesc, dbDestPath,
                                                                               primaryFormat, autoDownload);
                            }

                            //process all namedanchor elements under the document (not including named anchors under this document's child documents)
                            XmlNodeList anchorNodes =
                                node.SelectNodes(XML_ELE_DOCUMENTANCHOR + "/descendant-or-self::" +
                                                 XML_ELE_DOCUMENTANCHOR);
                            foreach (XmlNode anchorNode  in anchorNodes)
                            {
                                //get the properties for the document anchor dataset ready
                                string anchorName = GetAttributeValue(anchorNode.Attributes[XML_ATT_DOCUMENTANCHORNAME]);
                                string anchorTitle =
                                    GetAttributeValue(anchorNode.Attributes[XML_ATT_DOCUMENTANCHORTITLE]);
                                bool anchorHidden =
                                    (GetAttributeValue(anchorNode.Attributes[XML_ATT_HIDDEN]).ToLower() ==
                                     bool.TrueString.ToLower());
                                int anchorTocLeft = GetNodeLeftValue(anchorNode);
                                int anchorTocRight = GetNodeRightValue(anchorNode, anchorTocLeft);
                                bool anchorHasChildren = !(anchorTocLeft == anchorTocRight - 1);

                                //add namedanchor objects into underlying dataset
                                documentDs.NamedAnchor.AddNamedAnchorRow(documentRow, anchorName, anchorTitle,
                                                                         string.Empty, anchorTocLeft, anchorTocRight,
                                                                         anchorHasChildren, anchorHidden);
                            }
                        }
                        break;
                    case XML_ELE_BOOKFOLDER:
                        //get the properties for the book folder dataset ready
                        string bookFolderName = GetAttributeValue(node.Attributes[XML_ATT_BOOKFOLDERNAME]);
                        string bookFolderTitle = GetAttributeValue(node.Attributes[XML_ATT_BOOKFOLDERTITLE]);
                        string bookFolderUri = GetAttributeValue(node.Attributes[XML_ATT_BOOKFOLDERURI]);
                        int bookFolderTocLeft = GetNodeLeftValue(node);
                        int bookFolderTocRight = GetNodeRightValue(node, bookFolderTocLeft);
                        bool bookFolderHasChildren = !(bookFolderTocLeft == bookFolderTocRight - 1);

                        //update folders in the active bookdataset
                        bookFolderDs.BookFolder.AddBookFolderRow(bookFolderName, bookFolderTitle, bookFolderUri,
                                                                 bookFolderTocLeft, bookFolderTocRight,
                                                                 bookFolderHasChildren);
                        break;
                    case XML_ELE_DOCUMENTFORMAT:
                        //handled under document case
                        break;
                    case XML_ELE_DOCUMENTANCHOR:
                        //handled under document case
                        break;
                    case XML_ELE_RESOURCES:
                        //not handled
                        break;
                    case XML_ELE_RESOURCE:
                        string resourceUri = GetAttributeValue(node.Attributes[XML_ATT_RESOURCEURI]);
                        string resourceName = GetAttributeValue(node.Attributes[XML_ATT_RESOURCENAME]);
                        string resourcePath = Path.Combine(Path.Combine(bookFolder, BOOK_RESOURCEFOLDER), resourceName);

                        //if the URI is a relative reference, assume that it is relative to the makefile's location
                        if (!Path.IsPathRooted(resourceUri))
                        {
                            resourceUri = Path.Combine(makeFileDirectory, resourceUri);
                        }

                        //be sure that the uri points to an existing resource before continuing
                        if (!File.Exists(resourceUri))
                        {
                            throw new FileNotFoundException(string.Format(ERROR_BOOKRESOURCENOTFOUND, resourceUri, Name));
                        }

                        //copy the resource file over
                        File.Copy(resourceUri, resourcePath);
                        break;
                }
            }

            //clear all content out of the book instance
            ActiveBookDalc.ClearBookContent(Id);

            //save the Document dataset
            ActiveDocumentDalc.Save(documentDs);

            //save the BookFolder dataset
            ActiveBookDalc.Save(bookFolderDs);

            //populate our BookTocNode dataset using the folder values returned in the BookFolder dataset
            foreach (BookFolderDs.BookFolderRow bookFolderRow in bookFolderDs.BookFolder.Rows)
            {
                bookTocNodeDs.BookTocNode.AddBookTocNodeRow(Id, bookFolderRow.FolderId, bookFolderRow.Left,
                                                            bookFolderRow.Right, (byte) NodeType.BookFolder,
                                                            NodeType.BookFolder.ToString(), bookFolderRow.Name,
                                                            bookFolderRow.Title, bookFolderRow.HasChildren, false);
            }

            //populate our BookTocNode dataset using the document values returned in the Document dataset
            foreach (DocumentDs.DocumentRow documentRow in documentDs.Document.Rows)
            {
                bookTocNodeDs.BookTocNode.AddBookTocNodeRow(Id, documentRow.DocumentInstanceId, documentRow.Left,
                                                            documentRow.Right, (byte) NodeType.Document,
                                                            NodeType.Document.ToString(), documentRow.Name,
                                                            documentRow.Title, documentRow.HasChildren,
                                                            documentRow.Hidden);
            }

            //populate our BookTocNode dataset using the namedanchor values returned in the NamedAnchor dataset
            foreach (DocumentDs.NamedAnchorRow namedAnchorRow in documentDs.NamedAnchor.Rows)
            {
                bookTocNodeDs.BookTocNode.AddBookTocNodeRow(Id, namedAnchorRow.NamedAnchorId, namedAnchorRow.Left,
                                                            namedAnchorRow.Right, (byte) NodeType.DocumentAnchor,
                                                            NodeType.DocumentAnchor.ToString(), namedAnchorRow.Name,
                                                            namedAnchorRow.Title, namedAnchorRow.HasChildren,
                                                            namedAnchorRow.Hidden);
            }

            //save the BookTocNode dataset
            ActiveBookDalc.Save(bookTocNodeDs);

            //refresh our document collection in case someone wants to turn around and look at the documents
            //(only refresh if it has already been used; otherwise we will pull from the db on first use anyway)
            if (activeDocumentCollection != null)
            {
                activeDocumentCollection = new DocumentCollection(this);
            }

            //refresh our booktocnode collection in case someone wants to turn around and look at the booktocnodes
            //(only refresh if it has already been used; otherwise we will pull from the db on first use anyway)
            if (activeBookTocNodeCollection != null)
            {
                activeBookTocNodeCollection = new BookTocNodeCollection(this);
            }
        }

        /// <summary>
        ///   Returns an integer representing the given xml node's left toc entry value
        /// </summary>
        /// <param name = "xmlNode">The node for which you wish to retrieve a left toc entry value</param>
        /// <returns>An integer representing the specified xml node's left toc entry value</returns>
        private int GetNodeLeftValue(XmlNode xmlNode)
        {
            int leftVal = 0;
            int preceding =
                xmlNode.SelectNodes("preceding::*[local-name(.)='" + XML_ELE_BOOK + "' or local-name(.)='" +
                                    XML_ELE_BOOKFOLDER + "' or local-name(.)='" + XML_ELE_DOCUMENT +
                                    "' or local-name(.)='" + XML_ELE_DOCUMENTANCHOR + "']").Count;
            int ancestor =
                xmlNode.SelectNodes("ancestor::*[local-name(.)='" + XML_ELE_BOOK + "' or local-name(.)='" +
                                    XML_ELE_BOOKFOLDER + "' or local-name(.)='" + XML_ELE_DOCUMENT +
                                    "' or local-name(.)='" + XML_ELE_DOCUMENTANCHOR + "']").Count - 1;
                /* -1 is for disregarding the makefile element */
            leftVal = 1 + (preceding*2) + ancestor + 1;
            return leftVal;
        }

        /// <summary>
        ///   Returns an integer representing the given xml node's right toc entry value
        /// </summary>
        /// <param name = "xmlNode">The node for which you wish to retrieve a right toc entry value</param>
        /// <param name = "leftVal">An integer representing the specified xml node's left toc entry value</param>
        /// <returns>An integer representing the specified xml node's right toc entry value</returns>
        private int GetNodeRightValue(XmlNode xmlNode, int leftVal)
        {
            int rightVal = 0;
            int descendant =
                xmlNode.SelectNodes("descendant::*[local-name(.)='" + XML_ELE_BOOK + "' or local-name(.)='" +
                                    XML_ELE_BOOKFOLDER + "' or local-name(.)='" + XML_ELE_DOCUMENT +
                                    "' or local-name(.)='" + XML_ELE_DOCUMENTANCHOR + "']").Count;
            rightVal = leftVal + (descendant*2) + 1;
            return rightVal;
        }

        #endregion Private Methods

        #region IBook Methods

        /// <summary>
        ///   Retrieves the document that contains the specified document anchor.
        /// </summary>
        /// <param name = "documentAnchorName">The name of the document anchor for which you wish to retrieve the containing document.</param>
        /// <returns>An interface representing the document containing the specified document anchor.</returns>
        public IDocument GetDocumentByAnchorName(string documentAnchorName)
        {
            IDocument retDoc = null;
            DocumentDs documentDs = ActiveDocumentDalc.GetBookDocumentByDocumentAnchor(Id, documentAnchorName);
            if (documentDs.Document.Rows.Count == 1)
            {
                retDoc = new Document.Document(this, (DocumentDs.DocumentRow) documentDs.Document.Rows[0]);
            }
            return retDoc;
        }

        /// <summary>
        ///   Saves the current book.
        /// </summary>
        public void Save()
        {
            ActiveBookDalc.Save(ActiveBookDs);
        }

        /// <summary>
        ///   Retrieves the document that follows the specified document.
        /// </summary>
        /// <param name = "contextDocument">The context document.</param>
        /// <returns>An interface representing the next document in the book. If the context document is the last document in the book, a null value is returned.</returns>
        public IDocument GetNextDocument(IDocument contextDocument)
        {
            IDocument retDoc = null;
            BookTocNodeDs.BookTocNodeRow bookTocNodeRow = ActiveBookDalc.GetNextDocument(Id, contextDocument.Id);
            if (bookTocNodeRow != null)
            {
                retDoc = new Document.Document(this, bookTocNodeRow.NodeId);
            }
            return retDoc;
        }

        /// <summary>
        ///   Retrieves the document that precedes the specified document.
        /// </summary>
        /// <param name = "contextDocument">The context document.</param>
        /// <returns>An interface representing the preceding document in the book. If the context document is the first document in the book, a null value is returned.</returns>
        public IDocument GetPreviousDocument(IDocument contextDocument)
        {
            IDocument retDoc = null;
            BookTocNodeDs.BookTocNodeRow bookTocNodeRow = ActiveBookDalc.GetPreviousDocument(Id, contextDocument.Id);
            if (bookTocNodeRow != null)
            {
                retDoc = new Document.Document(this, bookTocNodeRow.NodeId);
            }
            return retDoc;
        }

        /// <summary>
        ///   Builds the book using the values contained in the SourceType and SourceUri properties.
        /// </summary>
        public void Build()
        {
            try
            {
                //if the source type is makefile, build go through the makefile build process
                if (SourceType == BookSourceType.Makefile)
                {
                    //build the book from a makefile
                    BuildBookFromMakefile();
                }
                else
                {
                    throw new Exception(string.Format(ERROR_BOOKBUILDSOURCETYPENOTIMPLEMENTED, SourceType));
                }

                //if we get here we were successful, so set the publish date and set our status to Built
                PublishDate = DateTime.Now;
                ActiveBuildStatus = BookBuildStatus.Built;
                Save();
            }
            catch (Exception e)
            {
                //If we get here, something went wrong with the build. Log the problem and set the BuildStatus to Error.
                //log the error
                Event bookEvent = new Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_BOOKBUILD_ERROR, MODULE_BOOK,
                                            METHOD_BOOKBUILD, Id.ToString(), e.Message);
                bookEvent.Save(false);
                //set our Error status and save the book
                ActiveBuildStatus = BookBuildStatus.Error;
                Save();
            }
        }

        public string GetTocXml(int nodeId, NodeType nodeType)
        {
            return GetTocXml(nodeId, nodeType, false);
        }

        /// <summary>
        ///   Retrieves XML representing the toc nodes that are immediate children to the context node.
        /// </summary>
        /// <param name = "nodeId">The id of the context node</param>
        /// <param name = "nodeType">The type of the context node</param>
        /// <returns>An interface representing the table of contents structure for the book.</returns>
        public string GetTocXml(int nodeId, NodeType nodeType, bool ignoreAnchors)
        {
            string retXml = string.Empty;

            BookTocNodeCollection bookTocNodes = new BookTocNodeCollection(nodeId, nodeType);
            retXml = GetXmlFromTocNodes(bookTocNodes, false, ignoreAnchors);

            return retXml;
        }

        #endregion IBook Methods

        #endregion Methods
    }
}