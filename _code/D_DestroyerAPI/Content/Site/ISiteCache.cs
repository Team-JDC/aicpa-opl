namespace AICPA.Destroyer.Content.Site
{
    /// <summary>
    ///   Implements a generic caching service for the site.
    /// </summary>
    public interface ISiteCache
    {
        #region Properties

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Sets a cached property for the site. If the object corresponding to the specified name already exists it is replaced.
        /// </summary>
        /// <param name = "name">The name of the property to set</param>
        /// <param name = "obj">The object you are setting</param>
        /// <returns></returns>
        void SetProperty(string name, object obj);

        /// <summary>
        ///   Sets a cached property for the site.
        /// </summary>
        /// <param name = "name">The name of the property to set</param>
        /// <param name = "obj">The object you are setting</param>
        /// <param name = "replace">Specifies whether or not an existing object corresponding to the specified name is replaced.</param>
        /// <returns></returns>
        void SetProperty(string name, object obj, bool replace);

        /// <summary>
        ///   Retrieves a cached property from the site.
        /// </summary>
        /// <param name = "name">The name of the property to set</param>
        /// <returns>An object for the specified property</returns>
        object GetProperty(string name);

        /// <summary>
        ///   Disposes of all objects stored in teh cache.
        /// </summary>
        void Clear();

        #endregion Methods
    }
}