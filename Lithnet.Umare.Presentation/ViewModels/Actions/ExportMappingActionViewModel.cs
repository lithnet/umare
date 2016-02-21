using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class ExportMappingActionViewModel : MappingActionViewModel
    {
        private ExportMappingAction typedModel;

        public ExportMappingActionViewModel(ExportMappingAction model)
            : base(model)
        {
            this.typedModel = model;
        }


        public override string DisplayName
        {
            get
            {
                if (this.Name == null)
                {
                    return "Undefined export flow";
                }
                else
                {
                    return string.Format("{1} ({0})", "export flow", this.Name);
                }
            }
        }
    }
}
