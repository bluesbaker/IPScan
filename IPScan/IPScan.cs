using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan
{
    public class IPScan
    {
        public IPScan(IPScanParameters scanParameters)
        {
            _scanParameters = scanParameters;
        }

        public async Task<PingReply> GetPingReplyAsync()
        {
            var ping = new Ping();

            var address = _scanParameters.Address;
            var timeout = _scanParameters.Timeout;

            // request                
            var reply = await ping.SendPingAsync(address, timeout);

            return reply;
        }

        private IPScanParameters _scanParameters;
    }

    public class IPScanException : Exception
    {
        public IPScanException(string message, Exception innerException) : base(message, innerException)
        {
            // your advertisement could be here...
        }
    }
}
