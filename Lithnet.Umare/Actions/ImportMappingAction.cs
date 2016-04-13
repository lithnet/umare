using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "import-mapping-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class ImportMappingAction : MappingAction, IImportMappingAction
    {
        [DataMember(Name = "merge-values")]
        public bool MergeValues { get; set; }

        public void MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            AttributeType type;
            IList<object> sourceValues;
            IList<object> returnValues;

            if (this.MergeValues)
            {
                sourceValues = this.GetSourceValuesFromMultipleConnectorsForImport(csentry, mventry, out type);
            }
            else
            {
                sourceValues = this.GetSourceValuesForImport(csentry, out type);
            }

            if (this.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                IList<object> existingTargetValues = this.GetExistingTargetValuesForImportLoopback(mventry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(this.Transforms, sourceValues, existingTargetValues);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(this.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForImport(mventry, returnValues, this.MergeValues);
        }

        private IList<object> GetSourceValuesForImport(CSEntry csentry, out AttributeType attributeType)
        {
            List<object> values = new List<object>();
            attributeType = AttributeType.Undefined;

            foreach (string attributeName in this.SourceAttributes)
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

        private IList<object> GetSourceValuesFromMultipleConnectorsForImport(CSEntry csentry, MVEntry mventry, out AttributeType attributeType)
        {
            List<object> values = new List<object>();
            attributeType = AttributeType.Undefined;

            IEnumerable<CSEntry> csentries = mventry.ConnectedMAs.OfType<ConnectedMA>().Where(t => t.Name == csentry.MA.Name).SelectMany(t => t.Connectors.OfType<CSEntry>());

            foreach (string attributeName in this.SourceAttributes)
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

        private IList<object> GetExistingTargetValuesForImportLoopback(MVEntry mventry)
        {
            List<object> values = new List<object>();

            Attrib attribute = mventry[this.TargetAttribute];

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

        private void SetDestinationAttributeValueForImport(MVEntry mventry, IEnumerable<object> values, bool clearTarget)
        {
            Attrib attribute = mventry[this.TargetAttribute];
            if (clearTarget)
            {
                attribute.Delete();
            }

            this.SetDestinationAttributeValue(attribute, values);
        }
    }
}
