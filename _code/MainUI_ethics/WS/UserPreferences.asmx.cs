#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Note;
using System.Xml;
using System.ServiceModel.Activation;
using AICPA.Destroyer.User.Bookmark;
using System.Text;
using System.Text.RegularExpressions;
using AICPA.Destroyer.User.Event;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Content.Book;
using MainUI.Shared;
#endregion

namespace MainUI.WS
{
    [WebService(Namespace = "https://publication.cpa2biz.com/MainUI/WS/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ToolboxItem(false)]
    [ScriptService]
    public class UserPreferences : AicpaService
    {
        const int MAX_STRING_LEN = 75;//Max length of bookmark name.

        #region User Preferences
        
        /// <summary>
        ///   Gets the user preference.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "This returns the current user preferences.")]
        public Preferences GetUserPreferences()
        {
            Preferences preferences = new Preferences();
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            IUser user = ContextManager.CurrentUser;
            preferences.UserPreference =
                user.Preferences.Keys.Select(key => new Preference { Key = key, Value = user.Preferences[key] }).ToList();
            preferences.PasswordChanged = PasswordStatus.NoChange;
            return preferences;
        }

        [WebMethod(true, Description = "This returns the current user preferences.")]
        public Preferences ChangeUserPassword()
        {
            Preferences preferences = new Preferences();
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            IUser user = ContextManager.CurrentUser;
            preferences.UserPreference =
                user.Preferences.Keys.Select(key => new Preference { Key = key, Value = user.Preferences[key] }).ToList();
            //Handle Password Change Here
            preferences.PasswordChanged = PasswordStatus.NoChange;
            
            
            return preferences;
        }


        /// <summary>
        /// Adds a user preference.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "This adds a user preference.")]
        public Preferences AddUserPreference(string preferenceKey, string preferenceValue)
        {
            KeyValuePair<string, string> preference = new KeyValuePair<string, string>(preferenceKey, preferenceValue);
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            IUser user = ContextManager.CurrentUser;
            user.Preferences.Add(preferenceKey, preferenceValue);
            Preferences preferences = GetUserPreferences();
            return preferences;
        }

        /// <summary>
        /// Saves a set of user preferences.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Saves a set of user preferences.")]
        public Preferences SaveUserPreferences(string preferenceString)
        {
            CurrentSite.Status = ContextManager.GetSiteStatus(ConfigurationManager.AppSettings["SiteStatus"]);
            IUser user = ContextManager.CurrentUser;

            Array preferences = preferenceString.Split('-');

            foreach (string preference in preferences)
            {
                if (preference.Length > 0)
                {
                    int delimeterIndex = preference.IndexOf('*');
                    user.Preferences[preference.Substring(0, delimeterIndex)] = preference.Substring(delimeterIndex + 1);
                }
            }

            Preferences prefs = GetUserPreferences();
            prefs.PreferencesChanged = true;

            return prefs;
        }
        #endregion

        #region Notes
        
        /// <summary>
        /// Saves a note.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Saves a note.")]
        public Note SaveNote(string targetDoc, string targetPtr, string noteText, string noteTitle)
        {
            IUser user = ContextManager.CurrentUser;

            INote note = new Note(targetDoc, targetPtr, user.UserId, noteText, noteTitle);
            note.Save();

            return (Note)note;
        }

        /// <summary>
        /// Retrieves a note.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Retrieves a note.")]
        public List<INote> GetNotes(string targetDoc, string targetPtr)
        {
            IUser user = ContextManager.CurrentUser;

            List<INote> notes = Note.GetNotesByUserIdAndTargetDocPtr(user.UserId, targetDoc, targetPtr);

            return notes;
        }

        /// <summary>
        /// Updates a note.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Updates a note.")]
        public Note UpdateNote(int id, string noteText, string noteTitle)
        {
            INote note = Note.GetNoteById(id);
            note.Text = noteText;
            note.Title = noteTitle;
            note.Save();

            return (Note)note;
        }

        /// <summary>
        /// Deletes a note.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Deletes a note.")]
        public void DeleteNote(int id)
        {
            Note.DeleteNoteById(id);
            return;
        }

        /// <summary>
        /// Deletes all notes.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Deletes all notes by ID given in comma-separated list.")]
        public void DeleteNotes(string noteIds)
        {
            foreach (string thisId in noteIds.Split(','))
            {
                try
                {
                    int id = Int32.Parse(thisId);
                    Note.DeleteNoteById(id);
                }
                catch (Exception e)
                {
                }
            }
            return;
        }

        /// <summary>
        /// Gets all of the notes for the current user.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Gets all of the notes for the current user.")]
        public List<MyNote> GetAllMyNotes()
        {
            List<MyNote> myNotes = new List<MyNote>();

            IUser user = ContextManager.CurrentUser;

            IEnumerable<INote> retrievedNotes = Note.GetNotesForUser(user.UserId, ContextManager.CurrentSite.Id);

            foreach (INote note in retrievedNotes)
            {
                string siteReferencePath = note.ResolveReferenceNode(ContextManager.CurrentSite);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(siteReferencePath);

                XmlNode node = xmlDoc.SelectSingleNode("//Book");
                string bookName = node.Attributes.GetNamedItem("Title").Value;
                XmlNodeList nodeList = xmlDoc.SelectNodes("/*/*");
                //gets the last node
                string documentAnchorName = nodeList[nodeList.Count - 1].Attributes.GetNamedItem("Title").Value;

                string location = bookName + " - " + documentAnchorName;

                myNotes.Add(
                    new MyNote()
                    {
                        Location = location,
                        Note = (Note)note
                    });
            }

            return myNotes;
        }
        #endregion

        #region Bookmarks

        /// <summary>
        /// Takes a Document ID and Document Type and returns the targetDoc and targetPtr if the NodeType is a "document"
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="type">Document Type</param>
        /// <param name="targetDoc">target Document</param>
        /// <param name="targetPtr">Target Pointer</param>        
        public void GetTargetDocPtrByBookIdDocType(int id, string type, out string targetDoc, out string targetPtr)
        {
            NodeType nodeType = SiteNode.ConvertToNodeType(type);
            switch (nodeType)
            {
                case NodeType.Document:
                    IDocument doc = new Document(id);
                    targetDoc = doc.Book.Name;
                    targetPtr = doc.Name;
                    break;
                default:
                    //Only handle Documents
                    targetDoc = string.Empty;
                    targetPtr = string.Empty;
                    break;
            }
        }

        [WebMethod(true, Description = "Saves a bookmark.")]
        public BookData GetBookDataByBookIdDocType(int id, string type)
        {
            string targetDoc;
            string targetPtr;
            GetTargetDocPtrByBookIdDocType(id, type, out targetDoc, out targetPtr);
            BookData result;
            result.BookId = id;
            result.BookType = type;
            result.TargetDoc = targetDoc;
            result.TargetPtr = targetPtr;
            return result;
        }

        
        /// <summary>
        /// Saves a bookmark.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Saves a bookmark.")]
        public IBookmark SaveBookmark(string targetDoc, string targetPtr, string bookmarkTitle = "")
        {
            if (GetBookmark(targetDoc, targetPtr) != null)
                return GetBookmark(targetDoc, targetPtr);

            IUser user = ContextManager.CurrentUser;
            
            if (bookmarkTitle == "")
                bookmarkTitle = GetDocumentTitle(targetDoc, targetPtr);
            IBookmark bookmark = new Bookmark(targetDoc, targetPtr, user.UserId, bookmarkTitle);
            bookmark.Save();

            return bookmark;
        }

        /// <summary>
        /// Saves a bookmark using the Document ID and Document Type
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Saves a bookmark.")]
        public IBookmark SaveBookmarkByBookIdDocType(int id, string type, string bookmarkTitle = "")
        {
            string targetDoc = string.Empty;
            string targetPtr = string.Empty;

            GetTargetDocPtrByBookIdDocType(id, type, out targetDoc, out targetPtr);
            if ((!targetDoc.Equals(string.Empty)) && (!targetPtr.Equals(string.Empty)))
                return SaveBookmark(targetDoc, targetPtr, bookmarkTitle);
            else return null;
        }

        /// <summary>
        /// Retrieves a bookmark using the Document ID and Document Type
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Retrieves a bookmark.")]
        public IBookmark GetBookmarkByBookIdDocType(int id, string type)
        {
            string targetDoc = string.Empty;
            string targetPtr = string.Empty;

            GetTargetDocPtrByBookIdDocType(id, type, out targetDoc, out targetPtr);

            if ((!targetDoc.Equals(string.Empty)) && (!targetPtr.Equals(string.Empty)))
                return GetBookmark(targetDoc,targetPtr);
            else return null;
        }


        /// <summary>
        /// Retrieves a bookmark.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Retrieves a bookmark.")]
        public IBookmark GetBookmark(string targetDoc, string targetPtr)
        {
            IUser user = ContextManager.CurrentUser;

            IBookmark bookmark = Bookmark.GetBookmarkByUserIdAndTargetDocPtr(user.UserId, targetDoc, targetPtr);

            return bookmark;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod(true, Description = "Updates a bookmark.")]
        public IBookmark GetBookmarkById(int id)
        {
            return Bookmark.GetBookmarkById(id);
        }

        /// <summary>
        /// Updates a bookmark.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Updates a bookmark.")]
        public IBookmark UpdateBookmark(int id, string bookmarkText)
        {
            IBookmark bookmark = Bookmark.GetBookmarkById(id);

            bookmark.Save();

            return bookmark;
        }

        /// <summary>
        /// Delete a bookmark using the Document ID and Document Type. 
        /// Returns the bookmark that was deleted otherwise it returns null.
        /// </summary>
        /// <returns>Bookmark</returns>
        [WebMethod(true, Description = "Updates a bookmark.")]
        public IBookmark DeleteBookmarkByBookIdDocType(int id, string type)
        {

            string targetDoc = string.Empty;
            string targetPtr = string.Empty;

            GetTargetDocPtrByBookIdDocType(id, type, out targetDoc, out targetPtr);

            if ((!targetDoc.Equals(string.Empty)) && (!targetPtr.Equals(string.Empty)))
                return DeleteBookmark(targetDoc, targetPtr);
            else return null;
        }

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Deletes a bookmark.")]
        public IBookmark DeleteBookmarkByID(int id)
        {
            IUser user = ContextManager.CurrentUser;
            IBookmark bookmark = Bookmark.GetBookmarkById(id);
            if (bookmark == null)
                return null;
            bookmark.Delete();

            return bookmark;
        }

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Deletes a bookmark.")]
        public IBookmark DeleteBookmark(string targetDoc, string targetPtr)
        {
            IUser user = ContextManager.CurrentUser;
            IBookmark bookmark = Bookmark.GetBookmarkByUserIdAndTargetDocPtr(user.UserId, targetDoc, targetPtr);
            if (bookmark == null)
                return null;
            bookmark.Delete();
            return bookmark;
        }

        private void LogEvent(string module, string method, string name, string description)
        {
            IEvent logEvent = new Event(EventType.Info, DateTime.Now, 4,
                              module, method, name, description, ContextManager.CurrentUser);
            logEvent.Save(true);
        }
         

        private string GetDocumentTitle(string TargetDoc, string TargetPtr)
        {
            var contentLink = new ContentLink(ContextManager.CurrentSite, TargetDoc, TargetPtr);
            string location = string.Empty;
            if ((contentLink != null) && (contentLink.Document != null))
            {
                string siteReferencePath = contentLink.Document.BookReferencePath;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(siteReferencePath);

                XmlNode node = xmlDoc.SelectSingleNode("//Book");
                string bookName = node.Attributes.GetNamedItem("Title").Value;
                XmlNodeList nodeList = xmlDoc.SelectNodes("/*/*");
                //gets the last node
                string documentAnchorName = nodeList[nodeList.Count - 1].Attributes.GetNamedItem("Title").Value;
                location = bookName + " - " + documentAnchorName;
            }
            else if (contentLink != null)
                location = contentLink.BookName;

            if (location.Length > MAX_STRING_LEN)
            {                
                int separatorIndex = location.Substring(0, MAX_STRING_LEN).LastIndexOfAny(new char[] { ' ', '.', ',', ';' });
                if (separatorIndex > 0)
                    location = location.Substring(0, separatorIndex) + "...";
                else location = location.Substring(0, MAX_STRING_LEN);
            }
            return location;
        }


        /// <summary>
        /// Gets all of the bookmarks for the current user.
        /// </summary>
        /// <returns></returns>
        [WebMethod(true, Description = "Gets all of the bookmarks for the current user.")]
        public List<MyBookmark> GetAllMyBookmarks()
        {
            List<MyBookmark> myBookmarks = new List<MyBookmark>();

            IUser user = ContextManager.CurrentUser;

            IEnumerable<IBookmark> retrievedBookmarks = Bookmark.GetBookmarksForUser(user.UserId);

            foreach (IBookmark bookmark in retrievedBookmarks)
            {
                //LogEvent("UserPreferences.asmx.cs", "GetAllMyBookmarks", "GetBookmarkTitle", "Begin");
                myBookmarks.Add(
                    new MyBookmark()
                    {
                        Location = bookmark.Title,
                        Bookmark = bookmark
                    });
                //LogEvent("UserPreferences.asmx.cs", "GetAllMyBookmarks", "GetBookmarkTitle", "End");
            }

            return myBookmarks;
        }
        #endregion
    }


    [Serializable]
    public struct MyNote
    {
        public Note Note;
        public string Location;
    }

    [Serializable]
    public struct MyBookmark
    {
        public IBookmark Bookmark;
        public string Location;
    }

    [Serializable]
    public struct Preferences
    {
        public List<Preference> UserPreference;
        public bool PreferencesChanged;
        public PasswordStatus PasswordChanged; /*   */
    }

    [Serializable]
    public enum PasswordStatus
    {
        NoChange=0,
        Error,
        InvalidCurrentPassword,
        UnmatchedPassword,
        Changed
    }

    [Serializable]
    public struct Preference
    {
        public string Key;
        public string Value;
    }

    [Serializable]
    public struct BookData
    {
        public int BookId;
        public string BookType;
        public string TargetDoc;
        public string TargetPtr;
    }

}