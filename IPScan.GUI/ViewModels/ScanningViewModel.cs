using IPScan.BLL;
using IPScan.GUI.Models;
using IPScan.GUI.Support;
using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace IPScan.GUI.ViewModels
{
    public class ScanningViewModel : NPCBase, IDataErrorInfo
    {
        #region Properties
        private string _startAddress = "77.88.55.77";
        public string StartAddress
        {
            get => _startAddress.ToString();
            set
            {
                if(Set(ref _startAddress, value))
                {
                    SetError(IPAddress.TryParse(value, out _) ? null : $"{value} is not ip address");
                }
            }
        }

        private string _endAddress = "77.88.55.80";
        public string EndAddress
        {
            get => _endAddress;
            set
            {
                if (Set(ref _endAddress, value))
                {
                    SetError(IPAddress.TryParse(value, out _) ? null : $"{value} is not ip address");
                }
            }
        }

        private string _startPort = "80";
        public string StartPort
        {
            get => _startPort;
            set
            {
                if (Set(ref _startPort, value))
                {
                    SetError(Int32.TryParse(value, out _) ? null : $"{value} is not a port numeric");
                }
            }
        }

        private string _endPort = "80";
        public string EndPort
        {
            get => _endPort;
            set
            {
                if (Set(ref _endPort, value))
                {
                    SetError(Int32.TryParse(value, out _) ? null : $"{value} is not a port numeric");
                }
            }
        }

        private float _progressValue = 0.0f;
        public float ProgressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            set => Set(ref _isScanning, value);
        }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public bool IsValid => Errors.Values.All(x => x != null);

        public ObservableCollection<HostReply> HostResults { get; } = new ObservableCollection<HostReply>();
        #endregion


        #region Commands
        private RelayCommand _scanningCommand;
        public RelayCommand ScanningCommand
        {
            get => _scanningCommand ??=  new RelayCommand(ScanningAsync, n => !IsScanning);
        }

        private RelayCommand _stopScanning;
        public RelayCommand StopScanningCommand
        {
            get => _stopScanning ??= new RelayCommand(StopScanningAsync, n => IsScanning);
        }

        private RelayCommand _clearListCommand;
        public RelayCommand ClearListCommand
        {
            get => _clearListCommand ??= new RelayCommand(n => HostResults.Clear(), n => HostResults.Count > 0);
        }
        #endregion


        #region Methods
        private async void ScanningAsync(object n)
        {
            IsScanning = true;

            var startAddress = IPAddress.Parse(StartAddress);
            var endAddress = IPAddress.Parse(EndAddress);
            var addressRange = startAddress.Range(endAddress);

            var startPort = Int32.Parse(StartPort);
            var endPort = Int32.Parse(EndPort);
            var portRange = Enumerable.Range(startPort, endPort - startPort + 1).ToList();

            float progressStep = 100 / addressRange.Count;

            foreach (var address in addressRange)
            {
                var scannerParameters = new ScannerParameters
                {
                    Address = address
                };
                var scanner = new Scanner(scannerParameters);

                // ping request
                var pingReply = await scanner.GetPingReplyAsync();

                if (pingReply.Status == IPStatus.Success)
                {                 
                    var host = new HostReply
                    {
                        Host = pingReply
                    };

                    HostResults.Add(host);

                    foreach (var port in portRange)
                    {
                        if (port > 0)
                        {
                            scanner.Parameters.Port = port;                           
                            var portReply = await scanner.GetPortReplyAsync();

                            host.Ports.Add(portReply);
                        }
                    }
                }

                ProgressValue += progressStep;
            }

            ProgressValue = 0.0f;
            
            IsScanning = false;
            ScanningCommand.Invalidate();
            StopScanningCommand.Invalidate();
        }

        private async void StopScanningAsync(object n)
        {
            // TODO: Add the cancel token in scanning 
        }

        private void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            if (Errors.ContainsKey(propertyName))
                Errors[propertyName] = message;
            else
                Errors.Add(propertyName, message);
        }
        #endregion


        #region IDataErrorInFo
        public string Error => string.Join(Environment.NewLine, Errors.Where(pair => pair.Value != null).Select(pair => $"{pair.Key}: \"{pair.Value}\""));

        public string this[string columnName] => Errors.TryGetValue(columnName, out string value) ? value : null;
        #endregion
    }
}
