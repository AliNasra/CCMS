using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models.Items
{
    public class FetchFuelRecord
    {
        public string   depotname       { get; set; }
        public DateTime recordDate      { get; set; }
        public int      consumedAmount  { get; set; }
        public int      importedAmount  { get; set; }
        public int      remainingAmount { get; set; }
    }
}
