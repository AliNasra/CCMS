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
using WpfApp2.ViewModels.Concrete;
using WpfApp2.ViewModels.Fuel;
using WpfApp2.Views.Fuel;

namespace WpfApp2.Views.Concrete
{
    /// <summary>
    /// Interaction logic for DisplayConcreteRecord.xaml
    /// </summary>
    public partial class DisplayConcreteRecord : Page
    {
        public DisplayConcreteRecordViewModel DisplayConcreteVM { get; set; }
        public DisplayConcreteRecord()
        {
            InitializeComponent();
            DisplayConcreteVM = new DisplayConcreteRecordViewModel();
            DataContext = DisplayConcreteVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DisplayConcreteVM.FrameWidth = e.NewSize.Width;
                DisplayConcreteVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConcreteMenu());
        }
    }
}
