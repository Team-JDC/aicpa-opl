using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using MainUI.Shared;
using System.IO;

namespace MainUI.Handlers
{
    /// <summary>
    /// Summary description for GetArchiveContent
    /// </summary>
    public class GetArchiveContent : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request[ContextManager.REQPARAM_ID];
            string subFolder = context.Request[ContextManager.REQPARAM_SUB];

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("Error: An id must be given to request archive content");
            }
            else if (id.IndexOf(Path.DirectorySeparatorChar) >= 0 || id.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            {
                 //throw an error if there are path characters in the resource name (prevent payload attacks)
                throw new Exception(string.Format("Id of '{0}' is not valid", id));
            }

            ContextManager contextManager = new ContextManager(context);

            string directory = Path.Combine(contextManager.BookContentFolder, contextManager.FafArchiveFolder);
            string filename = id;

            if (!string.IsNullOrWhiteSpace(subFolder))
            {
                if (subFolder.IndexOf(Path.DirectorySeparatorChar) >= 0 || subFolder.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
                {
                    //throw an error if there are path characters in the resource name (prevent payload attacks)
                    throw new Exception(string.Format("sub of '{0}' is not valid", subFolder));
                }

                directory = Path.Combine(directory, subFolder);
            }

            string filePath = Path.Combine(directory, filename);
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("File not found '{0}'", filePath));
            }

            // Serve up the file
            string mimeType;

            if (filename.Contains(".htm"))
            {
                mimeType = "text/html";
            }
            else
            {
                mimeType = MimeTypeUtil.CheckType(filePath);
            }

            context.Response.ContentType = mimeType;
            context.Response.AppendHeader("content-disposition", "filename=" + filename);
            context.Response.AppendHeader("Cache-Control", "no-cache, must-revalidate");

            context.Response.WriteFile(filePath);
            context.Response.End();

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}