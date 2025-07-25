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
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Fuel
{
    public class FetchFuelRecordViewModel : INotifyPropertyChanged
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


        public Thickness ReturnButtonMargin => new Thickness(
            FrameWidth * 0.1,      // Left
            FrameHeight * 0.01,     // Top
            0,       // Right
            FrameHeight * 0.01      // Bottom
        );

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );

        private ObservableCollection<FetchFuelRecord> _fuelRecords;
        public ObservableCollection<FetchFuelRecord> fuelRecords
        {
            get => _fuelRecords;
            set
            {
                _fuelRecords = value;
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

        private string _depotName;
        public string depotName
        {
            get => _depotName;
            set
            {
                _depotName = value;
                OnPropertyChanged();
                fetchRecords();
            }
        }

        public ObservableCollection<string> depotNames { get; set; }
        void fetchRecords()
        {
            fuelRecords = new ObservableCollection<FetchFuelRecord>(FuelService.fetchfuelRecords(depotName,startDate,endDate));
        }

        public FetchFuelRecordViewModel()
        {
            FuelService.initializeFuelRecords();
            depotNames = new ObservableCollection<string>(DepotService.fetchDepots().Select(x => x.depotName).Prepend("-"));
            depotName = depotNames.First();
            startDate = DateTime.Today.Date;
            endDate   = DateTime.Today.Date;
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    }
