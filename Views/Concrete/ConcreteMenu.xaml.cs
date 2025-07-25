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
using WpfApp2.ViewModels.Cement;
using WpfApp2.ViewModels.Concrete;
using WpfApp2.Views.Cement;

namespace WpfApp2.Views.Concrete
{
    /// <summary>
    /// Interaction logic for ConcreteMenu.xaml
    /// </summary>
    public partial class ConcreteMenu : Page
    {
        public ConcreteMenuViewModel ConcreteMenuVM { get; set; }
        public ConcreteMenu()
        {
            InitializeComponent();
            ConcreteMenuVM = new ConcreteMenuViewModel();
            DataContext = ConcreteMenuVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                ConcreteMenuVM.FrameWidth = e.NewSize.Width;
                ConcreteMenuVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void AddConcrete_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddConcreteRecord());
        }

        private void AddWallRecord_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddWallRecord());
        }

        private void DisplayConcrete_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DisplayConcreteRecord());
        }

        private void FetchConcreteRecord(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FetchConcreteRecords());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenu());
        }
    }
}
