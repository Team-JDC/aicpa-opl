#region

using System.Collections;
using System.Data;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Subscription
{
    /// <summary>
    ///   Summary description for SubscriptionCollection.
    /// </summary>
    public class SubscriptionCollection : DestroyerBpc, ISubscriptionCollection, IEnumerable
    {
        #region Constants

        #endregion Constants

        #region Private Properties

        /// <summary>
        ///   The active subscription dalc
        /// </summary>
        private SubscriptionDalc activeSubscriptionDalc;

        /// <summary>
        ///   The active subscription dataset
        /// </summary>
        private SubscriptionDs activeSubscriptionDs;

        private SubscriptionDalc ActiveSubscriptionDalc
        {
            get { return activeSubscriptionDalc ?? (activeSubscriptionDalc = new SubscriptionDalc()); }
        }

        private SubscriptionDs ActiveSubscriptionDs
        {
            get { return activeSubscriptionDs ?? (activeSubscriptionDs = new SubscriptionDs()); }
        }

        #endregion Private Properties

        #region Constructors

        /// <summary>
        ///   retrieves all subscriptions for the destroyer instance
        /// </summary>
        public SubscriptionCollection()
        {
            activeSubscriptionDs = ActiveSubscriptionDalc.GetSubscriptions();
        }

        /// <summary>
        ///   retrieves all subscriptions for the destroyer instance that contain a book with the specified name
        /// </summary>
        public SubscriptionCollection(string bookName)
        {
            activeSubscriptionDs = ActiveSubscriptionDalc.GetSubscriptions(bookName);
        }

        #endregion Constructors

        #region ISubscriptionCollection Properties

        /// <summary>
        ///   The number of subscriptions in the collection.
        /// </summary>
        public int Count
        {
            get { return ActiveSubscriptionDs.Subscription.Rows.Count; }
        }

        /// <summary>
        ///   Indexer for retrieving a subscription by ordinal value
        /// </summary>
        public ISubscription this[int index]
        {
            get { return new Subscription(((SubscriptionDs.SubscriptionRow) ActiveSubscriptionDs.Subscription.Rows[index])); }
        }

        /// <summary>
        ///   Indexer for retrieving a subscription by subscription code
        /// </summary>
        public ISubscription this[string subscriptionCode]
        {
            get
            {
                ISubscription retSubscription = null;
                //query our subscription table for a row containing the specified subscription code
                if (ActiveSubscriptionDs.Subscription.Rows.Count > 0)
                {
                    DataRow[] drs =
                        ActiveSubscriptionDs.Subscription.Select(string.Format("SubscriptionCode = '{0}'",
                                                                               subscriptionCode));
                    if (drs.Length > 0)
                    {
                        SubscriptionDs.SubscriptionRow subscriptionRow = (SubscriptionDs.SubscriptionRow) drs[0];
                        retSubscription = new Subscription(subscriptionRow);
                    }
                }
                return retSubscription;
            }
        }

        #endregion ISubscriptionCollection Properties

        #region ISubscriptionCollection Methods

        /// <summary>
        ///   Save changes to the subscription collection.
        /// </summary>
        public void Save()
        {
            ActiveSubscriptionDalc.Save(ActiveSubscriptionDs);
        }

        /// <summary>
        ///   Add a subscription to the subscription collection.
        /// </summary>
        /// <param name = "subscription"></param>
        public void Add(ISubscription subscription)
        {
            //pop the subscription into our collection's dataset
            SubscriptionDs.SubscriptionRow sr = ActiveSubscriptionDs.Subscription.AddSubscriptionRow(subscription.Code,
                                                                                                     subscription.Title,
                                                                                                     subscription.
                                                                                                         Description);

            //pop the booknames into our dataset
            foreach (string bookName in subscription.BookNames)
            {
                ActiveSubscriptionDs.SubscriptionBook.AddSubscriptionBookRow(sr, bookName);
            }
        }

        /// <summary>
        ///   Remove a subscription from the subscription collection.
        /// </summary>
        /// <param name = "subscription"></param>
        public void Remove(ISubscription subscription)
        {
            SubscriptionDs.SubscriptionRow sr =
                (SubscriptionDs.SubscriptionRow)
                ActiveSubscriptionDs.Subscription.Select(string.Format("SubscriptionCode='{0}'", subscription.Code))[0];
            sr.Delete();
        }

        #endregion ISubscriptionCollection Methods

        #region ISubscriptionCollection Members

        /// <summary>
        ///   return an IEnumerator object for enumerating through the subscription collection
        /// </summary>
        /// <returns>Object implementing IEnumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return new SubscriptionEnumerator(this);
        }

        #endregion

        #region Nested type: SubscriptionEnumerator

        /// <summary>
        ///   The SubscriptionEnumerator is a class that manages enumeration of a collection of subscriptions
        /// </summary>
        private class SubscriptionEnumerator : IEnumerator
        {
            private readonly SubscriptionCollection sc;
            private int index;

            /// <summary>
            ///   The constructor of our subscription enumerator.
            /// </summary>
            /// <param name = "SubscColl">The collection of subscriptions to enumerate</param>
            public SubscriptionEnumerator(SubscriptionCollection SubscColl)
            {
                sc = SubscColl;
                Reset();
            }

            #region IEnumerator Members

            /// <summary>
            ///   Reset our index
            /// </summary>
            public void Reset()
            {
                index = -1;
            }

            /// <summary>
            ///   Return the subscription row at the current index
            /// </summary>
            public object Current
            {
                get
                {
                    return
                        new Subscription(
                            ((SubscriptionDs.SubscriptionRow) sc.ActiveSubscriptionDs.Subscription.Rows[index]));
                }
            }

            /// <summary>
            ///   Advance our index
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                index++;
                return (index < sc.ActiveSubscriptionDs.Subscription.Rows.Count);
            }

            #endregion
        }

        #endregion
    }
}