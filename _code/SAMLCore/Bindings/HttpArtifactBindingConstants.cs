using System;

namespace dk.nita.saml20.Bindings
{
    /// <summary>
    /// Constants pertaining to the artifact binding over HTTP SOAP.
    /// </summary>
    public class HttpArtifactBindingConstants
    {
        /// <summary>
        /// Artifact query string name
        /// </summary>
        public const string ArtifactQueryStringName = "SAMLart";

        /// <summary>
        /// Name of artifact resolve
        /// </summary>
        public const string ArtifactResolve = "ArtifactResolve";

        /// <summary>
        /// Name of artifact response
        /// </summary>
        public const string ArtifactResponse = "ArtifactResponse";

        /// <summary>
        /// Default type code
        /// </summary>
        public const Int16 ArtifactTypeCode = 0x0004;

        /// <summary>
        /// Soap action
        /// </summary>
        public const string SOAPAction = "http://www.oasis-open.org/committees/security";
    }
}