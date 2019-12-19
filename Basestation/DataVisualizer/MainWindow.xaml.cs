using Basestation.Common.Abstractions;
using DataVisualizer.Viewmodels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int i = 0;
        public MainWindow()
        {
            InitializeComponent();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            App.PlotContent = c_plotContent;

            var filediag = new OpenFileDialog();
            filediag.Filter = "Yml files (*.yml)|*.yml";

            if (filediag.ShowDialog() == true)
            {
                var structure = new SystemStructure(filediag.FileName);
                var vm = new MainVM(structure);
                DataContext = vm;
            }
            else
            {
                Application.Current.Shutdown();
            }
            
        }

        private async void ChangeMargin()
        {
            while (true)
            {
                await Task.Delay(100);
                Application.Current.Dispatcher.Invoke(() => c_maingrid.Margin = new Thickness(i++));
            }
        }
    }
}
