using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Umare
{
    public interface IJoinMappingAction
    {
        void MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values);
    }
}
