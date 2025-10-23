using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml;

using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User.Event;
using MainUI.Shared;
using System.Web;

namespace MainUI.WS
{
    /// <summary>
    /// Summary description for MyDocuments
    /// </summary>
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Toolbars : AicpaService
    {
        [WebMethod(true)]
        public List<SiteNode> GetMyDocumentsList()
        {
            return SiteNode.ConvertList(CurrentMyDocumentsList);
        }

        private List<IDocument> CurrentMyDocumentsList
        {
            get
            {
                return ContextManager.MyDocuments;
            }
        }

        /// <summary>
        /// Sets MyDocuments[index] to be the document denoted by the targetdoc and target pointer
        /// </summary>
        /// <param name="index"></param>
        /// <param name="targetDoc"></param>
        /// <param name="targetPointer"></param>
        /// <returns>the documentid of the concerned document</returns>
        [WebMethod(true)]
        public SiteNode SetMyDocumentsContent(int index, string targetDoc, string targetPointer)
        {
            ContentLink link = new ContentLink(CurrentSite, targetDoc, targetPointer);
            return SetMyDocumentsContent(index, link.Document);
        }

        [WebMethod(true)]
        public SiteNode SetMyDocumentsContentById(int index, int id, string type)
        {
            var container = PrimaryContentContainer.ConstructContainer(CurrentSite, id, type);
            ContentWrapper wrapper = container.PrimaryContent;

            if (wrapper.HasDocument)
            {
                return SetMyDocumentsContent(index, wrapper.Document);
            }
            
            if (wrapper.HasUri)
            {
                NodeType nodeType = SiteNode.ConvertToNodeType(type);
                if (nodeType == NodeType.SiteFolder)
                {
                    ISiteFolder siteFolder = new SiteFolder(CurrentSite, id);

                    // Forget calling SetMyDocumentsContent because this is going to be refactored down to the UI anyway!
                    return new SiteNode
                               {
                                   Id = id,
                                   Name = siteFolder.Name,
                                   Title = siteFolder.Title,
                                   Type = type
                               };
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
        }

        private SiteNode SetMyDocumentsContent(int index, IDocument document)
        {
            if (index > CurrentMyDocumentsList.Count || index < 0)
            {
                throw new IndexOutOfRangeException(string.Format("Could not set list to index {0}", index));
            }
            
            if (index == CurrentMyDocumentsList.Count)
            {
                // we are adding to the end of the list
                CurrentMyDocumentsList.Add(document);
            }
            else
            {
                // putting in place of the current item at index
                CurrentMyDocumentsList[index] = document;
            }

            return new SiteNode(document);
        }

        [WebMethod(true)]
        public void Logout()
        {
            try
            {
                IEvent logEvent = new Event(EventType.Usage,
                    DateTime.Now,
                    3,
                    "Toolbars.Asmx.cs",
                    "Logout",
                    "UserLogout",
                    "User Logged out of System",
                    ContextManager.CurrentUser);
                logEvent.Save(false);
            } catch {
            ///
            }
            ContextManager.RemoveCookie(ContextManager.COOKIE_ETHICS);                
            CurrentUser.LogOff();
            ContextManager.ClearSession();
        }

        [WebMethod(true)]
        public void KeepSessionAlive()
        {
            ContextManager.KeepSessionAlive();
        }

        [WebMethod(true)]
        public void ThrowWebServiceError()
        {
            throw new Exception("A general web service error");
        }

        [WebMethod(true)]
        public void LogError(string logString)
        {
            try
            {
                // if try fails, log the error
                if (Event.IsEventToBeLogged(EventType.Error, DestroyerBpc.ERROR_SEVERITY_AJAX_FAILED, DestroyerBpc.MODULE_WEBSERVICES,
                                                  "LogError"))
                {
                    IEvent logEvent;

                    try
                    {
                        logEvent = new Event(EventType.Error, DateTime.Now, DestroyerBpc.ERROR_SEVERITY_AJAX_FAILED,
                                                      DestroyerBpc.MODULE_WEBSERVICES, "LogError",
                                                      "AJAX Web Service Exception", logString, ContextManager.CurrentUser);
                        logEvent.Save(false);
                    }
                    catch
                    {
                        logEvent = new Event(EventType.Error, DateTime.Now,
                                                    DestroyerBpc.ERROR_SEVERITY_AJAX_FAILED,
                                                    DestroyerBpc.MODULE_WEBSERVICES, "LogError",
                                                    "AJAX Web Service Exception", logString);

                        logEvent.Save(false);
                    }
                }
            }
            catch
            {}
        }
    

        [WebMethod(true)]
        public ForumData GetForumData()
        {
            ForumData data = new ForumData();
            
            data.PageUrl = ContextManager.ForumUrl;
            data.UserGuid = CurrentUser.UserId.ToString();

            return data;
        }
    }
}
