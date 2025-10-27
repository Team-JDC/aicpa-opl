using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using AICPA.Destroyer.User.Note;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content;
using MainUI.Shared;

namespace MainUI.Handlers
{
    public class PrintNotes : IHttpHandler, IRequiresSessionState
    {
        public string noteIds = null;
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            noteIds = context.Request["noteIds"];
            ContextManager contextManager = new ContextManager(context);
            
            string docString = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            docString += "<html xmlns=\"http://www.w3.org/1999/xhtml\">";
            docString += "<head>";
            docString += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />";
            docString += "<title>Print My Notes</title>";
            docString += "<script language=\"JavaScript\">";
            docString += "    function readyToPrint(){window.print();}";
            docString += "</script>";
            docString += "</head>";
            docString += "<body onload=\"readyToPrint();\">";
            docString += "<p style=\"font:bold 20px Georgia, 'Times New Roman', Times, serif; padding:25px 25px 10px; margin:0;\">My Notes</p>";

            foreach (string id in noteIds.Split(','))
            {
                int noteId = Convert.ToInt32(id);
                INote note = Note.GetNoteById(noteId);
                var contentLink = new ContentLink(contextManager.CurrentSite, note.TargetDoc, note.TargetPtr);
                docString += "<div style=\"margin:0 25px; padding:10px 0; border-top:1px solid #ccc;\">";
                docString += "     <p style=\"font:bold 14px Georgia, 'Times New Roman', Times, serif; margin-bottom: 0px;\">" + note.Title + "</p>";
                docString += "     <p style=\"font:11px Georgia, 'Times New Roman', Times, serif; margin-top: 0px;\">" + contentLink.Document.Title + "</p>";
                docString += "     <p style=\"font:12px Georgia, 'Times New Roman', Times, serif; line-height:1.5em;\">" + note.Text + "</p>";
                docString += "</div>";
            }
            
            docString += "</body>";
            docString += "</html>";

            context.Response.Write(docString);
            context.Response.End();
        }
    }
}