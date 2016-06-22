using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Common.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [KnownType(typeof(DeclineMappingAction))]
    [KnownType(typeof(MappingAction))]
    [KnownType(typeof(ExtensionPassThroughAction))]
    public abstract class Action : UINotifyPropertyChanges
    {
        [DataMember(Name = "rule-group")]
        public RuleGroup RuleGroup { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        public bool CanExecute(CSEntry csentry, MVEntry mventry)
        {
            if (this.RuleGroup == null)
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
