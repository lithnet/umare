using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "attribute-presence-rule", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [KnownType(typeof(CSEntryAttributePresenceRule))]
    [KnownType(typeof(MVEntryAttributePresenceRule))]
    public abstract class AttributePresenceRule : Rule
    {
        [DataMember(Name = "attribute-name")]
        public string Attribute { get; set; }

        [DataMember(Name = "operator")]
        public PresenceOperator Operator { get; set; }

        protected bool Evaluate(Attrib attribute)
        {
            if (this.Operator == PresenceOperator.IsPresent)
            {
                return attribute.IsPresent;
            }
            else
            {
                return !attribute.IsPresent;
            }
        }
    }
}
