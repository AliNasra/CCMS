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
    /// Interaction logic for FetchConcreteRecords.xaml
    /// </summary>
    public partial class FetchConcreteRecords : Page
    {
        public FetchConcreteRecordViewModel FetchConcreteVM { get; set; }
        public FetchConcreteRecords()
        {
            InitializeComponent();
            FetchConcreteVM = new FetchConcreteRecordViewModel();
            DataContext = FetchConcreteVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                FetchConcreteVM.FrameWidth = e.NewSize.Width;
                FetchConcreteVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConcreteMenu());
        }
    }
}
