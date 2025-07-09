using System;
using System.Web.UI;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Search;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;
using D_ODPPortalUI.Shared;

namespace D_ODPPortalUI
{
    public partial class ViewDocument : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDocument();

            if (FadeIn)
            {
                AddFadeInScript();
            }
        }

        private void LoadDocument()
        {
            //get the site
            ISite site = DestroyerUi.GetCurrentSite(Page);

            //get the book name, doc name, and formats parameters
            IDocument doc = Document;
            IBook book = (doc != null) ? doc.Book : null;
            IDocumentFormat format = doc.PrimaryFormat;
            bool hilite = ShowHitHighlights;

            //make sure the document is in our subscription
            if (doc.InSubscription)
            {
                //see if we have a current search result object
                var searchResults = (ISearchResults) Session[DestroyerUi.SESSPARAM_CURRENTSEARCHRESULTS];

                if (format != null)
                {
                    // Serve up the file by name
                    Response.AppendHeader("content-disposition", "filename=" + doc.Name);

                    // set the content type for the Response to that of the 
                    // document to display.  For example. "application/msword"
                    Response.ContentType = format.Description;

                    if (format.Description == "application/pdf")
                    {
                        Response.WriteFile(format.Uri);
                        Response.End();
                        return;
                    }

                    //set our bytes to be highlighted or not
                    byte[] contentBytes = null;
                    if (hilite && searchResults != null && format.ContentTypeId == (int) ContentType.TextHtml &&
                        searchResults.WordInterpretations != null && searchResults.WordInterpretations.Length > 0)
                    {
                        contentBytes = format.GetHighlightedContent(searchResults.WordInterpretations,
                                                                    DestroyerUi.HILITE_BEGINTAG,
                                                                    DestroyerUi.HILITE_ENDTAG);
                    }
                    else
                    {
                        contentBytes = format.Content;
                    }

                    string contentText = DestroyerBpc.ByteArrayToStr(contentBytes);

                    // TODO: Deal with Print Document / View Sources Issues

                    //// If we're suppossed to show sources, add that stylesheet in the content
                    //if ((string)Session[DestroyerUi.SESSPARAM_SHOWHIDESOURCE] != string.Empty)
                    //{
                    //    contentText = D_PrintDocument.IncludeCodificationStyleSheets(contentText, true);
                    //}

                    //// If it's an old FASB or archived book, then add the "this document is not current" notice
                    //if (D_PrintDocument.IsDocumentNotCurrent(book.Name))
                    //{
                    //    contentText = D_PrintDocument.IncludeDocumentNotCurrentNotice(contentText, true);
                    //}

                    // Write to response stream and end the response

                    litContent.Text = contentText;

                    //Response.Write(contentText);
                    //Response.End();
                }
            }
            else
            {
                Response.Redirect("D_ViewDocumentNotAuthorized.aspx", true);
            }
        }

        private void AddFadeInScript()
        {
            string divId = "divViewDocumentContainer";

            string script = string.Format("$('#{0}').hide().fadeIn();", divId);
            ScriptManager.RegisterStartupScript(this, typeof (ViewDocument), "content_ajax_key", script, true);
        }

        #region Basic Properties

        public const int NO_DOCUMENT_ID = int.MinValue;
        private IDocument document = null;
        private string documentAnchor = string.Empty;

        private int documentId = NO_DOCUMENT_ID;
        private bool fadeIn = false;
        private bool showHitHighlights = false;

        public int DocumentId
        {
            get { return documentId; }
            set
            {
                documentId = value;
                document = null;
            }
        }

        private bool HasDocumentId
        {
            get { return DocumentId != NO_DOCUMENT_ID; }
        }

        public IDocument Document
        {
            get
            {
                if (document == null)
                {
                    if (HasDocumentId)
                    {
                        document = new Document(DocumentId);
                    }
                }

                return document;
            }
            set
            {
                document = value;
                documentId = value.Id;
            }
        }

        /// <summary>
        /// Optional: Specifies the document anchor to jump to
        /// </summary>
        public string DocumentAnchor
        {
            get { return documentAnchor; }
            set { documentAnchor = value; }
        }

        public bool ShowHitHighlights
        {
            get { return showHitHighlights; }
            set { showHitHighlights = value; }
        }

        public bool FadeIn
        {
            get { return fadeIn; }
            set { fadeIn = true; }
        }

        #endregion
    }
}