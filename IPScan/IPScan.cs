using IPScan.Scanners;
using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan
{
    public class IPScan
    {
        public IPScan()
        {
            ScannerCollection = new List<IScanner>();
        }

        public void Init(IPScanParameters parameters)
        {
            foreach(var scan in ScannerCollection)
            {
                scan.Init(parameters);
            }
        }

        public async Task<IPInfo> Run()
        {
            IPInfo result = new IPInfo();
            foreach (IScanner scanner in ScannerCollection)
            {
                var scanResult = await scanner.Run();
                result.Merge(scanResult);
            }
            return result;
        }

        public List<IScanner> ScannerCollection { get; set; }

        public event Action<IPInfo> ResultAccepted;
    }
}
