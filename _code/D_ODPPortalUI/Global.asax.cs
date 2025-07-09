using System;
using System.Configuration;
using System.Web;
using AICPA.Destroyer.Shared;
using D_ODPPortalUI.Shared;

namespace D_ODPPortalUI
{
    public class Global : HttpApplication
    {
        private void Session_Start(object sender, EventArgs e)
        {
            Session[DestroyerUi.SESSPARAM_CURRENTSITESTATUS] = Enum.Parse(typeof (SiteStatus),
                                                                          ConfigurationManager.AppSettings[
                                                                              "PortalApp_SiteStatus"], true);
        }

        //*********************************************************************
        //
        // Application_BeginRequest Event
        //
        // The Application_BeginRequest method is an ASP.NET event that executes 
        // on each web request into the portal application.  The below method
        // obtains the current tabIndex and TabId from the querystring of the 
        // request -- and then obtains the configuration necessary to process
        // and render the request.
        //
        // This portal configuration is stored within the application's "Context"
        // object -- which is available to all pages, controls and components
        // during the processing of a single request.
        // 
        //*********************************************************************
        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // sburton 2010-04-21: This culture stuff may have just been for the portal UI.
            // I am commenting it out.  If we find we need it later, we can put it back.

            //try
            //{
            //    if (Request.UserLanguages != null)
            //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
            //    else
            //        // Default to English if there are no user languages
            //        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            //}
        }

        //*********************************************************************
        //
        // Application_AuthenticateRequest Event
        //
        // If the client is authenticated with the application, then determine
        // which security roles he/she belongs to and replace the "User" intrinsic
        // with a custom IPrincipal security object that permits "User.IsInRole"
        // role checks within the application
        //
        // Roles are cached in the browser in an in-memory encrypted cookie.  If the
        // cookie doesn't exist yet for this session, create it.
        //
        //*********************************************************************
        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            // please note that this gets called for each request, images, css, and all
        }

        //*********************************************************************
        //
        // GetApplicationPath Method
        //
        // This method returns the correct relative path when installing
        // the portal on a root web site instead of virtual directory
        //
        //*********************************************************************
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetApplicationPath(HttpRequest request)
        {
            string path = string.Empty;
            try
            {
                if (request.ApplicationPath != "/")
                    path = request.ApplicationPath;
            }
            catch (Exception e)
            {
                throw e;
            }

            return path;
        }

        private void InitializeComponent()
        {
            // 
            // Global
            // 
            Error += Application_Error;
        }

        //************************************************************************
        //
        // Application_Error
        //
        //  This Event will capture the error at the application error
        //
        //************************************************************************
        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            Exception exception = Server.GetLastError().GetBaseException();

            try
            {
                Session["Error_Msg"] = exception.Message;
                Session["Error_Desc"] = exception.StackTrace;
                Session["Error_Page"] = exception.TargetSite.ToString();
            }
            catch
            {
                // we couldn't log the message to the session (likely because it wasn't available at this point.
            }
        }
    }
}