using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Lithnet.Transforms.Presentation
{
    public class PowerShellScriptTransformViewModel : TransformViewModel
    {
        private PowerShellScriptTransform model;

        public PowerShellScriptTransformViewModel(PowerShellScriptTransform model)
            : base(model)
        {
            this.model = model;
            this.Commands.AddItem("SelectFile", x => this.SelectFile());
        }

        private void SelectFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".ps1";
            dialog.Filter = "PS1 files|*.ps1|All files|*.*";
            dialog.CheckFileExists = true;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.ScriptPath = dialog.FileName;
            }
        }

        public string ScriptPath
        {
            get
            {
                return this.model.ScriptPath;
            }
            set
            {
                this.model.ScriptPath = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.PowerShellScriptPathTransformDescription;
            }
        }
    }
}
