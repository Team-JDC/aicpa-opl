using System;
using System.Web.Services;
using System.Web.UI;
using AICPA.Destroyer.User;

namespace D_ODPPortalUI
{
    public partial class Content : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Page.Request["targetdoc"]) && !string.IsNullOrEmpty(Page.Request["targetptr"]))
            {
                string targetDoc = Page.Request["targetdoc"];
                string targetPointer = Page.Request["targetptr"];

                var master = (Main) Master;
                master.GotoDocument(targetDoc, targetPointer);

                //AddFadeInScript();
            }
        }

        private void AddFadeInScript()
        {
            string divId = "divWholePage";

            string script = string.Format("$('#{0}').hide().fadeIn('slow');", divId);
            ScriptManager.RegisterStartupScript(this, typeof (Content), "content_ajax_key", script, true);
        }

        [WebMethod(true)]
        public static string GetMyDocuments1()
        {
            string retString = "Id:123,Title:doctitle";

            return retString;
        }

        [WebMethod(true)]
        public static DocumentThumbnail GetMyDocuments2()
        {
            var dt = new DocumentThumbnail();
            dt.Id = 123;
            dt.Title = "doc title";

            return dt;
        }

        [WebMethod(true)]
        public static DocumentThumbnail[] GetMyDocuments3()
        {
            var dtArray = new DocumentThumbnail[3];

            var dt = new DocumentThumbnail();
            dt.Id = 123;
            dt.Title = "doc title";

            dtArray[0] = dt;

            dt = new DocumentThumbnail();
            dt.Id = 456;
            dt.Title = "doc title 2";

            dtArray[1] = dt;

            dt = new DocumentThumbnail();
            dt.Id = 789;
            dt.Title = "doc title 3";

            dtArray[2] = dt;

            return dtArray;
        }
    }

    public class DocumentThumbnail
    {
        public int Id;
        public string Title;
    }
}