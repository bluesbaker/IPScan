using IPScan.GUI.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPScan.GUI.ViewModels
{
    public class AboutDialogViewModel : NPCBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
    }
}
