using IPScan.GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IPScan.GUI.Support
{
    public static class UIAPConfigurator
    {
        // 
        public static readonly DependencyProperty CollectionFilterProperty =
            DependencyProperty.RegisterAttached("CollectionFilter", typeof(Predicate<object>), typeof(UIAPConfigurator));
        public static void SetCollectionFilter(DependencyObject element, Predicate<object> value)
        {
            element.SetValue(CollectionFilterProperty, value);
        }
        public static Predicate<object> GetCollectionFilter(DependencyObject element)
        {
            return (Predicate<object>)element.GetValue(CollectionFilterProperty);
        }

        public static readonly DependencyProperty IsFiltredProperty =
            DependencyProperty.RegisterAttached("IsFiltred", typeof(bool), typeof(UIAPConfigurator), new PropertyMetadata(OnIsFiltredPropertyChanged));
        public static void SetIsFiltred(DependencyObject element, bool value)
        {
            element.SetValue(IsFiltredProperty, value);
        }
        public static bool GetIsFiltred(DependencyObject element)
        {
            return (bool)element.GetValue(IsFiltredProperty);
        }

        private static void OnIsFiltredPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                var isFiltred = (bool)e.NewValue;
                if (isFiltred)
                {
                    var predicate = itemsControl.GetValue(CollectionFilterProperty) as Predicate<object>;
                    itemsControl.Items.Filter = predicate;
                    itemsControl.Items.Refresh();
                }
                else
                {
                    itemsControl.Items.Filter = null;
                    itemsControl.Items.Refresh();
                }
            }
        }

    }
}
