using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using Lithnet.Common.ObjectModel;
using Lithnet.Common.Presentation;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Controls;

namespace Lithnet.Umare.Presentation
{
    public class MainWindowViewModel : ViewModelBase
    {
        private XmlConfigFileViewModel configFile;

        private bool confirmedCloseOnDirtyViewModel;

        public MainWindowViewModel()
        {
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            ViewModelBase.GlobalIconProvider = new IconProvider();
            this.AddDependentPropertyNotification("ViewModelIsDirty", "DisplayName");

            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.IgnorePropertyHasChanged.Add("MRUItems");
            this.IgnorePropertyHasChanged.Add("ChildNodes");
            this.IgnorePropertyHasChanged.Add("Database");
            this.IgnorePropertyHasChanged.Add("XmlConfigFile");
            this.IgnorePropertyHasChanged.Add("ViewModelIsDirty");

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            string openFile = null;

            if (commandLineArgs != null)
            {
                if (commandLineArgs.Length >= 2)
                {
                    openFile = commandLineArgs[1];
                }
            }

            this.XmlConfigFile = new XmlConfigFileViewModel();

            if (openFile != null)
            {
                if (System.IO.File.Exists(openFile))
                {
                    this.XmlConfigFile.Open(openFile);
                }
            }

            this.XmlConfigFile.PropertyChanged += XmlConfigFile_PropertyChanged;

            this.Commands.AddItem("New", x => this.New());
            this.Commands.AddItem("Open", x => this.Open());
            this.Commands.AddItem("Save", x => this.Save(), x => this.CanSave());
            this.Commands.AddItem("SaveAs", x => this.SaveAs(), x => this.CanSave());
            this.Commands.AddItem("Close", x => this.Close());

            this.XmlConfigFile.IsExpanded = true;
            ViewModelBase.ViewModelChanged += ViewModelBase_ViewModelChanged;
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            UINotifyPropertyChanges.EndIgnoreAllChanges();
        }

        private bool ViewModelIsDirty { get; set; }

        private void XmlConfigFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayName")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }

        private void ViewModelBase_ViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != this)
            {
                if (!this.ViewModelIsDirty)
                {
                    if (!this.IgnorePropertyHasChanged.Contains(e.PropertyName))
                    {
                        this.ViewModelIsDirty = true;
                        this.RaisePropertyChanged("DisplayName");
                    }
                }
            }
        }

        public Cursor Cursor { get; set; }

        public XmlConfigFileViewModel XmlConfigFile
        {
            get
            {
                return this.configFile;
            }
            set
            {
                if (this.configFile != null)
                {
                    this.UnregisterChildViewModel(this.configFile);
                }

                this.configFile = value;

                if (value != null)
                {
                    this.RegisterChildViewModel(this.configFile);
                }
            }
        }

        [PropertyChanged.DependsOn("Database", "XmlConfigFile")]
        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.XmlConfigFile;
            }
        }

        public IEnumerable<MRUItem> MRUItems
        {
            get
            {
                List<string> mrulist = this.GetMRUItems();
                if (mrulist == null || mrulist.Count == 0)
                {
                    yield break;
                }
                else
                {
                    int count = 1;

                    foreach (var item in mrulist)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            yield return new MRUItem(string.Format("_{0} {1}", count, item), new DelegateCommand(t => this.Open(item, true)));
                            count++;
                        }
                    }

                }
            }
        }

        public string DisplayName
        {
            get
            {
                string filename = (this.XmlConfigFile == null || string.IsNullOrWhiteSpace(this.XmlConfigFile.FileName)) ? "(new file)" : this.XmlConfigFile.FileName;
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                return string.Format("Lithnet Universal MA Rule Extension Editor {0} - {1}{2}", version, filename, this.ViewModelIsDirty ? "*" : string.Empty);
            }
        }

        private void Open()
        {
            this.UpdateFocusedBindings();

            if (this.ViewModelIsDirty)
            {
                if (MessageBox.Show("There are unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".umarex";
            dialog.Filter = "Configuration Files (.umarex)|*.umarex|All Files|*.*";
            dialog.CheckFileExists = true;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.Open(dialog.FileName, false);
            }

            this.AddMRUItem(this.XmlConfigFile.FileName);
            this.RaisePropertyChanged("MRUItems");
            this.ViewModelIsDirty = false;
        }

        private void Open(string filename, bool askHasChanges)
        {
            this.UpdateFocusedBindings();

            if (System.IO.File.Exists(filename))
            {
                if (askHasChanges)
                {
                    if (this.ViewModelIsDirty)
                    {
                        if (MessageBox.Show("There are unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                }

                try
                {
                    this.Cursor = Cursors.Wait;
                    this.XmlConfigFile.Open(filename);
                    this.AddMRUItem(this.XmlConfigFile.FileName);
                    this.ViewModelIsDirty = false;
                }
                catch (Exception ex)
                {
                    //Logger.WriteException(ex);
                    MessageBox.Show(string.Format("Could not open the file\n\n{0}", ex.Message), "File Open", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Cursor = Cursors.Arrow;

                }
                finally
                {
                    this.Cursor = Cursors.Arrow;
                }
            }
            else
            {
                this.RemoveMRUItem(filename);
                MessageBox.Show("The specified file can not be found", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.RaisePropertyChanged("MRUItems");
        }

        private bool CanSave()
        {
            int count = 0;

            if (this.XmlConfigFile == null)
            {
                return false;
            }

            if (this.XmlConfigFile.Model.Transforms != null)
            {
                count += this.XmlConfigFile.Model.Transforms.Count;
            }

            if (this.XmlConfigFile.Model.FlowRuleAliases != null)
            {
                count += this.XmlConfigFile.Model.FlowRuleAliases.Count;
            }

            return count > 0;
        }

        private void UpdateFocusedBindings()
        {
            object focusedItem = Keyboard.FocusedElement;

            if (focusedItem == null)
            {
                return;
            }

            if (focusedItem is TextBox)
            {
                var expression = ((TextBox)focusedItem).GetBindingExpression(TextBox.TextProperty);
                if (expression != null)
                {
                    expression.UpdateSource();
                }
            }
            else if (focusedItem is ComboBox)
            {
                var expression = ((ComboBox)focusedItem).GetBindingExpression(ComboBox.TextProperty);
                if (expression != null)
                {
                    expression.UpdateSource();
                }
            }
        }

        private void Save()
        {
            this.UpdateFocusedBindings();

            if (this.HasErrors)
            {
                if (MessageBox.Show("There are one or more errors present in the configuration. Are you sure you want to save?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                if (string.IsNullOrWhiteSpace(this.XmlConfigFile.FileName))
                {
                    this.SaveAs();
                }
                else
                {
                    this.Cursor = Cursors.Wait;
                    this.XmlConfigFile.Save(this.XmlConfigFile.FileName);
                    this.AddMRUItem(this.XmlConfigFile.FileName);
                    this.RaisePropertyChanged("MRUItems");
                    this.ViewModelIsDirty = false;
                }
            }
            catch (Exception ex)
            {
                //Logger.WriteException(ex);
                MessageBox.Show(string.Format("Could not save the file\n\n{0}", ex.Message), "File Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void New()
        {
            this.UpdateFocusedBindings();

            if (this.ViewModelIsDirty)
            {
                if (MessageBox.Show("There are unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }
            try
            {
                this.Cursor = Cursors.Wait;
                this.XmlConfigFile.New();
                this.ViewModelIsDirty = false;
            }
            catch (Exception ex)
            {
                //Logger.WriteException(ex);
                MessageBox.Show(string.Format("Could not create a new file\n\n{0}", ex.Message), "New File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void SaveAs()
        {
            this.UpdateFocusedBindings();

            if (this.HasErrors)
            {
                if (MessageBox.Show("There are one or more errors present in the configuration. Are you sure you want to save?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".umarex";
                dialog.Filter = "Configuration Files (.umarex)|*.umarex|All Files|*.*";
                dialog.FileName = this.XmlConfigFile.FileName;

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    this.Cursor = Cursors.Wait;
                    this.XmlConfigFile.Save(dialog.FileName);
                    this.AddMRUItem(this.XmlConfigFile.FileName);
                    this.RaisePropertyChanged("MRUItems");
                    this.ViewModelIsDirty = false;
                }
            }
            catch (Exception ex)
            {
                //Logger.WriteException(ex);
                MessageBox.Show(string.Format("Could not save the file\n\n{0}", ex.Message), "Save File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Close()
        {
            this.UpdateFocusedBindings();

            if (this.ViewModelIsDirty)
            {
                if (MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            this.confirmedCloseOnDirtyViewModel = true;
            Application.Current.Shutdown();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.UpdateFocusedBindings();

            if (this.ViewModelIsDirty && !this.confirmedCloseOnDirtyViewModel)
            {
                if (MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void AddMRUItem(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || !System.IO.File.Exists(filename))
            {
                return;
            }

            List<string> mrulist = this.GetMRUItems();

            if (mrulist.Contains(filename))
            {
                mrulist.Remove(filename);
            }

            mrulist.Insert(0, filename);

            if (mrulist.Count >= 10)
            {
                for (int i = 10; i < mrulist.Count; i++)
                {
                    mrulist.RemoveAt(i);
                }
            }

            RegistrySettings.SetValue("MRUList", mrulist);
        }

        private void RemoveMRUItem(string filename)
        {
            List<string> mrulist = this.GetMRUItems();

            if (mrulist.Contains(filename))
            {
                mrulist.Remove(filename);
            }

            RegistrySettings.SetValue("MRUList", mrulist);
        }

        private List<string> GetMRUItems()
        {
            List<string> items = new List<string>();
            string[] list = RegistrySettings.GetValue("MRUList", null) as string[];

            if (list != null)
            {
                items.AddRange(list);
            }

            return items;
        }
    }
}