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
using WpfApp2.ViewModels.Cement;
using WpfApp2.ViewModels.Concrete;

namespace WpfApp2.Models.Items
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PreCastWallRecord : INotifyPropertyChanged
    {
        private string _unitName;
        [JsonProperty]
        public string unitName
        {
            get => _unitName;
            set
            {
                _unitName = value;
                if (!string.IsNullOrWhiteSpace(_unitName) && unitNames.Where(x=>x == _unitName).Any())
                {
                    var nameComponent = unitName.Split();
                    string unitDesignation = nameComponent[0].Trim();
                    int unitCode = int.Parse(nameComponent[1]);
                    string unitSpecialization = nameComponent[2].Trim();
                    Unit unit = AddWallRecordViewModel.unitList.Where(x=>x.unitCode == unitCode && x.unitDesignation == unitDesignation && x.unitSpecialization == unitSpecialization).FirstOrDefault();
                    unitID = unit.unitID;
                    plannedLength = unit.preCastWallTarget.ToString();
                }
                else
                {
                    plannedLength = "";
                }
                OnPropertyChanged();
                OnPropertyChanged(plannedLength);
            }
        }
        private int _unitID;
        [JsonProperty]
        public int unitID
        {
            get => _unitID;
            set
            {
                _unitID = value;
                OnPropertyChanged();
            }
        }

        private string _plannedLength;
        [JsonProperty]
        public string plannedLength
        {
            get => _plannedLength;
            set
            {
                _plannedLength = value;
                OnPropertyChanged();
            }

        }

        private string _previouslyAccomplished;
        [JsonProperty]
        public string previouslyAccomplished
        {
            get => _previouslyAccomplished;
            set
            {
                _previouslyAccomplished = value;
                if (_accomplishedToday!= null)
                {
                    int plannedLengthInt;
                    int previouslyAccomplishedInt;
                    int accomplishedTodayInt;
                    if (unitName.Length > 0 && int.TryParse(plannedLength, out plannedLengthInt) == true && int.TryParse(previouslyAccomplished, out previouslyAccomplishedInt) == true && int.TryParse(accomplishedToday, out accomplishedTodayInt) == true)
                    {
                        toBeAccomplished  = $"{plannedLengthInt - previouslyAccomplishedInt - accomplishedTodayInt}";
                        totalAccomplished = $"{previouslyAccomplishedInt + accomplishedTodayInt}";
                    }
                    else
                    {
                        toBeAccomplished  = "";
                        totalAccomplished = "";
                    }
                    OnPropertyChanged(toBeAccomplished);
                    OnPropertyChanged(totalAccomplished);
                }
                OnPropertyChanged(previouslyAccomplished);
            }

        }

        private string _accomplishedToday;
        [JsonProperty]
        public string accomplishedToday
        {
            get => _accomplishedToday;
            set
            {
                _accomplishedToday = value;
                if (_previouslyAccomplished != null)
                {
                    int plannedLengthInt;
                    int previouslyAccomplishedInt;
                    int accomplishedTodayInt;
                    if (unitName.Length > 0 && int.TryParse(plannedLength, out plannedLengthInt) == true && int.TryParse(previouslyAccomplished, out previouslyAccomplishedInt) == true && int.TryParse(accomplishedToday, out accomplishedTodayInt) == true)
                    {
                        toBeAccomplished = $"{plannedLengthInt - previouslyAccomplishedInt - accomplishedTodayInt}";
                        totalAccomplished = $"{previouslyAccomplishedInt + accomplishedTodayInt}";
                    }
                    else
                    {
                        toBeAccomplished = "";
                        totalAccomplished = "";
                    }
                    OnPropertyChanged(toBeAccomplished);
                    OnPropertyChanged(totalAccomplished);           
                }
                OnPropertyChanged(accomplishedToday);
            }
        }

        private string _totalAccomplished;
        [JsonProperty]
        public string totalAccomplished
        {
            get => _totalAccomplished;
            set
            {
                _totalAccomplished = value;
                OnPropertyChanged();
            }

        }

        private string _toBeAccomplished;
        [JsonProperty]
        public string toBeAccomplished
        {
            get => _toBeAccomplished;
            set
            {
                _toBeAccomplished = value;
                OnPropertyChanged();
            }

        }


        private string _transportedAmountToday;
        [JsonProperty]
        public string transportedAmountToday
        {
            get => _transportedAmountToday;
            set
            {
                _transportedAmountToday = value;
                if (previouslyTransported != null)
                {
                    int previouslyTransportedInt;
                    int transportedTodayInt;
                    if (unitName.Length > 0 && int.TryParse(previouslyTransported, out previouslyTransportedInt) == true && int.TryParse(transportedAmountToday, out transportedTodayInt) == true)
                    {
                        totalTransported = $"{previouslyTransportedInt + transportedTodayInt}";
                    }
                    else
                    {
                        totalTransported = "";
                    }
                    OnPropertyChanged(totalTransported);
                }
                OnPropertyChanged(transportedAmountToday);
            }

        }

        private string _previouslyTransported;
        [JsonProperty]
        public string previouslyTransported
        {
            get => _previouslyTransported;
            set
            {
                _previouslyTransported = value;
                if (transportedAmountToday != null)
                {
                    int previouslyTransportedInt;
                    int transportedTodayInt;
                    if (unitName.Length > 0 && int.TryParse(previouslyTransported, out previouslyTransportedInt) == true && int.TryParse(transportedAmountToday, out transportedTodayInt) == true)
                    {
                        totalTransported = $"{previouslyTransportedInt + transportedTodayInt}";
                    }
                    else
                    {
                        totalTransported = "";
                    }
                    OnPropertyChanged(totalTransported);
                }
                OnPropertyChanged(previouslyTransported);
            }
        }


        private string _totalTransported;
        [JsonProperty]
        public string totalTransported
        {
            get => _totalTransported;
            set
            {
                _totalTransported = value;
                OnPropertyChanged();
            }

        }

        private string _remainingOnSite;
        [JsonProperty]
        public string remainingOnSite
        {
            get => _remainingOnSite;
            set
            {
                _remainingOnSite = value;
                OnPropertyChanged();
            }

        }



        public ObservableCollection<string> unitNames { get; set; }


        public PreCastWallRecord()
        {
            unitNames = new ObservableCollection<string>(AddWallRecordViewModel.unitList.Select(x => $"{x.unitDesignation} {x.unitCode} {x.unitSpecialization}").ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

