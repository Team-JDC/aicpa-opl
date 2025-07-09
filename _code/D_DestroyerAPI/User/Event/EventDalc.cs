#region

using System;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace AICPA.Destroyer.User.Event
{
    ///<summary>
    ///  Saves events to the event log.
    ///</summary>
    public class EventDalc : DestroyerDalc, IEventDalc
    {
        #region Constants

        #region Stored Procedures

        private const string SP_INSERTEVENTLOG = "dbo.D_InsertEventLog";
        private const string SP_GETEVENTLOGGED = "dbo.D_GetEventLogByName";

        #endregion Stored Procedures

        #endregion Constants

        #region Methods

        #region IEventDalc Methods		

        ///<summary>
        ///  Save an event to the database.
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
        public void SaveEvent(Guid userId, int eventTypeId, DateTime eventTime, string sessionId, int severity,
                              string module, string method, string name, string description)
        {
            // DO NOT call the base datalayer classes here, because they actually call this
            // code if their is an error.  If there is an error in this code, we don't want it 
            // calling itself recursively!!
            if (userId == Guid.Empty)
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_INSERTEVENTLOG, DBNull.Value, eventTypeId, eventTime,
                                          sessionId ?? null, severity, module, method, name, description);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, SP_INSERTEVENTLOG, userId, eventTypeId, eventTime,
                                          sessionId ?? null, severity, module, method, name, description);
            }
        }

        ///<summary>
        ///  Retrieves the logged event description.
        ///</summary>
        ///<param name = "name">The name of the event to retrieve</param>
        ///<param name = "module">The module of the event to retrieve</param>
        ///<param name = "method">The method of the event to retrieve</param>
        public string RetrieveEventLogged(string name, string module, string method)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue =
                    (string) SqlHelper.ExecuteScalar(DBConnectionString, SP_GETEVENTLOGGED, name, module, method);
            }
            catch (Exception e)
            {
                returnValue = string.Format("ERROR Executing {0} || Error Msg: {1}", SP_GETEVENTLOGGED, e.Message);
            }

            return returnValue;
        }

        #endregion IEventDalc Methods

        #endregion Methods
    }
}