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
    public class ElectoralRegion : ICloneable
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

        #region Derived get only properties

        #region Seats By Party

        [XmlIgnore]
        public Dictionary<string,int> ConstituencySeatsByParty
        {
            get 
            {
                Dictionary<string, int> constSeats = new Dictionary<string, int>();

                foreach (var seat in ConstituencySeats)
                {
                    if (constSeats.ContainsKey(seat.Victor.Party))
                        constSeats[seat.Victor.Party]++;
                    else
                        constSeats.Add(seat.Victor.Party, 1);
                }

                return constSeats; 
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ListSeatsByParty
        {
            get
            {
                Dictionary<string, int> addSeats = new Dictionary<string, int>();

                foreach (var seat in ListSeats)
                {
                    if (addSeats.ContainsKey(seat.Victor.Party))
                        addSeats[seat.Victor.Party]++;
                    else
                        addSeats.Add(seat.Victor.Party, 1);
                }

                return addSeats;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> SeatsByParty
        {
            get
            {
                Dictionary<string, int> seatsByParty = 
                    new Dictionary<string, int>(ListSeatsByParty);

                foreach (var party in ConstituencySeatsByParty.Keys)
                {
                    if (seatsByParty.ContainsKey(party))
                        seatsByParty[party] += ConstituencySeatsByParty[party];
                    else
                        seatsByParty.Add(party, ConstituencySeatsByParty[party]);

                }

                return seatsByParty;
            }
        }

        #endregion

        #region Votes By Party and overall

        [XmlIgnore]
        public Dictionary<string, int> ConstituencyVotesByParty
        {
            get
            {
                Dictionary<string, int> constVotes = new Dictionary<string, int>();

                foreach (var seat in ConstituencySeats)
                {
                    foreach(var candidate in seat.Candidates)
                    {
                        if (constVotes.ContainsKey(candidate.Party))
                            constVotes[candidate.Party] += candidate.VotesFor;
                        else
                            constVotes.Add(candidate.Party, candidate.VotesFor);                    
                    }
                }

                return constVotes;
            }
        }

        [XmlIgnore]
        public int TotalConstituencyVotes
        {
            get
            {
                int total = 0;
                foreach (var party in ConstituencyVotesByParty.Keys)
                    total += ConstituencyVotesByParty[party];
                return total;
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> ConstituencyPercentagesByParty
        {
            get
            {
                double multiplier = 100.0 / (double)TotalConstituencyVotes;
                Dictionary<string, double> constPercentages = new Dictionary<string, double>();

                foreach (var party in ConstituencyVotesByParty.Keys)
                    constPercentages.Add(party, ConstituencyVotesByParty[party] * multiplier);

                return constPercentages;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ListVotesByParty
        {
            get
            {
                Dictionary<string, int> listVotes = new Dictionary<string, int>();

                foreach (var seat in ConstituencySeats)
                {
                    foreach (var party in seat.PartyListVotes.Keys)
                    {
                        if (listVotes.ContainsKey(party))
                            listVotes[party] += seat.PartyListVotes[party];
                        else
                            listVotes.Add(party, seat.PartyListVotes[party]);
                    }
                }

                return listVotes;
            }
        }

        [XmlIgnore]
        public int TotalListVotes
        {
            get
            {
                int total = 0;
                foreach (var party in ListVotesByParty.Keys)
                    total += ListVotesByParty[party];
                return total;
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> ListPercentagesByParty
        {
            get
            {
                double multiplier = 100.0 / (double)TotalListVotes;
                Dictionary<string, double> listPercentages = new Dictionary<string, double>();

                foreach (var party in ListVotesByParty.Keys)
                    listPercentages.Add(party, ListVotesByParty[party] * multiplier);

                return listPercentages;
            }
        }

        #endregion

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
      
        public void ApplyPartySwingToList(string party, double percentageSwing)
        {
            foreach (var seat in ConstituencySeats)
                seat.ApplyPartySwingToList(party, percentageSwing);
            SetupListSeats();
        }

        public void ApplyPartySwingsToList(Dictionary<string,double> partySwings)
        {
            foreach (var partySwing in partySwings)
            {
                foreach (var seat in ConstituencySeats)
                    seat.ApplyPartySwingToList(partySwing.Key, partySwing.Value);
            }

            SetupListSeats();
        }

        public void ApplyPartySwingToConstituencyCandidates(string party, double percentageSwing)
        {
            foreach (var seat in ConstituencySeats)
                seat.ApplyPartySwingToConstituencyCandidates(party, percentageSwing);
            SetupListSeats();
        }

        public void ApplyPartySwingsToConstituencyCandidates(Dictionary<string, double> partySwings)
        {
            foreach (var partySwing in partySwings)
            {
                foreach (var seat in ConstituencySeats)
                    seat.ApplyPartySwingToConstituencyCandidates(partySwing.Key, partySwing.Value);
            }

            SetupListSeats();
        }

        public void ApplyPartySwings(Dictionary<string, double> partyListSwings, 
            Dictionary<string, double> partyConstituencySwings)
        {
            foreach (var partySwing in partyConstituencySwings)
            {
                foreach (var seat in ConstituencySeats)
                    seat.ApplyPartySwingToConstituencyCandidates(partySwing.Key, partySwing.Value);
            }

            foreach (var partySwing in partyListSwings)
            {
                foreach (var seat in ConstituencySeats)
                    seat.ApplyPartySwingToList(partySwing.Key, partySwing.Value);
            }

            SetupListSeats();
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

        #region ICloneable

        public object Clone()
        {
            var cloned = new ElectoralRegion()
            {
                Name = this.Name,
                TotalNumberOfSeats = this.TotalNumberOfSeats,
                ConstituencySeats = new List<ConstituencySeat>(),
                ListSeats = new List<AdditionalListSeat>()
            };
            foreach (var constituencySeat in ConstituencySeats)
                cloned.ConstituencySeats.Add((ConstituencySeat)constituencySeat.Clone());
            foreach (var listSeat in ListSeats)
                cloned.ListSeats.Add((AdditionalListSeat)listSeat.Clone());

            return cloned;
        }

        #endregion

    }
}
