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
            throw new Exception("Application error: Not overridden the scan method");
        }

        public void Init(IPScanParameters parameters)
        {
            // clear!
            ScannerParameters.Clear();

            // default parameters by attribute
            foreach (var scannerKey in GetType().GetCustomAttributes<ScannerKeyAttribute>())
            {
                ScannerParameters[scannerKey.TextKey] = scannerKey.DefaultValue?.ToString();
            }

            // init
            foreach (var param in parameters)
            {
                ScannerParameters[param.Key] = param.Value;
            }
        }

        protected IPScanParameters ScannerParameters { get; private set; } = new IPScanParameters();
    } 
}
