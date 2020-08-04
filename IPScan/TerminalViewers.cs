using IPScan.BLL;
using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    /// <summary>
    /// Viewers of TerminalStream
    /// </summary>
    public static class TerminalViewers
    {
        public static void PingReplyViewer(PingReply pingReply, bool hasHeaders = false)
        {
            // data
            var address = pingReply.Address.ToString();
            var status = pingReply.Status.ToString();
            var roundtripTime = pingReply.RoundtripTime.ToString();

            var statusColor = pingReply.Status == IPStatus.Success ? ConsoleColor.Green : ConsoleColor.White;

            if (hasHeaders)
            {
                // view headers
                ColorConsole.FieldsLine(new[] { "Address", "Status", "Roundtrip time" }, 20, new ColorSection(ConsoleColor.DarkBlue));
            }

            // view address, status and ping*
            ColorConsole.FieldsLine(new[] { address, status, roundtripTime }, 20, new ColorSection(foreground: statusColor, section: status));
        }

        public static void PortAccessViewer(int port, bool isAccessed, bool hasHeaders = false)
        {
            var statusColor = isAccessed == true ? ConsoleColor.Green : ConsoleColor.Red;

            if (hasHeaders)
            {
                // view headers
                ColorConsole.FieldsLine(new[] { "Port", "Status" }, 20, new ColorSection(ConsoleColor.DarkGray));
            }

            // view port and his status
            ColorConsole.FieldsLine(new[] { $"Port:{port}", isAccessed.ToString() }, 20, new ColorSection(foreground: statusColor, section: isAccessed.ToString()));
        }

        public static void ErrorViewer(params Exception[] exceptions)
        {
            foreach (var exception in exceptions)
            {
                try
                {
                    throw exception;
                }
                catch (ScannerException exc)
                {
                    ColorConsole.WriteLine($"Scanner error {exc.Message} ", new ColorSection(ConsoleColor.DarkRed, section: "Scanner error"));
                }
                catch (Exception exc)
                {
                    ColorConsole.WriteLine($"System error {exc.Message} ", new ColorSection(ConsoleColor.DarkRed, section: "System error"));
                }
                finally
                {
                    HelpViewer();
                }
            }
        }

        public static void HelpViewer()
        {
            string helpString = "IPScan:\n";

            foreach (var setter in ScannerParameters.GetKeySetters())
            {
                helpString += $"{setter.Key}\t\t- {setter.Description}\n";
            }

            helpString +=
                "--help\t\t- FAQ\n" +
                "--about\t\t- About\n" +
                "--quit\t\t- Quit\n";

            ColorConsole.WriteLine(helpString, new ColorSection(foreground: ConsoleColor.DarkGray));
        }

        public static void AboutViewer()
        {
            // author
            ColorConsole.Field("Author ", width: 15);
            ColorConsole.WriteLine("github.com/bluesbaker", new ColorSection(foreground: ConsoleColor.Blue, section: "bluesbaker"));

            // copyright
            ColorConsole.Field("Copyright ", width: 15);
            ColorConsole.WriteLine("2020");
        }
    }
}
