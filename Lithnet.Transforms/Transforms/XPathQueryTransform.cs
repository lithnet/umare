namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.XPath;
    using Lithnet.MetadirectoryServices;
    using System.IO;

    /// <summary>
    /// Find an value in an XML data structure and replaces it with another value from within the XML file
    /// </summary>
    [DataContract(Name = "xpath-query", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("XPath query")]
    public class XPathQueryTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the XmlLookupTransform class
        /// </summary>
        public XPathQueryTransform()
            : base()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the data type that should be returned by the transform
        /// </summary>
        [DataMember(Name = "return-type")]
        public ExtendedAttributeType UserDefinedReturnType { get; set; }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Gets or sets the XPathQuery to use
        /// </summary>
        [DataMember(Name = "xpath-query")]
        public string XPathQuery { get; set; }

        /// <summary>
        /// Gets or sets the action to take when no match was found in the file
        /// </summary>
        [DataMember(Name = "on-missing-match")]
        public OnMissingMatch OnMissingMatch { get; set; }

        /// <summary>
        /// Gets or sets the default value to apply if no match was found in the file
        /// </summary>
        [DataMember(Name = "default-value")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Performs the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.GetXPathMatches(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "XPathQuery":
                    if (string.IsNullOrWhiteSpace(this.XPathQuery))
                    {
                        this.AddError("XPathQuery", "An XPath query must be specified");
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        XPathNavigator nav = doc.CreateNavigator();
                        try
                        {
                            XPathExpression expr = nav.Compile(this.XPathQuery);
                            this.RemoveError("XPathQuery");
                        }
                        catch (XPathException)
                        {
                            this.AddError("XPathQuery", "The XPath query was invalid");
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Looks up the input value in the XML file and returns the replacement value
        /// </summary>
        /// <param name="inputValue">The value to find</param>
        /// <returns>The replacement value</returns>
        private List<object> GetXPathMatches(string inputValue)
        {
            List<object> results = new List<object>();

            XPathDocument doc = new XPathDocument(new StringReader(inputValue));
            XPathNavigator navigator = doc.CreateNavigator();

            object queryResult = navigator.Evaluate(this.XPathQuery);

            if (queryResult is XPathNodeIterator)
            {
                XPathNodeIterator iterator = (XPathNodeIterator)queryResult;
                {
                    while (iterator.MoveNext())
                    {
                        if (iterator.Current.NodeType == XPathNodeType.Text)
                        {
                            results.Add(TypeConverter.ConvertData(iterator.Current.TypedValue, this.UserDefinedReturnType));
                        }
                        else
                        {
                            results.Add(iterator.Current.OuterXml);
                        }
                    }
                }
            }

            if (queryResult == null)
            {
                if (this.OnMissingMatch != Transforms.OnMissingMatch.UseNull)
                {
                    results.Add(this.GetValueFromMissingMatch(inputValue));
                }
            }
            
            return results;
        }

        /// <summary>
        /// Gets the value to use when no match was found
        /// </summary>
        /// <param name="input">The value that could not be matched</param>
        /// <returns>The value to use</returns>
        private string GetValueFromMissingMatch(string input)
        {
            switch (this.OnMissingMatch)
            {
                case OnMissingMatch.UseNull:
                    return null;

                case OnMissingMatch.UseOriginal:
                    return input;

                case OnMissingMatch.UseDefault:
                    return this.DefaultValue;

                default:
                    throw new ArgumentException("The OnMissingMatch value is unknown or unsupported");
            }
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.UserDefinedReturnType = ExtendedAttributeType.String;
        }

        /// <summary>
        /// Performs pre-deserialization initialization
        /// </summary>
        /// <param name="context">The current serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
