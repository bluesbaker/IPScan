using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IPScan.GUI.ViewModels.Providers
{
    public interface IAddressProvider
    {
        List<IPAddress> GetList();
        bool IsValid { get; }
    }
}
