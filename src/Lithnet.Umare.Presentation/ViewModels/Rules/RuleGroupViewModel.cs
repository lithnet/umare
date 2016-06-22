using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Windows;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class RuleGroupViewModel : RuleViewModel
    {
        private RuleGroup typedModel;

        private ListViewModel<RuleViewModel, Rule> rules;

        private string displayName;

        public RuleGroupViewModel(RuleGroup model)
            : base(model)
        {
            this.typedModel = model;

            this.Rules = new ListViewModel<RuleViewModel, Rule>((IList)model.Items, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");

            this.Commands.AddItem("DeleteRuleGroup", t => this.DeleteRuleGroup());
            this.Commands.AddItem("AddRuleGroup", t => this.AddRuleGroup());
            this.Commands.AddItem("AddConnectorCountRule", t => this.AddConnectorCountRule());
            this.Commands.AddItem("AddCSEntryAttributePresenceRule", t => this.AddCSEntryAttributePresenceRule());
            this.Commands.AddItem("AddMVEntryAttributePresenceRule", t => this.AddMVEntryAttributePresenceRule());
            this.Commands.AddItem("AddValueComparisonRule", t => this.AddValueComparisonRule());
            this.Commands.AddItem("Paste", t => this.Rules.Paste(), t => this.Rules.CanPaste());

            this.Rules.PasteableTypes.Add(typeof(RuleGroup));
            this.Rules.PasteableTypes.Add(typeof(ValueComparisonRule));
            this.Rules.PasteableTypes.Add(typeof(CSEntryAttributePresenceRule));
            this.Rules.PasteableTypes.Add(typeof(MVEntryAttributePresenceRule));
            this.Rules.PasteableTypes.Add(typeof(ConnectorCountRule));

            this.EnableCutCopy();
        }

        public RuleGroupViewModel(RuleGroup model, string displayName)
            : this(model)
        {
            this.displayName = displayName;
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                return this.Rules;
            }
        }

        public ListViewModel<RuleViewModel, Rule> Rules
        {
            get
            {
                return this.rules;
            }
            set
            {
                if (this.rules != null)
                {
                    this.UnregisterChildViewModel(this.rules);
                }

                this.rules = value;

                if (this.rules != null)
                {
                    this.RegisterChildViewModel(this.rules);
                }
            }
        }

        private string GetDisplayName()
        {
            if (string.IsNullOrWhiteSpace(this.displayName))
            {
                return string.Format("Rule group ({0})", this.Operator.ToSmartString().ToLower());
            }
            else
            {
                return string.Format("{0} ({1})", this.displayName, this.Operator.ToSmartString().ToLower());
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        [PropertyChanged.DependsOn("Operator")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public GroupOperator Operator
        {
            get
            {
                return this.typedModel.Operator;
            }
            set
            {
                this.typedModel.Operator = value;
            }
        }

        private void DeleteRuleGroup()
        {
            try
            {
                if (this.Rules.Count > 0)
                {
                    if (MessageBox.Show("Are you are you want to delete this group?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                if (this.ParentCollection != null)
                {
                    this.ParentCollection.Remove(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the group\n\n" + ex.Message);
            }
        }

        private RuleViewModel ViewModelResolver(Rule model)
        {
            if (model is RuleGroup)
            {
                return new RuleGroupViewModel(model as RuleGroup);
            }
            else if (model is ConnectorCountRule)
            {
                return new ConnectorCountRuleViewModel(model as ConnectorCountRule);
            }
            else if (model is ValueComparisonRule)
            {
                return new ValueComparisonRuleViewModel(model as ValueComparisonRule);
            }
            else if (model is CSEntryAttributePresenceRule)
            {
                return new CSEntryAttributePresenceRuleViewModel(model as CSEntryAttributePresenceRule);
            }
            else if (model is MVEntryAttributePresenceRule)
            {
                return new MVEntryAttributePresenceRuleViewModel(model as MVEntryAttributePresenceRule);
            }
            else
            {
                throw new ArgumentException("The object type is unknown", "model");
            }
        }

        public void AddRuleGroup()
        {
            this.IsExpanded = true;
            this.Rules.Add(new RuleGroup(), true);
        }

        public void AddConnectorCountRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new ConnectorCountRule(), true);
        }

        public void AddCSEntryAttributePresenceRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new CSEntryAttributePresenceRule(), true);
        }

        public void AddMVEntryAttributePresenceRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new MVEntryAttributePresenceRule(), true);
        }

        public void AddValueComparisonRule()
        {
            this.IsExpanded = true;
            this.Rules.Add(new ValueComparisonRule(), true);
        }
    }
}