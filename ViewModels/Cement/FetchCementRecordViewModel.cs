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

namespace WpfApp2.ViewModels.Cement
{
    public class FetchCementRecordViewModel : INotifyPropertyChanged
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

        private ObservableCollection<FetchCementRecord> _cementRecords;
        public ObservableCollection<FetchCementRecord> cementRecords
        {
            get => _cementRecords;
            set
            {
                _cementRecords = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startDate;
        public DateTime startDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }

        private DateTime _endDate;
        public DateTime endDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
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

        public ObservableCollection<string> mixerNames { get; set; }
        void fetchRecords()
        {
            cementRecords = new ObservableCollection<FetchCementRecord>(CementService.fetchCementRecords(mixerName, startDate, endDate));

        }

        public FetchCementRecordViewModel()
        {
            CementService.initializeCementRecords();
            mixerNames = new ObservableCollection<string>(MixerService.getOperationalMixers().Select(x => x.mixerName).Prepend("-"));
            mixerName  = mixerNames.First();
            startDate  = DateTime.Today.Date;
            endDate    = DateTime.Today.Date;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
