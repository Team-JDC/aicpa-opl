using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;

namespace AICPA.Destroyer.Content.Document
{
	public class DocumentBpcTest : ContentShared
	{
	}

	[TestFixture]
	public class DocumentGeneral : DocumentBpcTest
	{
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void DocumentGeneral_HitHighlightingTest()
		{
			string tempHtml = Path.GetTempFileName();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile(tempHtml);
			string htmlBefore = "<html><head><title>here is some squid test content in the head tag to try to mess things up</title></head><body><h1>What is the nature of squid ink?</h1>\n<p>What is the <!-- here is a squid comment to try to mess things up -->chemical nature of squid ink? I have been searching for the answer to this question on and off for one year. A student of mine posed it during a class discussion of squid avoidance behavior. I have searched books, the Net libraries. thanks.</p>\n<p>Cephalopod ink is a mixture of melanin, the pigment that gives the ink it's color, and mucus. The mucus gives the ink it's shape - i.e. dense ink blobs that act as decoys have more mucous while larger smoke screens have less. Cephs probably also control the shape, size and placement of the ink with their funnel. There is anecdotal evidence that there is something in the ink that will <script>\nvar test='here is some test squid content in a script to try to mess things up';\n</script>knock out fishes sense of smell.</p><p>squidlet squidlet <u>squidlet the nature</u> of <u><b>squid</b></u> ink</p>\n<p>the nature of squid ink</p>\n<h1>the nature of squid ink</h1>\n<h1>the nature of squid ink</h1>\n<h1>the nature of squid ink</h1>\n<h1>the nature of               squid ink</h1>\n<h1>the nature<br>\n\n\nof\n\n\nsquid\n\n\n           \tink.  These should not match because of case sensitivity: SQUID Squid sQuid</h1></body></html>";
			const string BEGIN_HILITE_TAG = "<SPAN style='font-weight:bold; background-color:pink;'>"; 
			const string END_HILITE_TAG = "</SPAN>";
			string docString = "";
			string htmlAfter = "";
			int expectedHitCount = 19;
			try
			{
				StreamWriter sw = new StreamWriter(tempHtml, false, new System.Text.UTF8Encoding(false));
				sw.Write(htmlBefore);
				sw.Close();

				IBook book = new Book.Book(bookName, "", "", "", BookSourceType.Makefile, tempMakefile);
				book.Build();
				book.Save();

				string[] terms = new string[] {"Net libraries thanks Cephalopod ink", "the nature of squid ink", "squid"};
				docString = DestroyerBpc.ByteArrayToStr(book.Documents[0].PrimaryFormat.GetHighlightedContent(terms, BEGIN_HILITE_TAG, END_HILITE_TAG));

				//make sure that the expected number of hilites came back
				MatchCollection matches = Regex.Matches(docString, BEGIN_HILITE_TAG);
				int markupCount = matches.Count;
				if(markupCount != expectedHitCount)
				{
					throw new Exception("The expected number of hit highlighting tags were not found. Expected " + expectedHitCount + ", found " + markupCount);
				}
				
				//remove the hilite tags and make sure we are exactly the same as when we started
				htmlAfter = docString;
				htmlAfter = htmlAfter.Replace(BEGIN_HILITE_TAG, "");
				htmlAfter = htmlAfter.Replace(END_HILITE_TAG, "");
				if(htmlAfter != htmlBefore)
				{
					throw new Exception("Hit highlighting before and after did not match up.");
				}
			
			}
			finally
			{
				Console.WriteLine("*** Hit Highlighting Test***\n\nBEFORE:\n" + htmlBefore + "\n\nAFTER:\n" + htmlAfter + "\n\nWITH HIGHLIGHTING:\n" + docString);
				DeleteNamedBook(bookName);
				File.Delete(tempMakefile);
			}

		}
		/// <summary>
		/// Make sure we can directly instantiate a document by id; test both instantiation by document id alone, as well as instantiation by book id and document id
		/// </summary>
		[Test]
		public void DocumentGeneral_GetDocument()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site.Site(siteName, "title", "desc", "searchUri");
				IBook book = new Book.Book(bookName, bookName, bookName, bookName, BookSourceType.Makefile, tempMakefile);
				book.Build();
				book.Save();
				site.Save();
				site.AddBook(book);

				IDocument doc1 = new Document(site.Books[0].Documents[0].Id);

				if(site.Books[0].Documents[0].Id != doc1.Id || site.Books[0].Documents[0].Name != doc1.Name || site.Books[0].Documents[0].Title != doc1.Title)
				{
					throw new Exception("The properties of the document retrieved directly do not match those of the document retrieved indirectly.");
				}

				IDocument doc2 = new Document(site.Books[0], site.Books[0].Documents[0].Id);

				if(site.Books[0].Documents[0].Id != doc2.Id || site.Books[0].Documents[0].Name != doc2.Name || site.Books[0].Documents[0].Title != doc2.Title)
				{
					throw new Exception("The properties of the document retrieved directly do not match those of the document retrieved indirectly.");
				}

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookName);
				File.Delete(tempMakefile);
			}
		}
		/// <summary>
		/// Make sure we can get at our named anchors
		/// </summary>
		[Test]
		public void DocumentGeneral_GetDocumentAnchor()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site.Site(siteName, "title", "name", "searchUri");
				IBook book = new Book.Book(bookName, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				book.Build();
				book.Save();
				site.Save();
				site.AddBook(book);

				IDocument doc = site.Books[0].Documents["aag-air_app_c"];
				if(doc.Anchors.Count != 18)
				{
					throw new Exception("Expected number of document anchors was not returned.");
				}

				IDocumentAnchor docAnchor1 = doc.Anchors[0];
				if(docAnchor1.Name != "summary-1" || docAnchor1.Title != "Summary")
				{
					throw new Exception("The properties of the retrieved document anchor did not match input.");
				}

				IDocumentAnchor docAnchor2 = new DocumentAnchor(docAnchor1.Id, doc);
				if(docAnchor2.Name != "summary-1" || docAnchor2.Title != "Summary")
				{
					throw new Exception("The properties of the retrieved document anchor did not match input.");
				}



			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookName);
				File.Delete(tempMakefile);
			}
		}
	}
}
