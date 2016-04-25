using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class ConstituencyCandidate
    {
        public string Name { get; set; }
        public int VotesFor { get; set; }
        public double PercentageVote { get; set; }
        public string Party { get; set; }


        public ConstituencyCandidate()
        {
            VotesFor = 0;
            PercentageVote = 0.0;
            Name = Party = "Not set";
        }
    }
}
