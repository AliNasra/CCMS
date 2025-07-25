using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WpfApp2.Services;
using WpfApp2.ViewModels.Document;
using WpfApp2.ViewModels.Resources;
using WpfApp2.Views.Resources;
using MessageBox = System.Windows.MessageBox;

namespace WpfApp2.Views.Document
{
    /// <summary>
    /// Interaction logic for DocumentList.xaml
    /// </summary>
    public partial class DocumentMenu : Page
    {
        public DocumentMenuViewModel DocumentMenuVM { get; set; }
        public DocumentMenu()
        {
            InitializeComponent();
            DocumentMenuVM = new DocumentMenuViewModel();
            DataContext = DocumentMenuVM;
        }



        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DocumentMenuVM.FrameWidth = e.NewSize.Width;
                DocumentMenuVM.FrameHeight = e.NewSize.Height;
            }
        }

        public void createFuelReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DocumentServices.createFuelDocument();
                MessageBox.Show($"تم تحديث تمام السولار بنجاح", "نجاح", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }


        }


        public void createColonelReport_Click(object sender, RoutedEventArgs e)
        {
   
            try
            {
               DocumentServices.createColonelDocument();
               MessageBox.Show($"تم تحديث تمام العقيد بنجاح", "نجاح", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }


        }

        public void createHQReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               DocumentServices.createHQDocument();
               MessageBox.Show($"تم تحديث تمام الإدارة بنجاح", "نجاح", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "تنبيه", MessageBoxButton.OK);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!\nStay Tuned!", "تنبيه",MessageBoxButton.OK);
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenu());
        }
    }
}
