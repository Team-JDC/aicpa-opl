#region

using AICPA.Destroyer.Content.Document;

#endregion

namespace AICPA.Destroyer.Content
{
    /// <summary>
    /// 	
    /// </summary>
    public class ContentWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWrapper"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public ContentWrapper(IDocument document)
        {
            Document = document;
            Uri = null;
            Anchor = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWrapper"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="document">The anchor for the document.</param>
        public ContentWrapper(IDocument document, string anchor)
        {
            Document = document;
            Uri = null;
            Anchor = anchor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWrapper"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public ContentWrapper(string uri)
        {
            Document = null;
            Uri = uri;
            Anchor = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentWrapper"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public ContentWrapper(string targetDoc, string targetPointer)
        {
            Document = null;
            Uri = null;
            Anchor = null;
            TargetDoc = targetDoc;
            TargetPointer = targetPointer;
        }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        public IDocument Document { get; private set; }
        /// <summary>
        /// Gets or sets the URI.	
        /// </summary>
        /// <value>The URI.</value>
        /// <remarks></remarks>
        public string Uri { get; private set; }
        /// <summary>
        /// Gets or sets the anchor for the document.
        /// </summary>
        /// <value>The anchor.</value>
        public string Anchor { get; private set; }
        public string TargetDoc { get; private set; }
        public string TargetPointer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has document.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has document; otherwise, <c>false</c>.
        /// </value>
        public bool HasDocument
        {
            get { return Document != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has URI.
        /// </summary>
        /// <value><c>true</c> if this instance has URI; otherwise, <c>false</c>.</value>
        public bool HasUri
        {
            get { return Uri != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the document has an anchor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this document has an anchor; otherwise, <c>false</c>.
        /// </value>
        public bool HasAnchor
        {
            get { return Anchor != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the document has an anchor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this document has a document pointer; otherwise, <c>false</c>.
        /// </value>
        public bool HasDocPointer
        {
            get { return TargetPointer != null; }
        }
    }
}