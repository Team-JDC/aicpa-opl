using System;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.User;

namespace AICPA.Destroyer.Shared
{
    public class Log
    {
        /// <summary>
        /// Generic Log Event Method
        /// </summary>
        /// <param name="severity">Severity of the Event</param>
        /// <param name="module">Module where event was logged</param>
        /// <param name="method">Method event was logged in</param>
        /// <param name="name">Name of Log</param>
        /// <param name="description">Description of Event</param>
        /// <param name="user">User</param>
        public static void LogEvent(int severity, string module, string method, string name, string description, IUser user)
        {
            LogEvent(EventType.Info, severity, module, method, name, description, user);
        }

        /// <summary>
        /// Generic Log Event Method
        /// </summary>
        /// <param name="eventType">Event Type</param>
        /// <param name="severity">Severity of the Event</param>
        /// <param name="module">Module where event was logged</param>
        /// <param name="method">Method event was logged in</param>
        /// <param name="name">Name of Log</param>
        /// <param name="description">Description of Event</param>
        /// <param name="user">User</param>
        public static void LogEvent(EventType eventType, int severity, string module, string method, string name, string description, IUser user)
        {
            IEvent logEvent = new Event(eventType, DateTime.Now, severity,
                              module, method, name, description, user);
            logEvent.Save(true);
        }

        /// <summary>
        /// Generic Log Event Method
        /// </summary>
        /// <param name="eventType">Event Type</param>
        /// <param name="severity">Severity of the Event</param>
        /// <param name="module">Module where event was logged</param>
        /// <param name="method">Method event was logged in</param>
        /// <param name="name">Name of Log</param>
        /// <param name="description">Description of Event</param>        
        public static void LogEvent(EventType eventType, int severity, string module, string method, string name, string description)
        {
            IEvent logEvent = new Event(eventType, DateTime.Now, severity,
                              module, method, name, description);
            logEvent.Save(true);
        }


    }
}
