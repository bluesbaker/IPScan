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
                        Scanning(parameters);
                        return true;
                }
            }           

            return true;
        }

        private static void Scanning(TerminalParameters commandParameters)
        {
            try
            {
                // checking parameters
                var isCheck = ScannerParameters.CheckingRequiredKeys(commandParameters);
                if(isCheck == false)
                {
                    throw new ScannerException("One or more required parameters is missing");
                }

                // split addresses to the range* collection
                var addresses = commandParameters["-ip"].Split('-');
                var startAddress = IPAddress.Parse(addresses[0]);
                var endAddress = IPAddress.Parse(addresses[addresses.Length - 1]);
                var addressRange = startAddress.Range(endAddress);         

                var pingResultCount = 0;

                foreach(var address in addressRange)
                {
                    // copying params with only one address(without range)
                    var parameters = commandParameters.Copy();
                    parameters["-ip"] = address.ToString();

                    var scanner = new Scanner(ScannerParameters.Parse(parameters));

                    // ping request
                    Task<PingReply> pingTask = scanner.GetPingReplyAsync();

                    Console.Title = "Scanning " + address;
                    ConsoleRender.Loader("Scanning " + address, (() => !pingTask.IsCompleted));
                    pingTask.Wait();

                    // ping response
                    if (pingTask.Result.Status == IPStatus.Success)
                    {
                        // view ping reply and also hearders if result is first
                        PingReplyViewer(pingTask.Result, pingResultCount == 0);
                        pingResultCount++;
                        
                        if (scanner.Parameters.Port > 0)
                        {
                            // port request
                            Task<bool> portTask = scanner.GetPortAccessAsync();
                    
                            ConsoleRender.Loader("Scanning port " + scanner.Parameters.Port, (() => !portTask.IsCompleted));
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

            var statusColor = pingReply.Status == IPStatus.Success ? ConsoleColor.Green : ConsoleColor.White;

            if (hasHeaders)
            {
                // view headers
                ConsoleRender.FieldsLine(new[] { "Address", "Status", "Roundtrip time" }, 20, new ColorSection(ConsoleColor.DarkBlue));
            }

            // view address, status and ping*
            ConsoleRender.FieldsLine(new[] { address, status, roundtripTime }, 20, new ColorSection(foreground: statusColor, text: status));
        }

        private static void PortAccessViewer(int port, bool isAccessed, bool hasHeaders = false)
        {
            var statusColor = isAccessed == true ? ConsoleColor.Green : ConsoleColor.Red;

            if (hasHeaders)
            {
                // view headers
                ConsoleRender.FieldsLine(new[] { "Port", "Status" }, 20, new ColorSection(ConsoleColor.DarkGray));
            }

            // view port and his status
            ConsoleRender.FieldsLine(new[] { $"Port:{port}", isAccessed.ToString() }, 20, new ColorSection(foreground: statusColor, text: isAccessed.ToString()));
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
                    ConsoleRender.WriteLine($"Scanner error {exc.Message} ", new ColorSection(ConsoleColor.DarkRed, text: "Scanner error"));                  
                }
                catch (Exception exc)
                {
                    ConsoleRender.WriteLine($"System error {exc.Message} ", new ColorSection(ConsoleColor.DarkRed, text: "System error"));
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

            ConsoleRender.WriteLine(helpString, new ColorSection(foreground: ConsoleColor.DarkGray));
        }

        private static void AboutViewer()
        {
            // author
            ConsoleRender.Field("Author ", width: 15);
            ConsoleRender.WriteLine("github.com/bluesbaker", new ColorSection(foreground: ConsoleColor.Blue, text: "bluesbaker"));

            // copyright
            ConsoleRender.Field("Copyright ", width: 15);
            ConsoleRender.WriteLine("2020");
        }
        #endregion
    }
}
