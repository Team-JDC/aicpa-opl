using System;
using System.Xml.Serialization;

namespace dk.nita.saml20.Schema.Core
{
    /// <summary>
    /// The &lt;AuthnContext&gt; element specifies the context of an authentication event. The element can contain
    /// an authentication context class reference, an authentication context declaration or declaration reference,
    /// or both.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.ASSERTION)]
    [XmlRoot(ELEMENT_NAME, Namespace = Saml20Constants.ASSERTION, IsNullable = false)]
    public class AuthnContext
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ELEMENT_NAME = "AuthnContext";

        private string[] authenticatingAuthorityField;
        private ItemsChoiceType5[] itemsElementNameField;
        private object[] itemsField;


        /// <summary>
        /// Gets or sets the items.
        /// Items may be of types: AuthnContextClassRef, AuthnContextDecl and AuthnContextDeclRef
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("AuthnContextClassRef", typeof (string), DataType = "anyURI")]
        [XmlElement("AuthnContextDecl", typeof (object))]
        [XmlElement("AuthnContextDeclRef", typeof (string), DataType = "anyURI")]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }


        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType5[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }


        /// <summary>
        /// Gets or sets the authenticating authority.
        /// Zero or more unique identifiers of authentication authorities that were involved in the authentication of
        /// the principal (not including the assertion issuer, who is presumed to have been involved without being
        /// explicitly named here).
        /// </summary>
        /// <value>The authenticating authority.</value>
        [XmlElement("AuthenticatingAuthority", DataType = "anyURI")]
        public string[] AuthenticatingAuthority
        {
            get { return authenticatingAuthorityField; }
            set { authenticatingAuthorityField = value; }
        }
    }
}