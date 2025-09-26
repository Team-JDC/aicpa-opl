using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic.Util
{
    public class IndexerHelper
    {
        public async Task<string> GenerateDocIdFromPath(string path)
        {
           // var indexDoc = await BuildDocumentAsync(path);//_indexerHelper.ParseHtml(filePath); 

            var html = await File.ReadAllTextAsync(path);
            var hdoc = new HtmlDocument();
            hdoc.LoadHtml(html);

            var head = hdoc.DocumentNode.SelectSingleNode("//head");
            if (head == null) return null;

            var metas = head.SelectNodes(".//meta") ?? new HtmlNodeCollection(null);

            string GetMeta(string name) =>
                metas.FirstOrDefault(m => m.GetAttributeValue("name", "") == name)
                     ?.GetAttributeValue("content", "");

            // Required ids
            var bookId = GetMeta("destroyer_book_id");
            var docId = GetMeta("destroyer_document_id");

            var Id = $"{bookId}_{docId}";
         
            return Id;
        }
        public ElasticDocument ParseHtml(string filePath)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
            var bodyText = htmlDoc.DocumentNode.SelectSingleNode("//body")?.InnerText ?? "";

            string GetMeta(string name) =>
                metaTags?.FirstOrDefault(m => m.GetAttributeValue("name", "") == name)
                         ?.GetAttributeValue("content", "") ?? "";

            var subs = metaTags?
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_subscription_code")
                .Select(m => m.GetAttributeValue("content", ""))
                .Distinct()
                .ToArray();

            var hierarchy = metaTags?
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_site_hierarchy")
                .Select(m =>
                {
                    var parts = m.GetAttributeValue("content", "").Split(':');
                    return new SiteHierarchyNode { Type = parts[0], Id = parts[1] };
                })
                .ToList() ?? new List<SiteHierarchyNode>();

            var chain = hierarchy.Select(h => (h.Type, h.Id)).ToList();

            var names = new SqlDestroyerService();

            // Try to get HTML book/document titles (optional but nicer)
            var htmlBookTitle = htmlDoc.DocumentNode.SelectSingleNode("//head/link[@rel='home']")
                                 ?.GetAttributeValue("title", null);
            var htmlDocTitle = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();

          

            var doc = new ElasticDocument
            {
                Id = int.Parse(GetMeta("destroyer_document_id")),
                Name = GetMeta("destroyer_document_name"),
                Title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "Untitled",
                Content = bodyText,
                //ReferencePath = GenerateReferencePath(hierarchy),
                SubscriptionCodes = subs,
                BookId = GetMeta("destroyer_book_id"),
                BookName = GetMeta("destroyer_book_name"),
                SiteHierarchy = hierarchy,
                InSubscription = true, // adjust as needed
                //DimensionXml = BuildDimensionXml(hierarchy, subs),
               // SitePath = BuildReferencePathXml(hierarchy)
            };
            doc.ReferencePath = PathBuilders.BuildReferencePathFriendly(chain, names, htmlBookTitle, htmlDocTitle);
            doc.SitePath = PathBuilders.BuildSitePathXmlFriendly(chain, names, htmlBookTitle, htmlDocTitle);

            return doc;
        }

        #region Private Methods

        private static string GenerateReferencePath(List<SiteHierarchyNode> hierarchy)
        {
            return string.Join(" > ", hierarchy.Select(h => $"{h.Type}:{h.Id}"));
        }

        private static string BuildReferencePathXml(List<SiteHierarchyNode> hierarchy)
        {
            var sb = new StringBuilder();
            sb.Append("<ReferencePath>");
            foreach (var node in hierarchy)
            {
                sb.AppendFormat(@"<Site{0} Id=""{1}"" Name=""{0}_{1}"" Title=""{0} {1}"" />", node.Type, node.Id);
            }
            sb.Append("</ReferencePath>");
            return sb.ToString();
        }

        private static string BuildDimensionXml(List<SiteHierarchyNode> hierarchy, string[] subscriptionCodes)
        {
            var sb = new StringBuilder();
            sb.Append("<Dimensions><SelectedDimensions>");

            var selected = hierarchy.LastOrDefault(h => h.Type == "Site") ?? hierarchy.FirstOrDefault();
            if (selected != null)
            {
                sb.AppendFormat("<Dimension><Name>{0}</Name><Id>{1}</Id><DimensionAncestors /><DimensionCompletePath>", selected.Type, selected.Id);
                foreach (var h in hierarchy)
                {
                    sb.AppendFormat("<Dimension><Name>{0}</Name><Id>{1}</Id></Dimension>", h.Type, h.Id);
                }
                sb.Append("</DimensionCompletePath></Dimension>");
            }

            sb.Append("</SelectedDimensions><RefinementDimensions>");

            foreach (var code in subscriptionCodes.Distinct())
            {
                sb.AppendFormat(@"<Dimension>
                <Name>destroyer_site_hierarchy</Name>
                <Id>{0}</Id>
                <DimensionValue>
                    <Name>{0}</Name>
                    <Id>{0}</Id>
                    <RecordCount>1</RecordCount>
                </DimensionValue>
                <DimensionAncestors />
                <DimensionCompletePath>
                    <Dimension><Name>destroyer_site_hierarchy</Name><Id>{0}</Id></Dimension>
                </DimensionCompletePath>
            </Dimension>", code);
            }

            sb.Append("</RefinementDimensions></Dimensions>");
            return sb.ToString();
        }
        #endregion


        public   async Task<ElasticDocument> BuildDocumentAsync(string file)
        {
            var html = await File.ReadAllTextAsync(file);
            var hdoc = new HtmlDocument();
            hdoc.LoadHtml(html);

            var head = hdoc.DocumentNode.SelectSingleNode("//head");
            if (head == null) return null;

            var metas = head.SelectNodes(".//meta") ?? new HtmlNodeCollection(null);

            string GetMeta(string name) =>
                metas.FirstOrDefault(m => m.GetAttributeValue("name", "") == name)
                     ?.GetAttributeValue("content", "");

            // Required ids
            var bookId = GetMeta("destroyer_book_id");
            var docId = GetMeta("destroyer_document_id");

            if (string.IsNullOrWhiteSpace(bookId) || string.IsNullOrWhiteSpace(docId))
                return null;

            var title = hdoc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim() ?? "";

            // Subscription codes (distinct)
            var subs = metas
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_subscription_code")
                .Select(m => m.GetAttributeValue("content", ""))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Site hierarchy from meta destroyer_site_hierarchy
            var siteTokens = metas
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_site_hierarchy")
                .Select(m => m.GetAttributeValue("content", ""))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var hierarchy = new List<SiteHierarchyNode>();
            foreach (var tok in siteTokens)
            {
                var parts = tok.Split(':');
                if (parts.Length == 2)
                {
                    var type = parts[0].Trim();
                    var id = parts[1].Trim();
                    hierarchy.Add(new SiteHierarchyNode { Type = type, Id = id });
                }
            }
            var chain = hierarchy.Select(h => (h.Type, h.Id)).ToList();

            var names = new SqlDestroyerService();

            // Try to get HTML book/document titles (optional but nicer)
            var htmlBookTitle = hdoc.DocumentNode.SelectSingleNode("//head/link[@rel='home']")
                                 ?.GetAttributeValue("title", null);
            var htmlDocTitle = hdoc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();

            // ensure we include the Book + Document nodes (some pages include them in site meta; yours do)
            // (if missing, add:)
            if (!hierarchy.Any(h => h.Type.Equals("Book", StringComparison.OrdinalIgnoreCase)))
                hierarchy.Add(new SiteHierarchyNode { Type = "Book", Id = bookId });
            if (!hierarchy.Any(h => h.Type.Equals("Document", StringComparison.OrdinalIgnoreCase)))
                hierarchy.Add(new SiteHierarchyNode { Type = "Document", Id = docId });

            // ReferencePath (string)
            //var referencePath = string.Join(" > ", hierarchy.Select(h => $"{h.Type}:{h.Id}"));

            // SitePath (xml-ish string like your sample)
            //var sitePath = BuildSitePathXml(hierarchy);

            // Content: your samples had only head; if body exists, grab text
            var bodyText = hdoc.DocumentNode.SelectSingleNode("//body")?.InnerText?.Trim() ?? "";

            // Optional enrich: Site name + Endeca dimension id (for facets to match Endeca)
           // var siteId = hierarchy.FirstOrDefault(h => h.Type.Equals("Site", StringComparison.OrdinalIgnoreCase))?.Id;
            //var siteName = (siteId != null) ? SiteName(siteId) : null;
            //var endecaDimId = (siteId != null) ? EndecaDimId(siteId) : null;

            // Minimal DimensionXml (keep if your UI still expects it; we mirror your example building from subs)
            var dimXml = BuildDimensionXmlFromSubs(subs, hierarchy);

            var doc = new ElasticDocument
            { 
                Id = long.Parse(docId),
                Name = GetMeta("destroyer_document_name"),
                Title = title,
                Content = bodyText,
               // ReferencePath = referencePath,
                SubscriptionCodes = subs,
                BookId = bookId,
                BookName = GetMeta("destroyer_book_name") ?? "",
                SiteHierarchy = hierarchy,
                InSubscription = true, // adjust as needed
                //DimensionXml = dimXml,
                //SitePath = sitePath
            };

            doc.ReferencePath = PathBuilders.BuildReferencePathFriendly(chain, names, htmlBookTitle, htmlDocTitle);
            doc.SitePath = PathBuilders.BuildSitePathXmlFriendly(chain, names, htmlBookTitle, htmlDocTitle);


            return doc;
        }
        // Mirrors your sample: build DimensionXml from subscription codes; keeps Refined dimensions
        private   string BuildDimensionXmlFromSubs(string[] subs, List<SiteHierarchyNode> hierarchy)
        {
            var site = hierarchy.FirstOrDefault(h => h.Type == "Site");
            var book = hierarchy.FirstOrDefault(h => h.Type == "Book");
            var doc = hierarchy.FirstOrDefault(h => h.Type == "Document");

            var selectedPath = new StringBuilder();
            selectedPath.Append("<Dimension><Name>Site</Name><Id>")
                        .Append(site?.Id ?? "")
                        .Append("</Id></Dimension>");
            if (book != null)
                selectedPath.Append("<Dimension><Name>Book</Name><Id>")
                            .Append(book.Id)
                            .Append("</Id></Dimension>");
            if (doc != null)
                selectedPath.Append("<Dimension><Name>Document</Name><Id>")
                            .Append(doc.Id)
                            .Append("</Id></Dimension>");

            var sb = new StringBuilder();
            sb.Append("<Dimensions>");
            sb.Append("<SelectedDimensions><Dimension><Name>Site</Name><Id>")
              .Append(site?.Id ?? "")
              .Append("</Id><DimensionAncestors /><DimensionCompletePath>")
              .Append(selectedPath.ToString())
              .Append("</DimensionCompletePath></Dimension></SelectedDimensions>");

            sb.Append("<RefinementDimensions>");
            foreach (var s in subs ?? Array.Empty<string>())
            {
                sb.Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>")
                  .Append(s)
                  .Append("</Id><DimensionValue><Name>")
                  .Append(s)
                  .Append("</Name><Id>")
                  .Append(s)
                  .Append("</Id><RecordCount>1</RecordCount></DimensionValue>")
                  .Append("<DimensionAncestors />")
                  .Append("<DimensionCompletePath><Dimension><Name>destroyer_site_hierarchy</Name><Id>")
                  .Append(s)
                  .Append("</Id></Dimension></DimensionCompletePath></Dimension>");
            }
            sb.Append("</RefinementDimensions></Dimensions>");
            return sb.ToString();
        }
         

    }
}
