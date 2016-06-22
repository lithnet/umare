using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Common;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "connector-count-rule", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [System.ComponentModel.Description("Connected to rule")]
    public class ConnectorCountRule : Rule
    {
        [DataMember(Name = "ma-name")]
        public string MAName { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "operator")]
        public ValueOperator Operator { get; set; }

        public override bool Evaluate(CSEntry csentry, MVEntry mventry)
        {
            if (mventry == null)
            {
                throw new NotSupportedException(string.Format("The rule type {0} is not supported in this context", this.GetType().Name));
            }

            int actual = mventry.ConnectedMAs.OfType<ConnectedMA>().Count(t => t.Name == this.MAName);
            return ComparisonEngine.CompareLong(actual, this.Count, this.Operator);
        }
    }
}
