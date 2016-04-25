using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class ElectionResult
    {
        public string Name { get; set; }
        public List<ElectoralRegion> Regions { get; set; }

        public ElectionResult()
        {
            Regions = new List<ElectoralRegion>();
        }
    }
}
