using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Transforms;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "export-mapping-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class ExportMappingAction : MappingAction, IExportMappingAction
    {
        public void MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            IList<object> sourceValues = this.GetSourceValuesForExport(mventry);

            IList<object> returnValues;

            if (this.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                object existingTargetValue = this.GetExistingTargetValueForExportLoopback(csentry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(this.Transforms, sourceValues, existingTargetValue);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(this.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForExport(csentry, returnValues);
        }

        private void SetDestinationAttributeValueForExport(CSEntry csentry, IEnumerable<object> values)
        {
            Attrib attribute = csentry[this.TargetAttribute];
            this.SetDestinationAttributeValue(attribute, values);
        }
        private object GetExistingTargetValueForExportLoopback(CSEntry csentry)
        {
            List<object> values = new List<object>();

            Attrib attribute = csentry[this.TargetAttribute];

            if (attribute.Values.Count > 1)
            {
                throw new NotSupportedException(string.Format("Attribute {0} has more than one value which is not supported for a loopback transform", attribute.Name));
            }
            else
            {
                return this.GetSVAttributeValue(attribute);
            }
        }
        private IList<object> GetSourceValuesForExport(MVEntry mventry)
        {
            List<object> values = new List<object>();

            foreach (string attributeName in this.SourceAttributes)
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
    }
}
