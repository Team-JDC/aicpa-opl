#region

using System.Linq;
using System.Text.RegularExpressions;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.User;

#endregion

namespace AICPA.Destroyer.Content
{
    /// <summary>
    /// 	
    /// </summary>
    public class ContentLink
    {
        #region Properties

        private readonly ISite site;
        private IBook book;

        private string bookName = string.Empty;

        private IDocument document;
        private IDocumentAnchor documentAnchor;
        private string documentAnchorName = string.Empty;

        private string documentName = string.Empty;
        private bool isInSubscription;

        /// <summary>
        /// Gets the book.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IBook Book
        {
            get { return book; }
        }

        /// <summary>
        /// Gets the name of the book.	
        /// </summary>
        /// <value>The name of the book.</value>
        /// <remarks></remarks>
        public string BookName
        {
            get { return bookName; }
        }

        /// <summary>
        /// Gets the document.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IDocument Document
        {
            get { return document; }
        }

        /// <summary>
        /// Gets the name of the document.	
        /// </summary>
        /// <value>The name of the document.</value>
        /// <remarks></remarks>
        public string DocumentName
        {
            get { return documentName; }
        }

        /// <summary>
        /// Gets the document anchor.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IDocumentAnchor DocumentAnchor
        {
            get { return documentAnchor; }
        }

        /// <summary>
        /// Gets the name of the document anchor.	
        /// </summary>
        /// <value>The name of the document anchor.</value>
        /// <remarks></remarks>
        public string DocumentAnchorName
        {
            get { return documentAnchorName; }
        }

        /// <summary>
        /// Gets the is in subscription.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool IsInSubscription
        {
            get { return isInSubscription; }
        }

        #endregion

        private const string BOOK_PREFIX_FASB = "fasb";
        private const string BOOK_PREFIX_GASB = "gasb";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLink" /> class.	
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="targetDoc">The target doc.</param>
        /// <param name="targetPointer">The target pointer.</param>
        /// <remarks></remarks>
        public ContentLink(ISite site, string targetDoc, string targetPointer)
        {
            this.site = site;
            ResolveLink(targetDoc, targetPointer);
        }

        private string GetBookDomainString()
        {
            string bookDomain = site.Books.Cast<IBook>().Aggregate(string.Empty, (current, book) => current + (book.Name + ","));

            if (bookDomain == "")
                return "";

            bookDomain = bookDomain.Substring(0, bookDomain.Length - 1);
            return bookDomain;
        }

        /// <summary>
        ///   If we are getting a fallback alternate target doc and target pointer to
        ///   AAM, we need to conver the target pointer to the approriate aam section.
        ///   This will take the target pointer and convert it if possible.
        ///   It will return string.Empty if it does not find a match.
        /// </summary>
        /// <param name = "targetPointer"></param>
        /// <returns>The alternate pointer if it is found, otherwise string.Empty</returns>
        private string ConvertAamTargetPointer(string targetPointer)
        {
            string alternatePtr = string.Empty;

            switch (targetPointer)
            {
                case "ara-gen":
                    alternatePtr = "aam_8010";
                    break;
                case "ara-cra":
                    alternatePtr = "aam_8015";
                    break;
                case "ara-hco":
                    alternatePtr = "aam_8030";
                    break;
                case "ara-ins":
                    alternatePtr = "aam_8040";
                    break;
                case "ara-dep":
                    alternatePtr = "aam_8050";
                    break;
                case "ara-ebp":
                    alternatePtr = "aam_8060";
                    break;
                case "ara-slg":
                    alternatePtr = "aam_8070";
                    break;
                case "ara-cir":
                    alternatePtr = "aam_8080";
                    break;
                case "ara-con":
                    alternatePtr = "aam_8090";
                    break;
                case "ara-inv":
                    alternatePtr = "aam_8100";
                    break;
                case "ara-brd":
                    alternatePtr = "aam_8110";
                    break;
                case "ara-npo":
                    alternatePtr = "aam_8120";
                    break;
                case "ara-ht":
                    alternatePtr = "aam_8130";
                    break;
                case "ara-rle":
                    alternatePtr = "aam_8140";
                    break;
                case "ara-aut":
                    alternatePtr = "aam_8160";
                    break;
                case "ara-sga":
                    alternatePtr = "aam_8220";
                    break;
                case "ara-mfg":
                    alternatePtr = "aam_8230";
                    break;
                case "ara-iet":
                    alternatePtr = "aam_8240";
                    break;
                case "ara-int":
                    alternatePtr = "aam_8250";
                    break;
                default:
                    break;
            }

            return alternatePtr;
        }

        private void ResolveLink(string targetDoc, string targetPointer)
        {
            // try to get the bookname, document name, and anchor name represented by the targetdoc and targetptr

            // default to "in subscription" this will be changed later if we find otherwise
            isInSubscription = true;

            // first try getting the book using the targetdoc
            book = site.Books[targetDoc];

            // if not found, try to get a fallback alternate book
            if (book == null)
            {
                string domain = GetBookDomainString();
                string alternameBookName = site.AlternateBook(targetPointer, targetDoc, domain);

                if (!string.IsNullOrEmpty(alternameBookName))
                {
                    targetDoc = alternameBookName;
                    book = site.Books[targetDoc];

                    if (targetDoc.StartsWith("aam"))
                    {
                        string alternatePtr = ConvertAamTargetPointer(targetPointer);

                        if (!string.IsNullOrEmpty(alternatePtr))
                        {
                            targetPointer = alternatePtr;
                        }
                    }
                }
            }
            else // book is NOT null
            {
                // TODO: This logical path may need to be executed after resolving alternate books as well

                // we found the book, so store its name
                bookName = targetDoc;

                // now try getting the doc using the targetptr
                if (targetPointer != null)
                {
                    document = book.Documents[targetPointer];
                    documentName = targetPointer;
                }
                else
                {
                    document = book.Documents[0];
                    documentName = document.Name;
                }
                //if the targetdoc and targetptr are equal, grab the first document in the book
                if (document == null && targetDoc == targetPointer)
                {
                    document = book.Documents[0];
                    documentName = document.Name;
                }


                if (document == null)
                {
                    // we failed to get the doc using the targetptr, so now
                    // try to get a doc that contains an anchor that matches
                    // targetptr
                    document = book.GetDocumentByAnchorName(targetPointer);

                    //try fallback link logic if we still don't have a document and we are dealing with a link to fasb or gasb content
                    if (document == null &&
                        (bookName.IndexOf(BOOK_PREFIX_FASB) != -1 || bookName.IndexOf(BOOK_PREFIX_GASB) != -1))
                    {
                        string fallbackTargetPtr = ResolveFasbLinks(targetPointer);

                        if (fallbackTargetPtr != string.Empty)
                        {
                            targetPointer = fallbackTargetPtr;
                            document = book.GetDocumentByAnchorName(targetPointer);
                        }
                    }


                    // if we have successfully found a document
                    if (document != null)
                    {
                        // we found the doc, so store its name
                        documentName = document.Name;

                        // we also determined that targetptr is the name of an anchor, so get the anchor and store its name
                        documentAnchor = document.GetDocumentAnchor(targetPointer);
                        documentAnchorName = documentAnchor.Name;
                    }
                }
            }

            // if we did not find the book in the user's site, make sure we set a flag before we continue
            if (bookName == string.Empty)
            {
                isInSubscription = false;
            }

            // if we could not get the book through normal means, the document either does not exist or is not in any of the user's subscriptions
            if (!isInSubscription)
            {
                FindBookInAnySubscription(targetDoc, targetPointer);
            }

            //*******************************
            // sburton 2010-04-28: This logic was used to handle the link process after we figure out the destination
            //*******************************
            //if (bookName == string.Empty || documentName == string.Empty)
            //{
            //    throw new Exception(string.Format(ERROR_DOCUMENTNOTFOUND, targetDoc, targetPointer));
            //}

            ////use the session to flag the toc for sync
            //if (bookInSubscription && document != null)
            //{
            //    Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = (documentAnchor != null) ? Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = DestroyerUi.GetTocPath(documentAnchor) : Session[DestroyerUi.REQPARAM_TOCSYNCPATH] = DestroyerUi.GetTocPath(document);
            //}

            //string docTabId = Request[DestroyerUi.REQPARAM_TARGETTAB];
            //string docCmd = Request[DestroyerUi.REQPARAM_TARGETCMD];

            ////okay, so now we construct our query params and redirect
            //string queryParams = string.Format("&{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}",
            //    REQPARAM_CURRENTBOOKNAME, bookName, REQPARAM_CURRENTDOCUMENTNAME, documentName,
            //    REQPARAM_CURRENTDOCUMENTANCHORNAME, documentAnchorName,
            //    REQPARAM_TARGETTAB, docTabId, REQPARAM_TARGETCMD, docCmd);
            //if (linkReferrer == LinkReferrer.SearchResults)
            //{
            //    queryParams += string.Format("&{0}={1}", REQPARAM_HITHIGHLIGHTS, bool.TrueString);
            //}

            ////DestroyerUi.ShowTab(this.Page, DestroyerUi.PortalTab.TocDoc, queryParams);
            //DestroyerUi.ShowTab(this.Page, this.displayToc(), queryParams);
        }

        // we should consider moving all the BOOK_PREFIX_* constants somewhere else

        private string ResolveFasbLinks(string targetPointer)
        {
            string fallbackTargetPtr = string.Empty;

            //if the targetptr ends with numbers within parens...
            if (Regex.Match(targetPointer, "\\([0-9]+\\)$").Success)
            {
                //strip everything from the last left paren to the end of the string
                targetPointer = targetPointer.Substring(0, targetPointer.LastIndexOf("(") - 1);
                fallbackTargetPtr = targetPointer;
            }

            //if the targetptr now ends with one or two alpha characters, preceded by a number character...
            if (Regex.Match(targetPointer, "[0-9][A-Za-z][A-Za-z]?$").Success)
            {
                //strip the trailing letter characters and try the link again
                fallbackTargetPtr = Regex.Match(targetPointer, "^(.*[0-9])([A-Za-z][A-Za-z]?)$").Groups[1].Value;
            }
            else if (Regex.Match(targetPointer, "[0-9]$").Success)
            {
                //add an 'a' and try the link again
                fallbackTargetPtr = targetPointer + "a";
            }

            return fallbackTargetPtr;
        }

        private void FindBookInAnySubscription(string targetDoc, string targetPointer)
        {
            // we want to grab a new version of our current site that does not filter by user access
            ISite allBookSite = new Site.Site(site.Id);

            // ...and see if this site has a book by the name of the targetdoc
            book = allBookSite.Books[targetDoc];

            if (book == null) return;
            // ...it does, so store the bookname
            bookName = targetDoc;

            // see if the targetptr exists in the book
            document = book.Documents[targetPointer];
            if (document != null)
            {
                //we found the doc, so store its name
                documentName = targetPointer;
            }
            else
            {
                //if the targetdoc and targetptr are equal, grab the first document in the book
                if (targetDoc == targetPointer)
                {
                    document = book.Documents[0];
                    documentName = document.Name;
                }
                else
                {
                    //we failed to get the doc using the targetptr, so lets
                    //try to get a doc that contains an anchor that matches
                    //targetptr
                    document = book.GetDocumentByAnchorName(targetPointer);
                    if (document != null)
                    {
                        //we found the doc, so store its name
                        documentName = document.Name;

                        //we also determined that targetptr is the name of an anchor, so store its name
                        documentAnchor = document.GetDocumentAnchor(targetPointer);
                        documentAnchorName = documentAnchor.Name;
                    }
                }
            }
        }
    }
}