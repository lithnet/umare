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
                IList<object> existingTargetValues = this.GetExistingTargetValuesForExportLoopback(csentry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(this.Transforms, sourceValues, existingTargetValues);
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
               
        private IList<object> GetExistingTargetValuesForExportLoopback(CSEntry csentry)
        {
            List<object> values = new List<object>();

            Attrib attribute = csentry[this.TargetAttribute];

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
