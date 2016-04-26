using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class PollingPrediction
    {
        public Dictionary<string, double> PartyPercentages { get; set; }

        public PollingPrediction()
        {
            PartyPercentages = new Dictionary<string, double>();
        }
    }
}
