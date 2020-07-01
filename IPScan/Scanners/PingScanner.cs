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
    [DeclarateKey("-t", "Timeout", "1000")]
    public class PingScanner : Scanner
    {
        public override async Task<IPInfo> Run()
        {
            var ping = new Ping();

            // async ping address
            var reply = await ping.SendPingAsync(base.Parameters["-ip"], Int32.Parse(base.Parameters["-t"]));

            // set result
            var result = new IPInfo()
            {
                ["Status"] = reply.Status.ToString()
            };
            Thread.Sleep(5000);
            return result;
        }

    }
}
