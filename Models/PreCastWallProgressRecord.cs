using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class PreCastWallProgressRecord
    {
        public int      recordID               { get; set; }
        public DateTime recordDate             { get; set; }
        public int      unitID                 { get; set; }
        public Unit     unit                   { get; set; }
        public int      previouslyAccomplished { get; set; }
        public int      accomplishedToday      { get; set; }
        public int      toBeAccomplished       { get; set; }
        public int      transportedAmountToday { get; set; }
        public int      previouslyTransported  { get; set; }
        public int      remaningOnSite         { get; set; }

    }
}
