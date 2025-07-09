using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AICPA.Destroyer.Shared;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;

namespace AICPA.Destroyer.User.Bookmark
{
    public class Bookmark : DestroyerBpc, IBookmark
    {
        private BookmarkDalc _activeBookmarkDalc;

        private BookmarkDalc ActiveBookmarkDalc
        {
            get { return _activeBookmarkDalc ?? (_activeBookmarkDalc = new BookmarkDalc()); }
        }

        #region IBookmark Members
        public Guid UserId
        {
            get;
            private set;
        }

        public int Id
        {
            get;
            private set;
        }

        public string TargetDoc
        {
            get;
            private set;
        }

        public string TargetPtr
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string ResolveReferenceNode(ISite site)
        {
            ContentLink content = new ContentLink(site, TargetDoc, TargetPtr);

            string siteReferencePath = "";

            if (content.DocumentAnchor != null)
            {
                siteReferencePath = content.DocumentAnchor.SiteReferencePath;
            }
            else if (content.Document != null)
            {
                siteReferencePath = content.Document.SiteReferencePath;
            }
            else if (content.Book != null)
            {
                siteReferencePath = content.Book.ReferencePath;
            }

            return siteReferencePath;
        }


        public void Save()
        {
            if (Title.Length > 150)
            {
                Title = Title.Substring(0, 149);
            }
            Id = ActiveBookmarkDalc.SaveBookmark(Id, UserId, TargetDoc, TargetPtr,Title);
        }

        public void Delete()
        {
            ActiveBookmarkDalc.DeleteBookmark(Id);
            Id = 0;
        }
        #endregion IBookmark Members

        #region Constructors
        public Bookmark(string targetDoc, string targetPtr, Guid userId)
        {
            TargetDoc = targetDoc;
            TargetPtr = targetPtr;
            UserId = userId;
        }

        public Bookmark(string targetDoc, string targetPtr, Guid userId, string title) : this(targetDoc,targetPtr,userId)
        {
            Title = title;
        }

        public Bookmark(BookmarkDS.D_BookmarkRow noteRow)
        {
            Id = noteRow.BookmarkID;
            UserId = noteRow.UserID;
            TargetDoc = noteRow.TargetDoc;
            TargetPtr = noteRow.TargetPtr;
            Title = noteRow.Title;
            
        }

        public Bookmark(int id, Guid userId, string targetDoc, string targetPtr, string title)
        {
            Id = id;
            UserId = userId;
            TargetDoc = TargetDoc;
            TargetPtr = targetPtr;
            Title = title;
        }
        #endregion Constructors


        #region Static Methods
        public static void DeleteBookmarkById(int id)
        {
            new BookmarkDalc().DeleteBookmark(id);
        }

        public static IBookmark GetBookmarkById(int noteId)
        {
            var retrievedBookmarkRow = new BookmarkDalc().GetBookmark(noteId);
            IBookmark note;

            note = retrievedBookmarkRow != null ?
                new Bookmark(retrievedBookmarkRow) :
                null;

            return note;
        }

        public static IBookmark GetBookmarkByUserIdAndTargetDocPtr(Guid userId, string targetDoc, string targetPtr)
        {
            var retrievedBookmarkRow = new BookmarkDalc().GetBookmarkForUser(userId, targetDoc, targetPtr);
            IBookmark note;

            note = retrievedBookmarkRow != null ?
                new Bookmark(retrievedBookmarkRow) :
                null;

            return note;
        }

        public static IBookmark GetBookmarkByDocInstanceId(Guid userId, int docInstanceId, string targetDoc)
        {
            var retrievedBookmarkRow = new BookmarkDalc().GetBookmarkByDocInstanceId(userId, docInstanceId, targetDoc);
            IBookmark note;

            note = retrievedBookmarkRow != null ?
                new Bookmark(retrievedBookmarkRow) :
                null;

            return note;
        }

        /// <summary>
        /// Gets the saved searches for user.	
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IEnumerable<IBookmark> GetBookmarksForUser(Guid userId)
        {
            var retrievedBookmarkRows = new BookmarkDalc().GetBookmarksForUser(userId);

            return retrievedBookmarkRows.Select(row => (IBookmark)new Bookmark(row));
        }
        #endregion Static Methods
    }
}
