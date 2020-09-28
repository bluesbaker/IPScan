using IPScan.BLL;
using IPScan.GUI.Models;
using IPScan.GUI.Support;
using IPScan.GUI.Providers;
using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Data;
using System.Windows.Input;

namespace IPScan.GUI.ViewModels
{
    public class ScanViewModel : NPCBase, IDataErrorInfo
    {
        public ScanViewModel()
        {
            AddressProviders.Add(new SingleAddressProvider() { SingleAddress = "77.88.55.77" });
            PortProviders.Add(new SinglePortProvider() { SinglePort = "80" });
        }

        #region Properties
        private bool _isSucceedAddress = false;
        public bool IsSucceedAddress
        {
            get => _isSucceedAddress;               
            set => Set(ref _isSucceedAddress, value);
        }

        private bool _isOpenedPort = false;
        public bool IsOpenedPort
        {
            get => _isOpenedPort;
            set => Set(ref _isOpenedPort, value);
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

        private bool _isScan = false;
        public bool IsScan
        {
            get => _isScan;
            set => Set(ref _isScan, value);
        }

        private bool _isStopScan = false;
        public bool IsStopScan
        {
            get => _isStopScan;
            set => Set(ref _isStopScan, value);
        }


        public bool IsValid
        {
            get
            {
                // check self errors
                if(Errors.Values.Any(x => x != null))
                {
                    return false;
                }
                // check validate of address providers
                foreach(var provider in AddressProviders)
                {
                    if (!provider.IsValid) return false;
                }
                // check validate of port providers
                foreach (var provider in PortProviders)
                {
                    if (!provider.IsValid) return false;
                }
                return true;
            }
        }

        public ObservableCollection<IAddressProvider> AddressProviders { get; } = new ObservableCollection<IAddressProvider>();
        public ObservableCollection<IPortProvider> PortProviders { get; } = new ObservableCollection<IPortProvider>();

        public ObservableCollection<HostReply> HostReplyCollection { get; } = new ObservableCollection<HostReply>();
        #endregion


        #region Filters of result list
        public Predicate<object> SucceedAddressFilter => (sender) =>
        {
            var hostReply = sender as HostReply;
            return hostReply.Status == IPStatus.Success;
        };
        public Predicate<object> OpenedPortFilter => (sender) =>
        {
            var portReply = sender as PortReply;
            return portReply.Status == PortStatus.Opened;
        };
        #endregion


        #region Address providers commands
        private RelayCommand _addSingleAddressProviderCommand;
        public RelayCommand AddSingleAddressProviderCommand
        {
            get => _addSingleAddressProviderCommand ??= new RelayCommand(n =>
            {
                AddressProviders.Add(new SingleAddressProvider() { SingleAddress = "77.88.55.77" });
            });
        }

        private RelayCommand _addRangeAddressProviderCommand;
        public RelayCommand AddRangeAddressProviderCommand
        {
            get => _addRangeAddressProviderCommand ??= new RelayCommand(n =>
            {
                AddressProviders.Add(new RangeAddressProvider() { StartAddress = "77.88.55.77", EndAddress = "77.88.55.80" });
            });
        }

        private RelayCommand _removeAddressProviderCommand;
        public RelayCommand RemoveAddressProviderCommand
        {
            get => _removeAddressProviderCommand ??= new RelayCommand(p =>
            {
                if (p is IAddressProvider provider)
                {
                    AddressProviders.Remove(provider);
                }
            });
        }
        #endregion


        #region Port providers commands
        private RelayCommand _addSinglePortProviderCommand;
        public RelayCommand AddSinglePortProviderCommand
        {
            get => _addSinglePortProviderCommand ??= new RelayCommand(n =>
            {
                PortProviders.Add(new SinglePortProvider() { SinglePort = "80" });
            });
        }

        private RelayCommand _addRangePortProviderCommand;
        public RelayCommand AddRangePortProviderCommand
        {
            get => _addRangePortProviderCommand ??= new RelayCommand(n =>
            {
                PortProviders.Add(new RangePortProvider() { StartPort = "80", EndPort = "85"});
            });
        }

        private RelayCommand _removePortProviderCommand;
        public RelayCommand RemovePortProviderCommand
        {
            get => _removePortProviderCommand ??= new RelayCommand(p =>
            {
                if (p is IPortProvider provider)
                {
                    PortProviders.Remove(provider);
                }
            });
        }
        #endregion


        #region Scan commands
        private RelayCommand _ScanCommand;
        public RelayCommand ScanCommand
        {
            get => _ScanCommand ??= new RelayCommand(ScanAsync, n => (!IsScan && IsValid));
        }
        
        private RelayCommand _stopScanCommand;
        public RelayCommand StopScanCommand
        {
            get => _stopScanCommand ??= new RelayCommand(StopScan, n => IsScan);
        }

        private RelayCommand _clearListCommand;
        public RelayCommand ClearListCommand
        {
            get => _clearListCommand ??= new RelayCommand(n => HostReplyCollection.Clear(), n => (HostReplyCollection.Count > 0));
        }

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand
        {
            get => _exportCommand ??= new RelayCommand(n => { }, n => (!IsScan && (HostReplyCollection.Count > 0)));
        }
        #endregion


        #region Methods
        private async void ScanAsync(object n)
        {
            List<IPAddress> addresses = new List<IPAddress>();
            foreach(var provider in AddressProviders)
            {
                addresses.AddRange(provider.GetList());
            }

            List<int> ports = new List<int>();
            foreach(var provider in PortProviders)
            {
                ports.AddRange(provider.GetList());
            }

            IsScan = true;
            ProgressDescription = $"Scan is started";

            double progressStep = 100 / addresses.Count;

            foreach (var address in addresses)
            {
                ProgressDescription = $"Scan address \"{address}\"";

                if(IsStopScan)
                {
                    ProgressDescription = "Scan is stopped";
                    break;
                }

                var scannerParameters = new ScannerParameters
                {
                    Address = address
                };
                var scanner = new Scanner(scannerParameters);

                // ping request
                var pingReply = await scanner.GetPingReplyAsync();

                var host = new HostReply
                {
                    Address = address,
                    Status = pingReply.Status
                };

                HostReplyCollection.Add(host);

                if (pingReply.Status == IPStatus.Success)
                {                 
                    foreach (var port in ports)
                    {
                        ProgressDescription = $"Scan port \"{port}\"";

                        if (IsStopScan)
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
            ProgressDescription = "Scan is completed";
            
            IsScan = false;
            IsStopScan = false;

            ScanCommand.Invalidate();
            StopScanCommand.Invalidate();
            ExportCommand.Invalidate();
        }

        private void StopScan(object n)
        {
            IsStopScan = true;
        }

        private void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            if (Errors.ContainsKey(propertyName))
                Errors[propertyName] = message;
            else
                Errors.Add(propertyName, message);
        }
        #endregion


        #region Implementation IDataErrorInfo
        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public string Error => string.Join(Environment.NewLine, Errors.Where(pair => pair.Value != null).Select(pair => $"{pair.Key}: \"{pair.Value}\""));

        public string this[string columnName] => Errors.TryGetValue(columnName, out string value) ? value : null;
        #endregion
    }
}
