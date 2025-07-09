using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AICPA.Destroyer.Content.Document;
using System.Xml;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Site;

namespace MainUI.Shared
{
    /// <summary>
    /// This class is used to pass a small set of information about a document (such as a title and id)
    /// back to the UI.
    /// </summary>
    public class MyDocument
    {
        // TODO: sburton: We may consider putting this class and/or some of its logic in the API instead of the UI.
        public MyDocument() { }

        public MyDocument(string targetDoc, string targetPointer)
        {
            TargetDoc = targetDoc;
            TargetPointer = targetPointer;
            Title = string.Empty;
        }

        public MyDocument(string targetDoc, string targetPointer, string title)  : this(targetDoc, targetPointer)
        {
            Title = title;
        }

        public string Title { get; set; }
        public string TargetDoc { get; set; }
        public string TargetPointer { get; set; }
    }
}