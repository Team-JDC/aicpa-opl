using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.Content.Subscription
{
	public class SubscriptionBpcTest : ContentShared
	{
	}

	[TestFixture]
	public class SubscriptionGeneral : SubscriptionBpcTest
	{
		/// <summary>
		/// create a new subscription, save it, make sure it gets stored, and then pull it back out
		/// </summary>
		[Test]
		public void SubscriptionGeneral_RemoveSubscription()
		{
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string subscriptionCode1 = Guid.NewGuid().ToString().Substring(0, 32); //truncate since subs codes can't be over 32 characters
			string subscriptionCode2 = Guid.NewGuid().ToString().Substring(0, 32); //truncate since subs codes can't be over 32 characters
			try
			{
				//create a couple of books
				IBook book1 = new Book.Book(book1Name, "Title", "Desc", "Copyright", BookSourceType.Makefile, "");
				book1.Save();
				IBook book2 = new Book.Book(book2Name, "Title", "Desc", "Copyright", BookSourceType.Makefile, "");
				book2.Save();

				//build an array of book names
				string[] bookArray1 = {book1.Name, book2.Name};
				
				//create a couple of new subscriptions
				ISubscription subscription1 = new Subscription(subscriptionCode1, bookArray1);
				subscription1.Save();
				ISubscription subscription2 = new Subscription(subscriptionCode2, bookArray1);
				subscription2.Save();

				//now retrieve all subscriptions from the database
				ISubscriptionCollection subscriptions = new SubscriptionCollection();

				int subscriptionCountBefore = subscriptions.Count;

				//now remove one of the subscriptions
				subscriptions.Remove(subscription2);
				subscriptions.Save();

				//pull back the subscriptions to make sure the removal occured and was persisted
				ISubscriptionCollection subscriptionsPullback = new SubscriptionCollection();
				int subscriptionCountAfter = subscriptionsPullback.Count;

				//error if subscription counts do not match up
				if(subscriptionCountBefore-1 != subscriptionCountAfter)
				{
					throw new Exception("Subscription removal failed.");
				}

			}
			finally
			{
				DeleteNamedSubscription(subscriptionCode1);
				DeleteNamedSubscription(subscriptionCode2);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
			}


		}
		/*
		/// <summary>
		/// create a new subscription, save it, make sure it gets stored, and then pull it back out
		/// </summary>
		[Test]
		public void SubscriptionGeneral_SaveSubscription()
		{
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string book3Name = Guid.NewGuid().ToString();
			string book4Name = Guid.NewGuid().ToString();
			string subscriptionCode1 = Guid.NewGuid().ToString().Substring(0, 32); //truncate since subs codes can't be over 32 characters
			string subscriptionCode2 = Guid.NewGuid().ToString().Substring(0, 32); //truncate since subs codes can't be over 32 characters
			string subscriptionCode3 = Guid.NewGuid().ToString().Substring(0, 32); //truncate since subs codes can't be over 32 characters
			string tempMakefile = CreateTestMakefile();
			try
			{
				//find out the initial number of subscriptions in the collection
				ISubscriptionCollection subscriptions1 = new SubscriptionCollection();
				int subsCount1 = subscriptions1.Count;

				//create a couple of books
				IBook book1 = new Book.Book(book1Name, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book1.Save();
				IBook book2 = new Book.Book(book2Name, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book2.Save();
				IBook book3 = new Book.Book(book3Name, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book3.Save();

                //build an array of book names
				ArrayList books1 = new ArrayList();
				books1.Add(book1Name);
				books1.Add(book2Name);
				ArrayList books2 = new ArrayList();
				books2.Add(book3Name);

                //create a new subscriptions from our book arrays
		        ISubscription subscription1 = new Subscription(subscriptionCode1, (string[])books1.ToArray(typeof(string)));
				ISubscription subscription2 = new Subscription(subscriptionCode2, (string[])books2.ToArray(typeof(string)));

				//save our subscriptions
				subscription1.Save();
				subscription2.Save();

				//make sure the subscription stuff went to the db
				CheckDBRecordExists("D_Subscription", "SubscriptionCode='" + subscriptionCode1 + "'");
				CheckDBRecordExists("D_SubscriptionBook DSB INNER JOIN D_Book DB ON DB.BookId = DSB.BookId ", "DB.[Name]='" + book1.Name + "'");
				CheckDBRecordExists("D_SubscriptionBook DSB INNER JOIN D_Book DB ON DB.BookId = DSB.BookId ", "DB.[Name]='" + book2.Name + "'");

				//now pull one back out and check-check-check it
				ISubscription subscriptionPullback = new Subscription(subscriptionCode1);

				// do the codes match?
				if(subscriptionPullback.Code != subscriptionCode1)
				{
					throw new Exception("The expected subscritpion code was not found in the subscription.");
				}

				//are the expected books in the subscription?
				ArrayList bookArrayList = new ArrayList(subscriptionPullback.BookNames);
				if(!(bookArrayList.Contains(book1Name) && bookArrayList.Contains(book2Name)) || bookArrayList.Count != 2)
				{
					throw new Exception("The expected books were not found in the subscription.");
				}

				//now pull back all subscriptions so we can test the collection
				ISubscriptionCollection subscriptions2 = new SubscriptionCollection();
				int subsCount2 = subscriptions2.Count;
				if(subsCount1 != subsCount2-2)
				{
					throw new Exception("The expected number of subscriptions were not returned.");
				}

				//test enumeration
				Hashtable subs = new Hashtable();
				foreach(ISubscription subscription in subscriptions2)
				{

					subs.Add(subscription.Code, new ArrayList());
					Console.WriteLine("The subscription code '" + subscription.Code + "' contains the following books: ");
					foreach(string bookName in subscription.BookNames)
					{
						Console.WriteLine(bookName);
						ArrayList books = (ArrayList)subs[subscription.Code];
						books.Add(bookName);
					}
				}
				if(!subs.Contains(subscriptionCode1) || !subs.Contains(subscriptionCode2))
				{
					throw new Exception("The expected specific subscriptions were not found.");
				}

				ArrayList subs1Books = (ArrayList)subs[subscriptionCode1];
				ArrayList subs2Books = (ArrayList)subs[subscriptionCode2];
				if(subs1Books.Count != 2 || !subs1Books.Contains(book1Name) || !subs1Books.Contains(book2Name) || subs2Books.Count != 1 || !subs2Books.Contains(book3Name))
				{
					throw new Exception("The expected specific books were not found.");
				}

				//now update the subscription book list on the pullback and resave
				ArrayList newSubscriptionBooks = new ArrayList();
				newSubscriptionBooks.Add(book3Name);
				subscriptionPullback.BookNames = (string[])newSubscriptionBooks.ToArray(typeof(string));
				subscriptionPullback.Save();

				ISubscription subscriptionPullback2 = new Subscription(subscriptionCode1);
				ArrayList tempArray = new ArrayList(subscriptionPullback2.BookNames);
				if(!(tempArray.Contains(book1Name) && tempArray.Contains(book2Name) && tempArray.Contains(book3Name)) || subscriptionPullback2.BookNames.Length != 3)
				{
					throw new Exception("Expected books not found in subscription.");
				}

				//now add a subscription to our collection, check the count, and pull back the subscription to make sure it was saved
				ArrayList books3 = new ArrayList();
				books3.Add(book1Name);
				books3.Add(book2Name);
				books3.Add(book3Name);
				ISubscription subscription3 = new Subscription(subscriptionCode3, (string[])books3.ToArray(typeof(string)));
				subscriptions2.Add(subscription3);
				subscriptions2.Save();



				if(subsCount2 != subscriptions2.Count-1)
				{
					throw new Exception("The expected number of subscriptions were not returned.");
				}

				ISubscription subscriptionPullback3 = new Subscription(subscriptionCode3);
				ArrayList tempArray3 = new ArrayList(subscriptionPullback3.BookNames);
				if(!(tempArray3.Contains(book1Name) && tempArray3.Contains(book2Name) && tempArray3.Contains(book3Name)) || subscriptionPullback3.BookNames.Length != 3)
				{
					throw new Exception("Expected books not found in subscription.");
				}


			}
			finally
			{
				DeleteNamedSubscription(subscriptionCode1);
				DeleteNamedSubscription(subscriptionCode2);
				DeleteNamedSubscription(subscriptionCode3);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
				DeleteNamedBook(book3Name);
				File.Delete(tempMakefile);
			}
		}*/
	}
}
