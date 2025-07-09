#region

using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.User.Event
{
    ///<summary>
    ///  A class that is used to build and persist an event to a log.  Also provides static
    ///  members used to determine if an event need be logged.
    ///</summary>
    public class Event : DestroyerBpc, IEvent
    {
        #region Constants

        internal const string APPLICATION_SETTING_EVENTLOGSECTION = "eventlog";
        internal const string LOGSETTINGS_EVENT_NODE = "event";
        internal const string LOGSETTINGS_MODULE_NODE = "module";
        internal const string LOGSETTINGS_METHOD_NODE = "method";
        internal const string LOGSETTINGS_TYPE_ATTRIBUTE = "type";
        internal const string LOGSETTINGS_LEVEL_ATTRIBUTE = "level";
        internal const string LOGSETTINGS_NAME_ATTRIBUTE = "name";

        #endregion Constants

        #region Constructors

        ///<summary>
        ///  Creates an instance of an event that has occurred.
        ///</summary>
        ///<param name = "eventTypeValue"></param>
        ///<param name = "eventTime"></param>
        ///<param name = "severity"></param>
        ///<param name = "module"></param>
        ///<param name = "method"></param>
        ///<param name = "name"></param>
        ///<param name = "description"></param>
        ///<param name = "userId"></param>
        ///<param name = "sessionId"></param>
        public Event(EventType eventTypeValue, DateTime eventTime, int severity, string module, string method,
                     string name, string description, Guid userId, string sessionId)
        {
            this.userId = userId;
            this.eventTypeValue = eventTypeValue;
            this.eventTime = eventTime;
            this.sessionId = sessionId;
            this.severity = severity;
            this.module = module;
            this.method = method;
            this.name = name;
            this.description = description;
        }

        ///<summary>
        ///</summary>
        ///<param name = "eventTypeValue"></param>
        ///<param name = "eventTime"></param>
        ///<param name = "severity"></param>
        ///<param name = "module"></param>
        ///<param name = "method"></param>
        ///<param name = "name"></param>
        ///<param name = "description"></param>
        ///<param name = "user"></param>
        public Event(EventType eventTypeValue, DateTime eventTime, int severity, string module, string method,
                     string name, string description, IUser user)
        {
            userId = user.UserId;
            this.eventTypeValue = eventTypeValue;
            this.eventTime = eventTime;
            sessionId = user.UserSecurity.SessionId;
            this.severity = severity;
            this.module = module;
            this.method = method;
            this.name = name;
            this.description = description;
        }

        ///<summary>
        ///</summary>
        ///<param name = "eventTypeValue"></param>
        ///<param name = "eventTime"></param>
        ///<param name = "severity"></param>
        ///<param name = "module"></param>
        ///<param name = "method"></param>
        ///<param name = "name"></param>
        ///<param name = "description"></param>
        public Event(EventType eventTypeValue, DateTime eventTime, int severity, string module, string method,
                     string name, string description)
        {
            this.eventTypeValue = eventTypeValue;
            this.eventTime = eventTime;
            this.severity = severity;
            this.module = module;
            this.method = method;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event" /> class.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        public Event(string name)
        {
            this.name = name;
        }

        #endregion Constructors

        #region Properties

        #region Private Properties

        private EventDalc activeEventDalc;

        ///<summary>
        ///  Gets the data layer for accessing the database.
        ///</summary>
        private EventDalc ActiveEventDalc
        {
            get { return activeEventDalc ?? (activeEventDalc = new EventDalc()); }
        }

        #endregion Private Properties

        #region IEvent Properties

        private readonly string description;
        private readonly DateTime eventTime;
        private readonly EventType eventTypeValue;
        private readonly string method;
        private readonly string module;
        private readonly string name;
        private readonly string sessionId;
        private readonly int severity;
        private readonly Guid userId = Guid.Empty;

        ///<summary>
        ///  A reference to the IUser who triggered the event.
        ///</summary>
        public Guid UserId
        {
            get { return userId; }
        }

        ///<summary>
        ///  The type of event that was triggered.
        ///</summary>
        public EventType EventTypeValue
        {
            get { return eventTypeValue; }
        }

        ///<summary>
        ///  The time the event was triggered.
        ///</summary>
        public DateTime EventTime
        {
            get { return eventTime; }
        }

        ///<summary>
        ///  The sessionId of the user who triggered the event.
        ///</summary>
        public string SessionId
        {
            get { return sessionId; }
        }

        ///<summary>
        ///  The severity of the event.
        ///</summary>
        public int Severity
        {
            get { return severity; }
        }

        ///<summary>
        ///  The module where the event was triggered.
        ///</summary>
        public string Module
        {
            get { return module; }
        }

        ///<summary>
        ///  The method where the event was triggered.
        ///</summary>
        public string Method
        {
            get { return method; }
        }

        ///<summary>
        ///  The name of the event that was triggered.
        ///</summary>
        public string Name
        {
            get { return name; }
        }

        ///<summary>
        ///  A description for the event that was triggered.
        ///</summary>
        public string Description
        {
            get { return description; }
        }

        #endregion IEvent Properties

        #region Protected Static Properties

        ///<summary>
        ///  Hashtable used to cash the eventLogSettings read from the app's config file.
        ///  This is static because it is an application setting
        ///  and will not change from user to user or class instance to class instance.
        ///</summary>
        private static Hashtable eventLogSettings;

        ///<summary>
        ///  Property used to access the eventLogSettings Hashtable that caches
        ///  the log settings read from the app's config file.
        ///</summary>
        protected static Hashtable EventLogSettings
        {
            get
            {
                //if it hasn't been created yet then create it:
                if (eventLogSettings == null)
                {
                    //populate the loggableActions hashtable from the app's config file
                    //optimize Hashtable balance to .1 so that it is fastest for index retrieval.
                    eventLogSettings = new Hashtable(10, 0.1f);

                    //read from the app's config settings:
                    XmlNode eventLog_ApplicationSettingSection =
                        (XmlNode) ConfigurationManager.GetSection(APPLICATION_SETTING_EVENTLOGSECTION);
                    //loop through each '<event>' node int the '<eventlog>' section of the app's config file
                    XmlNodeList eventNodes = eventLog_ApplicationSettingSection.SelectNodes(LOGSETTINGS_EVENT_NODE);
                    foreach (XmlNode eventNode in eventNodes)
                    {
                        //insert the default log level for each event type
                        //TODO: gracefully throw exception if this attribute is not present.

                        string eventType = eventNode.Attributes[LOGSETTINGS_TYPE_ATTRIBUTE].InnerText;
                        int eventLevel = Convert.ToInt16(eventNode.Attributes[LOGSETTINGS_LEVEL_ATTRIBUTE].InnerText);
                        //add this to the Hashtable:
                        if (!eventLogSettings.Contains(eventType))
                        {
                            eventLogSettings.Add(eventType, eventLevel);
                        }

                        //loop through eact '<module>' node in the <eventlog><event><module>'s
                        XmlNodeList moduleNodes = eventNode.SelectNodes(LOGSETTINGS_MODULE_NODE);
                        foreach (XmlNode moduleNode in moduleNodes)
                        {
                            //insert the default log level for each module
                            string moduleName = moduleNode.Attributes[LOGSETTINGS_NAME_ATTRIBUTE].InnerText;
                            int moduleLevel =
                                Convert.ToInt16(moduleNode.Attributes[LOGSETTINGS_LEVEL_ATTRIBUTE].InnerText);
                            //add this to the Hashtable:
                            if (!eventLogSettings.Contains(eventType + moduleName))
                            {
                                eventLogSettings.Add(eventType + moduleName, moduleLevel);
                            }

                            //loop through eact '<module>' node in the <eventlog><event><module><method>'s
                            XmlNodeList methodNodes = moduleNode.SelectNodes(LOGSETTINGS_METHOD_NODE);
                            foreach (XmlNode methodNode in methodNodes)
                            {
                                //insert the default log level for each method
                                string methodName = methodNode.Attributes[LOGSETTINGS_NAME_ATTRIBUTE].InnerText;
                                int methodLevel =
                                    Convert.ToInt16(methodNode.Attributes[LOGSETTINGS_LEVEL_ATTRIBUTE].InnerText);
                                //add this to the Hashtable:
                                if (!eventLogSettings.Contains(eventType + moduleName + methodName))
                                {
                                    eventLogSettings.Add(eventType + moduleName + methodName, methodLevel);
                                }
                            }
                        }
                    }
                }
                return eventLogSettings;
            }
        }

        #endregion Protected Static Properties

        #endregion Properties

        #region Methods

        #region Public Static Methods

        ///<summary>
        ///  Checks the provided EventLogEntry to see (according to it's severity, moduleName,
        ///  and methodName) if the EventLogEntry should be committed to the DataBase.
        ///  The App's config file defines the eventlog configuration.
        ///</summary>
        ///<example>
        ///  This example demonstrates how you would use the IsEventToBeLogged method to check
        ///  the log settings before going to the trouble of constructing the eventLog description.
        ///  <code>
        ///    SecurityBPC securityBPC = new SecurityBPC();
        ///    UserDS adminUserDS = securityBPC.Login("Administrator@mrc.com", "admin", SecurityBPC.UserType.Internal, "192.168.1.1","1111111111");
        ///    EventLogBPC eventLogBPC = new EventLogBPC(securityBPC, adminUserDS);
        ///    EventLogEntry eventLogEntry = new EventLogEntry(System.Guid.NewGuid(),  EventType.Usage, EventSeverity.Level8, "MRC_UnitTester", "LogEvent_DefaultModuleSetting");
        ///    if (eventLogBPC.IsEventToBeLogged(eventLogEntry))
        ///    {
        ///    eventLogEntry.eventName = "MRC_UnitTester Test LogEvent";
        ///    eventLogEntry.eventDescription = "MRC_UnitTester...Test case to make sure LogEvent is working";
        ///    //you may want to have more details in this eventDescription
        ///    eventLogBPC.LogEvent(eventLogEntry, false);
        ///    }
        ///    securityBPC.LogOut();
        ///  </code>
        ///</example>
        ///<param name = "eventTypeValue">The event type</param>
        ///<param name = "method">The name of the method where the event occurred that is being logged.</param>
        ///<param name = "module">The name of the module where the event occurred that is being logged.</param>
        ///<param name = "severity">The severity of the event on a scale of 1-10, 1 being the most severe</param>
        ///<returns>
        ///  Boolean true if the provided EventLogEntry is to be committed to the Database
        ///</returns>
        public static bool IsEventToBeLogged(EventType eventTypeValue, int severity, string module, string method)
        {
            bool returnValue = false;

            string eventType = "" + eventTypeValue;
            string moduleName = module;
            string methodName = method;

            //check for a most granular eventLog level
            if (EventLogSettings.Contains(eventType + moduleName + methodName))
            {
                returnValue = (severity <= (int) EventLogSettings[eventType + moduleName + methodName]);
            }
                //check the module default eventLog level
            else if (EventLogSettings.Contains(eventType + moduleName))
            {
                returnValue = (severity <= (int) EventLogSettings[eventType + moduleName]);
            }
                //check the event default eventLog level
            else if (EventLogSettings.Contains(eventType))
            {
                returnValue = (severity <= (int) EventLogSettings[eventType]);
            }
            return returnValue;
        }

        #endregion Public Static Methods

        #region IEvent Methods

        ///<summary>
        ///  Saves an event to the event log.
        ///</summary>
        public void Save()
        {
            Save(true);
        }

        ///<summary>
        ///  Saves an event to the event log.
        ///</summary>
        ///<param name = "checkLogSettings">Determines whether or not to check if an event is to be logged.</param>
        public void Save(bool checkLogSettings)
        {
            if (!checkLogSettings || (checkLogSettings && IsEventToBeLogged(EventTypeValue, Severity, Module, Method)))
            {
                ActiveEventDalc.SaveEvent(UserId, (int) EventTypeValue, EventTime, SessionId, Severity, Module, Method,
                                          Name, Description);
            }
        }

        ///<summary>
        ///  Retrieves the error logged by the EventLog.
        ///</summary>
        ///<param name = "module">Module name where the error occured.</param>
        ///<param name = "method">Method name where the error occured.</param>
        public string getEventLogged(string module, string method)
        {
            return ActiveEventDalc.RetrieveEventLogged(name, module, method);
        }

        #endregion IEvent Methods

        #endregion Methods
    }
}