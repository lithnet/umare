namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Lithnet.MetadirectoryServices;
    using System.Linq;

    /// <summary>
    /// Concatenates several string together using a specified delimiter
    /// </summary>
    [DataContract(Name = "concat-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Concatenate string")]
    [HandlesOwnMultivaluedInput]
    public class ConcatStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ConcatStringTransform class
        /// </summary>
        public ConcatStringTransform()
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
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
            }
        }

        /// <summary>
        /// Gets or sets the delimiter to use between values
        /// </summary>
        [DataMember(Name = "delimiter")]
        public string Delimiter { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return TypeConverter.ConvertData<string>(inputValue);
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            return new List<object>() { string.Join(this.Delimiter, inputValues.Select(t => TypeConverter.ConvertData<string>(t))) };
        }
    }
}