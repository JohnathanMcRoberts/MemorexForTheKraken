using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class Region
    {
        public string Name { get; set; }
        public List<ConstituencySeat> Constituencies { get; set; }

        public int TotalNumberSeats { get; set; }
        public int NumberRegional { get { return TotalNumberSeats - Constituencies.Count; } }

        public MajorPartySet RegionalVote { get; set; }
        public int TotalNumberOfRegionalVotes { get; set; }
    }
}
