namespace IPScan.BLL
{
    public class PortReply
    {
        public int Port { get; set; }
        public PortStatus Status { get; set; }
    }

    public enum PortStatus
    {
        Unknown = -1,
        Closed = 0,
        Opened = 1
    }
}
