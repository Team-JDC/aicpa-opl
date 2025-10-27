using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.SessionState;

using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Site;
using MainUI.Shared;
using AICPA.Destroyer.User;
using AICPA.Destroyer.User.Note;
using AICPA.Destroyer.User.Bookmark;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace MainUI.Handlers
{
    /// <summary>
    /// Summary description for GetResource
    /// </summary>
    public class GetResource : IHttpHandler, IRequiresSessionState
    {
        private const string ERROR_RESOURCEBOOKACCESSDENIED = "You do not have access to the specified resource (resource book: '{0}', resource: '{1}'.)";
        private const string ERROR_RESOURCENAMEINVALID = "The specified resource name is invalid (resource book: '{0}', resource: '{1}'.)";
        private const string ERROR_RESOURCEFOLDERNOTFOUND = "A resource folder was not found for the specified resource book (resource book: '{0}', resource: '{1}'.)";
        private const string ERROR_RESOURCENOTFOUND = "The specified resource was not found (resource book: '{0}', resource: '{1}'.)";

        private const string TYPE_NOTES = "notes";
        private const string TYPE_BOOKMARKS = "bookmark";
        private const string SUBSCRIPTION_ACCESS = "subscription_access";
        private const string USER_PREFERENCES = "user_preferences";
        private const string CSS_LOADED = "css_loaded_test";

        private ContextManager contextManager;

        public void ProcessRequest(HttpContext context)
        {
            contextManager = new ContextManager(context);

            //// sburton 2010-05-19: This Code is taken from D_Resource.aspx, there may be better ways to do this


            //get some request values
            string resourceBookName = context.Request[ContextManager.REQPARAM_RESOURCEBOOKNAME];
            string resourceName = context.Request[ContextManager.REQPARAM_RESOURCENAME];

            // Handlers/GetResource.ashx?type=notes&targetDoc=aag
            string type = context.Request[ContextManager.REQPARAM_TYPE];
            string targetDoc = context.Request[ContextManager.REQPARAM_TARGETDOC];

            if (!string.IsNullOrEmpty(type))
            {
                // check for various "types" here
                switch (type)
                {
                    case TYPE_NOTES:
                        StreamNotesStyleSheet(targetDoc);
                        break;
                    case TYPE_BOOKMARKS:
                        StreamBookmarkStyleSheet();
                        break;
                    case SUBSCRIPTION_ACCESS:
                        StreamAccessSpecificStyleSheet();
                        break;
                    case USER_PREFERENCES:
                        StreamUserPreferenceStyleSheet();
                        break;
                    case CSS_LOADED:
                        StreamCssTestStyleSheet();
                        break;

                    default:
                        throw new Exception(string.Format("Unrecognized type '{0}'", type));
                }

            }
            else
            {
                ProcessStandardResource(resourceBookName, resourceName);
            }
        }

        private void ProcessStandardResource(string resourceBookName, string resourceName)
        {
            // get some config values
            HttpContext context = contextManager.Context;
            string resourcePath = GetResourcePath(resourceBookName, resourceName, contextManager);

            // Find mimeType if not found already
            string mimeType = MimeTypeUtil.CheckType(resourcePath);
            //For some reason, there is some content that the .css is changed to _css, so the mimetype is coming up incorrect.  This
            // is a temporary fix until the content build process can be changed.
            if (mimeType == "text/plain" && ((context.Response.ContentType == "text/css") || (resourceName.ToLower().EndsWith("_css"))))
            {
                mimeType = "text/css";
            }

            context.Response.ContentType = mimeType;
            if (mimeType == "text/css")
            {
                contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
                contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
                contextManager.Context.Response.AppendHeader("Expires", "0");
            }

            

            //force a download prompt for all non-html content, making sure that the resource
            //filename is passed in the header to allow the download prompt to display the correct
            //filename
            if (mimeType != "text/html" && mimeType != "text/css")
            {
                string filename = resourceName;

                if (!Path.HasExtension(filename))
                {
                    // see if it has an underscore toward the end of the filename
                    int lastUnderscoreIndex = filename.LastIndexOf("_");
                    if (lastUnderscoreIndex != -1 && lastUnderscoreIndex != 0 && lastUnderscoreIndex != filename.Length - 1
                        && filename.Length - lastUnderscoreIndex <= 5)
                    {
                        // change that underscore to a period so it creates an extension
                        filename = filename.Substring(0, lastUnderscoreIndex) + "." + filename.Substring(lastUnderscoreIndex + 1);
                    }
                }

                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            }

            // Serve up the file
            context.Response.WriteFile(resourcePath);

            context.Response.End();
        }

        public static string GetResourcePath(string resourceBookName, string resourceName, ContextManager manager)
        {
            string bookContentFolder = ConfigurationManager.AppSettings[Book.BOOK_CONTENTFOLDER_KEY];
            string bookResourceFolder = ConfigurationManager.AppSettings[Book.BOOK_RESOURCEFOLDER_KEY];

            // get the site
            ISite site = manager.CurrentSite;

            //get the book mentioned in the request
            IBook book = site.Books[resourceBookName];

            if (book == null)
            {
                string alternateBook = Book.GetAlternateBookName(site.Books, resourceBookName);
                if (!string.IsNullOrEmpty(alternateBook))
                {
                    resourceBookName = alternateBook;
                    book = site.Books[resourceBookName];
                }
            }

            if (book == null)
            {
                throw new Exception(string.Format(ERROR_RESOURCEBOOKACCESSDENIED, resourceBookName, resourceName));
            }

            //throw an error if there are path characters in the resource name (prevent payload attacks)
            if (resourceName.IndexOf(Path.DirectorySeparatorChar) >= 0 || resourceName.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            {
                throw new Exception(string.Format(ERROR_RESOURCENAMEINVALID, resourceBookName, resourceName));
            }

            //construct a path to our resources and throw an error if the directory was not found
            string resourceDirectory = Path.Combine(Path.Combine(bookContentFolder, book.Name + "." + book.Version), bookResourceFolder);
            if (!Directory.Exists(resourceDirectory))
            {
                throw new DirectoryNotFoundException(string.Format(ERROR_RESOURCEFOLDERNOTFOUND, resourceBookName, resourceName));
            }

            //create our full path to the resource
            string resourcePath = Path.Combine(resourceDirectory, resourceName);

            if (!File.Exists(resourcePath))
            {
                resourcePath = resourcePath.Replace("%20", " ");
                if (!File.Exists(resourcePath))
                {
                    throw new FileNotFoundException(string.Format(ERROR_RESOURCENOTFOUND, resourceBookName, resourceName));
                }
            }

            return resourcePath;
        }

        public void StreamAccessSpecificStyleSheet()
        {
            contextManager.Context.Response.ContentType = "text/css";
            //contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            //contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
            //contextManager.Context.Response.AppendHeader("Expires", "0");
            StringBuilder css = new StringBuilder();
            //string css = "";

            // get the site
            ISite site = contextManager.CurrentSite;

            //get the book mentioned in the request
            foreach (IBook book in site.Books)
            {
                css.Append(".contentLink img." + book.Name + " { display: none; }");
            }

            // get all of the books that they should have access to whether or not
            // those books are in the database
            foreach (string book in site.Books.BookList)
            {
                css.Append(".contentLink img." + book + "{ display: none;}");
            }

            //http://stackoverflow.com/questions/21074198/leverage-browser-caching-in-iis-google-pagespeed-issue

            string stylesheet = RemoveWhiteSpaceFromStylesheets(css.ToString());
            int etag = stylesheet.GetHashCode();
            contextManager.Context.Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
            contextManager.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            contextManager.Context.Response.Cache.SetSlidingExpiration(true);
            contextManager.Context.Response.Cache.SetETag(etag.ToString());

            contextManager.Context.Response.Write(stylesheet);
            contextManager.Context.Response.End();
        }

        public void StreamNotesStyleSheet(string filterTargetDoc)
        {
            contextManager.Context.Response.ContentType = "text/css";
            contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
            contextManager.Context.Response.AppendHeader("Expires", "0");
            

            StringBuilder css = new StringBuilder();
            

            css.Append(".notesButtons .editNote { display: none; }");

            // get the site
            ISite site = contextManager.CurrentSite;

            // get the user
            IUser user = contextManager.CurrentUser;            
            IEnumerable<INote> retrievedNotes = Note.GetNotesForUser(user.UserId, site.Id);

            foreach (INote note in retrievedNotes)
            {
                string targetDoc = note.TargetDoc.Replace(".", "\\.");
                string targetPtr = note.TargetPtr.Replace(".", "\\.");

                if (targetDoc.Equals(filterTargetDoc, StringComparison.CurrentCultureIgnoreCase))
                {
                    css.Append("span#" + targetDoc + "-" + targetPtr + " span.editNote");
                    css.Append("{");
                    css.Append("display: inline;");
                    css.Append("}");
                    css.Append("\n");
                }

                /*css += "span#" + targetDoc + "-" + targetPtr + " span.addNote";
                css += "{";
                css += "display: none;";
                css += "}";
                css += "\n";*/
            }
            
            css.Append("");
            //http://stackoverflow.com/questions/21074198/leverage-browser-caching-in-iis-google-pagespeed-issue
            string stylesheet = css.ToString();
            //int etag = stylesheet.GetHashCode();
            //contextManager.Context.Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
            //contextManager.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            //contextManager.Context.Response.Cache.SetSlidingExpiration(true);
            //contextManager.Context.Response.Cache.SetETag(etag.ToString());

            contextManager.Context.Response.Write(stylesheet);
            contextManager.Context.Response.End();
        }

        public void StreamBookmarkStyleSheet()
        {
            contextManager.Context.Response.ContentType = "text/css";
            //contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            //contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
            //contextManager.Context.Response.AppendHeader("Expires", "0");

            StringBuilder css = new StringBuilder();

            css.Append(".bookmarksbuttons .deleteBookmark { display:none; }\n");

            // get the site
            ISite site = contextManager.CurrentSite;

            // get the user
            IUser user = contextManager.CurrentUser;
            IEnumerable<IBookmark> retrievedBookmarks = Bookmark.GetBookmarksForUser(user.UserId);

            foreach (IBookmark bookmark in retrievedBookmarks)
            {
                string targetDoc = bookmark.TargetDoc.Replace(".", "\\.");
                string targetPtr = bookmark.TargetPtr.Replace(".", "\\.");

                css.Append("span#" + targetDoc + "-" + targetPtr + " span.deleteBookmark { display: inline; }\n");

                css.Append("span#" + targetDoc + "-" + targetPtr + " span.addBookmark {display: none;}\n");
            }

            css.Append("");

            //http://stackoverflow.com/questions/21074198/leverage-browser-caching-in-iis-google-pagespeed-issue
            string stylesheet = css.ToString();
            int etag = stylesheet.GetHashCode();
            contextManager.Context.Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
            contextManager.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            contextManager.Context.Response.Cache.SetSlidingExpiration(true);
            contextManager.Context.Response.Cache.SetETag(etag.ToString());


            contextManager.Context.Response.Write(stylesheet);
            contextManager.Context.Response.End();
        }

        public void StreamUserPreferenceStyleSheet()
        {
            contextManager.Context.Response.ContentType = "text/css";
            contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
            contextManager.Context.Response.AppendHeader("Expires", "0");
            StringBuilder css = new StringBuilder();

            
            IUser user = contextManager.CurrentUser;
            //******************************************************************************************
            // font-size
            //******************************************************************************************
            string fontSizeInt = user.Preferences["FontSize"];

            switch (fontSizeInt)
            {
                case "1":
                    css.Append(smallCSS);
                    break;
                case "2":
                    css.Append(mediumCSS);
                    break;
                case "3":
                    css.Append(largeCSS);
                    break;
                case "4":
                    css.Append(xlargeCSS);
                    break;
                default:
                    css.Append(mediumCSS); //medium is the default.
                    break;
            }

            //http://stackoverflow.com/questions/21074198/leverage-browser-caching-in-iis-google-pagespeed-issue

            string stylesheet = RemoveWhiteSpaceFromStylesheets(css.ToString());
            //int etag = stylesheet.GetHashCode();
            //contextManager.Context.Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
            //contextManager.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            //contextManager.Context.Response.Cache.SetSlidingExpiration(true);
            //contextManager.Context.Response.Cache.SetETag(etag.ToString());


            contextManager.Context.Response.Write(stylesheet);
            contextManager.Context.Response.End();
        }

        public static string RemoveWhiteSpaceFromStylesheets(string body)
        {
            body = Regex.Replace(body, @"[a-zA-Z]+#", "#");
            body = Regex.Replace(body, @"[\n\r]+\s*", string.Empty);
            body = Regex.Replace(body, @"\s+", " ");
            body = Regex.Replace(body, @"\s?([:,;{}])\s?", "$1");
            body = body.Replace(";}", "}");
            body = Regex.Replace(body, @"([\s:]0)(px|pt|%|em)", "$1");

            // Remove comments from CSS
            body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);

            return body;
        }


        public void StreamCssTestStyleSheet()
        {
            contextManager.Context.Response.ContentType = "text/css";
            contextManager.Context.Response.AppendHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            contextManager.Context.Response.AppendHeader("Pragma", "no-cache");
            contextManager.Context.Response.AppendHeader("Expires", "0");

            string css = @"#css-loaded { width: 3px; height: 3px;}";

            contextManager.Context.Response.Write(css);
            contextManager.Context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region smallCSS (1.css)
        private const string smallCSS = @"
/* 1.css - smallCSS */
* 
{
	
}

.leftcol_content p {
	 	
	font-size:11px;
}
h1,h2,h3,h4,h5,h6
{

font-size:13px;
}
a 
{
     
    font-size:inherit;
}
.italic {}
.citetitle {font-size:inherit; }
span {font-size:inherit; }
.unicode_symbol {font-family: arial unicode ms;}

/* --------------------- Main/PS --------------------- */

div.preface * .ps_center_alpha, div.chapter * .ps_center_alpha, div.section * .ps_center_alpha, div.appendix * .ps_center_alpha, div.glossary * .ps_center_alpha
{
	font-size:18px;
}

div.navheader *, div.navfooter *
{
	
	font-size:11px;
}

div.preface * .ps_section_heading, div.chapter * .ps_section_heading, div.section * .ps_section_heading, div.appendix * .ps_section_heading, div.glossary * .ps_section_heading
{
	font-size: 18px;
}

div.preface * .ps_tab_heading, div.chapter * .ps_tab_heading, div.section * .ps_tab_heading, div.appendix * .ps_tab_heading, div.glossary * .ps_tab_heading
{
	font-size:18px;
}

div.toc *
{
	font-size:12px;
}

div.preface * .ps_level_2, div.chapter * .ps_level_2, div.section * .ps_level_2, div.appendix * .ps_level_2, div.glossary * .ps_level_2
{
	font-size:19px;
}

div.preface * .ps_tab_heading_index, div.chapter * .ps_tab_heading_index, div.section * .ps_tab_heading_index, div.appendix * .ps_tab_heading_index, div.glossary * .ps_tab_heading_index
{
	font-size:18px;
}


.footnote, .footnote a, .footnote p, .footnote p a, .footnote span
{
	font-size: 9px;
}


.ls_level_3
{
	font-size:18px;	
}

.ls_level_4
{
	font-size: 18px;

}

.ps_box_-_guide
{
	font-size: 11px;
}

.ps_box_-_guide_center
{
	font-size: 11px;
}

.ps_bullet
{
	font-size: 12px;
}

ul.ps_bullet
{
	font-size: 12px;
}

.ps_bullet_subparagraph
{
	font-size: 12px;
}

ul.ps_bullet_subparagraph
{
	font-size: 12px;
}

.ps_full
{
	font-size: 12px;
}

.ps_definition
{
	font-size: 12px;
}

.ps_left_text
{
	font-size: 12px;
}

.ps_main_heading {
	font-size: 15px;}

.ps_main_heading_center
{
	font-size: 15px;
}

.ps_para_number
{
	font-size: 11px;
}

.ps_para_number_full
{
	font-size: 11px;
}

.ps_para_number_center
{
	font-size: 11px;
}

.ps_para_number_left
{
	font-size: 11px;
}

.ps_para_number_right
{
	font-size: 11px;
}

.ps_paragraph
{
	font-size: 11px;
}

.ps_paragraph_center
{
	font-size: 11px;
}

.ps_paragraph_full
{
	font-size: 11px;
}

.ps_paragraph_left
{
	font-size: 11px;
}

.ps_paragraph_right
{
	font-size: 11px;
}

.ps_left_10
{
	font-size: 11px;
}

.ps_center_10
{
	font-size: 11px;
}

.ps_right_10
{
	font-size: 11px;
}

.ps_right_line
{
	font-size: 11px;
}

.ps_section_title
{
	font-size: 18px;
}

.ps_sub_heading
{
	font-size: 14px;
}

.ps_sub_heading_right
{
	font-size: 14px;
}

.ps_sub_heading_center
{
	font-size: 14px;
}

.ps_sub_heading_full
{
	font-size: 14px;
}

.ps_sub_heading_left
{
	font-size: 14px;
}

.ps_sub_heading_right
{
	font-size: 14px;
}

.ps_subparagraph
{
	font-size: 14px;
}

ul.ps_subparagraph
{
	font-size: 14px;
}

.ps_subsub_heading
{
	font-size: 12px;
}

.ps_subsub_heading_center
{
	font-size: 12px;
}

.ps_subsubsub_heading
{
	font-size: 12px;
}

.ps_tab_name
{
	font-size: 18px;
}

.ps_bullet_paragraph
{
	font-size: 12px;
}

ul.ps_bullet_paragraph
{
	font-size: 12px;
}

.ps_indent_section_paragraph
{
	font-size: 12px;
}

ul.ps_indent_section_paragraph
{
	font-size: 12px;
}

.ps_table_text_left
{
	font-size: 11px;
}

.ps_table_text_right
{
	font-size: 11px;
}

.ps_table_text_center
{
	font-size: 11px;
}

.ps_table_text_full
{
	font-size: 11px;
}

.ps_table_title
{
	font-size: 12px;
}

.ps_table_title_center
{
	font-size: 12px;
}

/* set all the following table cell content to 11px font */
div.ps_table_center table td, div.ps_table_center table th
{font-size: 11px;}

div.ps_table_left table td, div.ps_table_left table th
{font-size: 11px;}

div.ps_table_right table td, div.ps_table_right table th
{font-size: 11px;}

div.ps_table_blockquote table td, div.ps_table_blockquote table th
{font-size: 11px;}

div.ps_table_landscape table td, div.ps_table_landscape table th
{font-size: 11px;}

div.ps_indent table td, div.ps_indent table th
{font-size: 11px;}

div.ps_indent_0 table td, div.ps_indent_0 table th
{font-size: 11px;}

div.ps_indent_1 table td, div.ps_indent_1 table th
{font-size: 11px;}

div.ps_indent_2 table td, div.ps_indent_2 table th
{font-size: 11px;}

div.ps_indent_3 table td, div.ps_indent_3 table th
{font-size: 11px;}

div.ps_indent_4 table td, div.ps_indent_4 table th
{font-size: 11px;}

div.ps_indent_5 table td, div.ps_indent_5 table th
{font-size: 11px;}

div.ps_indent_6 table td, div.ps_indent_6 table th
{font-size: 11px;}

div.ps_indent_7 table td, div.ps_indent_7 table th
{font-size: 11px;}

div.ps_indent_8 table td, div.ps_indent_8 table th
{font-size: 11px;}

div.ps_indent_9 table td, div.ps_indent_9 table th
{font-size: 11px;}


/* set all the following table cell content to 12px font */
div.ps_table_center_normal table td, div.ps_table_center_normal table th
{font-size: 12px;}

div.ps_table_left_normal table td, div.ps_table_left_normal table th
{font-size: 12px;}

div.ps_table_right_normal table td, div.ps_table_right_normal table th
{font-size: 12px;}

div.ps_table_blockquote_normal table td, div.ps_table_blockquote_normal table th
{font-size: 12px;}

div.ps_table_landscape_normal table td, div.ps_table_landscape_normal table th
{font-size: 12px;}

div.ps_indent_normal table td, div.ps_indent_normal table th
{font-size: 12px;}

div.ps_indent_0_normal table td, div.ps_indent_0_normal table th
{font-size: 12px;}

div.ps_indent_1_normal table td, div.ps_indent_1_normal table th
{font-size: 12px;}

div.ps_indent_2_normal table td, div.ps_indent_2_normal table th
{font-size: 12px;}

div.ps_indent_3_normal table td, div.ps_indent_3_normal table th
{font-size: 12px;}

div.ps_indent_4_normal table td, div.ps_indent_4_normal table th
{font-size: 12px;}

div.ps_indent_5_normal table td, div.ps_indent_5_normal table th
{font-size: 12px;}

div.ps_indent_6_normal table td, div.ps_indent_6_normal table th
{font-size: 12px;}

div.ps_indent_7_normal table td, div.ps_indent_7_normal table th
{font-size: 12px;}

div.ps_indent_8_normal table td, div.ps_indent_8_normal table th
{font-size: 12px;}

div.ps_indent_9_normal table td, div.ps_indent_9_normal table th
{font-size: 12px;}

/* ------------------ Common LTR ------------------ */

.topictitle1 { font-size: 15px; }
.topictitle2 { font-size: 12px; }
.topictitle3 { font-size: 12px; }
.topictitle4 { font-size: 12px; }
.topictitle5 { font-size: 12px; }
.topictitle6 { font-size: 12px; }
.sectiontitle { font-size: 12px; }


/* ------------------ FASB ------------------ */

div.navheader *, div.navfooter *
{
font-size:11px;
}

.ps_cod_secondary_level, .ps_fasb_cod_secondary_level
{
font-size:11px;
}

.ps_cod_tertiary_level, .ps_fasb_cod_tertiary_level
{
font-size:11px;
}

.ps_cod_fourth_level, .ps_fasb_cod_fourth_level
{
font-size:11px;
}

.ps_cod_fifth_level, .ps_fasb_cod_fifth_level
{
font-size:11px;
}

.ps_level2, .ps_fasb_level2
{
font-size:11px;
}

.ps_subtitle, .ps_fasb_subtitle
{
font-size:13px;
}

.ps_tertiary_heading, .ps_fasb_tertiary_heading
{
font-size:13px;
}

.ps_toc_entry, .ps_fasb_toc_entry
{

}

.ps_toc_title, .ps_fasb_toc_title
{
font-size:12px;
}

.ps_document_type, .ps_fasb_document_type
{
font-size:12px;
}

.ps_document_type_sub, .ps_fasb_document_type_sub
{
font-size:18px;
}

.ps_title, .ps_fasb_title
{
font-size:13px;
}

.ps_fourth_level, .ps_fasb_fourth_level
{
font-size:11px;
}

.ps_fifth_level, .ps_fasb_fifth_level
{
font-size:11px;
}

.ps_sixth_level, .ps_fasb_sixth_level
{
font-size:11px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:9px;
}

.ps_eighth_level, .ps_fasb_eighth_level
{
font-size:9px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:9px;
}

.ps_fourth_level-type_heading, .ps_fasb_fourth_level-type_heading
{
font-size:11px;
}

.ps_quoted_footnote, .ps_fasb_quoted_footnote
{
font-size:11px;
}

.ps_fifth_heading, .ps_fasb_fifth_heading
{
font-size:11px;
}

.ps_fourth_heading, .ps_fasb_fourth_heading
{
font-size:11px;
}

.ps_volume_title, .ps_fasb_volume_title
{
font-size:45px;
}

.ps_sixth_heading, .ps_fasb_sixth_heading
{
font-size:9px;
}

.ps_section_title, .ps_fasb_section_title
{
font-size:18px;
}

.ps_third_level_para_bullet_list, .ps_fasb_third_level_para_bullet_list
{
font-size:12px;
}

.ps_fourth_level_para_bullet_list, .ps_fasb_fourth_level_para_bullet_list
{
font-size:12px;
}

.ps_index_primary_level, .ps_fasb_index_primary_level
{
font-size:11px;
}

.ps_index_heading, .ps_fasb_index_heading
{
font-size:11px;
}

.ps_index_entry, .ps_fasb_index_entry
{
font-size:11px;
}

.ps_index_entry_2, .ps_fasb_index_entry_2
{
font-size:11px;
}

.ps_index_entry_3, .ps_fasb_index_entry_3
{
font-size:11px;
}

.ps_index_entry_4, .ps_fasb_index_entry_4
{
font-size:11px;
}

.ps_alpha_menu, .ps_fasb_alpha_menu
{
font-size:18px;
}

.ps_normal_level-variation8, .ps_fasb_normal_level-variation8
{
font-size:12px;
}

/* ------------------ GASB ------------------ */

.ps_copyright_notice
{
font-size:11px;
}

/* ------------------ ASC ------------------ */

.pending-text-title 
{
	
    font-size:13px;
}
.pending-text span {font-size:11px; }

.secTitle
{
	font-size:18px;
}


.prev { 

}
.next { 	

}
.prevdisabled { }
.nextdisabled { }

.required { font-size: 13px; }

.asc_heading {
	font-size:18px; 
}

.asc_pgroup { 
	font-size:13px; 
}

.section_content #section_wrapper h1 {  
	font-size:18px; 
}

.section_content #section_wrapper h2 { 
	font-size:12px; 
}

.section_content #section_wrapper h3 { 
	font-size:11px; 
}

#fasb_info {
	font-size:11px; 
	
}

.general_note {
		 
}

";
        #endregion

        #region mediumCSS (2.css)
        private const string mediumCSS = @"
/* 2.css - MediumCSS */
* 
{
	
}

.leftcol_content p {
	 	
	font-size:13px;
}
a 
{
     
    font-size:inherit;
}
.italic {}
.citetitle {font-size:inherit; }
span {font-size:inherit; }
.unicode_symbol {font-family: arial unicode ms;}

/* --------------------- Main/PS --------------------- */

div.preface * .ps_center_alpha, div.chapter * .ps_center_alpha, div.section * .ps_center_alpha, div.appendix * .ps_center_alpha, div.glossary * .ps_center_alpha
{
	font-size:21px;
}

div.navheader *, div.navfooter *
{
	
	font-size:13px;
}

div.preface * .ps_section_heading, div.chapter * .ps_section_heading, div.section * .ps_section_heading, div.appendix * .ps_section_heading, div.glossary * .ps_section_heading
{
	font-size: 19px;
}

div.preface * .ps_tab_heading, div.chapter * .ps_tab_heading, div.section * .ps_tab_heading, div.appendix * .ps_tab_heading, div.glossary * .ps_tab_heading
{
	font-size:19px;
}

div.toc *
{
	font-size:15px;
}

div.preface * .ps_level_2, div.chapter * .ps_level_2, div.section * .ps_level_2, div.appendix * .ps_level_2, div.glossary * .ps_level_2
{
	font-size:12px;
}

div.preface * .ps_tab_heading_index, div.chapter * .ps_tab_heading_index, div.section * .ps_tab_heading_index, div.appendix * .ps_tab_heading_index, div.glossary * .ps_tab_heading_index
{
	font-size:19px;
}


.footnote, .footnote a, .footnote p, .footnote p a, .footnote span
{
	font-size: 12px;
}


.ls_level_3
{
	font-size:21px;	
}

.ls_level_4
{
	font-size: 21px;

}

.ps_box_-_guide
{
	font-size: 13px;
}

.ps_box_-_guide_center
{
	font-size: 13px;
}

.ps_bullet
{
	font-size: 15px;
}

ul.ps_bullet
{
	font-size: 15px;
}

.ps_bullet_subparagraph
{
	font-size: 15px;
}

ul.ps_bullet_subparagraph
{
	font-size: 15px;
}

.ps_full
{
	font-size: 15px;
}

.ps_definition
{
	font-size: 15px;
}

.ps_left_text
{
	font-size: 15px;
}

.ps_main_heading {
	font-size: 18px;}

.ps_main_heading_center
{
	font-size: 18px;
}

.ps_para_number
{
	font-size: 13px;
}

.ps_para_number_full
{
	font-size: 13px;
}

.ps_para_number_center
{
	font-size: 13px;
}

.ps_para_number_left
{
	font-size: 13px;
}

.ps_para_number_right
{
	font-size: 13px;
}

.ps_paragraph
{
	font-size: 13px;
}

.ps_paragraph_center
{
	font-size: 13px;
}

.ps_paragraph_full
{
	font-size: 13px;
}

.ps_paragraph_left
{
	font-size: 13px;
}

.ps_paragraph_right
{
	font-size: 13px;
}

.ps_left_10
{
	font-size: 13px;
}

.ps_center_10
{
	font-size: 13px;
}

.ps_right_10
{
	font-size: 13px;
}

.ps_right_line
{
	font-size: 13px;
}

.ps_section_title
{
	font-size: 21px;
}

.ps_sub_heading
{
	font-size: 16px;
}

.ps_sub_heading_right
{
	font-size: 16px;
}

.ps_sub_heading_center
{
	font-size: 16px;
}

.ps_sub_heading_full
{
	font-size: 16px;
}

.ps_sub_heading_left
{
	font-size: 16px;
}

.ps_sub_heading_right
{
	font-size: 16px;
}

.ps_subparagraph
{
	font-size: 16px;
}

ul.ps_subparagraph
{
	font-size: 16px;
}

.ps_subsub_heading
{
	font-size: 14px;
}

.ps_subsub_heading_center
{
	font-size: 14px;
}

.ps_subsubsub_heading
{
	font-size: 14px;
}

.ps_tab_name
{
	font-size: 21px;
}

.ps_bullet_paragraph
{
	font-size: 15px;
}

ul.ps_bullet_paragraph
{
	font-size: 15px;
}

.ps_indent_section_paragraph
{
	font-size: 15px;
}

ul.ps_indent_section_paragraph
{
	font-size: 15px;
}

.ps_table_text_left
{
	font-size: 13px;
}

.ps_table_text_right
{
	font-size: 13px;
}

.ps_table_text_center
{
	font-size: 13px;
}

.ps_table_text_full
{
	font-size: 13px;
}

.ps_table_title
{
	font-size: 15px;
}

.ps_table_title_center
{
	font-size: 15px;
}

/* set all the following table cell content to 13px font */
div.ps_table_center table td, div.ps_table_center table th
{font-size: 13px;}

div.ps_table_left table td, div.ps_table_left table th
{font-size: 13px;}

div.ps_table_right table td, div.ps_table_right table th
{font-size: 13px;}

div.ps_table_blockquote table td, div.ps_table_blockquote table th
{font-size: 13px;}

div.ps_table_landscape table td, div.ps_table_landscape table th
{font-size: 13px;}

div.ps_indent table td, div.ps_indent table th
{font-size: 13px;}

div.ps_indent_0 table td, div.ps_indent_0 table th
{font-size: 13px;}

div.ps_indent_1 table td, div.ps_indent_1 table th
{font-size: 13px;}

div.ps_indent_2 table td, div.ps_indent_2 table th
{font-size: 13px;}

div.ps_indent_3 table td, div.ps_indent_3 table th
{font-size: 13px;}

div.ps_indent_4 table td, div.ps_indent_4 table th
{font-size: 13px;}

div.ps_indent_5 table td, div.ps_indent_5 table th
{font-size: 13px;}

div.ps_indent_6 table td, div.ps_indent_6 table th
{font-size: 13px;}

div.ps_indent_7 table td, div.ps_indent_7 table th
{font-size: 13px;}

div.ps_indent_8 table td, div.ps_indent_8 table th
{font-size: 13px;}

div.ps_indent_9 table td, div.ps_indent_9 table th
{font-size: 13px;}


/* set all the following table cell content to 15px font */
div.ps_table_center_normal table td, div.ps_table_center_normal table th
{font-size: 15px;}

div.ps_table_left_normal table td, div.ps_table_left_normal table th
{font-size: 15px;}

div.ps_table_right_normal table td, div.ps_table_right_normal table th
{font-size: 15px;}

div.ps_table_blockquote_normal table td, div.ps_table_blockquote_normal table th
{font-size: 15px;}

div.ps_table_landscape_normal table td, div.ps_table_landscape_normal table th
{font-size: 15px;}

div.ps_indent_normal table td, div.ps_indent_normal table th
{font-size: 15px;}

div.ps_indent_0_normal table td, div.ps_indent_0_normal table th
{font-size: 15px;}

div.ps_indent_1_normal table td, div.ps_indent_1_normal table th
{font-size: 15px;}

div.ps_indent_2_normal table td, div.ps_indent_2_normal table th
{font-size: 15px;}

div.ps_indent_3_normal table td, div.ps_indent_3_normal table th
{font-size: 15px;}

div.ps_indent_4_normal table td, div.ps_indent_4_normal table th
{font-size: 15px;}

div.ps_indent_5_normal table td, div.ps_indent_5_normal table th
{font-size: 15px;}

div.ps_indent_6_normal table td, div.ps_indent_6_normal table th
{font-size: 15px;}

div.ps_indent_7_normal table td, div.ps_indent_7_normal table th
{font-size: 15px;}

div.ps_indent_8_normal table td, div.ps_indent_8_normal table th
{font-size: 15px;}

div.ps_indent_9_normal table td, div.ps_indent_9_normal table th
{font-size: 15px;}

/* ------------------ Common LTR ------------------ */

.topictitle1 { font-size: 18px; }
.topictitle2 { font-size: 15px; }
.topictitle3 { font-size: 15px; }
.topictitle4 { font-size: 15px; }
.topictitle5 { font-size: 15px; }
.topictitle6 { font-size: 15px; }
.sectiontitle { font-size: 15px; }


/* ------------------ FASB ------------------ */



div.navheader *, div.navfooter *
{
font-size:13px;
}

.ps_cod_secondary_level, .ps_fasb_cod_secondary_level
{
font-size:13px;
}

.ps_cod_tertiary_level, .ps_fasb_cod_tertiary_level
{
font-size:13px;
}

.ps_cod_fourth_level, .ps_fasb_cod_fourth_level
{
font-size:13px;
}

.ps_cod_fifth_level, .ps_fasb_cod_fifth_level
{
font-size:13px;
}

.ps_level2, .ps_fasb_level2
{
font-size:13px;
}

.ps_subtitle, .ps_fasb_subtitle
{
font-size:16px;
}

.ps_tertiary_heading, .ps_fasb_tertiary_heading
{
font-size:16px;
}

.ps_toc_entry, .ps_fasb_toc_entry
{

}

.ps_toc_title, .ps_fasb_toc_title
{
font-size:15px;
}

.ps_document_type, .ps_fasb_document_type
{
font-size:28px;
}

.ps_document_type_sub, .ps_fasb_document_type_sub
{
font-size:19px;
}

.ps_title, .ps_fasb_title
{
font-size:16px;
}

.ps_fourth_level, .ps_fasb_fourth_level
{
font-size:13px;
}

.ps_fifth_level, .ps_fasb_fifth_level
{
font-size:13px;
}

.ps_sixth_level, .ps_fasb_sixth_level
{
font-size:13px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:12px;
}

.ps_eighth_level, .ps_fasb_eighth_level
{
font-size:12px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:12px;
}

.ps_fourth_level-type_heading, .ps_fasb_fourth_level-type_heading
{
font-size:13px;
}

.ps_quoted_footnote, .ps_fasb_quoted_footnote
{
font-size:13px;
}

.ps_fifth_heading, .ps_fasb_fifth_heading
{
font-size:13px;
}

.ps_fourth_heading, .ps_fasb_fourth_heading
{
font-size:13px;
}

.ps_volume_title, .ps_fasb_volume_title
{
font-size:45px;
}

.ps_sixth_heading, .ps_fasb_sixth_heading
{
font-size:12px;
}

.ps_section_title, .ps_fasb_section_title
{
font-size:21px;
}

.ps_third_level_para_bullet_list, .ps_fasb_third_level_para_bullet_list
{
font-size:15px;
}

.ps_fourth_level_para_bullet_list, .ps_fasb_fourth_level_para_bullet_list
{
font-size:15px;
}

.ps_index_primary_level, .ps_fasb_index_primary_level
{
font-size:13px;
}

.ps_index_heading, .ps_fasb_index_heading
{
font-size:13px;
}

.ps_index_entry, .ps_fasb_index_entry
{
font-size:13px;
}

.ps_index_entry_2, .ps_fasb_index_entry_2
{
font-size:13px;
}

.ps_index_entry_3, .ps_fasb_index_entry_3
{
font-size:13px;
}

.ps_index_entry_4, .ps_fasb_index_entry_4
{
font-size:13px;
}

.ps_alpha_menu, .ps_fasb_alpha_menu
{
font-size:21px;
}

.ps_normal_level-variation8, .ps_fasb_normal_level-variation8
{
font-size:15px;
}

/* ------------------ GASB ------------------ */

.ps_copyright_notice
{
font-size:13px;
}

/* ------------------ ASC ------------------ */

.pending-text-title 
{
	
    font-size:16px;
}
.pending-text span {font-size:13px; }

.secTitle
{
	font-size:21px;
}


.prev { 

}
.next { 	

}
.prevdisabled { }
.nextdisabled { }

.required { font-size: 16px; }

.asc_heading {
	font-size:21px; 
}

.asc_pgroup { 
	font-size:16px; 
}

.section_content #section_wrapper h1 {  
	font-size:21px; 
}

.section_content #section_wrapper h2 { 
	font-size:15px; 
}

.section_content #section_wrapper h3 { 
	font-size:13px; 
}

#fasb_info {
	font-size:13px; 
	
}

.general_note {
		 
}


";
        #endregion

        #region largeCSS (3.css)
        private const string largeCSS = @"
/* 3.css - largeCSS */
* 
{
	
}

.leftcol_content p {
	
	font-size:15px;
}
h1,h2,h3,h4,h5,h6
{

font-size:18px;
}
a 
{
     
    font-size:inherit;
}
.italic {}
.citetitle {font-size:inherit; }
span {font-size:inherit; }
.unicode_symbol {font-family: arial unicode ms;}

/* --------------------- Main/PS --------------------- */

div.preface * .ps_center_alpha, div.chapter * .ps_center_alpha, div.section * .ps_center_alpha, div.appendix * .ps_center_alpha, div.glossary * .ps_center_alpha
{
	font-size:13px;
}

div.navheader *, div.navfooter *
{
	
	font-size:15px;
}

div.preface * .ps_section_heading, div.chapter * .ps_section_heading, div.section * .ps_section_heading, div.appendix * .ps_section_heading, div.glossary * .ps_section_heading
{
	font-size: 12px;
}

div.preface * .ps_tab_heading, div.chapter * .ps_tab_heading, div.section * .ps_tab_heading, div.appendix * .ps_tab_heading, div.glossary * .ps_tab_heading
{
	font-size:12px;
}

div.toc *
{
	font-size:18px;
}

div.preface * .ps_level_2, div.chapter * .ps_level_2, div.section * .ps_level_2, div.appendix * .ps_level_2, div.glossary * .ps_level_2
{
	font-size:28px;
}

div.preface * .ps_tab_heading_index, div.chapter * .ps_tab_heading_index, div.section * .ps_tab_heading_index, div.appendix * .ps_tab_heading_index, div.glossary * .ps_tab_heading_index
{
	font-size:12px;
}


.footnote, .footnote a, .footnote p, .footnote p a, .footnote span
{
	font-size: 13px;
}

.ls_level_3
{
	font-size:13px;	
}

.ls_level_4
{
	font-size: 13px;

}

.ps_box_-_guide
{
	font-size: 15px;
}

.ps_box_-_guide_center
{
	font-size: 15px;
}

.ps_bullet
{
	font-size: 18px;
}

ul.ps_bullet
{
	font-size: 18px;
}

.ps_bullet_subparagraph
{
	font-size: 18px;
}

ul.ps_bullet_subparagraph
{
	font-size: 18px;
}

.ps_full
{
	font-size: 18px;
}

.ps_definition
{
	font-size: 18px;
}

.ps_left_text
{
	font-size: 18px;
}

.ps_main_heading {
	font-size: 21px;}

.ps_main_heading_center
{
	font-size: 21px;
}

.ps_para_number
{
	font-size: 15px;
}

.ps_para_number_full
{
	font-size: 15px;
}

.ps_para_number_center
{
	font-size: 15px;
}

.ps_para_number_left
{
	font-size: 15px;
}

.ps_para_number_right
{
	font-size: 15px;
}

.ps_paragraph
{
	font-size: 15px;
}

.ps_paragraph_center
{
	font-size: 15px;
}

.ps_paragraph_full
{
	font-size: 15px;
}

.ps_paragraph_left
{
	font-size: 15px;
}

.ps_paragraph_right
{
	font-size: 15px;
}

.ps_left_10
{
	font-size: 15px;
}

.ps_center_10
{
	font-size: 15px;
}

.ps_right_10
{
	font-size: 15px;
}

.ps_right_line
{
	font-size: 15px;
}

.ps_section_title
{
	font-size: 13px;
}

.ps_sub_heading
{
	font-size: 18px;
}

.ps_sub_heading_right
{
	font-size: 18px;
}

.ps_sub_heading_center
{
	font-size: 18px;
}

.ps_sub_heading_full
{
	font-size: 18px;
}

.ps_sub_heading_left
{
	font-size: 18px;
}

.ps_sub_heading_right
{
	font-size: 18px;
}

.ps_subparagraph
{
	font-size: 18px;
}

ul.ps_subparagraph
{
	font-size: 18px;
}

.ps_subsub_heading
{
	font-size: 16px;
}

.ps_subsub_heading_center
{
	font-size: 16px;
}

.ps_subsubsub_heading
{
	font-size: 16px;
}

.ps_tab_name
{
	font-size: 13px;
}

.ps_bullet_paragraph
{
	font-size: 18px;
}

ul.ps_bullet_paragraph
{
	font-size: 18px;
}

.ps_indent_section_paragraph
{
	font-size: 18px;
}

ul.ps_indent_section_paragraph
{
	font-size: 18px;
}

.ps_table_text_left
{
	font-size: 15px;
}

.ps_table_text_right
{
	font-size: 15px;
}

.ps_table_text_center
{
	font-size: 15px;
}

.ps_table_text_full
{
	font-size: 15px;
}

.ps_table_title
{
	font-size: 18px;
}

.ps_table_title_center
{
	font-size: 18px;
}

/* set all the following table cell content to 15px font */
div.ps_table_center table td, div.ps_table_center table th
{font-size: 15px;}

div.ps_table_left table td, div.ps_table_left table th
{font-size: 15px;}

div.ps_table_right table td, div.ps_table_right table th
{font-size: 15px;}

div.ps_table_blockquote table td, div.ps_table_blockquote table th
{font-size: 15px;}

div.ps_table_landscape table td, div.ps_table_landscape table th
{font-size: 15px;}

div.ps_indent table td, div.ps_indent table th
{font-size: 15px;}

div.ps_indent_0 table td, div.ps_indent_0 table th
{font-size: 15px;}

div.ps_indent_1 table td, div.ps_indent_1 table th
{font-size: 15px;}

div.ps_indent_2 table td, div.ps_indent_2 table th
{font-size: 15px;}

div.ps_indent_3 table td, div.ps_indent_3 table th
{font-size: 15px;}

div.ps_indent_4 table td, div.ps_indent_4 table th
{font-size: 15px;}

div.ps_indent_5 table td, div.ps_indent_5 table th
{font-size: 15px;}

div.ps_indent_6 table td, div.ps_indent_6 table th
{font-size: 15px;}

div.ps_indent_7 table td, div.ps_indent_7 table th
{font-size: 15px;}

div.ps_indent_8 table td, div.ps_indent_8 table th
{font-size: 15px;}

div.ps_indent_9 table td, div.ps_indent_9 table th
{font-size: 15px;}


/* set all the following table cell content to 18px font */
div.ps_table_center_normal table td, div.ps_table_center_normal table th
{font-size: 18px;}

div.ps_table_left_normal table td, div.ps_table_left_normal table th
{font-size: 18px;}

div.ps_table_right_normal table td, div.ps_table_right_normal table th
{font-size: 18px;}

div.ps_table_blockquote_normal table td, div.ps_table_blockquote_normal table th
{font-size: 18px;}

div.ps_table_landscape_normal table td, div.ps_table_landscape_normal table th
{font-size: 18px;}

div.ps_indent_normal table td, div.ps_indent_normal table th
{font-size: 18px;}

div.ps_indent_0_normal table td, div.ps_indent_0_normal table th
{font-size: 18px;}

div.ps_indent_1_normal table td, div.ps_indent_1_normal table th
{font-size: 18px;}

div.ps_indent_2_normal table td, div.ps_indent_2_normal table th
{font-size: 18px;}

div.ps_indent_3_normal table td, div.ps_indent_3_normal table th
{font-size: 18px;}

div.ps_indent_4_normal table td, div.ps_indent_4_normal table th
{font-size: 18px;}

div.ps_indent_5_normal table td, div.ps_indent_5_normal table th
{font-size: 18px;}

div.ps_indent_6_normal table td, div.ps_indent_6_normal table th
{font-size: 18px;}

div.ps_indent_7_normal table td, div.ps_indent_7_normal table th
{font-size: 18px;}

div.ps_indent_8_normal table td, div.ps_indent_8_normal table th
{font-size: 18px;}

div.ps_indent_9_normal table td, div.ps_indent_9_normal table th
{font-size: 18px;}

/* ------------------ Common LTR ------------------ */

.topictitle1 { font-size: 21px; }
.topictitle2 { font-size: 18px; }
.topictitle3 { font-size: 18px; }
.topictitle4 { font-size: 18px; }
.topictitle5 { font-size: 18px; }
.topictitle6 { font-size: 18px; }
.sectiontitle { font-size: 18px; }


/* ------------------ FASB ------------------ */

div.navheader *, div.navfooter *
{
font-size:15px;
}

.ps_cod_secondary_level, .ps_fasb_cod_secondary_level
{
font-size:16px;
}

.ps_cod_tertiary_level, .ps_fasb_cod_tertiary_level
{
font-size:16px;
}

.ps_cod_fourth_level, .ps_fasb_cod_fourth_level
{
font-size:15px;
}

.ps_cod_fifth_level, .ps_fasb_cod_fifth_level
{
font-size:15px;
}

.ps_level2, .ps_fasb_level2
{
font-size:16px;
}

.ps_subtitle, .ps_fasb_subtitle
{
font-size:20px;
}

.ps_tertiary_heading, .ps_fasb_tertiary_heading
{
font-size:20px;
}

.ps_toc_entry, .ps_fasb_toc_entry
{

}

.ps_toc_title, .ps_fasb_toc_title
{
font-size:18px;
}

.ps_document_type, .ps_fasb_document_type
{
font-size:30px;
}

.ps_document_type_sub, .ps_fasb_document_type_sub
{
font-size:12px;
}

.ps_title, .ps_fasb_title
{
font-size:20px;
}

.ps_fourth_level, .ps_fasb_fourth_level
{
font-size:16px;
}

.ps_fifth_level, .ps_fasb_fifth_level
{
font-size:15px;
}

.ps_sixth_level, .ps_fasb_sixth_level
{
font-size:15px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:13px;
}

.ps_eighth_level, .ps_fasb_eighth_level
{
font-size:13px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:13px;
}

.ps_fourth_level-type_heading, .ps_fasb_fourth_level-type_heading
{
font-size:16px;
}

.ps_quoted_footnote, .ps_fasb_quoted_footnote
{
font-size:15px;
}

.ps_fifth_heading, .ps_fasb_fifth_heading
{
font-size:15px;
}

.ps_fourth_heading, .ps_fasb_fourth_heading
{
font-size:16px;
}

.ps_volume_title, .ps_fasb_volume_title
{
font-size:45px;
}

.ps_sixth_heading, .ps_fasb_sixth_heading
{
font-size:13px;
}

.ps_section_title, .ps_fasb_section_title
{
font-size:13px;
}

.ps_third_level_para_bullet_list, .ps_fasb_third_level_para_bullet_list
{
font-size:18px;
}

.ps_fourth_level_para_bullet_list, .ps_fasb_fourth_level_para_bullet_list
{
font-size:18px;
}

.ps_index_primary_level, .ps_fasb_index_primary_level
{
font-size:15px;
}

.ps_index_heading, .ps_fasb_index_heading
{
font-size:15px;
}

.ps_index_entry, .ps_fasb_index_entry
{
font-size:15px;
}

.ps_index_entry_2, .ps_fasb_index_entry_2
{
font-size:15px;
}

.ps_index_entry_3, .ps_fasb_index_entry_3
{
font-size:15px;
}

.ps_index_entry_4, .ps_fasb_index_entry_4
{
font-size:15px;
}

.ps_alpha_menu, .ps_fasb_alpha_menu
{
font-size:13px;
}

.ps_normal_level-variation8, .ps_fasb_normal_level-variation8
{
font-size:18px;
}

/* ------------------ GASB ------------------ */

.ps_copyright_notice
{
font-size:15px;
}

/* ------------------ ASC ------------------ */

.pending-text-title 
{
	
    font-size:18px;
}
.pending-text span {font-size:15px; }

.secTitle
{
	font-size:13px;
}


.prev { 

}
.next { 	

}
.prevdisabled { }
.nextdisabled { }

.required { font-size: 20px; }

.asc_heading {
	font-size:13px; 
}

.asc_pgroup { 
	font-size:20px; 
}

.section_content #section_wrapper h1 {  
	font-size:13px; 
}

.section_content #section_wrapper h2 { 
	font-size:18px; 
}

.section_content #section_wrapper h3 { 
	font-size:15px; 
}

#fasb_info {
	font-size:16px; 
	
}

.general_note {
		 
}


";
        #endregion

        #region xlargeCSS (4.css)
        private const string xlargeCSS = @"
/* 4.css - xlargeCSS */
* 
{
	
}

.leftcol_content p {
	
	font-size:16px;
}
h1,h2,h3,h4,h5,h6
{

font-size:20px;
}
a 
{
     
    font-size:inherit;
}
.italic {}
.citetitle {font-size:inherit; }
span {font-size:inherit; }
.unicode_symbol {font-family: arial unicode ms;}

/* --------------------- Main/PS --------------------- */

div.preface * .ps_center_alpha, div.chapter * .ps_center_alpha, div.section * .ps_center_alpha, div.appendix * .ps_center_alpha, div.glossary * .ps_center_alpha
{
	font-size:16px;
}

div.navheader *, div.navfooter *
{
	
	font-size:18px;
}

div.preface * .ps_section_heading, div.chapter * .ps_section_heading, div.section * .ps_section_heading, div.appendix * .ps_section_heading, div.glossary * .ps_section_heading
{
	font-size: 28px;
}

div.preface * .ps_tab_heading, div.chapter * .ps_tab_heading, div.section * .ps_tab_heading, div.appendix * .ps_tab_heading, div.glossary * .ps_tab_heading
{
	font-size:28px;
}

div.toc *
{
	font-size:21px;
}

div.preface * .ps_level_2, div.chapter * .ps_level_2, div.section * .ps_level_2, div.appendix * .ps_level_2, div.glossary * .ps_level_2
{
	font-size:30px;
}

div.preface * .ps_tab_heading_index, div.chapter * .ps_tab_heading_index, div.section * .ps_tab_heading_index, div.appendix * .ps_tab_heading_index, div.glossary * .ps_tab_heading_index
{
	font-size:28px;
}


.footnote, .footnote a, .footnote p, .footnote p a, .footnote span
{
	font-size: 15px;
}

.ls_level_3
{
	font-size:16px;	
}

.ls_level_4
{
	font-size: 16px;

}

.ps_box_-_guide
{
	font-size: 18px;
}

.ps_box_-_guide_center
{
	font-size: 18px;
}

.ps_bullet
{
	font-size: 21px;
}

ul.ps_bullet
{
	font-size: 21px;
}

.ps_bullet_subparagraph
{
	font-size: 21px;
}

ul.ps_bullet_subparagraph
{
	font-size: 21px;
}

.ps_full
{
	font-size: 21px;
}

.ps_definition
{
	font-size: 21px;
}

.ps_left_text
{
	font-size: 21px;
}

.ps_main_heading {
	font-size: 19px;}

.ps_main_heading_center
{
	font-size: 19px;
}

.ps_para_number
{
	font-size: 16px;
}

.ps_para_number_full
{
	font-size: 16px;
}

.ps_para_number_center
{
	font-size: 16px;
}

.ps_para_number_left
{
	font-size: 16px;
}

.ps_para_number_right
{
	font-size: 16px;
}

.ps_paragraph
{
	font-size: 16px;
}

.ps_paragraph_center
{
	font-size: 16px;
}

.ps_paragraph_full
{
	font-size: 16px;
}

.ps_paragraph_left
{
	font-size: 16px;
}

.ps_paragraph_right
{
	font-size: 16px;
}

.ps_left_10
{
	font-size: 16px;
}

.ps_center_10
{
	font-size: 16px;
}

.ps_right_10
{
	font-size: 16px;
}

.ps_right_line
{
	font-size: 16px;
}

.ps_section_title
{
	font-size: 15px;
}

.ps_sub_heading
{
	font-size: 21px;
}

.ps_sub_heading_right
{
	font-size: 21px;
}

.ps_sub_heading_center
{
	font-size: 21px;
}

.ps_sub_heading_full
{
	font-size: 21px;
}

.ps_sub_heading_left
{
	font-size: 21px;
}

.ps_sub_heading_right
{
	font-size: 21px;
}

.ps_subparagraph
{
	font-size: 21px;
}

ul.ps_subparagraph
{
	font-size: 21px;
}

.ps_subsub_heading
{
	font-size: 18px;
}

.ps_subsub_heading_center
{
	font-size: 18px;
}

.ps_subsubsub_heading
{
	font-size: 18px;
}

.ps_tab_name
{
	font-size: 15px;
}

.ps_bullet_paragraph
{
	font-size: 21px;
}

ul.ps_bullet_paragraph
{
	font-size: 21px;
}

.ps_indent_section_paragraph
{
	font-size: 21px;
}

ul.ps_indent_section_paragraph
{
	font-size: 21px;
}

.ps_table_text_left
{
	font-size: 18px;
}

.ps_table_text_right
{
	font-size: 18px;
}

.ps_table_text_center
{
	font-size: 18px;
}

.ps_table_text_full
{
	font-size: 18px;
}

.ps_table_title
{
	font-size: 21px;
}

.ps_table_title_center
{
	font-size: 21px;
}

/* set all the following table cell content to 18px font */
div.ps_table_center table td, div.ps_table_center table th
{font-size: 18px;}

div.ps_table_left table td, div.ps_table_left table th
{font-size: 18px;}

div.ps_table_right table td, div.ps_table_right table th
{font-size: 18px;}

div.ps_table_blockquote table td, div.ps_table_blockquote table th
{font-size: 18px;}

div.ps_table_landscape table td, div.ps_table_landscape table th
{font-size: 18px;}

div.ps_indent table td, div.ps_indent table th
{font-size: 18px;}

div.ps_indent_0 table td, div.ps_indent_0 table th
{font-size: 18px;}

div.ps_indent_1 table td, div.ps_indent_1 table th
{font-size: 18px;}

div.ps_indent_2 table td, div.ps_indent_2 table th
{font-size: 18px;}

div.ps_indent_3 table td, div.ps_indent_3 table th
{font-size: 18px;}

div.ps_indent_4 table td, div.ps_indent_4 table th
{font-size: 18px;}

div.ps_indent_5 table td, div.ps_indent_5 table th
{font-size: 18px;}

div.ps_indent_6 table td, div.ps_indent_6 table th
{font-size: 18px;}

div.ps_indent_7 table td, div.ps_indent_7 table th
{font-size: 18px;}

div.ps_indent_8 table td, div.ps_indent_8 table th
{font-size: 18px;}

div.ps_indent_9 table td, div.ps_indent_9 table th
{font-size: 18px;}


/* set all the following table cell content to 21px font */
div.ps_table_center_normal table td, div.ps_table_center_normal table th
{font-size: 21px;}

div.ps_table_left_normal table td, div.ps_table_left_normal table th
{font-size: 21px;}

div.ps_table_right_normal table td, div.ps_table_right_normal table th
{font-size: 21px;}

div.ps_table_blockquote_normal table td, div.ps_table_blockquote_normal table th
{font-size: 21px;}

div.ps_table_landscape_normal table td, div.ps_table_landscape_normal table th
{font-size: 21px;}

div.ps_indent_normal table td, div.ps_indent_normal table th
{font-size: 21px;}

div.ps_indent_0_normal table td, div.ps_indent_0_normal table th
{font-size: 21px;}

div.ps_indent_1_normal table td, div.ps_indent_1_normal table th
{font-size: 21px;}

div.ps_indent_2_normal table td, div.ps_indent_2_normal table th
{font-size: 21px;}

div.ps_indent_3_normal table td, div.ps_indent_3_normal table th
{font-size: 21px;}

div.ps_indent_4_normal table td, div.ps_indent_4_normal table th
{font-size: 21px;}

div.ps_indent_5_normal table td, div.ps_indent_5_normal table th
{font-size: 21px;}

div.ps_indent_6_normal table td, div.ps_indent_6_normal table th
{font-size: 21px;}

div.ps_indent_7_normal table td, div.ps_indent_7_normal table th
{font-size: 21px;}

div.ps_indent_8_normal table td, div.ps_indent_8_normal table th
{font-size: 21px;}

div.ps_indent_9_normal table td, div.ps_indent_9_normal table th
{font-size: 21px;}

/* ------------------ Common LTR ------------------ */

.topictitle1 { font-size: 19px; }
.topictitle2 { font-size: 21px; }
.topictitle3 { font-size: 21px; }
.topictitle4 { font-size: 21px; }
.topictitle5 { font-size: 21px; }
.topictitle6 { font-size: 21px; }
.sectiontitle { font-size: 21px; }


/* ------------------ FASB ------------------ */

div.navheader *, div.navfooter *
{
font-size:18px;
}

.ps_cod_secondary_level, .ps_fasb_cod_secondary_level
{
font-size:20px;
}

.ps_cod_tertiary_level, .ps_fasb_cod_tertiary_level
{
font-size:20px;
}

.ps_cod_fourth_level, .ps_fasb_cod_fourth_level
{
font-size:18px;
}

.ps_cod_fifth_level, .ps_fasb_cod_fifth_level
{
font-size:18px;
}

.ps_level2, .ps_fasb_level2
{
font-size:20px;
}

.ps_subtitle, .ps_fasb_subtitle
{
font-size:20px;
}

.ps_tertiary_heading, .ps_fasb_tertiary_heading
{
font-size:20px;
}

.ps_toc_entry, .ps_fasb_toc_entry
{

}

.ps_toc_title, .ps_fasb_toc_title
{
font-size:21px;
}

.ps_document_type, .ps_fasb_document_type
{
font-size:34px;
}

.ps_document_type_sub, .ps_fasb_document_type_sub
{
font-size:28px;
}

.ps_title, .ps_fasb_title
{
font-size:20px;
}

.ps_fourth_level, .ps_fasb_fourth_level
{
font-size:20px;
}

.ps_fifth_level, .ps_fasb_fifth_level
{
font-size:18px;
}

.ps_sixth_level, .ps_fasb_sixth_level
{
font-size:18px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:16px;
}

.ps_eighth_level, .ps_fasb_eighth_level
{
font-size:16px;
}

.ps_seventh_level, .ps_fasb_seventh_level
{
font-size:16px;
}

.ps_fourth_level-type_heading, .ps_fasb_fourth_level-type_heading
{
font-size:20px;
}

.ps_quoted_footnote, .ps_fasb_quoted_footnote
{
font-size:18px;
}

.ps_fifth_heading, .ps_fasb_fifth_heading
{
font-size:18px;
}

.ps_fourth_heading, .ps_fasb_fourth_heading
{
font-size:20px;
}

.ps_volume_title, .ps_fasb_volume_title
{
font-size:45px;
}

.ps_sixth_heading, .ps_fasb_sixth_heading
{
font-size:16px;
}

.ps_section_title, .ps_fasb_section_title
{
font-size:15px;
}

.ps_third_level_para_bullet_list, .ps_fasb_third_level_para_bullet_list
{
font-size:21px;
}

.ps_fourth_level_para_bullet_list, .ps_fasb_fourth_level_para_bullet_list
{
font-size:21px;
}

.ps_index_primary_level, .ps_fasb_index_primary_level
{
font-size:18px;
}

.ps_index_heading, .ps_fasb_index_heading
{
font-size:18px;
}

.ps_index_entry, .ps_fasb_index_entry
{
font-size:18px;
}

.ps_index_entry_2, .ps_fasb_index_entry_2
{
font-size:18px;
}

.ps_index_entry_3, .ps_fasb_index_entry_3
{
font-size:18px;
}

.ps_index_entry_4, .ps_fasb_index_entry_4
{
font-size:18px;
}

.ps_alpha_menu, .ps_fasb_alpha_menu
{
font-size:15px;
}

.ps_normal_level-variation8, .ps_fasb_normal_level-variation8
{
font-size:21px;
}

/* ------------------ GASB ------------------ */

.ps_copyright_notice
{
font-size:18px;
}

/* ------------------ ASC ------------------ */

.pending-text-title 
{
	
    font-size:20px;
}
.pending-text span {font-size:16px; }

.secTitle
{
	font-size:15px;
}


.prev { 

}
.next { 	

}
.prevdisabled { }
.nextdisabled { }

.required { font-size: 20px; }

.asc_heading {
	font-size:15px; 
}

.asc_pgroup { 
	font-size:20px; 
}

.section_content #section_wrapper h1 {  
	font-size:15px; 
}

.section_content #section_wrapper h2 { 
	font-size:21px; 
}

.section_content #section_wrapper h3 { 
	font-size:18px; 
}

#fasb_info {
	font-size:20px; 
	
}

.general_note {
		 
}


";
        #endregion
    }
}