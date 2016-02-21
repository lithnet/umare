// -----------------------------------------------------------------------
// <copyright file="AdvancedComparisonRule.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Umare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using System.Runtime.Serialization;
    using Lithnet.Transforms;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines a rule that can be used to determine the presence of an attribute
    /// </summary>    
    [DataContract(Name = "comparison-rule", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [System.ComponentModel.Description("Value comparison rule")]
    public class ValueComparisonRule : Rule
    {
        public ValueComparisonRule()
        {
            this.Initialize();
        }

        [DataMember(Name = "left-target")]
        public ComparisonTarget LeftTarget { get; set; }

        [DataMember(Name = "right-target")]
        public ComparisonTarget RightTarget { get; set; }

        [DataMember(Name = "left-value")]
        public string LeftValue { get; set; }

        [DataMember(Name = "right-value")]
        public string RightValue { get; set; }

        [DataMember(Name = "left-transforms")]
        public string LeftTransformString { get; set; }

        [DataMember(Name = "right-transforms")]
        public string RightTransformString { get; set; }

        private List<Transform> leftTransforms;

        private List<Transform> rightTransforms;

        private List<Transform> LeftTransforms
        {
            get
            {
                if (this.leftTransforms == null)
                {
                    this.leftTransforms = this.GetTransforms(this.LeftTransformString);
                }

                return this.leftTransforms;
            }
        }

        private List<Transform> RightTransforms
        {
            get
            {
                if (this.rightTransforms == null)
                {
                    this.rightTransforms = this.GetTransforms(this.RightTransformString);
                }

                return this.rightTransforms;
            }
        }

        /// <summary>
        /// Gets the operator to apply to the attribute value
        /// </summary>
        [DataMember(Name = "operator")]
        public ValueOperator ValueOperator { get; set; }

        [DataMember(Name = "compare-as")]
        public ExtendedAttributeType CompareAs { get; set; }

        /// <summary>
        /// Gets the conditions required to apply this rule to a multivalued attribute
        /// </summary>
        [DataMember(Name = "multivalued-condition")]
        public GroupOperator GroupOperator { get; set; }

        public override bool Evaluate(CSEntry csentry, MVEntry mventry)
        {
            if (mventry == null)
            {
                if (this.LeftTarget == ComparisonTarget.MetaverseObject || this.RightTarget == ComparisonTarget.MetaverseObject)
                {
                    throw new NotSupportedException(string.Format("The rule type {0} is not supported in this context", this.GetType().Name));
                }
            }

            if (csentry == null)
            {
                if (this.LeftTarget == ComparisonTarget.ConnectorSpaceObject || this.RightTarget == ComparisonTarget.ConnectorSpaceObject)
                {
                    throw new NotSupportedException(string.Format("The rule type {0} is not supported in this context", this.GetType().Name));
                }
            }

            IList<object> sourceValues = this.GetLeftValues(csentry, mventry);
            IList<object> targetValues = this.GetRightValues(csentry, mventry);

            if (sourceValues.Count == 0)
            {
                if (this.CompareAs == ExtendedAttributeType.Boolean)
                {
                    sourceValues.Add(false);
                }
                else
                {
                    if (this.ValueOperator == ValueOperator.NotEquals)
                    {
                        return true;
                    }
                    else
                    {
                        this.RaiseRuleFailure("Source declaration had no values");
                        return false;
                    }
                }
            }

            if (this.ValueOperator == ValueOperator.IsPresent)
            {
                if (sourceValues.Any(t => t != null))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("No values were present");
                    return false;
                }
            }

            if (this.ValueOperator == ValueOperator.NotPresent)
            {
                if (sourceValues.Count == 0 || sourceValues.All(t => t == null))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("At least one value was present");
                    return false;
                }
            }

            bool hasSuccess = false;

            foreach (object value in sourceValues)
            {

                bool result = this.EvaluateAttributeValue(value, targetValues);

                switch (this.GroupOperator)
                {
                    case GroupOperator.None:
                        if (result)
                        {
                            this.RaiseRuleFailure("Group operator was set to 'none', but an evaluation succeeded");
                            return false;
                        }

                        break;

                    case GroupOperator.All:
                        if (!result)
                        {
                            this.RaiseRuleFailure("Group operator was set to 'all', but an evaluation failed");
                            return false;
                        }

                        break;

                    case GroupOperator.Any:
                        if (result)
                        {
                            return true;
                        }

                        break;

                    case GroupOperator.One:
                        if (result)
                        {
                            if (hasSuccess)
                            {
                                this.RaiseRuleFailure("Group operator was set to 'one', but a second evaluation succeeded");
                                return false;
                            }
                        }

                        break;
                }

                if (result)
                {
                    hasSuccess = true;
                }
            }

            if (hasSuccess && ((this.GroupOperator == GroupOperator.All) || (this.GroupOperator == GroupOperator.One)))
            {
                return true;
            }
            else if (!hasSuccess && this.GroupOperator == GroupOperator.None)
            {
                return true;
            }
            else
            {
                this.RaiseRuleFailure("No evaluations succeeded");
                return false;
            }
        }

        /// <summary>
        /// Evaluates a specific value against the rule
        /// </summary>
        /// <param name="actualValue">The value to evaluate</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        protected bool EvaluateAttributeValue(object actualValue, IList<object> expectedValues)
        {
            if (actualValue == null)
            {
                if (this.CompareAs == ExtendedAttributeType.Boolean)
                {
                    actualValue = false;
                }
                else
                {
                    this.RaiseRuleFailure(string.Format("The source value was null"));
                    return false;
                }
            }

            bool result = false;

            foreach (object expectedValue in expectedValues)
            {
                switch (this.CompareAs)
                {
                    case ExtendedAttributeType.Binary:
                        result = ComparisonEngine.CompareBinary(TypeConverter.ConvertData<byte[]>(actualValue), TypeConverter.ConvertData<byte[]>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Boolean:
                        result = ComparisonEngine.CompareBoolean(TypeConverter.ConvertData<bool>(actualValue), TypeConverter.ConvertData<bool>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Integer:
                        result = ComparisonEngine.CompareLong(TypeConverter.ConvertData<long>(actualValue), TypeConverter.ConvertData<long>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.DateTime:
                        result = ComparisonEngine.CompareDateTime(TypeConverter.ConvertData<DateTime>(actualValue), TypeConverter.ConvertData<DateTime>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.String:
                        result = ComparisonEngine.CompareString(TypeConverter.ConvertData<string>(actualValue), TypeConverter.ConvertData<string>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Reference:
                        result = ComparisonEngine.CompareString(TypeConverter.ConvertData<string>(actualValue), TypeConverter.ConvertData<string>(expectedValue), this.ValueOperator);
                        break;

                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }

                if (result)
                {
                    break;
                }
            }

            if (!result)
            {
                this.RaiseRuleFailure("Comparison failed\nComparison Operator: {0}\nExpected Values: {1}\nActual Value: {2}", this.ValueOperator.ToString(), expectedValues.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString(), actualValue.ToSmartString());
            }

            return result;
        }

        private void Initialize()
        {
            this.GroupOperator = GroupOperator.Any;
            this.ValueOperator = ValueOperator.Equals;
        }

        private IList<object> GetLeftValues(CSEntry csentry, MVEntry mventry)
        {
            switch (this.LeftTarget)
            {
                case ComparisonTarget.MetaverseObject:
                    return this.GetLeftValues(mventry);

                case ComparisonTarget.ConnectorSpaceObject:
                    return this.GetLeftValues(csentry);

                case ComparisonTarget.Constant:
                    return this.GetLeftConstantValue();

                default:
                    throw new NotSupportedException("Unknown comparison type");
            }
        }

        private IList<object> GetRightValues(CSEntry csentry, MVEntry mventry)
        {
            switch (this.RightTarget)
            {
                case ComparisonTarget.MetaverseObject:
                    return this.GetRightValues(mventry);

                case ComparisonTarget.ConnectorSpaceObject:
                    return this.GetRightValues(csentry);

                case ComparisonTarget.Constant:
                    return this.GetRightConstantValue();

                default:
                    throw new NotSupportedException("Unknown comparison type");
            }
        }

        private IList<object> GetLeftValues(CSEntry csentry)
        {
            return this.LeftTransform(csentry[this.LeftValue].Values.ToList());
        }

        private IList<object> GetLeftValues(MVEntry mventry)
        {
            return this.LeftTransform(mventry[this.LeftValue].Values.ToList());
        }

        private IList<object> GetRightValues(CSEntry csentry)
        {
            return this.RightTransform(csentry[this.RightValue].Values.ToList());
        }

        private IList<object> GetRightValues(MVEntry mventry)
        {
            return this.RightTransform(mventry[this.RightValue].Values.ToList());
        }

        private IList<object> GetLeftConstantValue()
        {
            return this.LeftTransform(new List<object>() { this.LeftValue });
        }

        private IList<object> GetRightConstantValue()
        {
            return this.RightTransform(new List<object>() { this.RightValue });
        }

        private IList<object> LeftTransform(IList<object> values)
        {
            return Transform.ExecuteTransformChain(this.LeftTransforms, values);
        }

        private IList<object> RightTransform(IList<object> values)
        {
            return Transform.ExecuteTransformChain(this.RightTransforms, values);
        }

        private List<Transform> GetTransforms(string transformString)
        {
            if (string.IsNullOrWhiteSpace(transformString))
            {
                return new List<Transform>();
            }

            Match match = Regex.Match(transformString, @".*?>>(?<transform>.+)>>.+");

            if (match.Success)
            {
                string transformNames = match.Groups["transform"].Captures[0].Value;
                string[] transformNamesSplit = Regex.Split(transformNames, ">>");

                return this.PopulateTransformsFromNames(transformNamesSplit);
            }
            else
            {
                throw new ArgumentException("The transform list was not in a supported format");
            }
        }

        private List<Transform> PopulateTransformsFromNames(IEnumerable<string> transformNamesSplit)
        {
            List<Transform> list = new List<Transform>();

            foreach (string transformName in transformNamesSplit)
            {
                if (ConfigManager.CurrentConfig.Transforms.Contains(transformName))
                {
                    list.Add(ConfigManager.CurrentConfig.Transforms[transformName]);
                }
                else
                {
                    throw new NotFoundException("The specified transform was not found: " + transformName);
                }
            }

            return list;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}