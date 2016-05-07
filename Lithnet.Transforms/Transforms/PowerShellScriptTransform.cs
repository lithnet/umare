namespace Lithnet.Transforms
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.MetadirectoryServices;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    /// <summary>
    /// Executes a PowerShell script
    /// </summary>
    [DataContract(Name = "ps-script", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("PowerShell script")]
    [HandlesOwnMultivaluedInput]
    public class PowerShellScriptTransform : Transform
    {
        private string scriptText;

        private PowerShell ps;

        /// <summary>
        /// Initializes a new instance of the PowerShellScriptTransform class
        /// </summary>
        public PowerShellScriptTransform()
        {
        }

        private PowerShell PS
        {
            get
            {
                if (this.ps == null)
                {
                    this.ps = PowerShell.Create();
                    this.ps.AddScript(this.ScriptText);
                    this.ps.Invoke();
                }

                return this.ps;
            }
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.Reference;

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.Reference;

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the script
        /// </summary>
        [DataMember(Name = "script-path")]
        public string ScriptPath { get; set; }

        private string ScriptText
        {
            get
            {
                if (this.scriptText == null)
                {
                    this.scriptText = System.IO.File.ReadAllText(this.ScriptPath);
                }

                return this.scriptText;
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.Execute(new List<object>() { inputValue });
        }

        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            return this.Execute(inputValues);
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "ScriptPath":
                    if (string.IsNullOrWhiteSpace(this.ScriptPath))
                    {
                        this.AddError("ScriptPath", "A value must be specified");
                    }
                    else
                    {
                        this.RemoveError("ScriptPath");
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Performs the regular expression match and replacement
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private IList<object> Execute(IList<object> inputValues)
        {
            List<object> returnItems = new List<object>();

            this.PS.Commands.Clear();
            this.PS.Commands.AddCommand("Transform-Values");
            this.PS.AddParameter("items", inputValues.ToArray());

            foreach (var item in this.PS.Invoke())
            {
                if (item == null)
                {
                    continue;
                }

                returnItems.Add(item.BaseObject);
            }

            return returnItems;
        }
    }
}