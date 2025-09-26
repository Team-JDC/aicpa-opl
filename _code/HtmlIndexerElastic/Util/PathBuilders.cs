using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlIndexerElastic.Util
{
   
    public static class PathBuilders
    {
        // Outputs: "AICPA Online Professional Library > Audit & Accounting Literature > AICPA Guides > Airlines > Preface"
        public static string BuildReferencePathFriendly(
            System.Collections.Generic.List<(string Type, string Id)> chain,
            ISqlDestroyerService names,
            string bookTitleFromHtml = null,
            string docTitleFromHtml = null)
        {
            var parts = new System.Collections.Generic.List<string>();

            foreach (var (type, id) in chain)
            {
                var (name, title) = names.Get(type, id);

                // Prefer HTML-derived titles for Book/Document when present
                if (type == "Book" && !string.IsNullOrWhiteSpace(bookTitleFromHtml)) title = bookTitleFromHtml;
                if (type == "Document" && !string.IsNullOrWhiteSpace(docTitleFromHtml)) title = docTitleFromHtml;

                var label = string.IsNullOrWhiteSpace(title) ? $"{type} {id}" : title;
                parts.Add(label);
            }
            return string.Join(" > ", parts);
        }

        // Outputs proper XML with element names matching types and Name/Title attributes
        public static string BuildSitePathXmlFriendly(
            System.Collections.Generic.List<(string Type, string Id)> chain,
            ISqlDestroyerService names,
            string bookTitleFromHtml = null,
            string docTitleFromHtml = null)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<ReferencePath>");

            foreach (var (type, id) in chain)
            {
                var (name, title) = names.Get(type, id);
                if (type == "Book" && !string.IsNullOrWhiteSpace(bookTitleFromHtml)) title = bookTitleFromHtml;
                if (type == "Document" && !string.IsNullOrWhiteSpace(docTitleFromHtml)) title = docTitleFromHtml;

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

            string E(string s) => System.Security.SecurityElement.Escape(s ?? "");
        }
    }

}
