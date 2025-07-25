using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Unit
    {
        public int                             unitID                  { get; set; }
        public string                          unitDesignation         { get; set; }
        public int                             unitCode                { get; set; }
        public string                          unitSpecialization      { get; set; }
        public List<FuelDepot>                 unitFuelDepots          { get; set; }
        public List<PreCastWallProgressRecord> progressRecords         { get; set; }
        public int                             selfSufficienyReserve   { get; set; }
        public int                             benzine80Reserve        { get; set; }
        public int                             summerDieselReserve     { get; set; }
        public int                             preCastWallTarget       { get; set; }
        public bool                            isOperational           { get; set; }


    }
}
