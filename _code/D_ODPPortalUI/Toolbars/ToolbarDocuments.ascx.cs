using System;
using System.Web.UI;

namespace D_ODPPortalUI.Toolbars
{
    public partial class ToolbarDocuments : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ibtDocument1_Click(object sender, EventArgs e)
        {
            //BasePage page = (BasePage)this.Page;
            //page.GotoNewDocument();

            var master = (Main) Page.Master;
            master.GotoNewDocument();
        }

        protected void ibtNewDocument_Click(object sender, EventArgs e)
        {
            //BasePage page = (BasePage)this.Page;
            //page.GotoNewDocument();

            var master = (Main) Page.Master;
            master.GotoNewDocument();
        }
    }
}