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
            string[] tmpArg = { "ipscan", "-ip", "8.8.8.8" };
            Console.WriteLine(welcomeMessage);

            string parametersLine = String.Empty;
            while(parametersLine.ToLower().Trim() != "q")
            {
                ScannerWizard wizard = new ScannerWizard();
                wizard.ScanCollection.Add(new PingScanner());

                parametersLine = Console.ReadLine();

                wizard.Init(Parameters.Parse(parametersLine.Split(' '), "-ip"));

                var task = wizard.Run();

                while (!task.IsCompleted)
                {
                    string loading = DateTime.Now.Second % 2 == 0 ? "[.'.'.'.'.]" : "['.'.'.'.']";
                    Console.Write(loading);
                    Thread.Sleep(1000);
                    Console.Write("\b\b\b\b\b\b\b\b\b\b\b           \b\b\b\b\b\b\b\b\b\b\b");
                }
                task.Wait();

                Console.WriteLine();
                foreach(var prop in task.Result)
                {
                    Console.Write(prop.Value + "\t\t|");
                }
                Console.WriteLine();
            }
            

            
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }

        #region Private
        static readonly string welcomeMessage =
            $"ipscan – scanning ip-address\n______\n" +
            "usage: 192.168.0.1-192.168.0.255\n" +
            " help: --help\n";
        #endregion

        
    }
}
