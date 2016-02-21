using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "mventry-attribute-presence-rule", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [System.ComponentModel.Description("Metaverse attribute presence rule")]
    public class MVEntryAttributePresenceRule : AttributePresenceRule
    {
        public override bool Evaluate(CSEntry csentry, MVEntry mventry)
        {
            if (mventry == null)
            {
                throw new NotSupportedException(string.Format("The rule type {0} is not supported in this context", this.GetType().Name));
            }

            return base.Evaluate(mventry[this.Attribute]);
        }
    }
}
