using System.Windows.Navigation;

namespace Lithnet.Transforms.Presentation
{
    public partial class FormatNumberTransformView
    {
        private void HyperLink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            HyperLinkLauncher.RequestNavigate(e);
        }
    }
}
