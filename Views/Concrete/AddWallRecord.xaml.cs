using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using WpfApp2.Models.Items;
using WpfApp2.Services;
using WpfApp2.ViewModels.Concrete;
using WpfApp2.Views.Concrete;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp2.Views.Concrete
{
    /// <summary>
    /// Interaction logic for AddWallRecord.xaml
    /// </summary>
    public partial class AddWallRecord : Page
    {
        public AddWallRecordViewModel AddWallVM { get; set; }
        public AddWallRecord()
        {
            InitializeComponent();
            AddWallVM = new AddWallRecordViewModel();
            DataContext = AddWallVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddWallVM.FrameWidth = e.NewSize.Width;
                AddWallVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!PreCastWallService.canAddRecords())
                {
                    throw new Exception("لا يمكنك ادخال بيانات التمام اكتر من مرة واحدة فاليوم");
                }
                PreCastWallService.addRecord(AddWallVM.WallRecords.ToList());
                var emptyList = new List<PreCastWallRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(AddWallRecordViewModel.wallRecordsFilePath, json);
                MessageBox.Show($"تم إضافة تمام سور سابق الصب بنجاح", "تنبيه", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(AddWallVM.WallRecords, Formatting.Indented);
            File.WriteAllText(AddWallRecordViewModel.wallRecordsFilePath, json);
            NavigationService?.Navigate(new ConcreteMenu());
        }
    }
}
