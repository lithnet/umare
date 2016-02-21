using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Umare
{
    public interface IExportMappingAction
    {
        void MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry);
    }
}
