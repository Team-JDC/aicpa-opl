using System;
using System.Web.UI;

namespace D_ODPPortalUI
{
    public partial class SearchResults : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lbtResult1_Click(object sender, EventArgs e)
        {
            //BasePage basePage = (BasePage)this.Page;
            //basePage.GotoNewDocument();

            var master = (Main) Page.Master;
            master.GotoNewDocument();
        }
    }
}