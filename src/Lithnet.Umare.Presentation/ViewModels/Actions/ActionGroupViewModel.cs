using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public abstract class ActionGroupViewModel : ViewModelBase<ActionGroup>
    {
        private RuleGroupViewModel ruleGroup;

        public ActionGroupViewModel(ActionGroup model)
            : base(model)
        {
            if (this.Model.RuleGroup != null)
            {
                this.RuleGroup = new RuleGroupViewModel(this.Model.RuleGroup, "Execution rules");
            }

            this.EnableCutCopy();
            this.Commands.AddItem("AddExecutionConditions", t => this.AddExecutionConditions(), t => this.CanAddExecutionConditions());
            this.Commands.AddItem("RemoveExecutionConditions", t => this.RemoveExecutionConditions(), t => this.CanRemoveExecutionConditions());

            this.Actions = new ActionsViewModel(model.Actions);
            this.Actions.Parent = this;

            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.EnableCutCopy();
            this.Commands.AddItem("Delete", t => this.Delete());
        }

        public string DisplayName
        {
            get
            {
                return this.Name;
            }
        }

        [PropertyChanged.AlsoNotifyFor("DisplayName")]
        public string Name
        {
            get
            {
                return this.Model.Name;
            }
            set
            {
                this.Model.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return this.Model.Description;
            }
            set
            {
                this.Model.Description = value;
            }
        }

        public ActionsViewModel Actions { get; protected set; }

        //public override IEnumerable<ViewModelBase> ChildNodes
        //{
        //    get
        //    {
        //        if (this.RuleGroup != null)
        //        {
        //            yield return this.RuleGroup;
        //        }

        //        foreach (ViewModelBase vm in this.Actions)
        //        {
        //            yield return vm;
        //        }
        //    }
        //}

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

        public void AddExecutionConditions()
        {
            if (this.RuleGroup == null)
            {
                this.Model.RuleGroup = new RuleGroup();
                RuleGroupViewModel vm = new RuleGroupViewModel(this.Model.RuleGroup, "Execution rules");
                this.RuleGroup = vm;
                //this.RaisePropertyChanged("ChildNodes");
                this.IsExpanded = true;
                this.RuleGroup.IsExpanded = true;
                this.RuleGroup.IsSelected = true;
            }
        }

        public void RemoveExecutionConditions()
        {
            this.RuleGroup = null;
            this.Model.RuleGroup = null;
            //this.RaisePropertyChanged("ChildNodes");
        }

        protected override bool CanMoveDown()
        {
            return true;
        }

        protected override bool CanMoveUp()
        {
            return true;
        }

        private bool CanRemoveExecutionConditions()
        {
            return this.Model.RuleGroup != null;
        }

        private bool CanAddExecutionConditions()
        {
            return this.RuleGroup == null;
        }

        private void Delete()
        {
            this.ParentCollection.Remove(this.Model);
        }
    }
}