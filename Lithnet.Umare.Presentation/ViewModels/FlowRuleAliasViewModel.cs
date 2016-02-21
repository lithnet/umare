using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Umare;
using System;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class FlowRuleAliasViewModel : ViewModelBase<FlowRuleAlias>
    {
        private RuleGroupViewModel ruleGroup;

        public FlowRuleAliasViewModel(FlowRuleAlias model)
            : base(model)
        {
            this.Commands.AddItem("DeleteAlias", t => this.DeleteAlias());
            this.Commands.AddItem("AddConditions", t => this.AddFlowConditions(), t => this.CanAddFlowConditions());
            this.Commands.AddItem("RemoveConditions", t => this.RemoveFlowConditions(), t => this.CanRemoveFlowConditions());
            this.EnableCutCopy();

            if (this.Model.RuleGroup != null)
            {
                this.RuleGroup = new RuleGroupViewModel(this.Model.RuleGroup);
            }

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return this.Alias;
            }
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                if (this.RuleGroup != null)
                {
                    yield return this.RuleGroup;
                }
            }
        }

        public string Alias
        {
            get
            {
                return Model.Alias;
            }
            set
            {
                Model.Alias = value;
            }
        }

        public string FlowRuleDefinition
        {
            get
            {
                return Model.FlowRuleDefinition;
            }
            set
            {
                Model.FlowRuleDefinition = value;
            }
        }

        private void DeleteAlias()
        {
            this.ParentCollection.Remove(this.Model);
        }

        public RuleGroupViewModel RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }
            set
            {
                if (this.ruleGroup != null)
                {
                    this.UnregisterChildViewModel(this.ruleGroup);
                }

                this.ruleGroup = value;

                if (this.ruleGroup != null)
                {
                    this.RegisterChildViewModel(this.ruleGroup);
                }
            }
        }


        public void AddFlowConditions()
        {
            if (this.RuleGroup == null)
            {
                this.Model.RuleGroup = new RuleGroup();
                RuleGroupViewModel vm = new RuleGroupViewModel(this.Model.RuleGroup, "Flow conditions");
                this.RuleGroup = vm;
                this.IsExpanded = true;
                this.RuleGroup.IsExpanded = true;
                this.RuleGroup.IsSelected = true;
            }
        }

        public void RemoveFlowConditions()
        {
            this.RuleGroup = null;
            this.Model.RuleGroup = null;
        }

        protected override bool CanMoveDown()
        {
            return true;
        }

        protected override bool CanMoveUp()
        {
            return true;
        }

        private bool CanRemoveFlowConditions()
        {
            return this.Model.RuleGroup != null;
        }

        private bool CanAddFlowConditions()
        {
            return this.RuleGroup == null;
        }
    }
}
