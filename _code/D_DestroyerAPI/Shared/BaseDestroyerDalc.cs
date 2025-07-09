#region

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AICPA.Destroyer.User.Event;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace AICPA.Destroyer.Shared
{
    ///<summary>
    ///  A set of base methods that each dalc can implement.
    ///</summary>
    public class DestroyerDalc
    {
        #region Constants 

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const string APPLICATION_SETTING_DBCONNECTIONSTRING = "DbConnectionString";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_DB_ERROR = 1;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SERVER_NOT_AVAILABLE = 17;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const string SQL_EXCEPTION = "SqlException";

        #endregion Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected static string DBConnectionString =
            ConfigurationSettings.AppSettings[APPLICATION_SETTING_DBCONNECTIONSTRING];

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static int RoundTrips;

        #region Constructors

        internal DestroyerDalc()
        {
        }

        #endregion Constructors

        #region Properties

        #region Protected Properties

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected string moduleName = "Unknown";

        /// <summary>
        /// Gets the name of the module.	
        /// </summary>
        /// <value>The name of the module.</value>
        /// <remarks></remarks>
        protected string ModuleName
        {
            get { return moduleName; }
        }

        #endregion Protected Properties

        #endregion Properties

        #region Methods

        #region Private Methods	

        private void LogDalcError(string methodName, string errorTitle, Exception e, params SqlCommand[] commands)
        {
            if (!Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_DB_ERROR, ModuleName, methodName)) return;
            string errorDescription = e.Message + ";";
            foreach (SqlCommand command in commands.Where(command => command != null))
            {
                errorDescription += "\n" + command.CommandText;
                errorDescription = command.Parameters.Cast<SqlParameter>().Aggregate(errorDescription,
                                                                                     (current, parameter) =>
                                                                                     current +
                                                                                     (" " + parameter.ParameterName +
                                                                                      "=" +
                                                                                      Convert.ToString(
                                                                                          (parameter.SqlDbType ==
                                                                                           SqlDbType.VarChar ||
                                                                                           parameter.SqlDbType ==
                                                                                           SqlDbType.UniqueIdentifier)
                                                                                              ? "'"
                                                                                              : "") + parameter.Value +
                                                                                      Convert.ToString(
                                                                                          (parameter.SqlDbType ==
                                                                                           SqlDbType.VarChar ||
                                                                                           parameter.SqlDbType ==
                                                                                           SqlDbType.UniqueIdentifier)
                                                                                              ? "'"
                                                                                              : "") + ","));
                errorDescription += "\n";
            }
            LogDalcError(methodName, errorTitle, errorDescription);
        }

        private void LogDalcError(string methodName, string errorTitle, Exception e)
        {
            if (Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_DB_ERROR, ModuleName, methodName))
            {
                LogDalcError(methodName, errorTitle, e.Message);
            }
        }

        private void LogDalcError(string methodName, string errorTitle, Exception e, string storedProcedureName,
                                  params object[] parameterValues)
        {
            if (!Event.IsEventToBeLogged(EventType.Error, ERROR_SEVERITY_DB_ERROR, ModuleName, methodName)) return;
            string errorDescription = e.Message + "; Stored Procedure Name: " + storedProcedureName + "; ";
            int i = 1;
            foreach (object parameter in parameterValues)
            {
                errorDescription += "Parameter" + Convert.ToString(i) + "=\"" + Convert.ToString(parameter) + "\", ";
                i++;
            }
            LogDalcError(methodName, errorTitle, errorDescription);
        }

        private void LogDalcError(string methodName, string errorTitle, string errorDescription)
        {
            IEvent logEvent = new Event(EventType.Error, DateTime.Now, ERROR_SEVERITY_DB_ERROR, ModuleName, methodName,
                                        errorTitle, errorDescription);
            logEvent.Save(false);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        ///   Fills the dataset.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "storedProcedureName">Name of the stored procedure.</param>
        /// <param name = "dataSet">The data set.</param>
        /// <param name = "tableNames">The table names.</param>
        /// <param name = "parameterValues">The parameter values.</param>
        public void FillDataset(string methodName, string errorName, string storedProcedureName, DataSet dataSet,
                                string[] tableNames, params object[] parameterValues)
        {
            try
            {
                SqlHelper.FillDataset(DBConnectionString, storedProcedureName, dataSet, tableNames, parameterValues);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, storedProcedureName, parameterValues);
                }
                throw;
            }
        }

        /// <summary>
        ///   Updates the dataset.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "insertCommand">The insert command.</param>
        /// <param name = "deleteCommand">The delete command.</param>
        /// <param name = "udpateCommand">The udpate command.</param>
        /// <param name = "dataSet">The data set.</param>
        /// <param name = "tableName">Name of the table.</param>
        public void UpdateDataset(string methodName, string errorName, SqlCommand insertCommand,
                                  SqlCommand deleteCommand, SqlCommand udpateCommand, DataSet dataSet, string tableName)
        {
            try
            {
                SqlHelper.UpdateDataset(insertCommand, deleteCommand, udpateCommand, dataSet, tableName);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, insertCommand, deleteCommand, udpateCommand);
                }
                throw;
            }
        }

        /// <summary>
        ///   Updates the data rows.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "insertCommand">The insert command.</param>
        /// <param name = "deleteCommand">The delete command.</param>
        /// <param name = "udpateCommand">The udpate command.</param>
        /// <param name = "dataRows">The data rows.</param>
        public void UpdateDataRows(string methodName, string errorName, SqlCommand insertCommand,
                                   SqlCommand deleteCommand, SqlCommand udpateCommand, DataRow[] dataRows)
        {
            try
            {
                SqlHelper.UpdateDataRows(insertCommand, deleteCommand, udpateCommand, dataRows);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, insertCommand, deleteCommand, udpateCommand);
                }
                throw;
            }
        }

        /// <summary>
        ///   Executes the non query.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "storedProcedureName">Name of the stored procedure.</param>
        /// <param name = "parameterValues">The parameter values.</param>
        public void ExecuteNonQuery(string methodName, string errorName, string storedProcedureName,
                                    params object[] parameterValues)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(DBConnectionString, storedProcedureName, parameterValues);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, storedProcedureName, parameterValues);
                }
                throw;
            }
        }

        /// <summary>
        ///   Executes the scalar.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "storedProcedureName">Name of the stored procedure.</param>
        /// <param name = "parameterValues">The parameter values.</param>
        /// <returns></returns>
        public object ExecuteScalar(string methodName, string errorName, string storedProcedureName,
                                    params object[] parameterValues)
        {
            object retVal;
            try
            {
                retVal = SqlHelper.ExecuteScalar(DBConnectionString, storedProcedureName, parameterValues);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, storedProcedureName, parameterValues);
                }
                throw;
            }
            return retVal;
        }

        /// <summary>
        ///   Executes the reader.
        /// </summary>
        /// <param name = "methodName">Name of the method.</param>
        /// <param name = "errorName">Name of the error.</param>
        /// <param name = "storedProcedureName">Name of the stored procedure.</param>
        /// <param name = "parameterValues">The parameter values.</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string methodName, string errorName, string storedProcedureName,
                                           params object[] parameterValues)
        {
            SqlDataReader sqlDataReader;
            try
            {
                sqlDataReader = SqlHelper.ExecuteReader(DBConnectionString, storedProcedureName, parameterValues);
            }
            catch (Exception e)
            {
                if (e.GetType().Name != SQL_EXCEPTION ||
                    (e.GetType().Name == SQL_EXCEPTION && ((SqlException) e).Number != ERROR_SERVER_NOT_AVAILABLE))
                {
                    LogDalcError(methodName, errorName, e, storedProcedureName, parameterValues);
                }
                throw;
            }
            return sqlDataReader;
        }

        #endregion Public Methods

        #endregion Methods
    }
}