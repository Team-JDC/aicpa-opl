using System;

using AICPA.Destroyer.Content.Site;

namespace AICPA.Destroyer.User.Note
{
    ///<summary>
    ///  An INote describes the properties and methods for creating, saving and deleting an INote.
    ///</summary>
    public interface INote
    {
        #region Properties

        ///<summary>
        ///  The user id.
        ///</summary>
        Guid UserId { get; }

        ///<summary>
        ///  A unique Id for a note.
        ///</summary>
        int Id { get; }

        ///<summary>
        /// A note Title.
        /// </summary>
        string Title { get; set; }
        ///<summary>
        ///  The note's text.
        ///</summary>
        string Text { get; set; }

        ///<summary>
        ///  The date the note was last modified.
        ///</summary>
        DateTime LastModified { get; }

        string TargetDoc { get; }

        string TargetPtr { get; }

        int Available { get; set; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Save the note.
        ///</summary>
        void Save();

        ///<summary>
        ///  Delete the note.
        ///</summary>
        void Delete();

        string ResolveReferenceNode(ISite site);

        #endregion Methods
    }
}