using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class FuelConsumptionRecord
    {
        public int       recordID           { get; set; }
        public DateTime  recordDate         { get; set; }
        public FuelDepot depot              { get; set; }
        public int       depotID            { get; set; }
        public int       consumedAmount     { get; set; }
        public int       importedAmount     { get; set; }
        public int       fuelLevel          { get; set; }
    }
}
