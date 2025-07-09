#region

using System;

#endregion

namespace AICPA.Destroyer.User.Firm
{
    ///<summary>
    ///  A class that contain information about a firm including number 
    ///  of users logged on, current userids, aca and code.
    ///</summary>
    public class Firm : IFirm
    {
        #region Constructors

        #region Public Constructor

        ///<summary>
        ///  Public constructor for a user's firm.
        ///</summary>
        ///<param name = "aca">The aca of the firm.</param>
        public Firm(string aca, string code)
        {
            userDs.CurrentFirmUsers.AddCurrentFirmUsersRow(aca, code, Guid.Empty);
            currentFirmUsersRow = userDs.CurrentFirmUsers.FindByACACode(aca, code);
            currentFirmUsersDataTable = (UserDs.CurrentFirmUsersDataTable) currentFirmUsersRow.Table;
        }

        #endregion Public Constructor

        #region Internal Constructor

        ///<summary>
        ///  Internal constructor for a firm
        ///</summary>
        ///<param name = "currentFirmUsersRow"></param>
        internal Firm(UserDs.CurrentFirmUsersRow currentFirmUsersRow)
        {
            userDs = (UserDs) currentFirmUsersRow.Table.DataSet;
            currentFirmUsersDataTable = (UserDs.CurrentFirmUsersDataTable) currentFirmUsersRow.Table;
            this.currentFirmUsersRow = currentFirmUsersRow;
        }

        #endregion Internal Constructor

        #endregion Constructors

        #region Properties

        #region Private Properties

        private readonly UserDs.CurrentFirmUsersRow currentFirmUsersRow;
        private readonly UserDs userDs = new UserDs();

        private FirmDalc activeFirmDalc;
        private UserDs.CurrentFirmUsersDataTable currentFirmUsersDataTable;

        ///<summary>
        ///  Create the dalc if you need to, otherwise just return it.
        ///</summary>
        private FirmDalc ActiveFirmDalc
        {
            get { return activeFirmDalc ?? (activeFirmDalc = new FirmDalc()); }
        }

        #endregion Private Properties

        #region IFirm Properties

        ///<summary>
        ///  The Aca of the firm
        ///</summary>
        public string Aca
        {
            get { return currentFirmUsersRow.ACA; }
        }

        ///<summary>
        ///  The Code of the firm
        ///</summary>
        public string Code
        {
            get { return currentFirmUsersRow.Code; }
        }

        ///<summary>
        ///  The ids of the users in the firm.
        ///</summary>
        public Guid[] UserIds
        {
            get { return ActiveFirmDalc.GetFirmCurrentUserIds(Aca, Code); }
        }

        #endregion IFirm Properties

        #endregion Properties

        #region Methods

        #region IFirm Methods

        ///<summary>
        ///  Gets the firm's current user count
        ///</summary>
        ///<param name = "sessionTimeout">The amount of time before a user's session is timed out on the API</param>
        ///<returns></returns>
        public int GetCurrentFirmUserCount(int sessionTimeout)
        {
            return ActiveFirmDalc.GetCurrentFirmUserCount(Aca, Code, sessionTimeout);
        }

        /// <summary>
        /// Check to see if a given user is checked in
        /// </summary>
        /// <param name="userId">userId to check</param>
        /// <returns>true if userId exists in</returns>
        public bool UserLoggedIn(Guid userId)
        {
            Guid[] users = ActiveFirmDalc.GetFirmCurrentUserIds(Aca, Code);
            foreach (Guid guid in users)
            {
                if (guid.Equals(userId))
                    return true;
            }
            return false;
        }

        #endregion IFirm Methods

        #endregion Methods
    }
}