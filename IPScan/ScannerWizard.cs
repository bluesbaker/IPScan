﻿using IPScan.Scanners;
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
    public class ScannerWizard
    {
        public ScannerWizard()
        {
            ScanCollection = new List<IScanner>();
        }

        public void Init(Parameters parameters)
        {
            Parameters = parameters;
        }

        public async Task<IPInfo> Run()
        { 
            IPInfo result = new IPInfo();
            foreach (IScanner scan in ScanCollection)
            {
                scan.Init(Parameters);
                result = await scan.Run();
                Thread.Sleep(1000);
                //Console.WriteLine("\n" + result["Status"]);                
            }
            return result;
        }

        public List<IScanner> ScanCollection { get; set; }
        public Parameters Parameters { get; private set; }
    }
}
