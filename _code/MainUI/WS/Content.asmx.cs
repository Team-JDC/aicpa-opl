using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Runtime.Serialization;
using System.Xml;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using MainUI.Shared;
using System.Configuration;
using AICPA.Destroyer.User.Event;
using System.Text.RegularExpressions;

namespace MainUI.WS
{
    /// <summary>
    /// Summary description for Content
    /// </summary>
    [WebService(Namespace="https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class Content : AicpaService
    {
        
        //http://stackoverflow.com/questions/313324/declare-a-dictionary-inside-a-static-class
        private static readonly Dictionary<string, string> NodeTypeMap = new Dictionary<string, string>() {{"SiteFolder" , "folder"}, 
                                {"Book" , "book"}, 
                                {"Document" , "doc"}};


        [WebMethod(true)]
        public List<SiteNode> GetReferencePath(int id, string type)

        {
            List<SiteNode> list = new List<SiteNode>();

            NodeType nodeType = SiteNode.ConvertToNodeType(type);
            // TODO: check and see if this is a Document before we try to instantiate it as one.

            string xmlPathString = string.Empty;

            switch (nodeType)
            {
                case NodeType.Document:
                    IDocument doc = new Document(id);

                    string targetDoc = doc.Book.Name;
                    string targetPointer = doc.Name;

                    ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);
                    xmlPathString = link.Document.SiteReferencePath;

                    break;

                case NodeType.SiteFolder:
                    ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);
                    xmlPathString = siteFolder.SiteReferencePath;
                    break;

                case NodeType.Book:
                    IBook book = new Book(CurrentSite, id);
                    xmlPathString = book.ReferencePath;
                    break;

                default:
                    throw new Exception(string.Format("Unexpected NodeType {0} passed into GetReferencePath web service method.", nodeType));
            }

            // To make the UI rendering job easier, we're going to put this into an array rather than an xml document
            // we may want to push this logic down to the API, and possible take out the step of generating xml altogether
            //string xmlPathString = doc.BookReferencePath;
            if (!string.IsNullOrEmpty(xmlPathString))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlPathString);

                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    list.Add(new SiteNode(node));
                }
            }

            return list;
        }

        [WebMethod(true)]
        public int countSubDocuments(int docId)
        {
            int idToCount = 0;
            NodeType currentType = NodeType.Document;

            //First get the Toc XML for the current document
            string tocXml = CurrentSite.GetTocXml(docId, NodeType.Document);
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(tocXml);
            //Parse the xml to get the document name
            string docName = tempDoc.SelectSingleNode("/Document").Attributes["Name"].Value;

            
            string site = CurrentSite.SiteXml;
            XmlDocument siteXml = new XmlDocument();
            siteXml.LoadXml(site);
            //If a book exists with the same name as the document, we'll use the book ID instead
            if (site.Contains("Book Name=\"" + docName + "\""))
            {
                int bookId = int.Parse(siteXml.SelectSingleNode("//Book[@Name='" + docName + "']").Attributes["Id"].Value);
                idToCount = bookId;
                currentType = NodeType.Book;
            }
            else
            {
                idToCount = docId;
            }

            int count = 0;
            //Call an alternative function to count the sub documents, this way we don't have to do all the logic to figure out the node type twice.
            count = countSubDocuments(idToCount, currentType);
            return count;

        }
        private int countSubDocuments(int docId, NodeType type)
        {
            int count = 0;
            string tocXml = CurrentSite.GetTocXml(docId, type);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(tocXml);
            //recurse through all child documents
            foreach (XmlNode node in doc.SelectNodes("/*[@HasChildren='True']/Document"))
            {
                count++;
                int id = int.Parse(node.Attributes["Id"].Value);
                count = count + countSubDocuments(id, NodeType.Document);
                
            }
            return count;
        }

        [WebMethod(true)]
        public BreadcrumbNode GetTocToNode(int id, string type)
        {
            BreadcrumbNode root = new BreadcrumbNode();

            NodeType nodeType = SiteNode.ConvertToNodeType(type);
            // TODO: check and see if this is a Document before we try to instantiate it as one.

            string xmlPathString = string.Empty;

            switch (nodeType)
            {
                case NodeType.DocumentAnchor:
                    IDocumentAnchor docAnchor = new DocumentAnchor(CurrentSite, id);
                    xmlPathString = docAnchor.SiteReferencePath;
                    break;
                
                case NodeType.Document:
                    IDocument doc = new Document(id);

                    string targetDoc = doc.Book.Name;
                    string targetPointer = doc.Name;

                    ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);
                    xmlPathString = link.Document.SiteReferencePath;

                    break;

                case NodeType.SiteFolder:
                    ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);
                    xmlPathString = siteFolder.SiteReferencePath;
                    break;

                case NodeType.Book:
                    IBook book = new Book(CurrentSite, id);
                    xmlPathString = book.ReferencePath;
                    break;

                //this could probably be done a lot more efficiently
                case NodeType.Site:
                    xmlPathString = CurrentSite.SiteXml;
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlPathString);
                    XmlNode node = xmlDoc.SelectSingleNode("//Site");
                    xmlPathString = "<ReferencePath>" + node.OuterXml + "</ReferencePath>";
                    break;

                default:
                    throw new Exception(string.Format("Unexpected NodeType {0} passed into GetTocToNode web service method.", nodeType));
            }

            if (!string.IsNullOrEmpty(xmlPathString))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlPathString);

                BreadcrumbNode tempBreadcrumbNode = null;
                int count = 0;

                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    count++;
                    
                    if (node.LocalName == "Site")
                    {
                        SiteNode tempSiteNode = new SiteNode(node);
                        root = new BreadcrumbNode()
                        {
                            SiteNode = tempSiteNode,
                            Children = GetAllChildren(tempSiteNode.Id, tempSiteNode.Type), // get children
                            Expanded = true
                        };

                        // get grandchildren
                        foreach (BreadcrumbNode childNode in root.Children)
                        {
                            childNode.Children = GetAllChildren(childNode.SiteNode.Id, childNode.SiteNode.Type);
                        }

                        // here is the genius of the whole operation.  We set the tempBreadcrubNode as the root
                        // so it becomes the proxy for the root node.  This builds our root node since we are
                        // passing tempBreadcrumbNode by reference.
                        tempBreadcrumbNode = root;
                    }
                    else if (node.LocalName == "SiteFolder" && count == 2 && tempBreadcrumbNode != null)
                    {
                        foreach (BreadcrumbNode childNode in tempBreadcrumbNode.Children)
                        {
                            if (Int32.Parse(node.Attributes.GetNamedItem("Id").Value) == childNode.SiteNode.Id)
                            {
                                tempBreadcrumbNode = childNode;
                                tempBreadcrumbNode.Expanded = true;
                                foreach (BreadcrumbNode grandchildNode in tempBreadcrumbNode.Children)
                                {
                                    grandchildNode.Children = GetAllChildren(grandchildNode.SiteNode.Id, grandchildNode.SiteNode.Type);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (BreadcrumbNode childNode in tempBreadcrumbNode.Children)
                        {
                            if (Int32.Parse(node.Attributes.GetNamedItem("Id").Value) == childNode.SiteNode.Id)
                            {
                                tempBreadcrumbNode = childNode;
                                tempBreadcrumbNode.Expanded = true;
                                foreach (BreadcrumbNode grandchildNode in tempBreadcrumbNode.Children)
                                {
                                    grandchildNode.Children = GetAllChildren(grandchildNode.SiteNode.Id, grandchildNode.SiteNode.Type);
                                }
                                break;
                            }
                        }
                    }
                } //end foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            }

            return root;
        }

        /// <summary>
        /// This method only returns the first 20 children and then puts a link to the TOC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<BreadcrumbNode> GetChildren(int id, string type)
        {
            // sburton 2010-05-20: Logic taken from D_Toc.aspx.cs/tocControl_NodeExpand.

            List<BreadcrumbNode> list = new List<BreadcrumbNode>();
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            string childXml = CurrentSite.GetTocXml(id, nodeType);
            
            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.SelectNodes("/*/*");

            int counter = 0;
            foreach (XmlNode node in nodes)
            {
                BreadcrumbNode breadcrumbNode = new BreadcrumbNode();
                breadcrumbNode.SiteNode = new SiteNode(node);
                list.Add(breadcrumbNode);

                counter++;
                if (counter >= 20)
                {
                    BreadcrumbNode ellipsesNode = new BreadcrumbNode();
                    SiteNode nodeContent = new SiteNode();
                    nodeContent.Id = -1;
                    nodeContent.Title = "Please see the Table of Contents for more...";
                    nodeContent.Type = "Document";
                    ellipsesNode.SiteNode = nodeContent;
                    list.Add(ellipsesNode);
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// This is the alternative of GetChildren that does not limit the number of children
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<BreadcrumbNode> GetAllChildren(int id, string type)
        {
            // sburton 2010-05-20: Logic taken from D_Toc.aspx.cs/tocControl_NodeExpand.

            List<BreadcrumbNode> list = new List<BreadcrumbNode>();
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            string childXml = CurrentSite.GetTocXml(id, nodeType);

            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.SelectNodes("/*/*");

            foreach (XmlNode node in nodes)
            {
                BreadcrumbNode breadcrumbNode = new BreadcrumbNode();
                breadcrumbNode.SiteNode = new SiteNode(node);
                list.Add(breadcrumbNode);
            }

            return list;
        }

        /// <summary>
        /// Returns the node and all the nodes underneath the target node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<BreadcrumbNode> GetNodeAndAllDescendants(int id, string type, bool ignoreAnchors = false)
        {
            List<BreadcrumbNode> list = new List<BreadcrumbNode>();

            IDocument doc = new Document(id);
            BreadcrumbNode rootNode = new BreadcrumbNode();
            rootNode.SiteNode = new SiteNode(doc);
            list.Add(rootNode);

            list.AddRange(GetAllDescendants(id, type, ignoreAnchors));

            return list;
        }

        /// <summary>
        /// Returns all the nodes underneath the target node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true)]        
        public List<BreadcrumbNode> GetAllDescendants(int id, string type, bool ignoreAnchors = false)
        {
            List<BreadcrumbNode> list = new List<BreadcrumbNode>();
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            string childXml = CurrentSite.GetTocXml(id, nodeType, ignoreAnchors);

            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.SelectNodes("/*/*");

            foreach (XmlNode node in nodes)
            {
                BreadcrumbNode breadcrumbNode = new BreadcrumbNode();
                breadcrumbNode.SiteNode = new SiteNode(node);
                list.Add(breadcrumbNode);
                list.AddRange(GetAllDescendants(breadcrumbNode.SiteNode.Id, breadcrumbNode.SiteNode.Type,ignoreAnchors));
            }

            return list;
        }

        [WebMethod(true)]
        public BreadcrumbNode GetNodeToGrandChildren(int id, string type)
        {
            BreadcrumbNode root = new BreadcrumbNode();

            if (type == "Site")
            {
                root.SiteNode = new SiteNode() { Id = CurrentSite.Id, Name = CurrentSite.Name, Title = CurrentSite.Title, Type = "Site" };
                id = CurrentSite.Id;
            }
            else
            {
                root.SiteNode = new SiteNode() { Id = id, Name = "placeholder", Title = "placeholder", Type = type };
            }

            root.Children = GetAllChildren(id, type);            

            foreach (BreadcrumbNode childNode in root.Children)
            {
                childNode.Children = GetAllChildren(childNode.SiteNode.Id, childNode.SiteNode.Type);
            }

            return root;
        }

        [WebMethod(true)]
        public BreadcrumbNode GetFullToc()
        {
            BreadcrumbNode root = new BreadcrumbNode();
            root.SiteNode = new SiteNode() { Id = CurrentSite.Id, Name = CurrentSite.Name, Title = CurrentSite.Title, Type = "Site" };
            
            
            NodeType nodeType = SiteNode.ConvertToNodeType("Site");
            //string childXml = CurrentSite.GetTocXml(CurrentSite.Id, nodeType);
            string childXml = CurrentSite.SiteXml;
            //string bookXml = CurrentSite.SiteBookXml;
            //string templateXml = CurrentSite.SiteTemplateXml;

            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.ChildNodes;
            
            nodes = nodes[1].ChildNodes;

            root.Children = PopulateChildren(nodes);

            return root;
        }

        public List<BreadcrumbNode> PopulateChildren(XmlNodeList nodes)
        {
            List<BreadcrumbNode> list = new List<BreadcrumbNode>();
            foreach (XmlNode node in nodes)
            {
                BreadcrumbNode breadcrumbNode = new BreadcrumbNode();
                breadcrumbNode.SiteNode = new SiteNode(node);

                if (node.Attributes["Name"].Value == "")
                {
                    continue;
                }
                else if (node.Attributes["Name"].Value == "AICPA Audit & Accounting Literature")
                {
                    continue;
                }
                else if (node.Attributes["Name"].Value == "FASB Accounting Standards Codification")
                {
                    continue;
                }
                else if (node.Attributes["Name"].Value == "GASB Library")
                {
                    continue;
                }
                else if (node.Attributes["Name"].Value == "Management of an Accounting Practice Handbook")
                {
                    continue;
                }
                else if (node.Attributes["Name"].Value == "Archive Library")
                {
                    continue;
                }
                
                // base case
                if (node.HasChildNodes)
                {
                    breadcrumbNode.Children = PopulateChildren(node.ChildNodes);
                }
                else
                {
                    string childXml = CurrentSite.GetTocXml(Int32.Parse(node.Attributes["Id"].Value), SiteNode.ConvertToNodeType(node.Name));
                    
                    XmlDocument siteXmlDoc = new XmlDocument();
                    siteXmlDoc.LoadXml(childXml);
                    XmlNodeList childNodes = siteXmlDoc.ChildNodes;

                    childNodes = childNodes[0].ChildNodes;

                    if (childNodes.Count > 0)
                    {
                        breadcrumbNode.Children = PopulateChildren(childNodes);
                    }
                }

                list.Add(breadcrumbNode);
            }
            
            /*
            foreach (BreadcrumbNode childNode in node.Children)
            {
                childNode.Children = GetAllChildren(childNode.SiteNode.Id, childNode.SiteNode.Type);

                // base case
                if (childNode.Children.Count > 0)
                {
                    PopulateChildren(childNode);
                }
            }
             */

            return list;
        }



        [WebMethod(true)]
        public BreadcrumbNode GetInitialTreeToc(int id, string type)
        {
            BreadcrumbNode bcNode = GetTocToNode(id, type);

            return bcNode;
        }
             

        [WebMethod(true)]
        public List<SiteNode> GetMyLibraryBooks()
        {
            BreadcrumbNode bcNode = GetTocToNode(-1,"Site");
            List<SiteNode> list = new List<SiteNode>();
            foreach(BreadcrumbNode bc in bcNode.Children) {
                if (bc.SiteNode.Type == "SiteFolder")
                {
                    list.Add(bc.SiteNode);
                }
            }
            return list;
        }

        [WebMethod(true)]
        public string GetInitialTreeTocHtml(int id, string type)
        {
            BreadcrumbNode bcNode = GetTocToNode(id, type);
            string treeViewHtml = convertToTreeViewHtml(bcNode, false, 0, true);
            HomePageNode node = new HomePageNode();
            node.Domain = CurrentUser.UserSecurity.Domain; 

            
            if (node.IsCoso)
            {
                string cosopath = node.CosoPath;
                string linkOnclick = "onclick=\"top.doCosoLink('" + CurrentUser.UserSecurity.User.UserId + "', '" + CurrentUser.UserSecurity.Domain + "', 'COSO','" + cosopath + "')\"";
                string cosolink = "<li " + linkOnclick + "><div class=\"acc_head\"><div class=\"acc_head_inner\"><h2 class=\"clearfix\"><span class=\"toggle\"></span><span class=\"spacer\">&nbsp;</span><span class=\"acc_text\">COSO Collection</span></h2></div></div></a></li>";
                treeViewHtml = treeViewHtml + cosolink;
            }
                
            return treeViewHtml;
            
        }

        [WebMethod(true)]
        public string GetNodeToGrandChildrenHtml(int id, string type, int level=0)
        {
            BreadcrumbNode root = GetNodeToGrandChildren(id, type);

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < root.Children.Count; i++)
            {                
                BreadcrumbNode childNode = root.Children[i];
                result.Append(convertToTreeViewHtml(childNode, (i == root.Children.Count - 1) ? true : false, level));                
            }

            return result.ToString();
        }

        private string GetTypeIconName(string type)
        {
            if (NodeTypeMap.ContainsKey(type)) return NodeTypeMap[type];
            else return string.Empty;
        }

      
        private MyDocument CheckLink(MyDocument md)
        {
            string reg = "EXTRN[0-9]{1,}~([^~]+)~(.*)";

            if (Regex.Matches(md.TargetPointer, reg).Count > 0)
            {
                Match m = Regex.Match(md.TargetPointer, reg);
                md.TargetDoc = m.Groups[1].Value;
                md.TargetPointer = m.Groups[2].Value;
            }
            return md;
        }
        //EXTRN67~faf-industry~905-360

        private string convertToTreeViewHtml(BreadcrumbNode node, bool isALastChild, int level = 0, bool initial=false)
        {
            int id = node.SiteNode.Id;
            string type = node.SiteNode.Type;
            string title = node.SiteNode.Title;
            string uniqueId = string.Format("{0}-{1}", id, type);

            StringBuilder result = new StringBuilder();
            string formatString = string.Empty;

            string childrenStr = string.Empty;
            if ((node.Children != null) && (node.Children.Count > 0))
                childrenStr = "children";
            else if ((node.Children != null) && (node.Children.Count == 0))
                childrenStr = "nochildren";
            else childrenStr = string.Empty;

            if (type == "Site")
            {
                //for (int i = 0; i < node.Children.Count; i++)
                //{
                //    BreadcrumbNode childNode = node.Children[i];
                //    result.Append(convertToTreeViewHtml(childNode, (i == node.Children.Count - 1) ? true : false));
                //}
            } else if (type == "SiteFolder")
            {
                formatString = @"<li id=""currentLi-{3}"">" +
                               @"<div class=""acc_head{4}"" {2}>" +
                               @"<div class=""acc_head_inner"">"+
                               @"<h2 class=""clearfix""><span class=""toggle""></span><span class=""icon {0}""></span>" +
                               @"<span class=""acc_text"">{1}</span></h2></div></div>";
                string divString = string.Format("onclick=\"toggleTocNode('{0}', '{1}', '{2}', {3});\"", id, type, uniqueId, level);
                //divString = string.Empty;
                result.AppendFormat(formatString,GetTypeIconName(type), title, divString, uniqueId, (level > 1 ? level.ToString() : string.Empty));
            }
            else if (type == "Book")
            {
                string link = BuildLink(id, type);
                formatString = @"<li id=""currentLi-{3}"">" +
                               @"<div class=""{5}"">" +
                               @"<span class=""toggle"" {4}></span>" +
                               @"<span class=""icon {0}""></span>" +                                
                               @"<a href=""{2}"" class=""clearfix"">" +                               
                               @"<span class=""acc_text"">{1}</span></a></div>";
                string divString = string.Format("onclick=\"toggleTocNode('{0}', '{1}', '{2}', {3});\"", id, type, uniqueId, level);
                result.AppendFormat(formatString, GetTypeIconName(type), title, link, uniqueId, divString, childrenStr);
            }
            else
            {
                string link = BuildLink(id, type);
                formatString = @"<li id=""currentLi-{3}"">" +
                               @"<div class=""{5}"">" +
                               @"<span class=""toggle"" {4}></span>" +
                               @"<span class=""icon {0}""></span></div>" +
                               @"<a href=""{2}"" class=""clearfix"">" +
                               @"<span class=""acc_text"">{1}</span></a>";
                string divString = string.Format("onclick=\"toggleTocNode('{0}', '{1}', '{2}', {3});\"", id, type, uniqueId, level);
                result.AppendFormat(formatString, GetTypeIconName(type), title, link, uniqueId,divString,childrenStr);
            }
                        
            if (type == "SiteFolder")
            {
                result.AppendFormat("<div class=\"acc_content{0}\" id=\"childUl-{1}\" style=\"display:none;\" >", (level > 1 ? level.ToString() : string.Empty), uniqueId);
                result.AppendFormat("<ul>");                
            }
            else if (type != "Site")
            {
                result.Append("<!-- " + type + " -->");                
                result.AppendFormat("<div class=\"acc_sub{0}\" id=\"childUl-{1}\" style=\"display:none;\" >", (level > 0 ? level.ToString() : string.Empty), uniqueId);
                result.AppendFormat("<ul>");                
            }
            
            if (node.Children != null && node.Children.Count > 0)
            {
                level++;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    BreadcrumbNode childNode = node.Children[i];
                    result.Append(convertToTreeViewHtml(childNode, (i == node.Children.Count - 1) ? true : false, level, initial));
                }
                level--;
            }
            else
            {

            }

            if (type != "Site")
            {
                result.Append("</ul></div>");
                result.Append("</li>");
            }

            return result.ToString();
        }

        private string BuildLink(int id, string type)
        {            
            MyDocument md = CheckLink(GetTargetDocPtrByBookIdDocType(id, type));
            if (md != null)
            {
                return string.Format("/content/link/{0}/{1}", md.TargetDoc, md.TargetPointer);
            }
            else
            {
                return string.Format("/content/{0}/{1}", type, id);
            }            
        }


        private string convertToTreeViewHtmlold(BreadcrumbNode node, bool isALastChild)
        {
            int id = node.SiteNode.Id;
            string type = node.SiteNode.Type;
            string title = node.SiteNode.Title;
            string uniqueId = string.Format("{0}-{1}", id, type);

            StringBuilder result = new StringBuilder();
            string formatString =
                @"<li {3} id=""currentLi-{0}"" siteNodeType=""{1}"">{4}<a href=""#"" onclick=""doTocLink({5}, '{1}');"" ><img src=""images/icon_{1}.gif"" alt=""{1} Icon"" class=""icon"" border=""0""/><span>{2}</span></a>";

            if (type == "Site")
            {
                formatString = "<li {3} id=\"currentLi-{0}\" siteNodeType=\"{1}\">{4}<span>{2}</span>";
            }

            if (node.Children != null && node.Children.Count > 0)
            {
                string classString = "class=\"expandable{0}\"";
                string divString = "<div id=\"currentDiv-{3}\" class=\"hitarea expandable-hitarea{0}\" onclick=\"toggleTocNode('{1}', '{2}', '{3}');\"></div>";

                if (isALastChild)
                {
                    classString = string.Format(classString, " lastExpandable");
                    divString = string.Format(divString, " lastExpandable-hitarea", id, type, uniqueId);
                }
                else
                {
                    classString = string.Format(classString, string.Empty);
                    divString = string.Format(divString, string.Empty, id, type, uniqueId);
                }

                result.AppendFormat(formatString, uniqueId, type, title, classString, divString, id);
                result.AppendFormat("<ul id=\"childUl-{0}\" style=\"display:none;\">", uniqueId);

                for (int i = 0; i < node.Children.Count; i++)
                {
                    BreadcrumbNode childNode = node.Children[i];
                    result.Append(convertToTreeViewHtml(childNode, (i == node.Children.Count - 1) ? true : false));
                }

                result.Append("</ul>");
            }
            else
            {
                string classString = string.Empty;

                if (isALastChild)
                {
                    classString = "class=\"last\"";
                }

                result.AppendFormat(formatString, uniqueId, type, title, classString, string.Empty, id);
            }

            result.Append("</li>");

            return result.ToString();
        }

        [WebMethod(true)]
        public BreadcrumbNode GetNodeToGrandChildrenByTargetDocTargetPointer(string targetDoc, string targetPointer)
        {
            ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);
            SiteNode siteNode = new SiteNode(link.Document);

            BreadcrumbNode root = new BreadcrumbNode();

            root.SiteNode = siteNode;
            root.Children = GetAllChildren(siteNode.Id, siteNode.Type);

            foreach (BreadcrumbNode childNode in root.Children)
            {
                childNode.Children = GetAllChildren(childNode.SiteNode.Id, childNode.SiteNode.Type);
            }

            return root;
        }

        [WebMethod(true)]
        public List<BreadcrumbNode> GetFullBreadcrumb(int id, string type)
        {
            // sburton: This code could likely be optimized, but it should work to just call the two methods above in connection with each other.

            List<BreadcrumbNode> list = new List<BreadcrumbNode>();

            List<SiteNode> refPathList;

            if (type == "Site")
            {
                refPathList = new List<SiteNode>();
                SiteNode siteNode = new SiteNode() { Id = CurrentSite.Id, Name = CurrentSite.Name, Title = CurrentSite.Title, Type = "Site" };
                refPathList.Add(siteNode);
            }
            else
            {
                refPathList = GetReferencePath(id, type);
            }

            foreach (SiteNode siteNode in refPathList)
            {
                BreadcrumbNode node = new BreadcrumbNode();
                node.SiteNode = siteNode;
                node.Children = GetAllChildren(siteNode.Id, siteNode.Type);

                list.Add(node);
            }

            return list;
        }

        /// <summary>
        /// Will return a breadcrumb for a given targetDoc and targetPtr
        /// </summary>
        /// <param name="targetDoc">Target Document</param>
        /// <param name="targetPtr">Target Pointer</param>
        /// <returns>string representation of the bread crumb</returns>
        [WebMethod(true)]
        public string GetFullBreadcrumbStrByTargetDocTargetPtr(string targetDoc, string targetPtr)
        {
            SubscriptionSiteNode ssn = ResolveContentLink(targetDoc, targetPtr);

            return GetFullBreadcrumbStr(ssn.Id, ssn.Type);
        }

        /// <summary>
        /// Will return the breadcrumb for a given document id and document type
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="type">Document Type</param>
        /// <returns>string representation of the bread crumb</returns>
        [WebMethod(true)]
        public string GetFullBreadcrumbStr(int id, string type)
        {
            List<BreadcrumbNode> crumbList = GetFullBreadcrumb(id, type);
            string returnString = string.Empty;
            foreach (BreadcrumbNode node in crumbList)
            {
                returnString += (returnString.Length > 0 ? " >> " : string.Empty) + node.SiteNode.Title;
            }
            return returnString;
        }

        /// <summary>
        /// Gets the table of contents for the current documents book
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetPtr"></param>
        /// <param name="routeNodeType"></param>
        /// <remarks>mgardner: I know this is checked in as me but jstockett really is the one who wrote it with some help from dwatson.</remarks>
        /// <returns></returns>
        [WebMethod(true)]
        public string GetFullTocStrByTargetDocTargetPtr(string targetDoc, string targetPtr, string routeNodeType)
        {
            if(string.IsNullOrEmpty(targetPtr))
            {
                List<BreadcrumbNode> BreadCrumbNodes = GetFullBreadcrumb(Convert.ToInt32(targetDoc) , routeNodeType);
                return convertToTreeViewHtml(findBookBreadCrumbNode(BreadCrumbNodes), false, 0);
            } else {
                SubscriptionSiteNode ssn = ResolveContentLink(targetDoc, targetPtr);
                List<BreadcrumbNode> BreadCrumbNodes = GetFullBreadcrumb(ssn.Id, ssn.Type);
                return convertToTreeViewHtml(findBookBreadCrumbNode(BreadCrumbNodes), false, 0);
            }
        }

        private BreadcrumbNode findBookBreadCrumbNode(List<BreadcrumbNode> nodes)
        {
            foreach (BreadcrumbNode node in nodes)
            {
                if (node.SiteNode.Type == "Book")
                {
                    return node;
                }
            }
            return null;
        }
        

        public string TrimTitle(string title)
        {
            int TITLE_MAX = 20;
            if (title.Length > TITLE_MAX)
            {
                return title.Substring(0, TITLE_MAX) + "...";
            } else return title;
        }

        public string EncodeString(string title) {
            title = title.Replace("'", "&#39;");
            return title;        
        }
        [WebMethod(true)]
        public string GetFullBreadcrumbLinksByTargetDocTargetPtr(string targetDoc, string targetPtr)
        {
            ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPtr);
            SiteNode siteNode = new SiteNode(link.Document);
            return GetFullBreadcrumbLinks(siteNode.Id, siteNode.Type);
        }

        /// <summary>
        /// Will return the breadcrumb for a given document id and document type
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="type">Document Type</param>
        /// <returns>string representation of the bread crumb</returns>
        [WebMethod(true)]
        public string GetFullBreadcrumbLinks(int id, string type)
        {
            List<BreadcrumbNode> crumbList = GetFullBreadcrumb(id, type);
            string returnString = string.Empty;
            StringBuilder sb = new StringBuilder();
            int i = 0;
            string title = string.Empty;
            sb.Append("<div class=\"leftcol_header\">");
            sb.Append("<div id='opl_breadcrumbs'><span class='fa-stack internalToc' onclick='ToggleInternalToc();'><i class='fa fa-square fa-stack-2x'></i><i class='fa fa-bars fa-stack-1x fa-inverse'></i></span>");
            foreach (BreadcrumbNode node in crumbList)
            {                
                if (i < crumbList.Count -1 )
                {
                    string href = string.Format("href='/content/{0}/{1}'", node.SiteNode.Type, node.SiteNode.Id);
                    if (node.SiteNode.Type == "Book")
                      href = string.Empty;
                    sb.Append(string.Format("<a {0} tooltip='{1}'>{2}</a>",href,EncodeString(node.SiteNode.Title),TrimTitle(node.SiteNode.Title)));
                    sb.Append("<span class='divider'>&gt;</span>");
                } else {
                    title = node.SiteNode.Title;
                }
                i++;                
            }
            sb.Append("</div>");
            sb.Append("<div id='tocContentHolder' style='display: none;'><img src='/images/loading-spinner.gif'></img></div>");
            sb.Append(string.Format("<h1>{0}</h1>", title));
            sb.Append("</div>");
            return sb.ToString();
        }


        /// <summary>
        /// Get Next Document 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="vct">View Complete Topic - True if turned on</param>
        /// <returns></returns>
        [WebMethod(true)]
        public SiteNode GetNextDocumentEthics(int id, string type, bool vct)
        {
            if (vct)
            {
                List<BreadcrumbNode> results = GetNodeAndAllDescendants(id, type);
                int i = results.Count - 1;
                while ((i >= 0) && (results[i].SiteNode.Type != NodeType.Document.ToString()))
                {
                    i--;
                }
                if (i < 0)
                    return GetNextDocument(id, type);
                else
                {
                    BreadcrumbNode doc = results[i];
                    return GetNextDocument(doc.SiteNode.Id, doc.SiteNode.Type);
                }
            }
            else
            {
                return GetNextDocument(id, type);
            }
        }



        [WebMethod(true)]
        public SiteNode GetNextDocument(int id, string type)
        {
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            SiteNode siteNode;

            if (nodeType == NodeType.Document)
            {
                IDocument nextDoc = CurrentSite.GetNextDocument(new Document(id));
                ContentWrapper wrapper = nextDoc.PrimaryContent;

                if (wrapper.HasDocPointer)
                {
                    ContentLink link = new ContentLink(CurrentSite, wrapper.TargetDoc, wrapper.TargetPointer);
                    wrapper = new ContentWrapper(link.Document);
                }

                siteNode = new SiteNode(wrapper.Document);
            }
            else if (nodeType == NodeType.SiteFolder)
            {
                // sburton 2010-06-02: rather than throw, just return the same document
                SiteFolder sf = new SiteFolder(CurrentSite, id);
                siteNode = new SiteNode() { Id = id, Type = type, Name = sf.Name, Title = sf.Title };
            }
            else
            {
                throw new Exception("GetNextDocument should only accept Document node types.");
            }

            return siteNode;
        }

        /// <summary>
        /// Get Next Document 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="vct">View Complete Topic - True if turned on</param>
        /// <returns></returns>
        [WebMethod(true)]
        public SiteNode GetPreviousDocumentEthics(int id, string type, bool vct)
        {
            if (vct)
            {
                List<BreadcrumbNode> results = GetNodeAndAllDescendants(id, type);
                int i = 0;
                while ((i < results.Count) && (results[i].SiteNode.Type != NodeType.Document.ToString()))
                {
                    i++;
                }
                if (i >= results.Count)
                    return GetPreviousDocument(id, type);
                else
                {
                    BreadcrumbNode doc = results[i];
                    return GetPreviousDocument(doc.SiteNode.Id, doc.SiteNode.Type);
                }
            }
            else
            {
                return GetPreviousDocument(id, type);
            }
        }


        [WebMethod(true)]
        public SiteNode GetPreviousDocument(int id, string type)
        {
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            SiteNode siteNode;

            if (nodeType == NodeType.Document)
            {
                IDocument prevDoc = CurrentSite.GetPreviousDocument(new Document(id));
                ContentWrapper wrapper = prevDoc.PrimaryContent;

                if (wrapper.HasDocPointer)
                {
                    ContentLink link = new ContentLink(CurrentSite, wrapper.TargetDoc, wrapper.TargetPointer);
                    wrapper = new ContentWrapper(link.Document);
                }

                siteNode = new SiteNode(wrapper.Document);
            }
            else if (nodeType == NodeType.SiteFolder)
            {
                // sburton 2010-06-02: rather than throw, just return the same document
                SiteFolder sf = new SiteFolder(CurrentSite, id);
                siteNode = new SiteNode() { Id = id, Type = type, Name = sf.Name, Title = sf.Title }; 
            }
            else
            {
                throw new Exception("GetPrevDocument should only accept Document node types.");
            }

            return siteNode;

        }

        private const string COPYRIGHT_DEFAULT_FORMAT_STRING = "Copyright &copy; {0}, American Institute of Certified Public Accountants, Inc. All Rights Reserved.";

        [WebMethod(true)]
        public string GetCopyrightNotice(int id, string type)
        {
            string defaultCopyright = string.Format(COPYRIGHT_DEFAULT_FORMAT_STRING, DateTime.Now.Year.ToString());
            string copyright = defaultCopyright;

            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            switch (nodeType)
            {
                case NodeType.Site:
                    copyright = defaultCopyright;
                    break;
                
                case NodeType.SiteFolder:
                    // NOTE: TODO: This probably isn't the best way to get SiteFolder's copyright notice.
                    // What it is doing is just traversing the table of contents tree and getting the
                    // first book or document it can find and getting the copyright notice.
                    XmlNode folderChildNode;
                    do
                    {
                        string folderXml = CurrentSite.GetTocXml(id, nodeType);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(folderXml);

                        folderChildNode = xmlDoc.SelectSingleNode("*/*");

                        id = Convert.ToInt32(folderChildNode.Attributes[ContextManager.XML_ATT_ID].Value);

                    } while (folderChildNode.LocalName == "SiteFolder");

                    if (folderChildNode.LocalName == "Document")
                    {
                        Document childDocument = new Document(id);
                        copyright = childDocument.Book.Copyright;
                    }
                    else if (folderChildNode.LocalName == "Book")
                    {
                        Book childBook = new Book(CurrentSite, id);
                        copyright = childBook.Copyright;
                    }
                    break;

                case NodeType.Book:
                    Book book = new Book(CurrentSite, id);
                    copyright = book.Copyright;
                    break;

                case NodeType.Document:
                    Document document = new Document(id);
                    copyright = document.Book.Copyright;
                    break;

                case NodeType.DocumentAnchor:
                    DocumentAnchor documentAnchor = new DocumentAnchor(id);
                    copyright = documentAnchor.Document.Book.Copyright;
                    break;

                default:
                    copyright = defaultCopyright;
                    break;
            }

            return copyright;
        }

        [WebMethod(true)]
        public SubscriptionSiteNode ResolveContentLink(string targetDoc, string targetPointer)
        {
            ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);
            if (link.IsInSubscription)
            {
                return ResolveContentByLink(link);
            }
            else
            {
                
                SubscriptionSiteNode node = new SubscriptionSiteNode();
                node.Restricted = !link.IsInSubscription;
                if (link.Document != null)
                {
                    node.Id = link.Document.Id;
                    node.Type = link.Document.NodeType.ToString();
                }
                else
                {
                    node.Id = -1;
                    node.Type = string.Empty;
                }                
                return node;
            }
        }
        
        [WebMethod(true)]
        public SubscriptionSiteNode ResolveContentByLink(ContentLink link)
        {            
            SubscriptionSiteNode siteNode = new SubscriptionSiteNode(link.Document, !link.IsInSubscription);
            siteNode.Anchor = link.DocumentAnchorName;
            return siteNode;
        }

        [WebMethod(true)]
        public SiteNodeExtraInfo ResolveContentLinkExtra(string targetDoc, string targetPointer)
        {
            ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);


            SiteNode next = null;
            SiteNode prev = null;
            SiteNodeExtraInfo nextE = null;
            SiteNodeExtraInfo prevE = null;

            if ((link.Document != null) && (link.Document.hasNextDocument()))
            {
                next = GetNextDocument(link.Document.Id, link.Document.NodeType.ToString());
                nextE = new SiteNodeExtraInfo(next, !link.IsInSubscription, targetDoc, next.Name);    
            }
            if ((link.Document != null) && (link.Document.hasPreviousDocument()))
            {
                prev = GetPreviousDocument(link.Document.Id, link.Document.NodeType.ToString());
                prevE = new SiteNodeExtraInfo(prev, !link.IsInSubscription, targetDoc, prev.Name);    
            }
            SiteNodeExtraInfo siteNode = null;
            if (link.IsInSubscription)
            {
                siteNode = new SiteNodeExtraInfo(link.Document, !link.IsInSubscription, targetDoc, targetPointer, prevE, nextE);                
                siteNode.Anchor = link.DocumentAnchorName;
                siteNode.Formats = GetDocumentFormats(link.Document.Id);
            }
            else
            {
                //siteNode = new SiteNodeExtraInfo(link.Document, !link.IsInSubscription, targetDoc, targetPointer, prevE, nextE);                
                siteNode = new SiteNodeExtraInfo();
                if (link.Document != null)
                {
                    siteNode.Id = link.Document.Id;
                    siteNode.Type = link.Document.NodeType.ToString();
                }
                else
                {
                    siteNode.Id = -1;
                    siteNode.Type = string.Empty;
                }
                siteNode.Restricted = !link.IsInSubscription;
                siteNode.TargetDoc = targetDoc;
                siteNode.TargetPtr = targetPointer;
            }

            return siteNode;
        }

        [WebMethod(true)]
        public SiteNodeExtraInfo ResolveContentLinkExtraByIdType(int id, string type)
        {
            try
            {
                MyDocument md = GetTargetDocPtrByBookIdDocType(id, type);
                if (md == null) return new SiteNodeExtraInfo();
                else return ResolveContentLinkExtra(md.TargetDoc, md.TargetPointer);
            }
            catch (Exception ex)
            {
                IEvent logEvent;
                logEvent = new Event(EventType.Error, DateTime.Now, DestroyerBpc.ERROR_SEVERITY_AJAX_FAILED,
                                                DestroyerBpc.MODULE_WEBSERVICES, "ResolveContentLinkExtraByIdType",
                                                "ResolveContentLinkExtraByIdType", 
                                                string.Format("ID: {0} Type: {1} -- Ex: {2}",id, type, ex.ToString()), ContextManager.CurrentUser);
                logEvent.Save(false);
            }
            return new SiteNodeExtraInfo();
        }

        [WebMethod(true)]
        public SiteNode ResolveSiteFolder(string folderName)
        {
            int folderId = 96;
            
            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(CurrentSite.SiteXml);

            XmlNodeList list = siteXmlDoc.SelectNodes("//SiteFolder");

            //JTS:  If we can find the folder with xpath, that is fastest.  Otherwise, we'll do some normalizing to the names to try and find the right one.
            XmlNode node = siteXmlDoc.SelectSingleNode("//SiteFolder[@Name=\"" + folderName + "\"]");
            if (node != null)
            {
                folderId = Int32.Parse(node.Attributes.GetNamedItem("Id").Value);
            }
            else
            {
                foreach (XmlNode listNode in list)
                {
                    string name = listNode.Attributes.GetNamedItem("Name").Value;

                    //parse names for ampersand and apostraphe characters that have caused errors in the past
                    name = name.Replace("&amp;", "and");
                    folderName = folderName.Replace("&amp;", "and");
                    name = name.Replace("&apos;", "'");
                    folderName = folderName.Replace("&apos;", "'");
                    name = name.Replace("&", "and");
                    folderName = folderName.Replace("&", "and");
                    if (name == folderName)
                    {
                        folderId = Int32.Parse(listNode.Attributes.GetNamedItem("Id").Value);
                        break;
                    }
                }
            }

            
            ISiteFolder siteFolder = new SiteFolder(CurrentSite, folderId);

            SiteNode siteNode = new SiteNode(siteFolder);

            return siteNode;
        }

        [WebMethod(true)]
        public SiteNode GetPrimaryContent(int id, string type)
        {
            SiteNode siteNode = null;

            NodeType nodeType = SiteNode.ConvertToNodeType(type);
            if (nodeType == NodeType.SiteFolder)
            {
                ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);
                
                siteNode = new SiteNode
                {
                    Id = id,
                    Name = siteFolder.Name,
                    Title = siteFolder.Title,
                    Type = type
                };

                return siteNode;
            }

            var container = PrimaryContentContainer.ConstructContainer(CurrentSite, id, type);
            ContentWrapper wrapper = container.PrimaryContent;

            // This is to catch external faf documents
            if (wrapper.HasDocPointer)
            {
                ContentLink link = new ContentLink(CurrentSite, wrapper.TargetDoc, wrapper.TargetPointer);
                wrapper = new ContentWrapper(link.Document);
            }

            if (wrapper.HasDocument)
            {
                siteNode = new SiteNode(wrapper.Document);

                if (wrapper.HasAnchor)
                {
                    siteNode.Anchor = wrapper.Anchor;
                }
            }
            else if (wrapper.HasUri)
            {
                if (nodeType == NodeType.SiteFolder)
                {
                    ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);

                    siteNode = new SiteNode
                        {
                            Id = id,
                            Name = siteFolder.Name,
                            Title = siteFolder.Title,
                            Type = type
                        };

                    if (wrapper.HasAnchor)
                    {
                        siteNode.Anchor = wrapper.Anchor;
                    }
                }
                else
                {
                    throw new Exception("Expected only SiteFolder nodes to have a Uri rather than Document");
                }
            }
            else
            {
                throw new Exception("Expected a node to resolve to either Uri or Document");
            }

            return siteNode;
        }


        //public UpsellData GetUpsellDataByDocAndPtr(string targetDoc, string targetPtr)
        //{
        //    ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPtr);

        //}

        [WebMethod(true)]
        public UpsellData GetUpsellData(int id, string type)
        {
            UpsellData upsellData = new UpsellData();
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            if (nodeType != NodeType.Document)
            {
                throw new Exception(string.Format("Upsell data is only provided for Document type.  Given type was: '{0}'", type));
            }
            else
            {
                IDocument doc = new Document(id);
                IBook book = doc.Book;

                upsellData.Document = new SiteNode(doc);
                upsellData.Book = new SiteNode(book);
                upsellData.UserGuid = CurrentUser.UserId.ToString();

                if (CurrentUser.ReferringSiteValue == ReferringSite.C2b || CurrentUser.ReferringSiteValue == ReferringSite.Csc)
                {
                    //upsellData.StoreUrl = "http://www.cpa2biz.com";
                    upsellData.StorePurchaseDomain = GetPurchaseDomain(book.Name);
                    upsellData.MissingLibraryName = "AICPA";

                    if (book.Name.StartsWith(ContextManager.BOOK_PREFIX_FASB) || book.Name.StartsWith(ContextManager.BOOK_PREFIX_GASB) || book.Name.StartsWith(ContextManager.BOOK_PREFIX_FAF))
                    {
                        upsellData.ShowStoreLink = false;
                        upsellData.ShowPhoneContact = true;

                        if (book.Name.StartsWith(ContextManager.BOOK_PREFIX_FASB))
                        {
                            //upsellData.MissingLibraryName = "FASB";
                            upsellData.MissingLibraryName = "FASB Accounting Standards Codification";
                        }
                        else if (book.Name.StartsWith(ContextManager.BOOK_PREFIX_GASB))
                        {
                            upsellData.MissingLibraryName = "GASB";
                        }
                        else if (book.Name.StartsWith(ContextManager.BOOK_PREFIX_FAF))
                        {
                            //upsellData.MissingLibraryName = "FAF";
                            upsellData.MissingLibraryName = "FASB Accounting Standards Codification";
                        }
                    }
                    else
                    {
                        upsellData.ShowStoreLink = true;
                        upsellData.ShowPhoneContact = false;
                    }
                }
                else
                {
                    upsellData.ShowStoreLink = false;
                    upsellData.ShowPhoneContact = false;
                }

            }

            return upsellData;
        }

        private string GetPurchaseDomain(string targetDoc)
        {
            // sburton 2010-06-028: some of this logic is a bit dubious, but I am bringing it over from
            // javascript in the old project and trying to keep it as consistent as possible in case
            // there were undocumented (and un-unit tested) assumptions going on

            string domain = targetDoc;

            bool containsAag = domain.Contains("aag");
            bool containsAra = domain.Contains("ara");
            bool containsEmap = domain.Contains("emap");

            if (containsEmap)
            {
                domain = "emap";
            }

            if (containsAag)
            {
                if (domain.Contains("dep"))
                {
                    domain = domain + ";ara-dep";
                }
                if (domain.Contains("brd"))
                {
                    domain = domain + ";ara-brd";
                }
                if (domain.Contains("con"))
                {
                    domain = domain + ";ara-con";
                }
                if (domain.Contains("ebp"))
                {
                    domain = domain + ";ara-ebp";
                }
                if (domain.Contains("hco"))
                {
                    domain = domain + ";ara-hco";
                }
                if (domain.Contains("inv"))
                {
                    domain = domain + ";ara-inv";
                }
                if (domain.Contains("lhi"))
                {
                    domain = domain + ";ara-ins";
                }
                if (domain.Contains("npo"))
                {
                    domain = domain + ";ara-npo";
                }
                if (domain.Contains("pli"))
                {
                    domain = domain + ";ara-ins";
                }
                if (domain.Contains("slg"))
                {
                    domain = domain + ";ara-slg";
                }
                if (domain.Contains("slv"))
                {
                    domain = domain + ";ara-slg";
                }
                if (domain.Contains("sla"))
                {
                    domain = domain + ";ara-sga";
                }
                if (domain.Contains("cir"))
                {
                    domain = domain + ";ara-cir";
                }

            }
            if (containsAra)
            {
                if (domain.Contains("dep"))
                {
                    domain = "aag-dep;" + domain;
                }
                if (domain.Contains("brd"))
                {
                    domain = "aag-brd;" + domain;
                }
                if (domain.Contains("con"))
                {
                    domain = "aag-con;" + domain;
                }
                if (domain.Contains("ebp"))
                {
                    domain = "aag-ebp;" + domain;
                }
                if (domain.Contains("hco"))
                {
                    domain = "aag-hco;" + domain;
                }
                if (domain.Contains("inv"))
                {
                    domain = "aag-lhi;" + domain;
                }
                if (domain.Contains("npo"))
                {
                    domain = "aag-npo;" + domain;
                }
                if (domain.Contains("slg"))
                {
                    domain = "aag-slv;" + domain;
                }
                if (domain.Contains("sga"))
                {
                    domain = "aag-sla;" + domain;
                }
                if (domain.Contains("cir"))
                {
                    domain = "aag-cir;" + domain;
                }
            }

            return domain;
        }

        /// <summary>
        /// Gets all available document formats for a given document id.
        /// </summary>
        /// <param name="id">The id of the "Document"</param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<FormatOption> GetDocumentFormatsNoFilter(int id)
        {
            List<FormatOption> list = new List<FormatOption>();

            IDocument doc = new Document(id);
            IDocumentFormatCollection formatCollection = doc.Formats;

            foreach (IDocumentFormat format in formatCollection)
            {
                ContentType contentType = (ContentType)format.ContentTypeId;
                FormatOption option = new FormatOption(contentType, doc.Id);
                list.Add(option);
            }

            return list;
        }


        /// <summary>
        /// Gets all available document formats for a given document id.
        /// </summary>
        /// <param name="id">The id of the "Document"</param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<FormatOption> GetDocumentFormats(int id)
        {
            List<FormatOption> list = new List<FormatOption>();

            IDocument doc = new Document(id);
            IDocumentFormatCollection formatCollection = doc.Formats;

            foreach (IDocumentFormat format in formatCollection)
            {
                ContentType contentType = (ContentType)format.ContentTypeId;

                if (contentType == ContentType.TextHtml || contentType == ContentType.TextWlh || contentType == ContentType.TextArch)
                {
                    // don't add it do the list
                }
                else
                {
                    FormatOption option = new FormatOption(contentType, doc.Id);
                    list.Add(option);
                }
            }

            return list;
        }

        [WebMethod(true)]
        public SiteFolderDetails GetSiteFolderDetails(int id)
        {
            ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);
            SiteFolderDetails details = new SiteFolderDetails(siteFolder);

            // TODO: Fill in description and images, etc.
            string siteFoldersXmlFile = ContextManager.XmlFile_SiteFolders;
            XmlDocument doc = new XmlDocument();
            doc.Load(siteFoldersXmlFile);

            XmlNode node = doc.SelectSingleNode(string.Format("//include/siteFolder[@name=\"{0}\"]", siteFolder.Name));

            if (node == null)
            {
                details.Description = "";
                details.ImageUrl = "";
            }
            else
            {
                //If there is a title in the siteFolder xml node, use that instead of the toc/site xml
                details.Title = (String.IsNullOrEmpty(node.Attributes.GetNamedItem("title").Value)) ? details.Title : node.Attributes.GetNamedItem("title").Value;
                //If there is a description, use that, otherwise leave it blank
                details.Description = (String.IsNullOrEmpty(node.Attributes.GetNamedItem("description").Value)) ? "" : node.Attributes.GetNamedItem("description").Value;
                //If there is an imageUrl, use it, otherwise leave it blank
                details.ImageUrl = (String.IsNullOrEmpty(node.Attributes.GetNamedItem("imageUrl").Value)) ? "" : node.Attributes.GetNamedItem("imageUrl").Value;
            }

            //details.Description = string.Format("{0} folder description here", details.Name);
            //details.ImageUrl = "images/aicpa-lp.gif";

            details.Children = GetAllChildren(details.Id, details.Type);

            XmlNodeList excludeList = doc.SelectNodes("//exclude/*");

            foreach (XmlNode excludeNode in excludeList)
            {
                if (excludeNode.Attributes["name"] != null && !string.IsNullOrWhiteSpace(excludeNode.Attributes["name"].Value))
                {
                    details.Children.RemoveAll(child => child.SiteNode.Name == excludeNode.Attributes["name"].Value);
                }
            }

            return details;
        }

        /// <summary>
        /// Returns all the nodes underneath the target node,  same as GetaAllDescendants, but this one will short circuit after a given count
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="ignoreAnchors"></param>
        /// <param name="returnAfter">After the # of breadcrumb values is greater than this then return back.</param>
        /// <returns></returns>
        [WebMethod(true)]
        public List<BreadcrumbNode> GetDescendants(int id, string type, bool ignoreAnchors = false, int returnAfter = 0)
        {
            List<BreadcrumbNode> list = new List<BreadcrumbNode>();
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            string childXml = CurrentSite.GetTocXml(id, nodeType, ignoreAnchors);

            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.SelectNodes("/*/*");

            foreach (XmlNode node in nodes)
            {
                BreadcrumbNode breadcrumbNode = new BreadcrumbNode();
                breadcrumbNode.SiteNode = new SiteNode(node);
                list.Add(breadcrumbNode);
                list.AddRange(GetDescendants(breadcrumbNode.SiteNode.Id, breadcrumbNode.SiteNode.Type, ignoreAnchors));
                if ((returnAfter != 0) && (list.Count > returnAfter))
                    return list;
            }

            return list;
        }

        [WebMethod(true)]
        public bool HasDescendants(int id, string type, bool ignoreAnchors = false)
        {
            List<BreadcrumbNode> list = new List<BreadcrumbNode>();

            IDocument doc = new Document(id);
            BreadcrumbNode rootNode = new BreadcrumbNode();
            rootNode.SiteNode = new SiteNode(doc);

            return GetDescendants(id, type, ignoreAnchors, 1).Count > 0;
        }


        private IDocument GetDocument(int id, string type, out string targetPtr)
        {
            IDocument doc = new Document(id);
            targetPtr = doc.Name;
            if (string.IsNullOrEmpty(targetPtr))
            {
                if (doc.Anchors.Count > 0)
                    targetPtr = doc.Anchors[0].Name;
            }
            return doc;
        }

        /// <summary>
        /// Get the TargetDocument and TargetPtr but give the option to use the Anchor or the Document Name
        /// http://www.codeproject.com/Articles/8117/WebMethods-Reference-parameters-and-OverLoading
        /// </summary>
        /// <param name="id">document id</param>
        /// <param name="type">document type</param>
        /// <param name="useAnchor">boolean flag to use anchor or not</param>
        /// <returns>MyDocument object</returns>
        [WebMethod(true)]
        public MyDocument GetTargetDocPtrByBookIdDocType(int id, string type)
        {
            MyDocument result = null;
            string key = string.Format("{0}_{1}_{2}",ContextManager.CurrentSite.Id,id,type.ToLower());
            if (ContextManager.MyDocumentHash.ContainsKey(key))
            {
                return (MyDocument)ContextManager.MyDocumentHash[key];
            }
            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            switch (nodeType)
            {
                case NodeType.Document:
                    string targetPtr = string.Empty;
                    IDocument doc = GetDocument(id, type,out targetPtr);                    
                    result = new MyDocument(doc.Book.Name, targetPtr, doc.Title);
                    break;
                case NodeType.Book :
                    IBook book = new Book(id);
                    if (book.Documents.Count > 0)
                    {
                        targetPtr = string.Empty;
                        doc = GetDocument(book.Documents[0].Id, book.Documents[0].NodeType.ToString(),out targetPtr);
                        //result = GetTargetDocPtrByBookIdDocType(book.Documents[0].Id, book.Documents[0].NodeType.ToString());
                        result = new MyDocument(doc.Book.Name, targetPtr, doc.Title);
                    }
                    break;
                case NodeType.DocumentAnchor:
                    DocumentAnchor documentAnchor = new DocumentAnchor(id);                                       
                    targetPtr = string.Empty;
                    result = new MyDocument(documentAnchor.Document.Book.Name, documentAnchor.Name, documentAnchor.Title);
                    break;
                default:
                    //Only handle Documents
                    break;
            }

            if (!ContextManager.MyDocumentHash.ContainsKey(key))
            {
                ContextManager.MyDocumentHash.Add(key, result);
            }
            return result;
        }

        /// <summary>
        /// Will generate a list of Library Books to show under the "Library" link for COSO (and others if 
        /// they choose to use it).
        /// 
        /// Make sure you look at the books listed in LibraryBooks
        /// 
        /// It reuses the "MyDocument" object to hold the info.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true)]
        public List<MyDocument> GetLibraryBooks()
        {
            string bookList = ConfigurationManager.AppSettings["LibraryBooks"];
            
            List<MyDocument> list = new List<MyDocument>();
            if (string.IsNullOrEmpty(bookList))
                return list;
            string[] books = bookList.Split(';');
            foreach (string book in books)
            {
                ContentLink link = new ContentLink(CurrentSite, book, book);
                SubscriptionSiteNode siteNode = new SubscriptionSiteNode(link.Document, !link.IsInSubscription);
                MyDocument md = new MyDocument();
                md.TargetDoc = book;
                md.TargetPointer = book;
                md.Title = link.Book.Title;
                list.Add(md);
            }
            return list;            
        }     

    }
}
