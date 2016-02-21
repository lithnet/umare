using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Transforms
{
    [DataContract(Name = "direction", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public enum Direction
    {
        [EnumMember(Value = "left")]
        Left,

        [EnumMember(Value = "right")]
        Right
    }
}
