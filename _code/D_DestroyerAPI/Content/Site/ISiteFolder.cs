namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Interface the exposes properties and methods for creating, 
    ///   managing and saving a site folder object.
    /// </summary>
    public interface ISiteFolder : ITocNode, IPrimaryContentContainer
    {
        #region Properties

        /// <summary>
        /// </summary>
        int Id { get; }

        /// <summary>
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// </summary>
        new string Title { get; }

        /// <summary>
        /// </summary>
        new string Uri { get; }

        /// <summary>
        /// </summary>
        string SiteReferencePath { get; }

        #endregion Properties

        #region Methods

        #endregion Methods
    }
}