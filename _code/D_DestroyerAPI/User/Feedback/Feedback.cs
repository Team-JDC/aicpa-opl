#region

using System;

#endregion

namespace AICPA.Destroyer.User.Feedback
{
    /// <summary>
    ///   Summary description for Feedback.
    /// </summary>
    public class Feedback : IFeedback
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feedback" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public Feedback()
        {
            throw new Exception("This feature will not be implemented until phase 2");
            //
            // TODO: Add constructor logic here
            //
        }

        #region IFeedback Members

        /// <summary>
        /// Gets the user.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IUser User
        {
            get
            {
                // TODO:  Add Feedback.User getter implementation
                return null;
            }
        }

        /// <summary>
        /// Gets the id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Id
        {
            get
            {
                // TODO:  Add Feedback.Id getter implementation
                return 0;
            }
        }

        /// <summary>
        /// Gets the create date.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DateTime CreateDate
        {
            get
            {
                // TODO:  Add Feedback.CreateDate getter implementation
                return new DateTime();
            }
        }

        /// <summary>
        /// Gets the topic.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Topic
        {
            get
            {
                // TODO:  Add Feedback.Topic getter implementation
                return null;
            }
        }

        /// <summary>
        /// Gets the topic id.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int TopicId
        {
            get
            {
                // TODO:  Add Feedback.TopicId getter implementation
                return 0;
            }
        }

        /// <summary>
        /// Gets the description.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Description
        {
            get
            {
                // TODO:  Add Feedback.Description getter implementation
                return null;
            }
        }

        /// <summary>
        /// Saves this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Save()
        {
            // TODO:  Add Feedback.Save implementation
        }

        /// <summary>
        /// Deletes this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Delete()
        {
            // TODO:  Add Feedback.Delete implementation
        }

        #endregion
    }
}