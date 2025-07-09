using System;
using System.Threading;
using System.Collections;
using System.IO;
using System.Xml;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.Content.Book
{
	public class BookBpcTest : ContentShared
	{
	}
	[TestFixture]
	public class BookGeneral : BookBpcTest
	{
		/// <summary>
		/// This is just for test-loading content
		/// </summary>
		[Test]
		public void BookGeneral_FasbBookBuild()
		{
			try
			{
//				//create an object for each book
//				string baseDir = @"\\mandelbaum\d$\projects\aicpa\destroyerconvert\outputmak";
//				IBook ack = new Book("ack", "title", "desc", "copy", BookSourceType.Makefile, baseDir + @"\ack\ack.mak");
//				IBook amop = new Book("amop", "amop", "amop", "amop", BookSourceType.Makefile, baseDir + @"\amop\amop.mak");
//				IBook exh = new Book("exh", "exh", "exh", "exh", BookSourceType.Makefile, baseDir + @"\exh\exh.mak");
//				IBook fasbct = new Book("fasb-ct", "fasb-ct", "fasb-ct", "fasb-ct", BookSourceType.Makefile, baseDir + @"\fasb-ct\fasb-ct.mak");
//				IBook fasbop = new Book("fasb-op", "fasb-op", "fasb-op", "fasb-op", BookSourceType.Makefile, baseDir + @"\fasb-op\fasb-op.mak");
//				IBook fasbqa = new Book("fasb-qa", "fasb-qa", "fasb-qa", "fasb-qa", BookSourceType.Makefile, baseDir + @"\fasb-qa\fasb-qa.mak");
//				IBook part1 = new Book("part1", "part1", "part1", "part1", BookSourceType.Makefile, baseDir + @"\part1\part1.mak");
//				IBook part2 = new Book("part2", "part2", "part2", "part2", BookSourceType.Makefile, baseDir + @"\part2\part2.mak");
//				IBook part3 = new Book("part3", "part3", "part3", "part3", BookSourceType.Makefile, baseDir + @"\part3\part3.mak");
//
//				//build each book
//				ack.Build();
//				amop.Build();
//				exh.Build();
//				fasbct.Build();
//				fasbop.Build();
//				fasbqa.Build();
//				part1.Build();
//				part2.Build();
//				part3.Build();

			}
			finally
			{
			}
		}

		[Test]
		public void BookGeneral_BookTocXml()
		{
			const int EXPECTED_NODE_COUNT = 49;
			const int EXPECTED_ATT_COUNT = 249;

			string bookName1 = Guid.NewGuid().ToString();
			string bookName2 = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{			
				IBook book1 = new Book(bookName1, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				book1.Build();
				book1.Save();
				
				//make sure the book built properly
				if(book1.BuildStatus == BookBuildStatus.Error)
				{
					throw new Exception("There was an error building the books.");
				}

				//now request the book xml and write it to stdout
				string book1BookXml = book1.BookXml;
				Console.WriteLine("book1.BookXml:\n" + book1BookXml);
				XmlDocument book1XmlDoc = new XmlDocument();
				book1XmlDoc.LoadXml(book1BookXml);

				XmlNodeList allNodes = book1XmlDoc.SelectNodes("//*");
				XmlNodeList allAttributes = book1XmlDoc.SelectNodes("//@*");

				Console.WriteLine("node count: " + allNodes.Count);
				Console.WriteLine("att count: " + allAttributes.Count);

				if(allNodes.Count != EXPECTED_NODE_COUNT || allAttributes.Count != EXPECTED_ATT_COUNT)
				{
					throw new Exception("The expected number of nodes or attributes was not found in the book XML");
				}

			}
			finally
			{
				DeleteNamedBook(bookName1);
				DeleteNamedBook(bookName2);
				File.Delete(tempMakefile);
			}
		}
		
		[Test]
		public void BookGeneral_EditProductionBook()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			try
			{
				//create a site and save
				ISite site = new Site.Site(siteName, siteName, siteName, siteName);
				site.Save();
                
				//create a book and save
				IBook book = new Book(bookName, bookName, bookName, bookName, BookSourceType.Makefile, bookName);
				book.Save();

				//add the book to the site
				site.AddBook(book);

				//set the site to production
				site.Status = SiteStatus.Production;
				site.Save();

				IBook bookpullback = new Book(book.Id);

				//now try to edit the book's name property
				string error = "";
				try
				{
					bookpullback.Name = "test";
				}
				catch(Exception e)
				{
					error = e.Message;
				}

				//make sure we got the error we were expecting
				if(error != Book.ERROR_BOOKNOTEDITABLE)
				{
					throw new Exception("Expected exception not thrown when modifying production site.");
				}                

				//make sure book build status is NotBuilt
				if(bookpullback.BuildStatus != BookBuildStatus.NotBuilt)
				{
					throw new Exception("Expected BookBuildStatus to be NotBuilt.");
				}

				//now try a book build
				bookpullback.Build();
				if(bookpullback.BuildStatus != BookBuildStatus.Error)
				{
					throw new Exception("Expected BookBuildStatus to be Error.");
				}                

				//also try pulling back by book collection on the site
				IBook bookpullback2 = site.Books[book.Name];
				error = "";
				try
				{
					bookpullback2.Name = "test";
				}
				catch(Exception e)
				{
					error = e.Message;
				}

				//make sure we got the error we were expecting
				if(error != Book.ERROR_BOOKNOTEDITABLE)
				{
					throw new Exception("Expected exception not thrown when modifying production site.");
				}                

				//this is the same book, so make sure his book build status is still Error
				if(bookpullback2.BuildStatus != BookBuildStatus.Error)
				{
					throw new Exception("Expected BookBuildStatus to be Error.");
				}                

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookName);
			}
		}



		/// <summary>
		/// Test the next/previous document functionality
		/// </summary>
		[Test]
		public void BookGeneral_NextPreviousDocument()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName1 = Guid.NewGuid().ToString();
			string bookName2 = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site.Site(siteName, "title", "desc", "");
				site.Save();
				
				IBook book1 = new Book(bookName1, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				book1.Build();
				book1.Save();
				
				IBook book2 = new Book(bookName2, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				book2.Build();
				book2.Save();

				//make sure both of our books built properly
				if(book1.BuildStatus == BookBuildStatus.Error || book2.BuildStatus == BookBuildStatus.Error)
				{
					throw new Exception("There was an error building the books.");
				}

                site.AddBook(book1);
				site.AddBook(book2);

                site.Save();

				IDocument firstDocumentOfFirstBook = site.Books[0].Documents[0];
                IDocument secondDocumentOfFirstBook = site.Books[0].Documents[1];
				IDocument lastDocumentOfFirstBook = site.Books[0].Documents[site.Books[0].Documents.Count-1];
				IDocument firstDocumentOfLastBook = site.Books[1].Documents[0];
				IDocument lastDocumentOfLastBook = site.Books[1].Documents[site.Books[1].Documents.Count-1];

				Thread.Sleep(5000); //let things catch up here (?)

                IDocument validNextDoc = site.GetNextDocument(firstDocumentOfFirstBook);
                if (validNextDoc.Id != secondDocumentOfFirstBook.Id)
                {
                    throw new Exception("The expected document was not found on GetNextDocument test.");
                }

                IDocument validPreviousDoc = site.GetPreviousDocument(secondDocumentOfFirstBook);
                if (validPreviousDoc.Id != firstDocumentOfFirstBook.Id)
                {
                    throw new Exception("The expected document was not found on GetPreviousDocument test.");
                }

				IDocument nextDoc = site.GetNextDocument(lastDocumentOfFirstBook);
                if (nextDoc != null)
                {
                    throw new Exception("A document was returned instead of null from GetNextDocument.");
                }
                /*if (nextDoc.Id == firstDocumentOfLastBook.Id)
                {
                    throw new Exception("The expected document was not found on GetNextDocument test.");
                }*/

				IDocument prevDoc = site.GetPreviousDocument(firstDocumentOfLastBook);
                if (nextDoc != null)
                {
                    throw new Exception("A document was returned instead of null from GetPreviousDocument.");
                }
				/*if(prevDoc.Id == lastDocumentOfFirstBook.Id)
				{
					throw new Exception("The expected document was not found on GetNextDocument test.");
				}*/

                IDocument prevDoc2 = site.GetPreviousDocument(firstDocumentOfFirstBook);
                if (nextDoc != null)
                {
                    throw new Exception("A document was returned instead of null from GetPreviousDocument.");
                }
                /*if (prevDoc2.Id == lastDocumentOfLastBook.Id)
				{
					throw new Exception("Expected previous document to be null.");
				}*/

                IDocument nextDoc2 = site.GetNextDocument(lastDocumentOfLastBook);
                if (nextDoc != null)
                {
                    throw new Exception("A document was returned instead of null from GetNextDocument.");
                }
				/*if(nextDoc2.Id == firstDocumentOfFirstBook.Id)
				{
					throw new Exception("Expected next document to be null.");
				}*/
			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookName1);
				DeleteNamedBook(bookName2);
				File.Delete(tempMakefile);
			}
		}
		/// <summary>
		/// Build the book with a makefile that does not exist
		/// </summary>
		[Test]
		public void BookGeneral_BuildBookWithMissingMakefile()
		{
			string bookName = Guid.NewGuid().ToString();
			string bogusMakefile = "q:\\missing.mak";
			try
			{
				IBook book = new Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, bogusMakefile);
				book.Build();

				//make sure the book build failed
				if(book.BuildStatus != BookBuildStatus.Error)
				{
					throw new Exception("Book.Build failed to detect missing makefile.");
				}
			}
			finally
			{
				DeleteNamedBook(bookName);
			}		
		}
		/// <summary>
		/// Build the book with a makefile that is malformed XML
		/// </summary>
		[Test]
		public void BookGeneral_BuildBookWithMalformedMakefile()
		{
			string bookName = Guid.NewGuid().ToString();
			string malformedMakefile = CreateTestMakefile("&");
			try
			{
				IBook book = new Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, malformedMakefile);
				book.Build();

				//make sure the book build failed
				if(book.BuildStatus != BookBuildStatus.Error)
				{
					throw new Exception("Book.Build failed to detect malformed makefile.");
				}
			}
			finally
			{
				DeleteNamedBook(bookName);
			}		
		}
		/// <summary>
		/// get a list of all books in the datastore
		/// </summary>
		[Test]
		public void BookGeneral_GetAllBooks()
		{
			string bookName = Guid.NewGuid().ToString();
			string makefile = CreateTestMakefile();
			try
			{
				IBookCollection bookCollection1 = new BookCollection(false, true);
				int bookColl1Count = bookCollection1.Count;

				IBook book = new Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, makefile);
				book.Build();
				book.Save();

				IBookCollection bookCollection2 = new BookCollection(false, true);
				int bookColl2Count = bookCollection2.Count;

				if(bookColl1Count != bookColl2Count-1)
				{
					throw new Exception("Book collection before and after count were not supposed to be equal.");
				}

				//retrieve books by status
				IBookCollection errorBooks = new BookCollection(BookBuildStatus.Error);
				int errorCount = (int)GetDBScalarValue("Select count(*) from d_bookinstance where BuildStatusCode = " + (int)BookBuildStatus.Error);
				if(errorCount != errorBooks.Count)
				{
					throw new Exception("The expected number of books with a BuildStatus of Error was not found.");
				}
		
				IBookCollection buildRequestedBooks = new BookCollection(BookBuildStatus.BuildRequested);
				int buildRequestedCount = (int)GetDBScalarValue("Select count(*) from d_bookinstance where BuildStatusCode = " + (int)BookBuildStatus.BuildRequested);
				if(buildRequestedCount != buildRequestedBooks.Count)
				{
					throw new Exception("The expected number of books with a BuildStatus of BuildRequested was not found.");
				}

				IBookCollection buildBooks = new BookCollection(BookBuildStatus.Built);
				int builtCount = (int)GetDBScalarValue("Select count(*) from d_bookinstance where BuildStatusCode = " + (int)BookBuildStatus.Built);
				if(builtCount != buildBooks.Count)
				{
					throw new Exception("The expected number of books with a BuildStatus of Built was not found.");
				}

				IBookCollection notBuiltBooks = new BookCollection(BookBuildStatus.NotBuilt);
				int notBuiltCount = (int)GetDBScalarValue("Select count(*) from d_bookinstance where BuildStatusCode = " + (int)BookBuildStatus.NotBuilt);
				if(notBuiltCount != notBuiltBooks.Count)
				{
					throw new Exception("The expected number of books with a BuildStatus of NotBuilt was not found.");
				}
			}
			finally
			{
				DeleteNamedBook(bookName);
			}		
		}

		/// <summary>
		/// Build the book with new content type
		/// </summary>
		[Test]
		public void BookGeneral_BuildBookWithNewContentType()
		{
			string INVALID_CONTENTTYPE = "text/bogus";
			string bookName = Guid.NewGuid().ToString();
			string makefile = CreateTestMakefileNewContentType(INVALID_CONTENTTYPE);

			try
			{
				IBook book = new Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, makefile);
				book.Build();
	
				//make sure the book build failed
				if(book.BuildStatus != BookBuildStatus.Error)
				{
					throw new Exception("The system did not throw the anticipated exception whe nbuilding a book with an unsupported content type.");
				}
			}
			finally
			{
				DeleteNamedBook(bookName);
			}		
		}
		[Test]
		public void BookGeneral_SaveBookModifySaveBook()
		{
			string bookName = Guid.NewGuid().ToString();
			try
			{
				string bookTitle = Guid.NewGuid().ToString();
				string bookDesc = Guid.NewGuid().ToString();
				string bookCopy = Guid.NewGuid().ToString();
				string bookMakefile = CreateTestMakefile();

				IBook book = new Book(bookName, bookTitle, bookDesc, bookCopy, BookSourceType.Makefile, bookMakefile);
				book.Save();
				
				CheckDBRecordExists("D_BookInstance BI inner join D_Book B ON B.BookId = BI.BookId ", "B.Name='" + bookName + "' AND BI.Title='" + bookTitle + "' AND Description='" + bookDesc + "' AND Copyright='" + bookCopy + "' AND SourceUri='" + bookMakefile + "'");

				bookTitle = Guid.NewGuid().ToString();
				bookDesc = Guid.NewGuid().ToString();
				bookCopy = Guid.NewGuid().ToString();
				bookMakefile = CreateTestMakefile();

				book.Title = bookTitle;
				book.Description = bookDesc;
				book.Copyright = bookCopy;
				book.SourceUri = bookMakefile;
			
				book.Save();

				CheckDBRecordExists("D_BookInstance BI inner join D_Book B ON B.BookId = BI.BookId ", "Name='" + bookName + "' AND Title='" + bookTitle + "' AND Description='" + bookDesc + "' AND Copyright='" + bookCopy + "' AND SourceUri='" + bookMakefile + "'");
			}
			finally
			{
				DeleteNamedBook(bookName);
			}
		}

	}

	[TestFixture]
	public class CreateBook : BookBpcTest
	{
		/// <summary>
		/// Creates a new book using the "all parameters" constructor and verifies that all book properties are persisted and can be retrieved properly.
		/// </summary>
		[Test]
		public void CreateBook_AllParamConstructorTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string bookTitle = System.Guid.NewGuid().ToString();
			string bookDesc = System.Guid.NewGuid().ToString();
			string bookCopy = System.Guid.NewGuid().ToString();
			string bookSourceType = BookSourceType.Makefile.ToString();
			string bookSourceUri = System.Guid.NewGuid().ToString();

			try
			{
				IBook book = new Book(bookName, bookTitle, bookDesc, bookCopy, BookSourceType.Makefile, bookSourceUri);
				book.Save();

				CheckDBRecordExists("D_BookInstance BI inner join D_Book B ON B.BookId = BI.BookId ", "Name='" + bookName + "'");
				CheckDBRecordExists("D_BookInstance", "Title='" + bookTitle + "'");
				CheckDBRecordExists("D_BookInstance", "Description='" + bookDesc + "'");
				CheckDBRecordExists("D_BookInstance", "Copyright='" + bookCopy + "'");
				CheckDBRecordExists("D_BookInstance", "SourceTypeId=" + (int)BookSourceType.Makefile);
				CheckDBRecordExists("D_BookInstance", "SourceUri='" + bookSourceUri + "'");

				int recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_BookInstance WHERE bookId = (select bookid from d_book where name ='" + bookName + "')");

				if(recCount != 1)
				{
					throw new Exception(recCount + " records persisted but 1 record was expected.");
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
			}		
		}

		/// <summary>
		/// Creates a new book using the "no parameters" constructor.
		/// </summary>
		[Test]
		public void CreateBook_NoParamConstructorTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string bookTitle = System.Guid.NewGuid().ToString();
			string bookDesc = System.Guid.NewGuid().ToString();
			string bookCopy = System.Guid.NewGuid().ToString();
			string bookSourceUri = System.Guid.NewGuid().ToString();
			BookSourceType bookSourceType = BookSourceType.Makefile;

			try
			{
				IBook book = new Book();
				book.Name = bookName;
				book.Title = bookTitle;
				book.Description = bookDesc;
				book.Copyright = bookCopy;
				book.SourceType = bookSourceType;
				book.SourceUri = bookSourceUri;
				book.Save();

				CheckDBRecordExists("D_BookInstance BI inner join D_Book B ON B.BookId = BI.BookId ", "Name='" + bookName + "'");
				CheckDBRecordExists("D_BookInstance", "Title='" + bookTitle + "'");
				CheckDBRecordExists("D_BookInstance", "Description='" + bookDesc + "'");
				CheckDBRecordExists("D_BookInstance", "Copyright='" + bookCopy + "'");
				CheckDBRecordExists("D_BookInstance", "SourceTypeId='" + (int)BookSourceType.Makefile + "'");
				CheckDBRecordExists("D_BookInstance", "SourceUri='" + bookSourceUri + "'");

				int recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_BookInstance WHERE bookId = (select bookid from d_book where Name = '" + bookName + "')");

				if(recCount != 1)
				{
					throw new Exception(recCount + " records persisted but 1 record was expected.");
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);

			}		
		}

		/// <summary>
		/// Creates a new book, saves it, and then retrieves it again
		/// </summary>
		[Test]
		public void CreateBook_RetrieveBookById()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string bookTitle = System.Guid.NewGuid().ToString();
			string bookDesc = System.Guid.NewGuid().ToString();
			string bookCopy = System.Guid.NewGuid().ToString();
			string bookSourceType = BookSourceType.Makefile.ToString();
			string bookSourceUri = System.Guid.NewGuid().ToString();

			try
			{
				IBook book = new Book(bookName, bookTitle, bookDesc, bookCopy, BookSourceType.Makefile, bookSourceUri);
				book.Save();

                IBook bookPullback = new Book(book.Id);
				Console.WriteLine("Id:" + bookPullback.Id + ":" + book.Id);
				Console.WriteLine("Copyright:" + bookPullback.Copyright + ":" + book.Copyright);
				Console.WriteLine("Description:" + bookPullback.Description + ":" + book.Description);
				Console.WriteLine("SourceType:" + bookPullback.SourceType.ToString() + ":" + book.SourceType.ToString());
				Console.WriteLine("SourceUri:" + bookPullback.SourceUri + ":" + book.SourceUri);
				Console.WriteLine("Title:" + bookPullback.Title + ":" + book.Title);
				Console.WriteLine("Version:" + bookPullback.Version + ":" + book.Version);
				if(bookPullback.Id != book.Id || bookPullback.Copyright != book.Copyright || bookPullback.Description != book.Description || bookPullback.Name != book.Name || bookPullback.SourceType != book.SourceType || bookPullback.SourceUri != book.SourceUri || bookPullback.Title != book.Title || bookPullback.Version != book.Version)
				{
					throw new Exception("The retrieved book does not have the expected property values.");
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
			}		
		}

		/// <summary>
		/// Creates a new book from a makefile and verifies that all expected datatables were populated
		/// </summary>
		[Test]
		public void CreateBookFromMakefile_DbRecordCountTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string docAnchorName = System.Guid.NewGuid().ToString();
			string docFormatUri = Path.GetTempFileName();
			string tempMakefileFilename = CreateTestMakefile();
			try
			{
				Book book = new Book(bookName, "", "", "", BookSourceType.Makefile, tempMakefileFilename);
				book.Build();

				//test for tracer book, document, namedanchor, and document format values
				CheckDBRecordExists("D_BookInstance BI inner join D_Book B ON B.BookId = BI.BookId ", "Name='" + bookName + "'");
				CheckDBRecordExists("D_DocumentInstance DI inner join D_Document D ON DI.DocumentId = D.DocumentId", "Name='jjs_document'");
				CheckDBRecordExists("D_NamedAnchor", "Name='jjs_documentanchor'");

				//set up some counters and objects that we will reuse
				int recCount = 0;
				int xmlNodeCount = 0;
				XmlNodeList nodes = null;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(tempMakefileFilename);

				//make sure the total number of booktoc rows were created
				recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_BookToc DBT, D_BookInstance DB WHERE DBT.BookInstanceId = DB.BookInstanceId AND DB.BookId = (select bookid from d_book where name = '" + bookName + "')");
				nodes = xmlDoc.SelectNodes("//*[local-name()='Book' or local-name()='BookFolder' or local-name()='Document' or local-name()='DocumentAnchor']");
				xmlNodeCount = nodes.Count;
				if(recCount != xmlNodeCount)
				{
					throw new Exception(recCount + " book records persisted (expected " + xmlNodeCount + " book records)");
				}

				//make sure the total number of document rows were created
				recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_DocumentInstance DD, D_BookInstance DB WHERE DD.BookInstanceId = DB.BookInstanceId AND DB.BookId = (select bookid from d_book where name = '" + bookName + "')");
				nodes = xmlDoc.SelectNodes("//*[local-name()='Document']");
				xmlNodeCount = nodes.Count;
				if(recCount != xmlNodeCount)
				{
					throw new Exception(recCount + " document records persisted (expected " + xmlNodeCount + " document records)");
				}

				//make sure the total number of documentanchor rows were created
				recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_NamedAnchor DNA, D_DocumentInstance DD, D_BookInstance DB WHERE DNA.DocumentInstanceId = DD.DocumentInstanceId AND DD.BookInstanceId = DB.BookInstanceId AND DB.BookId = (Select bookid from d_book where name ='" + bookName + "')");
				nodes = xmlDoc.SelectNodes("//*[local-name()='DocumentAnchor']");
				xmlNodeCount = nodes.Count;
				if(recCount != xmlNodeCount)
				{
					throw new Exception(recCount + " document anchor records persisted (expected " + xmlNodeCount + " document anchor records)");
				}

				//make sure the total number of documentformat rows were created
				recCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_DocumentFormat DF, D_DocumentInstance DD, D_BookInstance DB WHERE DF.DocumentInstanceId = DD.DocumentInstanceId AND DD.BookInstanceId = DB.BookInstanceId AND DB.BookId = (select bookid from d_book where name='" + bookName + "')");
				nodes = xmlDoc.SelectNodes("//*[local-name()='DocumentFormat']");
				xmlNodeCount = nodes.Count;
				if(recCount != xmlNodeCount)
				{
					throw new Exception(recCount + " document format records persisted (expected " + xmlNodeCount + " document format records)");
				}

			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
				
				//delete the temp file
				File.Delete(tempMakefileFilename);
			}
		}

		[Test]
		public void CreateBookFromMakefile_DocumentIndexerTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string docName = System.Guid.NewGuid().ToString();
			string docAnchorName = System.Guid.NewGuid().ToString();
			string docFormatUri = Path.GetTempFileName();
			string tempMakefileFilename = CreateTestMakefile();
			
			try
			{
				//create a temporary makefile and build it into a book		
				Book book = new Book(bookName, "", "", "", BookSourceType.Makefile, tempMakefileFilename);
				book.Build();

				//create an xml document from the makefile to use for comparing against persisted book info
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(tempMakefileFilename);

				//select all document nodes from the makefile
				XmlNodeList nodes = xmlDoc.SelectNodes("//*[local-name()='" + Book.XML_ELE_DOCUMENT + "']");

				//iterate thru each document
				IDocumentCollection docs = book.Documents;
				int docCount = 0;
				foreach(IDocument doc in docs)
				{
					string xmlDocName = nodes[docCount].Attributes[Book.XML_ATT_DOCUMENTNAME].Value;
					string bookDocName = doc.Name;
					string xmlDocTitle = nodes[docCount].Attributes[Book.XML_ATT_DOCUMENTTITLE].Value;
					string bookDocTitle = doc.Title;
					string bookDocName_IndexerOrd = docs[docCount].Name;
					string bookDocName_IndexerStr = docs[bookDocName_IndexerOrd].Name;
          
					//did the document name get recorded?
					if(xmlDocName != bookDocName)
					{
						throw new Exception("The document name " + xmlDocName + " in the makefile XML did not match the document name " + bookDocName + " in the database.");
					}

					//did the document name get recorded?
					if(xmlDocTitle != bookDocTitle)
					{
						throw new Exception("The document title " + xmlDocTitle + " in the makefile XML did not match the document title " + bookDocTitle + " in the database.");
					}

					//does our indexer's ordinal lookup function the same as our indexer's name lookup?
					if(bookDocName_IndexerOrd == "" || bookDocName_IndexerStr == "" || bookDocName_IndexerOrd != bookDocName_IndexerStr)
					{
						throw new Exception("The document name " + bookDocName_IndexerOrd + " (ordinal indexer) did not match the document name " + bookDocName_IndexerStr + " (string indexer).");
					}

					//increment our document counter
					docCount++;
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
				
				//delete the temp file
				File.Delete(tempMakefileFilename);
			}
		}


		[Test]
		public void CreateBookFromMakefile_DocumentAnchorIndexerTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string docName = System.Guid.NewGuid().ToString();
			string docAnchorName = System.Guid.NewGuid().ToString();
			string docFormatUri = Path.GetTempFileName();
			string tempMakefileFilename = CreateTestMakefile();
			
			try
			{
				//create a temporary makefile and build it into a book
				Book book = new Book(bookName, "", "", "", BookSourceType.Makefile, tempMakefileFilename);
				book.Build();

				//create an xml document from the makefile to use for comparing against persisted book info
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(tempMakefileFilename);

				//select all document nodes from the makefile
				XmlNodeList docNodes = xmlDoc.SelectNodes("//*[local-name()='" + DestroyerBpc.XML_ELE_DOCUMENT + "']");

				//iterate thru each document
				IDocumentCollection docs = book.Documents;
				int docCount = 0;
				foreach(IDocument doc in docs)
				{
					string xmlDocName = docNodes[docCount].Attributes[DestroyerBpc.XML_ATT_DOCUMENTNAME].Value;
					string bookDocName = doc.Name;
					string xmlDocTitle = docNodes[docCount].Attributes[DestroyerBpc.XML_ATT_DOCUMENTTITLE].Value;
					string bookDocTitle = doc.Title;
                    
					//select all anchor nodes from the current document
					XmlNodeList anchorNodes = docNodes[docCount].SelectNodes(".//*[local-name()='" + DestroyerBpc.XML_ELE_DOCUMENTANCHOR + "']");
					
					//iterate thru each document
					IDocumentAnchorCollection anchors = doc.Anchors;
					int anchorCount = 0;
					foreach(IDocumentAnchor anchor in anchors)
					{
						string xmlAnchorName = anchorNodes[anchorCount].Attributes[DestroyerBpc.XML_ATT_DOCUMENTANCHORNAME].Value;
						string bookAnchorName = anchor.Name;
						string xmlAnchorTitle = anchorNodes[anchorCount].Attributes[DestroyerBpc.XML_ATT_DOCUMENTANCHORTITLE].Value;
						string bookAnchorTitle = anchor.Title;
						string bookAnchorName_IndexerOrd = anchors[anchorCount].Name;
						string bookAnchorName_IndexerStr = anchors[bookAnchorName_IndexerOrd].Name;

						//did the anchor name get recorded?
						if(xmlAnchorName != bookAnchorName)
						{
							throw new Exception("The anchor name " + xmlAnchorName + " in the makefile XML did not match the anchor name " + bookAnchorName + " in the database.");
						}

						//did the anchor title get recorded?
						if(xmlAnchorTitle != bookAnchorTitle)
						{
							throw new Exception("The anchor title " + xmlDocTitle + " in the makefile XML did not match the anchor title " + bookAnchorTitle + " in the database.");
						}

						//does our indexer's ordinal lookup function the same as our indexer's name lookup?
						if(bookAnchorName_IndexerOrd == "" || bookAnchorName_IndexerStr == "" || bookAnchorName_IndexerOrd != bookAnchorName_IndexerStr)
						{
							throw new Exception("The anchor name " + bookAnchorName_IndexerOrd + " (ordinal indexer) did not match the anchor name " + bookAnchorName_IndexerStr + " (string indexer).");
						}

						//increment anchor count
						anchorCount++;
					}

					//increment doc count
					docCount++;
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
				
				//delete the temp file
				File.Delete(tempMakefileFilename);
			}
		}


		[Test]
		public void CreateBookFromMakefile_DocumentFormatIndexerTest()
		{
			string bookName = System.Guid.NewGuid().ToString();
			string docName = System.Guid.NewGuid().ToString();
			string docAnchorName = System.Guid.NewGuid().ToString();
			string docFormatUri = Path.GetTempFileName();
			string tempMakefileFilename = CreateTestMakefile();
			
			try
			{
				//create a temporary makefile and build it into a book
				Book book = new Book(bookName, "", "", "", BookSourceType.Makefile, tempMakefileFilename);
				book.Build();

				//create an xml document from the makefile to use for comparing against persisted book info
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(tempMakefileFilename);

				//select all document nodes from the makefile
				XmlNodeList docNodes = xmlDoc.SelectNodes("//*[local-name()='" + Book.XML_ELE_DOCUMENT + "']");

				//iterate thru each document
				IDocumentCollection docs = book.Documents;
				int docCount = 0;
				foreach(IDocument doc in docs)
				{
					string xmlDocName = docNodes[docCount].Attributes[Book.XML_ATT_DOCUMENTNAME].Value;
					string bookDocName = doc.Name;
					string xmlDocTitle = docNodes[docCount].Attributes[Book.XML_ATT_DOCUMENTTITLE].Value;
					string bookDocTitle = doc.Title;
                    
					//select all format nodes from the current document
					XmlNodeList formatNodes = docNodes[docCount].SelectNodes(".//*[local-name()='" + Book.XML_ELE_DOCUMENTFORMAT + "']");
					int formatNodesCount = formatNodes.Count;

					if(formatNodesCount != doc.Formats.Count)
					{
						throw new Exception("The expected number of formats was not returned.");
					}
					
					//iterate thru each document
					foreach(IDocumentFormat format in doc.Formats)
					{
						string bookFormatUri = format.Uri;
						string bookFormatContentType = format.Description;

						//did the format uri get recorded?
						if(bookFormatUri == "" && bookFormatUri == null)
						{
							throw new Exception("The format uri for " + doc.Name + " was not found.");
						}

						//did the format content-type get recorded?
						if(bookFormatContentType == "" && bookFormatContentType == null)
						{
							throw new Exception("The format content-type for document " + doc.Name + " was not found.");
						}

					}

					//increment doc count
					docCount++;
				}
			}
			finally
			{
				//delete the test data
				DeleteNamedBook(bookName);
				
				//delete the temp file
				File.Delete(tempMakefileFilename);
			}
		}

	}
}
