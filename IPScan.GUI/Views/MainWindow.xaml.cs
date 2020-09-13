using IPScan.GUI.ViewModels;
using System.Windows;

namespace IPScan.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ScanningViewModel();
            InitializeComponent();
        }
    }
}
