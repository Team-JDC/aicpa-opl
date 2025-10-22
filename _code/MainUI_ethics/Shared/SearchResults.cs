using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AICPA.Destroyer.Content.Document;
using System.Xml;
using AICPA.Destroyer.Shared;

namespace MainUI.Shared
{
    /// <summary>
    /// This class is used to pass a small set of information about a document (such as a title and id)
    /// back to the UI.
    /// </summary>
    public class DimensionNavigationResult
    {
       public string DimensionId { get; set; }
       public string DimensionName { get; set; }
       public string DimensionValue { get; set; }
       public string DimensionCompletePath { get; set; }

    }

    /// <summary>
    /// This class is used to pass back information about the next/previous document for the
    /// hit prev/next document
    /// </summary>
    public class HitDocResult
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class SearchResult : SiteNode
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        public SearchResult(XmlNode xmlNode)
        {
            if (xmlNode.Attributes[DestroyerBpc.XML_ATT_ID] != null)
            {
                string idString = xmlNode.Attributes[DestroyerBpc.XML_ATT_ID].Value;

                Id = Convert.ToInt32(idString);
            }

            if (xmlNode.Attributes[DestroyerBpc.XML_ATT_NAME] != null)
            {
                Name = xmlNode.Attributes[DestroyerBpc.XML_ATT_NAME].Value;
            }

            if (xmlNode.Attributes[DestroyerBpc.XML_ATT_TITLE] != null)
            {
                Title = xmlNode.Attributes[DestroyerBpc.XML_ATT_TITLE].Value;
            }

            Type = xmlNode.Name;
            
            if (xmlNode.Attributes[DestroyerBpc.XML_ATT_EXT_KEYWORDS] != null)
            {
                Snippet = xmlNode.Attributes[DestroyerBpc.XML_ATT_EXT_KEYWORDS].Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public SearchResult(IDocument doc)
        {
            
            Id = doc.Id;
            Name = doc.Name;
            Title = doc.Title;
            ReferencePath = CreateReferencePathHtml(doc);
            Type = Enum.GetName(typeof(NodeType), NodeType.Document);
            Snippet = doc.KeyWordsInContext + " ";
            InSubscription = doc.InSubscription;
            SitePath = doc.SiteReferencePath;
            

        }

        /// <summary>
        /// Creates the reference path HTML.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private static string CreateReferencePathHtml(IDocument doc)
        {
            //set our site ref path xml string and our in subscription flag
            string siteRefPathXml = doc.SiteReferencePath;

            //default to an empty string
            string retHtml = "";

            //use an xml document ot load our refpath xml string
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(siteRefPathXml);

            //get the book node (for later...)
            XmlNode bookNode = xmlDoc.SelectSingleNode("*/" + DestroyerBpc.XML_ELE_BOOK);

            //go through each refpath node and add to the html
            XmlNodeList nodes = xmlDoc.SelectNodes("*/*");
            int nodeCount = nodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode node = nodes[i];
                string title = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_TITLE]);

                //if we are at the last hit and we are a subscription document, insert a document link around the title
                if (i == nodeCount - 1)
                {
                    string bookName = DestroyerBpc.GetAttributeValue(bookNode.Attributes[DestroyerBpc.XML_ATT_NAME]);
                    string docName = DestroyerBpc.GetAttributeValue(node.Attributes[DestroyerBpc.XML_ATT_NAME]);
                   
                }

                //send out the title
                retHtml += title;

                //insert a separator character between refpath nodes
                if (i < nodeCount - 1)
                {
                    retHtml += " > ";
                }

                
            }

            //return our html
            return retHtml;
        }
        public SearchResult() { }
        public bool InSubscription { get; set; }
        public string SitePath { get; set; }
        public string Snippet { get; set; }
        public string ReferencePath { get; set; }
        public int ResultEnumeration { get; set; }

    }
}