using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Transforms.Presentation
{
    public class ConditionalStringFlowTransformViewModel : TransformViewModel
    {
        private ConditionalStringFlowTransform model;

        public ConditionalStringFlowTransformViewModel(ConditionalStringFlowTransform model)
            : base(model)
        {
            this.model = model;
        }

        public StringComparison ComparisonType
        {
            get
            {
                return model.ComparisonType;
            }
            set
            {
                model.ComparisonType = value;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.ConditionalStringFlowTransform;
            }
        }
    }
}
