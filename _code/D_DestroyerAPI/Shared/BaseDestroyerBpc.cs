#region

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AICPA.Destroyer.Content;
using System.ComponentModel;

#endregion

namespace AICPA.Destroyer.Shared
{
    /// <summary>
    ///   Build status for sites
    /// </summary>
    public enum SiteBuildStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        NotBuilt = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Built = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BuildRequested = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Building = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Error = 4
    }

    /// <summary>
    ///   Build status for site indexes
    /// </summary>
    public enum SiteIndexBuildStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        NotBuilt = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Built = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BuildRequested = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Building = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Error = 4,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BuiltWithWarnings = 5
    }

    /// <summary>
    ///   Build status for books
    /// </summary>
    public enum BookBuildStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        NotBuilt = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Built = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BuildRequested = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Building = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Error = 4
    }

    /// <summary>
    ///   enum that defines the possible status of a site.
    /// </summary>
    public enum SiteStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Null = -1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Unassigned = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Staging = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        PreProduction = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Production = 3
    }

    /// <summary>
    ///   enum that defines the possible status of a site index.
    /// </summary>
    public enum SiteIndexStatus
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Ready = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Updating = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Error = 2
    }

    /// <summary>
    ///   enum that defines supported content types
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Unassigned = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("Word")]
        ApplicationMsword = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("PDF")]
        ApplicationPdf = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("RTF")]
        ApplicationRtf = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("PalmOS")]
        ApplicationVndPalm = 4,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("ZIP")]
        ApplicationZip = 5,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("BMP")]
        ImageBmp = 6,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("GIF")]
        ImageGif = 7,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("JPEG")]
        ImageJpeg = 8,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("PNG")]
        ImagePng = 9,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("TIFF")]
        ImageTiff = 10,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("CSS")]
        TextCss = 11,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("HTML")]
        TextHtml = 12,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("Plain")]        
        TextPlain = 13,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("Rich Text")]
        TextRichtext = 14,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("XML")]
        TextXml = 15,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("Excel")]
        ApplicationVndMsExcel = 16,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("What Links Here")]
        TextWlh = 17,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("Archive")]
        TextArch = 18,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        [Description("PowerPoint")]
        ApplicationVndMsPowerpoint = 19
    }

    /// <summary>
    ///   enum that defines the supported toc node types
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Site = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Book = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Document = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Format = 4,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BookFolder = 5,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        DocumentAnchor = 6,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        SiteFolder = 7
    }

    /// <summary>
    ///   enum that defines the possible source types of a book.
    /// </summary>
    public enum BookSourceType
    {
        /// <summary>
        ///   The book is generated from a makefile
        /// </summary>
        Makefile = 1,
        /// <summary>
        ///   The book is generated from a content management system
        /// </summary>
        Cms = 2
    }

    /// <summary>
    /// </summary>
    public enum SiteSortField
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Name = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Title = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        SiteVersion = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        SiteStatusCode = 4
    }

    /// <summary>
    /// </summary>
    public enum BookSortField
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Left = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Name = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        Title = 2,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        PublishDate = 3,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        BookVersion = 4
    }

    /// <summary>
    ///   Specifies the prefered type of search.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        ///   All words from the search text are to be used to determine hit documents (AND).
        /// </summary>
        AllWords = 1,
        /// <summary>
        ///   Any words from the search text are to be used to determine hit documents (OR).
        /// </summary>
        AnyWords = 2,
        /// <summary>
        ///   Exact Phrase
        /// </summary>
        ExactPhrase = 3,
        /// <summary>
        ///   The search text represent a boolean query in the syntax supported by the search engine.
        /// </summary>
        Boolean = 4
    }

    /// <summary>
    ///   Summary description for EventLogConfigurationHandler.
    /// </summary>
    public class XmlNodeConfigurationHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        /// <summary>
        /// Creates the specified parent.	
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The config context.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }

        #endregion
    }

    ///<summary>
    ///  A base class that all destroyer classes inherit.
    ///</summary>
    public class DestroyerBpc
    {
        #region Constants

        //Error Severity settings.
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_EXTERNAL_SERVICE = 1;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_DATALAYER_ERROR = 1;

        public const int ERROR_SEVERITY_AJAX_FAILED = 1;
        public const int INFO_PRINT_TO_PDF = 3;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_BOOKBUILD_ERROR = 5;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_SITEBUILD_ERROR = 5;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int ERROR_SEVERITY_SITEINDEXBUILD_ERROR = 5;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int USAGE_SEVERITY_SUCCESSFUL_LOGON = 3;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int USAGE_SEVERITY_SUCCESSFUL_LOGOFF = 3;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected const int USAGE_SEVERITY_DOCUMENT_ACCESSED = 5;

        //module names (for logging)
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string MODULE_BOOK = "Book";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string MODULE_SITE = "Site";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string MODULE_SITEINDEX = "SiteIndex";

        public const string MODULE_WEBSERVICES = "WebServices";

        //method names (for logging)
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string METHOD_BOOKBUILD = "BookBuild";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string METHOD_SITEBUILD = "SiteBuild";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string METHOD_SITEINDEXBUILD = "SiteIndexBuild";

        //Errors

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ERROR_INVALIDCONTENTTYPE = "The content type of '{0}' is not supported.";

        //URI constants
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string URI_FILEPROTOCOLPREFIX = "file:///";

        //reference path constants
        //keep in mind that you should not change these constants unless you first change the database stored procedure that constructs the reference path
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const int REFERENCEPATH_NODECOMPONENTCOUNT = 4;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char REFERENCEPATH_NODECOMPONENTSEPCHAR = '~';
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char REFERENCEPATH_NODESEPCHAR = ';';

        //domain constants
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char DOMAIN_SUBSCRIPTIONCODESEPCHAR = '~';

        //xml elements
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_MAKEFILE = "Makefile";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SITE = "Site";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SITEFOLDER = "SiteFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_BOOK = "Book";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_BOOKFOLDER = "BookFolder";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_DOCUMENT = "Document";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_DOCUMENTFORMAT = "DocumentFormat";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_DOCUMENTANCHOR = "DocumentAnchor";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_RESOURCES = "Resources";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_RESOURCE = "Resource";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTS = "SearchResults";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSSUMMARY = "Summary";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSTOTALHITS = "TotalHits";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSPAGEHITS = "PageHits";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSPAGEOFFSET = "PageOffset";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSRECORDS = "Records";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSRECORD = "Record";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSPARENTBOOK = "ParentBook";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSPARENTBOOKNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSPARENTBOOKID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDOCUMENT = "Document";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDOCUMENTNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDOCUMENTID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONS = "Dimensions";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSSELECTEDDIMENSIONS = "SelectedDimensions";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSREFINEMENTDIMENSIONS = "RefinementDimensions";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSION = "Dimension";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONVALUE = "DimensionValue";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONVALUENAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONVALUEID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONVALUERECORDCOUNT = "RecordCount";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONANCESTORS = "DimensionAncestors";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEM = "Dimension";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONANCESTORITEMID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATH = "DimensionCompletePath";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEM = "Dimension";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSDIMENSIONCOMPLETEPATHITEMID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSHIGHLIGHTTERMS = "HighlightTerms";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_SEARCHRESULTSHIGHLIGHTTERM = "HighlightTerm";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ELE_REFERENCEPATH = "ReferencePath";

        // xml attributes
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_ID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_NAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_TITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_HASCHILDREN = "HasChildren";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_HIDDEN = "Hidden";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_URI = "Uri";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_GUID = "Guid";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_LEFT = "Left";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_RIGHT = "Right";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITEID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITENAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITETITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKTITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKDESCRIPTION = "Description";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKCOPYRIGHT = "Copyright";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKFOLDERID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKFOLDERNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKFOLDERTITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_BOOKFOLDERURI = "Uri";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTTITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTANCHORID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTANCHORNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTANCHORTITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTFORMATCONTENTTYPE = "Content-Type";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTFORMATURI = "Uri";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTFORMATPRI = "Primary";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_DOCUMENTFORMATAUTODOWNLOAD = "AutoDownload";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITEFOLDERID = "Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITEFOLDERNAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITEFOLDERTITLE = "Title";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_SITEFOLDERURI = "Uri";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_RESOURCENAME = "Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_RESOURCEURI = "Uri";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_EXTERNAL = "External";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_EXT_BOOK_NAME = "BookName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string XML_ATT_EXT_KEYWORDS = "KeyWordsInContext";


        //endeca-specific  constants
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_SEARCHOPT_MATCHANY = "mode matchany";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_SEARCHOPT_MATCHBOOLEAN = "mode matchboolean";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_SEARCHOPT_MATCHALL = "mode matchall";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DESTROYERSITEHIERARCHY_DIMENSION = "destroyer_site_hierarchy";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char ENDECA_DESTROYERSITEHIERARCHY_SEPCHAR = ':';
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_FULLTEXT_FIELD = "Text";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_BOOKNAME_FIELD = "BookName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTNAME_FIELD = "DocumentName";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_SUBSCRIPTIONCODE_FIELD = "SubscriptionCode";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DIMENSIONID_INITIAL = "0";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_BOOKID_PROPERTY = "Destroyer.Book.Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_BOOKNAME_PROPERTY = "Destroyer.Book.Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTID_PROPERTY = "Destroyer.Document.Id";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTNAME_PROPERTY = "Destroyer.Document.Name";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTTEXTSNIPPET_PROPERTY = "Endeca.Document.Text.Snippet";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTSTATUS_PROPERTY = "Endeca.Document.Status";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_DOCUMENTSTATUSSUCCESS_KEYWORD = "Succeeded";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_KWICTERMMARKUPBEGIN = "<endeca_term>";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_KWICTERMMARKUPEND = "</endeca_term>";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_IDLE_SYSTEMSTATE = "IDLE";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_UPDATING_SYSTEMSTATE = "UPDATING";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_ERROR_SYSTEMSTATE = "SYSTEM_ERROR";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char ENDECA_DIMENSIONIDSEPCHAR = ' ';
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string ENDECA_PIPELINESPIDERURLXPATH = "PIPELINE/SPIDER/SPIDER_INIT/ROOT_URL";

        // other misc. constants
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const int EMPTY_INT = -1;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const bool EMPTY_BOOL = false;

        // db Title constant
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string EXTERNAL_DOCUMENT = "EXTRN";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const char EXTERNAL_DOC_DATA_SEPERATOR = '~';
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_NOTIMPLEMENTED = "Not implemented.";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string ERROR_REFERENCEPATHPARSE =
            "Error parsing reference path from the datastore. It is likely that a node title or node name contains a '{0}' character, which is not allowed. Reference path:\n{1}";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public static string EMPTY_STRING = string.Empty;

        #endregion

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        protected string moduleName = "Unknown";

        /// <summary>
        /// Gets the name of the module.	
        /// </summary>
        /// <value>The name of the module.</value>
        /// <remarks></remarks>
        protected string ModuleName
        {
            get { return moduleName; }
        }

        #region XML Helper Methods

        /// <summary>
        ///   Combines the xml from a book reference path (the path from the site to the book) with the xml
        ///   from a document reference path (the path from a book to a document).
        /// </summary>
        /// <param name = "bookReferencePath">XML from a book reference path (the path from the site to the book)</param>
        /// <param name = "docReferencePath">XML from a document reference path (the path from a book to a document)</param>
        /// <returns>An XML string combining the two reference paths</returns>
        public string CombineReferencePaths(string bookReferencePath, string docReferencePath)
        {
            //initialize our return string
            string retXml = EMPTY_STRING;

            //load the book reference path into an xml document
            XmlDocument bookRefXml = new XmlDocument();
            bookRefXml.LoadXml(bookReferencePath);

            //load the document reference path into an xml document
            XmlDocument docRefXml = new XmlDocument();
            docRefXml.LoadXml(docReferencePath);

            //place all but the book node of the document reference path xml at the end of the child nodes of the book xml
            XmlNodeList docRefXmlNodes = docRefXml.SelectNodes(XML_ELE_REFERENCEPATH + "/*");
            foreach (XmlNode docRefXmlNode in
                docRefXmlNodes.Cast<XmlNode>().Where(docRefXmlNode => docRefXmlNode.LocalName != XML_ELE_BOOK))
            {
                bookRefXml.FirstChild.InnerXml += docRefXmlNode.OuterXml;
            }

            //set our return string and return it
            retXml = bookRefXml.OuterXml;
            return retXml;
        }

        /// <summary>
        ///   Takes a reference path from the database in the form of "nodeId1~name1~title1~nodeTypeId1;nodeId2~name2~title2~nodeTypeId2;" and
        ///   converts it into a more easily parsed xml string
        /// </summary>
        /// <param name = "refPath"></param>
        /// <returns>A string containing the resulting XML.</returns>
        public string ReferencePathToXml(string refPath)
        {
            //set our return string to empty
            string retString = EMPTY_STRING;

            //semicolons cause problems, so we do this replacement to fix things
            refPath = refPath.Replace("&amp;", "&");

            //create an xml document and insert its root node
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootEle = xmlDoc.CreateElement(XML_ELE_REFERENCEPATH);

            //now parse the db ref path to build our xml from it
            string[] nodes = refPath.Split(REFERENCEPATH_NODESEPCHAR);
            foreach (string[] nodeComponents in
                from node in nodes where node != EMPTY_STRING select node.Split(REFERENCEPATH_NODECOMPONENTSEPCHAR))
            {
                if (nodeComponents.Length != REFERENCEPATH_NODECOMPONENTCOUNT)
                {
                    throw new Exception(string.Format(ERROR_REFERENCEPATHPARSE, REFERENCEPATH_NODECOMPONENTSEPCHAR,
                                                      refPath));
                }

                //these are our node components		'
                string nodestr = nodeComponents[0];
                int nodeId = int.Parse(nodestr);
                string nodeName = nodeComponents[1];
                string nodeTitle = nodeComponents[2];
                int nodeTypeId = int.Parse(nodeComponents[3]);
                string nodeType = Enum.GetName(typeof (NodeType), nodeTypeId);

                //create our node element, named according to our node type
                XmlElement referencePathEle = xmlDoc.CreateElement(nodeType);

                //create and insert the id attribute
                XmlAttribute xmlIdAtt = xmlDoc.CreateAttribute(XML_ATT_ID);
                xmlIdAtt.Value = nodeId.ToString();
                referencePathEle.Attributes.Append(xmlIdAtt);

                //create and insert the name attribute
                XmlAttribute xmlNameAtt = xmlDoc.CreateAttribute(XML_ATT_NAME);
                xmlNameAtt.Value = nodeName;
                referencePathEle.Attributes.Append(xmlNameAtt);

                //create and insert the title attribute
                XmlAttribute xmlTitleAtt = xmlDoc.CreateAttribute(XML_ATT_TITLE);
                xmlTitleAtt.Value = nodeTitle;
                referencePathEle.Attributes.Append(xmlTitleAtt);

                //append the node to our root node
                rootEle.AppendChild(referencePathEle);
            }

            //append the root node to the document
            xmlDoc.AppendChild(rootEle);

            //return our xml as a string
            retString = xmlDoc.OuterXml;
            return retString;
        }

        /// <summary>
        ///   Helper method for rendering a collection of toc nodes as an xml document
        /// </summary>
        /// <param name = "tocNodes">A collection of toc nodes</param>
        /// <param name = "siteXmlOptimizedMode">Enables an optimization for site xml retrieval that requires no gaps in left-right values (content cannot be filtered when using this mode)</param>
        /// <returns>An xml representation of the toc node collection</returns>
        public string GetXmlFromTocNodes(ITocNodeCollection tocNodes, bool siteXmlOptimizedMode, bool ignoreAnchors = false)
        {
            //our xml string to return
            string retXml = string.Empty;

            if (siteXmlOptimizedMode)
            {
                MemoryStream memStream = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.Indentation = 4;
                xmlTextWriter.WriteStartDocument(true);

                for (int i = 0; i < tocNodes.Count; i++)
                {
                    ITocNode currNode = tocNodes[i];
                    ITocNode prevNode = (i > 0) ? tocNodes[i - 1] : null;

                    if (prevNode != null)
                    {
                        //write out the same number of end tags as there are gaps between our left and our previous node's left
                        int leftGap = (currNode.Left - prevNode.Left) - 1;
                        for (int j = 0; j < leftGap; j++)
                        {
                            xmlTextWriter.WriteEndElement();
                        }
                    }

                    //write out our element
                    xmlTextWriter.WriteStartElement(currNode.NodeType.ToString());

                    //get the element's name and add it as an attribute			
                    xmlTextWriter.WriteAttributeString(XML_ATT_NAME, currNode.Name);

                    //get the element's title and add it as an attribute
                    xmlTextWriter.WriteAttributeString(XML_ATT_TITLE, currNode.Title);

                    //get the element's nodeid and add it as an attribute
                    xmlTextWriter.WriteAttributeString(XML_ATT_ID, currNode.NodeId.ToString());

                    //add the haschildren attribute
                    xmlTextWriter.WriteAttributeString(XML_ATT_HASCHILDREN, currNode.HasChildren.ToString());

                    //add the hidden attribute
                    xmlTextWriter.WriteAttributeString(XML_ATT_HIDDEN, currNode.Hidden.ToString());

                    //get the element's uri and add it as an attribute
                    if (currNode.NodeType == NodeType.SiteFolder || currNode.NodeType == NodeType.BookFolder)
                    {
                        if (currNode.Uri != string.Empty)
                        {
                            xmlTextWriter.WriteAttributeString(XML_ATT_URI, currNode.Uri);
                        }
                    }

                    //add a random attribute (used for workaround on endeca site build)
                    xmlTextWriter.WriteAttributeString(XML_ATT_GUID, Guid.NewGuid().ToString());
                }

                //end the document
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();

                StreamReader streamReader = new StreamReader(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                retXml = streamReader.ReadToEnd();

                //close the xml text writer
                xmlTextWriter.Close();
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                bool skipAnchors = false;
                foreach (ITocNode node in tocNodes)
                {
                    int tocLeft = node.Left;
                    int tocRight = node.Right;
                    bool hasChildren = node.HasChildren;
                    bool hidden = node.Hidden;
                    string uri = node.Uri;
                    string name = node.Name;
                    string title = node.Title;
                    string nodetype = node.NodeType.ToString();
                    string nodeid = node.NodeId.ToString(); 


                    //Temp Solution
                    if ((node.NodeType == NodeType.Document) && (!hasChildren))
                    {
                        skipAnchors = true && ignoreAnchors; // only returns true if ignoreAnchors is set to true
                    }
                    else if ((node.NodeType == NodeType.Document) && (hasChildren))
                    {
                        skipAnchors = false;
                    }
                    if ((node.NodeType == NodeType.DocumentAnchor) && (skipAnchors))
                        continue;

                    //this is good for debugging xml serialization of site xml
                    //Console.WriteLine(string.Format("{0} ({1})", name, nodetype));

                    //create our element
                    XmlElement xmlElement = xmlDoc.CreateElement(nodetype);

                    //get the element's name and add it as an attribute
                    XmlAttribute xmlNameAtt = xmlDoc.CreateAttribute(XML_ATT_NAME);
                    xmlNameAtt.Value = name;
                    xmlElement.Attributes.Append(xmlNameAtt);

                    //get the element's title and add it as an attribute
                    XmlAttribute xmlTitleAtt = xmlDoc.CreateAttribute(XML_ATT_TITLE);
                    xmlTitleAtt.Value = title;
                    xmlElement.Attributes.Append(xmlTitleAtt);

                    //get the element's nodeid and add it as an attribute
                    XmlAttribute xmlNodeIdAtt = xmlDoc.CreateAttribute(XML_ATT_ID);
                    xmlNodeIdAtt.Value = nodeid;
                    xmlElement.Attributes.Append(xmlNodeIdAtt);

                    //get the element's left value and add it as an attribute
                    XmlAttribute xmlLeftAtt = xmlDoc.CreateAttribute(XML_ATT_LEFT);
                    xmlLeftAtt.Value = tocLeft.ToString();
                    xmlElement.Attributes.Append(xmlLeftAtt);

                    //get the element's right value and add it as an attribute
                    XmlAttribute xmlRightAtt = xmlDoc.CreateAttribute(XML_ATT_RIGHT);
                    xmlRightAtt.Value = tocRight.ToString();
                    xmlElement.Attributes.Append(xmlRightAtt);

                    //add the haschildren attribute
                    XmlAttribute xmlHasChildrenAtt = xmlDoc.CreateAttribute(XML_ATT_HASCHILDREN);
                    xmlHasChildrenAtt.Value = hasChildren.ToString();
                    xmlElement.Attributes.Append(xmlHasChildrenAtt);

                    //add the hidden attribute
                    XmlAttribute xmlHiddenAtt = xmlDoc.CreateAttribute(XML_ATT_HIDDEN);
                    xmlHiddenAtt.Value = hidden.ToString();
                    xmlElement.Attributes.Append(xmlHiddenAtt);

                    //add the uri attribute
                    if (node.NodeType == NodeType.SiteFolder || node.NodeType == NodeType.BookFolder)
                    {
                        if (uri != string.Empty)
                        {
                            XmlAttribute xmlUriAtt = xmlDoc.CreateAttribute(XML_ATT_URI);
                            xmlUriAtt.Value = uri;
                            xmlElement.Attributes.Append(xmlUriAtt);
                        }
                    }

                    //add a random attribute (used for workaround on endeca site build)
                    XmlAttribute xmlRandomAtt = xmlDoc.CreateAttribute(XML_ATT_GUID);
                    xmlRandomAtt.Value = Guid.NewGuid().ToString();
                    xmlElement.Attributes.Append(xmlRandomAtt);

                    //determine the parent node for our context node
                    //see if there are any nodes that have a left value that is one less than our context node's left value
                    XmlNode parentNode = xmlDoc.SelectSingleNode("//*[@" + XML_ATT_LEFT + " = " + tocLeft + "-1]") ??
                                         (xmlDoc.SelectSingleNode("//*[@" + XML_ATT_RIGHT + " = " + tocLeft +
                                                                  "-1]/parent::*") ?? xmlDoc);
                    //add the element to its parent
                    try
                    {
                        parentNode.AppendChild(xmlElement);
                    }
                    catch
                    { 
                       
                    }
                    
                }
                //remove the left and right attributes
                XmlNodeList nodes = xmlDoc.SelectNodes("//*[@" + XML_ATT_LEFT + " or @" + XML_ATT_RIGHT + "]");
                foreach (XmlNode node in nodes)
                {
                    node.Attributes.RemoveNamedItem(XML_ATT_LEFT);
                    node.Attributes.RemoveNamedItem(XML_ATT_RIGHT);
                }
                retXml = xmlDoc.OuterXml;
            }

            return retXml;
        }

        /// <summary>
        /// </summary>
        /// <param name = "att"></param>
        /// <returns></returns>
        public static string GetAttributeValue(XmlAttribute att)
        {
            return att == null ? "" : att.Value;
        }

        #endregion XML Helper Methods

        /// <summary>
        ///   Translates a BPC domain list into a DALC domain list
        /// </summary>
        /// <param name = "bookList"></param>
        /// <returns></returns>
        public static string TranslateBookDomain(string[] bookList)
        {
            string dalcDomain = "";
            int bookCount = bookList.Length;
            for (int i = 0; i < bookCount; i++)
            {
                dalcDomain += "'" + bookList[i] + "'";
                if (i < bookCount - 1)
                {
                    dalcDomain += ",";
                }
            }
            return dalcDomain;
        }

        /// <summary>
        ///   Converts a string to a byte array
        /// </summary>
        /// <param name = "str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        ///   Converts a byte array to a string
        /// </summary>
        /// <returns></returns>
        public static string ByteArrayToStr(byte[] bytes)
        {
            string retStr = null;
            UTF8Encoding enc = new UTF8Encoding();
            retStr = enc.GetString(bytes);
            return retStr;
        }

        /// <summary>
        ///   Converts a content type description to the appropriate enum value
        /// </summary>
        /// <param name = "contentTypeDesc">A text description of a content type</param>
        /// <returns>An enum of type ContentType.</returns>
        public static ContentType GetContentTypeFromDescription(string contentTypeDesc)
        {
            //initialize to null
            ContentType retContentType = ContentType.Unassigned;

            //get the content type into the same string format as our enum
            string compareContentType = contentTypeDesc;
            compareContentType = compareContentType.Replace("/", "");
            compareContentType = compareContentType.Replace(".", "");
            compareContentType = compareContentType.Replace("-", "");

            //parse to an enum -- if fails, it is not a valid content type
            try
            {
                retContentType = (ContentType) Enum.Parse(typeof (ContentType), compareContentType, true);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(ERROR_INVALIDCONTENTTYPE, contentTypeDesc), e);
            }

            //return our content type
            return retContentType;
        }

        private const string CURRENT_YEAR_REPLACEMENT = "[[current-year]]";
        /// <summary>
        /// Replaces the string "[[current-year]]"
        /// </summary>
        /// <param name="copyright"></param>
        /// <returns></returns>
        public static string ReplaceCopyrightCurrentYear(string copyright)
        {
            string newCopyright = copyright.Replace(CURRENT_YEAR_REPLACEMENT, DateTime.Now.Year.ToString());

            return newCopyright;
        }

        #region File Management Methods

        /// <summary>
        ///   Copies a directory to a new location.
        /// </summary>
        /// <param name = "src">Source directory path</param>
        /// <param name = "dest">Destination directory path</param>
        public static void CopyDirectory(String src, String dest)
        {
            DirectoryInfo di = new DirectoryInfo(src);
            foreach (FileSystemInfo fsi in di.GetFileSystemInfos())
            {
                String destName = Path.Combine(dest, fsi.Name);
                if (fsi is FileInfo)
                    File.Copy(fsi.FullName, destName);
                else
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        #endregion File Management Methods
    }
}