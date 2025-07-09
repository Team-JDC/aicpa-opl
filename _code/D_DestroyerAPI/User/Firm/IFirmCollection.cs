#region

using System.Collections;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    ///<summary>
    ///  Summary description for IFirmCollection.
    ///</summary>
    public interface IFirmCollection : IEnumerable
    {
        ///<summary>
        ///  Read-only property returning the number of Firms in this collection.
        ///</summary>
        int Count { get; }

        /// <summary>
        /// </summary>
        IFirm this[int index] { get; }
    }
}