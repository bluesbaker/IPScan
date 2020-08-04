using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IPScan.BLL
{
    /// <summary>
    /// Scanning ip-address
    /// </summary>
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

        public async Task<PortReply> GetPortAccessAsync()
        {
            PortReply reply = new PortReply { Port = Parameters.Port };

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
                reply.Status = PortStatus.Closed;
                return reply;
            }
            catch (Exception exc)
            {
                throw new ScannerException("Port exception", exc);
            }

            reply.Status = PortStatus.Opened;
            return reply;
        }
    }

    public class ScannerException : Exception
    {
        public ScannerException(string message, Exception innerException = null) : base(message, innerException)
        {
            // your advertisement could be here...
        }
    }
}
