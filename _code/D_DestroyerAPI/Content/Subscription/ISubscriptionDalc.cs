namespace AICPA.Destroyer.Content.Subscription
{
    /// <summary>
    ///   Summary description for ISubscriptionDALC.
    /// </summary>
    public interface ISubscriptionDalc
    {
        ///<summary>
        ///  Get a single subscription based on its code.
        ///</summary>
        ///<param name = "subscriptionCode">The code of the subscription to be returned.</param>
        ///<returns>A strongly-typed dataset containing subscription rows.</returns>
        SubscriptionDs GetSubscription(string subscriptionCode);

        ///<summary>
        ///  Get all the subscriptions.
        ///</summary>
        ///<returns>A strongly-typed datatable containing subscription rows.</returns>
        SubscriptionDs GetSubscriptions();

        ///<summary>
        ///  Get all the subscriptions that contain the specified book name
        ///</summary>
        ///<param name = "bookName">The name of the book that the retrieved subscriptions should contain.</param>
        ///<returns>A strongly-typed datatable containing subscription rows.</returns>
        SubscriptionDs GetSubscriptions(string bookName);

        ///<summary>
        ///  Insert, Update, or Delete a subscription or subscriptions based on the dataset provided.
        ///</summary>
        ///<param name = "subscriptionDs">A strongly-typed datatable containing subscription rows.</param>
        void Save(SubscriptionDs subscriptionDs);
    }
}