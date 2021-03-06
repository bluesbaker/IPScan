﻿using IPScan.BLL;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPScan.GUI.ViewModels.Providers
{
    public interface IPortProvider
    {
        List<int> GetList();
        bool IsValid { get; }
    }
}
