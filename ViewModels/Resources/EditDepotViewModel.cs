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
    public class EditDepotViewModel : INotifyPropertyChanged
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


        private string _selectedDepotName;
        public string SelectedDepotName
        {
            get => _selectedDepotName;
            set
            {
                _selectedDepotName = value;             
                FuelDepot depot = DepotService.fetchDepot(_selectedDepotName);
                if (depot == null)
                {
                    _selectedUnit = "";
                    _depotName = "";
                    _depotStorageCapacity = "";
                    _currentReserve = "";
                    _lastImportedFuelAmount = "";
                    _lastConsignmentDate = DateTime.MinValue;
                    _selectedOperationality = "";
                }
                else
                {
                     Unit unit               = UnitService.fetchUnit(depot.unitID); 
                    _selectedUnit            = $"{unit.unitDesignation} {unit.unitCode} {unit.unitSpecialization}";
                    _depotName               = $"{depot.depotName}";
                    _depotStorageCapacity    = $"{depot.depotStorageCapacity}";
                    _currentReserve          = $"{depot.currentReserve}";
                    _lastImportedFuelAmount  = $"{depot.LastimportedFuelAmount}";
                    _lastConsignmentDate     = depot.LastConsignmentDate;
                    _selectedOperationality  = depot.isOperational == true ? "بالخدمة" : "ليست بالخدمة";
                }
                OnPropertyChanged(nameof(SelectedDepotName));
                OnPropertyChanged(nameof(SelectedUnit));
                OnPropertyChanged(nameof(DepotName));
                OnPropertyChanged(nameof(DepotStorageCapacity));
                OnPropertyChanged(nameof(CurrentReserve));
                OnPropertyChanged(nameof(LastImportedFuelAmount));
                OnPropertyChanged(nameof(LastConsignmentDate));
                OnPropertyChanged(nameof(SelectedOperationality));
            }
        }


        private string _selectedUnit;
        public string SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (_selectedUnit != null)
                {
                    _selectedUnit = value;
                    OnPropertyChanged(nameof(SelectedUnit));
                };

            }
        }

        private string _depotStorageCapacity;
        public string DepotStorageCapacity
        {
            get { return _depotStorageCapacity; }
            set
            {
                if (_depotStorageCapacity != null)
                {
                    _depotStorageCapacity = value;
                    OnPropertyChanged(nameof(DepotStorageCapacity));
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

        private string _currentReserve;
        public string CurrentReserve
        {
            get { return _currentReserve; }
            set
            {
                if (_currentReserve != null)
                {
                    _currentReserve = value;
                    OnPropertyChanged(nameof(CurrentReserve));
                };

            }
        }

        private string _lastImportedFuelAmount;
        public string LastImportedFuelAmount
        {
            get { return _lastImportedFuelAmount; }
            set
            {
                if (_lastImportedFuelAmount != null)
                {
                    _lastImportedFuelAmount = value;
                    OnPropertyChanged(nameof(LastImportedFuelAmount));
                };

            }
        }

        private DateTime _lastConsignmentDate;
        public DateTime LastConsignmentDate
        {
            get { return _lastConsignmentDate; }
            set
            {
                if (_lastConsignmentDate != null)
                {
                    _lastConsignmentDate = value;
                    OnPropertyChanged(nameof(LastConsignmentDate));
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

        public ObservableCollection<string> UnitList { get; set; }
        public ObservableCollection<string> DepotList { get; set; }
        public ObservableCollection<string> OperationalityList { get; set; }

        public EditDepotViewModel()
        {
            var unitList            = UnitService.retrieveUnits().Select(x => $"{x.unitDesignation} {x.unitCode} {x.unitSpecialization}").ToList();
            DepotList               = new ObservableCollection<string>(DepotService.fetchDepots().Select(x => x.depotName).ToList());
            OperationalityList      = new ObservableCollection<string>(new List<string> { "بالخدمة", "ليست بالخدمة" });
            UnitList                = new ObservableCollection<string>(unitList);
            _selectedDepotName      = "";
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}