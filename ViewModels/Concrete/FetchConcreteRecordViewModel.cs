using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models.Items;
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Concrete
{
    public class FetchConcreteRecordViewModel : INotifyPropertyChanged
    {
        private double _frameWidth;
        public double FrameWidth
        {
            get => _frameWidth;
            set
            {
                _frameWidth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonWidth));
                OnPropertyChanged(nameof(ComboBoxWidth));
                OnPropertyChanged(nameof(ReturnButtonMargin));
                OnPropertyChanged(nameof(ItemMargin));

            }
        }

        private double _frameHeight;
        public double FrameHeight
        {
            get => _frameHeight;
            set
            {
                _frameHeight = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonHeight));
                OnPropertyChanged(nameof(ComboboxHeight));
                OnPropertyChanged(nameof(ReturnButtonMargin));
                OnPropertyChanged(nameof(ItemMargin));
            }
        }

        public double ButtonWidth => FrameWidth * 0.15;
        public double ButtonHeight => FrameHeight * 0.15;
        public double ComboBoxWidth => FrameWidth * 0.10;
        public double ComboboxHeight => FrameHeight * 0.07;

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );

        public Thickness ReturnButtonMargin => new Thickness(
            FrameWidth * 0.1,      // Left
            FrameHeight * 0.01,     // Top
            0,       // Right
            FrameHeight * 0.01      // Bottom
        );

        private ObservableCollection<FetchConcreteRecord> _concreteRecords;
        public ObservableCollection<FetchConcreteRecord> concreteRecords
        {
            get => _concreteRecords;
            set
            {
                _concreteRecords = value;
                OnPropertyChanged();
            }
        }

        private string _concreteType;
        public string concreteType
        {
            get => _concreteType;
            set
            {
                _concreteType = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }
        private string _mixerName;
        public string mixerName
        {
            get => _mixerName;
            set
            {
                _mixerName = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }

        private string _company;
        public string company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }

        private string _project;
        public string project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }

        public ObservableCollection<string> mixerNames { get; set; }
        public ObservableCollection<string> companyNames { get; set; }
        public ObservableCollection<string> concreteTypes { get; set; }
        void fetchRecords()
        {
            concreteRecords = new ObservableCollection<FetchConcreteRecord>(ConcreteService.filterConcreteRecords(mixerName,project,company,concreteType));
        }

        public FetchConcreteRecordViewModel()
        {
            ConcreteService.initializeConcreteRecords();
            mixerNames = new ObservableCollection<string>(MixerService.getOperationalMixers().Select(x => x.mixerName).Prepend("-"));
            companyNames = new ObservableCollection<string>(ConcreteService.retrieveCompanyNames().Prepend("-"));
            concreteTypes = new ObservableCollection<string>(new List<string> { "-", "مسلحة", "عادية" });
            mixerName = mixerNames.First();
            company = companyNames.First();
            concreteType = concreteTypes.First();
            project = "";
            concreteRecords = new ObservableCollection<FetchConcreteRecord>(ConcreteService.filterConcreteRecords(mixerName, project, company, concreteType));

        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
