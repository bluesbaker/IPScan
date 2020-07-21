using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "IPScan";
            StartupViewer();

            var terminalParameters = TerminalParameters.Parse(args, "-ip");
            TerminalStream.Run(terminalParameters);
        }
        
        static void StartupViewer()
        {
            string startupMessage =
                $"IPScan – scanning ip-addresses\n" +
                $"----------------------------\n" +
                "Usage:\t-ip 192.168.0.1-192.168.0.255 -p 80\n" +
                "Help:\t--help\n";

            Console.WriteLine(startupMessage);
        }
    }
}
