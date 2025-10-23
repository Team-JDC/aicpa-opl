using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Nest;
using Elasticsearch.Net;
namespace MainUI.Shared.Elastic
{
    public static class SearchHelper
    {
        public enum NodeType { Unknown, Site, SiteFolder, Book, Document }

        public sealed class ResolvedNode
        {
            public  NodeType Type;
            public string Id;       // normalized numeric id, e.g., "345"
            public string Title;    // best label we could derive (optional)
            public double? level;
        }

        public static async Task<ResolvedNode> ResolveNodeAsync(IElasticClient client, string rawId)
        {
            var id = (rawId ?? "").Trim();
            if (string.IsNullOrEmpty(id))
                return new ResolvedNode { Type = NodeType.Unknown, Id = "", Title = "" };

            var resp = await client.SearchAsync<ElasticDocument>(s => s
                .Size(0)
                // narrow the search to docs that contain this id in the nested array
                .Query(q => q.Nested(n => n
                    .Path(p => p.SiteHierarchy)
                    .Query(nq => nq.Term(t => t
                        .Field("SiteHierarchy.Id") // exact match
                        .Value(id)
                    ))
                ))
                .Aggregations(a => a
                    // nested agg on SiteHierarchy
                    .Nested("sh", n => n
                        .Path(p => p.SiteHierarchy)
                        .Aggregations(aa => aa
                            // filter to the selected id
                            .Filter("by_id", f => f
                                .Filter(q => q.Term(t => t
                                    .Field("SiteHierarchy.Id")
                                    .Value(id)
                                ))
                                .Aggregations(fff => fff
                                    // which types exist for this id? (Site, SiteFolder, Book, Document)
                                    .Terms("SiteType", t => t
                                        .Field("SiteHierarchy.Type")
                                        .Size(4)
                                    )
                                    // lowest level for this id (to help infer specificity)
                                    .Min("min_level", m => m.Field("SiteHierarchy.Level"))
                                    // fetch a doc so we can read the full chain (titles, etc.)
                                    .TopHits("one", th => th
                                        .Size(1)
                                        .Source(src => src.Includes(i => i.Field(d => d.SiteHierarchy)))
                                    )
                                )
                            )
                        )
                    )
                )
            );

            // if anything went wrong, fail gracefully
            if (!resp.IsValid)
                return new ResolvedNode { Type = NodeType.Unknown, Id = id, Title = "" };

            var byId = resp.Aggregations.Nested("sh")?.Filter("by_id");
            // if the agg exists but matched nothing, buckets/metrics will be empty
            var types = byId?.Terms("SiteType");
            var minLv = byId?.Min("min_level")?.Value;

            // Prefer most specific node type: Book > SiteFolder > Site
            var nodeType = NodeType.Unknown;
            if (types != null && types.Buckets.Any())
            {
                var set = new HashSet<string>(types.Buckets.Select(b => b.Key), StringComparer.OrdinalIgnoreCase);
                if (set.Contains("Book")) nodeType = NodeType.Book;
                else if (set.Contains("SiteFolder")) nodeType = NodeType.SiteFolder;
                else if (set.Contains("Site")) nodeType = NodeType.Site;
                else if (set.Contains("Document")) nodeType = NodeType.Document;
            }

            // Try to get a nice title from the top_hit’s full SiteHierarchy chain
            string title = null;
            var hitDoc = byId?.TopHits("one")?.Documents<OnlyHierarchy>()?.FirstOrDefault();
            var chain = hitDoc?.SiteHierarchy ?? new List<SiteHierarchyNode>();
            if (chain.Count > 0)
            {
                // pick the matching element’s Title (fallback to Name)
                var node = chain.FirstOrDefault(n => n.Id == id);
                title = node?.Title ?? node?.Name;

                // last resort: pick the “best” label by type priority
                if (string.IsNullOrWhiteSpace(title))
                {
                    var preferred = chain
                        .OrderBy(n => n.Type == "Book" ? 0 : n.Type == "SiteFolder" ? 1 : n.Type == "Site" ? 2 : 3)
                        .FirstOrDefault();
                    title = preferred?.Title ?? preferred?.Name;
                }
            }

            return new ResolvedNode
            {
                Type = nodeType,
                Id = id,
                Title = title ?? $"Id {id}",
                level=minLv
            };
        }
         
        // Small XML encoder (keeps it Endeca-style with \u003c / \u003e if you need that, or simple replace)
        private static string XmlEncode(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        // Build Endeca-like DimensionXml (SelectedDimensions + one refinement dimension)
        public static string BuildDimensionXmlFor(
            NodeType selectionType,
            string siteTitleOrNull,
            string siteIdOrNull,
            string selectedFolderTitleOrNull,
            string selectedFolderIdOrNull,
            IEnumerable<Tuple<string, string, long>> refinementItems, // (Id, Title, Count)
            string refinementAxisId // "site_folders" or "books" or "documents"
        )
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<Dimensions>");

            // SelectedDimensions
            sb.Append("<SelectedDimensions>");
            // Always include the root site if we have it
            if (!string.IsNullOrEmpty(siteIdOrNull))
            {
                sb.Append("<Dimension>")
                  .Append("<Name>").Append(XmlEncode(siteTitleOrNull ?? "")).Append("</Name>")
                  .Append("<Id>").Append(XmlEncode(siteIdOrNull)).Append("</Id>")
                  .Append("<DimensionAncestors />")
                  .Append("<DimensionCompletePath>")
                  .Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>").Append(XmlEncode(refinementAxisId)).Append("</Id></Dimension>")
                  .Append("</DimensionCompletePath>")
                  .Append("</Dimension>");
            }
            // If a specific folder is selected, include it too
            if (!string.IsNullOrEmpty(selectedFolderIdOrNull))
            {
                sb.Append("<Dimension>")
                  .Append("<Name>").Append(XmlEncode(selectedFolderTitleOrNull ?? "")).Append("</Name>")
                  .Append("<Id>").Append(XmlEncode(selectedFolderIdOrNull)).Append("</Id>")
                  .Append("<DimensionAncestors />")
                  .Append("<DimensionCompletePath>")
                  .Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>").Append(XmlEncode(refinementAxisId)).Append("</Id></Dimension>")
                  .Append("</DimensionCompletePath>")
                  .Append("</Dimension>");
            }
            sb.Append("</SelectedDimensions>");

            // RefinementDimensions (one axis)
            sb.Append("<RefinementDimensions><Dimension>")
              .Append("<Name>destroyer_site_hierarchy</Name>")
              .Append("<Id>").Append(XmlEncode(refinementAxisId)).Append("</Id>");

            foreach (var item in refinementItems ?? Enumerable.Empty<Tuple<string, string, long>>())
            {
                var id = item.Item1; var title = item.Item2; var cnt = item.Item3;
                sb.Append("<DimensionValue>")
                  .Append("<Name>").Append(XmlEncode(title ?? ("Item " + id))).Append("</Name>")
                  .Append("<Id>").Append(XmlEncode(id)).Append("</Id>")
                  .Append("<RecordCount>").Append(cnt).Append("</RecordCount>")
                  .Append("</DimensionValue>");
            }

            sb.Append("<DimensionAncestors />");
            sb.Append("<DimensionCompletePath>")
              .Append("<Dimension><Name>destroyer_site_hierarchy</Name><Id>").Append(XmlEncode(refinementAxisId)).Append("</Id></Dimension>")
              .Append("</DimensionCompletePath>");

            sb.Append("</Dimension></RefinementDimensions>");
            sb.Append("</Dimensions>");

            // If you need Endeca-style \u003c/\u003e escapes instead of &lt;/&gt;, uncomment:
            // return sb.ToString().Replace("&", "\u0026").Replace("<", "\u003c").Replace(">", "\u003e");
            return sb.ToString();
        }

        public static List<DimensionNavigationResult> BuildSelectedChain(
     ISearchResponse<ElasticDocument> response,
     string selectedId // may be null/empty
 )
        {
            var selected = new List<DimensionNavigationResult>();

            // Case 1: no selection → show the root Site (Endeca-style)
            if (string.IsNullOrWhiteSpace(selectedId))
            {
                var site = response.Hits
                    .SelectMany(h => h.Source?.SiteHierarchy ?? new List<SiteHierarchyNode>())
                    .Where(n => n.Type == "Site")
                    .OrderBy(n => n.Level)
                    .FirstOrDefault();

                if (site != null)
                {
                    selected.Add(new DimensionNavigationResult
                    {
                        DimensionId = null,
                        DimensionName = site.Id,                  // Endeca swaps id/name
                        DimensionValue = site.Title ?? site.Name, // label
                        DimensionCompletePath = null
                    });
                }
                return selected;
            }

            // Case 2: we DO have a selection → find a hit that contains it
            var hitWithSelection = response.Hits
                .Select(h => h.Source)
                .FirstOrDefault(src => src?.SiteHierarchy?.Any(n => n.Id == selectedId) == true);

            if (hitWithSelection == null)
                return selected; // nothing to build

            var chain = hitWithSelection.SiteHierarchy ?? new List<SiteHierarchyNode>();
            if (chain.Count == 0) return selected;

            // find the selected node and then include all ancestors up to it
            var target = chain.FirstOrDefault(n => n.Id == selectedId);
            if (target == null) return selected;

            foreach (var n in chain
                .Where(n => n.Level <= target.Level && (n.Type == "Site" || n.Type == "SiteFolder" || n.Type == "Book"))
                .OrderBy(n => n.Level))
            {
                selected.Add(new DimensionNavigationResult
                {
                    DimensionId = null,
                    DimensionName = n.Id,                  // Endeca format: id in "Name"
                    DimensionValue = n.Title ?? n.Name,    // label in "Value"
                    DimensionCompletePath = null
                });
            }

            return selected;
        }



    }
}