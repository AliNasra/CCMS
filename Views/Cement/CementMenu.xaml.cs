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

namespace WpfApp2.Views.Cement
{
    /// <summary>
    /// Interaction logic for CementMenu.xaml
    /// </summary>
    public partial class CementMenu : Page
    {
        public CementMenuViewModel CementMenuVM { get; set; }
        public CementMenu()
        {
            InitializeComponent();
            CementMenuVM = new CementMenuViewModel();
            DataContext = CementMenuVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                CementMenuVM.FrameWidth = e.NewSize.Width;
                CementMenuVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void AddCement_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddCementRecord());
        }

        private void DisplayCement_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DisplayCementRecord());
        }

        private void FetchCementRecord(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FetchCementRecord());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenu());
        }
    }
}
