using System;
using System.Threading;
using System.IO;
using System.Xml;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.Content.Subscription;

namespace AICPA.Destroyer.Content.Site
{
	public class SiteBpcTest : ContentShared
	{
	}

	[TestFixture]
	public class SiteGeneral : SiteBpcTest
	{
		[Test]
		public void SiteGeneral_EmergencyBuild()
		{
            ISite site = new Site(1);
			site.Build();
		}

		[Test]
		public void SiteGeneral_WorkflowTest()
		{
			string tempBookMakefile = CreateTestMakefile();
			IBook book = new Book.Book("workflowbook", "workflowbook", "workflowbook", "workflowbook", BookSourceType.Makefile, tempBookMakefile);
			book.Build();
			book.Save();

			ISite site = new Site();
			site.Name = "workflowsite";
			site.Title = "workflowsite";
			site.Description = "workflowsite";
			site.SiteTemplateXml = "<Makefile><Site><Book Id='" + book.Id + "'/></Site></Makefile>";
			site.BuildStatus = SiteBuildStatus.BuildRequested;
			site.IndexBuildStatus = SiteIndexBuildStatus.BuildRequested;
			site.RequestedStatus = SiteStatus.Staging;
			site.Save();

			Console.WriteLine("Initial Settings:");
			Console.WriteLine(string.Format("Site.BuildStatus={0}; Site.IndexBuildStatus={1}; Site.Status={2}; Site.RequestedStatus={3};",site.BuildStatus, site.IndexBuildStatus, site.Status, site.RequestedStatus));

			ISiteCollection siteBuildSites = new SiteCollection(SiteBuildStatus.BuildRequested);
			foreach(ISite siteBuildSite in siteBuildSites)
			{
				siteBuildSite.Build();

				Console.WriteLine("Settings after Site.Build():");
				Console.WriteLine(string.Format("Site.BuildStatus={0}; Site.IndexBuildStatus={1}; Site.Status={2}; Site.RequestedStatus={3};",siteBuildSite.BuildStatus, siteBuildSite.IndexBuildStatus, siteBuildSite.Status, siteBuildSite.RequestedStatus));
			}

			ISiteCollection indexBuildSites = new SiteCollection(SiteIndexBuildStatus.BuildRequested);
			foreach(ISite indexBuildSite in indexBuildSites)
			{
				indexBuildSite.SiteIndex.Build();

				Console.WriteLine("Settings after Site.SiteIndex.Build():");
				Console.WriteLine(string.Format("Site.BuildStatus={0}; Site.IndexBuildStatus={1}; Site.Status={2}; Site.RequestedStatus={3};",indexBuildSite.BuildStatus, indexBuildSite.IndexBuildStatus, indexBuildSite.Status, indexBuildSite.RequestedStatus));
			}

		}

		[Test]
		public void SiteGeneral_SiteXmlTest()
		{
			string siteName = Guid.NewGuid().ToString();
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string book3Name = Guid.NewGuid().ToString();
			string book4Name = Guid.NewGuid().ToString();
			string tempBookMakefile = CreateTestMakefile();
			string tempSiteMakefile1 = null;
			string tempSiteMakefile2 = null;
			try
			{
				IBook book1 = new Book.Book(book1Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				IBook book2 = new Book.Book(book2Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				IBook book3 = new Book.Book(book3Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				IBook book4 = new Book.Book(book4Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);

				book1.Build();
				book2.Build();
				book3.Build();
				book4.Build();

				book1.Save();
				book2.Save();
				book3.Save();
				book4.Save();

				//create the first site template file
				tempSiteMakefile1 = CreateTestSiteMakefile(book1.Id, book2.Id, book3.Id);
				StreamReader sr1 = new StreamReader(tempSiteMakefile1, System.Text.Encoding.UTF8);
				string makeFileXml1 = sr1.ReadToEnd();
				sr1.Close();

				//create the second site template file
				tempSiteMakefile2 = CreateTestSiteMakefile(book1.Id, book2.Id, book4.Id);
				StreamReader sr2 = new StreamReader(tempSiteMakefile2, System.Text.Encoding.UTF8);
				string makeFileXml2 = sr2.ReadToEnd();
				sr2.Close();

                //create the site
				ISite site = new Site(siteName, "title", "desc", "searchUri");
				site.Save();
				site.Books.Add(book1);
				site.Books.Add(book2);
				site.Books.Add(book3);
				site.Books.Add(book4);

				 //set the site template using template 1
				site.SiteTemplateXml = makeFileXml1;
				site.Save();

				//okay, grab the site xml
				string siteXml1 = site.SiteXml;

				 //set the site template using template 2
				site.SiteTemplateXml = makeFileXml2;
				site.Save();

				//okay, grab the site xml
				string siteXml2 = site.SiteXml;


				//write out the two strings to the console
				Console.WriteLine(siteXml1);
				Console.WriteLine(siteXml2);

				//make sure there are differences in the two site xml strings
				if(siteXml1 == siteXml2)
				{
					throw new Exception("Expected differences in siteXml1 and siteXml2.");
				}

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
				DeleteNamedBook(book3Name);
				File.Delete(tempBookMakefile);
				File.Delete(tempSiteMakefile1);
				File.Delete(tempSiteMakefile2);
			}
		}
		
		[Test]
		public void SiteGeneral_BuildSite()
		{
			string siteName = Guid.NewGuid().ToString();
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			string book3Name = Guid.NewGuid().ToString();
			string tempBookMakefile = CreateTestMakefile();
			string tempSiteMakefile = null;
			try
			{
				IBook book1 = new Book.Book(book1Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				IBook book2 = new Book.Book(book2Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);
				IBook book3 = new Book.Book(book3Name, "title", "desc", "copy", BookSourceType.Makefile, tempBookMakefile);

				book1.Build();
				book2.Build();
				book3.Build();

				book1.Save();
				book2.Save();
				book3.Save();

				tempSiteMakefile = CreateTestSiteMakefile(book1.Id, book2.Id, book3.Id);
				StreamReader sr = new StreamReader(tempSiteMakefile, System.Text.Encoding.UTF8);
				string makeFileXml = sr.ReadToEnd();
				sr.Close();

				ISite site = new Site(siteName, "title", "desc", "searchUri");
				site.Save();
				site.Books.Add(book1);
				site.Books.Add(book2);
				site.Books.Add(book3);
				site.SiteTemplateXml = makeFileXml;
				site.Save();
				site.Build();

				string siteXml = site.SiteXml;

				Console.WriteLine("SiteXml: " + siteXml);

				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(siteXml);

				//check for the expected number of site folders
				if(xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_SITEFOLDER).Count != 2)
				{
					throw new Exception("Expected number of site folders not found");
				}

				//check for the specific site folders
				if(xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_SITEFOLDER + "[@" + DestroyerBpc.XML_ATT_SITEFOLDERNAME+ "='site_folder_1' or @" + DestroyerBpc.XML_ATT_SITEFOLDERNAME + "='site_folder_2']").Count != 2)
				{
					throw new Exception("Expected number of specific site folders not found");
				}

				//check for the expected number of books
				if(xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK).Count != 3)
				{
					throw new Exception("Expected number of books not found");
				}

				//check for the specific books
				if(xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKID+ "='" + book1.Id + "' or @" + DestroyerBpc.XML_ATT_BOOKID+ "='" + book2.Id + "' or @" + DestroyerBpc.XML_ATT_BOOKID+ "='" + book3.Id +"']").Count != 3)
				{
					throw new Exception("Expected number of specific books not found");
				}

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
				DeleteNamedBook(book3Name);
				File.Delete(tempBookMakefile);
				File.Delete(tempSiteMakefile);
			}
		}
		[Test]
		public void SiteGeneral_PrevNextTests()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName1 = Guid.NewGuid().ToString();
			string bookName2 = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site(siteName, "Title", "Desc", "");
				site.Save();
				IBook book1 = new Book.Book(bookName1, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book1.Build();
				book1.Save();
				IBook book2 = new Book.Book(bookName2, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book2.Build();
				book2.Save();
				site.AddBook(book1);
				site.AddBook(book2);

				if(book1.Id != site.GetPreviousBook(book2).Id || book2.Id != site.GetNextBook(book1).Id)
				{
					throw new Exception("Previous/Next books were not as expected.");
				}
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
		/// Create two sites with two books each and then request the toc XML at a few different nodes. Make sure
		/// all the expected elements and attributes are returned in the XML.
		/// </summary>
		[Test]
		public void SiteGeneral_GetTocXmlTest()
		{
			string siteName1 = Guid.NewGuid().ToString();
			string bookName1a = Guid.NewGuid().ToString();
			string bookName1b = Guid.NewGuid().ToString();

			string siteName2 = Guid.NewGuid().ToString();
			string bookName2a = Guid.NewGuid().ToString();
			string bookName2b = Guid.NewGuid().ToString();
			
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site1 = new Site(siteName1, "AICPA reSOURCE", "Desc", "uri");
				site1.Save();
				IBook book1a = new Book.Book(bookName1a, "Professional Standards", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book1a.Build();
				book1a.Save();
				site1.AddBook(book1a);
				IBook book1b = new Book.Book(bookName1b, "AICPA Industry Audit Guide: Airlines", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book1b.Build();
				book1b.Save();
				site1.AddBook(book1b);
				site1.Save();

				ISite site2 = new Site(siteName1, "Title", "Desc", "uri");
				site2.Save();
				IBook book2a = new Book.Book(bookName2a, "title", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book2a.Build();
				book2a.Save();
				site2.AddBook(book2a);
				IBook book2b = new Book.Book(bookName2b, "title", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book2b.Build();
				book2b.Save();
				site2.AddBook(book2b);
				site2.Save();

				

				//***** Test site level toc xml *****
				string tocXml = site2.GetTocXml(site2.Id, site2.NodeType);
				XmlDocument siteXmlDoc = new XmlDocument();
				siteXmlDoc.LoadXml(tocXml);

				//make sure our site information was returned
				XmlNodeList siteNodes = siteXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_SITE + "[@" + DestroyerBpc.XML_ATT_SITEID + "='" + site2.Id + "' and @" + DestroyerBpc.XML_ATT_SITENAME + "='" + site2.Name + "' and @" + DestroyerBpc.XML_ATT_SITETITLE + "='" + site2.Title + "']");
				int expectedNodeCount = 1;
				int nodeCount = siteNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected site information was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the expected number of books come back in the XML
				siteNodes = siteXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK);
				expectedNodeCount = 2;
				nodeCount = siteNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected number of books was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the specific first book we passed in come back
				siteNodes = siteXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKID + "='" + book2a.Id + "' and @" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + book2a.Name + "' and @" + DestroyerBpc.XML_ATT_BOOKTITLE + "='"+ book2a.Title +"']");
				expectedNodeCount = 1;
				nodeCount = siteNodes.Count;
				if(nodeCount != 1)
				{
					throw new Exception("The expected number of books was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the specific second book we passed in come back
				siteNodes = siteXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKID + "='" + book2b.Id + "' and @" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + book2b.Name + "' and @" + DestroyerBpc.XML_ATT_BOOKTITLE + "='"+ book2b.Title +"']");
				expectedNodeCount = 1;
				nodeCount = siteNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected number of books was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				Console.WriteLine("Toc Xml for Site: " + tocXml);



				//***** Test book level toc xml *****
				string bookXml = site2.GetTocXml(book2a.Id, book2a.NodeType);
				XmlDocument bookXmlDoc = new XmlDocument();
				bookXmlDoc.LoadXml(bookXml);

				//make sure our book information was returned
				XmlNodeList bookNodes = bookXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKID + "='" + book2a.Id+ "' and @" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + book2a.Name + "' and @" + DestroyerBpc.XML_ATT_BOOKTITLE + "='" + book2a.Title + "']");
				expectedNodeCount = 1;
				nodeCount = bookNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected book information was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the expected number of documents came back in the XML
				bookNodes = bookXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENT);

                // 2010-01-12 shb: changed to 12 from 14, in the xml doc, there are 12 "documents" at the root level
                // and then 2 under a book folder.  This method seems to be functioning as describing in its method
                // header, naming returning only the 12 documents that are immeadiate children, and yet
                // the unit test was expecting all 14.  We have decided to change the test case to look for 12.
                expectedNodeCount = 12;

				nodeCount = bookNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected number of documents was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the expected number of book folders came back in the XML
				bookNodes = bookXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOKFOLDER);
				expectedNodeCount = 1;
				nodeCount = bookNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected number of book folders was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}
				
				Console.WriteLine("Toc Xml for Book: " + bookXml);



				//***** Test document level toc xml *****
				IDocument doc = book2a.Documents["aag-air_app_c"];
				string docXml = site2.GetTocXml(doc.Id, doc.NodeType);
				XmlDocument docXmlDoc = new XmlDocument();
				docXmlDoc.LoadXml(docXml);

				//make sure our document information was returned
				XmlNodeList docNodes = docXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENT + "[@" + DestroyerBpc.XML_ATT_DOCUMENTID + "='" + doc.Id+ "' and @" + DestroyerBpc.XML_ATT_DOCUMENTNAME + "='" + doc.Name + "' and @" + DestroyerBpc.XML_ATT_DOCUMENTTITLE + "='" + doc.Title + "']");
				expectedNodeCount = 1;
				nodeCount = docNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected document information was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				//make sure the expected number of named anchors came back in the XML
				docNodes = docXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENTANCHOR);
				expectedNodeCount = 5;
				nodeCount = docNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected number of named anchors was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				Console.WriteLine("Toc Xml for Document: " + docXml);

				

				//***** Test anchor level toc xml *****
				IDocumentAnchor anchor = doc.Anchors["jjs_documentanchor"];
				string anchorXml = site2.GetTocXml(anchor.Id, anchor.NodeType);
				XmlDocument anchorXmlDoc = new XmlDocument();
				anchorXmlDoc.LoadXml(anchorXml);

				//make sure our anchor information was returned
				XmlNodeList anchorNodes = anchorXmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENTANCHOR + "[@" + DestroyerBpc.XML_ATT_DOCUMENTANCHORID + "='" + anchor.Id+ "' and @" + DestroyerBpc.XML_ATT_DOCUMENTANCHORNAME + "='" + anchor.Name + "' and @" + DestroyerBpc.XML_ATT_DOCUMENTANCHORTITLE + "='" + anchor.Title + "']");
				expectedNodeCount = 1;
				nodeCount = anchorNodes.Count;
				if(nodeCount != expectedNodeCount)
				{
					throw new Exception("The expected anchor information was not found in the XML. Expected " + expectedNodeCount + ", found " + nodeCount);
				}

				Console.WriteLine("Toc Xml for Anchor: " + anchorXml);

			}
			finally
			{
				DeleteNamedSite(siteName1);
				DeleteNamedBook(bookName1a);
				DeleteNamedBook(bookName1b);
				DeleteNamedSite(siteName2);
				DeleteNamedBook(bookName2a);
				DeleteNamedBook(bookName2b);
				File.Delete(tempMakefile);
			}		
		}


		/// <summary>
		/// Try indexing a site when one of the site documents cannot have metadata added to it properly.
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
        public void SiteGeneral_IndexSiteWithSourceHtmlContainingNoHeadTag()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempHtmlFile = Path.GetTempFileName();
			string tempMakefile = CreateTestMakefile(tempHtmlFile);
			try
			{
				StreamWriter sw = new StreamWriter(tempHtmlFile, false);
				sw.WriteLine("<HTML></HTML>");
				sw.Close();
				ISite site = new Site(siteName, "Title", "Desc", "uri");
				site.Save();

				IBook book = new Book.Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book.Build();
				book.Save();
				site.AddBook(book);
				site.RequestedStatus = SiteStatus.Staging;
				site.Save();
				try
				{
					site.SiteIndex.Build();
					
					//sleep while the index is updating
					while(site.SiteIndex.Status == SiteIndexStatus.Updating)
					{
						Thread.Sleep((5000));
					}
				}
				catch(Exception e)
				{
					if(e.Message.ToLower().IndexOf("head tag not found for file") == -1)
					{
                        throw new Exception("SiteIndex.Build failed to detect problematic HTML file (no head tag).");
					}
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
		/// Try indexing the site when one of the book documents has been deleted.
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
		public void SiteGeneral_IndexSiteWithMissingSourceDocument()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempHtmlFile = Path.GetTempFileName();
			string tempMakefile = CreateTestMakefile(tempHtmlFile);
			try
			{
				ISite site = new Site(siteName, "Title", "Desc", "uri");
				site.Save();

				IBook book = new Book.Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book.Build();
				book.Save();
				site.AddBook(book);
				site.RequestedStatus = SiteStatus.Staging;
				site.Save();

				//delete the one and only doc
				File.Delete(book.Documents[0].PrimaryFormat.Uri);

				try
				{
					site.SiteIndex.Build();
					
					//sleep while the index is updating
					while(site.SiteIndex.Status == SiteIndexStatus.Updating)
					{
						Thread.Sleep((5000));
					}

				}
				catch(Exception e)
				{
					if(e.Message.ToLower().IndexOf("document format file not found while building site") == -1)
					{
						throw new Exception("SiteIndex.Build failed to detect missing HTML file.");
					}
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
		/// Try indexing a site before saving the site.
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
        public void SiteGeneral_IndexSiteWithoutSavingSite()
		{
			string siteName = Guid.NewGuid().ToString();
			try
			{
				ISite site = new Site(siteName, "Title", "Desc", "uri");
				site.RequestedStatus = SiteStatus.Staging;
				try
				{
					site.SiteIndex.Build();
					
					//sleep while the index is updating
					while(site.SiteIndex.Status == SiteIndexStatus.Updating)
					{
						Thread.Sleep((5000));
					}

				}
				catch(Exception e)
				{
					if(e.Message.ToLower().IndexOf("you must save a site before building its index") == -1)
					{
						throw new Exception("SiteIndex.Build failed to detect unsaved site.");
					}
				}
			}
			finally
			{
			}				
		}
		/// <summary>
		/// Try adding a book to a site before saving the site.
		/// </summary>
		[Test]
		public void SiteGeneral_AddBookBeforeSavingSite()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site(siteName, "title", "desc", "");
				IBook book = new Book.Book(bookName, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				site.AddBook(book);
			}
			catch(Exception e)
			{
				if(e.Message.ToLower().IndexOf("error adding book to site. the site has pending changes and should be saved before adding books to it.") == -1)
				{
					throw new Exception("Book.AddBook() failed to detect unsaved site.");
				}
			}
			finally
			{
				File.Delete(tempMakefile);
			}
		}
		/// <summary>
		/// Try to add a book to a site before saving the book.
		/// </summary>
		[Test]
		public void SiteGeneral_AddBookBeforeSavingBook()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site(siteName, "title", "desc", "");
				site.Save();
				IBook book = new Book.Book(bookName, "title", "desc", "copy", BookSourceType.Makefile, tempMakefile);
				site.AddBook(book);
			}
			catch(Exception e)
			{
				if(e.Message.ToLower().IndexOf("error adding book to site. the book has pending changes and should be saved before adding it to a site.") == -1)
				{
					throw new Exception("Book.AddBook() failed to detect unsaved book.");
				}
			}
			finally
			{
				DeleteNamedSite(siteName);
				File.Delete(tempMakefile);
			}		
		}
		/// <summary>
		/// Save a site, modify it, and then resave it. Make sure all expected values get persisted on both saves.
		/// </summary>
		[Test]
		public void SiteGeneral_SaveSiteModifySaveSite()
		{
			string siteName = Guid.NewGuid().ToString();
			try
			{
				string siteTitle = Guid.NewGuid().ToString();
				string siteDesc = Guid.NewGuid().ToString();
				string siteSearchUri = Guid.NewGuid().ToString();

				ISite site = new Site(siteName, siteTitle, siteDesc, siteSearchUri);
				site.Save();
				
				CheckDBRecordExists("D_Site", "Name='" + siteName + "' AND Title='" + siteTitle + "' AND Description='" + siteDesc + "' AND SearchUri='" + siteSearchUri + "'");

				siteTitle = Guid.NewGuid().ToString();
				siteDesc = Guid.NewGuid().ToString();
				siteSearchUri = Guid.NewGuid().ToString();

				site.Title = siteTitle;
				site.Description = siteDesc;
				site.SearchUri = siteSearchUri;
			
				site.Save();

				CheckDBRecordExists("D_Site", "Name='" + siteName + "' AND Title='" + siteTitle + "' AND Description='" + siteDesc + "' AND SearchUri='" + siteSearchUri + "'");
			}
			finally
			{
				DeleteNamedSite(siteName);
			}
		}
	}

	[TestFixture]
	public class SiteIndex : SiteBpcTest
	{
		/// <summary>
		/// A big BuildIndex test
		/// </summary>
		[Test]
        [Explicit("This test probably doesn't make sense anymore with new Endeca engine.")]
		public void SiteIndex_BuildIndex()
		{
			//create our site
			string siteName = Guid.NewGuid().ToString();
			string siteTitle = "AICPA reSOURCE";
			string siteDesc = "<P>AICPA has created your core accounting and auditing library online. <B>AICPA RESOURCE: Accounting & Auditing Literature</B> is now customizable to suit your preferences or your firm's needs. Or, if you prefer to have access to the entire library – that's available too!</P><P>A subscription to the AICPA Library gives you access to all of the following:</P><UL><LI>AICPA Professional Standards</LI><LI>Technical Practice Aids</LI><LI>AICPA Audit and Accounting Guides and Audit Risk Alerts</LI><LI>Accounting Trends & Techniques</LI></UL>";
			string siteSearchUri = "http://localhost";
			string bookNameOne = Guid.NewGuid().ToString();
			string bookNameTwo = Guid.NewGuid().ToString();
			string bookMakefileOne = CreateTestMakefile();
			string bookMakefileTwo = CreateTestMakefile();

			try
			{
				ISite site = new Site(siteName, siteTitle, siteDesc, siteSearchUri);

				//save the site so we can start adding books to it
				site.Save();

				//create book 1, save it, and add to site
				string bookTitleOne = "Professional Standards";
				string bookDescOne = "<P><I>Professional Standards</I> brings together for continuing reference the currently effective pronouncements on professional standards issued by the American Institute of Certified Public Accountants (AICPA). In addition, this Reporter contains the full text of relevant Public Company Accounting Oversight Board (PCAOB) Rules and Standards and a section detailing the applicability and integration of AICPA Professional Standards and PCAOB Standards.</P>";
				string bookCopyOne = "<P ALIGN=\"center\">Copyright &copy; American Institute of Certified Public Accountants, Inc.<BR>All Rights Reserved</P>";
				BookSourceType bookSourcetypeOne = BookSourceType.Makefile;
			
				IBook bookOne = new Book.Book(bookNameOne, bookTitleOne, bookDescOne, bookCopyOne, bookSourcetypeOne, bookMakefileOne);
				bookOne.Save();
				bookOne.Build();
				site.Books.Add(bookOne);

				//create book 2, save it, and add to site
				string bookTitleTwo = "AICPA Industry Audit Guide: Airlines";
				string bookDescTwo = "<P>This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee's recommendations on and descriptions of financial accounting and reporting principles and practices for airlines. The AICPA Accounting Standards Executive Committee has found this Guide to be consistent with existing standards and principles covered by Rules 202 and 203 of the AICPA Code of Professional Conduct. AICPA members should be prepared to justify departures from the accounting guidance in this Guide.</P>";
				string bookCopyTwo = "<P ALIGN=\"center\">Copyright &copy; American Institute of Certified Public Accountants, Inc.<BR>All Rights Reserved</P>";
				BookSourceType bookSourcetypeTwo = BookSourceType.Makefile;
				
				IBook bookTwo = new Book.Book(bookNameTwo, bookTitleTwo, bookDescTwo, bookCopyTwo, bookSourcetypeTwo, bookMakefileTwo);
				bookTwo.Save();
				bookTwo.Build();
				site.Books.Add(bookTwo);
				site.RequestedStatus = SiteStatus.Staging;

				//save the site
				site.Save();

				//build the site index
				site.SiteIndex.Build();

				//sleep while the index is updating
				while(site.SiteIndex.Status == SiteIndexStatus.Updating)
				{
					Thread.Sleep((5000));
				}

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookNameOne);
				DeleteNamedBook(bookNameTwo);
				File.Delete(bookMakefileOne);
				File.Delete(bookMakefileTwo);
			}
		}
	}


	[TestFixture]
	public class SiteXml : SiteBpcTest
	{
		/// <summary>
		/// Normal request for SiteXml and SiteBookXml
		/// </summary>
		[Test]
		public void SiteXml_BasicTest()
		{
			string siteName = Guid.NewGuid().ToString();
			int siteId = 0;
			try
			{
				//create a new site and save it
				ISite site = new Site(siteName, "b", "c", "d");
				site.Save();
				siteId = site.Id;

				string siteXml = Regex.Replace(site.SiteXml, "Guid=\"[^\"]*\"", "");
				string siteBookXml = Regex.Replace(site.SiteBookXml, "Guid=\"[^\"]*\"", "");;



				if((siteXml == "" || siteBookXml == "") || siteXml != siteBookXml)
				{
					throw new Exception("The site xml or sitebook xml is empty or the site xml and sitebook xml strings do not match.");
				}
			}
			finally
			{
				DeleteNamedSite(siteName);
			}
		}
		/// <summary>
		/// To make sure a request for the SiteBookXml functions properly even if the books associated with the request have not yet been built.
		/// </summary>
		[Test]
		public void SiteXml_GetSiteBookXmlWithNonbuiltBooks()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site(siteName, "Title", "Desc", "uri");
				site.Save();

				IBook book = new Book.Book(bookName, "title", "desc", "cpyright", BookSourceType.Makefile, tempMakefile);
				book.Save();
				site.AddBook(book);
				site.Save();
				string siteBookXml = site.SiteBookXml;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(siteBookXml);
				if(xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_BOOK).Count != 1 && xmlDoc.SelectNodes("//" + DestroyerBpc.XML_ELE_DOCUMENT).Count != 0)
				{
					throw new Exception("Did not get expected number of book or document elements back from SiteBookXml.");
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

	[TestFixture]
	public class GetSite : SiteBpcTest
	{
		/// <summary>
		/// Try retrieving all sites associated with a particular book
		/// </summary>
		[Test]
		public void GetSite_GetBookSites()
		{
			string site1Name = Guid.NewGuid().ToString();
			string site2Name = Guid.NewGuid().ToString();
			string site3Name = Guid.NewGuid().ToString();
			string book1Name = Guid.NewGuid().ToString();
			string book2Name = Guid.NewGuid().ToString();
			try
			{
				//create some books
				IBook book1 = new Book.Book(book1Name, book1Name, book1Name, book1Name, BookSourceType.Makefile, book1Name);
				IBook book2 = new Book.Book(book2Name, book2Name, book2Name, book2Name, BookSourceType.Makefile, book2Name);
				book1.Save();
				book2.Save();

                //create some sites
                ISite site1 = new Site(site1Name, site1Name, site1Name, site1Name);
				ISite site2 = new Site(site2Name, site2Name, site2Name, site2Name);
				ISite site3 = new Site(site3Name, site3Name, site3Name, site3Name);
				site1.Save();
				site2.Save();
				site3.Save();

				//add the first book to two of the sites
                site1.AddBook(book1);
				site2.AddBook(book1);

				//now see if this book retrieves two sites
				ISiteCollection sites1 = new SiteCollection(book1);
				if(sites1.Count != 2)
				{
					throw new Exception("The expected number of sites was not returned.");
				}

				//now make sure that the second book does not retrieve any sites
				ISiteCollection sites2 = new SiteCollection(book2);
				if(sites2.Count != 0)
				{
					throw new Exception("The expected number of sites was not returned.");
				}
			}
			finally
			{
				DeleteNamedSite(site1Name);
				DeleteNamedSite(site2Name);
				DeleteNamedSite(site3Name);
				DeleteNamedBook(book1Name);
				DeleteNamedBook(book2Name);
			}
		}

		/// <summary>
		/// Try retrieving all sites
		/// </summary>
		[Test]
		public void GetSite_GetAllSites()
		{
			string siteName = Guid.NewGuid().ToString();
			try
			{
				ISiteCollection siteColl1 = new SiteCollection(false, true);
				int siteCount1 = siteColl1.Count;

				ISite site = new Site(siteName, "title", "desc", "");
				site.Save();

				ISiteCollection siteColl2 = new SiteCollection(false, true);
				int siteCount2 = siteColl2.Count;

				if(siteCount1 != siteCount2-1)
				{
					throw new Exception("The before and after site count was not as expected.");
				}

				int count = 0;
				foreach(ISite siteIter in siteColl2)
				{
					Console.WriteLine(siteIter.Name);
					count++;
				}

				if(count != siteCount2)
				{
					throw new Exception("Iteration of sites in collection failed.");
				}

				int unassignedSiteCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_Site where SiteStatusCode = " + (int)SiteStatus.Unassigned);
				int stagingSiteCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_Site where SiteStatusCode = " + (int)SiteStatus.Staging);
				int preproductionSiteCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_Site where SiteStatusCode = " + (int)SiteStatus.PreProduction);
				int productionSiteCount = (int)GetDBScalarValue("SELECT COUNT(*) FROM D_Site where SiteStatusCode = " + (int)SiteStatus.Production);

				ISiteCollection sites = new SiteCollection(SiteStatus.Unassigned);
				if(sites.Count != unassignedSiteCount)
				{
					throw new Exception("Expected number of sites was not retrieved.");
				}

				sites = new SiteCollection(SiteStatus.Staging);
				if(sites.Count != stagingSiteCount)
				{
					throw new Exception("Expected number of sites was not retrieved.");
				}

				sites = new SiteCollection(SiteStatus.PreProduction);
				if(sites.Count != preproductionSiteCount)
				{
					throw new Exception("Expected number of sites was not retrieved.");
				}

				sites = new SiteCollection(SiteStatus.Production);
				if(sites.Count != productionSiteCount)
				{
					throw new Exception("Expected number of sites was not retrieved.");
				}

			}
			finally
			{
				DeleteNamedSite(siteName);
			}
		}

		/// <summary>
		/// Try retrieving a site using every available constructor
		/// </summary>
		[Test]
		public void GetSite_AllConstructorTest()
		{
			Guid userId = Guid.NewGuid();
			string myDomain = "test;test1;test2;";
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, myDomain);
			string sessionId = Guid.NewGuid().ToString();
			User.User cscUser = new User.User(userId, ReferringSite.Csc);
			string tempMakefile = CreateTestMakefile();
			string bookName1 = Guid.NewGuid().ToString();

			string siteName = Guid.NewGuid().ToString();
			try
			{
				cscUser.LogOn(sessionId, myDomain);

				//create a completely new site 
				ISite site1 = new Site(siteName, "site title", "site desc", "site search uri");

				IBook book1 = new Book.Book(bookName1, "Title", "Desc", "Copyright", BookSourceType.Makefile, tempMakefile);
				book1.Save();
				book1.Build();

				site1.Save();
				site1.AddBook(book1);
				site1.RequestedStatus = SiteStatus.Staging;
				site1.Save();
				site1.Build();
				site1.SiteIndex.Build();
                
                // sburton 2010-01-14: Currently we do not have a working endeca environment up,
                // so this next line throws an exception.  By commenting it out, we can proceed
                // to test the site constructors which is the real purpose of the test, so that
                // what we'll do.

                //while(site1.SiteIndex.Status == SiteIndexStatus.Updating)
                //{
                //    Thread.Sleep(5000);
                //}				

				//				//retrieve the same site by user and site id
				ISite site2 = new Site(cscUser, site1.Id);
				//				//retrieve the same site by site id
				ISite site3 = new Site(site1.Id);
				//				//retrieve the same site by user, site name, and site status
				ISite site4 = new Site(cscUser, SiteStatus.Staging);
				//				//retrieve the same site by site name and site status
				ISite site5 = new Site(SiteStatus.Staging);
				//				//retrieve the same site by user, site name, and site version
				ISite site6 = new Site(cscUser, site1.Name, site1.Version);
				//				//retrieve the same site by site name and site version
				ISite site7 = new Site(site1.Name, site1.Version);
				//				//retrieve the same site by user and name
				ISite site8 = new Site(cscUser, site1.Name);
				//				//retrieve the same site by name
				ISite site9 = new Site(site1.Name);

				int site1Id = site1.Id;
				int site2Id = site2.Id;
				int site3Id = site3.Id;
				int site4Id = site4.Id;
				int site5Id = site5.Id;
				int site6Id = site6.Id;
				int site7Id = site7.Id;
				int site8Id = site8.Id;
				int site9Id = site9.Id;
				
				if(!(site1Id == site2Id && site2Id == site3Id && site3Id == site4Id && site4Id == site5Id && site5Id == site6Id && site6Id == site7Id && site7Id == site8Id && site8Id == site9Id))
				{
					throw new Exception("Site construction did not produce expected results.");
				}
			}
			finally
			{
				DeleteNamedSite(siteName);
				UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
				UserBpcTest.DeleteDestroyerUsers(userId);

				File.Delete(tempMakefile);
			}

		}
		/// <summary>
		/// Try retrieving a site in the context of a user and make sure the set of accessible books is restricted
		/// </summary>
		[Test]
		public void GetSite_FilteredBooks()
		{

			Guid userId = Guid.NewGuid();
			string myDomain = "test-Subscription1" + DestroyerBpc.DOMAIN_SUBSCRIPTIONCODESEPCHAR + "test-Subscription3";
			C2bUser c2bUser = UserBpcTest.CreateC2bWebServiceUser(userId, myDomain);
			string sessionId = Guid.NewGuid().ToString();
			User.User cscUser = new User.User(userId, ReferringSite.Csc);
			
			string siteName = Guid.NewGuid().ToString();
			string bookName1 = Guid.NewGuid().ToString();
			string bookName2 = Guid.NewGuid().ToString();
			string bookName3 = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();

			string testSubscriptionCode1 = "test-Subscription1";
			string testSubscriptionCode2 = "test-Subscription2";
			string testSubscriptionCode3 = "test-Subscription3";
			string tempSiteMakefile  = string.Empty;
			try
			{
				//create a completely new site 
				ISite site = new Site(siteName, "site title", "site desc", "site search uri");
				site.Save();

				//create some books
				IBook book1 = new Book.Book(bookName1, "title", "desc", "copyright", BookSourceType.Makefile, tempMakefile);
				book1.Save();
				IBook book2 = new Book.Book(bookName2, "title", "desc", "copyright", BookSourceType.Makefile, tempMakefile);
				book2.Save();
				IBook book3 = new Book.Book(bookName3, "title", "desc", "copyright", BookSourceType.Makefile, tempMakefile);
				book3.Save();

				//create the first site template file
				tempSiteMakefile = CreateTestSiteMakefile_Flat(book1.Id, book2.Id, book3.Id);
				StreamReader sr = new StreamReader(tempSiteMakefile, System.Text.Encoding.UTF8);
				string makeFileXml = sr.ReadToEnd();
				sr.Close();

				//add books to site
				site.SiteTemplateXml = makeFileXml;

                // sburton 2010-01-14: Added the requested status, otherwise, leaving it as null caused an error
                // in building the book.  I think this was because of a change to the Site code made late in the
                // game that only set the status if an actual index was built.
                site.RequestedStatus = SiteStatus.Unassigned;
                
                site.Save();
				site.Build();

				//build our subscriptions
				ArrayList bookList1 = new ArrayList();
				bookList1.Add(bookName1);
				ISubscription subscription1 = new Subscription.Subscription(testSubscriptionCode1, (string[])bookList1.ToArray(typeof(string)));
				subscription1.Save();

				ArrayList bookList2 = new ArrayList();
				bookList2.Add(bookName2);
				ISubscription subscription2 = new Subscription.Subscription(testSubscriptionCode2, (string[])bookList2.ToArray(typeof(string)));
				subscription2.Save();

				ArrayList bookList3 = new ArrayList();
				bookList3.Add(bookName3);
				ISubscription subscription3 = new Subscription.Subscription(testSubscriptionCode3, (string[])bookList3.ToArray(typeof(string)));
				subscription3.Save();

				//user login
				cscUser.LogOn(sessionId, myDomain);

				//construct our new site by passing in a user
				ISite filteredSite = new Site(cscUser, site.Id);

				//make sure we only see the books we have access to
				if(cscUser.UserSecurity.BookName.Length != filteredSite.Books.Count)
				{
					throw new Exception("The expected number of books was not returned from a filtered site.");
				}

				//make sure the expected books are in our book collection
				int foundCount = 0;
				foreach(IBook book in filteredSite.Books)
				{
					if(book.Name == book1.Name || book.Name == book3.Name)
					{
						foundCount++;
					}
				}

				if(foundCount != 2)
				{
					throw new Exception("The expected books were not returned from a filtered site.");
				}

				//make sure our site xml is filtered correctly
				string filteredXml = filteredSite.SiteBookXml;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(filteredXml);
				string booksXpath = "//" + DestroyerBpc.XML_ELE_BOOK;
				XmlNodeList bookNodes = xmlDoc.SelectNodes(booksXpath);
				if(bookNodes.Count != 2)
				{
					throw new Exception("The expected number of books was not returned from a filtered site's XML.");
				}

				string specificBooksXpath = "//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + bookName1 + "' or @" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + bookName3 + "']";
				XmlNodeList specificBookNodes = xmlDoc.SelectNodes(specificBooksXpath);
				if(specificBookNodes.Count != 2)
				{
					throw new Exception("The expected books were not returned from a filtered site's XML.");
				}

				//make sure our toc node xml is filtered properly
				string filteredTocXml = filteredSite.GetTocXml(site.Id, site.NodeType);
				xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(filteredTocXml);
				booksXpath = "//" + DestroyerBpc.XML_ELE_BOOK;
				bookNodes = xmlDoc.SelectNodes(booksXpath);
				if(bookNodes.Count != 2)
				{
					throw new Exception("The expected number of books was not returned from a filtered site's XML.");
				}

				specificBooksXpath = "//" + DestroyerBpc.XML_ELE_BOOK + "[@" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + bookName1 + "' or @" + DestroyerBpc.XML_ATT_BOOKNAME + "='" + bookName3 + "']";
				specificBookNodes = xmlDoc.SelectNodes(specificBooksXpath);
				if(specificBookNodes.Count != 2)
				{
					throw new Exception("The expected books were not returned from a filtered site's XML.");
				}
                

			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedSubscription(testSubscriptionCode1);
				DeleteNamedSubscription(testSubscriptionCode2);
				DeleteNamedSubscription(testSubscriptionCode3);
				DeleteNamedBook(bookName1);	
				DeleteNamedBook(bookName2);
				DeleteNamedBook(bookName3);
				UserBpcTest.DeleteC2bWebServiceUsers(c2bUser);
				UserBpcTest.DeleteDestroyerUsers(userId);
				File.Delete(tempMakefile);
				File.Delete(tempSiteMakefile);
			}

		}
		/// <summary>
		/// Try retrieving a site using a bogus Id
		/// </summary>
		[Test]
		public void GetSite_BadSiteTest()
		{
			int siteId = 99999999;
			try
			{
				Exception exception = null;
				ISite site = new Site(siteId);
				try
				{
					Console.WriteLine(site.Id);
				}
				catch(Exception e)
				{
					exception = e;
				}
				if(exception == null)
				{
					throw new Exception("System incorrectly allowed instantiation of site with bogus id '" + siteId + "'.");
				}
			}
			finally
			{
			}
		}
	}
	[TestFixture]
	public class CreateSite : SiteBpcTest
	{
		[Test]
		public void CreateSite_AllParamConstructorTest()
		{
			string siteName = Guid.NewGuid().ToString();
			int siteId = 0;
			try
			{
				Site site = new Site(siteName, "site title", "site desc", "site searchuri");
				site.Save();
				siteId = site.Id;

				CheckDBRecordExists("D_Site", "Name='" + siteName + "'");
				CheckDBRecordExists("D_SiteToc", "NodeTypeId = " + (int)NodeType.Site + " AND NodeId = " + site.Id);
			}
			finally
			{
				DeleteNamedSite(siteName);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CreateSite_AddBooksWithoutSavingSite()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookName = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site = new Site(siteName, siteName, siteName, siteName);

				IBook book = new Book.Book(bookName, bookName, bookName, bookName, BookSourceType.Makefile, tempMakefile);
				book.Build();
				site.AddBook(book);
			}
			catch(Exception e)
			{
				if(e.Message.ToLower().IndexOf("error adding book to site. the site has pending changes and should be saved before adding books to it.") == -1)
				{
					throw new Exception("Site.AddBook failed to detect adding unsaved site.");
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
		/// Create some books, add them to a new site, and then make sure everything can be properly persisted to the database
		/// </summary>
		[Test]
		public void CreateSite_AddBooksNormal()
		{
			string siteName = Guid.NewGuid().ToString();
			string bookOneName = Guid.NewGuid().ToString();
			string bookTwoName = Guid.NewGuid().ToString();
			try
			{
				int siteId = 0;
				int book1Id = 0;
				int book2Id = 0;

				//create a new site
				Site site = new Site(siteName, "site title", "site desc", "site searchuri");
				site.Save();

				//create a couple of new books
				Book.Book book1 = new Book.Book(bookOneName, "book title", "book desc", "book copy", BookSourceType.Makefile, "book source uri");
				book1.Save();
				book1Id = book1.Id;

				Book.Book book2 = new Book.Book(bookTwoName, "book title", "book desc", "book copy", BookSourceType.Makefile, "book source uri");
				book2.Save();
				book2Id = book2.Id;
			
				//add one of the books through the collection
				site.Books.Add(book1);

				//add the other book through the site
				site.AddBook(book2);
				
				//record the number of books prior to the save
				int presaveBookCount = site.Books.Count;

                //save our site and grab its assigned id
				site.Save();
				siteId = site.Id;

				//record the number of books after the save
				int postsaveBookCount = site.Books.Count;

				//make sure book counts are the same
				if(presaveBookCount != postsaveBookCount)
				{
					throw new Exception("Presave book count (" + presaveBookCount + ") was not equal to postsave book count (" + postsaveBookCount + ").");
				}

				//make sure our site and sitebook records exist
				CheckDBRecordExists("D_Site", "Name='" + siteName + "'");
				CheckDBRecordExists("D_SiteToc", "NodeTypeId = " + (int)NodeType.Site + " AND NodeId = " + site.Id);
				CheckDBRecordExists("D_SiteBook", "SiteId = " + siteId + " AND BookInstanceId = " + book1Id);
				CheckDBRecordExists("D_SiteBook", "SiteId = " + siteId + " AND BookInstanceId = " + book2Id);
			}
			finally
			{
				DeleteNamedSite(siteName);
				DeleteNamedBook(bookOneName);
				DeleteNamedBook(bookTwoName);
			}
		}

		[Test]
		public void CreateSite_RemoveBooksNormal()
		{
			string siteName1 = Guid.NewGuid().ToString();
			string siteName2 = Guid.NewGuid().ToString();
			string bookName1 = Guid.NewGuid().ToString();
			string bookName2 = Guid.NewGuid().ToString();
			string tempMakefile = CreateTestMakefile();
			try
			{
				ISite site1 = new Site(siteName1, "title", "desc", "search uri");
				site1.Save();
				IBook book1 = new Book.Book(bookName1, "title", "desc", "copyright", BookSourceType.Makefile, tempMakefile);
				book1.Save();
				IBook book2 = new Book.Book(bookName2, "title", "desc", "copyright", BookSourceType.Makefile, tempMakefile);
				book2.Save();

				site1.AddBook(book1);
				site1.AddBook(book2);

				site1.Save();

				if(site1.Books.Count != 2)
				{
					throw new Exception("The expected number of books was not found.");
				}
				CheckDBRecordExists("D_SiteBook", "SiteId = " + site1.Id + " AND BookInstanceId = " + book1.Id);
				CheckDBRecordExists("D_SiteBook", "SiteId = " + site1.Id + " AND BookInstanceId = " + book2.Id);

				//now try to remove a book
				site1.RemoveBook(book1);
				site1.Save();
				
				if(site1.Books.Count != 1)
				{
					throw new Exception("The expected number of books was not found.");
				}

				//now instantiate a new site using the original site's id to make sure we get back only one book
                ISite site2 = new Site(site1.Id);
				if(site2.Books.Count != 1 || site2.Books[0].Id != book2.Id)
				{
					throw new Exception("The expected number of books was not found.");
				}

			}
			finally
			{
				DeleteNamedSite(siteName1);
				DeleteNamedSite(siteName2);
				DeleteNamedBook(bookName1);
				DeleteNamedBook(bookName2);
			}
		}

	}
}
