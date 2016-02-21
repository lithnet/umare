using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Transforms;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "mapping-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [KnownType(typeof(ExportMappingAction))]
    [KnownType(typeof(ImportMappingAction))]
    [KnownType(typeof(JoinMappingAction))]
    public abstract class MappingAction : Action
    {
        private List<Transform> transforms;

        [DataMember(Name = "source-attributes")]
        public List<string> SourceAttributes { get; set; }

        [DataMember(Name = "target-attribute")]
        public string TargetAttribute { get; set; }

        [DataMember(Name = "transforms")]
        public string TransformString { get; set; }

        public MappingAction()
        {
            this.Initialize();
        }

        protected List<Transform> Transforms
        {
            get
            {
                if (this.transforms == null)
                {
                    this.PopulateTransformsFromNames(this.TransformString.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries));
                }

                return this.transforms;
            }
        }

        protected void SetDestinationAttributeValue(Attrib attribute, IEnumerable<object> values)
        {
            if (attribute.IsMultivalued)
            {
                if (values.Count() == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValues(attribute, values);
                }
            }
            else
            {
                if (values.Count() > 1)
                {
                    throw new TooManyValuesException(attribute.Name);
                }
                else if (values.Count() == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValue(attribute, values.First());
                }
            }
        }

        protected void SetAttributeValue(Attrib attribute, object attributeValue)
        {
            switch (attribute.DataType)
            {
                case AttributeType.Binary:
                    attribute.BinaryValue = TypeConverter.ConvertData<byte[]>(attributeValue);
                    break;

                case AttributeType.Boolean:
                    attribute.BooleanValue = TypeConverter.ConvertData<bool>(attributeValue);
                    break;

                case AttributeType.Integer:
                    attribute.IntegerValue = TypeConverter.ConvertData<long>(attributeValue);
                    break;

                case AttributeType.String:
                    attribute.StringValue = TypeConverter.ConvertData<string>(attributeValue);
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        protected void SetAttributeValues(Attrib attribute, IEnumerable<object> attributeValues)
        {
            if (attribute.Values.Count > 0)
            {
                attribute.Values.Clear();
            }

            if (!attribute.IsMultivalued && attributeValues.Count() > 1)
            {
                throw new TooManyValuesException(attribute.Name);
            }

            foreach (object value in attributeValues)
            {
                if (value == null)
                {
                    continue;
                }

                switch (attribute.DataType)
                {
                    case AttributeType.Binary:
                        attribute.Values.Add(TypeConverter.ConvertData<byte[]>(value));
                        break;

                    case AttributeType.Integer:
                        attribute.Values.Add(TypeConverter.ConvertData<long>(value));
                        break;

                    case AttributeType.String:
                        attribute.Values.Add(TypeConverter.ConvertData<string>(value));
                        break;

                    case AttributeType.Boolean:
                    case AttributeType.Reference:
                    case AttributeType.Undefined:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
        }

        protected object GetSVAttributeValue(Attrib attribute)
        {
            object csEntryAttributeValue;

            if (!attribute.IsPresent || attribute.Values.Count <= 0)
            {
                if (attribute.DataType == AttributeType.Boolean)
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }

            switch (attribute.DataType)
            {
                case AttributeType.Binary:
                    csEntryAttributeValue = attribute.Values[0].ToBinary();
                    break;

                case AttributeType.Boolean:
                    csEntryAttributeValue = attribute.Values[0].ToBoolean();
                    break;

                case AttributeType.Integer:
                    csEntryAttributeValue = attribute.Values[0].ToInteger();
                    break;

                case AttributeType.String:
                    csEntryAttributeValue = attribute.Values[0].ToString();
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
            return csEntryAttributeValue;
        }

        protected IEnumerable<object> GetMVAttributeValue(Attrib attribute)
        {
            List<object> attributeValues = new List<object>();

            foreach (Value item in attribute.Values)
            {
                object value;

                switch (item.DataType)
                {
                    case AttributeType.Binary:
                        value = item.ToBinary();
                        break;

                    case AttributeType.Integer:
                        value = item.ToInteger();
                        break;

                    case AttributeType.String:
                        value = item.ToString();
                        break;

                    case AttributeType.Boolean:
                    case AttributeType.Reference:
                    case AttributeType.Undefined:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }

                attributeValues.Add(value);
            }

            return attributeValues;
        }

        private void PopulateTransformsFromNames(IEnumerable<string> transformNamesSplit)
        {
            this.transforms = new List<Transform>();

            foreach (string transformName in transformNamesSplit)
            {
                if (ConfigManager.CurrentConfig.Transforms.Contains(transformName))
                {
                    this.transforms.Add(ConfigManager.CurrentConfig.Transforms[transformName]);
                }
                else
                {
                    throw new NotFoundException("The specified transform was not found: " + transformName);
                }
            }
        }

        private void Initialize()
        {
            this.transforms = new List<Transform>();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
