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
        public static void Run()
        {
            string parametersLine = String.Empty;

            while (parametersLine.ToLower().Trim() != "q")
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

                    RenderLoading("Scanning " + ipParameters["-ip"], (() => !task.IsCompleted));

                    task.Wait();

                    ResultViewer(task.Result);

                    Console.WriteLine();
                }
                catch (AggregateException exc)
                {
                    ErrorViewer(exc.InnerExceptions);
                }
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
                "Usage:\t--help\t- FAQ\n" +
                "\t-ip\t- address or range\n" +
                "\t-t\t- timeout";
            Console.WriteLine(helpString);
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
    }
}
