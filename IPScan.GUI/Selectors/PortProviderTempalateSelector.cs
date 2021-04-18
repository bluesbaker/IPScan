using IPScan.GUI.ViewModels.Providers;
using System.Windows;
using System.Windows.Controls;

namespace IPScan.GUI.Selectors
{
    public class PortProviderTempalateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            IPortProvider provider = item as IPortProvider;

            if(provider is SinglePortProvider)
                return element.FindResource("SinglePortDataTemplate") as DataTemplate;
            else if(provider is RangePortProvider)
                return element.FindResource("RangePortDataTemplate") as DataTemplate;

            return element.FindResource("SinglePortDataTemplate") as DataTemplate;
        }
    }
}
