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
        public string GenerateDocIdFromPath(string path)
        {
            var indexDoc = ParseHtml(path);
            var json = JsonConvert.SerializeObject(indexDoc);
            var docId = ($"{indexDoc.Id}_{indexDoc.BookId}")
                .Replace('/', '_')
                .Replace('\\', '_')
                .Replace(' ', '_');
            return docId;
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
                .ToList() ?? new List<string>();

            var hierarchy = metaTags?
                .Where(m => m.GetAttributeValue("name", "") == "destroyer_site_hierarchy")
                .Select(m =>
                {
                    var parts = m.GetAttributeValue("content", "").Split(':');
                    return new SiteHierarchyNode { Type = parts[0], Id = parts[1] };
                })
                .ToList() ?? new List<SiteHierarchyNode>();

            var doc = new ElasticDocument
            {
                Id = int.Parse(GetMeta("destroyer_document_id")),
                Name = GetMeta("destroyer_document_name"),
                Title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "Untitled",
                Content = bodyText,
                ReferencePath = GenerateReferencePath(hierarchy),
                SubscriptionCodes = subs,
                BookId = GetMeta("destroyer_book_id"),
                BookName = GetMeta("destroyer_book_name"),
                SiteHierarchy = hierarchy,
                InSubscription = true, // adjust as needed
                DimensionXml = BuildDimensionXml(hierarchy, subs),
                SitePath = BuildReferencePathXml(hierarchy)
            };

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

        private static string BuildDimensionXml(List<SiteHierarchyNode> hierarchy, List<string> subscriptionCodes)
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
    }
}
