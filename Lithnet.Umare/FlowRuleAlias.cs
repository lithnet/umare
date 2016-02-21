using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Schema;
using Lithnet.Common.ObjectModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "flow-rule-alias", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class FlowRuleAlias : UINotifyPropertyChanges, IExtensibleDataObject
    {
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "definition")]
        public string FlowRuleDefinition { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "flow-conditions")]
        public RuleGroup RuleGroup { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }

        public bool ShouldFlow(CSEntry csentry, MVEntry mventry)
        {
            if (this.RuleGroup == null || this.RuleGroup.Items.Count == 0)
            {
                return true;
            }
            else
            {
                return this.RuleGroup.Evaluate(csentry, mventry);
            }
        }
    }
}
