namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.MetadirectoryServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Splits a single value string into multiple values
    /// </summary>
    [DataContract(Name = "string-split", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("String split transform")]
    public class StringSplitTransform : Transform
    {
        private Regex regex;

        private Regex Regex
        {
            get
            {
                if (this.regex == null)
                {
                    this.regex = new Regex(this.SplitRegex);
                }

                return this.regex;
            }
        }
        /// <summary>
        /// Initializes a new instance of the StringSplitTransform class
        /// </summary>
        public StringSplitTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
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
        /// Gets or sets the value to use when comparing the source value with the selector operator
        /// </summary>
        [DataMember(Name = "split-regex")]
        public string SplitRegex { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (inputValue == null)
            {
                return null;
            }

            return this.GetSingleValue(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Gets the matching single value from the multivalued attribute
        /// </summary>
        /// <param name="inputValues">The values to find a match within</param>
        /// <returns>An attribute value</returns>
        private object GetSingleValue(string inputValue)
        {
            string[] values = this.Regex.Split(inputValue);

            return values.ToList<object>();
        }
    }
}