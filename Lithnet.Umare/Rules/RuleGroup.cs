// -----------------------------------------------------------------------
// <copyright file="RuleGroup.cs" company="Ryan Newington">
// Copyright (c) 2015 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Umare
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using System.Runtime.Serialization;
    using System.Xml.Schema;
    using System.Collections.Specialized;
    using System.Collections;

    /// <summary>
    /// Represents a group of rules
    /// </summary>
    [DataContract(Name = "rule-group", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class RuleGroup : Rule
    {
        public delegate void RuleGroupFailedEventHandler(RuleGroup sender, string failureReason);

        public static event RuleGroupFailedEventHandler RuleGroupFailedEvent;

        [DataMember(Name = "rules")]
        public ObservableCollection<Rule> Items { get; set; }

        public RuleGroup()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the logical operator used to apply to this rule group
        /// </summary>
        [DataMember(Name = "operator")]
        public GroupOperator Operator { get; set; }

        /// <summary>
        /// Evaluates the rules within the rule group
        /// </summary>
        /// <param name="sourceObject">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the rule group's conditions were met</returns>
        public override bool Evaluate(CSEntry csentry, MVEntry mventry)
        {
            bool hasSuccess = false;

            foreach (Rule rule in this.Items)
            {
                bool result = rule.Evaluate(csentry, mventry);

                switch (this.Operator)
                {
                    case GroupOperator.None:
                        if (result)
                        {
                            this.RaiseRuleGroupFailure("Group operator was set to 'none', but an evaluation succeeded");
                            return false;
                        }

                        break;

                    case GroupOperator.All:
                        if (!result)
                        {
                            this.RaiseRuleGroupFailure("Group operator was set to 'all', but an evaluation failed");
                            return false;
                        }

                        break;

                    case GroupOperator.Any:
                        if (result)
                        {
                            return true;
                        }

                        break;

                    case GroupOperator.One:
                        if (result)
                        {
                            if (hasSuccess)
                            {
                                this.RaiseRuleGroupFailure("Group operator was set to 'one', but a second evaluation succeeded");
                                return false;
                            }
                        }

                        break;
                }

                if (result)
                {
                    hasSuccess = true;
                }
            }

            if (hasSuccess && ((this.Operator == GroupOperator.All) || (this.Operator == GroupOperator.One)))
            {
                return true;
            }
            else if (!hasSuccess && this.Operator == GroupOperator.None)
            {
                return true;
            }
            else
            {
                this.RaiseRuleGroupFailure("No evaluations succeeded");
                return false;
            }
        }

        protected void RaiseRuleGroupFailure(string message, params object[] args)
        {
            if (RuleGroup.RuleGroupFailedEvent != null)
            {
                RuleGroup.RuleGroupFailedEvent(this, string.Format(message, args));
            }
        }

        private void Initialize()
        {
            this.Operator = GroupOperator.Any;
            this.Items = new ObservableCollection<Rule>();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
        }
    }
}