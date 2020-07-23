using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan
{
    public class Scanner
    {
        public ScannerParameters Parameters;

        public Scanner(ScannerParameters scannerParameters)
        {
            Parameters = scannerParameters;
        }

        public async Task<PingReply> GetPingReplyAsync(Action<PingReply> action = null)
        {
            var ping = new Ping();
            PingReply reply;

            // request
            try
            {
                reply = await ping.SendPingAsync(Parameters.Address, Parameters.Timeout);
                action?.Invoke(reply);
            }
            catch (Exception exc)
            {
                throw new ScannerException("Ping exception", exc);
            }

            return reply;
        }

        public async Task<bool> GetPortAccessAsync()
        {
            var tcpClient = new TcpClient
            {
                SendTimeout = 5
            };

            try
            {
                await tcpClient.ConnectAsync(Parameters.Address, Parameters.Port);
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception exc)
            {
                throw new ScannerException("Port exception", exc);
            }

            return true;
        }
    }

    public class ScannerException : Exception
    {
        public ScannerException(string message, Exception innerException) : base(message, innerException)
        {
            // your advertisement could be here...
        }
    }
}
