using System;
using System.Collections.Generic;
using System.Linq;

using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

namespace AICPA.Destroyer.User.Note
{
    /// <summary>
    ///   Summary description for NoteDalc.
    /// </summary>
    public class NoteDalc : DestroyerDalc, INoteDalc
    {
        #region Constants

        #region Dalc Errors

        private const string ERROR_GETNOTE = "Error getting a saved user note by ID.";
        private const string ERROR_GETNOTEFORUSER = "Error getting a saved user note by UserId and TargetDoc/Ptr.";
        private const string ERROR_GETNOTESFORUSER = "Error getting all of a user's notes.";

        #endregion

        #region Module and Method Names

        private const string MODULE_NOTEDALC = "NoteDalc";
        private const string METHOD_GETNOTE = "GetNote";
        private const string METHOD_GETNOTEFORUSER = "GetNoteForUser";
        private const string METHOD_GETNOTESFORUSER = "GetNotesForUser";

        #endregion Module and Method Names

        #region Stored Procedures

        private const string SP_INSERTNOTE= "dbo.D_InsertNote";
        private const string SP_UPDATENOTE = "dbo.D_UpdateNote";
        private const string SP_DELETENOTE = "dbo.D_DeleteNote";
        private const string SP_GETNOTEBYID = "dbo.D_GetNoteById";
        private const string SP_GETNOTEBYUSERIDANDTARGETDOCPTR = "dbo.D_GetNoteByUserIdAndTargetDocPtr";
        private const string SP_GETNOTESBYUSERID = "dbo.D_GetNotesByUserId";

        #endregion Stored Procedures

        #endregion Constants

        #region Constructors

        public NoteDalc()
        {
            moduleName = MODULE_NOTEDALC;
        }

        #endregion Constructors

        #region INoteDalc Members

        public NoteDS.D_NoteRow GetNote(int noteId)
        {
            NoteDS dataSet = new NoteDS();
            FillDataset(METHOD_GETNOTE, ERROR_GETNOTE, SP_GETNOTEBYID, dataSet,
                        new[] { dataSet.D_Note.TableName }, noteId);

            return dataSet.D_Note.SingleOrDefault();
        }

        public IEnumerable<NoteDS.D_NoteRow> GetNotesForUser(Guid userId, string targetDoc, string targetPtr)
        {
            NoteDS dataSet = new NoteDS();
            FillDataset(METHOD_GETNOTEFORUSER, ERROR_GETNOTEFORUSER, SP_GETNOTEBYUSERIDANDTARGETDOCPTR, dataSet,
                        new[] { dataSet.D_Note.TableName }, userId, targetDoc, targetPtr);

            return dataSet.D_Note.ToList();
        }

        public IEnumerable<NoteDS.D_NoteRow> GetNotesForUser(Guid userId, int siteId)
        {
            NoteDS dataSet = new NoteDS();
            FillDataset(METHOD_GETNOTESFORUSER, ERROR_GETNOTESFORUSER, SP_GETNOTESBYUSERID, dataSet,
                        new[] { dataSet.D_Note.TableName }, userId, siteId);

            return (NoteDS.D_NoteRow[])dataSet.D_Note.Select();
        }

        public int SaveNote(int noteId, Guid userId, string text, DateTime lastModified, string targetDoc, string targetPtr, string title)
        {
            int returnId;

            if (noteId == 0)
            {
                var noClueWhyThisIsDecimal = SqlHelper.ExecuteScalar(DBConnectionString, SP_INSERTNOTE,
                                                                     userId, text, lastModified, targetDoc, targetPtr, title);

                returnId = Convert.ToInt32(noClueWhyThisIsDecimal);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_UPDATENOTE,
                                          noteId, text, lastModified, title);

                returnId = noteId;
            }

            return returnId;
        }

        public void DeleteNote(int noteId)
        {
            SqlHelper.ExecuteNonQuery(DBConnectionString, SP_DELETENOTE, noteId);
        }

        #endregion
    }
}