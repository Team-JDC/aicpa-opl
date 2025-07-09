#region

using System;

#endregion

namespace AICPA.Destroyer.User.Feedback
{
    ///<summary>
    ///  An IFeedback describes the properties and methods for creating, saving and deleting an IFeedback.
    ///</summary>
    public interface IFeedback
    {
        #region Properties

        ///<summary>
        ///  The underlying IUser that created the note.
        ///</summary>
        IUser User { get; }

        ///<summary>
        ///  The Id of the IFeedback.
        ///</summary>
        int Id { get; }

        ///<summary>
        ///  The date that the IFeedback was created.
        ///</summary>
        DateTime CreateDate { get; }

        ///<summary>
        ///  The topic for the feedback.
        ///</summary>
        string Topic { get; }

        ///<summary>
        ///  An Id uniquely identifying a topic.
        ///</summary>
        int TopicId { get; }

        ///<summary>
        ///  The actual text for the feedback.
        ///</summary>
        string Description { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Saves the IFeedback.
        ///</summary>
        void Save();

        ///<summary>
        ///  Delete the IFeedback.
        ///</summary>
        void Delete();

        #endregion Methods
    }
}