using System;
using System.Collections.Generic;
using System.Linq;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.User.Note
{
    /// <summary>
    ///   Summary description for Note.
    /// </summary>
    public class Note : DestroyerBpc, INote
    {
        private INoteDalc _activeNoteDalc;

        private INoteDalc ActiveNoteDalc
        {
            get { return _activeNoteDalc ?? (_activeNoteDalc = new NoteDalc()); }
        }

        #region INote Members

        /// <summary>
        /// Gets the user.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Id { get; private set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime LastModified { get; private set; }

        public string TargetDoc { get; private set; }

        public string TargetPtr { get; private set;}

        public int Available { get; set; }

        // This constructor should not be used
        // The reason why it is here is so that it appears as though it is serializable
        // to the UserPreferences.asmx web service.  If you don't have a parameterless 
        // constructor, it won't appear as though it is serializable.
        public Note()
        {
            // This constructor should not be used
        }
        
        public Note(string targetDoc, string targetPtr, Guid userId, string noteText, string noteTitle)
        {
            TargetDoc = targetDoc;
            TargetPtr = targetPtr;
            UserId = userId;
            Text = noteText;
            Title = noteTitle;
        }

        public Note(string targetDoc, string targetPtr, Guid userId, string noteText, string noteTitle, int available)
        {
            TargetDoc = targetDoc;
            TargetPtr = targetPtr;
            UserId = userId;
            Text = noteText;
            Title = noteTitle;
            Available = available;
        }

        public Note(NoteDS.D_NoteRow noteRow)
        {
            Id = noteRow.NoteID;
            UserId = noteRow.UserID;
            Text = noteRow.NoteText;
            TargetDoc = noteRow.TargetDoc;
            TargetPtr = noteRow.TargetPtr;
            LastModified = noteRow.LastModifiedDate;
            Title = noteRow.NoteTitle;
            Available = noteRow.Available;
        }

        /// <summary>
        /// Saves this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Save()
        {
            LastModified = DateTime.Now;
            Id = ActiveNoteDalc.SaveNote(Id, UserId, Text, LastModified, TargetDoc, TargetPtr, Title);
        }

        /// <summary>
        /// Deletes this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Delete()
        {
            ActiveNoteDalc.DeleteNote(Id);
            Id = 0;
            LastModified = DateTime.MinValue;
        }

        public string ResolveReferenceNode(ISite site)
        {
            ContentLink content = new ContentLink(site, TargetDoc, TargetPtr);

            string siteReferencePath = "";

            if (content.DocumentAnchor != null)
            {
                siteReferencePath = content.DocumentAnchor.SiteReferencePath;
            }
            else if (content.Document != null)
            {
                siteReferencePath = content.Document.SiteReferencePath;
            }
            else if (content.Book != null)
            {
                siteReferencePath = content.Book.ReferencePath;
            }

            return siteReferencePath;
        }

        public static void DeleteNoteById(int id)
        {
            new NoteDalc().DeleteNote(id);
        }

        public static INote GetNoteById(int noteId)
        {
            var retrievedNoteRow = new NoteDalc().GetNote(noteId);
            INote note;

            note = retrievedNoteRow != null ?
                new Note(retrievedNoteRow) :
                null;

            return note;
        }

        public static List<INote> GetNotesByUserIdAndTargetDocPtr(Guid userId, string targetDoc, string targetPtr)
        {
            IEnumerable<NoteDS.D_NoteRow> retrievedNoteRows = new NoteDalc().GetNotesForUser(userId, targetDoc, targetPtr);

            return (from row in retrievedNoteRows.ToList()
                    select new Note(row)).ToList<INote>();
        }

        /// <summary>
        /// Gets the saved searches for user.	
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IEnumerable<INote> GetNotesForUser(Guid userId, int siteId)
        {
            var retrievedNoteRows = new NoteDalc().GetNotesForUser(userId, siteId);

            return retrievedNoteRows.Select(row => (INote)new Note(row));
        }

        #endregion
    }
}