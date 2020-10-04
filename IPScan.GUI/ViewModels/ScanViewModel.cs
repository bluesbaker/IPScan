using IPScan.BLL;
using IPScan.GUI.Models;
using IPScan.GUI.Support;
using IPScan.GUI.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using IPScan.GUI.UserControls;
using MaterialDesignThemes.Wpf;
using IPScan.GUI.Serializers;
using Microsoft.Win32;

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
                // check count of address
                if(AddressProviders.Count == 0)
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


        #region Filters
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


        #region Menu commands
        private RelayCommand _aboutOpenDialogCommand;
        public RelayCommand AboutOpenDialogCommand
        {
            get => _aboutOpenDialogCommand ??= new RelayCommand(async (n) => 
            {
                var view = new SampleDialog
                {
                    DataContext = new AboutDialogViewModel()
                    {
                        Title = "About",
                        Description = "Author: github.com/bluesbaker"
                    }
                };
                await DialogHost.Show(view);
            });
        }
        #endregion


        #region Address provider commands
        private RelayCommand _addAddressProviderCommand;
        public RelayCommand AddAddressProviderCommand
        {
            get => _addAddressProviderCommand ??= new RelayCommand(n =>
            {
                AddressProviders.Add(new SingleAddressProvider() { SingleAddress = "77.88.55.77" });
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
                ScanCommand.Invalidate();
            }, n => (AddressProviders.Count > 1) && !IsScan);
        }

        private RelayCommand _changeAddressProviderCommand;
        public RelayCommand ChangeAddressProviderCommand
        {
            get => _changeAddressProviderCommand ??= new RelayCommand(p =>
            {
                var provider = p as IAddressProvider;
                if (AddressProviders.Contains(provider))
                {
                    var addressList = provider.GetList();
                    var indexProvider = AddressProviders.IndexOf(provider);
                    // range to single
                    if(provider is RangeAddressProvider)
                    {
                        var singleAddressProvider = new SingleAddressProvider()
                        {
                            SingleAddress = addressList[^1].ToString()
                        };
                        AddressProviders.Insert(indexProvider, singleAddressProvider);
                    }
                    // single to range
                    else if(provider is SingleAddressProvider)
                    {
                        var rangeAddressProvider = new RangeAddressProvider()
                        {
                            StartAddress = addressList[0].ToString(),
                            EndAddress = addressList[^1].ToString()
                        };
                        AddressProviders.Insert(indexProvider, rangeAddressProvider);
                    }
                    AddressProviders.Remove(provider);
                }
            });
        }
        #endregion


        #region Port provider commands
        private RelayCommand _addPortProviderCommand;
        public RelayCommand AddPortProviderCommand
        {
            get => _addPortProviderCommand ??= new RelayCommand(n =>
            {
                PortProviders.Add(new SinglePortProvider() { SinglePort = "80" });
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
                ScanCommand.Invalidate();
            }, n => !IsScan);
        }

        private RelayCommand _changePortProviderCommand;
        public RelayCommand ChangePortProviderCommand
        {
            get => _changePortProviderCommand ??= new RelayCommand(p =>
            {
                var provider = p as IPortProvider;
                if (PortProviders.Contains(provider))
                {
                    var portList = provider.GetList();
                    var indexProvider = PortProviders.IndexOf(provider);
                    // range to single
                    if(provider is RangePortProvider)
                    {
                        var singlePortProvider = new SinglePortProvider()
                        {
                            SinglePort = portList[^1].ToString()
                        };
                        PortProviders.Insert(indexProvider, singlePortProvider);
                    }
                    // single to range
                    else if(provider is SinglePortProvider)
                    {
                        var rangePortProvider = new RangePortProvider()
                        {
                            StartPort = portList[0].ToString(),
                            EndPort = portList[^1].ToString()
                        };
                        PortProviders.Insert(indexProvider, rangePortProvider);                       
                    }
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
            get => _exportCommand ??= new RelayCommand(n => 
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Text files|*.txt|XML files|*.xml",
                    FileName = $"ipscan-{DateTime.Now:dd.MM.yyyy-hh.mm.ss}",
                    Title = "Export to..."
                };
                if (dialog.ShowDialog() == true)
                {
                    ICollectionSerializer<HostReply> serializer;
                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            serializer = new TextHostReplySerializer();
                            break;
                        case 2:
                            serializer = new XMLHostReplySerializer();
                            break;
                        default:
                            return;
                    }
                    serializer.Serialize(dialog.FileName, HostReplyCollection);
                }             
            }, n => (!IsScan && (HostReplyCollection.Count > 0)));
        }
        #endregion


        #region Scan methods
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
            ProgressDescription = $"Scanning is started";

            double progressStep = 100 / addresses.Count;

            foreach (var address in addresses)
            {
                if(IsStopScan)
                {
                    break;
                }

                ProgressDescription = $"Scanning {address}";

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
                    Status = pingReply.Status,
                    RoundtripTime = pingReply.RoundtripTime
                };

                HostReplyCollection.Add(host);

                if (pingReply.Status == IPStatus.Success)
                {                 
                    foreach (var port in ports)
                    {
                        if (IsStopScan)
                        {
                            break;
                        }

                        ProgressDescription = $"Scanning {address}:{port}";           

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

            IsScan = false;
            IsStopScan = false;

            // refresh
            InvalidateCommands();
        }

        private void StopScan(object n)
        {
            IsStopScan = true;         
        } 
        
        private void InvalidateCommands()
        {
            RemoveAddressProviderCommand.Invalidate();
            RemovePortProviderCommand.Invalidate();
            ScanCommand.Invalidate();
            StopScanCommand.Invalidate();
            ExportCommand.Invalidate();
        }
        #endregion


        #region Implementation IDataErrorInfo
        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public string Error => string.Join(Environment.NewLine, Errors.Where(pair => pair.Value != null).Select(pair => $"{pair.Key}: \"{pair.Value}\""));

        public string this[string columnName] => Errors.TryGetValue(columnName, out string value) ? value : null;

        private void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = message;
            }
            else
            {
                Errors.Add(propertyName, message);
            }
        }
        #endregion
    }
}
