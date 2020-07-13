using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using IPScan.Supports;

namespace IPScan
{
    public class IPScanParameters
    {
        [Key("-ip", "Address or range", true)]
        public IPAddress Address { get; set; }

        [Key("-t", "Timeout")]
        public int Timeout { get; set; }

        public static IPScanParameters Parse(Dictionary<string, string> collection)
        {
            var parameters = new IPScanParameters();

            parameters.Address = IPAddress.Parse(collection["-ip"]);
            parameters.Timeout = Int32.Parse(collection["-t"]);

            return parameters;
        }

        private KeyAttribute GetKeyAttribute(string key)
        {
            var attributes = GetType().GetCustomAttributes<KeyAttribute>();
            return attributes.FirstOrDefault(a => a.Key == key);
        }

        private IEnumerable<KeyAttribute> GetKeyAttributes() => GetType().GetCustomAttributes<KeyAttribute>();
    }
}
