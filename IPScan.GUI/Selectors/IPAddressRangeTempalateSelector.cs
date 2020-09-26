using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IPScan.GUI.Selectors
{
    public class IPAddressRangeTempalateSelector : DataTemplateSelector
    {
        public DataTemplate SingleAddressDataTemplate { get; set; }
        public DataTemplate AddressRangeDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is SingleIPAddressProvider)
            {
                return SingleAddressDataTemplate;
            }
            if(item is RangeIPAddressProvider)
            {
                return AddressRangeDataTemplate;
            }
            return SingleAddressDataTemplate;
        }
    }
}
