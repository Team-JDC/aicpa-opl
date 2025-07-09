using System;
using System.Xml.Serialization;
using dk.nita.saml20.Schema.Protocol;

namespace dk.nita.saml20.Schema.Core
{
    /// <summary>
    /// The &lt;Advice&gt; element contains any additional information that the SAML authority wishes to provide.
    /// This information MAY be ignored by applications without affecting either the semantics or the validity of
    /// the assertion.
    /// </summary>
    // Advice is optional, and there are only implicit demands on the reference types.
    // We do not use it (yet) and let it pass unvalidated.
    [Serializable]
    [XmlType(Namespace = Saml20Constants.ASSERTION)]
    [XmlRoot(ELEMENT_NAME, Namespace = Saml20Constants.ASSERTION, IsNullable = false)]
    public class Advice
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ELEMENT_NAME = "Advice";

        private ItemsChoiceType4[] itemsElementNameField;
        private object[] itemsField;


        /// <summary>
        /// Gets or sets the items.
        /// Items may be of types: Assertion, AssertionIDRef, AssertionURIRef abd EncryptedAssertion
        /// </summary>
        /// <value>The items.</value>        
        [XmlAnyElement]
        [XmlElement("Assertion", typeof (Assertion))]
        [XmlElement("AssertionIDRef", typeof (string), DataType = "NCName")]
        [XmlElement("AssertionURIRef", typeof (string), DataType = "anyURI")]
        [XmlElement("EncryptedAssertion", typeof (EncryptedElement))]
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
        public ItemsChoiceType4[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}