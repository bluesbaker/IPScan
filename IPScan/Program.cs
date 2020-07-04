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
            string parametersLine = String.Empty;

            while(parametersLine.ToLower().Trim() != "q")
            {
                Task<IPInfo> task = null;
                IPScan ipScan = new IPScan();
                ipScan.ScannerCollection.Add(new PingScanner());

                Console.Write("> ");
                parametersLine = Console.ReadLine();

                try
                {
                    var ipParameters = IPScanParameters.Parse(parametersLine.Split(' '), "-ip");
                    ipScan.Init(ipParameters);

                    task = ipScan.Run();

                    ViewLoading("Scanning " + ipParameters["-ip"], (() => !task.IsCompleted));

                    task.Wait();

                    foreach (var prop in task.Result)
                    {
                        Console.Write(prop.Value + "\t\t|");
                    }

                    Console.WriteLine();
                }                
                catch
                {
                    foreach(var exception in task.Exception.InnerExceptions)
                    {
                        try
                        {
                            throw exception;
                        }
                        catch (ScannerException exc)
                        {
                            ViewError("Scanner error", exc.InnerException.Message);
                            // view help
                            Console.Write("Help:");
                            foreach (var scan in ipScan.ScannerCollection)
                            {
                                Console.WriteLine(scan.Help());
                            }
                        }
                        catch (Exception exc)
                        {
                            ViewError("System error", exc.Message);
                        }
                    }
                    //ViewError("System error", ex.InnerException.Message);
                }
            }           
            
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }

        #region Private Methods
        private static void ViewError(string title, string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title);
            Console.ResetColor();
            Console.WriteLine($" {message} ");
        }

        private static void ViewLoading(string title, Func<bool> predicate, int pause = 100, int dotCount = 3)
        {
            int dots = 0;

            // -> delete last symbol at terminal
            void WriteBackspace(int count)
            {
                int c = count;
                while (c != 0)
                {
                    Console.Write("\b \b");
                    c--;
                }
            }

            // view
            Console.Write(title);
            while (predicate.Invoke())
            {
                dots++;
                Console.Write(".");
                Thread.Sleep(pause);
                
                // clear dots
                if (dots >= dotCount)
                {
                    WriteBackspace(dots);
                    dots = 0;
                }                
            }

            // clear last dots
            WriteBackspace(dots);

            // clear title
            WriteBackspace(title.Length);
        }
        #endregion

        #region Private
        static readonly string welcomeMessage =
            $"IPScan – scanning ip-address\n" +
            $"----------------------------\n" +
            "Usage:\t192.168.0.1-192.168.0.255\n" +
            "Help:\t--help or -help\n";
        #endregion
    }
}
