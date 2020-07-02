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
            ScanParameters.Clear();

            // default parameters by attribute
            foreach (var attr in GetType().GetCustomAttributes<ScannerArgumentAttribute>())
            {
                ScanParameters[attr.Argument] = attr.DefaultValue;
            }

            // init
            foreach(var param in parameters)
            {
                ScanParameters[param.Key] = param.Value;
            }
        }

        protected IPScanParameters ScanParameters { get; set; } = new IPScanParameters();
    } 
}
