using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class ConcreteProductionRecord
    {
        public int      recordID                { get; set; }
        public DateTime recordDate              { get; set; }
        public string   company                 { get; set; }
        public string   project                 { get; set; }
        public double   producedConcreteAmount  { get; set; }
        public bool     isReinforcedConcrete    { get; set; }
        public bool     isInformal              { get; set; }
        public int      mixerID                 { get; set; }
        public Mixer    mixer                   { get; set; }
    }
}
