#region

using AICPA.Destroyer.User.Firm;

#endregion

namespace AICPA.Destroyer.User
{
    /// <summary>
    ///   Interface that provides properties and methodes for creating managing
    ///   and saving a UserSecurity object. This interface should always be referenced
    ///   through the user interface.
    /// </summary>
    public interface IUserSecurity
    {
        #region Properties

        ///<summary>
        ///  Interface that points back to the user that is implimenting this security interface.
        ///</summary>
        IUser User { get; }

        ///<summary>
        ///  Security Credential that qualifies the content a user is allowed to view
        ///</summary>
        string Domain { get; }

        ///<summary>
        ///  A collection of the subscriptions that a user has access to.
        ///</summary>
        string[] BookName { get; }

        ///<summary>
        ///  The user's firm.
        ///</summary>
        IFirmCollection FirmCollection { get; }

        ///<summary>
        ///  Security credential to define a users access to the site
        ///</summary>
        int ActionPermission { get; }

        ///<summary>
        ///  The Id of the current session.
        ///</summary>
        string SessionId { get; }

        ///<summary>
        ///  Returns true if the user is authenticated else false.
        ///</summary>
        bool Authenticated { get; }

        ///<summary>
        ///  The reason why the user was not authenticated.
        ///</summary>
        string AuthenticationError { get; }

        /// <summary>
        ///   The user's email address
        /// </summary>
        string Email { get; }

        #endregion Properties

        #region Methods

        ///<summary>
        ///  Ends a users session.
        ///</summary>
        void EndUserSession();

        ///<summary>
        ///  Determines if a user is authorized for the given action.
        ///</summary>
        ///<param name = "action">The action to be authorized.</param>
        ///<returns>A bool indicating if the user is authorized for the given action.</returns>
        bool AuthorizeAction(int action);

        #endregion Methods
    }
}