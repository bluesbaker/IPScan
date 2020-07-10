using IPScan.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPScan.Scanners
{
    public interface IScanner
    {
        void Init(IPScanParameters parameters);
        string Help(string key = "");
        ScannerKeyAttribute GetKeyAttribute(string key);
        IEnumerable<ScannerKeyAttribute> GetKeyAttributes();
        Task<IPInfo> Run();
    }
}
