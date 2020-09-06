using IPScan.BLL;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;

namespace IPScan.GUI.Models
{
    public class HostReply
    {
        public PingReply Host { get; set; }
        public ObservableCollection<PortReply> Ports { get; set; } = new ObservableCollection<PortReply>();
    }
}
