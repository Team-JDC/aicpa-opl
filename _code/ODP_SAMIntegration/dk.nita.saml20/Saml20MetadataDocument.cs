using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using dk.nita.saml20.config;
using dk.nita.saml20.Schema.Core;
using dk.nita.saml20.Schema.Metadata;
using dk.nita.saml20.Utils;

namespace dk.nita.saml20
{
    /// <summary>
    /// The Saml20MetadataDocument class handles functionality related to the &lt;EntityDescriptor&gt; element.
    /// If a received metadata document contains a &lt;EntitiesDescriptor&gt; element, it is necessary to use an
    /// instance of this class for each &lt;EntityDescriptor&gt; contained.
    /// </summary>
    public class Saml20MetadataDocument
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        public Saml20MetadataDocument()
        {
        }

        /// <summary>
        /// Initialize the instance with an already existing metadata document.
        /// </summary>        
        public Saml20MetadataDocument(XmlDocument entityDescriptor)
            : this()
        {
            if (XmlSignatureUtils.IsSigned(entityDescriptor))
                if (!XmlSignatureUtils.CheckSignature(entityDescriptor))
                    throw new Saml20Exception("Metadata signature could not be verified.");

            ExtractKeyDescriptors(entityDescriptor);
            _entity = Serialization.DeserializeFromXmlString<EntityDescriptor>(entityDescriptor.OuterXml);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        /// <param name="sign">if set to <c>true</c> the metadata document will be signed.</param>
        public Saml20MetadataDocument(bool sign)
            : this()
        {
            Sign = sign;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="keyinfo">key information for the service provider certificate.</param>
        /// <param name="sign">if set to <c>true</c> the metadata document will be signed.</param>
        public Saml20MetadataDocument(SAML20FederationConfig config, KeyInfo keyinfo, bool sign)
            : this(sign)
        {
            ConvertToMetadata(config, keyinfo);
        }

        #endregion

        private List<Endpoint> _attributeQueryEndpoints;

        /// <summary>
        /// Takes the Safewhere configuration class and converts it to a SAML2.0 metadata document.
        /// </summary>        
        private void ConvertToMetadata(SAML20FederationConfig config, KeyInfo keyinfo)
        {
            EntityDescriptor entity = CreateDefaultEntity();
            entity.entityID = config.ServiceProvider.ID;
            entity.validUntil = DateTime.Now.AddDays(7);

            var spDescriptor = new SPSSODescriptor();
            spDescriptor.protocolSupportEnumeration = new[] {Saml20Constants.PROTOCOL};
            spDescriptor.AuthnRequestsSigned = XmlConvert.ToString(true);
            spDescriptor.WantAssertionsSigned = XmlConvert.ToString(true);
            if (config.ServiceProvider.NameIdFormats.All)
            {
                spDescriptor.NameIDFormat = new[]
                                                {
                                                    Saml20Constants.NameIdentifierFormats.Email,
                                                    Saml20Constants.NameIdentifierFormats.Entity,
                                                    Saml20Constants.NameIdentifierFormats.Kerberos,
                                                    Saml20Constants.NameIdentifierFormats.Persistent,
                                                    Saml20Constants.NameIdentifierFormats.Transient,
                                                    Saml20Constants.NameIdentifierFormats.Unspecified,
                                                    Saml20Constants.NameIdentifierFormats.Windows,
                                                    Saml20Constants.NameIdentifierFormats.X509SubjectName
                                                };
            }
            else
            {
                spDescriptor.NameIDFormat = new string[config.ServiceProvider.NameIdFormats.NameIdFormats.Count];
                int count = 0;
                foreach (NameIdFormatElement elem in config.ServiceProvider.NameIdFormats.NameIdFormats)
                {
                    spDescriptor.NameIDFormat[count++] = elem.NameIdFormat;
                }
            }


            var baseURL = new Uri(config.ServiceProvider.Server);
            var logoutServiceEndpoints = new List<Endpoint>();
            var signonServiceEndpoints = new List<IndexedEndpoint>();

            var artifactResolutionEndpoints = new List<IndexedEndpoint>(2);

            // Include endpoints.
            foreach (Saml20ServiceEndpoint endpoint in config.ServiceProvider.serviceEndpoints)
            {
                if (endpoint.endpointType == EndpointType.SIGNON)
                {
                    var loginEndpoint = new IndexedEndpoint();
                    loginEndpoint.index = endpoint.endPointIndex;
                    loginEndpoint.isDefault = true;
                    loginEndpoint.Location = new Uri(baseURL, endpoint.localPath).ToString();
                    loginEndpoint.Binding = GetBinding(endpoint.Binding, Saml20Constants.ProtocolBindings.HTTP_Post);
                    signonServiceEndpoints.Add(loginEndpoint);

                    var artifactSignonEndpoint = new IndexedEndpoint();
                    artifactSignonEndpoint.Binding = Saml20Constants.ProtocolBindings.HTTP_SOAP;
                    artifactSignonEndpoint.index = loginEndpoint.index;
                    artifactSignonEndpoint.Location = loginEndpoint.Location;
                    artifactResolutionEndpoints.Add(artifactSignonEndpoint);

                    continue;
                }

                if (endpoint.endpointType == EndpointType.LOGOUT)
                {
                    var logoutEndpoint = new Endpoint();
                    logoutEndpoint.Location = new Uri(baseURL, endpoint.localPath).ToString();
                    logoutEndpoint.ResponseLocation = logoutEndpoint.Location;
                    logoutEndpoint.Binding = GetBinding(endpoint.Binding, Saml20Constants.ProtocolBindings.HTTP_Redirect);
                    logoutServiceEndpoints.Add(logoutEndpoint);

                    var artifactLogoutEndpoint = new IndexedEndpoint();
                    artifactLogoutEndpoint.Binding = Saml20Constants.ProtocolBindings.HTTP_SOAP;
                    artifactLogoutEndpoint.index = endpoint.endPointIndex;
                    artifactLogoutEndpoint.Location = logoutEndpoint.Location;
                    artifactResolutionEndpoints.Add(artifactLogoutEndpoint);

                    continue;
                }
            }

            spDescriptor.SingleLogoutService = logoutServiceEndpoints.ToArray();
            spDescriptor.AssertionConsumerService = signonServiceEndpoints.ToArray();

            // Attribute consuming service. 
            if (config.RequestedAttributes.Attributes.Count > 0)
            {
                var attConsumingService = new AttributeConsumingService();
                spDescriptor.AttributeConsumingService = new[] {attConsumingService};
                attConsumingService.index = signonServiceEndpoints[0].index;
                attConsumingService.isDefault = true;
                attConsumingService.ServiceName = new[] {new LocalizedName("SP", "da")};

                attConsumingService.RequestedAttribute =
                    new RequestedAttribute[config.RequestedAttributes.Attributes.Count];

                for (int i = 0; i < config.RequestedAttributes.Attributes.Count; i++)
                {
                    attConsumingService.RequestedAttribute[i] = new RequestedAttribute();
                    attConsumingService.RequestedAttribute[i].Name = config.RequestedAttributes.Attributes[i].name;
                    if (config.RequestedAttributes.Attributes[i].IsRequired)
                        attConsumingService.RequestedAttribute[i].isRequired = true;
                    attConsumingService.RequestedAttribute[i].NameFormat = SamlAttribute.NAMEFORMAT_BASIC;
                }
            }
            else
            {
                spDescriptor.AttributeConsumingService = new AttributeConsumingService[0];
            }

            if (config.Metadata != null && config.Metadata.IncludeArtifactEndpoints)
                spDescriptor.ArtifactResolutionService = artifactResolutionEndpoints.ToArray();

            entity.Items = new object[] {spDescriptor};

            // Keyinfo
            var keySigning = new KeyDescriptor();
            var keyEncryption = new KeyDescriptor();
            spDescriptor.KeyDescriptor = new[] {keySigning, keyEncryption};

            keySigning.use = KeyTypes.signing;
            keySigning.useSpecified = true;

            keyEncryption.use = KeyTypes.encryption;
            keyEncryption.useSpecified = true;

            // Ugly conversion between the .Net framework classes and our classes ... avert your eyes!!
            keySigning.KeyInfo =
                Serialization.DeserializeFromXmlString<Schema.XmlDSig.KeyInfo>(keyinfo.GetXml().OuterXml);
            keyEncryption.KeyInfo = keySigning.KeyInfo;

            // apply the <Organization> element
            if (config.ServiceProvider.Organization != null)
                entity.Organization = config.ServiceProvider.Organization;

            if (config.ServiceProvider.ContactPerson != null && config.ServiceProvider.ContactPerson.Count > 0)
                entity.ContactPerson = config.ServiceProvider.ContactPerson.ToArray();
        }

        private string GetBinding(SAMLBinding samlBinding, string defaultValue)
        {
            switch (samlBinding)
            {
                case SAMLBinding.ARTIFACT:
                    return Saml20Constants.ProtocolBindings.HTTP_Artifact;
                case SAMLBinding.POST:
                    return Saml20Constants.ProtocolBindings.HTTP_Post;
                case SAMLBinding.REDIRECT:
                    return Saml20Constants.ProtocolBindings.HTTP_Redirect;
                case SAMLBinding.SOAP:
                    return Saml20Constants.ProtocolBindings.HTTP_SOAP;
                case SAMLBinding.NOT_SET:
                    return defaultValue;
                default:
                    throw new ConfigurationErrorsException(String.Format("Unsupported SAML binding {0}",
                                                                         Enum.GetName(typeof (SAMLBinding), samlBinding)));
            }
        }

        /// <summary>
        /// Extract KeyDescriptors from the metadata document represented by this instance.
        /// </summary>
        private void ExtractKeyDescriptors()
        {
            if (_keys != null)
                return;

            if (_entity != null)
            {
                _keys = new List<KeyDescriptor>();
                foreach (object item in _entity.Items)
                {
                    if (item is RoleDescriptor)
                    {
                        var rd = (RoleDescriptor) item;
                        foreach (KeyDescriptor keyDescriptor in rd.KeyDescriptor)
                            _keys.Add(keyDescriptor);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the key descriptors contained in the document
        /// </summary>
        private void ExtractKeyDescriptors(XmlDocument doc)
        {
            XmlNodeList list = doc.GetElementsByTagName(KeyDescriptor.ELEMENT_NAME, Saml20Constants.METADATA);
            _keys = new List<KeyDescriptor>(list.Count);

            foreach (XmlNode node in list)
                _keys.Add(Serialization.DeserializeFromXmlString<KeyDescriptor>(node.OuterXml));
        }

        /// <summary>
        /// Return a string containing the metadata XML based on the settings added to this instance.
        /// The resulting XML will be signed, if the AsymmetricAlgoritm property has been set.
        /// The default encoding (UTF-8) will be used for the resulting XML.
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return ToXml(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the ArtifactResolutionService endpoint.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetARSEndpoint(ushort index)
        {
            IndexedEndpoint ep = _ARSEndpoints[index];
            if (ep != null)
            {
                return ep.Location;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the location of the first AttributeQuery endpoint.
        /// </summary>
        /// <returns></returns>
        public string GetAttributeQueryEndpointLocation()
        {
            List<Endpoint> endpoints = GetAttributeQueryEndpoints();

            if (endpoints.Count == 0)
                throw new Saml20Exception("The identity provider does not support attribute queries.");

            return endpoints[0].Location;
        }

        /// <summary>
        /// Gets all AttributeQuery endpoints.
        /// </summary>
        /// <returns></returns>
        public List<Endpoint> GetAttributeQueryEndpoints()
        {
            if (_attributeQueryEndpoints == null)
            {
                ExtractEndpoints();
            }

            return _attributeQueryEndpoints;
        }

        /// <summary>
        /// Return a string containing the metadata XML based on the settings added to this instance.
        /// The resulting XML will be signed, if the AsymmetricAlgoritm property has been set.
        /// </summary>
        public string ToXml(Encoding enc)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            doc.LoadXml(Serialization.SerializeToXmlString(_entity));

            // Add the correct encoding to the head element.
            if (doc.FirstChild is XmlDeclaration)
                ((XmlDeclaration) doc.FirstChild).Encoding = enc.WebName;
            else
                doc.PrependChild(doc.CreateXmlDeclaration("1.0", enc.WebName, null));

            if (Sign)
                SignDocument(doc);

            return doc.OuterXml;
        }

        private static void SignDocument(XmlDocument doc)
        {
            X509Certificate2 cert =
                ConfigurationInstance<FederationConfig>.GetConfig().SigningCertificate.GetCertificate();

            if (!cert.HasPrivateKey)
                throw new InvalidOperationException("Private key access to the signing certificate is required.");

            var signedXml = new SignedXml(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SigningKey = cert.PrivateKey;

            // Retrieve the value of the "ID" attribute on the root assertion element.                        
            var reference = new Reference("#" + doc.DocumentElement.GetAttribute("ID"));

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.WholeChain));

            signedXml.ComputeSignature();
            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.            
            doc.DocumentElement.InsertBefore(doc.ImportNode(signedXml.GetXml(), true), doc.DocumentElement.FirstChild);
        }

        /// <summary>
        /// Creates a default entity in the 
        /// </summary>
        /// <returns></returns>
        public EntityDescriptor CreateDefaultEntity()
        {
            if (_entity != null)
                throw new InvalidOperationException("An entity is already created in this document.");
            _entity = GetDefaultEntityInstance();
            return _entity;
        }

        private static EntityDescriptor GetDefaultEntityInstance()
        {
            var result = new EntityDescriptor();
            result.ID = "id" + Guid.NewGuid().ToString("N");
            return result;
        }

        #region Properties

        private Dictionary<ushort, IndexedEndpoint> _ARSEndpoints;

        private List<IDPEndPointElement> _AssertionConsumerServiceEndpoints;
        private EntityDescriptor _entity;
        private List<KeyDescriptor> _keys;
        private List<IDPEndPointElement> _SLOEndpoints;
        private List<IDPEndPointElement> _SSOEndpoints;

        /// <summary>
        /// Determines whether the metadata should be signed when the ToXml() method is called.
        /// </summary>
        public bool Sign;

        /// <summary>
        /// The keys contained in the metadata document.
        /// </summary>
        public List<KeyDescriptor> Keys
        {
            get
            {
                if (_keys == null)
                    ExtractKeyDescriptors();

                return _keys;
            }
        }

        /// <summary>
        /// The ID of the entity described in the document.
        /// </summary>
        public string EntityId
        {
            get
            {
                if (_entity != null)
                    return _entity.entityID;

                throw new InvalidOperationException("This instance does not contain a metadata document");
            }
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public EntityDescriptor Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// The SSO endpoints
        /// </summary>
        /// <returns></returns>
        public List<IDPEndPointElement> SSOEndpoints()
        {
            if (_SSOEndpoints == null)
                ExtractEndpoints();

            return _SSOEndpoints;
        }

        /// <summary>
        /// The SLO endpoints.
        /// </summary>
        /// <returns></returns>
        public List<IDPEndPointElement> SLOEndpoints()
        {
            if (_SLOEndpoints == null)
                ExtractEndpoints();

            return _SLOEndpoints;
        }

        /// <summary>
        /// Get the first SLO endpoint that supports the given binding.
        /// </summary>        
        /// <returns>The endpoint or <c>null</c> if metadata does not have an SLO endpoint with the given binding.</returns>
        public IDPEndPointElement SLOEndpoint(SAMLBinding binding)
        {
            return SLOEndpoints().Find(
                delegate(IDPEndPointElement endp) { return endp.Binding == binding; });
        }

        /// <summary>
        /// Get the first SSO endpoint that supports the given binding.
        /// </summary>        
        /// <returns>The endpoint or <c>null</c> if metadata does not have an SSO endpoint with the given binding.</returns>
        public IDPEndPointElement SSOEndpoint(SAMLBinding binding)
        {
            return SSOEndpoints().Find(
                delegate(IDPEndPointElement endp) { return endp.Binding == binding; });
        }


        /// <summary>
        /// Contains the endpoints specified in the &lt;AssertionConsumerService&gt; element in the SPSSODescriptor.
        /// These endpoints are only applicable if we are reading metadata issued by a service provider.
        /// </summary>
        /// <returns></returns>
        public List<IDPEndPointElement> AssertionConsumerServiceEndpoints()
        {
            if (_AssertionConsumerServiceEndpoints == null)
                ExtractEndpoints();

            return _AssertionConsumerServiceEndpoints;
        }

        private void ExtractEndpoints()
        {
            if (_entity != null)
            {
                _SSOEndpoints = new List<IDPEndPointElement>();
                _SLOEndpoints = new List<IDPEndPointElement>();
                _ARSEndpoints = new Dictionary<ushort, IndexedEndpoint>();
                _AssertionConsumerServiceEndpoints = new List<IDPEndPointElement>();
                _attributeQueryEndpoints = new List<Endpoint>();

                foreach (object item in _entity.Items)
                {
                    if (item is IDPSSODescriptor)
                    {
                        var descriptor = (IDPSSODescriptor) item;
                        foreach (Endpoint endpoint in descriptor.SingleSignOnService)
                            _SSOEndpoints.Add(new IDPEndPointElement(endpoint));
                    }

                    if (item is SSODescriptor)
                    {
                        var descriptor = (SSODescriptor) item;

                        if (descriptor.SingleLogoutService != null)
                        {
                            foreach (Endpoint endpoint in descriptor.SingleLogoutService)
                                _SLOEndpoints.Add(new IDPEndPointElement(endpoint));
                        }

                        if (descriptor.ArtifactResolutionService != null)
                        {
                            foreach (IndexedEndpoint ie in descriptor.ArtifactResolutionService)
                            {
                                _ARSEndpoints.Add(ie.index, ie);
                            }
                        }
                    }

                    if (item is SPSSODescriptor)
                    {
                        var descriptor = (SPSSODescriptor) item;
                        foreach (IndexedEndpoint endpoint in descriptor.AssertionConsumerService)
                            _AssertionConsumerServiceEndpoints.Add(new IDPEndPointElement(endpoint));
                    }

                    if (item is AttributeAuthorityDescriptor)
                    {
                        var aad = (AttributeAuthorityDescriptor) item;
                        _attributeQueryEndpoints.AddRange(aad.AttributeService);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the keys marked with the usage given as parameter.
        /// </summary>
        /// <returns>A list containing the keys. If no key is marked with the given usage, the method returns an empty list.</returns>
        public List<KeyDescriptor> GetKeys(KeyTypes usage)
        {
            return Keys.FindAll(delegate(KeyDescriptor desc) { return desc.use == usage; });
        }

        #endregion
    }
}