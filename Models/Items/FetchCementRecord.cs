using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models.Items
{
    public class FetchCementRecord
    {
        public string   mixerName { get; set; }
        public DateTime recordDate { get; set; }
        public double   consumedAmount { get; set; }
        public double   importedAmount { get; set; }
        public double   remainingAmount { get; set; }
    }
}
