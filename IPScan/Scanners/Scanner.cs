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

        public void Init(Parameters parameters)
        {
            // clear!
            Parameters.Clear();

            // default parameters by attribute
            foreach (var attr in GetType().GetCustomAttributes<DeclarateKeyAttribute>())
            {
                Parameters[attr.Key] = attr.DefaultValue;
            }

            // init
            foreach(var param in parameters)
            {
                Parameters[param.Key] = param.Value;
            }
        }

        protected Parameters Parameters { get; set; } = new Parameters();
    } 
}
