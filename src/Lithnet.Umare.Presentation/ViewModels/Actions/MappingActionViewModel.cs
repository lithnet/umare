using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Umare.Presentation
{
    public class MappingActionViewModel : ActionViewModel
    {
        private MappingAction typedModel;

        public MappingActionViewModel(MappingAction model)
            : base(model)
        {
            this.typedModel = model;
        }

        public List<string> SourceAttributes
        {
            get
            {
                return this.typedModel.SourceAttributes;
            }
            set
            {
                this.typedModel.SourceAttributes = value;
            }
        }

        public string TargetAttribute
        {
            get
            {
                return this.typedModel.TargetAttribute;
            }
            set
            {
                this.typedModel.TargetAttribute = value;

            }
        }

        public string TransformString
        {
            get
            {
                return this.typedModel.TransformString;
            }
            set
            {
                this.typedModel.TransformString = value;
            }
        }
    }
}
