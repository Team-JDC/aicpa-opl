using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MainUI.Shared
{
    public class AICPAAnalytics
    {
        const string ContentTracking_Book_Mask = @"try{{ dataLayer.push({{'collection': '{0}', 'publication':'{1}', 'document':'{2}', pageName: '{3}', loggedInState: '{4}', siteProperty: '{5}', siteSection: '{6}', siteSubSection: '{7}'}});}} catch (e) {{ console.log(e); }}";
        const string SearchTracking_General_Mask = "dataLayer.push({{'Search Type': '{0}', 'Search Term': '{1}', 'event': 'search'}});";
        const string LoginTracking_Mask = "dataLayer.push({{'memberType': '{0}', 'memberID': '{1}', 'loginEntry': '{2}', 'event': '{3}'}});";

        static private string doReplace(String input)
        {
            return Regex.Replace(input, "('|\"|’)", "\\'");
        }

        static public string ContentTracking(List<BreadcrumbNode> bNodes)
        {
            string collection = string.Empty;
            string publication = string.Empty;
            string document = string.Empty;
            int i = bNodes.Count - 1;
            bool done = false;
            while (!done && i >= 0)
            {
                switch (bNodes[i].SiteNode.Type)
                {
                    case "SiteFolder":
                    case "BookFolder":
                        collection = bNodes[i].SiteNode.Title;
                        i--;
                        done = true;
                        break;
                    case "Book":
                        publication = bNodes[i].SiteNode.Title;
                        i--;
                        break;
                    case "Document" :
                        document = bNodes[i].SiteNode.Title;
                        i--;
                        break;
                    default:
                        i--;
                        break;
                }
            }
            return string.Format(ContentTracking_Book_Mask, doReplace(collection), doReplace(publication), doReplace(document), doReplace(document), "Logged In", "OPL", doReplace(collection), doReplace(publication));
        }

        static public string GeneralSearch(string term)
        {
            return string.Format(SearchTracking_General_Mask, "General Search", doReplace(term));
        }

        static public string InternalSearchFilter(string term, string category)
        {
            return string.Empty;
        }

        static private string CreateHash(string inputStr)
        {
            string encstring = inputStr;
            //Instantuate the encoder
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            //Create a byter array big enough for the string
            byte[] unicodeText = new byte[encstring.Length * 2];
            enc.GetBytes(encstring.ToCharArray(), 0, encstring.Length, unicodeText, 0, true);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            //Build the final String by converting each byte into hex and appending it to a string builder
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }       


        static public string LoginLogout(string memberType, string memberId, string loginEntry, string eventType)
        {
            memberId = CreateHash(memberId);
            return string.Format(LoginTracking_Mask, memberType, memberId, loginEntry, eventType);        
        }
    }
}