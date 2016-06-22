using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "decline-mapping-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class DeclineMappingAction :Action, IImportMappingAction, IExportMappingAction, IJoinMappingAction
    {
        public void MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            throw new DeclineMappingException();
        }

        public void MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            throw new DeclineMappingException();
        }

        public void MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            throw new DeclineMappingException();
        }
    }
}
