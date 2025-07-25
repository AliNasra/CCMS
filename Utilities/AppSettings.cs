using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp2.Services;
using WpfApp2.ViewModels.Concrete;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Windows.Media.FontFamily;

namespace WpfApp2.Utilities
{
    public class AppSettings : INotifyPropertyChanged
    {
        public static AppSettings _settings;

        /// Getter Function for utilizing the static properties of _settings
        public static AppSettings getSettings()
        {
            if (_settings == null)
            {
                _settings = new AppSettings();
                return _settings;
            }
            else
            {
                return _settings;
            }
        }
        public string companyListPath = "companyList.json";

        private ObservableCollection<string> _companyList;
        public ObservableCollection<string> companyList
        {
            get => _companyList;
            set
            {
                _companyList = value;
                OnPropertyChanged();
                
            }
        }

        public void updateCompanyList(string newCompany)
        {
            if (string.IsNullOrWhiteSpace(newCompany) == false &&!companyList.Any(x => x == newCompany.Trim()))
            {
                companyList.Add(newCompany);
                List<string> companyListSorted = companyList.OrderBy(x=>x).ToList();
                companyList.Clear();
                companyList = new ObservableCollection<string>(companyListSorted);
            }      
            var json = JsonConvert.SerializeObject(companyList, Formatting.Indented);
            File.WriteAllText(companyListPath, json);

        }


        ////////////////// Private constuctor to AppSettings
        private AppSettings()
        {
            BackgroundBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 0),
                GradientStops = new GradientStopCollection
            {
                new GradientStop((Color)ColorConverter.ConvertFromString("#08203e"), 0.0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#557c93"), 1.0)
            }
            };
            TitleColor = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#e6e2d6")));
            LabelColor = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FFD6A2")));
            ButtonColor = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FED07B")));
            TextColor = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#014968")));
            TextFont = new FontFamily("Arabic Typesetting");
            TextFontSize = 20;
            if (File.Exists(companyListPath))
            {
                var companyRecordsJsonString = File.ReadAllText(companyListPath);
                companyList =  new ObservableCollection<string>(JsonConvert.DeserializeObject<List<string>>(companyRecordsJsonString));
            }
            else
            {
                companyList = new ObservableCollection<string>(ConcreteService.retrieveCompanyNames());
                var json = JsonConvert.SerializeObject(companyList, Formatting.Indented);
                File.WriteAllText(companyListPath, json);
            }
        }

        ////////////////// General Features common among pages

        private int _textFontSize;

        public int TextFontSize
        {
            get => _textFontSize;
            set
            {
                _textFontSize = value;
                OnPropertyChanged(nameof(TextFontSize));
            }
        }

        private FontFamily _textFont;

        public FontFamily TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                OnPropertyChanged(nameof(TextFont));
            }
        }


        private SolidColorBrush _textColor;

        public SolidColorBrush TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }


        private LinearGradientBrush _backgroundBrush;

        public LinearGradientBrush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;
                OnPropertyChanged(nameof(BackgroundBrush));
            }
        }
        private SolidColorBrush _titleColor;

        public SolidColorBrush TitleColor
        {
            get => _titleColor;
            set
            {
                _titleColor = value;
                OnPropertyChanged(nameof(TitleColor));
            }
        }
        private SolidColorBrush _labelColor;

        public SolidColorBrush LabelColor
        {
            get => _labelColor;
            set
            {
                _labelColor = value;
                OnPropertyChanged(nameof(LabelColor));
            }
        }

        private SolidColorBrush _buttonColor;

        public SolidColorBrush ButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                OnPropertyChanged(nameof(ButtonColor));
            }
        }
        //////////////////

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
