using Newtonsoft.Json;
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
using WpfApp2.ViewModels.Cement;
using WpfApp2.ViewModels.Resources;
using WpfApp2.Views.Fuel;
using WpfApp2.Views.Resources;
using Newtonsoft.Json;
using System.IO;
using WpfApp2.Models.Items;
using WpfApp2.ViewModels.Concrete;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp2.Views.Cement
{
    /// <summary>
    /// Interaction logic for AddCementRecord.xaml
    /// </summary>
    public partial class AddCementRecord : Page
    {
        public AddCementRecordViewModel AddCementVM { get; set; }
        public AddCementRecord()
        {
            InitializeComponent();
            AddCementVM = new AddCementRecordViewModel();
            DataContext = AddCementVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddCementVM.FrameWidth = e.NewSize.Width;
                AddCementVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CementService.canAddRecords())
                {
                    throw new Exception("لا يمكنك ادخال بيانات التمام اكتر من مرة واحدة فاليوم");
                }
                CementService.addCementRecords(AddCementVM.CementRecords.ToList());
                var emptyList = new List<CementRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(AddCementRecordViewModel.cementRecordsFilePath, json);
                MessageBox.Show($"تم إضافة تمام الأسمنت بنجاح", "تنبيه", MessageBoxButton.OK);
                NavigationService?.Navigate(new CementMenu());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(AddCementVM.CementRecords, Formatting.Indented);
            File.WriteAllText(AddCementRecordViewModel.cementRecordsFilePath, json);
            NavigationService?.Navigate(new CementMenu());
        }
    }
}
