namespace Lithnet.Transforms
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.MetadirectoryServices;
    using System.CodeDom;
    using System.Reflection;
    using System.CodeDom.Compiler;
    using Microsoft.CSharp;

    /// <summary>
    /// Executes a C# script
    /// </summary>
    [DataContract(Name = "cs-script", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("C# script")]
    [HandlesOwnMultivaluedInput]
    public class CSharpScriptTransform : Transform
    {
        private MethodInfo method;

        /// <summary>
        /// Initializes a new instance of the PowerShellScriptTransform class
        /// </summary>
        public CSharpScriptTransform()
        {
        }

        private MethodInfo Method
        {
            get
            {
                if (this.method == null)
                {
                    this.method = this.Compile();
                }

                return this.method;
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

        [DataMember(Name = "script-text")]
        public string ScriptText { get; set; }

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
                case "ScriptText":
                    if (string.IsNullOrWhiteSpace(this.ScriptText))
                    {
                        this.AddError("ScriptText", "A value must be specified");
                    }
                    else
                    {
                        this.RemoveError("ScriptText");
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

            try
            {
                object retval = this.Method.Invoke(null, new object[] { inputValues });

                IList<object> list = retval as IList<object>;

                if (list != null)
                {
                    return list;
                }
                else
                {
                    return new List<object>() { retval };
                }
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetCode()
        {
            return  string.Format(@"
            using System;
            using System.Xml;
            using Lithnet.Transforms;
            using Microsoft.MetadirectoryServices;
            using System.Collections.Generic;

            public static class CSExtension
            {{
                public static object Transform(IList<object> obj)
                {{
                    {0}
                }}
            }}", 
            this.ScriptText);
        }

        private MethodInfo Compile()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add(typeof(Transform).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Microsoft.MetadirectoryServices.IMASynchronization).Assembly.Location);
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Collections.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.ReferencedAssemblies.Add(typeof(Lithnet.MetadirectoryServices.ComparisonEngine).Assembly.Location);

            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = true;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = false;
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, this.ScriptText);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}, Line {1}, Col {2}): {3}", error.ErrorNumber, error.Line, error.Column, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            Assembly assembly = results.CompiledAssembly;
            Type program = assembly.GetType("CSExtension");
            return program.GetMethod("Transform");
        }
    }
}