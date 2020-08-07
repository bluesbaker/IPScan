using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;

namespace IPScan.BLL
{
    /// <summary>
    /// Parameters of Scanner
    /// </summary>
    public class ScannerParameters
    {
        // required
        public IPAddress Address { get; set; }

        // default
        public int Timeout { get; set; } = 1000;
        public int Port { get; set; } = 0;

        /// <summary>
        /// Parsing the dictionary of string to ScannerParameters
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
            catch (Exception exc)
            {
                throw new ScannerException("Parsing exception", exc);
            }

            return scannerParameters;
        }

        /// <summary>
        /// Returns the key setters collection(*for help)
        /// </summary>
        /// <returns>KeySetter collection</returns>
        public static IEnumerable<KeySetterAttribute> GetKeySetters()
        {
            var keySetters = new Collection<KeySetterAttribute>();
            var setters = new ScannerParameters().GetSetters();

            foreach (var setter in setters)
            {
                keySetters.Add(setter.GetCustomAttribute<KeySetterAttribute>());
            }

            return keySetters;
        }

        #region Setters
        [KeySetter("-ip", "Address")]
        public void AddressSetter(string value)
        {
            try
            {
                Address = IPAddress.Parse(value);
            }
            catch
            {
                Address = null;
            }
        }

        [KeySetter("-t", "Timeout")]
        public void TimeoutSetter(string value)
        {
            try
            {
                Timeout = Int32.Parse(value);
            }
            catch
            {
                Timeout = 1000;
            }
        }

        [KeySetter("-p", "Port")]
        public void PortSetter(string value)
        {
            try
            {
                Port = Int32.Parse(value);
            }
            catch
            {
                Port = 0;
            }
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
