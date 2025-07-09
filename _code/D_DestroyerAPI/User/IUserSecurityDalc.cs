#region

using System;

#endregion

namespace AICPA.Destroyer.User
{
    /// <summary>
    ///   Summary description for IUserSecurityDalc.
    /// </summary>
    public interface IUserSecurityDalc
    {
        /// <summary>
        /// </summary>
        /// <param name = "userDs"></param>
        void CreateUserSession(UserDs userDs);

        /// <summary>
        /// </summary>
        /// <param name = "userDs"></param>
        void EndUserSession(UserDs userDs);

        /// <summary>
        /// </summary>
        /// <param name = "domainString"></param>
        /// <param name = "domainStringDelimiter"></param>
        /// <returns></returns>
        string[] GetSubscriptionBookNames(string domainString, char domainStringDelimiter);

        /// <summary>
        /// </summary>
        /// <param name = "userId"></param>
        /// <param name = "sessionId"></param>
        /// <returns></returns>
        bool IsSessionIdValid(Guid userId, string sessionId);
    }
}