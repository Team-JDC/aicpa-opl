using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

namespace AICPA.Destroyer.User.Bookmark
{
    class BookmarkDalc : DestroyerDalc
    {
        #region Constants

        #region Dalc Errors

        private const string ERROR_GETBOOKMARK = "Error getting a saved user bookmark by ID.";
        private const string ERROR_GETBOOKMARKFORUSER = "Error getting a saved user bookmark by UserId and TargetDoc/Ptr.";
        private const string ERROR_GETBOOKMARKSFORUSER = "Error getting all of a user's bookmarks.";

        #endregion

        #region Module and Method Names

        private const string MODULE_BOOKMARKDALC = "BookmakrDalc";
        private const string METHOD_GETBOOKMARK = "GetBookmark";
        private const string METHOD_GETBOOKMARKFORUSER = "GetBookmarkForUser";
        private const string METHOD_GETBOOKMARKSFORUSER = "GetBookmarksForUser";

        #endregion Module and Method Names

        #region Stored Procedures

        private const string SP_INSERTBOOKMARK= "dbo.D_InsertBookmark";
        private const string SP_UPDATEBOOKMARK = "dbo.D_UpdateBookmark";
        private const string SP_DELETEBOOKMARK = "dbo.D_DeleteBookmark";
        private const string SP_GETBOOKMARKBYID = "dbo.D_GetBookmarkById";
        private const string SP_GETBOOKMARKBYUSERIDANDTARGETDOCPTR = "dbo.D_GetBookmarkByUserIdAndTargetDocPtr";
        private const string SP_GETBOOKMARKBYDOCINSTANCEID = "dbo.D_GetBookmarkByDocInstanceId";
        private const string SP_GETBOOKMARKSBYUSERID = "dbo.D_GetBookmarksByUserId";

        #endregion Stored Procedures

        #endregion Constants

        #region Constructors

        public BookmarkDalc()
        {
            moduleName = MODULE_BOOKMARKDALC;
        }

        #endregion Constructors

        #region IBookmarkDalc Members

        public BookmarkDS.D_BookmarkRow GetBookmark(int noteId)
        {
            BookmarkDS dataSet = new BookmarkDS();
            FillDataset(METHOD_GETBOOKMARK, ERROR_GETBOOKMARK, SP_GETBOOKMARKBYID, dataSet,
                        new[] { dataSet.D_Bookmark.TableName }, noteId);

            return dataSet.D_Bookmark.SingleOrDefault();
        }

        public BookmarkDS.D_BookmarkRow GetBookmarkForUser(Guid userId, string targetDoc, string targetPtr)
        {
            BookmarkDS dataSet = new BookmarkDS();
            FillDataset(METHOD_GETBOOKMARKFORUSER, ERROR_GETBOOKMARKFORUSER, SP_GETBOOKMARKBYUSERIDANDTARGETDOCPTR, dataSet,
                        new[] { dataSet.D_Bookmark.TableName }, userId, targetDoc, targetPtr);

            return dataSet.D_Bookmark.SingleOrDefault();
        }

        public BookmarkDS.D_BookmarkRow GetBookmarkByDocInstanceId(Guid userId, int docInstanceId, string targetDoc)
        {
            BookmarkDS dataSet = new BookmarkDS();
            FillDataset(METHOD_GETBOOKMARKFORUSER, ERROR_GETBOOKMARKFORUSER, SP_GETBOOKMARKBYDOCINSTANCEID, dataSet,
                        new[] { dataSet.D_Bookmark.TableName }, userId, docInstanceId, targetDoc);

            return dataSet.D_Bookmark.FirstOrDefault();
        }

        public IEnumerable<BookmarkDS.D_BookmarkRow> GetBookmarksForUser(Guid userId)
        {
            BookmarkDS dataSet = new BookmarkDS();
            FillDataset(METHOD_GETBOOKMARKSFORUSER, ERROR_GETBOOKMARKSFORUSER, SP_GETBOOKMARKSBYUSERID, dataSet,
                        new[] { dataSet.D_Bookmark.TableName }, userId);

            return (BookmarkDS.D_BookmarkRow[])dataSet.D_Bookmark.Select();
        }

        public int SaveBookmark(int noteId, Guid userId, string targetDoc, string targetPtr, string title)
        {
            int returnId;

            if (noteId == 0)
            {
                var noClueWhyThisIsDecimal = SqlHelper.ExecuteScalar(DBConnectionString, SP_INSERTBOOKMARK,
                                                                     userId, targetDoc, targetPtr, title);

                returnId = Convert.ToInt32(noClueWhyThisIsDecimal);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_UPDATEBOOKMARK,
                                          noteId);

                returnId = noteId;
            }

            return returnId;
        }

        public void DeleteBookmark(int noteId)
        {
            SqlHelper.ExecuteNonQuery(DBConnectionString, SP_DELETEBOOKMARK, noteId);
        }

        #endregion
    }
}
