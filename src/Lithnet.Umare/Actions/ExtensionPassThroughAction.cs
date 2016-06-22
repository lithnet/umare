using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lithnet.Umare
{
    [DataContract(Name = "passthrough-action", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class ExtensionPassThroughAction : Action,
        IImportMappingAction,
        IExportMappingAction,
        IDeprovisionAction,
        IDisconnectAction,
        IJoinMappingAction,
        IProjectAction,
        IResolveJoinSearchAction
    {
        private bool hasInitialized;

        private Assembly extensionAssembly;

        private IMASynchronization extensionInterface;

        [DataMember(Name = "file-name")]
        public string ExtensionFileName { get; set; }

        internal static Dictionary<string, IMASynchronization> InitializedExtensions { get; set; }

        static ExtensionPassThroughAction()
        {
            ExtensionPassThroughAction.InitializedExtensions = new Dictionary<string, IMASynchronization>();
        }

        public static void TerminateInitializedExtensions()
        {
            foreach (IMASynchronization item in ExtensionPassThroughAction.InitializedExtensions.Values)
            {
                try
                {
                    item.Terminate();
                }
                catch (Exception)
                {
                    //TODO: Log
                }
            }
        }

        public void MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            this.Initialize();
            this.extensionInterface.MapAttributesForExport(FlowRuleName, mventry, csentry);
        }

        public void MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            this.Initialize();
            this.extensionInterface.MapAttributesForImport(FlowRuleName, csentry, mventry);
        }

        public DeprovisionAction Deprovision(CSEntry csentry)
        {
            this.Initialize();
            return this.extensionInterface.Deprovision(csentry);
        }

        public bool FilterForDisconnection(CSEntry csentry)
        {
            this.Initialize();
            return this.extensionInterface.FilterForDisconnection(csentry);
        }

        public void MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            this.Initialize();
            this.extensionInterface.MapAttributesForJoin(FlowRuleName, csentry, ref values);
        }

        public bool ShouldProjectToMV(CSEntry csentry, out string MVObjectType)
        {
            this.Initialize();
            return this.extensionInterface.ShouldProjectToMV(csentry, out MVObjectType);
        }

        public bool ResolveJoinSearch(string joinCriteriaName, CSEntry csentry, MVEntry[] rgmventry, out int imventry, ref string MVObjectType)
        {
            this.Initialize();
            return this.extensionInterface.ResolveJoinSearch(joinCriteriaName, csentry, rgmventry, out imventry, ref MVObjectType);
        }

        private void Initialize()
        {
            if (!hasInitialized)
            {
                if (ExtensionPassThroughAction.InitializedExtensions.ContainsKey(this.ExtensionFileName))
                {
                    this.extensionInterface = ExtensionPassThroughAction.InitializedExtensions[this.ExtensionFileName];
                }
                else
                {
                    this.LoadExtension();
                    this.extensionInterface.Initialize();
                    ExtensionPassThroughAction.InitializedExtensions.Add(this.ExtensionFileName, this.extensionInterface);
                }

                hasInitialized = true;
            }
        }

        internal void LoadExtension()
        {
            this.extensionAssembly = Assembly.LoadFrom(this.ExtensionFileName);
            Type intType = typeof(IMASynchronization);

            foreach (Type t in this.extensionAssembly.GetExportedTypes())
            {
                if (!t.IsClass || t.IsNotPublic)
                {
                    continue;
                }
                if (intType.IsAssignableFrom(t))
                {
                    this.extensionInterface = (IMASynchronization)Activator.CreateInstance(t);
                    return;
                }
            }

            throw new EntryPointNotFoundException("The extension {0} did not contain a type with the IMASynchronization interface");
        }
    }
}
