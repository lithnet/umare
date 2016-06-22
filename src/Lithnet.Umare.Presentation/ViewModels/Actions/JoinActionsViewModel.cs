using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class JoinActionsViewModel : ListViewModel<ActionViewModel, Action>
    {
        public JoinActionsViewModel(IList model)
            : base()
        {
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.PasteableTypes.Add(typeof(JoinMappingAction));
            this.PasteableTypes.Add(typeof(DeclineMappingAction));
            this.PasteableTypes.Add(typeof(ExtensionPassThroughAction));
            this.Commands.AddItem("AddJoinMappingAction", t => this.AddJoinMappingAction());
            this.Commands.AddItem("AddDeclineMappingAction", t => this.AddDeclineMappingAction());
            this.Commands.AddItem("AddPassThroughAction", t => this.AddPassThroughAction());
        }

        public string DisplayName
        {
            get
            {
                return "Join mapping actions";
            }
        }

        private void AddJoinMappingAction()
        {
            this.Add(new JoinMappingAction(), true);
        }

        private void AddDeclineMappingAction()
        {
            this.Add(new DeclineMappingAction(), true);
        }

        private void AddPassThroughAction()
        {
            this.Add(new ExtensionPassThroughAction(), true);
        }

        private ActionViewModel ViewModelResolver(Action t)
        {
            if (t is JoinMappingAction)
            {
                return new JoinMappingActionViewModel((JoinMappingAction)t);
            }
            else if (t is DeclineMappingAction)
            {
                return new DeclineMappingActionViewModel((DeclineMappingAction)t);
            }
            else if (t is ExtensionPassThroughAction)
            {
                return new ExtensionPassThroughActionViewModel((ExtensionPassThroughAction)t);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
