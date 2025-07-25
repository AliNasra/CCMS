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
    /// Interaction logic for EditUnitPage.xaml
    /// </summary>
    public partial class EditUnitPage : Page
    {
        public EditUnitViewModel EditUnitVM { get; set; }
        public EditUnitPage()
        {
            InitializeComponent();
            EditUnitVM = new EditUnitViewModel();
            DataContext = EditUnitVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                EditUnitVM.FrameWidth = e.NewSize.Width;
                EditUnitVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UnitService.editUnit(EditUnitVM.unitName, EditUnitVM.SelectedDesignation, EditUnitVM.SelectedSpecialization, EditUnitVM.PrecastWallTarget, EditUnitVM.UnitCode, EditUnitVM.SelfSufficienyReserve, EditUnitVM.SelectedOperationality,EditUnitVM.Benzine80Reserve, EditUnitVM.SummerDieselReserve);
                MessageBox.Show($"تم تعديل بيانات  {EditUnitVM.unitName} بنجاح", "تنبيه", MessageBoxButton.OK);
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
