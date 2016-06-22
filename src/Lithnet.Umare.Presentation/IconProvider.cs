using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using Lithnet.Common.Presentation;

namespace Lithnet.Umare.Presentation
{
    public class IconProvider : IIconProvider
    {
        public BitmapSource GetImageForItem(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Type itemType = item.GetType();

            if (itemType == typeof(FlowRuleAliasViewModel))
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Umare.Presentation;component/Resources/alias.png", UriKind.Absolute));
            }
            else if (itemType == typeof(FlowRuleAliasCollectionViewModel))
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Umare.Presentation;component/Resources/alias.png", UriKind.Absolute));
            }
            else if (itemType == typeof(RuleGroupViewModel))
            {
                return this.GetIcon(item as RuleGroupViewModel);
            }
            else if (itemType == typeof(RuleViewModel))
            {
                return this.GetIcon(item as RuleViewModel);
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Umare.Presentation;component/Resources/Rule.png", UriKind.Absolute));
            }
        }

        private BitmapSource GetIcon(RuleGroupViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Umare.Presentation;component/Resources/ExecutionConditions.png", UriKind.Absolute));
        }

        private BitmapSource GetIcon(RuleViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Umare.Presentation;component/Resources/Rule.png", UriKind.Absolute));
        }
    }
}
