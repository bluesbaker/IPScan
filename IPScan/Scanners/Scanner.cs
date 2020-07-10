using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Scanners
{
    public abstract class Scanner : IScanner
    {
        public virtual Task<IPInfo> Run()
        {
            throw new Exception("Not overridden the scan method");
        }

        public void Init(IPScanParameters parameters)
        {
            var filtredParameters = from p in parameters
                                where GetKeyAttribute(p.Key) != null
                                where p.Value != null
                                select p;

            // initialization
            ScannerParameters.Clear();
            foreach (var attr in GetKeyAttributes())
            {
                // external param or default
                var param =
                    filtredParameters.FirstOrDefault(p => p.Key == attr.Key).Value ?? attr.DefaultValue;
                
                // param or exception
                ScannerParameters[attr.Key] =
                    param ?? throw new ScannerException($"Not exist '{attr.Key}' key in required parameters");    
            }
        }

        public string Help(string key = "")
        {
            if (key != String.Empty)
            {
                return GetKeyAttribute(key).Description;
            }
            else
            {
                string result = String.Empty;

                foreach(var attr in GetKeyAttributes())
                {
                    result += $"\t{attr.Key}\t- {attr.Description}\n";
                }

                return result;
            }
        }

        public ScannerKeyAttribute GetKeyAttribute(string key)
        {
            var attributes = GetType().GetCustomAttributes<ScannerKeyAttribute>();
            return attributes.FirstOrDefault(a => a.Key == key);
        }

        public IEnumerable<ScannerKeyAttribute> GetKeyAttributes() => GetType().GetCustomAttributes<ScannerKeyAttribute>();

        protected IPScanParameters ScannerParameters { get; private set; } = new IPScanParameters();
    }
    
    public class ScannerException : Exception
    {
        public ScannerException(string message, Exception innerException = null) : base(message, innerException)
        {
            // your advertisement could be here...
        }
    }
}
