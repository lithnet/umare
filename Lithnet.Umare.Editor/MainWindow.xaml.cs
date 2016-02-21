using System.Windows;
using Lithnet.Transforms;
using Lithnet.Umare.Presentation;
using Lithnet.Umare;
using System.Windows.Controls;

namespace Lithnet.Umare.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
            TransformGlobal.HostProcessSupportsNativeDateTime = false;
            this.DataContext = new MainWindowViewModel();
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null && !tvi.IsFocused)
            {
                tvi.Focus();
                tvi.BringIntoView();
            }
        }
    }
}
