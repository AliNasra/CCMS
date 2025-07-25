using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class CementDailyRecord
    {
        public int      recordID         { get; set; }
        public DateTime recordDate       { get; set; }
        public int      mixerID          { get; set; }
        public Mixer    mixer            { get; set; }
        public double   consumedCement   { get; set; }
        public double   importedCement   { get; set; }
        public double   remaniningCement { get; set; }

    }
}
