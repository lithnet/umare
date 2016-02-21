using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Umare.Presentation
{
    public abstract class RuleViewModel : ViewModelBase<Rule>
    {
        public RuleViewModel(Rule model)
            : base(model)
        {
        }

        public abstract string DisplayName { get; }

        public abstract string DisplayNameLong { get; }

        protected override bool CanMoveDown()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override bool CanMoveUp()
        {
            if (this.ParentCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
