using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace IPScan.GUI.Providers
{
    public class RangePortProvider : NPCBase, IDataErrorInfo, IPortProvider
    {
        #region Properties
        private string _startPort;
        public string StartPort
        {
            get => _startPort;
            set
            {
                if (Set(ref _startPort, value))
                {
                    SetError(Int32.TryParse(value, out _) ? null : $"{value} is not a port numeric");
                }
            }
        }

        private string _endPort;
        public string EndPort
        {
            get => _endPort;
            set
            {
                if (Set(ref _endPort, value))
                {
                    SetError(Int32.TryParse(value, out _) ? null : $"{value} is not a port numeric");
                }
            }
        }
        #endregion

        #region Implementation IAddressProvider
        public List<int> GetList()
        {
            var startPort = Int32.Parse(StartPort);
            var endPort = Int32.Parse(EndPort);
            return Enumerable.Range(startPort, endPort - startPort + 1).ToList();
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
