using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MainUI.Shared
{
    public class DocumentDisplayHelpers
    {
        #region Codification Specific Methods
        // sburton: These methods handle the specific cases of the codification, such as
        //   join sections and print friendly

        public static string RemoveCodificationHeaders(string wholeFile)
        {
            // the banner is inside the header, so by stripping it out first, we don't have a problem of nested divs

            wholeFile = Regex.Replace(wholeFile, "<div id=\"banner\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            wholeFile = Regex.Replace(wholeFile, "<div class=\"runhead\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return wholeFile;
        }

        public static string RemoveCodificationFooters(string wholeFile)
        {
            return Regex.Replace(wholeFile, "<div class=\"runfoot\">.+?</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static string IncludeCodificationStyleSheets(string wholeFile, bool ShowSources)
        {
            if (ShowSources)
            {
                const string pattern = "hideCodificationSources.css";
                const string replacement = "showCodificationSources.css";

                wholeFile = Regex.Replace(wholeFile, pattern, replacement, RegexOptions.IgnoreCase);
            }
            else
            {
                // currently we don't have any "print" stylesheets that need to be added
            }

            return wholeFile;
        }

        public static string RemoveNoPrintSpans(string wholeFile)
        {
            return Regex.Replace(wholeFile, "<span class=\"noPrint\">.+?</span>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
        #endregion

        #region DocumentNotCurrentNotice Helper Methods
        public static bool IsDocumentNotCurrent(string bookName)
        {
            return bookName.StartsWith(ContextManager.BOOK_PREFIX_FASB) || bookName.StartsWith(ContextManager.BOOK_PREFIX_ARCH);
        }

        /// <summary>
        /// Adds the div above the content to say that it is out of date.
        /// </summary>
        /// <param name="wholeFile">the text of the whole file</param>
        /// <param name="prepareForScrollBars">True will limit the content to "100%" which then provides scrollbars.
        /// This is best for on-screen viewing.  False lets the document run over, which is better for printouts.</param>
        /// <returns></returns>
        public static string IncludeDocumentNotCurrentNotice(string wholeFile, bool prepareForScrollBars)
        {
            string scrollBarStyleAddition = string.Empty;
            /* - Removed.  This was creating problems in print preview and was doing nothing otherwise.
            if (prepareForScrollBars)
            {
                scrollBarStyleAddition = " height: 100%; max-height: 100%; ";
            }*/

            string styles = @"<style type=""text/css"">
    #docNotCurrentHeader {
    top: 0px;
    left:0px;
    right:0px;
    margin: 0;
    padding-top:10px;
    display: block;
    width: 98%;
    height: 23px;
    background: #e5ebf5;
    z-index: 4;
    border-top: 1px solid #999;
    border-bottom: 1px solid #999;
    }
    #originalDoc {
    display: block;
    overflow: auto;
    z-index: 3;
    }
    .headerPad {
    display: block;
    width: 18px;
    height: 28px;
    float: left;
    }
    .contentPad {
    display: block;
    height: 28px;
    }
</style>";

            const string bodyReplaceRegex = @"<body$1>
	<div id=""docNotCurrentHeader""
	style=""font-family: Verdana, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: darkblue;"">
		<div class=""headerPad""></div>
		This document is not current. Please see the
		<a href=""#"" onclick=""doLink('faf-noticeToConstituents', '115496', false);""
		style=""text-decoration: underline;"" onmouseover=""this.style.color = 'red';"" onmouseout=""this.style.color = '#258';"">FASB Accounting Standards Codification</a>
		for current accounting guidance.
	</div>
	<div id=""originalDoc"">
		<div class=""contentPad""></div>
		$2
	</div>
</body>";

            wholeFile = Regex.Replace(wholeFile, "<head>", "<head>" + styles, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Regex.Replace(wholeFile, "<body([^>]*)>(.*)</body>", bodyReplaceRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
        #endregion
    }
}