using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.ViewModels.Cement;

namespace WpfApp2.Models.Items
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CementRecord : INotifyPropertyChanged
    {
        private string _mixerName;
        [JsonProperty]
        public string MixerName
        {
            get => _mixerName;
            set
            {
                _mixerName = value;
                if (_mixerName.Length > 0 && MixerNames.Contains(_mixerName))
                {
                    var val  = AddCementRecordViewModel.mixerList.Where(x => x.mixerName == MixerName).Select(g => new { g.currentCementLevel, g.mixerID } ).First();
                    mixerID = val.mixerID;
                    previouslyRemainingCement = val.currentCementLevel.ToString();
                }
                else
                {
                    previouslyRemainingCement = "";
                }
                OnPropertyChanged(MixerName);
                OnPropertyChanged(previouslyRemainingCement);
            }
        }
        private int _mixerID;
        [JsonProperty]
        public int mixerID
        {
            get => _mixerID;
            set
            {
                _mixerID = value;
                OnPropertyChanged();
            }
        }
        private string _remaniningCement;
        [JsonProperty]
        public string remaniningCement
        {
            get => _remaniningCement;
            set { 
                _remaniningCement = value;
                if (importedCement != null)
                {
                    double previouslyRemainingCementDouble;
                    double importedCementDouble;
                    double remainingCementDouble;
                    if (_mixerName.Length > 0 && double.TryParse(previouslyRemainingCement, out previouslyRemainingCementDouble) && double.TryParse(importedCement, out importedCementDouble) && double.TryParse(remaniningCement, out remainingCementDouble))
                    {
                        consumedCement = $"{(decimal)previouslyRemainingCementDouble + (decimal)importedCementDouble - (decimal)remainingCementDouble}";
                    }
                    else
                    {
                        _consumedCement = "";
                    }
                    OnPropertyChanged(consumedCement);
                }
                OnPropertyChanged(remaniningCement);
            }

        }

        private string _previouslyRemainingCement;
        [JsonProperty]
        public string previouslyRemainingCement
        {
            get => _previouslyRemainingCement;
            set { _previouslyRemainingCement = value; OnPropertyChanged(); }
        }

        private string _consumedCement;
        [JsonProperty]
        public string consumedCement
        {
            get => _consumedCement;
            set { _consumedCement = value; OnPropertyChanged(); }
        }

        private string _importedCement;
        [JsonProperty]
        public string importedCement
        {
            get => _importedCement;
            set { 
                _importedCement = value;
                double previouslyRemainingCementDouble;
                double importedCementDouble;
                double remainingCementDouble;
                if (_mixerName.Length > 0 && double.TryParse(previouslyRemainingCement, out previouslyRemainingCementDouble) && double.TryParse(importedCement, out importedCementDouble) && double.TryParse(remaniningCement, out remainingCementDouble) )
                {
                    consumedCement = $"{(decimal)previouslyRemainingCementDouble + (decimal)importedCementDouble - (decimal)remainingCementDouble}";
                }
                else
                {
                    _consumedCement = "";
                }
                OnPropertyChanged(importedCement); 
                OnPropertyChanged(consumedCement);
            }
        }

        public ObservableCollection<string> MixerNames { get; set; }

        
        public CementRecord()
        {
            MixerNames = new ObservableCollection<string>(AddCementRecordViewModel.mixerList.Select(x => x.mixerName).ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
