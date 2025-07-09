#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.Content.Subscription
{
    /// <summary>
    ///   ISubscriptionCollection is an interface for managing a collection of subscriptions.
    /// </summary>
    public interface ISubscriptionCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        ///   The number of subscriptions in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   Indexer for retrieving a subscription by ordinal value.
        /// </summary>
        ISubscription this[int index] { get; }

        /// <summary>
        ///   Indexer for retrieving a subscription by subscription code.
        /// </summary>
        ISubscription this[string name] { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Save changes to the ISubscriptionCollection.
        /// </summary>
        void Save();

        /// <summary>
        ///   Add a subscription to the subscription collection.
        /// </summary>
        /// <param name = "subscription"></param>
        void Add(ISubscription subscription);

        /// <summary>
        ///   Remove a subscription from the subscription collection.
        /// </summary>
        /// <param name = "subscription"></param>
        void Remove(ISubscription subscription);

        #endregion Methods
    }
}