using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class ActionGroupCollectionViewModel : ListViewModel<ActionGroupViewModel, ActionGroup>
    {
        private string displayName;

        public ActionGroupCollectionViewModel(IList model, string name, Func<ActionGroup, ActionGroupViewModel> viewModelResolver)
            : base()
        {
            this.displayName = name;
            this.SetCollectionViewModel((IList)model, viewModelResolver);
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.Commands.AddItem("AddActionGroup", t => this.AddActionGroup());
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        private void AddActionGroup()
        {
            this.Add(new ActionGroup() { Name = "New flow rule group" });
        }
    }
}
