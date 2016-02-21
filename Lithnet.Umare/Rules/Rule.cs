using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Umare
{
    [DataContract(Name = "rule-base", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    [KnownType(typeof(CSEntryAttributePresenceRule))]
    [KnownType(typeof(MVEntryAttributePresenceRule))]
    [KnownType(typeof(ConnectorCountRule))]
    [KnownType(typeof(ValueComparisonRule))]
    [KnownType(typeof(RuleGroup))]
    public abstract class Rule : UINotifyPropertyChanges
    {
        public abstract bool Evaluate(CSEntry csentry, MVEntry mventry);

        public delegate void RuleFailedEventHandler(Rule sender, string failureReason);

        public static event RuleFailedEventHandler RuleFailedEvent;

        /// <summary>
        /// Initializes a new instance of the RuleBase class
        /// </summary>
        protected Rule()
        {
        }

        protected void RaiseRuleFailure(string message, params object[] args)
        {
            if (Rule.RuleFailedEvent != null)
            {
                Rule.RuleFailedEvent(this, string.Format(message, args));
            }
        }
    }
}
