using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class ImportMappingActionViewModel : MappingActionViewModel
    {
        private ImportMappingAction typedModel;

        public ImportMappingActionViewModel(ImportMappingAction model)
            : base(model)
        {
            this.typedModel = model;
        }

        public bool MergeValues
        {
            get
            {
                return this.typedModel.MergeValues;
            }
            set
            {
                this.typedModel.MergeValues = value;
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.Name == null)
                {
                    return "Undefined import flow";
                }
                else
                {
                    return string.Format("{1} ({0})", "import flow", this.Name);
                }
            }
        }
    }
}
