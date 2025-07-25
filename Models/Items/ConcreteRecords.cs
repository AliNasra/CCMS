using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Services;
using WpfApp2.ViewModels.Concrete;
using WpfApp2.Utilities;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows;
namespace WpfApp2.Models.Items
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConcreteRecords : INotifyPropertyChanged
    {
        
        private string _project;
        [JsonProperty]
        public string project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        private string _mixerName;
        [JsonProperty]
        public string mixerName
        {
            get => _mixerName;
            set
            {
                _mixerName = value;
                if (_mixerName.Length > 0 && mixerNames.Contains(_mixerName))
                {
                    _mixerID = AddConcreteRecordViewModel.mixerList.Where(x => x.mixerName == mixerName).Select(g => g.mixerID).First();
                }
                OnPropertyChanged();
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

        private string _company;
        [JsonProperty]
        public string company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged();
            }
        }
        

        private string _isReinforced;
        [JsonProperty]
        public string isReinforced
        {
            get => _isReinforced;
            set
            {
                _isReinforced = value;
                OnPropertyChanged();
            }
        }

        private string _concreteAmount;
        [JsonProperty]
        public string concreteAmount
        {
            get => _concreteAmount;
            set
            {
                _concreteAmount = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _mixerNames;
        public ObservableCollection<string> mixerNames
        {
            get => _mixerNames;
            set
            {
                _mixerNames = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _companyList;
        public ObservableCollection<string> companyList
        {
            get => _companyList;
            set
            {
                _companyList = value;
                OnPropertyChanged();
            }
        }

        private bool _isInformal;
        [JsonProperty]
        public bool isInformal
        {
            get => _isInformal;
            set
            {
                _isInformal = value;
                OnPropertyChanged();
            }
        }

        private string _companyFilteringText;
        public string companyFilteringText
        {
            get => _companyFilteringText;
            set
            {
                _companyFilteringText = value;
                if (!string.IsNullOrWhiteSpace(_companyFilteringText))
                {
                    companyList = new ObservableCollection<string>(AppSettings.getSettings().companyList.Where(x => x.Contains(_companyFilteringText)).ToList());
                }
                else
                {
                    companyList = new ObservableCollection<string>(AppSettings.getSettings().companyList);
                }
                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> _concreteOptions;
        public ObservableCollection<string> ConcreteOptions
        {
            get => _concreteOptions;
            set
            {
                _concreteOptions = value;
                OnPropertyChanged();
            }
        }
        


        public ConcreteRecords()
        {
            ConcreteOptions = new ObservableCollection<string>(new List<string>{ "مسلحة", "عادية" });
            mixerNames = new ObservableCollection<string>(AddConcreteRecordViewModel.mixerList.Select(g => g.mixerName).ToList());
            companyList = new ObservableCollection<string>(new List <string>(AppSettings.getSettings().companyList));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
