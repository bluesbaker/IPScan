using IPScan.BLL;
using IPScan.SUP;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static IPScan.TerminalViewers;

namespace IPScan
{
    /// <summary>
    /// User interface of the terminal
    /// </summary>
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
            foreach (var param in parameters)
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
                if (isCheck == false)
                {
                    throw new ScannerException("One or more required parameters is missing");
                }

                // split addresses to the range* collection
                var addresses = commandParameters["-ip"].Split('-');
                var startAddress = IPAddress.Parse(addresses[0]);
                var endAddress = IPAddress.Parse(addresses[addresses.Length - 1]);
                var addressRange = startAddress.Range(endAddress);

                var pingResultCount = 0;

                foreach (var address in addressRange)
                {
                    // copying params with only one address(without range)
                    var parameters = commandParameters.Copy();
                    parameters["-ip"] = address.ToString();

                    var scanner = new Scanner(ScannerParameters.Parse(parameters));

                    // ping request
                    Task<PingReply> pingTask = scanner.GetPingReplyAsync();

                    Console.Title = "Scanning " + address;
                    ColorConsole.Loader("Scanning " + address, (() => !pingTask.IsCompleted));
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
                            Task<PortReply> portTask = scanner.GetPortAccessAsync();

                            ColorConsole.Loader("Scanning port " + scanner.Parameters.Port, (() => !portTask.IsCompleted));
                            portTask.Wait();

                            // view port status
                            PortAccessViewer(portTask.Result);
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
    }
}
