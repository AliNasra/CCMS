using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2.ViewModels.Resources
{
    public class AddUnitViewModel : INotifyPropertyChanged
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

        private string _selectedDesignaton;
        public string SelectedDesignation
        {
            get { return _selectedDesignaton; }
            set
            {
                if (_selectedDesignaton != null)
                {
                    _selectedDesignaton = value;
                    OnPropertyChanged(nameof(SelectedDesignation));
                };

            }
        }

        private string _selectedSpecialization;
        public string SelectedSpecialization
        {
            get { return _selectedSpecialization; }
            set
            {
                if (_selectedSpecialization != null)
                {
                    _selectedSpecialization = value;
                    OnPropertyChanged(nameof(SelectedSpecialization));
                };

            }
        }

        private string _precastWallTarget;
        public string PrecastWallTarget
        {
            get { return _precastWallTarget; }
            set
            {
                if (_precastWallTarget != null)
                {
                    _precastWallTarget = value;
                    OnPropertyChanged(nameof(PrecastWallTarget));
                };

            }
        }

        private string _unitCode;
        public string UnitCode
        {
            get { return _unitCode; }
            set
            {
                if (_unitCode != null)
                {
                    _unitCode = value;
                    OnPropertyChanged(nameof(UnitCode));
                };

            }
        }

        private string _selfSufficienyReserve;
        public string SelfSufficienyReserve
        {
            get { return _selfSufficienyReserve; }
            set
            {
                if (_selfSufficienyReserve != null)
                {
                    _selfSufficienyReserve = value;
                    OnPropertyChanged(nameof(SelfSufficienyReserve));
                };

            }
        }

        private string _benzine80Reserve;
        public string Benzine80Reserve
        {
            get { return _benzine80Reserve; }
            set
            {
                if (_benzine80Reserve != null)
                {
                    _benzine80Reserve = value;
                    OnPropertyChanged(nameof(Benzine80Reserve));
                };

            }
        }

        private string _summerDieselReserve;
        public string SummerDieselReserve
        {
            get { return _summerDieselReserve; }
            set
            {
                if (_summerDieselReserve != null)
                {
                    _summerDieselReserve = value;
                    OnPropertyChanged(nameof(SummerDieselReserve));
                };

            }
        }

        public ObservableCollection<string> DesignationList    { get; set; }
        public ObservableCollection<string> SpecializationList { get; set; }

        public AddUnitViewModel()
        {
            DesignationList         = new ObservableCollection<string>(new List<string> {"اللواء","الكتيبة"});
            SpecializationList      = new ObservableCollection<string>(new List<string> {"إنشاءات", "حفر أنفاق" });
            _selectedSpecialization = SpecializationList.First();
            _selectedDesignaton     = DesignationList.First();
            _unitCode               = "";
            _selfSufficienyReserve  = "";
            _precastWallTarget      = "";
            _benzine80Reserve       = "";
            _summerDieselReserve    = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
