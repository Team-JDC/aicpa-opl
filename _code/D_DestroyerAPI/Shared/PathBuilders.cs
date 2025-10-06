using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AICPA.Destroyer.Shared
{

    public static class PathBuilders
    {
        // Outputs: "AICPA Online Professional Library > Audit & Accounting Literature > AICPA Guides > Airlines > Preface"
        public static string BuildReferencePathFriendly(
            System.Collections.Generic.List<(string Type, string Id)> chain,
            string bookTitleFromHtml = null,
            string docTitleFromHtml = null)
        {
            var parts = new List<string>();

            foreach (var (type, id) in chain)
            {
                string title = null;

                switch (type)
                {
                    case "Site":
                        title = DestroyerCache.SiteTitleById[id].Title;
                        break;
                    case "SiteFolder":
                        title = DestroyerCache.FolderTitleById[id].Title;
                        break;
                    case "Book":
                        title = DestroyerCache.BookTitleById[id].Title ?? bookTitleFromHtml;
                        break;
                    case "Document":
                        title = docTitleFromHtml; // prefer <title> from HTML
                        break;
                }

                parts.Add(!string.IsNullOrWhiteSpace(title) ? title : $"{type} {id}");
            }

            return string.Join(" > ", parts);
        }

        // Outputs proper XML with element names matching types and Name/Title attributes
        public static string BuildSitePathXmlFriendly(
            System.Collections.Generic.List<(string Type, string Id)> chain,
            string bookTitleFromHtml = null,
            string docTitleFromHtml = null)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<ReferencePath>");

            foreach (var (type, id) in chain)
            {
                string name = null, title = null;

                switch (type)
                {
                    case "Site":
                        title = DestroyerCache.SiteTitleById[id].Title;
                        name = DestroyerCache.SiteTitleById[id].Name;
                        break;
                    case "SiteFolder":
                        title = DestroyerCache.FolderTitleById[id].Title;
                        name = DestroyerCache.FolderTitleById[id].Name;
                        break;
                    case "Book":
                        title = DestroyerCache.BookTitleById[id].Title ?? bookTitleFromHtml;
                        break;
                    case "Document":
                        title = docTitleFromHtml;
                        break;
                }

                sb.Append("<").Append(type)
                  .Append(" Id=\"").Append(E(id)).Append("\"");
                if (!string.IsNullOrWhiteSpace(name))
                    sb.Append(" Name=\"").Append(E(name)).Append("\"");
                if (!string.IsNullOrWhiteSpace(title))
                    sb.Append(" Title=\"").Append(E(title)).Append("\"");
                sb.Append(" />");
            }

            sb.Append("</ReferencePath>");
            return sb.ToString();

            string E(string s) => SecurityElement.Escape(s ?? "");
        }
    }

}
