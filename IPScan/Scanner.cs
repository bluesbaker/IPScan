using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    public class Scanner
    {
        public Scanner(string ip, int timeout)
        {
            Address = IPAddress.Parse(ip);
            Timeout = timeout;
        }

        public IPAddress Address { get; set; }
        public int Timeout { get; set; }

        public PingReply Run()
        {
            var ping = new Ping();
            return ping.Send(Address, Timeout);           
        }
    }
}
