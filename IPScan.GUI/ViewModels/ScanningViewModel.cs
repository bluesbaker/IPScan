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
using System.Text;
using System.Threading.Tasks;
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
                _startAddress = value;
                OnPropertyChanged();

                // validation
                if (IPAddress.TryParse(value, out _) == false)
                {
                    _errors["StartAddress"] = $"{value} is not ip address";
                }
                else
                {
                    _errors["StartAddress"] = null;                    
                }
            }
        }

        private string _endAddress = "77.88.55.90";
        public string EndAddress
        {
            get => _endAddress;
            set
            {
                _endAddress = value;
                OnPropertyChanged();

                // validation
                if (IPAddress.TryParse(value, out _) == false)
                {
                    _errors["EndAddress"] = $"{value} is not ip address";
                }
                else
                {
                    _errors["EndAddress"] = null;
                }
            }
        }

        private string _startPort = "0";
        public string StartPort
        {
            get => _startPort;
            set
            {
                _startPort = value;
                OnPropertyChanged();

                // validation
                if (Int32.TryParse(value, out _) == false)
                {
                    _errors["StartPort"] = $"{value} is not a port numeric";
                }
                else
                {
                    _errors["StartPort"] = null;
                }
            }
        }

        private string _endPort = "0";
        public string EndPort
        {
            get => _endPort;
            set
            {
                _endPort = value;
                OnPropertyChanged();

                // validation
                if (Int32.TryParse(value, out _) == false)
                {
                    _errors["EndPort"] = $"{value} is not a port numeric";
                }
                else
                {
                    _errors["EndPort"] = null;
                }
            }
        }

        private float _progressValue = 0.0f;
        public float ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                _isScanning = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, string> _errors = new Dictionary<string, string>();
        public Dictionary<string, string> Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid => !Errors.Values.Any(x => x != null);

        public ObservableCollection<HostReply> HostResults { get; set; } = new ObservableCollection<HostReply>();
        #endregion


        #region Commands
        public ICommand ScanningCommand
        {
            get => new RelayCommand(ScanningAsync, n => IsValid && !IsScanning);
        }

        public ICommand StopScanningCommand
        {
            get => new RelayCommand(StopScanningAsync, n => IsScanning);
        }

        public ICommand ClearListCommand
        {
            get => new RelayCommand(n => HostResults.Clear(), n => HostResults.Count > 0);
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
                    ObservableCollection<PortReply> portList = new ObservableCollection<PortReply>();                   

                    var host = new HostReply
                    {
                        Host = pingReply,
                        Ports = portList
                    };

                    HostResults.Add(host);

                    foreach (var port in portRange)
                    {
                        if (port > 0)
                        {
                            scanner.Parameters.Port = port;                           
                            var portReply = await scanner.GetPortReplyAsync();

                            portList.Add(portReply);
                        }
                    }
                }

                ProgressValue += progressStep;
            }

            ProgressValue = 0.0f;
            IsScanning = false;
        }

        private async void StopScanningAsync(object n)
        {

        }
        #endregion


        #region IDataErrorInFo
        public string Error => throw new Exception("Scanning error");

        public string this[string columnName] => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
        #endregion
    }
}
