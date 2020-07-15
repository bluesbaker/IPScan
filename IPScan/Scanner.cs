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
    public class Scanner
    {
        public Scanner(ScannerParameters scannerParameters)
        {
            Parameters = scannerParameters;
        }

        public async Task<PingReply> GetPingReplyAsync()
        {
            var ping = new Ping();

            var address = Parameters.Address;
            var timeout = Parameters.Timeout;
            PingReply reply = null;

            // request
            try
            {
                reply = await ping.SendPingAsync(address, timeout);               
            }
            catch (Exception exc)
            {
                throw new ScannerException("Ping exception", exc);
            }

            return reply;
        }

        public ScannerParameters Parameters;
    }

    public class ScannerException : Exception
    {
        public ScannerException(string message, Exception innerException) : base(message, innerException)
        {
            // your advertisement could be here...
        }
    }
}
