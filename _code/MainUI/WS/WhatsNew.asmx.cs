#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using AICPA.Destroyer.Content.Book;

#endregion

namespace MainUI.WS
{
    /// <summary>
    ///   Summary description for WhatsNew
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class WhatsNew : AicpaService
    {
        private const string XML_TEXT = "text";
        private const string XML_BOOK_ID = "bookId";
        
        /// <summary>
        /// Gets the whats new.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "This is the what's new web service.")]
        public List<WhatsNewResult> GetWhatsNew()
        {
            string xmlFilePath = ContextManager.XmlFile_WhatsNew;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            List<WhatsNewResult> list = new List<WhatsNewResult>();

            foreach (XmlNode newsItemNode in xmlDoc.DocumentElement.ChildNodes)
            {
                if (newsItemNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                string bookName;

                if (newsItemNode.Attributes[XML_BOOK_ID] == null || string.IsNullOrEmpty(newsItemNode.Attributes[XML_BOOK_ID].Value))
                {
                    throw new Exception("A News Item must have book id");
                }
                else
                {
                    bookName = newsItemNode.Attributes[XML_BOOK_ID].Value;
                }

                IBook book = CurrentSite.Books[bookName];

                if (book != null)
                {
                    // this means the book is in our subscription

                    WhatsNewResult result = new WhatsNewResult();

                    result.TargetDocument = bookName;
                    result.TargetPointer = book.Documents[0].Name;
                    result.NewsDate = book.PublishDate.ToShortDateString();

                    if (newsItemNode.Attributes[XML_TEXT] == null || string.IsNullOrEmpty(newsItemNode.Attributes[XML_TEXT].Value))
                    {
                        result.NewsText = book.Title;
                    }
                    else
                    {
                        // use the provided text
                        result.NewsText = newsItemNode.Attributes[XML_TEXT].Value;
                    }

                    list.Add(result);
                }


            }

            return list;
        }
    }

    [Serializable]
    public struct WhatsNewResult
    {
        public string NewsDate;
        public string NewsText;
        public string TargetDocument;
        public string TargetPointer;
    }

}