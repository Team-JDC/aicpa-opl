#region

using System;
using System.Collections;
using System.Data;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content.Subscription
{
    /// <summary>
    ///   Summary description for Subscription.
    /// </summary>
    public class Subscription : DestroyerBpc, ISubscription
    {
        #region Constants

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_SUBSCRIPTIONNOTFOUND = "A subscription did not exist for subscription code '{0}'";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKDUPLICATEDINSUBSCRIPTION =
            "The subscription '{0}' already contains the book '{1}'";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_BOOKNOTFOUNDINSUBSCRIPTION = "The subscription '{0}' does not contain the book '{1}'";

        #endregion Constants

        #region Private

        //private storage for book names
        private SubscriptionDs.SubscriptionBookRow activeSubscriptionBookRow;

        /// <summary>
        ///   A dalc to use for database management of this dataset
        /// </summary>
        private SubscriptionDalc activeSubscriptionDalc;

        /// <summary>
        ///   A subscription dataset to use for our private storage of subscription data
        /// </summary>
        private SubscriptionDs activeSubscriptionDs;

        //private storage for our subscription codes
        private SubscriptionDs.SubscriptionRow activeSubscriptionRow;

        private SubscriptionDs.SubscriptionBookRow ActiveSubscriptionBookRow
        {
            get {
                return activeSubscriptionBookRow ??
                       (activeSubscriptionBookRow =
                        ActiveSubscriptionDs.SubscriptionBook.AddSubscriptionBookRow(ActiveSubscriptionRow, string.Empty));
            }
        }

        private SubscriptionDs.SubscriptionRow ActiveSubscriptionRow
        {
            get
            {
                return activeSubscriptionRow ??
                       (activeSubscriptionRow = ActiveSubscriptionDs.Subscription.AddSubscriptionRow(EMPTY_STRING,
                                                                                                     EMPTY_STRING,
                                                                                                     EMPTY_STRING));
            }
        }

        private SubscriptionDs ActiveSubscriptionDs
        {
            get
            {
                //if we are null it means someone is creating a new subscription, so create a new empty subscription dataset
                return activeSubscriptionDs ?? (activeSubscriptionDs = new SubscriptionDs());
            }
        }

        private SubscriptionDalc ActiveSubscriptionDalc
        {
            get { return activeSubscriptionDalc ?? (activeSubscriptionDalc = new SubscriptionDalc()); }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor for retrieving a subscription from the database. Pulls subscription from the database and sets the active dataset and the active table rows.
        /// </summary>
        public Subscription(string subscriptionCode)
        {
            //get the specified subscription from the database
            activeSubscriptionDs = ActiveSubscriptionDalc.GetSubscription(subscriptionCode);

            //make sure we got something back...
            if (activeSubscriptionDs.Subscription.Rows.Count > 0)
            {
                // set our active subscription row to the first row in the returned subscription datatable
                activeSubscriptionRow = (SubscriptionDs.SubscriptionRow) activeSubscriptionDs.Subscription.Rows[0];
            }
            else
            {
                //throw an error if the subscription was not in the databse
                throw new Exception(string.Format(ERROR_SUBSCRIPTIONNOTFOUND, subscriptionCode));
            }

            //it is possible (and acceptable) to get back a subscription that has no books associated with it
            if (activeSubscriptionDs.SubscriptionBook.Rows.Count > 0)
            {
                //set the active subscription book row to the first 
                activeSubscriptionBookRow =
                    (SubscriptionDs.SubscriptionBookRow) activeSubscriptionDs.SubscriptionBook.Rows[0];
            }
        }

        /// <summary>
        ///   Constructor for creating a new subscription.
        /// </summary>
        /// <param name = "subscriptionCode"></param>
        /// <param name = "bookList"></param>
        public Subscription(string subscriptionCode, string[] bookList)
        {
            //set the subscription code on our active subscription row
            ActiveSubscriptionRow.SubscriptionCode = subscriptionCode;

            //pop our books into the subscription book row
            foreach (string bookName in bookList)
            {
                ActiveSubscriptionDs.SubscriptionBook.AddSubscriptionBookRow(ActiveSubscriptionRow, bookName);
            }
        }

        ///<summary>
        ///  Creates a new subscription object using a datarow provided by a SubscriptionCollection.
        ///  Note that the access visibility is "internal."  This constructor is intended for
        ///  use only by SubscriptionCollection; users of this API can't use this constructor 
        ///  themselves.  Note also that this object's dataset will be a reference to the
        ///  dataset in the SubscriptionCollection that spawned it - it doesn't "belong" to this Subscription
        ///  object directly in this case.
        ///</summary>
        ///<param name = "subscriptionRow"></param>
        internal Subscription(SubscriptionDs.SubscriptionRow subscriptionRow)
        {
            activeSubscriptionRow = subscriptionRow;
            //it is possible for a subscription to exist without any books, so be careful here around [0] assumptions
            DataRow[] drs = activeSubscriptionRow.GetChildRows(SubscriptionDalc.DR_SUBSCRIPTIONSUBSCRIPTIONBOOK);
            if (drs.Length > 0)
            {
                activeSubscriptionBookRow = (SubscriptionDs.SubscriptionBookRow) drs[0];
            }
            activeSubscriptionDs = (SubscriptionDs) activeSubscriptionRow.Table.DataSet;
        }

        #endregion

        #region ISubscription Members

        /// <summary>
        /// </summary>
        public string Code
        {
            get { return ActiveSubscriptionRow.SubscriptionCode; }
        }

        /// <summary>
        /// </summary>
        public string Title
        {
            get { return ActiveSubscriptionRow.Title; }

            set { ActiveSubscriptionRow.Title = value; }
        }

        /// <summary>
        /// </summary>
        public string Description
        {
            get { return ActiveSubscriptionRow.Description; }

            set { ActiveSubscriptionRow.Description = value; }
        }

        /// <summary>
        /// </summary>
        public string[] BookNames
        {
            get
            {
                //create an arraylist and squirt our book names into it from the subscription book table in our dataset
                ArrayList bookList = new ArrayList();
                foreach (
                    SubscriptionDs.SubscriptionBookRow subscriptionBookRow in
                        ActiveSubscriptionRow.GetChildRows(SubscriptionDalc.DR_SUBSCRIPTIONSUBSCRIPTIONBOOK))
                {
                    bookList.Add(subscriptionBookRow.BookName);
                }

                return (string[]) bookList.ToArray(typeof (string));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "bookName"></param>
        public void AddBook(string bookName)
        {
            ArrayList newBookList = new ArrayList();
            foreach (string s in BookNames)
            {
                newBookList.Add(s);
            }
            if (!newBookList.Contains(bookName))
            {
                ActiveSubscriptionDs.SubscriptionBook.AddSubscriptionBookRow(ActiveSubscriptionRow, bookName);
            }
            else
            {
                throw new BusinessRuleException(string.Format(ERROR_BOOKDUPLICATEDINSUBSCRIPTION, Title, bookName));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "bookName"></param>
        public void RemoveBook(string bookName)
        {
            ArrayList newBookList = new ArrayList();
            foreach (string s in BookNames)
            {
                newBookList.Add(s);
            }
            if (newBookList.Contains(bookName))
            {
                ActiveSubscriptionDs.SubscriptionBook.FindBySubscriptionCodeBookName(Code, bookName).Delete();
            }
            else
            {
                throw new BusinessRuleException(string.Format(ERROR_BOOKNOTFOUNDINSUBSCRIPTION, Title, bookName));
            }
        }

        /// <summary>
        /// </summary>
        public void Save()
        {
            //use the dalc to save our subscription
            ActiveSubscriptionDalc.Save(ActiveSubscriptionDs);
        }

        #endregion
    }
}