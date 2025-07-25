using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Resources
{
    public class AdditionalDataViewModel : INotifyPropertyChanged
    {
        public static string AdditionalDataPath = "additionalData.json";
        private double _frameWidth;
        public double FrameWidth
        {
            get => _frameWidth;
            set
            {
                _frameWidth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonWidth));
                OnPropertyChanged(nameof(ButtonMargin));
                OnPropertyChanged(nameof(TextBoxWidth));
                OnPropertyChanged(nameof(LabelWidth));
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
                OnPropertyChanged(nameof(ButtonMargin));
                OnPropertyChanged(nameof(TextBoxHeight));
                OnPropertyChanged(nameof(LabelHeight));
                OnPropertyChanged(nameof(ItemMargin));
            }
        }

        public double ButtonWidth => FrameWidth * 0.15;
        public double ButtonHeight => FrameHeight * 0.10;

        public double LabelWidth => FrameWidth * 0.13;
        public double LabelHeight => FrameHeight * 0.14;

        public double TextBoxWidth => FrameWidth * 0.3;
        public double TextBoxHeight => FrameHeight * 0.14;

        public Thickness ItemMargin => new Thickness(
            FrameWidth * 0.005,      // Left
            FrameHeight * 0.005,     // Top
            FrameWidth * 0.005,      // Right
            FrameHeight * 0.005      // Bottom
        );

        public Thickness ButtonMargin => new Thickness(
            FrameWidth * 0.01,      // Left
            FrameHeight * 0.01,     // Top
            FrameWidth * 0.01,      // Right
            FrameHeight * 0.01      // Bottom
        );


        

        private string _deputyRank;
        public string deputyRank
        {
            get => _deputyRank;
            set
            {
                _deputyRank = value;
                OnPropertyChanged();
            }
        }

        private string _deputyName;
        public string deputyName
        {
            get => _deputyName;
            set
            {
                _deputyName = value;
                OnPropertyChanged();
            }
        }


        private string _administrativeAffairsOfficerRank;
        public string administrativeAffairsOfficerRank
        {
            get => _administrativeAffairsOfficerRank;
            set
            {
                _administrativeAffairsOfficerRank = value;
                OnPropertyChanged();
            }
        }

        private string _administrativeAffairsOfficerName;
        public string administrativeAffairsOfficerName
        {
            get => _administrativeAffairsOfficerName;
            set
            {
                _administrativeAffairsOfficerName = value;
                OnPropertyChanged();
            }
        }

        private string _automotivesOfficerRank;
        public string automotivesOfficerRank
        {
            get => _automotivesOfficerRank;
            set
            {
                _automotivesOfficerRank = value;
                OnPropertyChanged();
            }
        }

        private string _automotivesOfficerName;
        public string automotivesOfficerName
        {
            get => _automotivesOfficerName;
            set
            {
                _automotivesOfficerName = value;
                OnPropertyChanged();
            }
        }


        private string _procurmentOfficeFuelAmount;
        public string procurmentOfficeFuelAmount
        {
            get => _procurmentOfficeFuelAmount;
            set
            {
                _procurmentOfficeFuelAmount = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<string> rankNames { get; set; }
        public AdditionalDataViewModel()
        {
            rankNames = new ObservableCollection<string>(new List<string>{"نقيب","رائد","مقدم","عقيد"});
            AdditionalInfo infoData = new AdditionalInfo();
            if (File.Exists(AdditionalDataPath))
            {
                var additionalRecordsJsonString = File.ReadAllText(AdditionalDataPath);
                infoData                        = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);         
            }
            else
            {
                infoData = new AdditionalInfo
                {
                    deputyRank = rankNames.First(),
                    deputyName = "",
                    administrativeAffairsOfficerRank = rankNames.First(),
                    administrativeAffairsOfficerName = "",
                    automotivesOfficerRank = rankNames.First(),
                    automotivesOfficerName = "",
                    procurmentOfficeFuelAmount = 0,
                };
                var json = JsonConvert.SerializeObject(infoData, Formatting.Indented);
                File.WriteAllText(AdditionalDataPath, json);    
            }
            deputyRank                       = infoData.deputyRank;
            deputyName                       = infoData.deputyName;
            administrativeAffairsOfficerRank = infoData.administrativeAffairsOfficerRank;
            administrativeAffairsOfficerName = infoData.administrativeAffairsOfficerName;
            automotivesOfficerRank           = infoData.automotivesOfficerRank;
            automotivesOfficerName           = infoData.automotivesOfficerName;
            procurmentOfficeFuelAmount       = infoData.procurmentOfficeFuelAmount.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}