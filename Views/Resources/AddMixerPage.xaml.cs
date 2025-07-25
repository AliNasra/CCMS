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
    /// Interaction logic for AddMixerPage.xaml
    /// </summary>
    public partial class AddMixerPage : Page
    {
        public AddMixerViewModel AddMixerVM { get; set; }
        public AddMixerPage()
        {
            InitializeComponent();
            AddMixerVM = new AddMixerViewModel();
            DataContext = AddMixerVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddMixerVM.FrameWidth = e.NewSize.Width;
                AddMixerVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MixerService.addMixer(AddMixerVM.CabbageNo, AddMixerVM.MixerName, AddMixerVM.IsOperational, AddMixerVM.OperationalCapacity, AddMixerVM.CurrentCementLevel, AddMixerVM.DepotName);
                MessageBox.Show($"تم إضافة الخلاطة بنجاح", "تنبيه", MessageBoxButton.OK);
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
