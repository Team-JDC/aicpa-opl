using System;
using System.Web;
using System.Web.SessionState;

using AICPA.Destroyer.Content.Document;
using MainUI.Shared;
using System.IO;

namespace MainUI.Handlers
{
    /// <summary>
    /// Summary description for DownloadDocument
    /// </summary>
    public class DownloadDocument : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //try to grab the current docid and docformatid from the request
            string docId = context.Request[ContextManager.REQPARAM_DOCID];
            string docFormatId = context.Request[ContextManager.REQPARAM_CURRENTDOCUMENTFORMATID];

            ContextManager contextManager = new ContextManager(context);

            //get the current document
            IDocument doc = new Document(Convert.ToInt32(docId));
            
            //initialize to null
            IDocumentFormat docFormat = null;
            AICPA.Destroyer.Shared.ContentType contentType = 0;

            if (!string.IsNullOrEmpty(docFormatId))
            {
                //get the actual document format object and pass it back
                contentType = (AICPA.Destroyer.Shared.ContentType)int.Parse(docFormatId);
                docFormat = doc.Formats[contentType];
            }

            if (docFormat == null)
            {
                string message = "<div>" + 
                    "<h1>Download Document</h1>" +
                    "<h2>Sorry, nothing was found for document '" + doc.Name + "' with the format '" + contentType + "'.</h2>" + 
                    "<p>If you were looking for 'What Links Here', this page simply means that there are no links to this document.</p>" + 
                    "<p>If you were looking for 'Archive', this page means that there are no archived documents.</p>" +
                    "</div>";
                context.Response.Write(message);
                context.Response.Flush();
            }
            else
            {
                docFormat.LogContentAccess(contextManager.CurrentUser);

                string filename = doc.Name;
                
                //There are certain documents where Lois didn't like the filename of the download changing, so we are going to make them use the book name rather than the document name.
                if (filename.Contains("fvs") && docFormat.Description=="application/pdf")
                {
                    filename = doc.Book.Name;
                }
                

                if (Path.HasExtension(docFormat.Uri))
                {
                    filename += Path.GetExtension(docFormat.Uri);
                }

                context.Response.AddHeader("Content-Type", contentType.ToString());
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                context.Response.AddHeader("Content-Length", docFormat.ContentLength.ToString());
                context.Response.ContentType = docFormat.Description;
                context.Response.BinaryWrite(docFormat.Content);
                context.Response.Flush();
            }
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