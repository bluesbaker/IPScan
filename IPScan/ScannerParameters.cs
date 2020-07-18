using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using IPScan.Supports;
using System.Collections.ObjectModel;

namespace IPScan
{
    public class ScannerParameters
    {
        // required
        public IPAddress Address { get; set; }

        // default
        public int Timeout { get; set; } = 1000;
        public int Port { get; set; } = 0;


        /// <summary>
        /// Parsing a string dictionary to ScannerParameters with instance properties
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Scanner parameters</returns>
        public static ScannerParameters Parse(Dictionary<string, string> parameters)
        {
            var scannerParameters = new ScannerParameters();

            try
            {
                foreach (var param in parameters)
                {
                    var setter = scannerParameters.GetSetter(param.Key);
                    setter?.Invoke(scannerParameters, new[] { param.Value });
                }
            }
            catch(Exception exc)
            {
                throw new ScannerException("Parsing exception", exc);
            }

            return scannerParameters;
        }

        /// <summary>
        /// Checking required keys
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Returns true if the required keys exists in the collection</returns>
        public static bool CheckingRequiredKeys(Dictionary<string, string> parameters)
        {
            var isCheck = true;

            foreach(var keySetter in GetKeySetters())
            {
                var param = parameters.FirstOrDefault(p => p.Key == keySetter.Key);

                if(keySetter.IsRequired == true && param.Value == null)
                {
                    isCheck = false;
                }
            }

            return isCheck;
        }


        /// <summary>
        /// Return key setters collection(*for help)
        /// </summary>
        /// <returns>KeySetter collection</returns>
        public static IEnumerable<KeySetterAttribute> GetKeySetters()
        {
            var keySetters = new Collection<KeySetterAttribute>();
            var setters = new ScannerParameters().GetSetters();

            foreach(var setter in setters)
            {
                keySetters.Add(setter.GetCustomAttribute<KeySetterAttribute>());
            }

            return keySetters;
        }

        #region Setters
        [KeySetter("-ip", "Address or range", true)]
        public void AddressSetter(string value)
        {
            Address = IPAddress.Parse(value);
        }

        [KeySetter("-t", "Timeout")]
        public void TimeoutSetter(string value)
        {
            Timeout = Int32.Parse(value);
        }

        [KeySetter("-p", "Port")]
        public void PortSetter(string value)
        {
            Port = Int32.Parse(value);
        }
        #endregion


        #region Tools
        private MethodInfo GetSetter(string key) => 
            GetSetters().FirstOrDefault(s => s.GetCustomAttribute<KeySetterAttribute>().Key == key);

        private IEnumerable<MethodInfo> GetSetters() => 
            GetType().GetMethods().Where(m => m.GetCustomAttribute<KeySetterAttribute>() != null);
        #endregion
    }
}
