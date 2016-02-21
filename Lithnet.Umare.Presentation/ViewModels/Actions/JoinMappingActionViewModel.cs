using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public class JoinMappingActionViewModel : MappingActionViewModel
    {
        private JoinMappingAction typedModel;

        public JoinMappingActionViewModel(JoinMappingAction model)
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
                    return "Undefined join mapping";
                }
                else
                {
                    return string.Format("{1} ({0})", "join mapping", this.Name);
                }
            }
        }
    }
}
