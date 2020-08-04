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
            Console.Title = "IPScan";
            StartupViewer();

            var terminalParameters = TerminalParameters.Parse(args, "-ip");
            TerminalStream.Run(terminalParameters);
        }
        
        static void StartupViewer()
        {
            var appTitle = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            var appDescription = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            var appInfo = "Usage:\t-ip 192.168.0.1-192.168.0.255 -p 80\nHelp:\t--help\n";

            var commonHeader = $"{appTitle} - {appDescription}";

            // header
            ColorConsole.WriteLine(commonHeader, new ColorSection(foreground: ConsoleColor.Cyan, section: appTitle));
            // splitter
            ColorConsole.WriteLine(new String('-', commonHeader.Length));
            // additional info
            ColorConsole.WriteLine(appInfo, new ColorSection(foreground: ConsoleColor.DarkGray));
        }
    }
}
