#region

using System;

#endregion

namespace AICPA.Destroyer.User.Event
{
    ///<summary>
    ///  Allows persisting of events.
    ///</summary>
    public interface IEventDalc
    {
        #region Methods

        ///<summary>
        ///  Saves and event to the database.
        ///</summary>
        ///<param name = "userId">The UserId of the user that caused the event</param>
        ///<param name = "eventTypeId">The event type</param>
        ///<param name = "eventTime">The time of the event</param>
        ///<param name = "sessionId">The sessionId of the user that caused the event</param>
        ///<param name = "severity">The severity of the event</param>
        ///<param name = "module">The module where the event occurred</param>
        ///<param name = "method">The method where the event occurred</param>
        ///<param name = "name">The name of the event</param>
        ///<param name = "description">The description of the event</param>
        void SaveEvent(Guid userId, int eventTypeId, DateTime eventTime, string sessionId, int severity, string module,
                       string method, string name, string description);

        ///<summary>
        ///  Retrieves the logged event description.
        ///</summary>
        ///<param name = "name">The name of the event to retrieve</param>
        ///<param name = "module">The module of the event to retrieve</param>
        ///<param name = "method">The method of the event to retrieve</param>
        string RetrieveEventLogged(string name, string module, string method);

        #endregion Methods
    }
}