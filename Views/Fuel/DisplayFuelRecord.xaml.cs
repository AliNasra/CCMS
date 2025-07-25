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
    /// Interaction logic for DisplayFuelRecord.xaml
    /// </summary>
    public partial class DisplayFuelRecord : Page
    {
        public DisplayFuelRecordViewModel DisplayFuelVM { get; set; }
        public DisplayFuelRecord()
        {
            InitializeComponent();
            DisplayFuelVM = new DisplayFuelRecordViewModel();
            DataContext = DisplayFuelVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DisplayFuelVM.FrameWidth = e.NewSize.Width;
                DisplayFuelVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FuelMenu());
        }
    }
}
