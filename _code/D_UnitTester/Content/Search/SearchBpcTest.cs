using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using NUnit.Framework;

namespace AICPA.Destroyer.Content.Search
{
	public class SearchBpcTest : ContentShared
	{
	}

	[TestFixture]
	public class SearchGeneral : SearchBpcTest
	{
		/// <summary>
		/// Test search against a staging site index
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
        public void SearchGeneral_Search_Staging()
		{
			string siteName = Guid.NewGuid().ToString();
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string book3Name = Guid.NewGuid().ToString();
			string tempBookMakefile = CreateTestMakefile();
			try
			{
				//the site
				ISite site = new Site.Site(siteName, "AICPA reSOURCE",  "This electronic research tool combines the power and speed of the Web with comprehensive accounting and auditing standards.", "");
				site.Save();
				
				//the first book
				IBook book1 = new Book.Book(book1Name, "AICPA Industry Guide: Airlines", "This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee's recommendations on and descriptions of financial accounting and reporting principles and practices for airlines. The AICPA Accounting Standards Executive Committee has found this Guide to be consistent with existing standards and principles covered by Rules 202 and 203 of the AICPA Code of Professional Conduct. AICPA members should be prepared to justify departures from the accounting guidance in this Guide.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book1.Build();
				book1.Save();
				site.AddBook(book1);

				//the second book
				IBook book2 = new Book.Book(book2Name, "AICPA Audit and Accounting Guide: Casinos", "This Audit and Accounting Guide presents recommendations of the AICPA Gaming Industry Special Committee on the application of generally accepted auditing standards to audits of financial statements of casinos. This Guide also presents the Committee's recommendations on and descriptions of financial accounting and reporting principles and practice for casinos. The AICPA Accounting Standards Executive Committee has found this Guide to be consistent with existing standards and principles covered by Rules 202 and 203 of the AICPA Code of Professional Conduct. AICPA members should be prepared to justify departures from the accounting guidance in this Guide.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book2.Build();
				book2.Save();
				site.AddBook(book2);

				//the third book
				IBook book3 = new Book.Book(book3Name, "AICPA Professional Standards", "<i>AICPA Professional Standards</i> contains all of the outstanding pronouncements on professional standards issued by the AICPA, the International Federation of Accountants, and the International Accounting Standards Board. These pronouncements are arranged by subject, with amendments noted and superseded portions deleted.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book3.Build();
				book3.Save();
				site.AddBook(book3);

				//save the site
				site.Save();

				//get some site xml and use it to build the site
				string tempSiteMakefile = CreateTestSiteMakefile(book1.Id, book2.Id, book3.Id);
				StreamReader sr = new StreamReader(tempSiteMakefile, System.Text.Encoding.UTF8);
				string makeFileXml = sr.ReadToEnd();
				sr.Close();
				site.SiteTemplateXml = makeFileXml;
				site.RequestedStatus = SiteStatus.Staging;
				site.Save();
				site.Build();

                //now build the site index
				site.SiteIndex.Build();

				const string keywords = "aicpa";
				const SearchType searchType = SearchType.AllWords;
				const int maxHits = 100;
				const int pageSize = 10;
				const int pageOffset = 0;
				const string sortOrder = "content";

				ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);

                // NOTE: The below message was returned during step through.
                ISearchResults searchResults = site.SiteIndex.Search(searchCriteria);
                
                
				Console.WriteLine("There were " + searchResults.TotalHitCount + " total hits.");
				Console.WriteLine("There were " + searchResults.PageHitCount + " hits returned for this page.");
				Console.WriteLine("The hits returned for this page are:");

				foreach(IDocument doc in searchResults.Documents)
				{
					Console.WriteLine(doc.SiteReferencePath);
				}

				Console.WriteLine("The highlight terms are as follows:");
				foreach(string term in searchResults.WordInterpretations)
				{
					Console.WriteLine(term);
				}
			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
				DeleteNamedBook(book3Name);
				File.Delete(tempBookMakefile);
			}
		}
		/// <summary>
		/// Test search against a staging site index
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
         public void SearchGeneral_Search_Production()
		{
			string siteName = Guid.NewGuid().ToString();
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string book3Name = Guid.NewGuid().ToString();
			string tempBookMakefile = CreateTestMakefile();
			try
			{
				//the site
				ISite site = new Site.Site(siteName, "AICPA reSOURCE",  "This electronic research tool combines the power and speed of the Web with comprehensive accounting and auditing standards.", "");
				site.Save();
				
				//the first book
				IBook book1 = new Book.Book(book1Name, "AICPA Industry Guide: Airlines", "This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee's recommendations on and descriptions of financial accounting and reporting principles and practices for airlines. The AICPA Accounting Standards Executive Committee has found this Guide to be consistent with existing standards and principles covered by Rules 202 and 203 of the AICPA Code of Professional Conduct. AICPA members should be prepared to justify departures from the accounting guidance in this Guide.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book1.Build();
				book1.Save();
				site.AddBook(book1);

				//the second book
				IBook book2 = new Book.Book(book2Name, "AICPA Audit and Accounting Guide: Casinos", "This Audit and Accounting Guide presents recommendations of the AICPA Gaming Industry Special Committee on the application of generally accepted auditing standards to audits of financial statements of casinos. This Guide also presents the Committee's recommendations on and descriptions of financial accounting and reporting principles and practice for casinos. The AICPA Accounting Standards Executive Committee has found this Guide to be consistent with existing standards and principles covered by Rules 202 and 203 of the AICPA Code of Professional Conduct. AICPA members should be prepared to justify departures from the accounting guidance in this Guide.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book2.Build();
				book2.Save();
				site.AddBook(book2);

				//the third book
				IBook book3 = new Book.Book(book3Name, "AICPA Professional Standards", "<i>AICPA Professional Standards</i> contains all of the outstanding pronouncements on professional standards issued by the AICPA, the International Federation of Accountants, and the International Accounting Standards Board. These pronouncements are arranged by subject, with amendments noted and superseded portions deleted.", "Copyright &copy; American Institute of Certified Public Accountants, Inc. All Rights Reserved.", BookSourceType.Makefile, tempBookMakefile);
				book3.Build();
				book3.Save();
				site.AddBook(book3);

				//save the site
				site.Save();

				//get some site xml and use it to build the site
				string tempSiteMakefile = CreateTestSiteMakefile(book1.Id, book2.Id, book3.Id);
				StreamReader sr = new StreamReader(tempSiteMakefile, System.Text.Encoding.UTF8);
				string makeFileXml = sr.ReadToEnd();
				sr.Close();
				site.SiteTemplateXml = makeFileXml;
				site.RequestedStatus = SiteStatus.Staging;
				site.Save();
				site.Build();

				//now build the site index
				site.SiteIndex.Build();


				//now set us to production
				site.RequestedStatus = SiteStatus.Production;
				site.Save();
				site.Build();

				//now build the site index
				site.SiteIndex.Build();

				const string keywords = "aicpa";
				const SearchType searchType = SearchType.AllWords;
				const int maxHits = 100;
				const int pageSize = 10;
				const int pageOffset = 0;
				const string sortOrder = "content";

				ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);
				ISearchResults searchResults = site.SiteIndex.Search(searchCriteria);

				Console.WriteLine("There were " + searchResults.TotalHitCount + " total hits.");
				Console.WriteLine("There were " + searchResults.PageHitCount + " hits returned for this page.");
				Console.WriteLine("The hits returned for this page are:");

				foreach(IDocument doc in searchResults.Documents)
				{
					Console.WriteLine(doc.SiteReferencePath);
				}

				Console.WriteLine("The highlight terms are as follows:");
				foreach(string term in searchResults.WordInterpretations)
				{
					Console.WriteLine(term);
				}
			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
				DeleteNamedBook(book3Name);
				File.Delete(tempBookMakefile);
			}
		}
		/// <summary>
		/// Simple tests to make sure search criteria object private storage and retrieval is working
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
        public void SearchGeneral_SearchCriteriaBasicTest()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempBookMakefile = CreateTestMakefile();

			try
			{
				ISite site = new Site.Site(siteName, "title", "description", "");
				site.Save();
				IBook book = new Book.Book(bookName, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				book.Build();
				book.Save();
				site.AddBook(book);
				site.RequestedStatus = SiteStatus.Staging;
				site.Save();
				site.SiteIndex.Build();
				//sleep while the index is updating
				while(site.SiteIndex.Status == SiteIndexStatus.Updating)
				{
					Thread.Sleep((5000));
				}

				const string keywords = "this is a test query";
				const SearchType searchType = SearchType.AllWords;
				const int maxHits = 100;
				const int pageSize = 10;
				const int pageOffset = 0;
				const string sortOrder = "content";
				var extraParams = new System.Collections.Specialized.NameValueCollection
                                      {
                                          {"scope", "all"},
                                          {"test", "yes"}
                                      };

			    ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, extraParams);

				if(searchCriteria.Keywords != keywords || searchCriteria.SearchType != searchType || searchCriteria.MaxHits != maxHits || searchCriteria.PageSize != pageSize || searchCriteria.PageOffset != pageOffset || searchCriteria.SortOrder != sortOrder || searchCriteria.SearchParameters["scope"] != "all" || searchCriteria.SearchParameters["test"] != "yes")
				{
					throw new Exception("The search criteria properties were not properly saved.");
				}

				const string newKeywords = "this is the new query";
				searchCriteria.Keywords = newKeywords;

				if(searchCriteria.Keywords != newKeywords)
				{
					throw new Exception("Problem with search criteria storage");
				}

				var extraParams2 = new System.Collections.Specialized.NameValueCollection
			                           {
			                               {"runfast", "true"},
			                               {"break", "false"},
			                               {"all", "maybe"}
			                           };

			    searchCriteria.SearchParameters = extraParams2;
				if(searchCriteria.SearchParameters.Count != 3 || searchCriteria.SearchParameters["runfast"] != "true" || searchCriteria.SearchParameters["break"] != "false" || searchCriteria.SearchParameters["all"] != "maybe")
				{
					throw new Exception("Problem with search criteria storage of extra params.");
				}
	
			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookName);
				File.Delete(tempBookMakefile);
			}			
		}

        /// <summary>
		/// Test saving search to the database and retrieving it back
		/// </summary>
        [Test]
        public void SearchGeneral_Save_and_Retrieve_Search()
        {
            const string searchName = "My Test Saved Search 1";
            Guid userId = Guid.NewGuid();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);
            string sessionId = Guid.NewGuid().ToString();
            var cscUser = new User.User(userId, ReferringSite.Csc);

            const string keywords = "aicpa";
            const SearchType searchType = SearchType.AllWords;
            const int maxHits = 100;
            const int pageSize = 10;
            const int pageOffset = 0;
            const string sortOrder = "content";

            try
            {
                //user login
                cscUser.LogOn(sessionId, myDomain);

                // Create a dummy search and save it; test the Save method for first-time persistence of Search
                ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);
                searchCriteria.Save(searchName, userId);

                // Make sure SaveId comes back populated after Save call
                Assert.AreNotEqual(0, searchCriteria.SaveId, "Calling Save method on a SearchCriteria was expected to bring back a valid SearchId value from DB.");

                // Change a few values and see if they come through correctly; test the two versions of Save method for existing, already-persisted Searches
                int savedId = searchCriteria.SaveId;
                string newSearchName = "My Test Saved Search 2";
                int newPageSize = 50;
                searchCriteria.PageSize = newPageSize;
                searchCriteria.Save(newSearchName);

                int newPageOffset = 1;
                searchCriteria.PageOffset = newPageOffset;
                searchCriteria.Save();

                Assert.AreEqual(savedId, searchCriteria.SaveId, "Expected searchId in DB not to change on update.");
                Assert.AreEqual(newSearchName, searchCriteria.Name, "Expected new saved search name.");
                Assert.AreEqual(newPageSize, searchCriteria.PageSize, "Expected new search page size.");
                Assert.AreEqual(newPageOffset, searchCriteria.PageOffset, "Expected new search page offset.");

                // Now retrieve all saved searches from the DB for the dummy user; assert that there is only 1 row
                IEnumerable<ISearchCriteria> retrievedSearches = SearchCriteria.GetSavedSearchesForUser(userId);

                Assert.AreEqual(1, retrievedSearches.Count(), "Expected exactly one saved search to be retrieved for test user from DB.");

                // Get the one row and let's do some asserts on its properties
                ISearchCriteria retrievedSearch = retrievedSearches.First();

                Assert.AreEqual(searchCriteria.SaveId, retrievedSearch.SaveId, "Saved Search was expected to have been assigned a valid SearchId value from DB.");
                Assert.AreEqual(searchCriteria.UserId, retrievedSearch.UserId, "Saved UserId didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Name, retrievedSearch.Name, "Saved Name didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Excerpts, retrievedSearch.Excerpts, "Saved Excerpts flag didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.FilterUnsubscribed, retrievedSearch.FilterUnsubscribed, "Saved FilterUnsubscribed flag didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Keywords, retrievedSearch.Keywords, "Saved Keywords didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.MaxHits, retrievedSearch.MaxHits, "Saved MaxHits didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.PageOffset, retrievedSearch.PageOffset, "Saved PageOffset didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.PageSize, retrievedSearch.PageSize, "Saved PageSize didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.SearchType, retrievedSearch.SearchType, "Saved SearchType didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.SortOrder, retrievedSearch.SortOrder, "Saved SortOrder didn't match retrieved value.");

                // Now retrieve just the one search by Id that we saved earlier and do some asserts on its properties
                retrievedSearch = SearchCriteria.GetSavedSearchById(retrievedSearch.SaveId);

                Assert.AreNotEqual(0, retrievedSearch.SaveId, "Saved Search was expected to have been assigned a valid SearchId value from DB.");
                Assert.AreEqual(searchCriteria.SaveId, retrievedSearch.SaveId, "Saved SearchId (SaveId) didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.UserId, retrievedSearch.UserId, "Saved UserId didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Name, retrievedSearch.Name, "Saved Name didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Excerpts, retrievedSearch.Excerpts, "Saved Excerpts flag didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.FilterUnsubscribed, retrievedSearch.FilterUnsubscribed, "Saved FilterUnsubscribed flag didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.Keywords, retrievedSearch.Keywords, "Saved Keywords didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.MaxHits, retrievedSearch.MaxHits, "Saved MaxHits didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.PageOffset, retrievedSearch.PageOffset, "Saved PageOffset didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.PageSize, retrievedSearch.PageSize, "Saved PageSize didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.SearchType, retrievedSearch.SearchType, "Saved SearchType didn't match retrieved value.");
                Assert.AreEqual(searchCriteria.SortOrder, retrievedSearch.SortOrder, "Saved SortOrder didn't match retrieved value.");

                // Test deleting search
                SearchCriteria.DeleteSearchById(retrievedSearch.SaveId);
                Assert.IsNull(SearchCriteria.GetSavedSearchById(retrievedSearch.SaveId));
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }

        [Test] 
        public void SearchGeneral_Test_User_SavedSearchs_Collection_Usage()
        {
            const string searchName = "My Test Saved Search 1";
            Guid userId = Guid.NewGuid();
            string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
            C2bUser c2bUser = CreateC2bWebServiceUser(userId, myDomain);
            string sessionId = Guid.NewGuid().ToString();
            var cscUser = new User.User(userId, ReferringSite.Csc);

            const string keywords = "aicpa";
            const SearchType searchType = SearchType.AllWords;
            const int maxHits = 100;
            const int pageSize = 10;
            const int pageOffset = 0;
            const string sortOrder = "content";

            try
            {
                //user login
                cscUser.LogOn(sessionId, myDomain);

                // Create a dummy search and save it; test the Save method for first-time persistence of Search
                ISearchCriteria searchCriteria = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);
                searchCriteria.Save(searchName, userId);

                //DateTime lastModified = searchCriteria.LastModified;
                SearchCollection searches = cscUser.SavedSearches;

                // Make some baseline assertions first; SavedSearches on user object should start off with one search populated
                Assert.AreEqual(1, searches.Count, "Expected user's SavedSearches collection to have one search already persisted.");
                //Assert.AreEqual(lastModified, searches[searchName].LastModified, "Expected saved search last modified date to match retrieved value.");
                Assert.AreEqual(userId, searches[searchCriteria.SaveId].UserId, "Expected saved search userId to match retrieved value.");

                // This will test my IEquatable<ISearchCriteria> stuff
                Assert.True(searches.Contains(searchCriteria), "User's SavedSearches collection should contain the requested search.");

                // Clear the collection and assert
                searches.Clear();
                Assert.AreEqual(0, searches.Count, "Expected user's SavedSearches collection to be empty after Clear was called.");
                Assert.False(searches.Contains(searchCriteria), "Empty User's SavedSearches collection should not contain the requested search.");

                // Add back in via the collection and assert
                ISearchCriteria newSearch = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);
                searches.Add(newSearch, searchName, userId);
                Assert.AreEqual(1, searches.Count, "Expected user's SavedSearches collection to have one search after adding a new search in.");
                Assert.AreEqual(newSearch, searches[newSearch.SaveId], "Expected the one seach in the collection to match the one just persisted.");
                Assert.True(searches.Contains(newSearch), "User's SavedSearches collection should contain the requested search.");

                // Test adding in a duplicate search (one with same name and user); needs to fail
                ISearchCriteria duplicateSearch = new SearchCriteria(null, keywords, searchType, maxHits, pageSize, pageOffset, sortOrder, false, false, null);

                try
                {
                    searches.Add(duplicateSearch, searchName, userId);
                    throw new Exception("Inserting duplicate search should have thrown an excpetion bud didn't fail.");
                }
                catch (SqlException e)
                {
                    Assert.True(e.Message.Contains("Violation of UNIQUE KEY constraint") && e.Message.Contains("Cannot insert duplicate key"), 
                        "Exception thrown by duplicate saved search insertion attempt doesn't say expected words in exception message.");
                }
                catch (Exception e)
                {
                    throw new Exception("Attempting to insert duplicate saved search didn't throw the expected exception; instead it through " + e.Message);
                }
                
                Assert.AreEqual(0, duplicateSearch.SaveId, "Expected the duplicate SearchCriteria object to not get a SavedSearchId back from the database.");
                Assert.AreEqual(1, searches.Count, "Expected user's SavedSearches collection to still have only one search in it after attempting to add in a duplicate search (one with same name for same user).");
                Assert.False(searches.Contains(duplicateSearch), "Expected user's SavedSearches collection to not contain the duplicate search that was attempted to be added in.");

                // Test looping and update Save method
                int newMaxHits = 50;

                foreach (ISearchCriteria search in searches)
                {
                    search.MaxHits = newMaxHits;
                    search.Save();
                }

                Assert.AreEqual(1, searches.Count, "Expected user's SavedSearches collection to still have only one search in it.");
                Assert.AreEqual(newMaxHits, searches[newSearch.SaveId].MaxHits, "Expected MaxHits to have been updated when Save was called.");

                // Test Remove method
                searches.Remove(newSearch);
                Assert.AreEqual(0, searches.Count, "Expected user's SavedSearches collection to be empty after Remove was called.");
            }
            finally
            {
                DeleteC2bWebServiceUsers(c2bUser);
                DeleteDestroyerUsers(userId);
            }
        }
	}
}
