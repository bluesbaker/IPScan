using IPScan.BLL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace IPScan.GUI.Model
{
    public class HostReply
    {
        public PingReply Host { get; set; }
        public ObservableCollection<PortReply> Ports { get; set; } = new ObservableCollection<PortReply>();
    }
}
