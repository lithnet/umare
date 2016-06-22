using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;
using Lithnet.Transforms;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Umare
{
    [DataContract(Name = "Lithnet.Fim.UniversalMARE", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class XmlConfigFile : UINotifyPropertyChanges, IExtensibleDataObject
    {
        public XmlConfigFile()
        {
            this.Initialize();
        }

        [DataMember(Name = "transforms")]
        public TransformKeyedCollection Transforms { get; private set; }

        [DataMember(Name = "flow-rule-aliases")]
        public FlowRuleAliasKeyedCollection FlowRuleAliases { get; private set; }

        [DataMember(Name = "ma-operations")]
        public MAOperations MAOperations { get; private set; }

        public ExtensionDataObject ExtensionData { get; set; }

        public string FileName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        private void Initialize()
        {
            this.Transforms = new TransformKeyedCollection();
            this.FlowRuleAliases = new FlowRuleAliasKeyedCollection();
            this.MAOperations = new MAOperations();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
