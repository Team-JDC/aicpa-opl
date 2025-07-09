using dk.nita.saml20.Schema.Core;

namespace dk.nita.saml20.Profiles.DKSaml20.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class DKSaml20UserCertificateAttribute : DKSaml20Attribute
    {
        /// <summary>
        /// Friendly name
        /// </summary>
        public const string FRIENDLYNAME = "userCertificate";

        /// <summary>
        /// Attribute name
        /// </summary>
        public const string NAME = "urn:oid:1.3.6.1.4.1.1466.115.121.1.8";

        /// <summary>
        /// Creates an attribute with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static SamlAttribute Create(string value)
        {
            return Create(NAME, FRIENDLYNAME, value);
        }
    }
}