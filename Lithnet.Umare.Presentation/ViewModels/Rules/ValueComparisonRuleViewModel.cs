using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class ValueComparisonRuleViewModel : RuleViewModel
    {
        private ValueComparisonRule typedModel;

        public ValueComparisonRuleViewModel(ValueComparisonRule model)
            : base(model)
        {
            this.typedModel = model;
        }

        [PropertyChanged.DependsOn("LeftTarget", "LeftValue", "RightTarget", "RightValue", "ValueOperator")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        private string GetDisplayName()
        {
            string leftName = null;

            switch (this.LeftTarget)
            {
                case ComparisonTarget.MetaverseObject:
                    leftName = "mv:" + this.LeftValue;
                    break;

                case ComparisonTarget.ConnectorSpaceObject:
                    leftName = "cs:" + this.LeftValue;
                    break;

                case ComparisonTarget.Constant:
                    leftName = "constant:" + this.LeftValue;
                    break;

                default:
                    break;
            }

            string rightName = null;

            switch (this.RightTarget)
            {
                case ComparisonTarget.MetaverseObject:
                    rightName = "mv:" + this.RightValue;
                    break;

                case ComparisonTarget.ConnectorSpaceObject:
                    rightName = "cs:" + this.RightValue;
                    break;

                case ComparisonTarget.Constant:
                    rightName = "constant:" + this.RightValue;
                    break;

                default:
                    break;
            }

            if (leftName == null || rightName == null)
            {
                return string.Format("Undefined value comparison rule");
            }
            else
            {
                    return string.Format("'{0}' {1} '{2}'", leftName, this.ValueOperator, rightName);
            }
        }

        public override string DisplayNameLong
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public ValueOperator ValueOperator
        {
            get
            {
                return this.typedModel.ValueOperator;
            }
            set
            {
                this.typedModel.ValueOperator = value;
            }
        }

        public GroupOperator GroupOperator
        {
            get
            {
                return this.typedModel.GroupOperator;
            }
            set
            {
                this.typedModel.GroupOperator = value;
            }
        }

        public string LeftValue
        {
            get
            {
                return this.typedModel.LeftValue;
            }
            set
            {
                this.typedModel.LeftValue = value;
            }
        }

        public string RightValue
        {
            get
            {
                return this.typedModel.RightValue;
            }
            set
            {
                this.typedModel.RightValue = value;
            }
        }

        public ComparisonTarget LeftTarget
        {
            get
            {
                return this.typedModel.LeftTarget;
            }
            set
            {
                this.typedModel.LeftTarget = value;
            }
        }

        public ComparisonTarget RightTarget
        {
            get
            {
                return this.typedModel.RightTarget;
            }
            set
            {
                this.typedModel.RightTarget = value;
            }
        }

        public string LeftTransformString
        {
            get
            {
                return this.typedModel.LeftTransformString;
            }
            set
            {
                this.typedModel.LeftTransformString = value;
            }
        }

        public ExtendedAttributeType CompareAs
        {
            get
            {
                return this.typedModel.CompareAs;
            }
            set
            {
                this.typedModel.CompareAs = value;
            }
        }

        public string RightTransformString
        {
            get
            {
                
                return this.typedModel.RightTransformString;
            }
            set
            {
                this.typedModel.RightTransformString = value;
            }
        }
    }
}