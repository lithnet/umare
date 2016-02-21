using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Lithnet.Umare
{
    [DataContract(Name = "ma-operation", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class MAOperation
    {
        [DataMember(Name = "ma-name")]
        public string MAName { get; set; }

        [DataMember(Name = "import-flow-actions")]
        public ActionGroupCollection<IImportMappingAction> ImportFlowActions { get; private set; }

        [DataMember(Name = "export-flow-actions")]
        public ActionGroupCollection<IExportMappingAction> ExportFlowActions { get; private set; }

        [DataMember(Name = "join-flow-actions")]
        public ActionGroupCollection<IJoinMappingAction> JoinActions { get; private set; }

        [DataMember(Name = "deprovision-actions")]
        public ActionGroupCollection<IDeprovisionAction> DeprovisionActions { get; private set; }

        [DataMember(Name = "disconnect-actions")]
        public ActionGroupCollection<IDisconnectAction> DisconnectActions { get; private set; }

        [DataMember(Name = "projection-actions")]
        public ActionGroupCollection<IProjectAction> ProjectionActions { get; private set; }

        [DataMember(Name = "resolve-join-actions")]
        public ActionGroupCollection<IResolveJoinSearchAction> ResolveJoinActions { get; private set; }

        public MAOperation()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.ImportFlowActions = new ActionGroupCollection<IImportMappingAction>();
            this.ExportFlowActions = new ActionGroupCollection<IExportMappingAction>();
            this.JoinActions = new ActionGroupCollection<IJoinMappingAction>();
            this.DeprovisionActions = new ActionGroupCollection<IDeprovisionAction>();
            this.DisconnectActions = new ActionGroupCollection<IDisconnectAction>();
            this.ProjectionActions = new ActionGroupCollection<IProjectAction>();
            this.ResolveJoinActions = new ActionGroupCollection<IResolveJoinSearchAction>();
        }


        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
