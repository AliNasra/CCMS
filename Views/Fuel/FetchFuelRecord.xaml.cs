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
using WpfApp2.ViewModels.Fuel;

namespace WpfApp2.Views.Fuel
{
    /// <summary>
    /// Interaction logic for FetchFuelRecord.xaml
    /// </summary>
    public partial class FetchFuelRecord : Page
    {
        public FetchFuelRecordViewModel FetchFuelVM { get; set; }
        public FetchFuelRecord()
        {
            InitializeComponent();
            FetchFuelVM = new FetchFuelRecordViewModel();
            DataContext = FetchFuelVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                FetchFuelVM.FrameWidth = e.NewSize.Width;
                FetchFuelVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FuelMenu());
        }
    }
}
