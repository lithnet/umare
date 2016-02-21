namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.MetadirectoryServices;

    /// <summary>
    /// Allows a flow of a string attribute only if value (not the case) has changed
    /// </summary>
    [LoopbackTransform]
    [DataContract(Name = "conditional-string-flow", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Condition flow on string comparison")]
    public class ConditionalStringFlowTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ConditionalStringFlow class
        /// </summary>
        public ConditionalStringFlowTransform()
        {
            this.Initialize();
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
        /// Gets or sets the type of string comparison to perform
        /// </summary>
        [DataMember(Name = "comparison-type")]
        public StringComparison ComparisonType { get; set; }

        /// <summary>
        /// This method is unsupported in this transform type
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>This method always throws an exception</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            throw new NotSupportedException("This transform does not support simple transforms");
        }

        protected override object TransformMultiValuesWithLoopback(IList<object> inputValues, object targetValue)
        {
            if (inputValues.Count > 1)
            {
                throw new ArgumentException("This transform only supports a single input value");
            }

            string inputValue = TypeConverter.ConvertData<string>(inputValues.First());

            return this.GetReturnValue(inputValue, TypeConverter.ConvertData<string>(targetValue));
        }

        private string GetReturnValue(string inputValue, string targetValue)
        {
            if (string.Equals(inputValue, targetValue, this.ComparisonType))
            {
                return targetValue;
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.ComparisonType = StringComparison.CurrentCultureIgnoreCase;
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