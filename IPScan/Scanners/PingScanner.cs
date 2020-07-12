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
    [ScannerKey("-ip", "Address", null, true)]
    [ScannerKey("-t", "Timeout", 1000)]
    [ScannerResultHeaders("PingScanner", new[]{"Address", "Status", "Ping"})]
    public class PingScanner : Scanner
    {
        public override async Task<IPInfo> Run()
        {           
            var result = new IPInfo();

            try
            {
                var ping = new Ping();
                var host = ScannerParameters["-ip"].ToString();
                var timeout = int.Parse(ScannerParameters["-t"].ToString());

                // request                
                var reply = await ping.SendPingAsync(host, timeout);               

                // response
                result["Address"] = reply.Address.ToString();
                result["Status"] = reply.Status.ToString();
                result["Ping"] = reply.RoundtripTime.ToString();
            }
            catch(PingException ex)
            {
                throw new ScannerException($"{ex.Message} -> {ex.InnerException.Message}", ex);
            }

            return result;
        }

    }
}
