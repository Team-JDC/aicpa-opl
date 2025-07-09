using System;
using System.IO;
using System.Xml;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;

namespace AICPA.Destroyer.Content
{
	/// <summary>
	/// Summary description for ContentShared.
	/// </summary>
	public class ContentShared : BaseTest
	{
		/// <summary>
		/// Creates a test makefile using the specified trace parameters.
		/// </summary>
		/// <param name="tempMakefileFilename"></param>
		/// <param name="bookName"></param>
		/// <param name="docName"></param>
		/// <param name="docAnchorName"></param>
		/// <param name="docFormatUri"></param>
		/// <returns>The name of the temporary makefile</returns>
		public string CreateTestMakefile()
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Book Name='aag-air' Title='AICPA Industry Audit Guide' Description='This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee&apos;s recommendations on and descriptions of financial accounting and reporting principles and practices for airlines.' Copyright='Copyright 2005'>";
			makefileContent += "<Document Name='aag-air1' Title='Chapter 1'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air1.html' /><DocumentFormat Content-Type='application/msword' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\illustrations.doc' /></Document>";
			makefileContent += "<Document Name='aag-air2' Title='Chapter 2'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air2.html' /></Document>";
			makefileContent += "<Document Name='aag-air3' Title='Chapter 3'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air3.html' /></Document>";
			makefileContent += "<Document Name='aag-air4' Title='Chapter 4'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air4.html' /></Document>";
			makefileContent += "<Document Name='aag-air5' Title='Chapter 5'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air5.html' /></Document>";
			makefileContent += "<Document Name='aag-air_app_a' Title='Appendix A'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_a.html' /></Document>";
			makefileContent += "<Document Name='aag-air_app_b' Title='Appendix B'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_b.html' /></Document>";
			makefileContent += "<Document Name='aag-air_app_c' Title='Appendix C'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_c.html' />";
			makefileContent += "<DocumentAnchor Name='summary-1' Title='Summary' Hidden='True'/>";
			makefileContent += "<DocumentAnchor Name='foreword' Title='Foreward'/>";
			makefileContent += "<DocumentAnchor Name='introduction_and_background' Title='Introduction and Background'>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.1' Title='1.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.2' Title='2.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.3' Title='3.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.4' Title='4.'/>";
			makefileContent += "</DocumentAnchor>";
			makefileContent += "<DocumentAnchor Name='scope' Title='Scope'>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.5' Title='5.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.6' Title='6.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.7' Title='7.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.8' Title='8.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.9' Title='9.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.10' Title='10.'/>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.11' Title='11.'/>";
			makefileContent += "</DocumentAnchor>";
			makefileContent += "<DocumentAnchor Name='conclusions' Title='Conclusions'>";
			makefileContent += "<DocumentAnchor Name='jjs_documentanchor' Title='Accounting for Start-Up Costs'>";
			makefileContent += "<DocumentAnchor Name='aag-air_app_c.12' Title='12.'/>";
			makefileContent += "</DocumentAnchor>";
			makefileContent += "</DocumentAnchor>";
			makefileContent += "</Document>";
			makefileContent += "<Document Name='aag-air_app_d' Title='Appendix D'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_d.html' /></Document>";
			makefileContent += "<Document Name='aag-air_app_e' Title='Appendix E'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_e.html' /></Document>";
			makefileContent += "<Document Name='aag-air_fb7e1ae7-6608-416f-9c66-8df9a1119822' Title='aag-air_fb7e1ae7-6608-416f-9c66-8df9a1119822'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_fb7e1ae7-6608-416f-9c66-8df9a1119822.html' /></Document>";
			makefileContent += "<Document Name='jjs_document' Title='Glossary'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\glossary.html' /></Document>";
			makefileContent += "<BookFolder Name='stuff' Title='Stuff'>";
			makefileContent += "<Document Name='aag-air_app_d_in_bookfolder' Title='Appendix D_in_bookfolder'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_d.html' /></Document>";
			makefileContent += "<Document Name='aag-air_app_e_in_bookfolder' Title='Appendix E_in_bookfolder'><DocumentFormat Primary='True' Content-Type='text/html' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_e.html' /></Document>";
			makefileContent += "</BookFolder></Book></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public string CreateTestMakefile(string alternateUri)
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Book Name='aag-air' Title='AICPA Industry Audit Guide' Description='This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee&apos;s recommendations on and descriptions of financial accounting and reporting principles and practices for airlines.' Copyright='Copyright 2005'>";
			makefileContent += "<Document Name='aag-air1' Title='Chapter 1'><DocumentFormat Primary='True' Content-Type='text/html' Uri='" + alternateUri + "' /></Document>";
			makefileContent += "</Book></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public string CreateTestMakefileNewContentType(string contentType)
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Book Name='aag-air' Title='AICPA Industry Audit Guide' Description='This industry audit Guide presents recommendations of the AICPA Civil Aeronautics Subcommittee on the application of generally accepted auditing standards to audits of financial statements of airlines. This Guide also presents the committee&apos;s recommendations on and descriptions of financial accounting and reporting principles and practices for airlines.' Copyright='Copyright 2005'>";
			makefileContent += "<Document Name='aag-air1' Title='Chapter 1'><DocumentFormat Primary='True' Content-Type='" + contentType + "' Uri='\\\\destroyervm\\Destroyer_Content_Indexing\\test_content\\aag-air\\aag-air_app_d.html' /></Document>";
			makefileContent += "</Book></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public string CreateTestSiteMakefile(int book1Id, int book2Id, int book3Id)
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Site>";
			makefileContent += "<SiteFolder Name='site_folder_1' Title='Audit and Accounting Guides' Uri='http://www.cnn.com'>";
			makefileContent += "<Book Id='" + book1Id + "'/>";
			makefileContent += "<Book Id='" + book2Id + "'/>";
			makefileContent += "</SiteFolder>";
			makefileContent += "<SiteFolder Name='site_folder_2' Title='Destroyer Test Folder' Uri='http://www.knowlysis.com'>";
			makefileContent += "<Book Id='" + book3Id + "'/>";
			makefileContent += "</SiteFolder>";
			makefileContent += "</Site></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public string CreateTestSiteMakefile_Flat(int book1Id, int book2Id, int book3Id)
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Site>";
			makefileContent += "<Book Id='" + book1Id + "'/>";
			makefileContent += "<Book Id='" + book2Id + "'/>";
			makefileContent += "<Book Id='" + book3Id + "'/>";
			makefileContent += "</Site></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public string CreateTestSiteMakefile_FlatSingle(int bookId)
		{
			string makeTempfile = Path.GetTempFileName();
			string makefileContent = "<?xml version='1.0' encoding='utf-8'?><MakeFile>";
			makefileContent += "<Site>";
			makefileContent += "<Book Id='" + bookId + "'/>";
			makefileContent += "</Site></MakeFile>";

			StreamWriter sw = new StreamWriter(makeTempfile, false, System.Text.Encoding.UTF8);
			sw.WriteLine(makefileContent);
			sw.Close();
			return makeTempfile;
		}

		public static void DeleteNamedSite(string siteName)
		{
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DSB FROM D_SiteBook DSB, D_Site DS WHERE DSB.SiteId = DS.SiteId AND DS.[Name] = '" + siteName + "'");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DSF FROM D_SiteFolder DSF, D_Site DS, D_SiteToc DST WHERE DSF.FolderId = DST.NodeId AND DST.NodeTypeId = 7 AND DST.SiteId = DS.SiteId AND DS.[Name] = '" + siteName + "'");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DST FROM D_SiteToc DST, D_Site DS WHERE DST.SiteId = DS.SiteId AND DS.[Name] = '" + siteName + "'");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_Site WHERE [Name] = '" + siteName + "'");
		}

		public static void DeleteNamedBook(string bookName)
		{
			object bookInstanceId = SqlHelper.ExecuteScalar(TestShared.dbConnectionString, CommandType.Text, "SELECT BookInstanceId FROM D_BookInstance BI INNER JOIN D_Book B ON BI.BookId = B.BookId WHERE B.Name = '" + bookName + "'");
			if (bookInstanceId != null)
			{
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DDF FROM D_DocumentFormat DDF INNER JOIN D_DocumentInstance DD ON DDF.DocumentInstanceID = DD.DocumentInstanceID WHERE DD.BookInstanceId = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DNA FROM D_NamedAnchor DNA INNER JOIN D_DocumentInstance DD ON DNA.DocumentInstanceID = DD.DocumentInstanceID WHERE DD.BookInstanceId = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DBF FROM D_BookFolder DBF, D_BookToc DBT, D_BookInstance DB WHERE DBF.FolderId = DBT.NodeId AND DBT.NodeTypeId = 5 AND DBT.BookInstanceId = DB.BookInstanceId AND DB.[BookInstanceId] = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DBT FROM D_BookToc DBT WHERE DBT.BookInstanceId  = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DD FROM D_DocumentInstance DD WHERE DD.BookInstanceId = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DD FROM D_Document DD, D_Book DB WHERE DD.BookId = DB.BookId AND DB.[Name] = '" + bookName + "'" );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_BookInstance WHERE BookInstanceId = " + Convert.ToString(bookInstanceId) );
				SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE DB FROM D_Book DB WHERE DB.[Name] = '" + bookName + "'");
			}
		}

		public static void DeleteNamedSubscription(string subscriptionCode)
		{
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_SubscriptionBook WHERE SubscriptionCode = '" + subscriptionCode + "'");
			SqlHelper.ExecuteNonQuery(TestShared.dbConnectionString, CommandType.Text, "DELETE FROM D_Subscription WHERE SubscriptionCode = '" + subscriptionCode + "'");
		}
	}
}
