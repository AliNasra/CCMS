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
using WpfApp2.Services;
using WpfApp2.ViewModels.Resources;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp2.Views.Resources
{
    /// <summary>
    /// Interaction logic for AddDepotPage.xaml
    /// </summary>
    public partial class AddDepotPage : Page
    {
        public AddDepotViewModel AddDepotVM { get; set; }
        public AddDepotPage()
        {
            InitializeComponent();
            AddDepotVM = new AddDepotViewModel();
            DataContext = AddDepotVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddDepotVM.FrameWidth = e.NewSize.Width;
                AddDepotVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DepotService.addDepot(AddDepotVM.SelectedUnit, AddDepotVM.DepotStorageCapacity, AddDepotVM.DepotName, AddDepotVM.CurrentReserve,AddDepotVM.LastImportedFuelAmount,AddDepotVM.LastConsignmentDate);
                MessageBox.Show($"تم إضافة مستودع الوقود بنجاح", "تنبيه", MessageBoxButton.OK);
                NavigationService?.Navigate(new ResourceMenu());

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ResourceMenu());
        }

    }
}
