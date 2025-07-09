using System;
using System.Collections.Generic;

namespace AICPA.Destroyer.User.Note
{
    /// <summary>
    ///   Summary description for INoteDalc.
    /// </summary>
    public interface INoteDalc
    {
        NoteDS.D_NoteRow GetNote(int noteId);

        IEnumerable<NoteDS.D_NoteRow> GetNotesForUser(Guid userId, string targetDoc, string targetPtr);
                

        int SaveNote(int noteId, Guid userId, string text, DateTime lastModified, string targetDoc, string targetPtr, string noteTitle);

        void DeleteNote(int noteId);
    }
}