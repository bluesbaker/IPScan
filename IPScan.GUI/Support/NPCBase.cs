using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IPScan.GUI.Support
{
    /// <summary>
    /// Implementation of INotifyPropertyChanged
    /// </summary>
    public class NPCBase : INotifyPropertyChanged
    {       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T _filed, T value, [CallerMemberName] string propertyName = "")
        {
            if ((_filed == null && value == null) || (_filed != null && _filed.Equals(value)))
                return false;

            _filed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
