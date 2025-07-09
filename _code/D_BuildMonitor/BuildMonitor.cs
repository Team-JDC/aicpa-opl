using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Threading;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;

namespace AICPA.Destroyer
{
	public class BuildMonitor : System.ServiceProcess.ServiceBase
	{
		private BuildMonitor bm;
		private System.Threading.Timer stateTimer; 
		private TimerCallback timerDelegate;
		private static bool currentlyIndexing = false;
		private StreamWriter _StreamLogFile = null;
		private string _LogFileName = ConfigurationSettings.AppSettings["BuildMonitor_LogFile"];
		private int _SleepTime = Convert.ToInt32(ConfigurationSettings.AppSettings["BuildMonitor_SleepTime"]);
		private int _MaxLogFileSize = Convert.ToInt32(ConfigurationSettings.AppSettings["BuildMonitor_MaxLogFileSize"]);

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new BuildMonitor() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			bm = new BuildMonitor();
			timerDelegate = new TimerCallback(bm.Monitor);
			stateTimer = new System.Threading.Timer(timerDelegate, null, this._SleepTime, this._SleepTime);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			stateTimer.Dispose();
		}

		#region Monitoring
		/// <summary>
		/// Do our monitoring every time a timer event is raised
		/// </summary>
		/// <param name="stateObject"></param>
		private void Monitor(object stateObject)
		{
			if(!currentlyIndexing)
			{
				lock(this)
				{
					try
					{
						//set this flag to let other threads know that they do not have to try to call the monitor
						currentlyIndexing = true;

						//this.logMsg("----------------------------------------------------");
						//this.logMsg("Monitoring process started on: " + System.DateTime.Now);

						//function to build the books requested to be build
						this.buildBooks();

						//function to build sites
						this.buildSites();

						//function to build site indexes
						this.buildSiteIndexes();

						//this.logMsg("Monitoring done at :" + System.DateTime.Now);
						//this.logMsg("----------------------------------------------------");
					}
					finally
					{
						currentlyIndexing = false;
					}
				}
			}

		}

		/// <summary>
		/// 
		/// </summary>
		private void openLogFile()
		{
			FileStream fs = null;
			string fileToUse = this._LogFileName;
			
			if(!File.Exists(fileToUse) || new FileInfo(fileToUse).Length > _MaxLogFileSize)
			{
				fs = File.Create(fileToUse);
			}
			else
			{
				fs = File.Open(fileToUse,FileMode.Append);
			}

			this._StreamLogFile = new StreamWriter(fs);
		}

		/// <summary>
		/// 
		/// </summary>
		private void closeLogFile()
		{
			this._StreamLogFile.Flush();
			this._StreamLogFile.Close();
		}

		private void logMsg(string msg)
		{
			this.openLogFile();
			this._StreamLogFile.WriteLine(msg);
			this.closeLogFile();
		}

		/// <summary>
		/// 
		/// </summary>
		private void buildBooks()
		{
			try
			{
				int builtBooks = 0;
				int notBuiltBooks = 0;
				int totalToBuild = 0;
				IBookCollection books = new BookCollection(BookBuildStatus.BuildRequested);

				if(books.Count > 0)
				{
					this.logMsg("************************************");
					this.logMsg("Ready to build books (" + System.DateTime.Now + ")");
					this.logMsg("Total books to build: " + books.Count);

					foreach(IBook book in books)
					{
						this.logMsg(string.Format("Building book: {0} (id={1})", book.Name, book.Id));
						try
						{
							book.Build();
							if(book.BuildStatus == BookBuildStatus.Error)
							{
								notBuiltBooks++;
								this.logMsg(string.Format("Book not built: {0} (id={1})", book.Name, book.Id));

								Event eventError = new Event(book.Id.ToString());
								this.logMsg("Error description: " + eventError.getEventLogged(DestroyerBpc.MODULE_BOOK, DestroyerBpc.METHOD_BOOKBUILD));
							}
							else
							{
								builtBooks++;
							}
						}
						catch (Exception e)
						{
							Event bookEvent = new Event(EventType.Error,DateTime.Now,5, DestroyerBpc.MODULE_BOOK, DestroyerBpc.METHOD_BOOKBUILD, book.Id.ToString(), e.Message);
							bookEvent.Save(false);
							notBuiltBooks++;

							this.logMsg("Build error: "+e.Message);
						}
				
						totalToBuild++;
					}
			
					this.logMsg("Total books to build: " + totalToBuild);
					this.logMsg("Total books built: " + builtBooks);
					this.logMsg("Total books not built: " + notBuiltBooks);
					this.logMsg("Done building books (" + System.DateTime.Now + ")");
					this.logMsg("************************************");
				}
			}
			catch (Exception e)
			{
				this.logMsg("Book error: "+e.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void buildSites()
		{
			try
			{
				int builtSites = 0;
				int notBuiltSites = 0;
				int totalToBuild = 0;
				ISiteCollection sites = new SiteCollection(SiteBuildStatus.BuildRequested);
				
				if(sites.Count > 0)
				{
					this.logMsg("************************************");
					this.logMsg("Ready to build sites (" + System.DateTime.Now + ")");
					this.logMsg("Total sites to build: "+sites.Count);

					foreach(Destroyer.Content.Site.ISite site in sites)
					{
						this.logMsg(string.Format("Building site {0} (id={1})", site.Name, site.Id));
						try
						{
							site.Build();
							if(site.BuildStatus == SiteBuildStatus.Error)
							{
								notBuiltSites++;
								this.logMsg(string.Format("Sites not built: {0} (id={1})", site.Name, site.Id));

								Event eventError = new Event(site.Id.ToString());
								this.logMsg("Error description: "+eventError.getEventLogged(DestroyerBpc.MODULE_SITE, DestroyerBpc.METHOD_SITEBUILD));
							}
							else
							{
								this.logMsg("Site Built");

								builtSites++;
							}
						}
						catch (Exception e)
						{
							Event siteEvent = new Event(EventType.Error,DateTime.Now,5, DestroyerBpc.MODULE_SITE, DestroyerBpc.METHOD_SITEBUILD,site.Id.ToString(),e.Message);
							siteEvent.Save(false);
							notBuiltSites++;

							this.logMsg("Build error: "+e.Message);
						}
				
						totalToBuild++;
					}
			
					this.logMsg("Total sites to build: " + totalToBuild);
					this.logMsg("Total sites built: " + builtSites);
					this.logMsg("Total sites not built: " + notBuiltSites);
					this.logMsg("Done building sites (" + System.DateTime.Now + ")");
					this.logMsg("************************************");
				}
			}
			catch (Exception e)
			{
				this.logMsg("Site error: "+e.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void buildSiteIndexes()
		{
			try
			{
				int builtSiteIndexes = 0;
				int notBuiltSiteIndexes = 0;
				int totalToBuild = 0;
				ISiteCollection sites = new SiteCollection(SiteIndexBuildStatus.BuildRequested);
				
				if(sites.Count > 0)
				{
					this.logMsg("************************************");
					this.logMsg("Ready to build site indexes (" + System.DateTime.Now + ")");
					this.logMsg("Total site indexes to build: "+sites.Count);

					foreach(Destroyer.Content.Site.ISite site in sites)
					{
						this.logMsg(string.Format("Building site index {0} (id={1})", site.Name, site.Id));
						try
						{
							site.SiteIndex.Build();
							if(site.IndexBuildStatus == SiteIndexBuildStatus.Error)
							{
								notBuiltSiteIndexes++;
								this.logMsg(string.Format("Site indexes not built: {0} (id={1})", site.Name, site.Id));

								Event eventError = new Event(site.Id.ToString());
								this.logMsg("Error description: "+eventError.getEventLogged(DestroyerBpc.MODULE_SITEINDEX, DestroyerBpc.METHOD_SITEINDEXBUILD));
							}
							else
							{
								this.logMsg("Site Index Built");
								builtSiteIndexes++;
							}
						}
						catch (Exception e)
						{
							Event siteIndexEvent = new Event(EventType.Error,DateTime.Now,5, DestroyerBpc.MODULE_SITEINDEX, DestroyerBpc.METHOD_SITEINDEXBUILD,site.Id.ToString(),e.Message);
							siteIndexEvent.Save(false);
							notBuiltSiteIndexes++;
							this.logMsg("Build error: "+ e.Message);
						}
						totalToBuild++;
					}
			
					this.logMsg("Total site indexes to build: " + totalToBuild);
					this.logMsg("Total siteindexes built: " + builtSiteIndexes);
					this.logMsg("Total site indexes not built: " + notBuiltSiteIndexes);
					this.logMsg("Done building site indexes (" + System.DateTime.Now + ")");
					this.logMsg("************************************");
				}
			}
			catch (Exception e)
			{
				this.logMsg("Site index error: " + e.Message);
			}
		}		
		
		#endregion Monitoring
	}
}
