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
    [DataContract(Name = "join-mapping-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class JoinMappingAction : MappingAction, IJoinMappingAction
    {
        public void MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            AttributeType type;
            IList<object> sourceValues = this.GetSourceValuesForImport(csentry, out type);
            IList<object> returnValues = Transform.ExecuteTransformChain(this.Transforms, sourceValues);

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
    }
}
