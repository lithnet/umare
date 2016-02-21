namespace Lithnet.Umare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    [Flags]
    [DataContract(Name = "comparison-target", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public enum ComparisonTarget
    {
        [EnumMember(Value = "mv-object")]
        MetaverseObject = 1,

        [EnumMember(Value = "cs-object")]
        ConnectorSpaceObject = 2,

        [EnumMember(Value = "constant")]
        Constant = 3
    }
}
