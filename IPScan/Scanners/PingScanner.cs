using IPScan.Supports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan.Scanners
{
    [ScannerArgument("-ip", "Address", null, true)]
    [ScannerArgument("-t", "Timeout", 1000)]
    [ScannerResultHeaders("Address", "Status", "Ping")]
    public class PingScanner : Scanner
    {
        public override async Task<IPInfo> Run()
        {
            var ping = new Ping();

            // async ping address
            var reply = await ping.SendPingAsync(base.ScanParameters["-ip"], Int32.Parse(base.ScanParameters["-t"]));

            // set result
            var result = new IPInfo()
            {
                ["Address"] = reply.Address.ToString(),
                ["Status"] = reply.Status.ToString()
            };
            return result;
        }

    }
}
