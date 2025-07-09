using System;
using System.Web.UI;
using AICPA.Destroyer.Content;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.User;
using D_ODPPortalUI.Shared;

namespace D_ODPPortalUI
{
    public partial class Main : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // sburton 2010-04-21: This call to get a user ensures that they are authenticated
            // since we aren't really _using_ the user here, we should probably move the call
            // somewhere else, but may want to still have something like this to check "IfAuthenticated"
            try
            {
                IUser u = DestroyerUi.GetCurrentUser(Page);
                ISite s = DestroyerUi.GetCurrentSite(Page);
            }
            catch (Exception ex)
            {
            }


            //Create breadcrumb copntrol and add to page.
            var ucBreadcrumb = (Breadcrumb) Page.LoadControl("~/Breadcrumb.ascx");
            cphBreadcrumb.Controls.Add(ucBreadcrumb);
            //upBreadcrumb.Update();

            //Create search control and add to page.
            var ucSearch = (Search) Page.LoadControl("~/Search.ascx");
            cphSearch.Controls.Add(ucSearch);
            //upSearch.Update();

            //Create footer control and add to page.
            var ucFooter = (Footer) Page.LoadControl("~/Footer.ascx");
            cphFooter.Controls.Add(ucFooter);
            //upFooter.Update();
        }

        public void GotoNewDocument()
        {
            // TODO: switch this to simply use a new document.
            // right now for testing purposes we're going to load up one
            // that we know exists

            string targetDoc = "ps";
            string targetPointer = "how_professional_standards_is_organized";

            GotoDocument(targetDoc, targetPointer);
        }

        public void GotoDocument(string targetDoc, string targetPointer)
        {
            ISite site = DestroyerUi.GetCurrentSite(Page);

            var link = new ContentLink(site, targetDoc, targetPointer);

            var ucViewDocument = (ViewDocument) Page.LoadControl("~/ViewDocument.ascx");
            ucViewDocument.Document = link.Document;
            //ucViewDocument.FadeIn = true;

            cphMainContainer.Controls.Clear();
            cphMainContainer.Controls.Add(ucViewDocument);

            upMainContainer.Update();
        }

        public void GotoSearchResults()
        {
            var ucSearchReults = (SearchResults) Page.LoadControl("~/SearchResults.ascx");

            cphMainContainer.Controls.Clear();
            cphMainContainer.Controls.Add(ucSearchReults);

            upMainContainer.Update();
        }
    }
}