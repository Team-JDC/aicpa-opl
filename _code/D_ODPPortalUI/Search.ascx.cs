using System;
using System.Web.UI;

namespace D_ODPPortalUI
{
    public partial class Search : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ibtResults_Click(object sender, EventArgs e)
        {
            //BasePage page = (BasePage)this.Page;
            //page.GotoSearchResults();

            var master = (Main) Page.Master;
            master.GotoSearchResults();
        }
    }
}