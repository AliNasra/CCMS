using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WpfApp2.ViewModels.Cement;
using WpfApp2.ViewModels.Fuel;

namespace WpfApp2.Models.Items
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FuelRecord : INotifyPropertyChanged
    {
        private int _depotID;
        [JsonProperty]
        public int depotID
        {
            get => _depotID;
            set
            {
                _depotID = value;
            }
        }
        private string _depotName;
        [JsonProperty]
        public string DepotName
        {
            get => _depotName;
            set
            {
                _depotName = value;
                if (_depotName.Length > 0 && DepotNames.Contains(_depotName))
                {
                    previouslyRemainingFuel = AddFuelRecordViewModel.depotList.Where(x => x.depotName == _depotName).Select(g => g.currentReserve).First().ToString();
                    depotID = AddFuelRecordViewModel.depotList.Where(x => x.depotName == _depotName).Select(x => x.depotID).First();
                }
                else
                {
                    previouslyRemainingFuel = "";
                }
                OnPropertyChanged(DepotName);
                OnPropertyChanged(previouslyRemainingFuel);
            }
        }
        

        private string _previouslyRemainingFuel;
        public string previouslyRemainingFuel
        {
            get => _previouslyRemainingFuel;
            set { _previouslyRemainingFuel = value; OnPropertyChanged(); }
        }

        private string _remaniningFuel;
        [JsonProperty]
        public string remainingFuel
        {
            get => _remaniningFuel;
            set { _remaniningFuel = value; OnPropertyChanged(); }
        }

        private string _consumedFuel;
        [JsonProperty]
        public string consumedFuel
        {
            get => _consumedFuel;
            set
            {
                _consumedFuel = value;
                OnPropertyChanged(consumedFuel);
                if (importedFuel != null)
                {
                    int previouslyRemainingFuelInt;
                    int importedFuelInt;
                    int consumedFuelInt;
                    if (_depotName.Length > 0 && int.TryParse(previouslyRemainingFuel, out previouslyRemainingFuelInt) && int.TryParse(importedFuel, out importedFuelInt) && int.TryParse(consumedFuel, out consumedFuelInt))
                    {
                        remainingFuel = $"{previouslyRemainingFuelInt + importedFuelInt - consumedFuelInt}";
                    }
                    else
                    {
                        _consumedFuel = "";
                    }
                    OnPropertyChanged(remainingFuel);
                }          
            }
        }

        private string _importedFuel;
        [JsonProperty]
        public string importedFuel
        {
            get => _importedFuel;
            set
            {
                _importedFuel = value;
                int previouslyRemainingFuelInt;
                int importedFuelInt;
                int consumedFuelInt;
                if (_depotName.Length > 0 && int.TryParse(previouslyRemainingFuel, out previouslyRemainingFuelInt) && int.TryParse(importedFuel, out importedFuelInt) && int.TryParse(consumedFuel, out consumedFuelInt))
                {
                    remainingFuel = $"{previouslyRemainingFuelInt + importedFuelInt - consumedFuelInt}";
                }
                else
                {
                    _consumedFuel = "";
                }
                OnPropertyChanged(remainingFuel);
                OnPropertyChanged(importedFuel);
            }
        }

        public  ObservableCollection<string> DepotNames { get; set; }

        public FuelRecord()
        {
            DepotNames = new ObservableCollection<string>(AddFuelRecordViewModel.depotNames);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
