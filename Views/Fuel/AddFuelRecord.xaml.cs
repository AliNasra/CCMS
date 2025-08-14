
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
using WpfApp2.Models.Items;
using WpfApp2.ViewModels.Cement;
using WpfApp2.ViewModels.Fuel;
using WpfApp2.Views.Cement;
using System.IO;
using Newtonsoft.Json;
using WpfApp2.Services;
using MessageBox = System.Windows.MessageBox;


namespace WpfApp2.Views.Fuel
{
    /// <summary>
    /// Interaction logic for AddFuelRecord.xaml
    /// </summary>
    public partial class AddFuelRecord : Page
    {
        public AddFuelRecordViewModel AddFuelVM { get; set; }
        public AddFuelRecord()
        {
            InitializeComponent();
            AddFuelVM = new AddFuelRecordViewModel();
            DataContext = AddFuelVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddFuelVM.FrameWidth = e.NewSize.Width;
                AddFuelVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!FuelService.canAddRecords())
                {
                    throw new Exception("لا يمكنك ادخال بيانات التمام اكتر من مرة واحدة فاليوم");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(
                        "هل انت متأكد من تمام السولار؟",       
                        "تنبيه",                             
                        MessageBoxButton.YesNo,                     
                        MessageBoxImage.Question                    
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        int sumofImportedFuel = AddFuelVM.FuelRecords.Select(x => int.Parse(x.importedFuel)).Sum();
                        FuelService.UpdateProcurementOfficeFuelStorage(sumofImportedFuel);
                        FuelService.AddFuelRecords(AddFuelVM.FuelRecords.ToList());
                        var emptyList = new List<FuelRecord>();
                        var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                        File.WriteAllText(AddFuelVM.fuelRecordsFilePath, json);
                        MessageBox.Show($"تم إضافة تمام السولار بنجاح", "تنبيه", MessageBoxButton.OK,MessageBoxImage.Information);
                        NavigationService?.Navigate(new FuelMenu());
                    }
                    else
                    {
                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(AddFuelVM.FuelRecords, Formatting.Indented);
            File.WriteAllText(AddFuelVM.fuelRecordsFilePath, json);
            NavigationService?.Navigate(new FuelMenu());
        }
    }
}
