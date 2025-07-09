#region

using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using AICPA.Destroyer.Shared;

#endregion
namespace AICPA.Destroyer.Content.Book
{
    /// <summary>
    ///		Contains a set of methods used to access the database for books, bookToc and BookFolder updates.
    /// </summary>
    public class BookDalc : DestroyerDalc, IBookDalc
    {
        #region Constants

        #region Command Settings

        private const int MAX_TIMEOUT = 0;

        #endregion Command Settings

        #region Stored Procedures Names

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOK = "dbo.D_GetBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKSALL = "dbo.D_GetBooksAll";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKS = "dbo.D_GetBooks";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKSBYBUILDSTATUS = "dbo.D_GetBooksByBuildStatus";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKBYSITE = "dbo.D_GetBookBySite";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKSBYSITE = "dbo.D_GetBooksBySite";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKSBYSITEFILTERED = "dbo.D_GetBooksBySiteFiltered";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETIMMEDIATEDESCENDANTNODESBOOKTOC = "dbo.D_GetImmediateDescendantNodesBookToc";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETBOOKTOCNODES = "dbo.D_GetBookTocNodes";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTBOOK = "dbo.D_InsertBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTBOOKTOCNODE = "dbo.D_InsertBookTocNode";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETPREVIOUSDOCUMENTBOOKTOCNODE = "dbo.D_GetPreviousDocument";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNEXTDOCUMENTBOOKTOCNODE = "dbo.D_GetNextDocument";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTBOOKFOLDER = "dbo.D_InsertBookFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_UPDATEBOOK = "dbo.D_UpdateBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTCOUNT = "dbo.D_GetDocumentCount";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_CLEARBOOKCONTENT = "dbo.D_ClearBookContent";

        #endregion Stored Procedures Names

        #region Dalc Errors

        private const string ERROR_GETBOOK = "Error getting a book.";
        private const string ERROR_GETBOOKS = "Error getting books.";
        private const string ERROR_GETSITEBOOKS = "Error getting a site book.";
        private const string ERROR_GETCHILDBOOKTOCNODES = "Error getting child book toc nodes.";
        private const string ERROR_GETBOOKTOCNODES = "Error getting book toc nodes.";
        private const string ERROR_GETNEXTDOCUMENT = "Error getting next document.";
        private const string ERROR_GETPREVIOUSDOCUMENT = "Error getting previous document.";
        private const string ERROR_SAVE = "Error saving book information.";
        private const string ERROR_GETDOCUMENTCOUNT = "Error getting document count.";
        private const string ERROR_CLEARBOOKCONTENT = "Error clearing book content";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_BOOKDALC = "BookDalc";
        private const string METHOD_GETBOOK = "GetBook";
        private const string METHOD_GETBOOKS = "GetBooks";
        private const string METHOD_GETSITEBOOKS = "GetSiteBooks";
        private const string METHOD_GETCHILDBOOKTOCNODES = "GetChildBookTocNodes";
        private const string METHOD_GETBOOKTOCNODES = "GetBookTocNodes";
        private const string METHOD_GETNEXTDOCUMENT = "GetNextDocument";
        private const string METHOD_GETPREVIOUSDOCUMENT = "GetPreviousDocument";
        private const string METHOD_SAVE = "Save";
        private const string METHOD_GETDOCUMENTCOUNT = "GetDocumentCount";
        private const string METHOD_CLEARBOOKCONTENT = "ClearBookContent";

        #endregion Module and Method Names

        #endregion Constants

        #region Constructors

        internal BookDalc()
        {
            moduleName = MODULE_BOOKDALC;
        }

        #endregion Constructors

        #region Methods

        #region Private Methods

        /// <summary>
        /// Gets the book data set.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="errorName">Name of the error.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        private BookDs GetBookDataSet(string methodName, string errorName, string storedProcedureName,
                                      params object[] parameterValues)
        {
            BookDs bookDs = new BookDs();
            this.FillDataset(methodName, errorName, storedProcedureName, bookDs, new string[1] {bookDs.Book.TableName},
                             parameterValues);
            return bookDs;
        }

        /// <summary>
        /// Gets the book toc node data table.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="errorName">Name of the error.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        private BookTocNodeDs.BookTocNodeDataTable GetBookTocNodeDataTable(string methodName, string errorName,
                                                                           string storedProcedureName,
                                                                           params object[] parameterValues)
        {
            BookTocNodeDs bookTocNodeDs = new BookTocNodeDs();
            this.FillDataset(methodName, errorName, storedProcedureName, bookTocNodeDs,
                             new string[1] {bookTocNodeDs.BookTocNode.TableName}, parameterValues);
            return bookTocNodeDs.BookTocNode;
        }

        /// <summary>
        /// Gets the book toc node row.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="errorName">Name of the error.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        private BookTocNodeDs.BookTocNodeRow GetBookTocNodeRow(string methodName, string errorName,
                                                               string storedProcedureName,
                                                               params object[] parameterValues)
        {
            BookTocNodeDs.BookTocNodeRow retRow = null;
            BookTocNodeDs bookTocNodeDs = new BookTocNodeDs();
            this.FillDataset(methodName, errorName, storedProcedureName, bookTocNodeDs,
                             new string[1] {bookTocNodeDs.BookTocNode.TableName}, parameterValues);
            if (bookTocNodeDs.BookTocNode.Rows.Count > 0)
            {
                retRow = (BookTocNodeDs.BookTocNodeRow) bookTocNodeDs.BookTocNode.Rows[0];
            }
            return retRow;
        }

        /// <summary>
        /// Saves the specified insert command.
        /// </summary>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="deleteCommand">The delete command.</param>
        /// <param name="updateCommand">The update command.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableName">Name of the table.</param>
        private void Save(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet,
                          string tableName)
        {
            //this could be a long-running operation, so increase command timeout
            insertCommand.CommandTimeout = MAX_TIMEOUT;

            this.UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertCommand, deleteCommand, updateCommand, dataSet, tableName);
        }

        #endregion Private Methods

        #region IBookDalc Methods

        #region GetBookDataSet

        /// <summary>
        /// Returns a book dataset with the first row of the book table populated with information for the specified book.
        /// </summary>
        /// <param name="bookId">The id of the book you wish to retrieve</param>
        /// <returns>A book dataset</returns>
        public BookDs GetBook(int bookId)
        {
            return GetBookDataSet(METHOD_GETBOOK, ERROR_GETBOOK, SP_GETBOOK, bookId);
        }

        /// <summary>
        /// Returns a book dataset with the first row of the book table populated with information from the specified book. Used
        /// when retrieving a book from within the context of a site (extra site information is available with this retrieval)
        /// </summary>
        /// <param name="siteId">The id of the context site</param>
        /// <param name="bookId">The id of the book you wish to retrieve</param>
        /// <returns>A book dataset</returns>
        public BookDs GetBook(int siteId, int bookId)
        {
            return GetBookDataSet(METHOD_GETBOOK, ERROR_GETBOOK, SP_GETBOOKBYSITE, siteId, bookId);
        }

        /// <summary>
        /// Returns a list of all books in the datastore
        /// </summary>
        /// <param name="latestVersion">Determines whether all versions of all books should be returned or only the current version of all books.</param>
        /// <param name="includeArchived">if set to <c>true</c> [include archived].</param>
        /// <returns>A book dataset</returns>
        public BookDs GetBooks(bool latestVersion, bool includeArchived)
        {
            return GetBookDataSet(METHOD_GETBOOKS, ERROR_GETBOOKS, includeArchived ? SP_GETBOOKSALL : SP_GETBOOKS,
                                  latestVersion);
        }

        /// <summary>
        /// Returns a list of all books in the datastore matching the specified book build status
        /// </summary>
        /// <param name="buildStatus">Specifies the book build status to use for retrieving books.</param>
        /// <returns>A book dataset</returns>
        public BookDs GetBooks(BookBuildStatus buildStatus)
        {
            return GetBookDataSet(METHOD_GETBOOKS, ERROR_GETBOOKS, SP_GETBOOKSBYBUILDSTATUS, (int) buildStatus);
        }

        /// <summary>
        /// Returns a book dataset containing the books associated with the specified site.
        /// </summary>
        /// <param name="siteId">The siteId of the sitebook to be returned.</param>
        /// <returns>A book dataset</returns>
        public BookDs GetSiteBooks(int siteId)
        {
            return GetBookDataSet(METHOD_GETSITEBOOKS, ERROR_GETSITEBOOKS, SP_GETBOOKSBYSITE, siteId);
        }

        /// <summary>
        /// Returns a book dataset containing the books associated with the specified site, filtered by the specified bookDomain.
        /// </summary>
        /// <param name="siteId">The siteId of the sitebook to be returned.</param>
        /// /// <param name="bookDomain">The list of books allowed to be returned. Must be formatted for a SQL WHERE clause as follows:
        ///		'book1','book2','book3'</param>
        /// <returns>A book dataset</returns>
        public BookDs GetSiteBooks(int siteId, string bookDomain)
        {
            return GetBookDataSet(METHOD_GETSITEBOOKS, ERROR_GETSITEBOOKS, SP_GETBOOKSBYSITEFILTERED, siteId, bookDomain);
        }

        #endregion GetBookDataSet

        #region GetBookTocNodeDataTable

        /// <summary>
        /// Returns a book toc node dataset containing the book toc node children underneath the context node
        /// </summary>
        /// <param name="nodeId">The id of the context node for which you want to retrieve children</param>
        /// <param name="nodeType">The node type of the context node</param>
        /// <returns>A book toc node dataset</returns>
        public BookTocNodeDs.BookTocNodeDataTable GetChildBookTocNodes(int nodeId, NodeType nodeType)
        {
            return GetBookTocNodeDataTable(METHOD_GETCHILDBOOKTOCNODES, ERROR_GETCHILDBOOKTOCNODES,
                                           SP_GETIMMEDIATEDESCENDANTNODESBOOKTOC, nodeId, (int) nodeType);
        }

        /// <summary>
        /// Returns a book toc node dataset containing all book toc nodes for the specified book
        /// </summary>
        /// <param name="bookId">The id of the book for which you want to retrieve children</param>
        /// <returns>A book toc node dataset</returns>
        public BookTocNodeDs.BookTocNodeDataTable GetBookTocNodes(int bookId)
        {
            return GetBookTocNodeDataTable(METHOD_GETBOOKTOCNODES, ERROR_GETBOOKTOCNODES, SP_GETBOOKTOCNODES, bookId);
        }

        #endregion GetBookTocNodeDataTable

        #region GetBookTocNodeRow

        /// <summary>
        /// Returns a book toc node dataset containing the document book toc node that follows the specified document
        /// </summary>
        /// <param name="bookId">The id of the book containing the context document</param>
        /// <param name="documentId">The id of the context document</param>
        /// <returns></returns>
        public BookTocNodeDs.BookTocNodeRow GetNextDocument(int bookId, int documentId)
        {
            return this.GetBookTocNodeRow(METHOD_GETNEXTDOCUMENT, ERROR_GETNEXTDOCUMENT, SP_GETNEXTDOCUMENTBOOKTOCNODE,
                                          bookId, documentId);
        }

        /// <summary>
        /// Returns a book toc node dataset containing the document book toc node that precedes the specified document
        /// </summary>
        /// <param name="bookId">The id of the book containing the context document</param>
        /// <param name="documentId">The id of the context document</param>
        /// <returns></returns>
        public BookTocNodeDs.BookTocNodeRow GetPreviousDocument(int bookId, int documentId)
        {
            return GetBookTocNodeRow(METHOD_GETPREVIOUSDOCUMENT, ERROR_GETPREVIOUSDOCUMENT,
                                     SP_GETPREVIOUSDOCUMENTBOOKTOCNODE, bookId, documentId);
        }

        #endregion GetBookTocNodeRow

        #region ClearContent

        /// <summary>
        /// Clears all documents and related content associated with the specified book instance id
        /// </summary>
        /// <param name="bookId">The book instance id for which you want to retrieve a document count.</param>
        /// <returns></returns>	
        public void ClearBookContent(int bookId)
        {
            this.ExecuteNonQuery(METHOD_CLEARBOOKCONTENT, ERROR_CLEARBOOKCONTENT, SP_CLEARBOOKCONTENT, bookId);
        }

        #endregion ClearContent

        #region GetDocumentCount

        /// <summary>
        /// Returns the number of documents in the specified book instance
        /// </summary>
        /// <param name="bookId">The book instance id for which you want to retrieve a document count.</param>
        /// <returns></returns>
        public int GetDocumentCount(int bookId)
        {
            return (int) ExecuteScalar(METHOD_GETDOCUMENTCOUNT, ERROR_GETDOCUMENTCOUNT, SP_GETDOCUMENTCOUNT, bookId);
        }

        #endregion GetDocumentCount

        #region Save

        /// <summary>
        /// Saves the book dataset
        /// </summary>
        /// <param name="bookDs">The book dataset to save</param>
        public void Save(BookDs bookDs)
        {
            SqlCommand insertBookCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString), SP_INSERTBOOK,
                                                                   bookDs.Book.BookInstanceIdColumn.ColumnName,
                                                                   bookDs.Book.NameColumn.ColumnName,
                                                                   bookDs.Book.TitleColumn.ColumnName,
                                                                   bookDs.Book.DescriptionColumn.ColumnName,
                                                                   bookDs.Book.PublishDateColumn.ColumnName,
                                                                   bookDs.Book.CopyrightColumn.ColumnName,
                                                                   bookDs.Book.SourceTypeIdColumn.ColumnName,
                                                                   bookDs.Book.SourceUriColumn.ColumnName,
                                                                   bookDs.Book.BuildStatusCodeColumn.ColumnName,
                                                                   bookDs.Book.BookVersionColumn.ColumnName);
            SqlCommand updateBookCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString), SP_UPDATEBOOK,
                                                                   bookDs.Book.BookInstanceIdColumn.ColumnName,
                                                                   bookDs.Book.NameColumn.ColumnName,
                                                                   bookDs.Book.TitleColumn.ColumnName,
                                                                   bookDs.Book.DescriptionColumn.ColumnName,
                                                                   bookDs.Book.PublishDateColumn.ColumnName,
                                                                   bookDs.Book.CopyrightColumn.ColumnName,
                                                                   bookDs.Book.SourceTypeIdColumn.ColumnName,
                                                                   bookDs.Book.SourceUriColumn.ColumnName,
                                                                   bookDs.Book.ArchivedColumn.ColumnName,
                                                                   bookDs.Book.BuildStatusCodeColumn.ColumnName);

            //this could be a long-running operation, so increase command timeout
            insertBookCommand.CommandTimeout = MAX_TIMEOUT;
            updateBookCommand.CommandTimeout = MAX_TIMEOUT;

            this.Save(insertBookCommand, null, updateBookCommand, bookDs, bookDs.Book.TableName);
        }

        /// <summary>
        /// Saves the book toc node dataset.
        /// </summary>
        /// <param name="bookTocNodeDs">The book toc node dataset to save</param>
        public void Save(BookTocNodeDs bookTocNodeDs)
        {
            SqlCommand insertBookTocNodeCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                          SP_INSERTBOOKTOCNODE,
                                                                          bookTocNodeDs.BookTocNode.BookTocIdColumn.
                                                                              ColumnName,
                                                                          bookTocNodeDs.BookTocNode.BookInstanceIdColumn
                                                                              .ColumnName,
                                                                          bookTocNodeDs.BookTocNode.NodeIdColumn.
                                                                              ColumnName,
                                                                          bookTocNodeDs.BookTocNode.NodeTypeIdColumn.
                                                                              ColumnName,
                                                                          bookTocNodeDs.BookTocNode.LeftColumn.
                                                                              ColumnName,
                                                                          bookTocNodeDs.BookTocNode.RightColumn.
                                                                              ColumnName,
                                                                          bookTocNodeDs.BookTocNode.HiddenColumn.
                                                                              ColumnName);

            //this could be a long-running operation, so increase command timeout
            insertBookTocNodeCommand.CommandTimeout = MAX_TIMEOUT;

            this.Save(insertBookTocNodeCommand, null, null, bookTocNodeDs, bookTocNodeDs.BookTocNode.TableName);
        }

        /// <summary>
        /// Saves the book folder dataset
        /// </summary>
        /// <param name="bookFolderDs">The book folder dataset to save</param>
        public void Save(BookFolderDs bookFolderDs)
        {
            SqlCommand insertBookFolderCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                         SP_INSERTBOOKFOLDER,
                                                                         bookFolderDs.BookFolder.FolderIdColumn.
                                                                             ColumnName,
                                                                         bookFolderDs.BookFolder.NameColumn.ColumnName,
                                                                         bookFolderDs.BookFolder.TitleColumn.ColumnName,
                                                                         bookFolderDs.BookFolder.UriColumn.ColumnName);

            //this could be a long-running operation, so increase command timeout
            insertBookFolderCommand.CommandTimeout = MAX_TIMEOUT;

            Save(insertBookFolderCommand, null, null, bookFolderDs, bookFolderDs.BookFolder.TableName);
        }

        #endregion Save

        #endregion IBookDalc Methods

        #endregion Methods
    }
}