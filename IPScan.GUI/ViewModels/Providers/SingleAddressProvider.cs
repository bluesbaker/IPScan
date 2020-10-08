using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace IPScan.GUI.ViewModels.Providers
{
    public class SingleAddressProvider : NPCBase, IAddressProvider, IDataErrorInfo
    {
        #region Properties
        private string _singleAddress;
        public string SingleAddress
        {
            get => _singleAddress;
            set
            {
                if (Set(ref _singleAddress, value))
                {
                    SetError(IPAddress.TryParse(value, out _) ? null : $"{value} is not ip address");
                }
            }
        }
        #endregion

        #region Implementation IAddressProvider
        public bool IsValid => !Errors.Values.Any(x => x != null);

        public List<IPAddress> GetList()
        {
            return new List<IPAddress> { IPAddress.Parse(SingleAddress) };
        }
        #endregion

        #region Implementation IDataErrorInFo
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
