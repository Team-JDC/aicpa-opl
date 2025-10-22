using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainUI.Shared
{
    public static class DomainStringHelpers
    {
        public static bool AllDomainsHaveQuickFind(string domain)
        {
            string[] domainList = domain.Split("~".ToCharArray());

            foreach (string s in domainList)
            {
                if (!s.StartsWith("fvs", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(s) && !s.StartsWith("pfp", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}