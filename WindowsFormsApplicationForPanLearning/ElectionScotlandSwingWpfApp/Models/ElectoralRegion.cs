using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;


namespace ElectionScotlandSwingWpfApp.Models
{

    [XmlType("ElectoralRegion")] // define Type
    [XmlInclude(typeof(ConstituencySeat))] // include type class ConstituencySeat
    [XmlInclude(typeof(AdditionalListSeat))] // include type class AdditionalListSeat
    public class ElectoralRegion
    {
        #region Public Properties

        [XmlElement]
        public string Name { get; set; }

        [XmlIgnore]
        public int TotalNumberOfSeats { get; private set; }

        [XmlArray("ConstituencySeats")]
        [XmlArrayItem("ConstituencySeat")]
        public List<ConstituencySeat> ConstituencySeats { get; private set; }

        [XmlArray("ListSeats")]
        [XmlArrayItem("AdditionalListSeat")]
        public List<AdditionalListSeat> ListSeats { get; private set; }


        [XmlIgnore]
        public const int NumberListSeats = 7;

        #endregion

        #region Constructor

        public ElectoralRegion()
        {
            Name = "Not set";
            TotalNumberOfSeats = 0;
            ConstituencySeats = new List<ConstituencySeat>();
            ListSeats = new List<AdditionalListSeat>();
        }

        #endregion

        #region Public Methods

        public void SetupListSeats()
        {
            // first populate the regional Lists per party
            var votesPerParty = new Dictionary<string, int>();
            foreach (var seat in ConstituencySeats)
            {
                foreach(var partyVote in seat.PartyListVotes)
                {
                    if (votesPerParty.ContainsKey(partyVote.Key))
                        votesPerParty[partyVote.Key] += partyVote.Value;
                    else
                        votesPerParty.Add(partyVote.Key, partyVote.Value);
                }
            }

            // initialise the seats per party
            var seatsPerParty = new Dictionary<string, int>();
            foreach (string party in votesPerParty.Keys)
                seatsPerParty.Add(party, 0);
            foreach (var seat in ConstituencySeats)
                if (seatsPerParty.ContainsKey(seat.Victor.Party))
                    seatsPerParty[seat.Victor.Party]++;

            // then add them in 
            ListSeats = new List<AdditionalListSeat>();
            for (int i = 0; i < NumberListSeats; i++ )
                CalculateListSeat(i, ref votesPerParty, ref seatsPerParty);
        }

        #endregion

        #region Utility Functions

        private void CalculateListSeat(int i, 
            ref Dictionary<string, int> votesPerParty,
            ref Dictionary<string, int> seatsPerParty)
        {
            // make up the new seat 
            AdditionalListSeat newSeat = new AdditionalListSeat()
            {
                Name = "Additional Seat " + (1 + i),
                Candidates = new List<ConstituencyCandidate>()
            };

            // populate the D'Hondt totals for each candidate
            foreach (var party in votesPerParty.Keys.ToList())
            {
                int totalVotes = votesPerParty[party];
                int divisor = (1+ seatsPerParty[party]);
                int dHondtVote = totalVotes / divisor;
                newSeat.Candidates.Add(
                    new ConstituencyCandidate() { Party = party, Name = party, VotesFor = dHondtVote });
            }
            
            //add it to the list
            ListSeats.Add(newSeat);

            // update the seat totals
            seatsPerParty[newSeat.Victor.Party]++;
        }

        #endregion
    }
}
