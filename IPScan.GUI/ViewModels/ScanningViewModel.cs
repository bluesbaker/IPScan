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

        private double _progressValue = 0.0f;
        public double ProgressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private string _progressDescription = String.Empty;
        public string ProgressDescription
        {
            get => _progressDescription;
            set => Set(ref _progressDescription, value);
        }

        private bool _isScanning = false;
        public bool IsScanning
        {
            get => _isScanning;
            set => Set(ref _isScanning, value);
        }

        private bool _isStopScanning = false;
        public bool IsStopScanning
        {
            get => _isStopScanning;
            set => Set(ref _isStopScanning, value);
        }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public bool IsValid => !Errors.Values.Any(x => x != null);

        public ObservableCollection<HostReply> HostResults { get; } = new ObservableCollection<HostReply>();
        #endregion


        #region Commands
        private RelayCommand _scanningCommand;
        public RelayCommand ScanningCommand
        {
            get => _scanningCommand ??= new RelayCommand(ScanningAsync, n => (!IsScanning && IsValid));
        }
        
        private RelayCommand _stopScanningCommand;
        public RelayCommand StopScanningCommand
        {
            get => _stopScanningCommand ??= new RelayCommand(StopScanning, n => IsScanning);
        }

        private RelayCommand _clearListCommand;
        public RelayCommand ClearListCommand
        {
            get => _clearListCommand ??= new RelayCommand(n => HostResults.Clear(), n => (HostResults.Count > 0));
        }

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand
        {
            get => _exportCommand ??= new RelayCommand(n => { }, n => (!IsScanning && (HostResults.Count > 0)));
        }
        #endregion


        #region Methods
        private async void ScanningAsync(object n)
        {
            IsScanning = true;
            ProgressDescription = $"Scanning is started";

            var startAddress = IPAddress.Parse(StartAddress);
            var endAddress = IPAddress.Parse(EndAddress);
            var addressRange = startAddress.Range(endAddress);

            var startPort = Int32.Parse(StartPort);
            var endPort = Int32.Parse(EndPort);
            var portRange = Enumerable.Range(startPort, endPort - startPort + 1).ToList();

            double progressStep = 100 / addressRange.Count;

            foreach (var address in addressRange)
            {
                ProgressDescription = $"Scanning address \"{address}\"";

                if(IsStopScanning)
                {
                    ProgressDescription = "Scanning is stopped";
                    break;
                }

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
                        ProgressDescription = $"Scanning port \"{port}\"";

                        if (IsStopScanning)
                        {
                            break;
                        }

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
            ProgressDescription = "Scanning is completed";
            
            IsScanning = false;
            IsStopScanning = false;

            ScanningCommand.Invalidate();
            StopScanningCommand.Invalidate();
            ExportCommand.Invalidate();
        }

        private void StopScanning(object n)
        {
            IsStopScanning = true;
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
