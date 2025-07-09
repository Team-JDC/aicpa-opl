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
    public class SiteNode
    {
        // TODO: sburton: We may consider putting this class and/or some of its logic in the API instead of the UI.
        public SiteNode() { }

        public SiteNode(IDocument doc)
        {
            Id = doc.Id;
            Name = doc.Name;
            Title = doc.Title;
            Type = Enum.GetName(typeof(NodeType), NodeType.Document);
            Anchor = null;

            hasPrevious = doc.hasPreviousDocument();

            hasNext = doc.hasNextDocument();
            copyright = doc.Book.Copyright.Replace("&copy;", "©");
        }

        public SiteNode(IBook book)
        {
            Id = book.Id;
            Name = book.Name;
            Title = book.Title;
            Type = Enum.GetName(typeof(NodeType), book.NodeType);
            Anchor = null;
            hasPrevious = false;
            hasNext = false;
            copyright = book.Copyright.Replace("&copy;", "©");

        }

        public SiteNode(ISiteFolder siteFolder)
        {
            Id = siteFolder.Id;
            Name = siteFolder.Name;
            Title = siteFolder.Title;
            Type = Enum.GetName(typeof(NodeType), siteFolder.NodeType);
            hasPrevious = false;
            hasNext = false;
            copyright = null;
        }

        public SiteNode(XmlNode xmlNode)
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
        }

        /// <summary>
        /// Converts a list of IDocuments to a list of SiteNodes
        /// </summary>
        /// <param name="docList"></param>
        /// <returns></returns>
        public static List<SiteNode> ConvertList(List<IDocument> docList)
        {
            return docList.Select(d => new SiteNode(d)).ToList<SiteNode>();
        }

        public static NodeType ConvertToNodeType(string nodeTypeString)
        {
            // there is probably a better way to do this.
            NodeType nodeType = (NodeType)Enum.Parse(typeof(NodeType), nodeTypeString);

            return nodeType;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Anchor { get; set; }
        public bool hasPrevious { get; set; }
        public bool hasNext { get; set; }
        public string copyright{ get;set;  }
        public bool hasCopyright
        {
            get
            {
                return (!string.IsNullOrEmpty(copyright));
            }
        }
    }
}