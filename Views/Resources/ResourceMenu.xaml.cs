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
using WpfApp2.ViewModels.Resources;

namespace WpfApp2.Views.Resources
{
    /// <summary>
    /// Interaction logic for ResourceMenu.xaml
    /// </summary>
    public partial class ResourceMenu : Page
    {
        public ResourceMenuViewModel ResourceMenuVM { get; set; }
        public ResourceMenu()
        {
            InitializeComponent();
            ResourceMenuVM = new ResourceMenuViewModel();
            DataContext = ResourceMenuVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                ResourceMenuVM.FrameWidth = e.NewSize.Width;
                ResourceMenuVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void AddUnit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUnitPage());
        }

        private void AddMixer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddMixerPage());
        }

        private void AddDepot_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddDepotPage());
        }

        private void EditUnit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EditUnitPage());
        }

        private void EditMixer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EditMixerPage());
        }

        private void EditDepot_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EditDepotPage());
        }

        private void AdditionalData_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdditionalDataPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenu());
        }
    }
}
