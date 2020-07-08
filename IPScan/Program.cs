using IPScan.Scanners;
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
            Console.WriteLine(welcomeMessage);

            TerminalStream.Run();
            
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }
        
        static readonly string welcomeMessage =
            $"IPScan – scanning ip-address\n" +
            $"----------------------------\n" +
            "Usage:\t192.168.0.1-192.168.0.255\n" +
            "Help:\t--help or -help\n";       
    }
}
