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
using WpfApp2.Utilities;
using WpfApp2.ViewModels;

namespace WpfApp2.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {

        public MainMenuViewModel MainMenuPageVM { get; set; }
        public MainMenu()
        {
            InitializeComponent();
            MainMenuPageVM = new MainMenuViewModel();
            DataContext = MainMenuPageVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                MainMenuPageVM.FrameWidth = e.NewSize.Width;
                MainMenuPageVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void Concrete_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Concrete.ConcreteMenu());
        }

        private void Cement_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Cement.CementMenu());
        }

        private void Fuel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Fuel.FuelMenu());
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Resources.ResourceMenu());
        }

        private void Files_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Document.DocumentMenu());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
   
    }
}
