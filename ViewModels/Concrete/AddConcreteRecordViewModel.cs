using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using WpfApp2.Services;
using Newtonsoft.Json;
using System.IO;

namespace WpfApp2.ViewModels.Concrete
{
    public class AddConcreteRecordViewModel : INotifyPropertyChanged
    {
        public static string concreteRecordsFilePath = "ConcreteRecords.json";
        
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
                if (int.Parse(SelectedMixerCount) <= ConcreteRecords.Count)
                {
                    var trimmedList = ConcreteRecords.Take(int.Parse(SelectedMixerCount)).ToList();
                    ConcreteRecords = new ObservableCollection<ConcreteRecords>(trimmedList);
                }
                else
                {
                    for (var i = ConcreteRecords.Count; i < int.Parse(SelectedMixerCount); i++)
                    {
                        ConcreteRecords.Add(new ConcreteRecords
                        {
                            company              = "",
                            project              = "",
                            isReinforced         = "",
                            concreteAmount       = "",
                            mixerName            = "",
                            companyFilteringText = "",
                            isInformal           = false
                        });
                    }
                }
            }
        }

        private ObservableCollection<ConcreteRecords> _concreteRecords;
        public ObservableCollection<ConcreteRecords> ConcreteRecords
        {
            get => _concreteRecords;
            set
            {
                _concreteRecords = value;
                OnPropertyChanged();
            }
        }

        public static List<Mixer> mixerList;

        

        public AddConcreteRecordViewModel()
        {
            mixerList = MixerService.getOperationalMixers();
            List<string> stringNumbers = Enumerable.Range(0, 51).Select(x => x.ToString()).ToList();
            MixerCount = new ObservableCollection<string>(stringNumbers);
            if (File.Exists(concreteRecordsFilePath))
            {
                var concreteRecordsJsonString = File.ReadAllText(concreteRecordsFilePath);
                List<ConcreteRecords> filteredConcreteRecords = ConcreteService.FilterConcreteRecords(JsonConvert.DeserializeObject<List<ConcreteRecords>>(concreteRecordsJsonString));
                ConcreteRecords = new ObservableCollection<ConcreteRecords>(filteredConcreteRecords);
                SelectedMixerCount = ConcreteRecords.Count.ToString();
            }
            else
            {
                var emptyList = new List<CementRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(concreteRecordsFilePath, json);
                ConcreteRecords = new ObservableCollection<ConcreteRecords>();
                SelectedMixerCount = MixerCount.First();
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
