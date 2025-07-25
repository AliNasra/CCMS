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


namespace WpfApp2.ViewModels.Cement
{
    public class AddCementRecordViewModel : INotifyPropertyChanged
    {
        public static string cementRecordsFilePath = "CementRecords.json";
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


        public ObservableCollection<string> MixerCount { get; set; }

        private string _selectedMixerCount;
        public string SelectedMixerCount
        {
            get => _selectedMixerCount;
            set
            {
                _selectedMixerCount = value;
                OnPropertyChanged(nameof(SelectedMixerCount));
                if (int.Parse(SelectedMixerCount) <= CementRecords.Count)
                {
                    var trimmedList = CementRecords.Take(int.Parse(SelectedMixerCount)).ToList();
                    CementRecords   = new ObservableCollection<CementRecord>(trimmedList);
                }
                else
                {
                    for (var i = CementRecords.Count; i < int.Parse(SelectedMixerCount); i++)
                    {
                        CementRecords.Add(new CementRecord
                        {
                            MixerName                 = "",
                            remaniningCement          = "",
                            importedCement            = "",                        
                        });      
                    }
                }            
            }
        }
        public static List<Mixer> mixerList;

        private ObservableCollection<CementRecord> _cementRecords;
        public ObservableCollection<CementRecord> CementRecords
        {
            get => _cementRecords;
            set
            {
                _cementRecords = value;
                OnPropertyChanged();
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public AddCementRecordViewModel()
        {
            mixerList = MixerService.getOperationalMixers();
            List<string> stringNumbers = Enumerable.Range(0, mixerList.Count + 1).Select(x => x.ToString()).ToList();
            MixerCount = new ObservableCollection<string>(stringNumbers);
            if (File.Exists(cementRecordsFilePath))
            {
                var cementRecordsJsonString = File.ReadAllText(cementRecordsFilePath);
                List<CementRecord> filteredCementRecords = CementService.FilterCementRecords(JsonConvert.DeserializeObject<List<CementRecord>>(cementRecordsJsonString));
                CementRecords = new ObservableCollection<CementRecord>(filteredCementRecords);
                SelectedMixerCount = CementRecords.Count.ToString();
            }
            else
            {
                var emptyList = new List<CementRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(cementRecordsFilePath, json);               
                CementRecords = new ObservableCollection<CementRecord>();
                SelectedMixerCount = MixerCount.First();
            }
            
            
            //MessageBox.Show($"{CementRecords.Count} {SelectedMixerCount}", "تنبيه", MessageBoxButton.OK);
        }
    }
}
