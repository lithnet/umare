using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections;

namespace Lithnet.Umare.Presentation
{
    public class ExportActionGroupViewModel : ActionGroupViewModel
    {
        public ExportActionGroupViewModel(ActionGroup model)
            : base(model)
        {
            this.PasteableTypes.Add(typeof(ExportMappingAction));
            this.PasteableTypes.Add(typeof(DeclineMappingAction));
            this.PasteableTypes.Add(typeof(ExtensionPassThroughAction));
            this.Commands.AddItem("AddExportMappingAction", t => this.Actions.AddExportMappingAction());
            this.Commands.AddItem("AddDeclineMappingAction", t => this.Actions.AddDeclineMappingAction());
            this.Commands.AddItem("AddPassThroughAction", t => this.Actions.AddPassThroughAction());
        }
    }
}