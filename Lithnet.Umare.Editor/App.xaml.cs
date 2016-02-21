using System;
using System.Windows;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace Lithnet.Umare.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();
        }
    }
}