using System.Windows.Navigation;

namespace Lithnet.Transforms.Presentation
{
    public partial class FormatStringTransformView
    {
        private void HyperLink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            HyperLinkLauncher.RequestNavigate(e);
        }
    }
}
