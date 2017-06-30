using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Lithnet.Common.Presentation;
using Microsoft.Win32;
using System.ComponentModel;
using System.Linq;
using Lithnet.Transforms;
using Lithnet.Umare;
using Lithnet.Transforms.Presentation;
using System.Collections.ObjectModel;
using System.Collections;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Umare.Presentation
{
    public class XmlConfigFileViewModel : ViewModelBase<XmlConfigFile>
    {
        private TransformCollectionViewModel transformCollectionViewModel;

        private FlowRuleAliasCollectionViewModel flowRulesAliasCollectionViewModel;

        private MAOperationsViewModel maOperationsViewModel;

        public XmlConfigFileViewModel()
            : base()
        {
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            this.Model = new XmlConfigFile();

            this.TransformCollectionViewModel = new TransformCollectionViewModel(this.Model.Transforms);
            this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);
            this.MAOperationsViewModel = new Presentation.MAOperationsViewModel(this.Model.MAOperations);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            this.ResetChangeState();
        }

        public TransformCollectionViewModel TransformCollectionViewModel
        {
            get
            {
                return this.transformCollectionViewModel;
            }
            set
            {
                if (this.transformCollectionViewModel != null)
                {
                    this.UnregisterChildViewModel(this.transformCollectionViewModel);
                }

                this.transformCollectionViewModel = value;
                this.RegisterChildViewModel(this.transformCollectionViewModel);
            }
        }


        public MAOperationsViewModel MAOperationsViewModel
        {
            get
            {
                return this.maOperationsViewModel;
            }
            set
            {
                if (this.maOperationsViewModel != null)
                {
                    this.UnregisterChildViewModel(this.maOperationsViewModel);
                }

                this.maOperationsViewModel = value;
                this.RegisterChildViewModel(this.maOperationsViewModel);
            }
        }

        public FlowRuleAliasCollectionViewModel FlowRulesAliasCollectionViewModel
        {
            get
            {
                return this.flowRulesAliasCollectionViewModel;
            }
            set
            {
                if (this.flowRulesAliasCollectionViewModel != null)
                {
                    this.UnregisterChildViewModel(this.flowRulesAliasCollectionViewModel);
                }

                this.flowRulesAliasCollectionViewModel = value;
                this.RegisterChildViewModel(this.flowRulesAliasCollectionViewModel);
            }
        }

        public string DisplayName
        {
            get
            {
                return "Configuration";
            }
        }

        public string FileName
        {
            get
            {
                return this.Model.FileName;
            }
        }

        public string Description
        {
            get
            {
                return this.Model.Description;
            }
            set
            {
                this.Model.Description = value;
            }
        }

        [PropertyChanged.DependsOn("MAOperationsViewModel", "TransformCollectionViewModel", "FlowRuleAliasCollectionViewModel")]
        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.TransformCollectionViewModel;
                yield return this.FlowRulesAliasCollectionViewModel;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    yield return this.MAOperationsViewModel;
                }
            }
        }

        public void Open(string fileName)
        {
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            this.Model = ConfigManager.LoadXml(fileName);
            this.TransformCollectionViewModel = new Transforms.Presentation.TransformCollectionViewModel(this.Model.Transforms);
            this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);
            this.MAOperationsViewModel = new Presentation.MAOperationsViewModel(this.Model.MAOperations);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            this.RaisePropertyChanged("DisplayName");
            this.RaisePropertyChanged("ChildNodes");
            this.IsExpanded = true;
            this.ResetChangeState();
        }

        public void Save(string filename)
        {
            ConfigManager.Save(filename, this.Model);
            this.ResetChangeState();
        }

        public void New()
        {
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            this.Model = new XmlConfigFile();
            this.TransformCollectionViewModel = new Transforms.Presentation.TransformCollectionViewModel(this.Model.Transforms);
            this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);
            this.MAOperationsViewModel = new Presentation.MAOperationsViewModel(this.Model.MAOperations);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            this.RaisePropertyChanged("DisplayName");
            this.RaisePropertyChanged("ChildNodes");
            this.IsExpanded = true;
            this.ResetChangeState();
        }
    }
}