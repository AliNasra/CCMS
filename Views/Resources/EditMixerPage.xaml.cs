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
    /// Interaction logic for EditMixerPage.xaml
    /// </summary>
    public partial class EditMixerPage : Page
    {
        public EditMixerViewModel EditMixerVM { get; set; }
        public EditMixerPage()
        {
            InitializeComponent();
            EditMixerVM = new EditMixerViewModel();
            DataContext = EditMixerVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                EditMixerVM.FrameWidth = e.NewSize.Width;
                EditMixerVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MixerService.editMixer(EditMixerVM.SelectedMixerName.Trim(), EditMixerVM.CabbageNo, EditMixerVM.MixerName.Trim(), EditMixerVM.IsOperational, EditMixerVM.OperationalCapacity, EditMixerVM.CurrentCementLevel, EditMixerVM.DepotName);
                MessageBox.Show($"تم تعديل الخلاطة بنجاح", "تنبيه", MessageBoxButton.OK);
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
