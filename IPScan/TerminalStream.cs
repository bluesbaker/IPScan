using IPScan.Scanners;
using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan
{
    public static class TerminalStream
    {
        static IPScan ipScan;

        public static void Run()
        {
            // Init
            ipScan = new IPScan();
            ipScan.ScannerCollection.Add(new PingScanner());

            string parametersLine = String.Empty;

            while (TryCommand(parametersLine))
            {
                Console.Write("> ");
                parametersLine = Console.ReadLine();
            }
        }

        private static bool TryCommand(string commandLine)
        {
            var parameters = IPScanParameters.Parse(commandLine.Split(' '), "-ip");

            foreach(var param in parameters)
            {
                switch (param.Key)
                {
                    case "--help":
                        RenderHelp();
                        return true;
                    case "--about":
                        RenderAbout();
                        return true;
                    case "-quit":
                        return false;
                    default:
                        TryScan(parameters);
                        return true;
                }
            }           

            return true;
        }

        private static void TryScan(IPScanParameters parameters)
        {
            try
            {
                var hosts = parameters["-ip"].ToString().Split('-');
                var fromIp = IPAddress.Parse(hosts[0]);
                var toIp = IPAddress.Parse(hosts[hosts.Length - 1]);
                var range = fromIp.Range(toIp);

                var resultCount = 0;

                foreach(var ip in range)
                {
                    // copy parameters to local parameters with single ip
                    var localParameters = parameters.Copy(new[] { "-ip", ip.ToString() });

                    ipScan.Init(localParameters);
                    Task<IPInfo> task = ipScan.Run();

                    RenderLoading("Scanning " + localParameters["-ip"], (() => !task.IsCompleted));

                    task.Wait();

                    var result = task.Result;

                    if (result["Status"] == "Success")
                    {
                        resultCount++;
                        var isFirstResult = resultCount == 1 ? true : false;
                        ResultViewer(result, isFirstResult);
                    }
                }

                Console.WriteLine();
            }
            catch (AggregateException exc)
            {
                ErrorViewer(exc.InnerExceptions.ToArray());
            }
            catch (Exception exc)
            {
                ErrorViewer(exc);
            }
        }

        private static void ResultViewer(IPInfo ipInfo, bool hasHeadersInfo = false)
        {
            if (hasHeadersInfo)
            {
                RenderResponseHeaders(ipInfo);
            }
            RenderResponse(ipInfo);       
        }

        private static void ErrorViewer(params Exception[] exceptions)
        {
            foreach (var exception in exceptions)
            {
                try
                {
                    throw exception;
                }
                catch (ScannerException exc)
                {
                    RenderError("Scanner error", exc);
                    RenderHelp();
                }
                catch (Exception exc)
                {
                    RenderError("System error`", exc);
                }
            }
        }

        private static void RenderHelp()
        {
            string helpString =
                "IPScan:\n" +
                "\t--help\t- FAQ\n" +
                "\t--clear\t- clear terminal\n\n";

            foreach (var scanner in ipScan.ScannerCollection)
            {
                helpString += $"{scanner}:\n";
                foreach(var attr in scanner.GetKeyAttributes())
                {
                    helpString += $"\t{attr.Key}\t- {attr.Description}\n";
                }
                helpString += "\n";
            }
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(helpString);
            Console.ResetColor();
        }

        private static void RenderResponse(IPInfo ipInfo, int fieldWidth = 20)
        {
            foreach (var field in ipInfo)
            {
                Console.Write($"{field.Value}");

                for (int i = 0; i < fieldWidth - field.Value.Length; i++)
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }

        private static void RenderResponseHeaders(IPInfo ipInfo, int fieldWidth = 20)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var field in ipInfo)
            {
                Console.Write($"{field.Key}");

                for (int i = 0; i < fieldWidth - field.Key.Length; i++)
                {
                    Console.Write(" ");
                }
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void RenderError(string title, Exception exc)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title);
            Console.ResetColor();
            Console.WriteLine($" {exc.Message} ");
        }

        private static void RenderLoading(string title, Func<bool> predicate, int pause = 100, int dotCount = 3)
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

        private static void RenderAbout()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Author");
            Console.ResetColor();
            Console.WriteLine(" github.com/bluesbaker\n");
        }
    }
}
