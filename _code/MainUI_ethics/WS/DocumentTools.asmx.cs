#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using MainUI.Shared;

#endregion

namespace MainUI.WS
{
    /// <summary>
    /// Summary description for DocumentTools
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class DocumentTools : AicpaService
    {
        /// <summary>
        ///   Gets the information needed for the drop down combo boxes used in the
        ///   Goto Faf tool.
        /// </summary>
        /// <param name = "topicNum">The topic number (ex. 105)</param>
        /// <param name = "subNum">The sub-topic number (ex. 10)</param>
        /// <returns>GotoInformation</returns>
        [WebMethod(true, Description = "Get the drop down information for the combo boxes.")]
        public GotoInformation GetGotoInformation(string topicNum, string subNum)
        {
            GotoInformation result = new GotoInformation
                                         {
                                             SelectedSubTopicNum = subNum,
                                             SelectedTopicNum = topicNum
                                         };

            List<SubTopic> subTopicsList = new List<SubTopic>();
            List<Section> sectionsList = new List<Section>();

            //************************************************************************
            // Topics
            //************************************************************************
            // Get the unique collection of Codification topics from the database
            SiteDs.Cod_MetaRow[] topics = ContextManager.CurrentSite.GetSJoinTopics();

            List<Topic> topicsList = (from t in topics
                                      select new Topic
                                                 {
                                                     BookTitle = t.BookTitle,
                                                     TopicNum = t.TopicNum,
                                                     TopicTitle = t.TopicTitle
                                                 }).ToList();

            result.Topics = topicsList;

            //************************************************************************
            // Topic Groups
            //************************************************************************
            string curBook = string.Empty;            
            List<TopicGroup> topicGroups = new List<TopicGroup>();
            TopicGroup currentTopicGroup = new TopicGroup { Topics = new List<Topic>() };

            foreach (SiteDs.Cod_MetaRow topic in topics)
            {
                // If changing to different book collection,
                //	add extra ListItem entry in order to group topics together
                if (topic.BookTitle != curBook)
                {
                    //create a new topicGroup and set the name
                    currentTopicGroup = new TopicGroup
                                            {
                                                Name = topic.BookTitle,
                                                Topics = new List<Topic>()
                                            };

                    topicGroups.Add(currentTopicGroup);

                    curBook = topic.BookTitle;
                }

                Topic topicObj = new Topic
                                     {
                                         BookTitle = topic.BookTitle,
                                         TopicNum = topic.TopicNum,
                                         TopicTitle = topic.TopicTitle
                                     };

                currentTopicGroup.Topics.Add(topicObj);
            }

            result.TopicGroups = topicGroups;

            //************************************************************************
            // SubTopics
            //************************************************************************
            if (!String.IsNullOrEmpty(topicNum))
            {
                /* TODO: This should be followed up on to see what needs to be done if anything.
                 * Again, this is the part that I'm not going to implement yet.  
                if (topicNum != string.Empty && int.Parse(topicNum) < 100)
                {
                    gtTopic.SelectedIndex = int.Parse(gtTopic.SelectedValue);
                    topicNum = gtTopic.SelectedValue;
                }
                */

                // Get the unique collection of Codification Subtopics from the database
                SiteDs.Cod_MetaRow[] Subtopics = ContextManager.CurrentSite.GetSubtopicByTopic(topicNum);

                subTopicsList.AddRange(from t in Subtopics
                                       select new SubTopic
                                                  {
                                                      SubtopicTitle = t.SubtopicTitle,
                                                      SubtopicNum = t.SubtopicNum
                                                  });
            }

            result.Subtopics = subTopicsList;

            //************************************************************************
            // Sections
            //************************************************************************
            if (!String.IsNullOrEmpty(topicNum) && !String.IsNullOrEmpty(subNum))
            {
                SiteDs.Cod_MetaRow[] Sections = ContextManager.CurrentSite.GetSectionByTopicSubtopic(topicNum, subNum);

                sectionsList.AddRange(from t in Sections
                                      select new Section
                                                 {
                                                     SectionTitle = t.SectionTitle,
                                                     SectionNum = t.SectionNum
                                                 });
            }

            result.Sections = sectionsList;

            return result;
        }

        /// <summary>
        ///   Gets the results of a join sections query.
        /// </summary>
        /// <param name = "topicNum">The topic number (ex. 105)</param>
        /// <param name = "sectionNum">The section number (ex. 10)</param>
        /// <param name = "includeSubtopics">The boolean of whether to include intersection subtopics</param>
        /// <returns>List[JoinSectionsResult]</returns>
        [WebMethod(true, Description = "Gets the results of the join sections query.")]
        public List<JoinSectionsResult> GetJoinSectionsResults(string topicNum, string sectionNum, bool includeSubtopics)
        {
            List<JoinSectionsResult> results = new List<JoinSectionsResult>();

            if (!String.IsNullOrEmpty(topicNum) && !String.IsNullOrEmpty(sectionNum))
            {
                //Get intersection info
                int isect = includeSubtopics ? 1 : 0;

                SiteDs.Cod_MetaRow[] Results = ContextManager.CurrentSite.GetSJoinDocsByTopicSection(topicNum, sectionNum, isect);

                results.AddRange(from t in Results
                                 select new JoinSectionsResult
                                            {
                                                BookTitle = t.BookTitle,
                                                TopicNum = t.TopicNum,
                                                TopicTitle = t.TopicTitle,
                                                SubtopicNum = t.SubtopicNum,
                                                SubtopicTitle = t.SubtopicTitle,
                                                SectionTitle = t.SectionTitle,
                                                SectionNum = t.SectionNum
                                            });
            }

            return results;
        }

        /// <summary>
        ///   Gets the information needed for the drop down combo boxes used in the
        ///   Join Sections Faf tool.
        /// </summary>
        /// <param name = "topicNum">The topic number (ex. 105)</param>
        /// <param name = "content">The radio button value (ex. SEC)</param>
        /// <param name = "includeSubtopics">The boolean of whether to include intersection subtopics</param>
        /// <returns>JoinSectionsInformation</returns>
        [WebMethod(true, Description = "Gets the drop down information for the combo boxes.")]
        public JoinSectionsInformation GetJoinSectionsInformation(string topicNum, string content, bool includeSubtopics)
        {
            JoinSectionsInformation result = new JoinSectionsInformation
                                                 {
                                                     SelectedTopicNum = topicNum,
                                                     SelectedContent = content,
                                                     IncludeIntersectionSubtopics = includeSubtopics
                                                 };

            List<Section> sectionsList = new List<Section>();

            //************************************************************************
            // Topics
            //************************************************************************
            // Get the unique collection of Codification topics from the database
            SiteDs.Cod_MetaRow[] topics = ContextManager.CurrentSite.GetSJoinTopics();

            List<Topic> topicsList = (from t in topics
                                      select new Topic
                                                 {
                                                     BookTitle = t.BookTitle,
                                                     TopicNum = t.TopicNum,
                                                     TopicTitle = t.TopicTitle
                                                 }).ToList();

            result.Topics = topicsList;

            //************************************************************************
            // Topic Groups
            //************************************************************************
            string curBook = string.Empty;            
            List<TopicGroup> topicGroups = new List<TopicGroup>();
            TopicGroup currentTopicGroup = new TopicGroup { Topics = new List<Topic>() };

            foreach (SiteDs.Cod_MetaRow topic in topics)
            {
                // If changing to different book collection,
                //	add extra ListItem entry in order to group topics together
                if (topic.BookTitle != curBook)
                {
                    //create a new topicGroup and set the name
                    currentTopicGroup = new TopicGroup
                    {
                        Name = topic.BookTitle,
                        Topics = new List<Topic>()
                    };

                    topicGroups.Add(currentTopicGroup);

                    curBook = topic.BookTitle;
                }

                Topic topicObj = new Topic
                {
                    BookTitle = topic.BookTitle,
                    TopicNum = topic.TopicNum,
                    TopicTitle = topic.TopicTitle
                };

                currentTopicGroup.Topics.Add(topicObj);
            }

            result.TopicGroups = topicGroups;

            //************************************************************************
            // Sections
            //************************************************************************
            if (!String.IsNullOrEmpty(topicNum) && !String.IsNullOrEmpty(content))
            {
                //Get intersection info
                int isect = includeSubtopics ? 1 : 0;
                int sec;

                switch (content)
                {
                    case "SEC":
                        sec = 1;
                        break;
                    case "ALL":
                        sec = 2;
                        break;
                    default:
                        sec = 0;
                        break;
                }

                SiteDs.Cod_MetaRow[] Sections = ContextManager.CurrentSite.GetSJoinSectionsByTopic(topicNum, isect, sec);

                sectionsList.AddRange(from t in Sections
                                      select new Section
                                                 {
                                                     SectionTitle = t.SectionTitle,
                                                     SectionNum = t.SectionNum
                                                 });
            }

            result.Sections = sectionsList;

            return result;
        }

        [WebMethod(true, Description = "Check to see what tools are available for book of the current SiteNode.")]
        public BookTools GetBookToolsByDocAndPtr(string targetDoc, string targetPtr)
        {
            Content contentService = new Content();
            AICPA.Destroyer.Content.ContentLink link = new AICPA.Destroyer.Content.ContentLink(CurrentSite, targetDoc, targetPtr);
            if (link.IsInSubscription)
            {
                SiteNode sn = contentService.ResolveContentLink(targetDoc, targetPtr);
                return GetBookTools(sn.Id, sn.Type);
            }
            else
            {
                BookTools bookTools = new BookTools();
                if (link.Document != null)
                {
                    bookTools.Id = link.Document.Id;                    
                    bookTools.Type = link.Document.NodeType.ToString();
                }
                else
                {
                    bookTools.Id = -1;
                    bookTools.Type = string.Empty;
                }
                bookTools.ArchivedBook = false;
                bookTools.ReferencingLinks = false;
                bookTools.CrossReference = false;
                bookTools.JoinSections = false;
                bookTools.JoinTopics = false;
                bookTools.Inactive = false;
                bookTools.ViewSources = false;                
                return bookTools;
            }            
        }

        /// <summary>
        ///   Gets the book tools.
        /// </summary>
        /// <param name = "id">The id of the SiteNode.</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true, Description = "Check to see what tools are available for book of the current SiteNode.")]
        public BookTools GetBookTools(int id, string type)
        {
            BookTools bookTools = new BookTools();
            string currentName;
            bookTools.Id = id;
            bookTools.Type = type;

            bookTools.HasArchivedContent = false;
            bookTools.HasWhatLinksHereContent = false;
            bookTools.ShowFafTools = false;
            bookTools.IsFafInMySubscription = false;

            //IBookCollection books = CurrentSite.Books;
            string[] authenticatedBookNames = CurrentUser.UserSecurity.BookName;

            // loop through the authenticated book list and see if there is any book that has faf in the title
            // if they have even one book, then we will show faf tools
            foreach (string authenticatedBookName in authenticatedBookNames)
            {
                if (authenticatedBookName.Contains("faf"))
                {
                    bookTools.IsFafInMySubscription = true;
                    break;
                }
            }

            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            switch (nodeType)
            {
                case NodeType.Site:
                    {
                        if (bookTools.IsFafInMySubscription)
                        {
                            bookTools.ArchivedBook = false;
                            bookTools.ReferencingLinks = false;
                            bookTools.CrossReference = true; // true
                            bookTools.JoinSections = false;
                            bookTools.JoinTopics = false;
                            bookTools.Inactive = false;
                            bookTools.ViewSources = false;
                        }
                        else
                        {
                            bookTools.ArchivedBook = false;
                            bookTools.ReferencingLinks = false;
                            bookTools.CrossReference = false;
                            bookTools.JoinSections = false;
                            bookTools.JoinTopics = false;
                            bookTools.Inactive = false;
                            bookTools.ViewSources = false;
                        }

                        return bookTools;
                    }
                
                case NodeType.SiteFolder:
                    {
                        ISiteFolder siteFolder = new SiteFolder(ContextManager.CurrentSite, id);
                        currentName = siteFolder.Name;

                        //if (siteFolder.Name == "FASB Accounting Standards Codification")
                        if (siteFolder.Name.Contains("FASB"))
                        {
                            bookTools.ShowFafTools = true;
                        }
                    }

                    break;

                case NodeType.Book:
                    {
                        IBook book = new Book(ContextManager.CurrentSite, id);
                        currentName = book.Name;
                        bookTools.TargetDoc = book.Name;
                        bookTools.TargetPtr = book.Name;
                        string referencePath = book.ReferencePath;

                        XmlDocument siteXmlDoc = new XmlDocument();
                        siteXmlDoc.LoadXml(referencePath);

                        XmlNode node = siteXmlDoc.SelectSingleNode("//SiteFolder");
                        //if (node.Attributes.GetNamedItem("Name").Value == "FASB Accounting Standards Codification")
                        if (node.Attributes.GetNamedItem("Name").Value.Contains("FASB"))
                        {
                            bookTools.ShowFafTools = true;
                        }
                    }

                    break;

                case NodeType.Document:
                    {
                        IDocument document = new Document(id);
                        bookTools.TargetDoc = document.Book.Name;

                        bookTools.TargetPtr = document.Name;
                        currentName = document.Book.Name;

                        IBook book = new Book(ContextManager.CurrentSite, document.Book.Id);
                        document = new Document(book, id);

                        bookTools.HasArchivedContent = document.Formats[ContentType.TextArch] != null;
                        bookTools.HasWhatLinksHereContent = document.Formats[ContentType.TextWlh] != null;

                        string referencePath = document.SiteReferencePath;

                        XmlDocument siteXmlDoc = new XmlDocument();
                        siteXmlDoc.LoadXml(referencePath);

                        XmlNode node = siteXmlDoc.SelectSingleNode("//SiteFolder");
                        //if (node.Attributes.GetNamedItem("Name").Value == "FASB Accounting Standards Codification")
                        if (node!=null&& node.Attributes.GetNamedItem("Name").Value.Contains("FASB"))
                        {
                            bookTools.ShowFafTools = true;
                        }
                    }

                    break;

                default:
                    throw new ArgumentException(string.Format("Unexpected type '{0}' for Get Book Tools.", type));
            }

            // TODO: push the handling of the web config into the context manager and use constants
            string archivedBooks = ConfigurationManager.AppSettings["Site_Archive_Book_Id_List"];
            string referenceingLinksBooks = ConfigurationManager.AppSettings["Site_Referencing_Links_Book_Id_List"];
            string crossReference = ConfigurationManager.AppSettings["Site_Xref_Book_Id_List"];
            string joinSections = ConfigurationManager.AppSettings["Site_JSection_Book_Id_List"];
            string joinTopics = ConfigurationManager.AppSettings["Site_Join_Topics_list"];
            string inactiveBooks = ConfigurationManager.AppSettings["InactiveAAGs"];
            string viewSourcesBooks = ConfigurationManager.AppSettings["Site_ViewSources_Book_Id_List"];

            bookTools.ArchivedBook = archivedBooks.Contains(currentName);
            bookTools.ReferencingLinks = referenceingLinksBooks.Contains(currentName);
            bookTools.CrossReference = crossReference.Contains(currentName);
            bookTools.JoinSections = joinSections.Contains(currentName);
            bookTools.JoinTopics = joinTopics.Contains(currentName);
            bookTools.Inactive = inactiveBooks.Contains(currentName);
            bookTools.ViewSources = viewSourcesBooks.Contains(currentName);

            return bookTools;
        }

        /// <summary>
        ///   Gets the book tools.
        /// </summary>
        /// <param name = "standard">The standard.</param>
        /// <param name = "topic">The topic.</param>
        /// <param name = "subTopic">The sub topic.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "Get standards for cross references.")]
        public StandardsResponse GetStandardsForCrossReference(string standard, string topic,
                                                               string subTopic)
        {
            StandardsResponse standardsResponse = new StandardsResponse
                                                      {
                                                          SelectedTopic = topic,
                                                          SelectedStandard = standard,
                                                          SelectedSubTopic = subTopic,
                                                          Standards = CurrentSite.GetXRefStandardTypes().ToList(),
                                                          CrossReferenceTopics = CurrentSite.GetXRefTopics().ToList(),
                                                          StandardNumbers = CurrentSite.GetXRefStandardNumbersForStandardType(standard).ToList()
                                                      };

            List<string> subTopics = new List<string>();
            List<string> sections = new List<string>();

            if (topic != "")
            {
                // Query the database for the Standard Numbers of the newly selected Standard Type
                string[] SubTopics = CurrentSite.GetXRefSubtopicsByTopic(topic);

                if (subTopic == "")
                {
                    subTopic = SubTopics[0];
                }

                subTopics.AddRange(SubTopics);

                string[] Sections = CurrentSite.GetXRefSectionsByTopicSubtopic(topic, subTopic);

                sections.AddRange(Sections);
            }

            standardsResponse.SubTopics = subTopics;
            standardsResponse.Sections = sections;

            return standardsResponse;
        }

        /// <summary>
        /// Gets the cross reference results.
        /// </summary>
        /// <param name="standard">The standard.</param>
        /// <param name="number">The number.</param>
        /// <param name="topic">The topic.</param>
        /// <param name="subTopic">The sub topic.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        [WebMethod(true, Description = "Get cross reference results.")]
        public CrossReferenceResults GetCrossReferenceResults(string standard, string number, string topic,
                                                              string subTopic, string section)
        {
            //Create a result set to return.
            CrossReferenceResults standardsResult = new CrossReferenceResults();

            //Check to see if the incoming values are empty, if they are return an empty result set.
            if (((standard == string.Empty) || (number == string.Empty)) &&
                ((topic == string.Empty) || (subTopic == string.Empty) || (section == string.Empty)))
            {
                //Do nothing, return an empty result set.
                return standardsResult;
            }

            SiteDs.XRefRow[] searchResults;

            //If the standard is not empty. 
            if (standard != string.Empty)
            {
                searchResults = CurrentSite.GetXRefCodByStandard(standard, number);
            }
            else
            {
                searchResults = CurrentSite.GetXRefStandardByCod(topic, subTopic, section);
            }

            standardsResult.CrossReferenceResult = (from searchRow in searchResults
                                                    select new CrossReference
                                                    {
                                                        Standard = searchRow.StandardType,
                                                        StandardNumber = searchRow.StandardID,
                                                        TopicName = searchRow.Topic,
                                                        SubTopicName = searchRow.Subtopic,
                                                        SectionName = searchRow.Section,
                                                        Paragraph = searchRow.Para_Label,
                                                        LinkText =
                                                            (String.IsNullOrEmpty(searchRow.CodPara)
                                                                 ? string.Empty
                                                                 : searchRow.CodPara) +
                                                            (String.IsNullOrEmpty(searchRow.term)
                                                                 ? string.Empty
                                                                 : searchRow.term),
                                                        TargetDocument =
                                                            searchRow.CodLink.Substring(
                                                                (searchRow.CodLink.IndexOf("=") + 1),
                                                                (searchRow.CodLink.IndexOf("&") -
                                                                 searchRow.CodLink.IndexOf("=")) - 1),
                                                        TargetPointer = searchRow.CodLink.Substring(
                                                            (searchRow.CodLink.LastIndexOf("=") + 1),
                                                            (searchRow.CodLink.Length -
                                                             searchRow.CodLink.LastIndexOf("=")) - 1)
                                                    }).ToList();

            // Return the results.
            return standardsResult;
        }

        // LA means License Agreement, but I'm trying to obfuscate this a bit by not broadcasting it in the Description or Web Method name
        [WebMethod(true, Description = "Update user's LA status")]
        public void UpdateLAStatus(bool newVal)
        {
            CurrentUser.SetLicenseAgreementValue(newVal
                                                     ? LicenseAgreementStatus.Agreed
                                                     : LicenseAgreementStatus.Declined);
        }

        /// <summary>
        ///   Gets the book tools.
        /// </summary>
        /// <param name = "id">The id of the SiteNode.</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [WebMethod(true, Description = "Get Hit Detail for ")]
        public HitResults GetHitDetail(int id, string type, string keywords, int searchMode)
        {
            keywords = keywords.Replace(",,", "~,"); 
            string[] terms = keywords.Split(new char[] {','});            

            HitResults hitResults;
            hitResults.HitResultList = new List<HitResult>();

            NodeType nodeType = SiteNode.ConvertToNodeType(type);

            switch (nodeType)
            {
                case NodeType.Site:
                    {
                        // ?
                        break;
                    }

                case NodeType.SiteFolder:
                    {
                        // ?
                    }

                    break;

                case NodeType.Book:
                    {
                        // ?? Not sure what to do
                        
                    }

                    break;

                case NodeType.Document:
                    {
                        IDocument document = new Document(id);
                        IDocumentFormat format = document.PrimaryFormat;


                        Dictionary<string, int> results = format.GetDocumentHits(terms);
                        

                        foreach (KeyValuePair<string, int> kvp in results)
                        {
                            string[] labels = kvp.Key.Split(new char[] { '|' }); // 0 = TargetPtr, 1 = Position, 2 = Label
                            hitResults.HitResultList.Add(new HitResult() { Title = document.Title, Label = labels[2], Count = kvp.Value, TargetDoc = document.Book.Name, TargetPtr = labels[0], Id = document.Id,  Type = document.NodeType.ToString() });                            
                        }
                    }

                    break;

                default:
                    throw new ArgumentException(string.Format("Unexpected type '{0}' for Get Book Tools.", type));
            }

            return hitResults;

        }


        #region Nested type: HitResult
        [Serializable]
        public struct HitResult
        {
            public string Title;
            public string Label;
            public int Count;
            public string TargetDoc;
            public string TargetPtr;
            public int Id;
            public string Type;
        }

        [Serializable]
        public struct HitResults
        {
            public List<HitResult> HitResultList;
        }

        #endregion

        #region Nested type: BookTools

        [Serializable]
        public struct BookTools
        {
            public int Id;
            public string Type;
            public string TargetDoc;
            public string TargetPtr;
            public bool ArchivedBook;
            public bool CrossReference;
            public bool Inactive;
            public bool JoinSections;
            public bool JoinTopics;
            public bool ReferencingLinks;
            public bool ViewSources;
            public bool HasArchivedContent;
            public bool HasWhatLinksHereContent;
            public bool ShowFafTools;
            public bool IsFafInMySubscription;
        }

        #endregion

        #region Nested type: CrossReference

        [Serializable]
        public struct CrossReference
        {
            public string Paragraph;
            public string SectionName;
            public string Standard;
            public string StandardNumber;
            public string SubTopicName;
            public string TargetDocument;
            public string TargetPointer;
            public string TopicName;
            public string LinkText;
        }

        #endregion

        #region Nested type: CrossReferenceResults

        [Serializable]
        public struct CrossReferenceResults
        {
            public List<CrossReference> CrossReferenceResult;
        }

        #endregion

        #region Nested type: GotoInformation

        [Serializable]
        public struct GotoInformation
        {
            public List<Section> Sections;
            public string SelectedSubTopicNum;
            public string SelectedTopicNum;
            public List<SubTopic> Subtopics;
            public List<Topic> Topics;
            public List<TopicGroup> TopicGroups;
        }

        #endregion

        #region Nested type: JoinSectionsInformation

        [Serializable]
        public struct JoinSectionsInformation
        {
            public bool IncludeIntersectionSubtopics;
            public List<Section> Sections;
            public List<TopicGroup> TopicGroups;
            public string SelectedContent;
            public string SelectedTopicNum;
            public List<Topic> Topics;
        }

        #endregion

        #region Nested type: JoinSectionsResult

        [Serializable]
        public struct JoinSectionsResult
        {
            public string BookTitle;
            public string TopicNum;
            public string TopicTitle;
            public string SubtopicNum;
            public string SubtopicTitle;
            public string SectionNum;
            public string SectionTitle;
        }

        #endregion

        #region Nested type: TopicGroup

        [Serializable]
        public struct TopicGroup
        {
            public string Name;
            public List<Topic> Topics;
        }

        #endregion

        #region Nested type: Section

        [Serializable]
        public struct Section
        {
            public string SectionNum;
            public string SectionTitle;
        }

        #endregion

        #region Nested type: StandardsResponse

        [Serializable]
        public struct StandardsResponse
        {
            public List<string> CrossReferenceTopics;
            public List<string> Sections;
            public string SelectedStandard;
            public string SelectedSubTopic;
            public string SelectedTopic;
            public List<string> StandardNumbers;
            public List<string> Standards;
            public List<string> SubTopics;
        }

        #endregion

        #region Nested type: SubTopic

        [Serializable]
        public struct SubTopic
        {
            public string SubtopicNum;
            public string SubtopicTitle;
        }

        #endregion

        #region Nested type: Topic

        [Serializable]
        public struct Topic
        {
            public string BookTitle;
            public string TopicNum;
            public string TopicTitle;
        }

        #endregion
    }
}