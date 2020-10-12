using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPScan.GUI.ViewModels
{
    public class AboutDialogViewModel : NPCBase
    {
        private string _title = "IPScan";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _description = 
            "Author: github.com/bluesbaker\n" +
            "Version: 1.0.8.8\n" +
            "Copyright: 2020\n";
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
    }
}
