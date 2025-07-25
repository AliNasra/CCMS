using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class FuelDepot
    {
        public int                         depotID                                { get; set; }
        public int                         depotStorageCapacity                   { get; set; }
        public string                      depotName                              { get; set; }
        public Unit                        unit                                   { get; set; }
        public int                         unitID                                 { get; set; }
        public int                         currentReserve                         { get; set; }
        public int                         LastimportedFuelAmount                 { get; set; }
        public bool                        isOperational                          { get; set; }
        public DateTime                    LastConsignmentDate                    { get; set; }
        public List<Mixer>                 affiliatedMixers                       { get; set; }
        public List<FuelConsumptionRecord> fuelConsumptionRecords                 { get; set; }


    }
}
