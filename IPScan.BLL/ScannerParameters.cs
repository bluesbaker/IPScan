using IPScan.SUP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;

namespace IPScan.BLL
{
    /// <summary>
    /// Parameters of Scanner
    /// </summary>
    public class ScannerParameters
    {
        // required
        public IPAddress Address { get; set; }

        // default
        public int Timeout { get; set; } = 1000;
        public int Port { get; set; } = 0;
    }
}
