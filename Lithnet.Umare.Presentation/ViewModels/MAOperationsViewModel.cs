using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class MAOperationsViewModel : ListViewModel<MAOperationViewModel, MAOperation>
    {
        private MAOperations typedModel;

        public MAOperationsViewModel(MAOperations model)
            : base()
        {
            this.typedModel = model;
            this.SetCollectionViewModel((IList)model, t => this.ViewModelResolver(t));
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.PasteableTypes.Add(typeof(MAOperation));
            this.Commands.AddItem("AddManagementAgent", t => this.AddManagementAgent());
        }

        public string DisplayName
        {
            get
            {
                return "Management Agent Operations";
            }
        }

        private void AddManagementAgent()
        {
            this.Add(new MAOperation() { MAName = "New management agent" }, true);
        }

        private MAOperationViewModel ViewModelResolver(MAOperation t)
        {
            return new MAOperationViewModel(t);
        }
    }
}
