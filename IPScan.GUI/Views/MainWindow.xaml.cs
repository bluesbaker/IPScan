using IPScan.GUI.ViewModels;
using System.Windows;

namespace IPScan.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScanningViewModel _dataContext;

        public MainWindow()
        {
            _dataContext = new ScanningViewModel();
            this.DataContext = _dataContext;

            InitializeComponent();
        }
    }
}
