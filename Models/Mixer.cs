using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Mixer : IComparable<Mixer>
    {
        public int                            mixerID                { get; set; }
        public int                            cabbageNo              { get; set; }
        public string                         mixerName              { get; set; }
        public bool                           isOperational          { get; set; }
        public int                            operationalCapacity    { get; set; }
        public double                         currentCementLevel     { get; set; }
        public int                            depotID                { get; set; }
        public FuelDepot                      depot                  { get; set; }
        public List<CementDailyRecord>        cementRecords          { get; set; }
        public List<ConcreteProductionRecord> concreteRecords        { get; set; }

        public int CompareTo (Mixer other)
        {
            return this.cabbageNo.CompareTo(other.cabbageNo);
        }

    }
}
