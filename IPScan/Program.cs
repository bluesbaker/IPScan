using IPScan.SUP;
using System;
using System.Reflection;

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
            Assembly? assembly = Assembly.GetEntryAssembly();

            if(assembly != null)
            {
                // I'm not sure if this is a good idea
                var appTitle = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
                var appDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
                var appVersion = assembly.GetName().Version?.ToString();

                var appHelp = "Usage:\t-ip 192.168.0.1-192.168.0.255 -p 80-81\nHelp:\t--help\n";

                var commonHeader = $"{appTitle} {appVersion} - {appDescription}";

                // header
                ColorConsole.WriteLine(commonHeader, 
                    new ColorSection(foreground: ConsoleColor.Cyan, section: appTitle),
                    new ColorSection(foreground: ConsoleColor.DarkGray, section: appVersion));
                // splitter
                ColorConsole.WriteLine(new String('-', commonHeader.Length));
                // additional info
                ColorConsole.WriteLine(appHelp, new ColorSection(foreground: ConsoleColor.DarkGray));
            }

        }
    }
}
