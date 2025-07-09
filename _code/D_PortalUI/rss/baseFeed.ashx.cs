using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Syndication;
using System.Web.Services;
using System.IO;
using System.Xml;

namespace AICPA.Destroyer.UI.Portal.rss
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class baseFeed : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            SyndicationFeed feed = CreateRecentFeed();
            
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

            Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);
            formatter.WriteTo(xmlTextWriter);

            context.Response.ContentType = "application/rss+xml";
            context.Response.Write(stringWriter.ToString());
            
        }

        private SyndicationFeed CreateRecentFeed()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            items.Add(CreateItem("article-1"));
            items.Add(CreateItem("article-2"));
            items.Add(CreateItem("article-3"));

            SyndicationFeed feed = new SyndicationFeed(items);
            feed.Title = new TextSyndicationContent("AICPA Silverdox Demo Feed");
            feed.Description = new TextSyndicationContent("This is the best RSS feed ever.");
            feed.ImageUrl = new Uri("http://www.knowlysis.com/images/logo2.png");


            return feed;
        }

        private SyndicationItem CreateItem(string id)
        {
            SyndicationItem item = new SyndicationItem(id + " - title", "content here...", new Uri("http://knowlysis.com/" + id + ".html"), id, new DateTimeOffset(DateTime.Now));
            item.Summary = new TextSyndicationContent("summary text here for " + id);

            return item;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
