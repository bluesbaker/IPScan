using System;
using System.Windows;
using System.Windows.Controls;

namespace IPScan.GUI.Support
{
    public static class ItemsControlAssist
    {
        /// <summary>
        /// The filter for ItemsControl
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached("Filter", typeof(Predicate<object>), typeof(ItemsControlAssist));
        public static void SetFilter(DependencyObject element, Predicate<object> value) => element.SetValue(FilterProperty, value);
        public static Predicate<object> GetFilter(DependencyObject element) => (Predicate<object>)element.GetValue(FilterProperty);

        /// <summary>
        /// Will the filter be applied?
        /// </summary>
        public static readonly DependencyProperty IsFiletredProperty =
            DependencyProperty.RegisterAttached("IsFiltered", typeof(bool), typeof(ItemsControlAssist), new PropertyMetadata(OnIsFilteredPropertyChanged));
        public static void SetIsFiltered(DependencyObject element, bool value) => element.SetValue(IsFiletredProperty, value);
        public static bool GetIsFiltered(DependencyObject element) => (bool)element.GetValue(IsFiletredProperty);

        private static void OnIsFilteredPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                var isFiltred = (bool)e.NewValue;
                if (isFiltred)
                {
                    var filter = itemsControl.GetValue(FilterProperty) as Predicate<object>;
                    itemsControl.Items.Filter = filter;
                }
                else
                {
                    itemsControl.Items.Filter = null;
                }
                itemsControl.Items.Refresh();
            }
        }
    }
}
