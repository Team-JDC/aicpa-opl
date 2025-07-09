using AICPA.Destroyer.User;
using System.Collections.Generic;
namespace AICPA.Destroyer.Content.Document
{
    /// <summary>
    ///   Summary description for IDocumentFormat.
    /// </summary>
    public interface IDocumentFormat
    {
        #region Properties

        /// <summary>
        ///   The content type string describing the context format
        /// </summary>
        string Description { get; }

        /// <summary>
        ///   The identifier of the content type description
        /// </summary>
        int ContentTypeId { get; }

        /// <summary>
        ///   A uri pointing to the document in the context format
        /// </summary>
        string Uri { get; }

        /// <summary>
        ///   A byte array containing the content of the document in the context format
        /// </summary>
        byte[] Content { get; }

        /// <summary>
        ///   The byte size of the context document format
        /// </summary>
        long ContentLength { get; }

        /// <summary>
        ///   A boolean value indicating whether or not this is the primary format for a document.
        /// </summary>
        bool IsPrimary { get; }

        /// <summary>
        ///   A boolean value indicating whether or not the content should be "automatically downloaded" in the client UI.
        /// </summary>
        bool IsAutoDownload { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Returns a byte array containing the document format's content with hit highlighting markup applied.
        /// </summary>
        /// <param name = "highlightTerms">The terms you wish to highlight</param>
        /// <param name = "highlightBeginMarkup">Markup begin tag. You may include a format template containing {0} to include a counter in your markup. Example: &lt;SPAN id="hitnum_{0}" class="hit"&gt;</param>
        /// <param name = "highlightEndMarkup">Markup end tag</param>
        /// <returns></returns>
        byte[] GetHighlightedContent(string[] highlightTerms, string highlightBeginMarkup, string highlightEndMarkup);

        /// <summary>
        ///   Returns a byte array containing the document format's content with hit highlighting markup applied.
        /// </summary>
        /// <param name = "highlightTerms">The terms you wish to highlight</param>
        /// <param name = "highlightBeginMarkup">Markup begin tag. You may include a format template containing {0} to include a counter in your markup. Example: &lt;SPAN id="hitnum_{0}" class="hit"&gt;</param>
        /// <param name = "highlightEndMarkup">Markup end tag</param>
        /// <param name = "ignoreCase">Ignore case when searching for terms</param>
        /// <returns></returns>
        byte[] GetHighlightedContent(string[] highlightTerms, string highlightBeginMarkup, string highlightEndMarkup, bool ignoreCase);

        byte[] GetClientHighlightContent(string[] highlightTerms);
        
        byte[] GetClientHighlightContent(string[] highlightTerms, string hitAnchor);

        byte[] GetContentNoDoLinks();

        Dictionary<string, int> GetDocumentHits(string[] highlightTerms);

        void LogContentAccess(IUser user);

        #endregion Methods
    }
}