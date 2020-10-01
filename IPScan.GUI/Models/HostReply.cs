using IPScan.BLL;
using IPScan.GUI.Support;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;

namespace IPScan.GUI.Models
{
    public class HostReply : NPCBase
    {
        private IPAddress _address;
        public IPAddress Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        private IPStatus _status;
        public IPStatus Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private long _roundtripTime;
        public long RoundtripTime
        {
            get => _roundtripTime;
            set => Set(ref _roundtripTime, value);
        }

        public ObservableCollection<PortReply> Ports { get; } = new ObservableCollection<PortReply>();
    }
}
