using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models.Items;
using WpfApp2.Models;
using WpfApp2.Services;
using System.Windows;
using WpfApp2.Views.Concrete;
using System.IO;

namespace WpfApp2.ViewModels.Concrete
{
    public class AddWallRecordViewModel : INotifyPropertyChanged
    {
        public static string wallRecordsFilePath = "PreCastWallRecords.json";
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

        public ObservableCollection<string> unitCount { get; set; }

        private string _selectedUnitCount;
        public string selectedUnitCount
        {
            get => _selectedUnitCount;
            set
            {
                _selectedUnitCount = value;
                OnPropertyChanged(nameof(selectedUnitCount));
                if (int.Parse(selectedUnitCount) <= WallRecords.Count)
                {
                    var trimmedList = WallRecords.Take(int.Parse(selectedUnitCount)).ToList();
                    WallRecords = new ObservableCollection<PreCastWallRecord>(trimmedList);
                }
                else
                {
                    for (var i = WallRecords.Count; i < int.Parse(selectedUnitCount); i++)
                    {
                        WallRecords.Add(new PreCastWallRecord
                        {
                            unitName               = "",
                            plannedLength          = "",
                            previouslyAccomplished = "",
                            accomplishedToday      = "",
                            transportedAmountToday = "",
                            previouslyTransported  = "",
                            remainingOnSite        = "",
                        });
                    }
                }
            }
        }

        private ObservableCollection<PreCastWallRecord> _wallRecords;
        public ObservableCollection<PreCastWallRecord> WallRecords
        {
            get => _wallRecords;
            set
            {
                _wallRecords = value;
                OnPropertyChanged();
            }
        }

        public static List<Unit> unitList;

        public AddWallRecordViewModel()
        {
            unitList = UnitService.getUnitsWithPreCastWallTarget();
            List<string> stringNumbers = Enumerable.Range(0, unitList.Count + 1).Select(x => x.ToString()).ToList();
            unitCount = new ObservableCollection<string>(stringNumbers);
            if (File.Exists(wallRecordsFilePath))
            {
                var wallRecordsJsonString = File.ReadAllText(wallRecordsFilePath);
                List<PreCastWallRecord> filteredList = PreCastWallService.FilterWallRecords(JsonConvert.DeserializeObject<List<PreCastWallRecord>>(wallRecordsJsonString));
                WallRecords = new ObservableCollection<PreCastWallRecord>(filteredList);
                selectedUnitCount = WallRecords.Count.ToString();
            }
            else
            {
                var emptyList = new List<CementRecord>();
                var json = JsonConvert.SerializeObject(emptyList, Formatting.Indented);
                File.WriteAllText(wallRecordsFilePath, json);
                WallRecords = new ObservableCollection<PreCastWallRecord>();
                selectedUnitCount = unitCount.First();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

