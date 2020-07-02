using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Supports
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScannerResultHeadersAttribute : Attribute
    {
        public ScannerResultHeadersAttribute(params string[] headers)
        {
            Headers = headers;
        }

        public string[] Headers { get; private set; }
    }
}
