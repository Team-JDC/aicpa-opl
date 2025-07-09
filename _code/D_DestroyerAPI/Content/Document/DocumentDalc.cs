#region

using System.Data.SqlClient;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace AICPA.Destroyer.Content.Document
{
    ///<summary>
    ///  Methods for getting documents and creating and updating documents and their related information.
    ///</summary>
    public class DocumentDalc : DestroyerDalc, IDocumentDalc
    {
        #region Constants

        #region Command Settings

        private const int MAX_TIMEOUT = 0;

        #endregion Command Settings

        #region Stored Procedures

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTSBYBOOK = "D_GetDocumentsByBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTBYBOOKDOCUMENTID = "D_GetDocumentByBookDocumentId";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTBYBOOKDOCUMENTNAME = "D_GetDocumentByBookDocumentName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTBYBOOKDOCUMENTINDEX = "D_GetDocumentByBookDocumentIndex";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENTIDBYBOOKDOCUMENTANCHOR = "D_GetDocumentByBookDocumentAnchor";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETDOCUMENT = "D_GetDocument";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETANCESTORNODES = "D_GetAncestorNodes";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNAMEDANCHOR = "D_GetNamedAnchor";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNAMEDANCHORBYNAME = "D_GetNamedAnchorByName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETNAMEDANCHORSBYDOCUMENT = "D_GetNamedAnchorsByDocument";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTDOCUMENT = "D_InsertDocument";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTDOCUMENTFORMAT = "D_InsertDocumentFormat";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTNAMEDANCHOR = "D_InsertNamedAnchor";

        #endregion Stored Procedures

        #region Dalc Errors

        private const string ERROR_GETDOCUMENT = "Error getting a document.";
        private const string ERROR_GETBOOKDOCUMENT = "Error getting a book document.";
        private const string ERROR_GETDOCUMENTID = "Error getting documentId.";
        private const string ERROR_GETBOOKDOCUMENTS = "Error getting book documents.";
        private const string ERROR_GETNAMEDANCHOR = "Error getting a named anchor.";
        private const string ERROR_SAVE = "Error saving document information.";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_DOCUMENTDALC = "DocumentDalc";
        private const string METHOD_GETDOCUMENT = "GetDocument";
        private const string METHOD_GETBOOKDOCUMENT = "GetBookDocument";
        private const string METHOD_GETDOCUMENTID = "GetDocumentId";
        private const string METHOD_GETBOOKDOCUMENTS = "GetBookDocuments";
        private const string METHOD_GETNAMEDANCHOR = "GetNamedAnchor";
        private const string METHOD_SAVE = "Save";

        #endregion Module and Method Names

        //data relations
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DR_DOCUMENT_DOCUMENTFORMAT = "DocumentDocumentFormat";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DR_DOCUMENT_NAMEDANCHOR = "DocumentNamedAnchor";

        #endregion Constants

        #region Constructors

        internal DocumentDalc()
        {
            moduleName = MODULE_DOCUMENTDALC;
        }

        #endregion Constructors

        #region Methods

        #region IDocumentDalc Methods

        /// <summary>
        ///   Retrieves a single document into a dataset and returns the dataset.
        /// </summary>
        /// <param name = "documentId"></param>
        /// <returns></returns>
        public DocumentDs GetDocument(int documentId)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETDOCUMENT, ERROR_GETDOCUMENT, SP_GETDOCUMENT, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, documentId);
            return documentDs;
        }

        /// <summary>
        ///   Retrieves a single document into a dataset and returns the dataset. Same as GetDocument, but passes in the book context.
        ///   We should probably use this method rather than GetDocument whenever we are dealing with documents contained in books.
        ///   This way we will be better off in case we decide to support multipe containment of documents in the future.
        /// </summary>
        /// <param name = "bookId"></param>
        /// <param name = "documentId"></param>
        /// <returns></returns>
        public DocumentDs GetBookDocument(int bookId, int documentId)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETBOOKDOCUMENT, ERROR_GETBOOKDOCUMENT, SP_GETDOCUMENTBYBOOKDOCUMENTID, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, bookId, documentId);
            return documentDs;
        }

        /// <summary>
        ///   Get a single document dataset from within the context of a book, using an ordinal index value.
        /// </summary>
        /// <param name = "bookId">The Id of the book containing the desired document</param>
        /// <param name = "index">The index of the document to be retrieved (order is determined by toc placement or the document node). This value is zero based.</param>
        /// <returns></returns>
        public DocumentDs GetBookDocumentByIndex(int bookId, int index)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETBOOKDOCUMENT, ERROR_GETBOOKDOCUMENT, SP_GETDOCUMENTBYBOOKDOCUMENTINDEX, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, bookId, index + 1);
            return documentDs;
        }

        /// <summary>
        /// </summary>
        /// <param name = "bookId">The id of the book containing the document you are looking for.</param>
        /// <param name = "documentAnchorName">The name of the document anchor for which you wish to retrieve the containing document.</param>
        /// <returns></returns>
        public DocumentDs GetBookDocumentByDocumentAnchor(int bookId, string documentAnchorName)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETBOOKDOCUMENT, ERROR_GETBOOKDOCUMENT, SP_GETDOCUMENTIDBYBOOKDOCUMENTANCHOR, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, bookId, documentAnchorName);
            return documentDs;
        }

        /// <summary>
        ///   DALC method to retrieve the documents belonging to the specified book
        /// </summary>
        /// <param name = "bookId">The id of the book for which to retrieve documents</param>
        /// <returns></returns>
        public DocumentDs GetBookDocuments(int bookId)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETBOOKDOCUMENTS, ERROR_GETBOOKDOCUMENTS, SP_GETDOCUMENTSBYBOOK, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, bookId);
            return documentDs;
        }

        /// <summary>
        ///   Gets a named achor row for the specified named anchor id
        /// </summary>
        /// <param name = "namedAnchorId">The name of the named anchor to be retrieved</param>
        /// <returns></returns>
        public DocumentDs.NamedAnchorRow GetNamedAnchor(int namedAnchorId)
        {
            DocumentDs documentDs = new DocumentDs();
            //disable constraints since we are not going to be pulling back the entire dataset
            documentDs.EnforceConstraints = false;
            FillDataset(METHOD_GETNAMEDANCHOR, ERROR_GETNAMEDANCHOR, SP_GETNAMEDANCHOR, documentDs,
                        new[] {documentDs.NamedAnchor.TableName}, namedAnchorId);
            return (DocumentDs.NamedAnchorRow) documentDs.NamedAnchor.Rows[0];
        }

        /// <summary>
        ///   Gets a named achor row for the specified named anchor name
        /// </summary>
        /// <param name = "documentInstanceId">The id of the document containing the named anchor</param>
        /// <param name = "namedAnchorName">The name of the named anchor to be retrieved</param>
        /// <returns></returns>
        public DocumentDs.NamedAnchorRow GetNamedAnchor(int documentInstanceId, string namedAnchorName)
        {
            DocumentDs documentDs = new DocumentDs();
            //disable constraints since we are not going to be pulling back the entire dataset
            documentDs.EnforceConstraints = false;
            FillDataset(METHOD_GETNAMEDANCHOR, ERROR_GETNAMEDANCHOR, SP_GETNAMEDANCHORBYNAME, documentDs,
                        new[] {documentDs.NamedAnchor.TableName}, documentInstanceId, namedAnchorName);
            return (DocumentDs.NamedAnchorRow) documentDs.NamedAnchor.Rows[0];
        }

        /// <summary>
        ///   Gets the named achor rows associated with the given document id
        /// </summary>
        /// <param name = "documentId">The id of the document for which named anchors are needed.</param>
        /// <returns></returns>
        public DocumentDs.NamedAnchorRow[] GetNamedAnchors(int documentId)
        {
            DocumentDs documentDs = new DocumentDs();
            //disable constraints since we are not going to be pulling back the entire dataset
            documentDs.EnforceConstraints = false;
            FillDataset(METHOD_GETNAMEDANCHOR, ERROR_GETNAMEDANCHOR, SP_GETNAMEDANCHORSBYDOCUMENT, documentDs,
                        new[] {documentDs.NamedAnchor.TableName}, documentId);
            return (DocumentDs.NamedAnchorRow[]) documentDs.NamedAnchor.Select();
        }

        /// <summary>
        ///   DALC method to persist a Document dataset
        /// </summary>
        /// <param name = "documentDs"></param>
        public void Save(DocumentDs documentDs)
        {
            //persist the documents first
            SqlCommand insertDocumentCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                       SP_INSERTDOCUMENT,
                                                                       documentDs.Document.DocumentInstanceIdColumn.
                                                                           ColumnName,
                                                                       documentDs.Document.BookInstanceIdColumn.
                                                                           ColumnName,
                                                                       documentDs.Document.NameColumn.ColumnName,
                                                                       documentDs.Document.TitleColumn.ColumnName);
            //this could be a long-running operation, so increase command timeout
            insertDocumentCommand.CommandTimeout = MAX_TIMEOUT;
            UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertDocumentCommand, null, null, documentDs,
                          documentDs.Document.TableName);

            //persist the formats next
            DocumentDs.DocumentFormatDataTable documentFormatDt = documentDs.DocumentFormat;
            SqlCommand insertDocumentFormatCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                             SP_INSERTDOCUMENTFORMAT,
                                                                             documentFormatDt.DocumentInstanceIdColumn.
                                                                                 ColumnName,
                                                                             documentFormatDt.ContentTypeIdColumn.
                                                                                 ColumnName,
                                                                             documentFormatDt.UriColumn.ColumnName,
                                                                             documentFormatDt.PrimaryColumn.ColumnName,
                                                                             documentFormatDt.AutoDownloadColumn.
                                                                                 ColumnName);
            //this could be a long-running operation, so increase command timeout
            insertDocumentFormatCommand.CommandTimeout = MAX_TIMEOUT;
            UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertDocumentFormatCommand, null, null, documentDs,
                          documentFormatDt.TableName);

            //finally, persist the named anchors
            DocumentDs.NamedAnchorDataTable namedAnchorDt = documentDs.NamedAnchor;
            SqlCommand insertNamedAnchorCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                          SP_INSERTNAMEDANCHOR,
                                                                          namedAnchorDt.NamedAnchorIdColumn.ColumnName,
                                                                          namedAnchorDt.DocumentInstanceIdColumn.
                                                                              ColumnName,
                                                                          namedAnchorDt.NameColumn.ColumnName,
                                                                          namedAnchorDt.TitleColumn.ColumnName);
            //this could be a long-running operation, so increase command timeout
            insertNamedAnchorCommand.CommandTimeout = MAX_TIMEOUT;
            UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertNamedAnchorCommand, null, null, documentDs,
                          namedAnchorDt.TableName);
        }

        /// <summary>
        ///   Retrieves a single document into a dataset and returns the dataset. Same as GetDocument, but passes in the book context.
        ///   We should probably use this method rather than GetDocument whenever we are dealing with documents contained in books.
        ///   This way we will be better off in case we decide to support multipe containment of documents in the future.
        /// </summary>
        /// <param name = "bookId"></param>
        /// <param name = "documentName"></param>
        /// <returns></returns>
        public DocumentDs GetBookDocument(int bookId, string documentName)
        {
            DocumentDs documentDs = new DocumentDs();
            FillDataset(METHOD_GETBOOKDOCUMENT, ERROR_GETBOOKDOCUMENT, SP_GETDOCUMENTBYBOOKDOCUMENTNAME, documentDs,
                        new[]
                            {
                                documentDs.Document.TableName, documentDs.DocumentFormat.TableName,
                                documentDs.NamedAnchor.TableName
                            }, bookId, documentName);
            return documentDs;
        }

        #endregion IDocumentDalc Methods

        #endregion Methods
    }
}