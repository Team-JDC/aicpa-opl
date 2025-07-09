#region

using System.Data;
using System.Data.SqlClient;
using AICPA.Destroyer.Shared;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace AICPA.Destroyer.Content.Subscription
{
    ///<summary>
    ///  Contains a set of methods used to access the database for subscriptions.
    ///</summary>
    public class SubscriptionDalc : DestroyerDalc, ISubscriptionDalc
    {
        #region Constants

        #region Stored Procedures Names

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSUBSCRIPTION = "dbo.D_GetSubscription";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSUBSCRIPTIONS = "dbo.D_GetSubscriptions";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_GETSUBSCRIPTIONSBYBOOKNAME = "dbo.D_GetSubscriptionsByBookName";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSUBSCRIPTION = "dbo.D_InsertSubscription";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_DELETESUBSCRIPTION = "dbo.D_DeleteSubscription";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_INSERTSUBSCRIPTIONBOOK = "dbo.D_InsertSubscriptionBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_DELETESUBSCRIPTIONBOOK = "dbo.D_DeleteSubscriptionBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string SP_UPDATESUBSCRIPTION = "dbo.D_UpdateSubscription";

        #endregion

        #region Data Relations

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DR_SUBSCRIPTIONSUBSCRIPTIONBOOK = "SubscriptionSubscriptionBook";

        #endregion Data Relations

        #region Dalc Errors

        private const string ERROR_GETSUBSCRIPTION = "Error getting subscription information.";
        private const string ERROR_SAVE = "Error saving subscription information.";

        #endregion Dalc Errors

        #region Module and Method Names

        private const string MODULE_SUBSCRIPTIONDALC = "SubscriptionDalc";
        private const string METHOD_GETSUBSCRIPTION = "GetSubscription";
        private const string METHOD_SAVE = "Save";

        #endregion Module and Method Names

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDalc" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public SubscriptionDalc()
        {
            moduleName = MODULE_SUBSCRIPTIONDALC;
        }

        #endregion

        #region Methods

        #region Private Methods

        private void Save(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet,
                          string tableName)
        {
            UpdateDataset(METHOD_SAVE, ERROR_SAVE, insertCommand, deleteCommand, updateCommand, dataSet, tableName);
        }

        #endregion Private Methods

        #region ISubscriptionDalc Methods

        /// <summary>
        /// </summary>
        /// <param name = "subscriptionCode"></param>
        /// <returns></returns>
        public SubscriptionDs GetSubscription(string subscriptionCode)
        {
            SubscriptionDs subscriptionDs = new SubscriptionDs();
            FillDataset(METHOD_GETSUBSCRIPTION, ERROR_GETSUBSCRIPTION, SP_GETSUBSCRIPTION, subscriptionDs,
                        new string[2] {subscriptionDs.Subscription.TableName, subscriptionDs.SubscriptionBook.TableName},
                        subscriptionCode);
            return subscriptionDs;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public SubscriptionDs GetSubscriptions()
        {
            SubscriptionDs subscriptionDs = new SubscriptionDs();
            FillDataset(METHOD_GETSUBSCRIPTION, ERROR_GETSUBSCRIPTION, SP_GETSUBSCRIPTIONS, subscriptionDs,
                        new string[2] {subscriptionDs.Subscription.TableName, subscriptionDs.SubscriptionBook.TableName});
            return subscriptionDs;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public SubscriptionDs GetSubscriptions(string bookName)
        {
            SubscriptionDs subscriptionDs = new SubscriptionDs();
            FillDataset(METHOD_GETSUBSCRIPTION, ERROR_GETSUBSCRIPTION, SP_GETSUBSCRIPTIONSBYBOOKNAME, subscriptionDs,
                        new string[2] {subscriptionDs.Subscription.TableName, subscriptionDs.SubscriptionBook.TableName},
                        bookName);
            return subscriptionDs;
        }

        /// <summary>
        /// </summary>
        /// <param name = "subscriptionDs"></param>
        public void Save(SubscriptionDs subscriptionDs)
        {
            //subscription insert first
            SqlCommand insertSubscriptionCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                           SP_INSERTSUBSCRIPTION,
                                                                           subscriptionDs.Subscription.
                                                                               SubscriptionCodeColumn.ColumnName,
                                                                           subscriptionDs.Subscription.TitleColumn.
                                                                               ColumnName,
                                                                           subscriptionDs.Subscription.DescriptionColumn
                                                                               .ColumnName);
            UpdateDataRows(METHOD_SAVE, ERROR_SAVE, insertSubscriptionCommand, null, null,
                           subscriptionDs.Subscription.Select("", "", DataViewRowState.Added));

            //subscription book insert next
            SqlCommand insertSubscriptionBookCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                               SP_INSERTSUBSCRIPTIONBOOK,
                                                                               subscriptionDs.SubscriptionBook.
                                                                                   SubscriptionCodeColumn.ColumnName,
                                                                               subscriptionDs.SubscriptionBook.
                                                                                   BookNameColumn.ColumnName);
            UpdateDataRows(METHOD_SAVE, ERROR_SAVE, insertSubscriptionBookCommand, null, null,
                           subscriptionDs.SubscriptionBook.Select("", "", DataViewRowState.Added));

            //update the subscriptions
            SqlCommand updateSubscriptionCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                           SP_UPDATESUBSCRIPTION,
                                                                           subscriptionDs.Subscription.
                                                                               SubscriptionCodeColumn.ColumnName,
                                                                           subscriptionDs.Subscription.TitleColumn.
                                                                               ColumnName,
                                                                           subscriptionDs.Subscription.DescriptionColumn
                                                                               .ColumnName);
            UpdateDataRows(METHOD_SAVE, ERROR_SAVE, null, null, updateSubscriptionCommand,
                           subscriptionDs.Subscription.Select("", "", DataViewRowState.ModifiedCurrent));

            //subscription book deletion first
            SqlCommand deleteSubscriptionBookCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                               SP_DELETESUBSCRIPTIONBOOK,
                                                                               subscriptionDs.SubscriptionBook.
                                                                                   SubscriptionCodeColumn.ColumnName,
                                                                               subscriptionDs.SubscriptionBook.
                                                                                   BookNameColumn.ColumnName);
            UpdateDataRows(METHOD_SAVE, ERROR_SAVE, null, deleteSubscriptionBookCommand, null,
                           subscriptionDs.SubscriptionBook.Select("", "", DataViewRowState.Deleted));

            //subscripiton deletion next
            SqlCommand deleteSubscriptionCommand = SqlHelper.CreateCommand(new SqlConnection(DBConnectionString),
                                                                           SP_DELETESUBSCRIPTION,
                                                                           subscriptionDs.Subscription.
                                                                               SubscriptionCodeColumn.ColumnName);
            UpdateDataRows(METHOD_SAVE, ERROR_SAVE, null, deleteSubscriptionCommand, null,
                           subscriptionDs.Subscription.Select("", "", DataViewRowState.Deleted));
        }

        #endregion ISubscriptionDalc Methods

        #endregion
    }
}