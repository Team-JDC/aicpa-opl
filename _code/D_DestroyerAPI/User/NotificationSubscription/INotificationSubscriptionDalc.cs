#region

using System;

#endregion

namespace AICPA.Destroyer.User.NotificationSubscription
{
    /// <summary>
    ///   Summary description for INotificationSubscription.
    /// </summary>
    public interface INotificationSubscriptionDalc
    {
        ///<summary>
        ///  Get notification subscriptions based on the userId.
        ///</summary>
        ///<param name = "userId">The Id of the user who is subscribed to the notifications to be returned.</param>
        ///<returns>A strongly-typed datatable containing Notification Subscription rows.</returns>
        NotificationSubscriptionDS.NotificationSubscriptionDataTable GetNotificationSubscriptions(Guid userId);

        ///<summary>
        ///  Get notification subscriptions based on the bookId.
        ///</summary>
        ///<param name = "bookId">The bookId of the book that has been updated.</param>
        ///<param name = "bookVersion">The version of the book that has been updated.</param>
        ///<returns>A strongly-typed datatable containing Notification Subscription rows.</returns>
        NotificationSubscriptionDS.NotificationSubscriptionDataTable GetNotificationSubscriptions(string bookId,
                                                                                                  int bookVersion);

        ///<summary>
        ///  Insert, Update, or Delete a NotificationSubscription or NotificationSubscriptions based on the dataset provided.
        ///</summary>
        ///<param name = "notificationSubscriptionDataTable">A strongly-typed datatable containing Notification Subscription rows.</param>
        void Save(NotificationSubscriptionDS.NotificationSubscriptionDataTable notificationSubscriptionDataTable);
    }
}