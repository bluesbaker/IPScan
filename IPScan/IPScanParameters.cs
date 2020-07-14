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
    public class IPScanParameters
    {
        // required
        public IPAddress Address { get; set; }

        // default
        public int Timeout { get; set; } = 1000;
        public int Port { get; set; } = 0;


        /// <summary>
        /// Parsing a string dictionary to IPScanParameters with instance properties
        /// </summary>
        /// <param name="collection">Key collection</param>
        /// <returns>IPScan parameters</returns>
        public static IPScanParameters Parse(Dictionary<string, string> collection)
        {
            var parameters = new IPScanParameters();

            foreach(var field in collection)
            {
                var setter = parameters.GetSetter(field.Key);
                setter?.Invoke(parameters, new[] { field.Value });
            }

            return parameters;
        }

        /// <summary>
        /// Checking required keys
        /// </summary>
        /// <param name="collection">Key collection</param>
        /// <returns>Returns true if the required keys exists in the collection</returns>
        public static bool CheckingRequiredKeys(Dictionary<string, string> collection)
        {
            var isCheck = true;

            foreach(var keySetter in GetKeySetters())
            {
                var field = collection.FirstOrDefault(f => f.Key == keySetter.Key);

                if(keySetter.IsRequired == true && field.Value == null)
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
            var setters = new IPScanParameters().GetSetters();

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
