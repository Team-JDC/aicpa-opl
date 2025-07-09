using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;

namespace D_AuthWS
{
    public class Token
    {
        #region Indexer Class

        public class ClaimTextCollection
        {
            private readonly Token _instance;

            public ClaimTextCollection(Token instance)
            {
                _instance = instance;
            }

            /// <summary>
            /// The text value of the claim.
            /// </summary>
            /// <param name="uri">The claim URI as a string</param>
            /// <returns>The text value of the claim</returns>
            public string this[string uri]
            {
                get
                {
                    foreach (Claim claim in _instance.m_identityClaims.FindClaims(uri, Rights.PossessProperty))
                    {
                        return GetResourceValue(claim);
                    }
                    throw new ArgumentException(String.Format("Claim {0} not found", uri));
                }
            }
        }

        #endregion

        #region Token Decryption Magic

        private static readonly int[] Aes = {
                                                "http://www.w3.org/2001/04/xmlenc#aes128-cbc".GetHashCode(),
                                                "http://www.w3.org/2001/04/xmlenc#aes192-cbc".GetHashCode(),
                                                "http://www.w3.org/2001/04/xmlenc#aes256-cbc".GetHashCode(),
                                                "http://www.w3.org/2001/04/xmlenc#kw-aes128".GetHashCode(),
                                                "http://www.w3.org/2001/04/xmlenc#kw-aes192".GetHashCode(),
                                                "http://www.w3.org/2001/04/xmlenc#kw-aes256".GetHashCode()
                                            };

        private static readonly int[] TripleDes = {
                                                      "http://www.w3.org/2001/04/xmlenc#tripledes-cbc".GetHashCode(),
                                                      "http://www.w3.org/2001/04/xmlenc#kw-tripledes".GetHashCode()
                                                  };

        #region Nested type: WSSecurityStrings

        private static class WSSecurityStrings
        {
            public const string KeyIdentifier = "KeyIdentifier";

            public const string Namespace =
                "http://www.oasis-open.org/committees/download.php/16791/oasis-wss-wssecurity-secext-1.1.xsd";
        }

        #endregion

        #region Nested type: XmlEncryptionStrings

        private static class XmlEncryptionStrings
        {
            public const string Algorithm = "Algorithm";
            public const string CipherValue = "CipherValue";
            public const string Encoding = "Encoding";
            public const string EncryptionMethod = "EncryptionMethod";
            public const string MimeType = "MimeType";
            public const string Namespace = "http://www.w3.org/2001/04/xmlenc#";
        }

        #endregion

        #region Nested type: XmlSignatureStrings

        private static class XmlSignatureStrings
        {
            public const string DigestMethod = "DigestMethod";
            public const string KeyInfo = "KeyInfo";
            public const string Namespace = "http://www.w3.org/2000/09/xmldsig#";
        }

        #endregion

        #endregion

        #region Resuable Data

        // cache the certificates by thumbprint, lowers overhead on lookup 
        private static readonly Hashtable m_certificates = new Hashtable();
        // store the token skew TimeSpan, save the object creation time
        private static object _maxTokenSkew;

        protected static TimeSpan MaximumTokenSkew
        {
            get
            {
                if (null == _maxTokenSkew)
                {
                    int skew = 300000; // default to 5 minutes
                    string tokenskew = ConfigurationManager.AppSettings["MaximumClockSkew"];
                    if (!string.IsNullOrEmpty(tokenskew))
                        skew = Int32.Parse(tokenskew);
                    _maxTokenSkew = new TimeSpan(0, 0, skew);
                }
                return (TimeSpan) _maxTokenSkew;
            }
        }

        #endregion

        #region instance data

        private readonly AuthorizationContext m_authorizationContext;
        private readonly SamlSecurityToken m_token;
        private ClaimTextCollection _claimIndexer;
        private EndpointIdentity m_endpointIdentity;
        private ClaimSet m_identityClaims;
        private EndpointIdentity m_issuer;
        // provides support for indexed claims accessor (ie : th.Claim[Claims.PPID] )

        #endregion

        /// <summary>
        /// Token Constructor
        /// </summary>
        /// <param name="xmlToken">Encrypted xml token</param>
        public Token(String xmlToken)
        {
            byte[] decryptedData = decryptToken(xmlToken);

            XmlReader reader = new XmlTextReader(new StreamReader(new MemoryStream(decryptedData), Encoding.UTF8));
            m_token = (SamlSecurityToken) WSSecurityTokenSerializer.DefaultInstance.ReadToken(reader, null);

            var authenticator = new SamlSecurityTokenAuthenticator(new List<SecurityTokenAuthenticator>(
                                                                       new SecurityTokenAuthenticator[]
                                                                           {
                                                                               new RsaSecurityTokenAuthenticator(),
                                                                               new X509SecurityTokenAuthenticator()
                                                                           }), MaximumTokenSkew);


            if (authenticator.CanValidateToken(m_token))
            {
                ReadOnlyCollection<IAuthorizationPolicy> policies = authenticator.ValidateToken(m_token);
                m_authorizationContext = AuthorizationContext.CreateDefaultAuthorizationContext(policies);
                FindIdentityClaims();
            }
            else
            {
                throw new Exception("Unable to validate the token.");
            }
        }

        /// <summary>
        /// Returns the collection of claim objects from the token
        /// </summary>
        public ClaimSet IdentityClaims
        {
            get { return m_identityClaims; }
        }

        /// <summary>
        /// Returns the Issuer's Identity Claim (typically, the public key of the issuer)
        /// </summary>
        public Claim IssuerIdentityClaim
        {
            get { return m_issuer.IdentityClaim; }
        }

        public AuthorizationContext AuthorizationContext
        {
            get { return m_authorizationContext; }
        }

        /// <summary>
        /// Gets the UniqueID (IdentityClaim) of this token. 
        /// 
        /// By default, this uses the PPID and the Issuer's Public Key and hashes them 
        /// together to generate a UniqueID. To use a different field, add a line:
        /// 
        ///     <add name="IdentityClaimType" value="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier" />
        ///
        /// Replacing the value with the URI for your unique claim. 
        /// </summary>
        public string UniqueID
        {
            get { return GetResourceValue(m_endpointIdentity.IdentityClaim); }
        }

        // 
        /// <summary>
        /// Read-only String collection of the claims in the token.
        /// 
        /// Provides support for indexed claims accessor (ie : 
        /// 
        ///     securityToken.Claims[ClaimsTypes.PPID]
        /// 
        /// </summary>
        public ClaimTextCollection Claims
        {
            get
            {
                // perform lazy instantiation to avoid unnecesary object creation.
                if (null == _claimIndexer)
                    _claimIndexer = new ClaimTextCollection(this);
                return _claimIndexer;
            }
        }

        /// <summary>
        /// Utility function to create an EndpointIdentity from a ClaimSet
        /// </summary>
        /// <param name="claims">Claims to create the EndpointIdentity with</param>
        /// <returns>An EndpointIdentity</returns>
        private static EndpointIdentity CreateIdentityFromClaimSet(ClaimSet claims)
        {
            foreach (Claim claim in claims.FindClaims(null, Rights.Identity))
            {
                return EndpointIdentity.CreateIdentity(claim);
            }
            return null;
        }

        /// <summary>
        /// Processes the Auth Context to get the IdentityClaims
        /// </summary>
        /// <returns></returns>
        protected void FindIdentityClaims()
        {
            // Pick up the claim type to use for generating the UniqueID claim
            string identificationClaimType = ConfigurationManager.AppSettings["IdentityClaimType"];

            // Or, default to PPID
            if (string.IsNullOrEmpty(identificationClaimType))
                identificationClaimType = ClaimTypes.PPID;

            ClaimSet result = null;
            m_endpointIdentity = null;

            foreach (ClaimSet claimSet in m_authorizationContext.ClaimSets)
            {
                //
                // This loops through the claims looking for the configured claim type
                // that will be used as part of the generation of the unique id.
                //
                foreach (Claim claim in claimSet.FindClaims(identificationClaimType, Rights.PossessProperty))
                {
                    // found a matching claim. This is good.
                    m_issuer = CreateIdentityFromClaimSet(claimSet.Issuer);
                    byte[] issuerkey;

                    // we need to get a byte array that represents the issuer's key
                    // to use as part of the hash to generate a unique id. 
                    // currently supported: (byte[]) or an RSACryptoServiceProvider,
                    // This could be expanded to support other types. The key just 
                    // needs to be extracted as a byte array.
                    issuerkey = m_issuer.IdentityClaim.Resource as byte[];
                    if (null == issuerkey)
                    {
                        var csp = m_issuer.IdentityClaim.Resource as RSACryptoServiceProvider;
                        if (null != csp)
                            issuerkey = csp.ExportCspBlob(false);

                        // Can't use this claim to get the key.
                        if (null == issuerkey)
                            throw new Exception("Unsupported IdentityClaim resource type");
                    }

                    // It doesn't matter what encoding type we use, as long as it is consistent.
                    // this conversion is just to get a consistent set of bytes from the claim.
                    byte[] uniqueClaim = Encoding.UTF8.GetBytes(GetResourceValue(claim));

                    // copy the thumbprint data and the uniqueClaim together.
                    var thumbprintData = new byte[uniqueClaim.Length + issuerkey.Length];
                    issuerkey.CopyTo(thumbprintData, 0);
                    uniqueClaim.CopyTo(thumbprintData, issuerkey.Length);

                    // generate a hash.
                    using (SHA256 sha = new SHA256Managed())
                    {
                        var thumbprintClaim = new Claim(ClaimTypes.Thumbprint,
                                                        Convert.ToBase64String(sha.ComputeHash(thumbprintData)),
                                                        Rights.Identity);
                        m_endpointIdentity = EndpointIdentity.CreateIdentity(thumbprintClaim);
                    }

                    m_identityClaims = claimSet;
                    return;
                }


                if (null == m_endpointIdentity)
                {
                    //
                    // check for identity claim, if found, hold on to it, we may need to use it :)
                    //
                    result = claimSet;
                    m_endpointIdentity = CreateIdentityFromClaimSet(claimSet);
                }
            }

            if (null != m_endpointIdentity)
            {
                m_identityClaims = result;
                return;
            }

            //
            // we have looped all claim sets with out finding an identity claim.
            // we will return the ppidIdentity and claimset if they were found.
            //
            throw new Exception("The XML Token data provided no Identity claim.");
        }

        /// <summary>
        ///     Translates claims to strings
        /// </summary>
        /// <param name="claim">Claim to translate to a string</param>
        /// <returns></returns>
        protected static string GetResourceValue(Claim claim)
        {
            var reference = claim.Resource as IdentityReference;
            if (null != reference)
            {
                return reference.Value;
            }

            var rsa = claim.Resource as ICspAsymmetricAlgorithm;
            if (null != rsa)
            {
                using (SHA256 sha = new SHA256Managed())
                {
                    return Convert.ToBase64String(sha.ComputeHash(rsa.ExportCspBlob(false)));
                }
            }

            var mail = claim.Resource as MailAddress;
            if (null != mail)
            {
                return mail.ToString();
            }

            var bufferValue = claim.Resource as byte[];
            if (null != bufferValue)
            {
                return Convert.ToBase64String(bufferValue);
            }

            return claim.Resource.ToString();
        }

        /// <summary>
        /// Decrpyts a security token from an XML EncryptedData 
        /// </summary>
        /// <param name="xmlToken">the XML token to decrypt</param>
        /// <returns>A byte array of the contents of the encrypted token</returns>
        private static byte[] decryptToken(string xmlToken)
        {
            int encryptionAlgorithm;
            int m_keyEncryptionAlgorithm;
            string m_keyHashAlgorithm;
            byte[] thumbprint;
            byte[] securityTokenData;
            byte[] symmetricKeyData;

            XmlReader reader = new XmlTextReader(new StringReader(xmlToken));

            // Find the EncryptionMethod element, grab the Algorithm
            if (!reader.ReadToDescendant(XmlEncryptionStrings.EncryptionMethod, XmlEncryptionStrings.Namespace))
                throw new ArgumentException("Cannot find token EncryptedMethod.");
            encryptionAlgorithm = reader.GetAttribute(XmlEncryptionStrings.Algorithm).GetHashCode();

            // Find the EncryptionMethod element for the key, grab the Algorithm
            if (!reader.ReadToFollowing(XmlEncryptionStrings.EncryptionMethod, XmlEncryptionStrings.Namespace))
                throw new ArgumentException("Cannot find key EncryptedMethod.");
            m_keyEncryptionAlgorithm = reader.GetAttribute(XmlEncryptionStrings.Algorithm).GetHashCode();

            // Find the Digest method for the key identifier
            if (!reader.ReadToFollowing(XmlSignatureStrings.DigestMethod, XmlSignatureStrings.Namespace))
                throw new ArgumentException("Cannot find Digest Method.");
            m_keyHashAlgorithm = reader.GetAttribute(XmlEncryptionStrings.Algorithm);
            // Find the key identifier
            if (!reader.ReadToFollowing(WSSecurityStrings.KeyIdentifier, WSSecurityStrings.Namespace))
                throw new ArgumentException("Cannot find Key Identifier.");
            reader.Read();
            thumbprint = Convert.FromBase64String(reader.ReadContentAsString());

            // Find the encrypted Symmetric Key
            if (!reader.ReadToFollowing(XmlEncryptionStrings.CipherValue, XmlEncryptionStrings.Namespace))
                throw new ArgumentException("Cannot find symmetric key.");
            reader.Read();
            symmetricKeyData = Convert.FromBase64String(reader.ReadContentAsString());

            // Find the encrypted Security Token
            if (!reader.ReadToFollowing(XmlEncryptionStrings.CipherValue, XmlEncryptionStrings.Namespace))
                throw new ArgumentException("Cannot find encrypted security token.");
            reader.Read();
            securityTokenData = Convert.FromBase64String(reader.ReadContentAsString());

            reader.Close();

            SymmetricAlgorithm alg = null;
            X509Certificate2 certificate = FindCertificate(thumbprint);

            foreach (int i in Aes)
                if (encryptionAlgorithm == i)
                {
                    alg = new RijndaelManaged();
                    break;
                }
            if (null == alg)
                foreach (int i in TripleDes)
                    if (encryptionAlgorithm == i)
                    {
                        alg = new TripleDESCryptoServiceProvider();
                        break;
                    }

            if (null == alg)
                throw new ArgumentException("Could not determine Symmetric Algorithm");
            alg.Key = (certificate.PrivateKey as RSACryptoServiceProvider).Decrypt(symmetricKeyData, true);
            ;
            int ivSize = alg.BlockSize/8;
            var iv = new byte[ivSize];
            Buffer.BlockCopy(securityTokenData, 0, iv, 0, iv.Length);
            alg.Padding = PaddingMode.ISO10126;
            alg.Mode = CipherMode.CBC;
            ICryptoTransform decrTransform = alg.CreateDecryptor(alg.Key, iv);
            byte[] plainText = decrTransform.TransformFinalBlock(securityTokenData, iv.Length,
                                                                 securityTokenData.Length - iv.Length);
            decrTransform.Dispose();
            return plainText;
        }

        /// <summary>
        /// Finds certificates by thumbprint, caches them statically.
        /// 
        /// This may be configured with the following parameters in the web.config file:
        /// 
        /// The Certificate Thumbprint can be hard coded:
        ///   <add name="CertifcateThumbprint" value="01234567890ABCDEFEDCBA01234567890ABCDEFEDCBA" />
        ///
        /// The Store Name Can be specified:
        ///   <add name="StoreName" value="My" />  
        ///     
        /// The Store Location Can be specified
        ///   <add name="StoreLocation" value="LocalMachine" />
        /// 
        /// </summary>
        /// <param name="thumbprint">Certificate's thumbprint</param>
        /// <returns>the certificate, or the default certificate</returns>
        private static X509Certificate2 FindCertificate(object thumbprint)
        {
            var result = m_certificates[thumbprint] as X509Certificate2;

            if (null == result)
            {
                StoreName storeName = StoreName.My;
                StoreLocation storeLocation = StoreLocation.LocalMachine;

                string rpCertificateThumbprint = ConfigurationManager.AppSettings["CertificateThumbprint"];
                string rpStoreName = ConfigurationManager.AppSettings["StoreName"];
                string rpStoreLocation = ConfigurationManager.AppSettings["StoreLocation"];

                if (!string.IsNullOrEmpty(rpStoreName))
                    storeName = (StoreName) Enum.Parse(typeof (StoreName), rpStoreName, true);

                if (!string.IsNullOrEmpty(rpStoreLocation))
                    storeLocation = (StoreLocation) Enum.Parse(typeof (StoreLocation), rpStoreLocation, true);

                var store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly);

                // if the thumbprint is passed as a string thumbprint, good
                // otherwise if it is a byte array turn it to a string
                string strThumbprint = "";
                if (null != thumbprint as byte[])
                    foreach (byte b in (thumbprint as byte[]))
                        strThumbprint += b.ToString("X2");
                else
                    strThumbprint = thumbprint.ToString();

                // search based off the thumbprint in the token.
                X509Certificate2Collection collection = store.Certificates.Find(X509FindType.FindByThumbprint,
                                                                                strThumbprint, true);
                if (collection.Count == 0)
                {
                    // if it's not found there, search by the thumbprint specified in the App.config
                    if (string.IsNullOrEmpty(rpCertificateThumbprint))
                        throw new Exception("Relying Party Certificate thumbprint not specified");

                    collection = store.Certificates.Find(X509FindType.FindByThumbprint, rpCertificateThumbprint, true);
                    if (collection.Count == 0)
                        throw new Exception("Unable to find certificate by thumbprint.");
                }
                result = collection[0];
                m_certificates.Add(thumbprint, result);
            }
            return result;
        }
    }
}