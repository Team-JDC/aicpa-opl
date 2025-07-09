#region

using AICPA.Destroyer.User.Event;
using MainUI.Shared;
using System;
using System.Web;
using System.Web.Routing;

#endregion

namespace MainUI
{
    public class Global : HttpApplication
    {
        // Ignoring a route: http://stackoverflow.com/questions/2704338/asp-net-4-0-web-forms-routing-default-wildcard-route
        static void RegisterRoutes(RouteCollection routes)
        {
            //routes.Ignore("singlesignonhelper/{*catchall}");
            //routes.Ignore("{*path}", new { path = @"singlesignonhelper\/(.*)" });
            routes.Ignore("elmah.axd");
            routes.Ignore("PrintDocument.ashx");
            routes.Ignore("PrintNotes.ashx");
            

            routes.MapPageRoute("default", "default", "~/Default.aspx");
            /**************************************************************
             *  tooltype - The tool operationi to perform.
             *    notes - 
             *    bookmarks
             *  nodetype - This is the Node Type or the Document Type
             *      link
             *      site
             *      document
             *  targetdoc - The targetdoc or documentId
             *  targetptr 
             */
            routes.MapPageRoute("toolsarchive", "tools/{tooltype}/{nodetype}/{targetdoc}/{*targetptr}", "~/Tools.aspx",
                false,
                new RouteValueDictionary
                   {{"tooltype", ""}, {"nodetype", ""}, {"targetdoc", ""}, {"targetptr",""}});
            //routes.MapPageRoute("toolsarchived", "tools/loadarchived/{*nodetype}", "~/Tools.aspx");
            //routes.MapPageRoute("tools", "tools/", "~/Tools.aspx");
            /**************************************************************
             *  tooltype - The tool operationi to perform.
             *    notes - 
             *    bookmarks
             *  nodetype - This is the Node Type or the Document Type
             *      link
             *      site
             *      document
             *  targetdoc - The targetdoc or documentId
             *  targetptr 
             */ 
            routes.MapPageRoute("content", "content/{nodetype}/{targetdoc}/{*targetptr}", "~/ContentPage.aspx");
            routes.MapPageRoute("search", "search", "~/SearchPage.aspx");
            //routes.MapPageRoute("content", "content/{targetdoc}/{targetptr}", "~/ContentPage.aspx");
            routes.MapPageRoute("imags", "content/{targetdoc}/images", "~/");
            //routes.Ignore("{*images}", new { images = @"images/.*" });
            routes.MapPageRoute("logout", "logout", "~/LogoutPage.aspx");
            routes.MapPageRoute("login", "login", "~/LoginPage.aspx");
            routes.MapPageRoute("error", "error", "~/Error.aspx");
            routes.MapPageRoute("sessionexpired", "sessionexpired", "~/SessionExpired.aspx");
            routes.MapPageRoute("keep-alive", "keep-alive", "~/Keep-Alive.aspx");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            
          RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                // TODO: Log this error to a server log file

                // Do we really want to pass the error message on the URL rather than just
                // having the error page get the last error from the server?
                string hideReturnLink = string.Empty;
                string showLoginLink = string.Empty;
                HttpContext context = HttpContext.Current;
                Exception ex = context.Server.GetLastError();
                //Exception of type 'System.Web.HttpUnhandledException' was thrown.
                
                if ((ex.InnerException != null) && (ex.InnerException.Source.Contains("Destroyer")))
                {
                    ex = ex.InnerException;
                }

                if (ex.Message.Contains("DUPLICATE USER"))
                {
                    EndUserSession();
                    hideReturnLink = "&hideReturnLink=true";
                    showLoginLink = "&showLoginLink=true";
                }
          

                string urlMessage = Server.UrlEncode(ex.Message);
                if (ex.InnerException != null)
                {
                    urlMessage = urlMessage + "---" + Server.UrlEncode(ex.InnerException.Message);
                }
                string stackTrace = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    stackTrace = ex.InnerException.StackTrace;
                }


                AICPA.Destroyer.User.IUser user = null;
                IEvent logEvent = null;
                if (HttpContext.Current != null)
                {

                    string message = urlMessage;
                    var contextManager = new ContextManager(HttpContext.Current);

                    if (contextManager.CurrentUser == null)
                    {
                        message += " (Unknown user. Session already destroyed.)";
                    }
                    else
                    {
                        user = contextManager.CurrentUser;
                    }

                    logEvent = new Event(EventType.Usage,
                           DateTime.Now,
                           3,
                           "Global.asax.cs",
                           "Application_Error",
                           "Application Error",
                           urlMessage+" "+stackTrace,
                           user);
                    logEvent.Save();
                }
                else
                {
                    logEvent = new Event(EventType.Usage,
                           DateTime.Now,
                           3,
                           "Global.asax.cs",
                           "Application_Error",
                           "Application Error",
                           urlMessage + " " + stackTrace);
                    logEvent.Save();
                }



                // I believe the URL encode method takes care of these, but just in case, we don't want any new line characters
                urlMessage = urlMessage.Replace("\n", "");
                urlMessage = urlMessage.Replace("\r", "");

                Response.Redirect("/Error?ex=" + urlMessage+hideReturnLink+showLoginLink,true);
            }
            catch
            {
                // is it not a good idea to have this method throw an exception
                // for debugging purposes I'm going to leave it in for now

                // TODO: remove this and log the error instead
                throw;
            }
            
        }

        private void EndUserSession()
        {
            Session.Abandon();
            string[] myCookies = Request.Cookies.AllKeys;
            string cookieList = string.Empty;
            foreach (string cookie in myCookies)
            {
                //cookieList += cookie + ";";
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

            try
            {
                AICPA.Destroyer.User.IUser user = null;
                string message = "User session ended.";
                IEvent logEvent = null;
                if (HttpContext.Current != null)
                {
                    var contextManager = new ContextManager(HttpContext.Current);

                    if (contextManager.CurrentUser == null)
                    {
                        message += " (Unknown user. Session already destroyed.)";
                    }
                    else
                    {
                        user = contextManager.CurrentUser;
                    }
                    logEvent = new Event(EventType.Usage,
                        DateTime.Now,
                        3,
                        "Global.asax.cs",
                        "Session_End",
                        "Session Ended",
                        message,
                        user);
                    logEvent.Save(false);
                }
            }
            catch
            {
                //nothing
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}