using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

namespace Lithnet.Umare.Presentation
{
    public class ActionsViewModel : ListViewModel<ActionViewModel, Action>
    {
        public ActionsViewModel(ObservableCollection<Action> model)
            : base()
        {
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        private ActionViewModel ViewModelResolver(Action t)
        {
            if (t is ImportMappingAction)
            {
                return new ImportMappingActionViewModel((ImportMappingAction)t);
            }
            else if (t is DeclineMappingAction)
            {
                return new DeclineMappingActionViewModel((DeclineMappingAction)t);
            }
            else if (t is ExtensionPassThroughAction)
            {
                return new ExtensionPassThroughActionViewModel((ExtensionPassThroughAction)t);
            }
            else if (t is ExportMappingAction)
            {
                return new ExportMappingActionViewModel((ExportMappingAction)t);
            }
            else if (t is JoinMappingAction)
            {
                return new JoinMappingActionViewModel((JoinMappingAction)t);
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        internal void AddImportMappingAction()
        {
            this.Add(new ImportMappingAction(), true);
        }

        internal void AddDeclineMappingAction()
        {
            this.Add(new DeclineMappingAction(), true);
        }

        internal void AddPassThroughAction()
        {
            this.Add(new ExtensionPassThroughAction(), true);
        }

        internal void AddExportMappingAction()
        {
            this.Add(new ExportMappingAction(), true);
        }

        internal void AddJoinMappingAction()
        {
            this.Add(new JoinMappingAction(), true);
        }

    }
}
