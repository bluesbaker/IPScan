using IPScan.BLL;
using IPScan.GUI.Model;
using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IPScan.GUI
{
    public class MainViewModel : NPCBase
    {
        public string StartAddress { get; set; } = "192.168.88.254";
        public string EndAddress { get; set; } = "192.168.88.255";

        public ObservableCollection<HostReply> HostResults { get; set; } = new ObservableCollection<HostReply>();

        public ICommand ScanningCommand
        {
            get => new RelayCommand(ScanningAsync, n => true);
        }

        private async void ScanningAsync(object n)
        {
            var startAddress = IPAddress.Parse(StartAddress);
            var endAddress = IPAddress.Parse(EndAddress);
            var addressRange = startAddress.Range(endAddress);

            foreach (var address in addressRange)
            {
                var scannerParameters = new ScannerParameters
                {
                    Address = address
                };
                var scanner = new Scanner(scannerParameters);

                // ping request
                var pingReply = await scanner.GetPingReplyAsync();                

                if (pingReply != null)
                {
                    var result = new HostReply
                    {
                        Host = pingReply
                    };

                    HostResults.Add(result);
                }

            }
        }


    }
}
