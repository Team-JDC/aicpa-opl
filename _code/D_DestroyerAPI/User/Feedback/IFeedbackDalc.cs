#region

using System;

#endregion

namespace AICPA.Destroyer.User.Feedback
{
    /// <summary>
    ///   Summary description for IFeedback.
    /// </summary>
    public interface IFeedbackDalc
    {
        ///<summary>
        ///  Insert, Update, or Delete feedback based on the dataset provided.
        ///</summary>
        ///<param name = "feedbackDataTable">A strongly typed datatable for holding feedback rows.</param>
        void Save(FeedbackDS.FeedbackDataTable feedbackDataTable);

        ///<summary>
        ///  Provides a report of feedback based on a time frame.
        ///</summary>
        ///<param name = "feedbackTopicId">The feedback topic to be returned.</param>
        ///<param name = "beginTime">The begin time for viewing the feedback.</param>
        ///<param name = "endTime">The end time for viewing the feedback.</param>
        ///<returns>A strongly typed datatable for holding feedback rows.</returns>
        FeedbackDS.FeedbackDataTable GetFeedBackReport(int feedbackTopicId, DateTime beginTime, DateTime endTime);
    }
}