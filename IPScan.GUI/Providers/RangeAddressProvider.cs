using IPScan.GUI.Support;
using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;

namespace IPScan.GUI.Providers
{
    public class RangeAddressProvider : NPCBase, IAddressProvider, IDataErrorInfo
    {
        #region Properties
        private string _startAddress;
        public string StartAddress
        {
            get => _startAddress;
            set
            {
                if(Set(ref _startAddress, value))
                {
                    SetError(IPAddress.TryParse(value, out _) ? null : $"{value} is not ip address");
                }
            }
        }

        private string _endAddress;
        public string EndAddress
        {
            get => _endAddress;
            set
            {
                if (Set(ref _endAddress, value))
                {
                    SetError(IPAddress.TryParse(value, out _) ? null : $"{value} is not ip address");
                }
            }
        }
        #endregion

        #region Implementation IAddressProvider
        public List<IPAddress> GetList()
        {
            var startAddress = IPAddress.Parse(StartAddress);
            var endAddress = IPAddress.Parse(EndAddress);
            return startAddress.Range(endAddress);
        }
        public bool IsValid => !Errors.Values.Any(x => x != null);
        #endregion

        #region IDataErrorInfo
        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        private void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            if (Errors.ContainsKey(propertyName))
                Errors[propertyName] = message;
            else
                Errors.Add(propertyName, message);
        }

        public string Error => string.Join(Environment.NewLine, Errors.Where(pair => pair.Value != null).Select(pair => $"{pair.Key}: \"{pair.Value}\""));

        public string this[string columnName] => Errors.TryGetValue(columnName, out string value) ? value : null;
        #endregion
    }
}
