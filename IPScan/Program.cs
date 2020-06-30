using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IPScan
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(welcomeMessage);

            string[] tmpArg = { "ipscan", "77.88.55.27", "-d" };

            //var paramList = ArgumentsConverter(tmpArg);

            //string[] ipRange = paramList["-ip"].Split('-');

            //IPAddress ip = IPAddress.Parse(paramList["-ip"]);

            //var tttt = tmpArg.Where(a => a != tmpArg[0]).ToDictionary((a, k) => 
            //{
            //    k = a.ToUpper();
            //});
            //var tttt = ArgumentsConverter(tmpArg);

            string ipsString = "192.168.88.0-192.168.88.10-192.168.88.15";
            IPAddress[] ips = ipsString
                .Split('-')
                .Select(ip => IPAddress.Parse(ip))
                .ToArray();

            var range = ScannerModifier.GetAddressRange(ips[0], ips[ips.Length-1]);

            int i = 190;


            //ScannerModifier argReader = new ScannerModifier();
            //var result = argReader.GetKey("--help");



            //IPHostEntry host = Dns.GetHostEntry(ip);
            //var param = tmpArg.ToDictionary()

            //Console.Write("> --help\n");
            //Console.WriteLine(argReader.Help());


            //var ranger = ScannerModifier.GetAddressRange(IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.8.8"));

            //var request = String.Empty;
            //while (request.Trim().ToLower() != "exit")
            //{
            //    Console.Write("> ");
            //    request = Console.ReadLine();
            //    string response = new ScannerModifier().Scan(request);
            //    Console.WriteLine(response);
            //}
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }

        #region Private
        static AssemblyName APP => Assembly.GetExecutingAssembly().GetName();
        static readonly string welcomeMessage =
            $"ipscan – scanning ip-address\n______\n" +
            "usage: 192.168.0.1-192.168.0.255\n" +
            " help: --help\n";
        #endregion

        /// <summary>
        /// Convert args[] with keys to Dictionary
        /// </summary>
        /// <param name="args">args array</param>
        /// <param name="defaultKey">default first key</param>
        /// <param name="defaultValue">default value of arg</param>
        /// <returns>arguments collection</returns>
        public static Dictionary<string, string> ArgumentsConverter(string[] args, string defaultKey = "", string defaultValue = "true")
        {
            var argCollection = new Dictionary<string, string>();
            string lastKey = defaultKey;

            foreach (string arg in args)
            {
                // arg is key
                if (arg[0] == '-')
                {
                    lastKey = arg;
                    argCollection[lastKey] = defaultValue;  // set default value
                }
                // or value
                else if (lastKey != String.Empty)
                {
                    argCollection[lastKey] = arg;
                }
            }

            return argCollection;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(string key, string description = "")
        {
            Key = key;
            Description = description;
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
    }


    public class ScannerModifier
    {       
        public ScannerModifier(Dictionary<string,string> parameters)
        {
            foreach (var methodInfo in GetType().GetMethods())
            {
                var argKey = methodInfo.GetCustomAttribute<KeyAttribute>();

                if (argKey?.Key == parameters[argKey.Key])
                {
                    //helpInfo += $"\t{argKey.Key}\t- {argKey.Description}\n";
                }
            }
        }

        #region Properties
        private readonly IPAddress start, end;
        #endregion

        #region Commands
        /// <summary>
        /// Help information
        /// </summary>
        /// <returns>format! string</returns>
        [Key("--help", "help information and boobs")]
        public virtual string Help()
        {
            string helpInfo = "\n";

            foreach (var methodInfo in GetType().GetMethods())
            {
                var argKey = methodInfo.GetCustomAttribute<KeyAttribute>();
                if (argKey != null)
                {
                    helpInfo += $"\t{argKey.Key}\t- {argKey.Description}\n";
                }
            }

            return helpInfo;
        }

        /// <summary>
        /// Scan modifier
        /// </summary>
        /// <param name="ips">once ip(0.0.0.0) or rangev(0.0.0.0-1.1.1.1)</param>
        /// <returns></returns>
        [Key("-ip", "address, range of addresses or range of mask")]
        public virtual List<IPAddress> SetIP(this IPAddress ipAddress, string ips)
        {
            List<IPAddress> resultList = new List<IPAddress>();
            
            
        
            // start scan
            foreach(IPAddress ip in GetAddressRange(from, to))
            {                
                Ping ping = new Ping();
                PingReply reply = ping.Send(ip, 1000);
                resultList += $"{reply.Address}\t\t|{reply.Status}\n";
            }
        
            return resultList;
        }

        //public virtual string View(string option)
        //{
        //
        //}

        #endregion

        #region Private methods
        static IPAddress ParseToIPList(string ip)
        {
            IPAddress[] ips = ipsString
                .Split('-')
                .Select(ip => IPAddress.Parse(ip))
                .ToArray();

            return IPAddress.Parse(ipString);
        }

        public static List<IPAddress> GetAddressRange(IPAddress from, IPAddress to)
        {
            var result = new List<IPAddress>();

            // convert IPAdresses to bytes
            var fromAsBytes = from.GetAddressBytes();
            var toAsBytes = to.GetAddressBytes();

            // reverse the from-to bytes 
            // for a correct calculate of range
            Array.Reverse(fromAsBytes);
            Array.Reverse(toAsBytes);

            // convert bytes to int
            var fromAsInt = BitConverter.ToInt32(fromAsBytes, 0);
            var toAsInt = BitConverter.ToInt32(toAsBytes, 0);

            for(int ipAsInt = fromAsInt; ipAsInt <= toAsInt; ipAsInt++)
            {
                var ipAsBytes = BitConverter.GetBytes(ipAsInt);
                // more reverse... more!
                Array.Reverse(ipAsBytes);
                var ip = new IPAddress(ipAsBytes);
                result.Add(ip);
            }

            return result;
        }
        #endregion
    }


}
