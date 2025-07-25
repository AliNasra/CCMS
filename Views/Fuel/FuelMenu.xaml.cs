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
using WpfApp2.Views.Fuel;

namespace WpfApp2.Views.Fuel
{
    /// <summary>
    /// Interaction logic for FuelMenu.xaml
    /// </summary>
    public partial class FuelMenu : Page
    {
        public FuelMenuViewModel FuelMenuVM { get; set; }
        public FuelMenu()
        {
            InitializeComponent();
            FuelMenuVM = new FuelMenuViewModel();
            DataContext = FuelMenuVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                FuelMenuVM.FrameWidth = e.NewSize.Width;
                FuelMenuVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void AddFuel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddFuelRecord());
        }

        private void DisplayFuel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DisplayFuelRecord());
        }

        private void FetchFuelRecord_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FetchFuelRecord());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenu());
        }
    }
}
