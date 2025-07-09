#region

using System;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    /// <summary>
    ///   Summary description for IFirm.
    /// </summary>
    public interface IFirmDalc
    {
        ///<summary>
        ///  Get the firms current concurrent user count
        ///</summary>
        ///<param name = "aca">The Aca of the firm.</param>
        ///<param name = "code">The Code fo the firm.</param>
        ///<returns>The count of the number of current users logged onto Destroyer for the given firm.</returns>
        int GetCurrentFirmUserCount(string aca, string code, int sessionTimeout);

        ///<summary>
        ///  Get all the current userIds for a given firm.  This will be used in reporting.
        ///</summary>
        ///<param name = "aca">The Aca of the firm.</param>
        ///<param name = "code">The Code fo the firm.</param>
        ///<returns>An array of Guids.</returns>
        Guid[] GetFirmCurrentUserIds(string aca, string code);
    }
}