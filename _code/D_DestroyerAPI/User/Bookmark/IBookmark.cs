using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AICPA.Destroyer.Content.Site;

namespace AICPA.Destroyer.User.Bookmark
{
    public interface IBookmark
    {
        #region Properties

        Guid UserId { get; }

        int Id { get; }

        string TargetDoc { get; }

        string TargetPtr { get; }

        string Title { get; }
        #endregion Properties

        #region Methods

        void Save();

        void Delete();

        string ResolveReferenceNode(ISite site);

        #endregion Methods
    }
}
