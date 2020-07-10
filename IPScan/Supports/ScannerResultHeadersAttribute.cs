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
        public ScannerResultHeadersAttribute(string responder, params string[] headers)
        {
            Responder = responder;
            Headers = headers;
        }

        public string Responder { get; private set; }
        public string[] Headers { get; private set; }
    }
}
