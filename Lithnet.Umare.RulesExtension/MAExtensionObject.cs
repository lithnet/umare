using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Transforms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Lithnet.Umare
{
    /// <summary>
    /// Summary description for MAExtensionObject.
    /// </summary>
    public class MAExtensionObject : IMASynchronization
    {
        public XmlConfigFile config;

        public MAExtensionObject()
        {
            TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
            this.config = new XmlConfigFile();
        }

        void IMASynchronization.Initialize()
        {
            string path = MAExtensionObject.AppConfigPath;

            if (!System.IO.File.Exists(path))
            {
                path = this.FindUmarex();

                if (path == null)
                {
                    throw new FileNotFoundException(
@"Unable to find a UMAREX configuration file. To resolve this, either
1. Use the UMARE editor to create a file and place it in the Sync Engine extensions folder, or
2. Modify the Lithnet.Umare.RulesExtension.dll.config file in the extensions folder to point to the appropriate path containing your umarex file");
                }
            }

            this.config = ConfigManager.LoadXml(path);
        }

        void IMASynchronization.Terminate()
        {
            ExtensionPassThroughAction.TerminateInitializedExtensions();
        }

        bool IMASynchronization.ShouldProjectToMV(CSEntry csentry, out string MVObjectType)
        {
            throw new EntryPointNotImplementedException();
        }

        DeprovisionAction IMASynchronization.Deprovision(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        bool IMASynchronization.FilterForDisconnection(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        void IMASynchronization.MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Join);

            if (!parameters.ShouldFlow(csentry, null))
            {
                throw new DeclineMappingException();
            }

            AttributeType type;
            IList<object> sourceValues = this.GetSourceValuesForImport(parameters, csentry, out type);
            IList<object> returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);

            foreach (object value in returnValues)
            {
                switch (type)
                {
                    case AttributeType.Binary:
                        values.Add(TypeConverter.ConvertData<byte[]>(value));
                        break;

                    case AttributeType.Integer:
                        values.Add(TypeConverter.ConvertData<long>(value));
                        break;

                    case AttributeType.String:
                        values.Add(TypeConverter.ConvertData<string>(value));
                        break;

                    case AttributeType.Reference:
                    case AttributeType.Boolean:
                    case AttributeType.Undefined:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
        }

        bool IMASynchronization.ResolveJoinSearch(string joinCriteriaName, CSEntry csentry, MVEntry[] rgmventry, out int imventry, ref string MVObjectType)
        {
            throw new EntryPointNotImplementedException();
        }

        void IMASynchronization.MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Import);

            if (!parameters.ShouldFlow(csentry, mventry))
            {
                throw new DeclineMappingException();
            }

            AttributeType type;
            IList<object> sourceValues;
            IList<object> returnValues;

            if (parameters.MergeValues)
            {
                sourceValues = this.GetSourceValuesFromMultipleConnectorsForImport(parameters, csentry, mventry, out type);
            }
            else
            {
                sourceValues = this.GetSourceValuesForImport(parameters, csentry, out type);
            }

            if (parameters.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                IList<object> existingTargetValues = this.GetExistingTargetValueForImportLoopback(parameters, mventry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(parameters.Transforms, sourceValues, existingTargetValues);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForImport(parameters, mventry, returnValues, parameters.MergeValues);
        }

        void IMASynchronization.MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Export);

            if (!parameters.ShouldFlow(csentry, mventry))
            {
                throw new DeclineMappingException();
            }

            IList<object> sourceValues = this.GetSourceValuesForExport(parameters, mventry);

            IList<object> returnValues;

            if (parameters.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                IList<object> existingTargetValues = this.GetExistingTargetValueForExportLoopback(parameters, csentry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(parameters.Transforms, sourceValues, existingTargetValues);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForExport(parameters, csentry, returnValues);
        }

        private IList<object> GetSourceValuesForImport(FlowRuleParameters parameters, CSEntry csentry, out AttributeType attributeType)
        {
            List<object> values = new List<object>();
            attributeType = AttributeType.Undefined;

            foreach (string attributeName in parameters.SourceAttributeNames)
            {
                Attrib attribute = csentry[attributeName];

                if (attributeType == AttributeType.Undefined)
                {
                    attributeType = attribute.DataType;
                }
                else if (attributeType != attribute.DataType)
                {
                    attributeType = AttributeType.String;
                }

                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private IList<object> GetSourceValuesFromMultipleConnectorsForImport(FlowRuleParameters parameters, CSEntry csentry, MVEntry mventry, out AttributeType attributeType)
        {
            List<object> values = new List<object>();
            attributeType = AttributeType.Undefined;

            IEnumerable<CSEntry> csentries = mventry.ConnectedMAs.OfType<ConnectedMA>().Where(t => t.Name == csentry.MA.Name).SelectMany(t => t.Connectors.OfType<CSEntry>());

            foreach (string attributeName in parameters.SourceAttributeNames)
            {
                foreach (CSEntry othercsentry in csentries.Where(t => t.ObjectType == csentry.ObjectType))
                {
                    Attrib attribute = othercsentry[attributeName];

                    if (attributeType == AttributeType.Undefined)
                    {
                        attributeType = attribute.DataType;
                    }
                    else if (attributeType != attribute.DataType)
                    {
                        attributeType = AttributeType.String;
                    }

                    if (attribute.IsMultivalued)
                    {
                        values.AddRange(this.GetMVAttributeValue(attribute));
                    }
                    else
                    {
                        values.Add(this.GetSVAttributeValue(attribute));
                    }
                }
            }

            return values;
        }


        private IList<object> GetSourceValuesForExport(FlowRuleParameters parameters, MVEntry mventry)
        {
            List<object> values = new List<object>();

            foreach (string attributeName in parameters.SourceAttributeNames)
            {
                Attrib attribute = mventry[attributeName];

                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private IList<object> GetExistingTargetValueForImportLoopback(FlowRuleParameters parameters, MVEntry mventry)
        {
            List<object> values = new List<object>();

            Attrib attribute = mventry[parameters.TargetAttributeName];
            
            if (attribute.IsPresent)
            {
                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private IList<object> GetExistingTargetValueForExportLoopback(FlowRuleParameters parameters, CSEntry csentry)
        {
            List<object> values = new List<object>();

            Attrib attribute = csentry[parameters.TargetAttributeName];

            if (attribute.IsPresent)
            {
                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private void SetDestinationAttributeValueForImport(FlowRuleParameters parameters, MVEntry mventry, IList<object> values, bool clearTarget)
        {
            Attrib attribute = mventry[parameters.TargetAttributeName];
            if (clearTarget)
            {
                attribute.Delete();
            }

            this.SetDestinationAttributeValue(parameters, values, attribute);
        }

        private void SetDestinationAttributeValueForExport(FlowRuleParameters parameters, CSEntry csentry, IList<object> values)
        {
            Attrib attribute = csentry[parameters.TargetAttributeName];
            this.SetDestinationAttributeValue(parameters, values, attribute);
        }

        private void SetDestinationAttributeValue(FlowRuleParameters parameters, IList<object> values, Attrib attribute)
        {
            if (attribute.IsMultivalued)
            {
                if (values.Count == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValues(values, attribute);
                }
            }
            else
            {
                if (values.Count > 1)
                {
                    throw new TooManyValuesException(parameters.TargetAttributeName);
                }
                else if (values.Count == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValue(values.First(), attribute);
                }
            }
        }

        private void SetAttributeValue(object attributeValue, Attrib attribute)
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

        private void SetAttributeValues(IList<object> attributeValues, Attrib attribute)
        {
            if (attribute.Values.Count > 0)
            {
                attribute.Values.Clear();
            }

            if (!attribute.IsMultivalued && attributeValues.Count > 1)
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

        private object GetSVAttributeValue(Attrib attribute)
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

        private IList<object> GetMVAttributeValue(Attrib attribute)
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

        private static Configuration appConfig;

        public static string AppConfigPath
        {
            get
            {
                KeyValueConfigurationElement element = MAExtensionObject.AppConfig.AppSettings.Settings["ConfigFile"];
                if (element != null)
                {
                    string value = element.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }

                return string.Empty;
            }
        }

        public static Configuration AppConfig
        {
            get
            {
                if (MAExtensionObject.appConfig == null)
                {
                    Uri exeConfigUri = new Uri(typeof(MAExtensionObject).Assembly.CodeBase);
                    MAExtensionObject.appConfig = ConfigurationManager.OpenExeConfiguration(exeConfigUri.LocalPath);
                }

                return MAExtensionObject.appConfig;
            }
        }

        private string FindUmarex()
        {
            return Directory.EnumerateFiles(Microsoft.MetadirectoryServices.Utils.ExtensionsDirectory, "*.umarex", SearchOption.TopDirectoryOnly).FirstOrDefault();
        }
    }
}