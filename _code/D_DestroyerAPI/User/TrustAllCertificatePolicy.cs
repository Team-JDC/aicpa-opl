#region

using System.Net;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace AICPA.Destroyer.User
{
    /// <summary>
    ///   Summary description for TrustAllCertificatePolicy.
    /// </summary>
    public class TrustAllCertificatePolicy : ICertificatePolicy
    {
        #region ICertificatePolicy Members

        /// <summary>
        /// Checks the validation result.	
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="req">The req.</param>
        /// <param name="problem">The problem.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CheckValidationResult(ServicePoint sp,
                                          X509Certificate cert, WebRequest req, int problem)
        {
            return true;
        }

        #endregion
    }
}