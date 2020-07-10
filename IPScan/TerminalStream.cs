using IPScan.Scanners;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        RenderScan(parameters);
                        return true;
                }
            }           

            return true;
        }

        private static void RenderScan(IPScanParameters parameters)
        {
            try
            {
                Task<IPInfo> task = null;

                ipScan.Init(parameters);
                task = ipScan.Run();

                RenderLoading("Scanning " + parameters["-ip"], (() => !task.IsCompleted));

                task.Wait();

                ResultViewer(task.Result);

                Console.WriteLine();
            }
            catch (AggregateException exc)
            {
                ErrorViewer(exc.InnerExceptions);
            }
        }

        private static void ResultViewer(IPInfo ipInfo)
        {
            RenderResponseHeaders(ipInfo);
            RenderResponse(ipInfo);
        }

        private static void ErrorViewer(ICollection<Exception> exceptions)
        {
            foreach (var exception in exceptions)
            {
                try
                {
                    throw exception;
                }
                catch (ScannerException exc)
                {
                    RenderError("Scanner error", exc.Message);
                    RenderHelp();
                }
                catch (Exception exc)
                {
                    RenderError("System error", exc.Message);
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

        private static void RenderError(string title, string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title);
            Console.ResetColor();
            Console.WriteLine($" {message} ");
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
