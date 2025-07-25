using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models.Items
{
    public class FetchConcreteRecord
    {
        public string   mixerName { get; set; }
        public DateTime recordDate { get; set; }
        public string   company { get; set; }
        public string   project { get; set; }
        public double   producedConcreteAmount { get; set; }
        public string   isReinforcedConcrete { get; set; }
    }
}
