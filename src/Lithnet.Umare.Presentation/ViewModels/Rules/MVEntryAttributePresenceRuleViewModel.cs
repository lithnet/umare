using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class MVEntryAttributePresenceRuleViewModel : RuleViewModel
    {
        private MVEntryAttributePresenceRule typedModel;

        public MVEntryAttributePresenceRuleViewModel(MVEntryAttributePresenceRule model)
            : base(model)
        {
            this.typedModel = model;
        }

        [PropertyChanged.DependsOn("Attribute", "Operator")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        private string GetDisplayName()
        {
            if (this.Attribute == null)
            {
                return string.Format("Undefined MV attribute presence rule");
            }
            else
            {
                string attributeName = this.Attribute;

                if (this.Operator == PresenceOperator.IsPresent)
                {
                    return string.Format("If MV attribute {{{0}}} is present", attributeName);
                }
                else
                {
                    return string.Format("If MV attribute {{{0}}} is not present", attributeName);
                }
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public PresenceOperator Operator
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

        public string Attribute
        {
            get
            {
                return this.typedModel.Attribute;
            }
            set
            {
                this.typedModel.Attribute = value;
            }
        }
    }
}