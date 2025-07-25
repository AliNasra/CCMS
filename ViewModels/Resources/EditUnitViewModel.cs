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


namespace WpfApp2.ViewModels.Resources
{
    public class EditUnitViewModel : INotifyPropertyChanged
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


        private string _unitName;
        public string unitName
        {
            get => _unitName;
            set
            {
                _unitName = value;               
                Unit unit = UnitService.fetchUnit(_unitName);                
                if (unit == null)
                {
                    _selectedDesignation    = "";
                    _selectedSpecialization = "";
                    _precastWallTarget      = "";
                    _unitCode               = "";
                    _selfSufficienyReserve  = "";
                    _selectedOperationality = "";
                    _benzine80Reserve       = "";
                    _summerDieselReserve    = "";
                }
                else
                {
                    _selectedDesignation    = unit.unitDesignation;
                    _selectedSpecialization = unit.unitSpecialization;
                    _precastWallTarget      = $"{unit.preCastWallTarget}";
                    _unitCode               = $"{unit.unitCode}";
                    _selfSufficienyReserve  = $"{unit.selfSufficienyReserve}";
                    _selectedOperationality = unit.isOperational == true ? "بالخدمة" : "ليست بالخدمة";
                    _benzine80Reserve       = $"{unit.benzine80Reserve}";
                    _summerDieselReserve    = $"{unit.summerDieselReserve}";
                }    
                OnPropertyChanged(nameof(unitName));
                OnPropertyChanged(nameof(SelectedDesignation));
                OnPropertyChanged(nameof(SelectedSpecialization));
                OnPropertyChanged(nameof(PrecastWallTarget));
                OnPropertyChanged(nameof(UnitCode));
                OnPropertyChanged(nameof(SelfSufficienyReserve));
                OnPropertyChanged(nameof(Benzine80Reserve));
                OnPropertyChanged(nameof(SummerDieselReserve));
                OnPropertyChanged(nameof(SelectedOperationality));
            }
        }

        private string _selectedDesignation;
        public string SelectedDesignation
        {
            get { return _selectedDesignation; }
            set
            {
                if (_selectedDesignation != null)
                {
                    _selectedDesignation = value;
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

        private string _selectedOperationality;
        public string SelectedOperationality
        {
            get { return _selectedOperationality; }
            set
            {
                if (_selectedOperationality != null)
                {
                    _selectedOperationality = value;
                    OnPropertyChanged(nameof(SelectedOperationality));
                };

            }
        }

        public ObservableCollection<string> unitNames          { get; set; }
        public ObservableCollection<string> DesignationList    { get; set; }
        public ObservableCollection<string> SpecializationList { get; set; }
        public ObservableCollection<string> OperationalityList { get; set; }
        public EditUnitViewModel()
        {
            unitNames          = new ObservableCollection<string>(UnitService.retrieveUnitNames());
            DesignationList    = new ObservableCollection<string>(new List<string> { "اللواء", "الكتيبة" });
            SpecializationList = new ObservableCollection<string>(new List<string> { "إنشاءات", "حفر أنفاق" });
            OperationalityList = new ObservableCollection<string>(new List<string> { "بالخدمة", "ليست بالخدمة" });
            unitName           = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
