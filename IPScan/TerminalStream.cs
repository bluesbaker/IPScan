using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

            while (TryCommand(parametersLine))
            {
                Console.Write("> ");
                parametersLine = Console.ReadLine();
            }
        }

        private static bool TryCommand(string commandLine)
        {
            var parameters = Parameters.Parse(commandLine.Split(' '), "-ip");

            foreach(var param in parameters)
            {
                switch (param.Key)
                {
                    case "--help":
                        HelpViewer();
                        return true;
                    case "--about":
                        AboutViewer();
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

        private static void TryScan(Parameters parameters)
        {
            try
            {
                // parse one or more ip adresses to collection
                var addresses = parameters["-ip"].ToString().Split('-');
                var ipCollection = IPAddressRange.Get(addresses[0], addresses[addresses.Length - 1]);

                var resultCount = 0;

                // scanning the collection
                foreach(var ip in ipCollection)
                {
                    var ipScanParameters = new IPScanParameters()
                    {
                        Address = ip,
                        Timeout = 1000
                    };
                    var ipScan = new IPScan(ipScanParameters);

                    // request
                    Task<PingReply> task = ipScan.GetPingReplyAsync();

                    RenderLoading("Scanning " + ipScanParameters.Address, (() => !task.IsCompleted));
                    task.Wait();

                    // response
                    var result = task.Result;

                    // view ping reply
                    if (result.Status == IPStatus.Success)
                    {
                        // also view hearders if first result
                        PingReplyViewer(result, resultCount == 0);
                        resultCount++;
                    }
                }

                Console.WriteLine($"\nTotal results: {resultCount}");
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


        #region Viewers
        private static void PingReplyViewer(PingReply pingReply, bool hasHeaders = false)
        {
            // data
            var address = pingReply.Address.ToString();
            var status = pingReply.Status.ToString();
            var roundtripTime = pingReply.RoundtripTime.ToString();

            var statusColor = 
                pingReply.Status == IPStatus.Success ? ConsoleColor.Green : ConsoleColor.White;

            if (hasHeaders)
            {
                // view headers
                RenderField("Address", ConsoleColor.DarkBlue, fieldWidth: 20);
                RenderField("Status", ConsoleColor.DarkBlue, fieldWidth: 20);
                RenderField("Roundtrip time", ConsoleColor.DarkBlue, fieldWidth: 20);

                Console.WriteLine();
            }

            // view address, status and ping*
            RenderField(address, fieldWidth: 20);
            RenderField(status, fgColor: statusColor, fieldWidth: 20);
            RenderField(roundtripTime, fieldWidth: 20);

            Console.WriteLine();
        }

        private static void ErrorViewer(params Exception[] exceptions)
        {
            foreach (var exception in exceptions)
            {
                try
                {
                    throw exception;
                }
                //catch (ScannerException exc)
                //{
                //    RenderError("Scanner error", exc);
                //    RenderHelp();
                //}
                catch (Exception exc)
                {
                    RenderField("System error", bgColor: ConsoleColor.DarkRed);
                    Console.WriteLine($" {exc.Message} ");

                    HelpViewer();
                }
            }
        }

        private static void HelpViewer()
        {
            string helpString =
                "IPScan:\n" +
                "\t--help\t- FAQ\n" +
                "\t--clear\t- clear terminal\n\n";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(helpString);
            Console.ResetColor();
        }

        private static void AboutViewer()
        {
            RenderField("Author", fieldWidth: 20);
            Console.WriteLine("github.com/bluesbaker");

            RenderField("Copyright", fieldWidth: 20);
            Console.WriteLine("...-2020");
        }
        #endregion


        #region Optional rendering tools
        /// <summary>
        /// Rendering text of loading for example "Please wait..."
        /// </summary>
        private static void RenderLoading(string title, Func<bool> predicate, int pause = 100, int dotCount = 3)
        {
            int dots = 0;

            // -> delete last symbols at terminal
            void WriteBackspace(int count)
            {
                for (int c = 0; c < count; c++)
                {
                    Console.Write("\b \b");
                }
            }

            Console.Write(title);

            // render dots
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

        /// <summary>
        /// Rendering a color field in terminal
        /// </summary>
        private static void RenderField(string textField, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.White, int fieldWidth = 0)
        {
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;

            Console.Write(textField);

            if (fieldWidth >= textField.Length)
            {
                // add remaining spaces
                var spaces = new String(' ', fieldWidth - textField.Length);
                Console.Write(spaces);
            }

            Console.ResetColor();
        }
        #endregion
    }
}
