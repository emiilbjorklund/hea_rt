using DataVisualizer.Viewmodels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DataVisualizer.Views
{
    /// <summary>
    /// Interaction logic for EcgPlot.xaml
    /// </summary>
    public partial class EcgPlot : UserControl
    {
        public EcgPlot()
        {
            InitializeComponent();
            //Task.Run(UpdatePlot);
        }

        //private async Task UpdatePlot()
        //{
        //    c_plotLaRa.plt.Title("La-Ra");
        //    c_plotLlRa.plt.Title("Ll-Ra");
        //    c_plotLlLa.plt.Title("Ll-La");
        //    c_plotVxRl.plt.Title("Vx-Rl");

        //    while (true)
        //    {
                
        //        try
        //        {
                    
        //            Application.Current.Dispatcher.Invoke(() => {
        //                var vm = DataContext as EcgPlotVM;
        //                if (vm.Buffer.Count > 0)
        //                {
        //                    c_plotLaRa.plt.Clear();
        //                    c_plotLaRa.plt.PlotSignal(vm.Buffer.Select(d => d.La_Ra).ToArray());
        //                    c_plotLaRa.Render();

        //                    c_plotLlRa.plt.Clear();
        //                    c_plotLlRa.plt.PlotSignal(vm.Buffer.Select(d => d.Ll_Ra).ToArray());
        //                    c_plotLlRa.Render();

        //                    c_plotLlLa.plt.Clear();
        //                    c_plotLlLa.plt.PlotSignal(vm.Buffer.Select(d => d.Ll_La).ToArray());
        //                    c_plotLlLa.Render();

        //                    c_plotVxRl.plt.Clear();
        //                    c_plotVxRl.plt.PlotSignal(vm.Buffer.Select(d => d.Vx_Rl).ToArray());
        //                    c_plotVxRl.Render();
        //                }
        //            });
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //        }
                
        //        await Task.Delay(250);
        //    }
        //}
    }
}
