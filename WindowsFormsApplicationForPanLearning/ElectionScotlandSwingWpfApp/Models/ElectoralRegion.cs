using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class ElectoralRegion
    {
        #region Public Properties

        public string Name { get; set; }
        public int TotalNumberOfSeats { get; set; }
        public List<ConstituencySeat> ConstituencySeats { get; set; }
        public Dictionary<string, int> VotesPerParty { get; set; }
        public Dictionary<string, int> SeatsPerParty { get; set; }
        

        public const int NumberListSeats = 7;

        #endregion

        #region Constructor

        public ElectoralRegion()
        {
            Name = "Not set";
            TotalNumberOfSeats = 0;
            ConstituencySeats = new List<ConstituencySeat>();
            VotesPerParty = new Dictionary<string, int>();
            SeatsPerParty = new Dictionary<string, int>();
        }

        #endregion

        public void SetupListSeats()
        {
            // first populate the regional Lists per party
            VotesPerParty = new Dictionary<string, int>();
            foreach (var seat in ConstituencySeats)
            {
                foreach(var partyVote in seat.PartyListVotes)
                {
                    if (VotesPerParty.ContainsKey(partyVote.Key))
                        VotesPerParty[partyVote.Key] += partyVote.Value;
                    else
                        VotesPerParty.Add(partyVote.Key, partyVote.Value);
                }
            }

            // initialise the seats per party
            SeatsPerParty = new Dictionary<string, int>();
            foreach (string party in VotesPerParty.Keys)
                SeatsPerParty.Add(party, 0);
            foreach (var seat in ConstituencySeats)
                if (SeatsPerParty.ContainsKey(seat.Victor.Party))
                    SeatsPerParty[seat.Victor.Party]++;

        }
    }
}
