#region

using System;

#endregion

namespace AICPA.Destroyer.User.Event
{
    /// <summary>
    ///   Event Error Type
    /// </summary>
    public enum EventType
    {
        ///<summary>
        ///  An event that occurred because of an error.
        ///</summary>
        Error = 1,

        ///<summary>
        ///  An informational event occurred, but not necessarily an error.
        ///</summary>
        Info = 2,

        ///<summary>
        ///  An event that occurred by normal site usage.
        ///</summary>
        Usage = 3
    }

    /// <summary>
    ///   Summary description for IEvent.
    /// </summary>
    public interface IEvent
    {
        #region Properties

        ///<summary>
        ///  The Event's underlying user.
        ///</summary>
        Guid UserId { get; }

        ///<summary>
        ///  The event type.
        ///</summary>
        EventType EventTypeValue { get; }

        ///<summary>
        ///  The time the event occurred.
        ///</summary>
        DateTime EventTime { get; }

        ///<summary>
        ///  The user's session id.
        ///</summary>
        string SessionId { get; }

        ///<summary>
        ///  The severity of the event that occurred.
        ///</summary>
        int Severity { get; }

        ///<summary>
        ///  The module where the event occurred (in the code).
        ///</summary>
        string Module { get; }

        ///<summary>
        ///  The method where the event occurred (in the code).
        ///</summary>
        string Method { get; }

        ///<summary>
        ///  The name of the event.
        ///</summary>
        string Name { get; }

        ///<summary>
        ///  A description of the event that occurred.
        ///</summary>
        string Description { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Save the event.
        ///</summary>
        void Save();

        ///<summary>
        ///  Save the event.
        ///</summary>
        ///<param name = "checkLogSettings">Will affect whether or not the save call will run a check to see if it is supposed to save the event or not.</param>
        void Save(bool checkLogSettings);

        ///<summary>
        ///  Retrieves the error logged by the EventLog.
        ///</summary>
        ///<param name = "module">Module name where the error occured.</param>
        ///<param name = "method">Method name where the error occured.</param>
        string getEventLogged(string module, string method);

        #endregion Methods
    }
}