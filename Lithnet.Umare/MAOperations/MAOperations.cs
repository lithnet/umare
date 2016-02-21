using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Lithnet.Umare
{
    [CollectionDataContract(Name = "ma-operations", ItemName = "ma-operation", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class MAOperations : KeyedCollection<string, MAOperation>
    {
        protected override string GetKeyForItem(MAOperation item)
        {
            return item.MAName;
        }
    }
}
