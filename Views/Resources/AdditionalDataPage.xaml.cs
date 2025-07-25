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
using WpfApp2.Models;
using WpfApp2.Services;
using WpfApp2.ViewModels.Resources;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp2.Views.Resources
{
    /// <summary>
    /// Interaction logic for AdditionalDataPage.xaml
    /// </summary>
    public partial class AdditionalDataPage : Page
    {
        public AdditionalDataViewModel AdditionalDataVM { get; set; }
        public AdditionalDataPage()
        {
            InitializeComponent();
            AdditionalDataVM = new AdditionalDataViewModel();
            DataContext = AdditionalDataVM;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                AdditionalDataVM.FrameWidth = e.NewSize.Width;
                AdditionalDataVM.FrameHeight = e.NewSize.Height;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AdditionalDataVM.deputyName) || string.IsNullOrWhiteSpace(AdditionalDataVM.administrativeAffairsOfficerName) || string.IsNullOrWhiteSpace(AdditionalDataVM.automotivesOfficerName))
                {
                    throw new Exception("برجاء إدخال أسماء صحيحة"); 
                }
                int procurmentOfficeFuelAmountInt;
                if (string.IsNullOrWhiteSpace(AdditionalDataVM.procurmentOfficeFuelAmount))
                {
                    procurmentOfficeFuelAmountInt = 0;
                }
                else
                {
                    bool procurmentOfficeFuelAmountIsInt = int.TryParse(AdditionalDataVM.procurmentOfficeFuelAmount, out procurmentOfficeFuelAmountInt);
                    if (procurmentOfficeFuelAmountIsInt == false)
                    {
                        throw new Exception("برجاء إدخال رقم صحيح لمخزون مكتب الإتصال");

                    }
                }
                AdditionalInfo info = new AdditionalInfo
                {
                    deputyRank                       = AdditionalDataVM.deputyRank,
                    deputyName                       = AdditionalDataVM.deputyName,
                    administrativeAffairsOfficerRank = AdditionalDataVM.administrativeAffairsOfficerRank,
                    administrativeAffairsOfficerName = AdditionalDataVM.administrativeAffairsOfficerName,
                    automotivesOfficerRank           = AdditionalDataVM.automotivesOfficerRank,
                    automotivesOfficerName           = AdditionalDataVM.automotivesOfficerName,
                    procurmentOfficeFuelAmount       = procurmentOfficeFuelAmountInt,

                };
                var json = JsonConvert.SerializeObject(info, Formatting.Indented);
                File.WriteAllText(AdditionalDataViewModel.AdditionalDataPath, json);
                MessageBox.Show($"تم تعديل البيانات بنجاح", "تنبيه", MessageBoxButton.OK);
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
