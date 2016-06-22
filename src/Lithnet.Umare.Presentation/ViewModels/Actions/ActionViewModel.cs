using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class ActionViewModel : ViewModelBase<Action>
    {
        private RuleGroupViewModel ruleGroup;

        public ActionViewModel(Action model)
            : base(model)
        {
            if (this.Model.RuleGroup != null)
            {
                this.RuleGroup = new RuleGroupViewModel(this.Model.RuleGroup, "Execution rules");
            }

            this.EnableCutCopy();
            this.Commands.AddItem("AddExecutionConditions", t => this.AddExecutionConditions(), t => this.CanAddExecutionConditions());
            this.Commands.AddItem("RemoveExecutionConditions", t => this.RemoveExecutionConditions(), t => this.CanRemoveExecutionConditions());
            this.Commands.AddItem("DeleteAction", t => this.DeleteAction());
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public virtual string DisplayName
        {
            get
            {
                return this.Name;
            }
        }

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

        //public override IEnumerable<ViewModelBase> ChildNodes
        //{
        //    get
        //    {
        //        if (this.RuleGroup != null)
        //        {
        //            yield return this.RuleGroup;
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
            return this.Model.RuleGroup == null;
        }

        private void DeleteAction()
        {
            this.ParentCollection.Remove(this.Model);
        }
    }
}