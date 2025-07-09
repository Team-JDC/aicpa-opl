using System;
using System.Web.UI;
using System.Xml;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using D_ODPPortalUI.Shared;

namespace D_ODPPortalUI
{
    public partial class ucTOC : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get xml for the root node and load it into the tree
            string siteXml = site.GetTocXml(site.Id, site.NodeType);
            var siteXmlDoc = new XmlDocument();
            siteXmlDoc.LoadXml(siteXml);
            XmlNodeList siteNodes = siteXmlDoc.SelectNodes("/" + DestroyerBpc.XML_ELE_SITE);

            foreach (XmlNode siteNode in siteNodes)
            {
                //tocControl.Nodes.Add(BuildTreeNodeUI(siteNode));
            }
        }

        private string BuildTOC(XmlNode xmlNode)
        {
            string subTreeNode = null;

            string subNodeTypeDesc = xmlNode.LocalName;
            var subNodeType = (NodeType) Enum.Parse(typeof (NodeType), subNodeTypeDesc, true);
            int subNodeId = int.Parse(xmlNode.Attributes[DestroyerBpc.XML_ATT_ID].Value);
            string subNodeName = xmlNode.Attributes[DestroyerBpc.XML_ATT_NAME].Value;
            string subNodeTitle = xmlNode.Attributes[DestroyerBpc.XML_ATT_TITLE].Value;
            string hasChildren = xmlNode.Attributes[DestroyerBpc.XML_ATT_HASCHILDREN].Value;

            //For reference while while developing.

            //subTreeNode = new RadTreeNode();
            //subTreeNode.Text = subNodeTitle;
            //subTreeNode.Attributes.Add("NODEID", AbbreviateNodeTypeDesc(subNodeTypeDesc) + DestroyerUi.RADTREE_IDSEPCHAR + subNodeId.ToString());
            //subTreeNode.Value = subNodeName;
            //subTreeNode.ToolTip = subNodeTitle;
            //subTreeNode.ImageUrl = "~/images/Square/3DClassic/" + AbbreviateNodeTypeDesc(subNodeType.ToString()) + "_c.gif";
            //subTreeNode.ExpandedImageUrl = "~/images/Square/3DClassic/" + AbbreviateNodeTypeDesc(subNodeType.ToString()) + "_o.gif";


            return subTreeNode;
        }
    }
}