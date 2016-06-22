using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "action-group", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [KnownType(typeof(DeclineMappingAction))]
    [KnownType(typeof(ExportMappingAction))]
    [KnownType(typeof(ImportMappingAction))]
    [KnownType(typeof(JoinMappingAction))]
    [KnownType(typeof(ExtensionPassThroughAction))]
    [KnownType(typeof(MappingAction))]
    public class ActionGroup
    {
        [DataMember(Name = "rule-group")]
        public RuleGroup RuleGroup { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name="actions")]
        public ObservableCollection<Action> Actions { get; set; }

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

        private void Initialize()
        {
            this.Actions = new ObservableCollection<Action>();
        }

        public ActionGroup()
        {
            this.Initialize();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
