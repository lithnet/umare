using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Lithnet.Transforms.Presentation
{
    public class CSharpScriptTransformViewModel : TransformViewModel
    {
        private CSharpScriptTransform model;

        public CSharpScriptTransformViewModel(CSharpScriptTransform model)
            : base(model)
        {
            this.model = model;
            if (this.model.ScriptText == null)
            {
                this.SeDefaultScriptText();
            }
        }

        private void SeDefaultScriptText()
        {
            this.ScriptText = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

public static class CSExtension
{
    public static IList<object> Transform(IList<object> obj)
    {
        // Your code here
    }
}";
        }

        public string ScriptText
        {
            get
            {
                return this.model.ScriptText;
            }
            set
            {
                this.model.ScriptText = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.CSharpScriptTransformDescription;
            }
        }
    }
}
