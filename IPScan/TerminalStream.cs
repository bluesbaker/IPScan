using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static void Run(TerminalParameters beginParameters)
        {
            var parameters = beginParameters;

            while (TryCommand(parameters))
            {
                Console.Write("> ");

                var commandLine = Console.ReadLine().Split(' ');
                parameters = TerminalParameters.Parse(commandLine, "-ip");
            }
        }

        #region Controls
        private static bool TryCommand(TerminalParameters parameters)
        {
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
                    case "--quit":
                        return false;
                    default:
                        TryScan(parameters);
                        return true;
                }
            }           

            return true;
        }

        private static void TryScan(TerminalParameters commandParameters)
        {
            try
            {
                // check parameters
                var isCheck = ScannerParameters.CheckingRequiredKeys(commandParameters);
                if(isCheck == false)
                {
                    throw new Exception("One or more required parameters is missing");
                }

                // split addresses to collection
                var addresses = commandParameters["-ip"].Split('-');
                var addressCollection = IPAddress.Parse(addresses[0]).Range(addresses[addresses.Length - 1]);         

                var pingResultCount = 0;

                foreach(var address in addressCollection)
                {
                    // copying params with only one address(without range)
                    var injection = new Dictionary<string, string>()
                    {
                        ["-ip"] = address.ToString()
                    };         
                    var parameters = commandParameters.Copy(injection);
                    var scanner = new Scanner(ScannerParameters.Parse(parameters));

                    // ping request
                    Task<PingReply> pingTask = scanner.GetPingReplyAsync();

                    Console.Title = "Scanning " + address;
                    RenderLoading("Scanning " + address, (() => !pingTask.IsCompleted));
                    pingTask.Wait();

                    // ping response
                    if (pingTask.Result.Status == IPStatus.Success)
                    {
                        // view ping reply and also hearders if first result
                        PingReplyViewer(pingTask.Result, pingResultCount == 0);
                        pingResultCount++;
                        
                        if (scanner.Parameters.Port > 0)
                        {
                            // port request
                            Task<bool> portTask = scanner.GetPortAccessAsync();
                    
                            RenderLoading("Scanning port " + scanner.Parameters.Port, (() => !portTask.IsCompleted));
                            portTask.Wait();
                    
                            // port response
                            var portAccess = portTask.Result;
                    
                            // view port status
                            PortAccessViewer(scanner.Parameters.Port, portAccess);                            
                        }
                        
                    }
                }

                Console.WriteLine($"\nTotal results: {pingResultCount}");
            }
            catch (AggregateException exc)
            {
                ErrorViewer(exc.InnerExceptions.ToArray());
            }
            catch (Exception exc)
            {
                ErrorViewer(exc);
            }
            finally
            {
                Console.Title = "IPScan";
            }
        }
        #endregion


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

        private static void PortAccessViewer(int port, bool isAccessed, bool hasHeaders = false)
        {
            var statusColor =
                isAccessed == true ? ConsoleColor.Green : ConsoleColor.Red;

            if (hasHeaders)
            {
                // view headers
                RenderField("Port", bgColor: ConsoleColor.DarkGray, fieldWidth: 20);
                RenderField("Status", bgColor: ConsoleColor.DarkGray, fieldWidth: 20);

                Console.WriteLine();
            }

            // view port and his status
            RenderField($"Port:", fgColor: ConsoleColor.DarkGray, fieldWidth: 5);
            RenderField(port.ToString(), fieldWidth: 15);
            RenderField(isAccessed.ToString(), fgColor: statusColor, fieldWidth: 20);

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
                catch (ScannerException exc)
                {
                    RenderField("Scanner error", bgColor: ConsoleColor.DarkRed);
                    Console.WriteLine($" {exc.Message} ");
                }
                catch (Exception exc)
                {
                    RenderField("System error", bgColor: ConsoleColor.DarkRed);
                    Console.WriteLine($" {exc.Message} ");
                }
                finally
                {
                    HelpViewer();
                }
            }
        }

        private static void HelpViewer()
        {
            string helpString = "IPScan:\n";

            foreach(var setter in ScannerParameters.GetKeySetters())
            {
                helpString += $"{setter.Key}\t\t- {setter.Description}\n";
            }

            helpString += 
                "--help\t\t- FAQ\n" +
                "--about\t\t- About\n" +
                "--quit\t\t- Quit\n";

            RenderField(helpString, fgColor: ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        private static void AboutViewer()
        {
            RenderField("Author", fieldWidth: 20);
            Console.WriteLine("github.com/bluesbaker");

            RenderField("Copyright", fieldWidth: 20);
            Console.WriteLine("...-2020");
        }
        #endregion


        #region Additional rendering tools
        /// <summary>
        /// Rendering text of loading for example "Please wait..."
        /// </summary>
        private static void RenderLoading(string title, Func<bool> predicate, int pause = 100, int dotCount = 3)
        {
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
            int dots = 0;  
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
