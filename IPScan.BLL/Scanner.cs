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

        public async Task<PingReply> GetPingReplyAsync(Action<PingReply> callbackAction = null)
        {
            var ping = new Ping();
            PingReply reply;

            // request
            try
            {
                reply = await ping.SendPingAsync(Parameters.Address, Parameters.Timeout);                               
            }
            catch (Exception exc)
            {
                throw new ScannerException("Ping exception", exc);
            }

            callbackAction?.Invoke(reply);
            return reply;
        }

        public async Task<PortReply> GetPortReplyAsync(Action<PortReply> callbackAction = null)
        {
            var reply = new PortReply { Port = Parameters.Port };
            var tcpCLient = new TcpClient();
            var ar = tcpCLient.BeginConnect(Parameters.Address, Parameters.Port, null, null);

            await Task.Factory.StartNew(() =>
            {
                var wh = ar.AsyncWaitHandle;

                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                    {
                        reply.Status = PortStatus.Closed;
                        tcpCLient.Close();                      
                    }
                    else
                    {
                        reply.Status = PortStatus.Opened;
                        tcpCLient.EndConnect(ar);
                    }
                }
                finally
                {
                    wh.Close();
                }
            });

            callbackAction?.Invoke(reply);
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
