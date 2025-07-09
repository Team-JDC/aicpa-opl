namespace AICPA.Destroyer.UI.Portal.DesktopModules
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Xml;
    using Telerik.Web.UI;

    using AICPA.Destroyer.Shared;
    using AICPA.Destroyer.Content;
    using AICPA.Destroyer.Content.Site;
    using AICPA.Destroyer.Content.Book;
    using AICPA.Destroyer.Content.Document;


    /// <summary>
    ///		Summary description for D_Toc.
    /// </summary>
    public partial class D_Toc : AICPA.Destroyer.UI.Portal.PortalModuleControl
    {

        #region Event Handlers
        /// <summary>
        /// Handles the page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //get the site
                ISite site = DestroyerUi.GetCurrentSite(this.Page);

                //get xml for the root node and load it into the tree
                string siteXml = site.GetTocXml(site.Id, site.NodeType);
                XmlDocument siteXmlDoc = new XmlDocument();
                siteXmlDoc.LoadXml(siteXml);
                XmlNodeList siteNodes = siteXmlDoc.SelectNodes("/" + DestroyerBpc.XML_ELE_SITE);

                foreach (XmlNode siteNode in siteNodes)
                {
                    tocControl.Nodes.Add(BuildTreeNodeUI(siteNode));
                }

                //if(Session["nodeClickedToScroll"] != null && Session["nodeClickedToScroll"].ToString() != "")
                //{
                //    jsLabel_scroll.Text = "<script>getTocPosition('" + Session["nodeClickedToScroll"].ToString() + "', false)</script>";
                //    jsLabel_scroll.Visible = true;
                //    Session["nodeClickedToScroll"] = null;
                //}
            }
        }

        /// <summary>
        /// This is the last event handler in which we can manipulate the control content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_OnPreRender(object sender, System.EventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(this.Page);

            //get the current doc
            IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);
            IDocumentAnchor docAnchor = DestroyerUi.GetCurrentDocumentAnchor(this.Page);

            //sync to the toc path if there is one specified in the URL or in the session
            string tocSyncPath = DestroyerUi.GetTocSyncPath(this.Page);
            if (tocSyncPath != null && tocSyncPath != DestroyerBpc.EMPTY_STRING && (doc == null || (doc != null && doc.InSubscription)))
            {
                //djf
                SyncTocToPathUI(site, tocSyncPath);
                //clear the autosync flag
                Session.Remove(DestroyerUi.REQPARAM_TOCSYNCPATH);
            }
        }

        /// <summary>
        /// Handles the clicking of a node in the table of contents
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void tocControl_NodeClick(object o, RadTreeNodeEventArgs e)
        {
            //get the current site
            ISite site = null;
            string bookName = null;
            string docName = null;

            //get the clicked node

            RadTreeNode treeNode = e.Node;
            NodeType clickedNodeType = (NodeType)Enum.Parse(typeof(NodeType), GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeTypeDesc));

            //Session variable to store node click to scroll down once the page was rendered.
            Session["nodeClickedToScroll"] = treeNode.ClientID;

            switch (clickedNodeType)
            {
                //if a book was clicked...
                case NodeType.Book:

                    //get the site
                    site = DestroyerUi.GetCurrentSite(this.Page);

                    //get the document from the book, and update the document name in our session to reflect the selected document
                    bookName = GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeName);

                    //redirect through D_Link
                    string bookRedirectUrl = string.Format("D_Link.aspx?{0}={1}", DestroyerUi.REQPARAM_TARGETDOC, bookName);
                    Response.Redirect(bookRedirectUrl, true);

                    break;
                //if a document was clicked...
                case NodeType.Document:

                    //get the site
                    site = DestroyerUi.GetCurrentSite(this.Page);

                    //get the book object that contains the clicked document
                    bookName = GetParentTocNodeValueUI(treeNode, NodeType.Book, DestroyerUi.TocNodeValue.NodeName);

                    string docRedirectUrl = "";

                    //get the document from the book, and update the document name in our session to reflect the selected document
                    docName = GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeName);
                    string[] docSplit = docName.Split(DestroyerBpc.EXTERNAL_DOC_DATA_SEPERATOR);
                    if (docSplit.Length > 1 && docSplit[0].StartsWith(DestroyerBpc.EXTERNAL_DOCUMENT))
                    {
                        //redirect through D_Link
                        docRedirectUrl = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, docSplit[1], DestroyerUi.REQPARAM_TARGETPTR, docSplit[2]);
                    }
                    else
                    {
                        //redirect through D_Link
                        docRedirectUrl = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, bookName, DestroyerUi.REQPARAM_TARGETPTR, docName);
                    }
                    Response.Redirect(docRedirectUrl, true);

                    break;
                //if a document anchor was clicked...
                case NodeType.DocumentAnchor:

                    //get the site
                    site = DestroyerUi.GetCurrentSite(this.Page);

                    //get the book object that contains the clicked document
                    bookName = GetParentTocNodeValueUI(treeNode, NodeType.Book, DestroyerUi.TocNodeValue.NodeName);

                    //get the document anchor name
                    string docAnchorName = GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeName);

                    //redirect through D_Link
                    string docAnchorRedirectUrl = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_TARGETDOC, bookName, DestroyerUi.REQPARAM_TARGETPTR, docAnchorName);
                    Response.Redirect(docAnchorRedirectUrl, true);

                    break;

                //if a site folder was clicked...
                case NodeType.SiteFolder:

                    //get the site
                    site = DestroyerUi.GetCurrentSite(this.Page);

                    //get the site folder id that was clicked
                    int siteFolderId = int.Parse(GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeId));

                    //redirect through D_Link
                    string siteFolderRedirectUrl = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_LINKTYPE, NodeType.SiteFolder, DestroyerUi.REQPARAM_SITEFOLDERID, siteFolderId);
                    Response.Redirect(siteFolderRedirectUrl, true);
                    break;

                //if a book folder was clicked...
                case NodeType.BookFolder:

                    //get the site
                    site = DestroyerUi.GetCurrentSite(this.Page);

                    //get the book folder id that was clicked
                    int bookFolderId = int.Parse(GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeId));

                    //redirect through D_Link
                    string bookFolderRedirectUrl = string.Format("D_Link.aspx?{0}={1}&{2}={3}", DestroyerUi.REQPARAM_LINKTYPE, NodeType.BookFolder, DestroyerUi.REQPARAM_BOOKFOLDERID, bookFolderId);
                    Response.Redirect(bookFolderRedirectUrl, true);
                    break;
            }
        }
        /// <summary>
        /// Handles the expansion of a node in the table of contents
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void tocControl_NodeExpand(object o, RadTreeNodeEventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(this.Page);

            //get the expanding node
            RadTreeNode treeNode = e.Node;

            //set the expanding node to client load so he doesn't have to come back to the server anymore
            treeNode.ExpandMode = TreeNodeExpandMode.ClientSide;

            //clear our nodes and load the tree nodes under us
            treeNode.Nodes.Clear();
            int nodeId = int.Parse(treeNode.Attributes["NODEID"].Split(DestroyerUi.RADTREE_IDSEPCHAR)[1]);
            NodeType nodeType = (NodeType)Enum.Parse(typeof(NodeType), GetTocNodeValueUI(treeNode, DestroyerUi.TocNodeValue.NodeTypeDesc), true);
            string childXml = site.GetTocXml(nodeId, nodeType);
            XmlDocument siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(childXml);
            XmlNodeList nodes = siteXmlDoc.SelectNodes("/*/*");
            foreach (XmlNode node in nodes)
            {
                treeNode.Nodes.Add(BuildTreeNodeUI(node));
            }

            if (treeNode != null && treeNode.ClientID != null)
            {
                jsLabel_scroll.Text = "<script>getTocPosition('" + treeNode.ClientID + "', false)</script>";
                jsLabel_scroll.Visible = true;
            }
        }

        /// <summary>
        /// Handles sync toc click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SyncTocImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(this.Page);

            //get the current doc
            IDocument doc = DestroyerUi.GetCurrentDocument(this.Page);

            //sync the toc to the specified location
            if (doc != null && doc.InSubscription)
            {
                string syncTocPath = DestroyerUi.GetTocPath(doc);
                //SyncTocToPath(site, syncTocPath);
                SyncTocToPathUI(site, syncTocPath);
            }
        }
        #endregion Event Handlers

        #region Helper Methods

        /// <summary>
        /// Synchronizes the treeview to the specified document
        /// </summary>
        /// <param name="syncPath">An id path of the nodes to which you wish to sync the toc</param>
        private void SyncTocToPathUI(ISite site, string syncPath)
        {
            //collapse the treeview and clear all selections
            tocControl.CollapseAllNodes();
            tocControl.ClearSelectedNodes();

            //get the ids of the nodes that we must sync to
            string[] tocSyncNodes = syncPath.Split(DestroyerUi.RADTREE_TOCPATHSEPCHAR);

            //go through each id, expanding if the node is found, loading if it is not found
            for (int i = 0; i < tocSyncNodes.Length; i++)
            {
                string tocSyncNode = tocSyncNodes[i];

                //ignore empty nodes
                if (tocSyncNode != DestroyerBpc.EMPTY_STRING)
                {
                    string[] tocSyncNodeComponents = tocSyncNode.Split(DestroyerUi.RADTREE_VALUESEPCHAR);
                    string tocSyncNodeTypeDesc = tocSyncNodeComponents[0];
                    string tocSyncNodeId = tocSyncNodeComponents[1];
                    string tocSyncRadTreeNodeId = AbbreviateNodeTypeDesc(tocSyncNodeTypeDesc) + DestroyerUi.RADTREE_IDSEPCHAR + tocSyncNodeId;
                    NodeType tocSyncNodeType = (NodeType)Enum.Parse(typeof(NodeType), tocSyncNodeTypeDesc, true);

                    //find the current node in the toc
                    RadTreeNode radTreeSyncNode = tocControl.FindNodeByAttribute("NODEID", tocSyncRadTreeNodeId);

                    //if the node is not in the toc, we need to add it (along with its siblings)
                    if (radTreeSyncNode == null)
                    {
                        //make sure the very first node is not triggering this condition (that would be bad)
                        if (i == 0)
                        {
                            //this condition should not happen unless someone tries to feed in a bogus sync path on the url
                            throw new Exception(string.Format(DestroyerUi.ERROR_TOCSYNCFAILURE, syncPath));
                        }

                        //get the info for the parent tree node to which we are adding a new node
                        string parentTocSyncNode = tocSyncNodes[i - 1];
                        string[] parentTocSyncNodeComponents = parentTocSyncNode.Split(DestroyerUi.RADTREE_VALUESEPCHAR);
                        string parentTocSyncNodeTypeDesc = parentTocSyncNodeComponents[0];
                        string parentTocSyncNodeId = parentTocSyncNodeComponents[1];
                        string parentTocSyncRadTreeNodeId = AbbreviateNodeTypeDesc(parentTocSyncNodeTypeDesc) + DestroyerUi.RADTREE_IDSEPCHAR + parentTocSyncNodeId;
                        NodeType parentTocSyncNodeType = (NodeType)Enum.Parse(typeof(NodeType), parentTocSyncNodeTypeDesc, true);

                        //get the xml describing the children of the parent sync node
                        string parentTocSyncNodeXml = site.GetTocXml(int.Parse(parentTocSyncNodeId), parentTocSyncNodeType);
                        XmlDocument parentTocSyncNodeXmlDoc = new XmlDocument();
                        parentTocSyncNodeXmlDoc.LoadXml(parentTocSyncNodeXml);

                        //find this parent node in the toc
                        RadTreeNode parentRadTreeSyncNode = tocControl.FindNodeByAttribute("NODEID", parentTocSyncRadTreeNodeId);

                        //add all the children to the parent tree node
                        XmlNodeList parentTocSyncNodeChildren = parentTocSyncNodeXmlDoc.SelectNodes("/*/*");
                        foreach (XmlNode parentTocSyncNodeChild in parentTocSyncNodeChildren)
                        {
                            RadTreeNode newTreeNode = BuildTreeNodeUI(parentTocSyncNodeChild);
                            parentRadTreeSyncNode.Nodes.Add(newTreeNode);
                            parentRadTreeSyncNode.ExpandMode = TreeNodeExpandMode.ClientSide;
                            parentRadTreeSyncNode.Expanded = true;
                        }

                        //now find our node, which should now be loaded in the toc
                        radTreeSyncNode = tocControl.FindNodeByAttribute("NODEID", tocSyncRadTreeNodeId);
                    }

                    if (radTreeSyncNode != null)
                    {
                        //set the expand and select states on the nodes as appropriate
                        if (i == tocSyncNodes.Length - 1)
                        {
                            radTreeSyncNode.Selected = true;
                        }
                        else
                        {
                            radTreeSyncNode.Expanded = true;
                        }
                    }
                    else
                    {
                        //this condition should not happen unless:
                        //1) someone tries to feed in a bogus sync path on the url
                        //2) they are accessing a document for which they do not have a subscription, or
                        //3) if the requested document anchor is hidden
                        //in either of these cases, just back up to the last node and select it
                        if (i != 0)
                        {
                            tocSyncNode = tocSyncNodes[i - 1];

                            tocSyncNodeComponents = tocSyncNode.Split(DestroyerUi.RADTREE_VALUESEPCHAR);
                            tocSyncNodeTypeDesc = tocSyncNodeComponents[0];
                            tocSyncNodeId = tocSyncNodeComponents[1];
                            tocSyncRadTreeNodeId = tocSyncNodeTypeDesc + DestroyerUi.RADTREE_IDSEPCHAR + tocSyncNodeId;
                            tocSyncNodeType = (NodeType)Enum.Parse(typeof(NodeType), tocSyncNodeTypeDesc, true);

                            //find the current node in the toc
                            radTreeSyncNode = tocControl.FindNodeByAttribute("NODEID", tocSyncRadTreeNodeId);

                            //select it
                            if (radTreeSyncNode != null)
                            {
                                radTreeSyncNode.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches the ancestors of the specified rad tree node and returns the specified value
        /// </summary>
        /// <param name="treeNode">The context rad tree node</param>
        /// <param name="nodeType">The type of ancestor node you are looking for</param>
        /// <param name="tocNodeValue">The toc node value you are looking for</param>
        /// <returns></returns>
        private string GetParentTocNodeValueUI(RadTreeNode treeNode, NodeType nodeType, DestroyerUi.TocNodeValue tocNodeValue)
        {
            string retVal = DestroyerBpc.EMPTY_STRING;
            RadTreeNode parentTreeNode = treeNode.ParentNode;
            while (parentTreeNode != null)
            {
                if ((NodeType)Enum.Parse(typeof(NodeType), GetTocNodeValueUI(parentTreeNode, DestroyerUi.TocNodeValue.NodeTypeDesc), true) == nodeType)
                {
                    retVal = GetTocNodeValueUI(parentTreeNode, tocNodeValue);
                    break;
                }
                parentTreeNode = parentTreeNode.ParentNode;
            }
            return retVal;
        }
        /// <summary>
        /// Retrieves the specified value from the rad tree node value list
        /// </summary>
        /// <param name="treeNodeValue">The rad tree value list</param>
        /// <param name="tocNodeValue">The named value to pull</param>
        /// <returns></returns>
        private string GetTocNodeValueUI(RadTreeNode treeNode, DestroyerUi.TocNodeValue tocNodeValue)
        {
            //the string to return
            string retVal = string.Empty;

            //get the value and id properties of the node
            string treeNodeValue = treeNode.Value;
            string treeNodeId = treeNode.Attributes["NODEID"];

            //if being asked for the node id
            if (tocNodeValue == DestroyerUi.TocNodeValue.NodeId)
            {
                string[] valueList = treeNodeId.Split(DestroyerUi.RADTREE_IDSEPCHAR);
                retVal = valueList[1];
            }
            else if (tocNodeValue == DestroyerUi.TocNodeValue.NodeName)
            {
                retVal = treeNodeValue;
            }
            else if (tocNodeValue == DestroyerUi.TocNodeValue.NodeTypeDesc)
            {
                string[] valueList = treeNodeId.Split(DestroyerUi.RADTREE_IDSEPCHAR);
                retVal = ExpandNodeTypeDesc(valueList[0]);
            }

            return retVal;
        }
        /// <summary>
        /// Node type descriptions are abbreviated when stored on the radtreeview node as a performance optimization.
        /// This method is used to expand the abbreviations into a full node type description string.
        /// </summary>
        /// <param name="abbrevNodeTypeDesc">The abbreviated node type description string</param>
        /// <returns></returns>
        private string ExpandNodeTypeDesc(string abbrevNodeTypeDesc)
        {
            string retVal = string.Empty;
            switch (abbrevNodeTypeDesc)
            {
                case "B":
                    retVal = NodeType.Book.ToString();
                    break;
                case "BF":
                    retVal = NodeType.BookFolder.ToString();
                    break;
                case "D":
                    retVal = NodeType.Document.ToString();
                    break;
                case "A":
                    retVal = NodeType.DocumentAnchor.ToString();
                    break;
                case "F":
                    retVal = NodeType.Format.ToString();
                    break;
                case "S":
                    retVal = NodeType.Site.ToString();
                    break;
                case "SF":
                    retVal = NodeType.SiteFolder.ToString();
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Node type descriptions are abbreviated when stored on the radtreeview node as a performance optimization.
        /// This method is used to perform the abbreviation from a full node type description string.
        /// </summary>
        /// <param name="fullNodeTypeDesc">The full node type description string</param>
        /// <returns></returns>
        private string AbbreviateNodeTypeDesc(string fullNodeTypeDesc)
        {
            string retVal = string.Empty;
            if (fullNodeTypeDesc == NodeType.Book.ToString())
            {
                retVal = "B";
            }
            else if (fullNodeTypeDesc == NodeType.BookFolder.ToString())
            {
                retVal = "BF";
            }
            else if (fullNodeTypeDesc == NodeType.Document.ToString())
            {
                retVal = "D";
            }
            else if (fullNodeTypeDesc == NodeType.DocumentAnchor.ToString())
            {
                retVal = "A";
            }
            else if (fullNodeTypeDesc == NodeType.Format.ToString())
            {
                retVal = "F";
            }
            else if (fullNodeTypeDesc == NodeType.Site.ToString())
            {
                retVal = "S";
            }
            else if (fullNodeTypeDesc == NodeType.SiteFolder.ToString())
            {
                retVal = "SF";
            }
            return retVal;
        }

        /// <summary>
        /// Builds a red tree node for the table of contents from an xml node.
        /// The rad tree node will contain the following in its properties:
        /// .Text - The Title attribute of the XML node
        /// .ID - The Id attribute of the XML node in the form NodeTypeDescriptionAbbreviation:NodeId
        /// .Value - Node Name
        /// .ToolTip - The Title attribute of the XML node
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private RadTreeNode BuildTreeNodeUI(XmlNode xmlNode)
        {
            RadTreeNode subTreeNode = null;

            string subNodeTypeDesc = xmlNode.LocalName;
            NodeType subNodeType = (NodeType)Enum.Parse(typeof(NodeType), subNodeTypeDesc, true);
            int subNodeId = int.Parse(xmlNode.Attributes[DestroyerBpc.XML_ATT_ID].Value);
            string subNodeName = xmlNode.Attributes[DestroyerBpc.XML_ATT_NAME].Value;
            string subNodeTitle = xmlNode.Attributes[DestroyerBpc.XML_ATT_TITLE].Value;
            string hasChildren = xmlNode.Attributes[DestroyerBpc.XML_ATT_HASCHILDREN].Value;

            subTreeNode = new RadTreeNode();
            subTreeNode.Text = subNodeTitle;
            subTreeNode.Attributes.Add("NODEID", AbbreviateNodeTypeDesc(subNodeTypeDesc) + DestroyerUi.RADTREE_IDSEPCHAR + subNodeId.ToString());
            subTreeNode.Value = subNodeName;
            subTreeNode.ToolTip = subNodeTitle;
            subTreeNode.ImageUrl = "~/images/Square/3DClassic/" + AbbreviateNodeTypeDesc(subNodeType.ToString()) + "_c.gif";
            subTreeNode.ExpandedImageUrl = "~/images/Square/3DClassic/" + AbbreviateNodeTypeDesc(subNodeType.ToString()) + "_o.gif";

            //we only want nodes with children to expand
            if (bool.Parse(hasChildren))
            {
                subTreeNode.ExpandMode = TreeNodeExpandMode.ServerSide;
            }

            //we only want book, document, document anchor, and site folder nodes to post back when clicked
            //...oh yeah, and now book folders, too!
            if (subNodeType != NodeType.Book && subNodeType != NodeType.Document && subNodeType != NodeType.DocumentAnchor && subNodeType != NodeType.SiteFolder && subNodeType != NodeType.BookFolder)
            {
                subTreeNode.PostBack = false;
            }

            return subTreeNode;
        }

        #endregion Helper Methods


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tocControl.NodeExpand += new RadTreeViewEventHandler(this.tocControl_NodeExpand);
            this.tocControl.NodeClick += new RadTreeViewEventHandler(this.tocControl_NodeClick);
            this.PreRender += new System.EventHandler(this.Page_OnPreRender);

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HelpImageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DestroyerUi.ShowHelp(this.Page, DestroyerUi.HelpTopic.Toc);
        }
    }
}
