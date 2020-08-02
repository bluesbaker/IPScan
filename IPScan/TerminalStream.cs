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

            var statusColor = 
                pingReply.Status == IPStatus.Success ? ConsoleColor.Green : ConsoleColor.White;

            if (hasHeaders)
            {
                // view headers
                ConsoleRender.Field("Address", ConsoleColor.DarkBlue, fieldWidth: 20);
                ConsoleRender.Field("Status", ConsoleColor.DarkBlue, fieldWidth: 20);
                ConsoleRender.Field("Roundtrip time", ConsoleColor.DarkBlue, fieldWidth: 20);

                Console.WriteLine();
            }

            // view address, status and ping*
            ConsoleRender.Field(address, fieldWidth: 20);
            ConsoleRender.Field(status, fgColor: statusColor, fieldWidth: 20);
            ConsoleRender.Field(roundtripTime, fieldWidth: 20);

            Console.WriteLine();
        }

        private static void PortAccessViewer(int port, bool isAccessed, bool hasHeaders = false)
        {
            var statusColor =
                isAccessed == true ? ConsoleColor.Green : ConsoleColor.Red;

            if (hasHeaders)
            {
                // view headers
                ConsoleRender.Field("Port", bgColor: ConsoleColor.DarkGray, fieldWidth: 20);
                ConsoleRender.Field("Status", bgColor: ConsoleColor.DarkGray, fieldWidth: 20);

                Console.WriteLine();
            }

            // view port and his status
            ConsoleRender.Field($"Port:", fgColor: ConsoleColor.DarkGray, fieldWidth: 5);
            ConsoleRender.Field(port.ToString(), fieldWidth: 15);
            ConsoleRender.Field(isAccessed.ToString(), fgColor: statusColor, fieldWidth: 20);

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
                    ConsoleRender.Field("Scanner error", bgColor: ConsoleColor.DarkRed);
                    Console.WriteLine($" {exc.Message} ");
                }
                catch (Exception exc)
                {
                    ConsoleRender.Field("System error", bgColor: ConsoleColor.DarkRed);
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

            ConsoleRender.Field(helpString, fgColor: ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        private static void AboutViewer()
        {
            // author
            ConsoleRender.Field("Author ", fieldWidth: 15);
            ConsoleRender.Field("github.com/");
            ConsoleRender.Field("bluesbaker\n", fgColor: ConsoleColor.Blue);

            // copyright
            ConsoleRender.Field("Copyright ", fieldWidth: 15);
            ConsoleRender.Field("2020\n");
        }
        #endregion
    }
}
