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
    /// Interaction logic for DisplayCementRecord.xaml
    /// </summary>
    public partial class DisplayCementRecord : Page
    {
        public DisplayCementRecordViewModel DisplayCementVM { get; set; }
        public DisplayCementRecord()
        {
            InitializeComponent();
            DisplayCementVM = new DisplayCementRecordViewModel();
            DataContext = DisplayCementVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DisplayCementVM.FrameWidth = e.NewSize.Width;
                DisplayCementVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CementMenu());
        }
    }
}
