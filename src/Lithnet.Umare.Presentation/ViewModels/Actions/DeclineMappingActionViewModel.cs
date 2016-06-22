using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class DeclineMappingActionViewModel : ActionViewModel
    {
        private DeclineMappingAction typedModel;

        public DeclineMappingActionViewModel(DeclineMappingAction model)
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
                    return "Decline mapping";
                }
                else
                {
                    return string.Format("{1} ({0})", "Decline mapping", this.Name);
                }
            }
        }
    }
}
