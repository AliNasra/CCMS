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
using WpfApp2.Services;

namespace WpfApp2.ViewModels.Resources
{
    public class EditMixerViewModel : INotifyPropertyChanged
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


        private string _cabbageNo;
        public string CabbageNo
        {
            get { return _cabbageNo; }
            set
            {
                if (_cabbageNo != null)
                {
                    _cabbageNo = value;
                    OnPropertyChanged(nameof(CabbageNo));
                };

            }
        }

        private string _mixerName;
        public string MixerName
        {
            get { return _mixerName; }
            set
            {
                if (_mixerName != null)
                {
                    _mixerName = value;
                    OnPropertyChanged(nameof(MixerName));
                };

            }
        }

        private string _selectedMixerName;
        public string SelectedMixerName
        {
            get { return _selectedMixerName; }
            set
            {
                if (_selectedMixerName != null)
                {
                    _selectedMixerName = value;
                    Mixer mixer = MixerService.getMixer(_selectedMixerName);
                    if (mixer == null)
                    {
                        _mixerName             = "";
                        _depotName             = "";
                        _currentCementLevel    = "";
                        _isOperational         = "";
                        _operationalCapacity   = "";
                        _cabbageNo             = "";

                    }
                    else
                    {
                        FuelDepot depot = DepotService.fetchDepot(mixer.depotID);
                        _mixerName = mixer.mixerName;
                        _depotName = depot.depotName;
                        _currentCementLevel = mixer.currentCementLevel.ToString();
                        _isOperational = mixer.isOperational == true ? "بالخدمة" : "ليست بالخدمة";
                        _operationalCapacity = mixer.operationalCapacity.ToString();
                        _cabbageNo = mixer.cabbageNo.ToString();
                    }
                    OnPropertyChanged(nameof(SelectedMixerName));
                    OnPropertyChanged(nameof(MixerName));
                    OnPropertyChanged(nameof(DepotName));
                    OnPropertyChanged(nameof(CurrentCementLevel));
                    OnPropertyChanged(nameof(IsOperational));
                    OnPropertyChanged(nameof(OperationalCapacity));
                    OnPropertyChanged(nameof(CabbageNo));
                };

            }
        }


        private string _depotName;
        public string DepotName
        {
            get { return _depotName; }
            set
            {
                if (_depotName != null)
                {
                    _depotName = value;
                    OnPropertyChanged(nameof(DepotName));
                };

            }
        }

        private string _currentCementLevel;
        public string CurrentCementLevel
        {
            get { return _currentCementLevel; }
            set
            {
                if (_currentCementLevel != null)
                {
                    _currentCementLevel = value;
                    OnPropertyChanged(nameof(CurrentCementLevel));
                };

            }
        }

        private string _isOperational;
        public string IsOperational
        {
            get { return _isOperational; }
            set
            {
                if (_isOperational != null)
                {
                    _isOperational = value;
                    OnPropertyChanged(nameof(IsOperational));
                };

            }
        }

        private string _operationalCapacity;
        public string OperationalCapacity
        {
            get { return _operationalCapacity; }
            set
            {
                if (_operationalCapacity != null)
                {
                    _operationalCapacity = value;
                    OnPropertyChanged(nameof(OperationalCapacity));
                };

            }
        }


        public ObservableCollection<string> OperationStatusList { get; set; }
        public ObservableCollection<string> DepotList { get; set; }
        public ObservableCollection<string> MixerList { get; set; }

        public EditMixerViewModel()
        {
            var depotList         = DepotService.fetchDepots().Select(x=>x.depotName).ToList();
            OperationStatusList  = new ObservableCollection<string>(new List<string> { "بالخدمة", "ليست بالخدمة" });
            MixerList            = new ObservableCollection<string>(MixerService.getAllMixers().Select(x => x.mixerName).ToList());
            DepotList            = new ObservableCollection<string>(depotList);
            _selectedMixerName   = "";

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
