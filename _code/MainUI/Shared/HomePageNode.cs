using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.Content.Book;
using System.Configuration;
using AICPA.Destroyer.User.Event.EventDSNewTableAdapters;

namespace MainUI.Shared
{
    public class HomePageNode
    {
        #region XML Constants
        private const string XML_DISPLAY_NAME = "displayName";
        private const string XML_TARGET_DOC = "targetDoc";
        private const string XML_TARGET_POINTER = "targetPtr";
        private const string XML_IMAGE_URL = "imageUrl";
        private const string XML_DESCRIPTION = "description";
        private const string XML_TOOL = "tool";
        private const string XML_SITE_FOLDER_NAME = "siteFolderName";
        private const string CONFIG_NEXT_GENERATION_PATH = "eXacctpath";
        public const string CONFIG_COSO_PATH_KEY = "COSOpath";
        #endregion

        public HomePageNode()
        {
            NextGenerationPath = ConfigurationManager.AppSettings[CONFIG_NEXT_GENERATION_PATH];
            CosoPath = ConfigurationManager.AppSettings[CONFIG_COSO_PATH_KEY];

        }

        /// <summary>
        /// Builds a home page node from the given xml.
        /// </summary>
        /// <param name="xmlNode">The xml source for the homepage node</param>
        /// <param name="buildTree">If true, this will build the recursive tree.</param>
        public HomePageNode(XmlNode xmlNode, bool buildTree)
        {
            Type = xmlNode.LocalName;

            DisplayName = GetAttributeValue(xmlNode, XML_DISPLAY_NAME);
            TargetDoc = GetAttributeValue(xmlNode, XML_TARGET_DOC);
            TargetPointer = GetAttributeValue(xmlNode, XML_TARGET_POINTER);
            ImageUrl = GetAttributeValue(xmlNode, XML_IMAGE_URL);
            Description = GetAttributeValue(xmlNode, XML_DESCRIPTION);
            Tool = GetAttributeValue(xmlNode, XML_TOOL);
            SiteFolderName = GetAttributeValue(xmlNode, XML_SITE_FOLDER_NAME).Replace("'", "\\'");
            NextGenerationPath = ConfigurationManager.AppSettings[CONFIG_NEXT_GENERATION_PATH];
            CosoPath = ConfigurationManager.AppSettings[CONFIG_COSO_PATH_KEY];

            if (xmlNode.HasChildNodes)
            {
                Children = new List<HomePageNode>();
                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    Children.Add(new HomePageNode(child, true));
                }
            }
        }

        #region Public Properties
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string TargetDoc { get; set; }
        public string TargetPointer { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Tool { get; set; }
        public string SiteFolderName { get; set; }
        public string NextGenerationPath { get; set; }
        public string CosoPath { get; set; }
        public bool IsFafInMySubscription { get; set; }
        public bool IsWhatsnewInMySubscription { get; set; }
        public bool IsNGToolInMySubscription { get; set; }
        public bool IsPfpInMySubscription { get; set; }
        public bool IsCoso { 
            get {
                if (string.IsNullOrEmpty(Domain)) return false;
                string[] validCosoDomains = ConfigurationManager.AppSettings["CosoDomains"].ToString().Split('~');
                foreach (string validDomain in validCosoDomains)
                {
                    if (Domain.IndexOf(validDomain) >= 0)
                    {
                        return true;
                    }
                }

                    if (Domain.IndexOf("cosocollection") >= 0) {
                        return true;
                    }
                    else if (Domain.IndexOf("coso-comp") >= 0)
                    {
                        return true;
                    }
                    else if (Domain.IndexOf("cosoframework") >= 0)
                    {
                        return true;
                    }
                    return false;
            }  
        }
        public bool DisplayQuickFind {
            get
            {
                if (string.IsNullOrEmpty(Domain))
                    return true;
                else
                    return DomainStringHelpers.AllDomainsHaveQuickFind(Domain);
            }
        }

        public string UserGuid { get; set; }
        public string Domain { get; set; }
        public string ReferringSite { get; set; }

        public List<HomePageNode> Children { get; set; }
        public bool ShowSearch
        {
            get
            {
                if (ConfigurationManager.AppSettings["RemoveSearch"] == "true")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public List<RecentDocument> RecentDocs { get; set; }

        #endregion

        #region Private XML Helper Methods
        // sburton 2010-07-02: I suspect these are written elsewhere, it's just some shorthand

        /// <summary>
        /// Checks to see if an attribute is null or if the value of that attribute is null or empty
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private bool IsAttributeNullOrEmpty(XmlNode node, string attributeName)
        {
            return node.Attributes[attributeName] == null || string.IsNullOrEmpty(node.Attributes[attributeName].Value);
        }

        /// <summary>
        /// Returns the attribute value if present otherwise returns string.Empty
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private string GetAttributeValue(XmlNode node, string attributeName)
        {
            return IsAttributeNullOrEmpty(node, attributeName) ? string.Empty : node.Attributes[attributeName].Value;
        }
        #endregion

        /// <summary>
        /// Goes recursively through the tree and fills in missing target pointers
        /// to be the first document within the book.
        /// </summary>
        /// <param name="site"></param>
        public void FillInTargetPointers(ISite site)
        {
            // check if we have a target doc, but an empty target pointer
            if (!string.IsNullOrEmpty(TargetDoc) && string.IsNullOrEmpty(TargetPointer))
            {
                IBook book = site.Books[TargetDoc];

                // if book is null, it is not in our subscription

                if (book != null)
                {
                    if (book.Documents.Count > 0)
                    {
                        TargetPointer = book.Documents[0].Name;
                    }
                    else
                    {
                        TargetPointer = TargetDoc; 
                    }
                }
            }

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.FillInTargetPointers(site);
                }
            }
        }

        public void PruneChildrenBySubscription(ISite site)
        {
            if (Children != null)
            {
                List<HomePageNode> removeList = new List<HomePageNode>();
                foreach (HomePageNode child in Children)
                {
                    if (!string.IsNullOrEmpty(child.TargetDoc))
                    {
                        if (!child.IsInSubscription(site))
                        {
                            // mark that this child should be removed
                            removeList.Add(child);
                            continue;
                        }
                    }

                    if (child.Children != null && child.Children.Count > 0)
                    {
                        child.PruneChildrenBySubscription(site);

                        // check if the node has any non tool children left
                        if (!child.HasNonToolChildren)
                        {
                            // mark that this child should be removed, because all book children were pruned
                            removeList.Add(child);
                        }
                    }
                } // end of foreach child loop

                foreach (HomePageNode node in removeList)
                {
                    Children.Remove(node);
                }
            }
        } // end of PruneChildrenBySubscription Method

        public void SetIsFafInMySubscription(IUser user)
        {
            string[] authenticatedBookNames = user.UserSecurity.BookName;
            IsFafInMySubscription = false;
            

            // loop through the authenticated book list and see if there is any book that has faf in the title
            // if they have even one book, then we will show faf tools
            foreach (string authenticatedBookName in authenticatedBookNames)
            {
                if (authenticatedBookName.Contains("faf"))
                {
                    IsFafInMySubscription = true;
                    break;
                }
            }
        }
        public void SetIsWhatsnewInMySubscription(IUser user)
        {
            string[] authenticatedBookNames = user.UserSecurity.BookName;
            IsWhatsnewInMySubscription = false;

            // loop through the authenticated book list and see if there is any book that has whatsnew in the title
            // if they have even one book, then we will show whatsnew tool
            foreach (string authenticatedBookName in authenticatedBookNames)
            {
                if (authenticatedBookName.Contains("whatsnew"))
                {
                    IsWhatsnewInMySubscription = true;
                    break;
                }
            }
        }
        public void SetIsNGToolInMySubscription(IUser user)
        {
            string[] authenticatedBookNames = user.UserSecurity.BookName;
            IsNGToolInMySubscription = false;

            // loop through the authenticated book list and see if there is any book that has whatsnew in the title
            // if they have even one book, then we will show whatsnew tool
            foreach (string authenticatedBookName in authenticatedBookNames)
            {
                if (authenticatedBookName.Contains("ng-tool"))
                {
                    IsNGToolInMySubscription = true;
                    break;
                }
            }
        }

        

        public bool IsInSubscription(ISite site)
        {
            bool isInSubscription = true;

            // sburton 2010-07-02: This old way uses the existing ContentLink logic which is the most thorough.
            // It will resolve alternate pointers, and get down to the actual document in question.
            // but it is very slow, so I am commenting it out and instead just checking if the target doc
            // is in our book list.  This should be sufficient (and much faster) for the home page.

            // old extra thorough way:
            //ContentLink link = new ContentLink(site, TargetDoc, TargetPointer, user.ReferringSiteValue);
            //return link.IsInSubscription;

            if (!string.IsNullOrEmpty(TargetDoc))
            {
                // quicker/lighter check
                if ((site.Books[TargetDoc] == null) || (site.Books.BookList.Length == 0))
                {
                    isInSubscription = false;
                }
            }

            return isInSubscription;
        }

        /// <summary>
        /// Returns whether or not the given node is a "tool" as opposed to a link to a book
        /// </summary>
        public bool IsTool
        {
            get
            {
                return !string.IsNullOrEmpty(Tool);
            }
        }

        /// <summary>
        /// Returns true if the node has any children that are not tools (aka books)
        /// </summary>
        public bool HasNonToolChildren
        {
            get
            {
                bool hasNonToolChildren = false;

                if (Children != null && Children.Count > 0)
                {
                    foreach (var child in Children)
                    {
                        if (!child.IsTool || child.HasNonToolChildren)
                        {
                            hasNonToolChildren = true;
                            break;
                        }
                    }
                }

                return hasNonToolChildren;
            }
        }

        public void getRecentDocuments(IUser user, ISite site, int MaxRecentDocs, bool usePrefs)
        {
            this.RecentDocs = new List<RecentDocument>();

            const int MAX_NULL_DOCUMENTS = 0;
            D_EventLogTableAdapter eventLogTa = new D_EventLogTableAdapter();

            int maxRecentDocuments = MaxRecentDocs;
            if (user.Preferences["RecentDocumentsToDisplay"] != null && usePrefs)
                maxRecentDocuments = Int32.Parse(user.Preferences["RecentDocumentsToDisplay"]);

            //It is possible for this to give a timeout error, so we restrict this function to only iterate through 4 times for every 1 document we'd like to place in the history.
            int maxIterations = maxRecentDocuments * 4;

            List<string> recentDocPointers = new List<string>();

            int count = 0;
            int nullCount = 0;
            foreach (var row in eventLogTa.GetRecentDocuments(user.UserId))
            {

                count++;
                if (RecentDocs.Count >= maxRecentDocuments || count > maxIterations)
                    break;
                string logDescription = row.Description;
                string[] docPieces = logDescription.Split(';');

                string bookName = docPieces[0].Split('=')[1];
                string docName = docPieces[1].Split('=')[1];
                

                ContentLink link = null;
                if (user.UserSecurity.BookName.Contains(bookName))
                {
                    link = new ContentLink(site, bookName, docName);
                }
                try
                {
                    if (link != null && link.Document != null && !recentDocPointers.Contains(docName))
                    {
                        recentDocPointers.Add(docName);
                        RecentDocs.Add(new RecentDocument { Title = link.Document.Title, TargetDoc = bookName, TargetPtr = docName });
                    }
                    else if (link == null || link.Document == null)
                    {
                        //null documents are very costly in terms of time spent looking them up.  If we get too many, we'll just return what we've found already.
                        nullCount++;
                        if (nullCount > MAX_NULL_DOCUMENTS)
                        {
                            break;
                        }

                    }

                }
                catch (Exception ex)
                {
                    break;
                }

            }
        }
    }
}