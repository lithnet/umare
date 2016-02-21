using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Transforms
{
    [DataContract(Name = "triggers", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public enum BitwiseOperation
    {
        [EnumMember(Value="and")]
        And,

        [EnumMember(Value = "or")]
        Or,

        [EnumMember(Value = "nand")]
        Nand,

        [EnumMember(Value = "xor")]
        Xor
    }
}
