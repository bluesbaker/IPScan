using IPScan.GUI.ViewModels.Providers;
using System.Windows;
using System.Windows.Controls;

namespace IPScan.GUI.Selectors
{
    public class AddressProviderTempalateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            IAddressProvider provider = item as IAddressProvider;

            if(provider is SingleAddressProvider)
                return element.FindResource("SingleAddressDataTemplate") as DataTemplate;
            else if(provider is RangeAddressProvider)
                return element.FindResource("RangeAddressDataTemplate") as DataTemplate;

            return element.FindResource("SingleAddressDataTemplate") as DataTemplate;
        }
    }
}
