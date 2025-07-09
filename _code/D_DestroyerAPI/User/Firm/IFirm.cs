#region

using System;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    ///<summary>
    ///  Summary description for IFirm.
    ///</summary>
    public interface IFirm
    {
        #region Properties

        ///<summary>
        ///  The Aca of the firm.
        ///</summary>
        string Aca { get; }

        ///<summary>
        ///  The Code of a firm
        ///</summary>
        string Code { get; }

        ///<summary>
        ///  The users in this firm.
        ///</summary>
        Guid[] UserIds { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Gets the current number of users logged into the system that are still valid.
        ///</summary>
        ///<param name = "sessionTimeout"></param>
        ///<returns></returns>
        int GetCurrentFirmUserCount(int sessionTimeout);

        bool UserLoggedIn(Guid userId);

        #endregion Methods
    }
}