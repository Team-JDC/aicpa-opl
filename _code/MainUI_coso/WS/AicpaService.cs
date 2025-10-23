#region Imported Namespaces

using System.Web.Services;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.User;
using MainUI.Shared;

#endregion

namespace MainUI.WS
{
    public class AicpaService : WebService
    {
        private ContextManager contextManager;

        protected ContextManager ContextManager
        {
            get
            {
                if (contextManager == null)
                {
                    contextManager = new ContextManager(Context);
                }
                return contextManager;
            }
        }

        public bool IsAuthenticated
        {
            get { return ContextManager.IsAuthenticated; }
        }

        public IUser CurrentUser
        {
            get { return ContextManager.CurrentUser; }
        }

        public ISite CurrentSite
        {
            get { return ContextManager.CurrentSite; }
        }
    }
}