using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class ExtensionPassThroughActionViewModel : ActionViewModel
    {
        private ExtensionPassThroughAction typedModel;

        public ExtensionPassThroughActionViewModel(ExtensionPassThroughAction model)
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
                    return "Pass-through";
                }
                else
                {
                    return string.Format("{1} ({0})", "Pass-through", this.Name);
                }
            }
        }

        public string ExtensionFileName
        {
            get
            {
                return this.typedModel.ExtensionFileName;
            }
            set
            {
                this.typedModel.ExtensionFileName = value;
            }
        }
    }
}
