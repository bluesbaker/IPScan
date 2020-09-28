using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace IPScan.GUI.Providers
{
    public class SinglePortProvider : NPCBase, IDataErrorInfo, IPortProvider
    {
        #region Properties
        private string _singlePort;
        public string SinglePort
        {
            get => _singlePort;
            set
            {
                if (Set(ref _singlePort, value))
                {
                    SetError(Int32.TryParse(value, out _) ? null : $"{value} is not a port numeric");
                }
            }
        }
        #endregion

        #region Implementation IAddressProvider
        public bool IsValid => !Errors.Values.Any(x => x != null);

        public List<int> GetList()
        {
            return new List<int> { Int32.Parse(SinglePort) };
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
