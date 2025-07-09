#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.Content
{
    /// <summary>
    ///   Summary description for ITocNodeCollection.
    /// </summary>
    public interface ITocNodeCollection : IEnumerable
    {
        #region Properties

        /// <summary>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// </summary>
        ITocNode this[int index] { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Save changes to the ITocNodeCollection.
        ///</summary>
        void Save();

        #endregion Methods
    }
}