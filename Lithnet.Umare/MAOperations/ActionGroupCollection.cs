using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Umare
{
    [CollectionDataContract(Name = "action-group-collection", ItemName = "action-group", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class ActionGroupCollection<T> : ObservableKeyedCollection<string, ActionGroup>
    {
        protected override string GetKeyForItem(ActionGroup item)
        {
            return item.Name;
        }
    }
}
