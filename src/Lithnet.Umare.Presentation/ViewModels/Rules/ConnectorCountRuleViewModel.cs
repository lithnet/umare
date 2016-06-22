using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Umare.Presentation
{
    public class ConnectorCountRuleViewModel : RuleViewModel
    {
        private ConnectorCountRule typedModel;

        public ConnectorCountRuleViewModel(ConnectorCountRule model)
            : base(model)
        {
            this.typedModel = model;
        }

        [PropertyChanged.DependsOn("MAName", "Operator", "Count")]
        public override string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        private string GetDisplayName()
        {
            if (this.MAName == null)
            {
                return string.Format("Undefined connector count rule");
            }
            else
            {
                if (this.Operator == ValueOperator.Equals)
                {
                    if (this.Count == 0)
                    {
                        return string.Format("Not connected to {0}", this.MAName);
                    }
                    else if (this.Count == 1)
                    {
                        return string.Format("Connected to {0}", this.MAName);
                    }
                    else
                    {
                        return string.Format("{1} connections to {0}", this.MAName, this.Count);
                    }
                }
                else if (this.Operator == ValueOperator.NotEquals)
                {
                    if (this.Count == 0)
                    {
                        return string.Format("Connected to {0}", this.MAName);
                    }
                    else if (this.Count == 1)
                    {
                        return string.Format("Not connected to {0}", this.MAName);
                    }
                    else
                    {
                        return string.Format("Does not have {1} connections to {0}", this.MAName, this.Count);

                    }
                }
                else if (this.Operator == ValueOperator.GreaterThan)
                {
                    return string.Format("More than {1} connections to {0}", this.MAName, this.Count);
                }
                else if (this.Operator == ValueOperator.GreaterThanOrEq)
                {
                    return string.Format("At least {1} connections to {0}", this.MAName, this.Count);
                }
                else if (this.Operator == ValueOperator.LessThan)
                {
                    return string.Format("Less than {1} connections to {0}", this.MAName, this.Count);
                }
                else if (this.Operator == ValueOperator.LessThanOrEq)
                {
                    return string.Format("{1} or fewer connections to {0}", this.MAName, this.Count);
                }
                else
                {
                    return string.Format("Undefined connector count rule");
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

        public string MAName
        {
            get
            {
                return this.typedModel.MAName;
            }
            set
            {
                this.typedModel.MAName = value;
            }
        }

        public ValueOperator Operator
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

        public IEnumerable<EnumExtension.EnumMember> AllowedOperators
        {
            get
            {
                return this.GetAllowedValueOperators();
            }
        }

        private IEnumerable<EnumExtension.EnumMember> GetAllowedValueOperators()
        {
            var enumValues = Enum.GetValues(typeof(ValueOperator));

            foreach (var value in enumValues)
            {
                if (ComparisonEngine.AllowedIntegerOperators.Any(t => (int)t == (int)value))
                {
                    EnumExtension.EnumMember enumMember = new EnumExtension.EnumMember();
                    enumMember.Value = value;
                    enumMember.Description = ((Enum)value).GetEnumDescription();
                    yield return enumMember;
                }
            }
        }

        public int Count
        {
            get
            {
                return this.typedModel.Count;
            }
            set
            {
                this.typedModel.Count = value;
            }
        }
    }
}