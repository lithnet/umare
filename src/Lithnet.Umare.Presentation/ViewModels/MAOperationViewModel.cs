using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class MAOperationViewModel : ViewModelBase<MAOperation>
    {
        public MAOperationViewModel(MAOperation model)
            : base(model)
        {
            this.ImportMappingActions = new ActionGroupCollectionViewModel(model.ImportFlowActions, "Import mappings", (t) => this.ViewModelResolverImportActionGroup(t));
            this.ExportMappingActions = new ActionGroupCollectionViewModel(model.ExportFlowActions, "Export mappings", (t) => this.ViewModelResolverExportActionGroup(t));
            this.JoinMappingActions = new ActionGroupCollectionViewModel(model.JoinActions, "Join mappings", (t) => this.ViewModelResolverJoinActionGroup(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Commands.AddItem("DeleteManagementAgent", t => this.DeleteManagementAgent());
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                if (this.ImportMappingActions != null)
                {
                    yield return this.ImportMappingActions;
                }

                if (this.ExportMappingActions != null)
                {
                    yield return this.ExportMappingActions;
                }

                if (this.JoinMappingActions != null)
                {
                    yield return this.JoinMappingActions;
                }
            }
        }

        public string MAName
        {
            get
            {
                return this.Model.MAName;
            }
            set
            {
                this.Model.MAName = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.MAName;
            }
        }

        public ActionGroupCollectionViewModel ImportMappingActions { get; set; }

        public ActionGroupCollectionViewModel ExportMappingActions { get; set; }

        public ActionGroupCollectionViewModel JoinMappingActions { get; set; }

        private void DeleteManagementAgent()
        {
            this.ParentCollection.Remove(this.Model);
        }


        private ActionGroupViewModel ViewModelResolverImportActionGroup(ActionGroup t)
        {
            return new ImportActionGroupViewModel(t);
        }

        private ActionGroupViewModel ViewModelResolverExportActionGroup(ActionGroup t)
        {
            return new ExportActionGroupViewModel(t);
        }

        private ActionGroupViewModel ViewModelResolverJoinActionGroup(ActionGroup t)
        {
            return new JoinMappingActionGroupViewModel(t);
        }

        protected override bool CanMoveDown()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override bool CanMoveUp()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
