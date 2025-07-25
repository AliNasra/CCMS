using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Services;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using Newtonsoft.Json;
using System.IO;



namespace WpfApp2.ViewModels.Fuel
{
    public class AddFuelRecordViewModel : INotifyPropertyChanged
    {
        public string fuelRecordsFilePath = "FuelRecords.json";
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
                OnPropertyChanged(nameof(ButtonMargin));
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
                OnPropertyChanged(nameof(ButtonMargin));
                OnPropertyChanged(nameof(ItemMargin));
            }
        }

        public double ButtonWidth => FrameWidth * 0.35;
        public double ButtonHeight => FrameHeight * 0.15;
        public double ComboBoxWidth => FrameWidth * 0.10;
        public double ComboboxHeight => FrameHeight * 0.07;


        public Thickness ButtonMargin => new Thickness(
            FrameWidth * 0.05,      // Left
            FrameHeight * 0.05,     // Top
            FrameWidth * 0.05,      // Right
            FrameHeight * 0.05      // Bottom
        );

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );


        public ObservableCollection<string> DepotCount { get; set; }

        private string _selectedDepotCount;
        public string SelectedDepotCount
        {
            get => _selectedDepotCount;
            set
            {
                _selectedDepotCount = value;
                OnPropertyChanged(nameof(SelectedDepotCount));
                if (int.Parse(SelectedDepotCount) <= FuelRecords.Count)
                {
                    var trimmedList = FuelRecords.Take(int.Parse(SelectedDepotCount)).ToList();
                    FuelRecords = new ObservableCollection<FuelRecord>(trimmedList);
                }
                else
                {
                    for (var i = FuelRecords.Count; i < int.Parse(SelectedDepotCount); i++)
                    {
                        FuelRecords.Add(new FuelRecord
                        {                     
                            DepotName               = "",
                            consumedFuel            = "",
                            importedFuel            = "",                        
                        });      
                    }
                }            
            }
        }
        public static List<FuelDepot> depotList;
        public static List<string> depotNames;

        private ObservableCollection<FuelRecord> _fuelRecords;
        public ObservableCollection<FuelRecord> FuelRecords
        {
            get => _fuelRecords;
            set
            {
                _fuelRecords = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public AddFuelRecordViewModel()
        {
            depotList = DepotService.fetchOperationalDepots();
            depotNames = depotList.Select(x => x.depotName).ToList();
            List<string> stringNumbers = Enumerable.Range(0, depotList.Count + 1).Select(x => x.ToString()).ToList();
            DepotCount = new ObservableCollection<string>(stringNumbers);          
            if (File.Exists(fuelRecordsFilePath))
            {
                var fuelRecordsJsonString = File.ReadAllText(fuelRecordsFilePath);
                List<FuelRecord> filteredFuelRecords = FuelService.FilterFuelRecords(JsonConvert.DeserializeObject<List<FuelRecord>>(fuelRecordsJsonString));
                FuelRecords = new ObservableCollection<FuelRecord>(filteredFuelRecords);
                SelectedDepotCount = FuelRecords.Count.ToString();
            }
            else
            {
                var emptyList = new List<FuelRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(fuelRecordsFilePath, json);
                FuelRecords = new ObservableCollection<FuelRecord>();
                SelectedDepotCount = DepotCount.First();
            }
        }
    }
}
