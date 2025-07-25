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
using Newtonsoft.Json;
using System.IO;
using WpfApp2.Services;
using WpfApp2.Models.Items;
using WpfApp2.Utilities;
using System.Diagnostics;
using MessageBox = System.Windows.MessageBox;
using ComboBox = System.Windows.Controls.ComboBox;

namespace WpfApp2.Views.Concrete
{
    /// <summary>
    /// Interaction logic for AddConcreteRecord.xaml
    /// </summary>
    public partial class AddConcreteRecord : Page
    {
        public AddConcreteRecordViewModel AddConcreteVM { get; set; }
        public AddConcreteRecord()
        {
            InitializeComponent();
            AddConcreteVM = new AddConcreteRecordViewModel();
            DataContext   = AddConcreteVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AddConcreteVM.FrameWidth = e.NewSize.Width;
                AddConcreteVM.FrameHeight = e.NewSize.Height;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ConcreteService.canAddRecords())
                {
                    throw new Exception("لا يمكنك ادخال بيانات التمام اكتر من مرة واحدة فاليوم");
                }
                ConcreteService.addConcreteRecord(AddConcreteVM.ConcreteRecords.ToList());
                var emptyList = new List<ConcreteRecords>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(AddConcreteRecordViewModel.concreteRecordsFilePath, json);
                MessageBox.Show($"تم إضافة تمام الخرسانة بنجاح", "تنبيه", MessageBoxButton.OK);
                NavigationService?.Navigate(new ConcreteMenu());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }
        }

        private void Company_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var comboBox = sender as ComboBox;
                if (comboBox == null) return;

                string newCompany = comboBox.Text.Trim();
                if (AppSettings.getSettings().companyList.Contains(newCompany))
                {
                    comboBox.SelectedItem = newCompany;
                }
                else
                {
                    comboBox.SelectedItem = newCompany;
                    AppSettings.getSettings().updateCompanyList(newCompany);
                    foreach (var concreteRecord in AddConcreteVM.ConcreteRecords)
                    {

                        concreteRecord.companyList = new System.Collections.ObjectModel.ObservableCollection<string>(AppSettings.getSettings().companyList);
                    }
                }
                
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(AddConcreteVM.ConcreteRecords, Formatting.Indented);
            File.WriteAllText(AddConcreteRecordViewModel.concreteRecordsFilePath, json);
            NavigationService?.Navigate(new ConcreteMenu());
        }
    }
}
