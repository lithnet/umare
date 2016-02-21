using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Umare;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class FlowRuleAliasCollectionViewModel : ListViewModel<FlowRuleAliasViewModel, FlowRuleAlias>
    {
        private FlowRuleAliasKeyedCollection model;

        public FlowRuleAliasCollectionViewModel(FlowRuleAliasKeyedCollection model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel((IList)this.model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddAlias", t => this.AddAlias());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.PasteableTypes.Add(typeof(FlowRuleAlias));
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Flow Rule Aliases");
            }
        }

        public void AddAlias()
        {
            NewAliasWindow window = new NewAliasWindow();
            NewAliasViewModel vm = new NewAliasViewModel(this, window);
            window.DataContext = vm;
            window.ShowDialog();
        }

        private FlowRuleAliasViewModel ViewModelResolver(FlowRuleAlias model)
        {
            return new FlowRuleAliasViewModel(model);
        }
    }
}