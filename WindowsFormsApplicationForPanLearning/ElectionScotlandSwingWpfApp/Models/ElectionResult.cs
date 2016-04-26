using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace ElectionScotlandSwingWpfApp.Models
{
    [XmlType("ElectionResult")] // define Type
    [XmlInclude(typeof(ElectoralRegion))] // include type class AdditionalListSeat
    public class ElectionResult
    {
        #region Public Properties
        [XmlElement]
        public string Name { get; set; }

        [XmlArray("Regions")]
        [XmlArrayItem("ElectoralRegion")]
        public List<ElectoralRegion> Regions { get; set; }

        #endregion

        #region Derived Properties

        [XmlIgnore]
        public Dictionary<string, int> SeatsByParty
        {
            get
            {
                Dictionary<string, int> seatsByParty = new Dictionary<string, int>();
                foreach (var region in Regions)
                {
                    foreach (var party in region.SeatsByParty.Keys)
                    {
                        if (seatsByParty.ContainsKey(party))
                            seatsByParty[party] += region.SeatsByParty[party];
                        else
                            seatsByParty.Add(party, region.SeatsByParty[party]);
                    }
                }
                return seatsByParty;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ListVotesByParty
        {
            get
            {
                Dictionary<string, int> votesByParty = new Dictionary<string, int>();
                foreach (var region in Regions)
                {
                    foreach (var party in region.ListVotesByParty.Keys)
                    {
                        if (votesByParty.ContainsKey(party))
                            votesByParty[party] += region.ListVotesByParty[party];
                        else
                            votesByParty.Add(party, region.ListVotesByParty[party]);
                    }
                }
                return votesByParty;
            }
        }

        [XmlIgnore]
        public int TotalListVotes
        {
            get
            {
                return ListVotesByParty.Values.ToList().Sum();
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> ListPercentagesByParty
        {
            get
            {
                double multiplier = 100.0 / (double)TotalListVotes;
                Dictionary<string, double> percentagesByParty = new Dictionary<string, double>();
                foreach (var partyVote in ListVotesByParty)
                {
                    percentagesByParty.Add(partyVote.Key, partyVote.Value * multiplier);
                }
                return percentagesByParty;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ListSeatsByParty
        {
            get
            {
                Dictionary<string, int> seatsByParty = new Dictionary<string, int>();
                foreach (var region in Regions)
                {
                    foreach (var party in region.ListSeatsByParty.Keys)
                    {
                        if (seatsByParty.ContainsKey(party))
                            seatsByParty[party] += region.ListSeatsByParty[party];
                        else
                            seatsByParty.Add(party, region.ListSeatsByParty[party]);
                    }
                }
                return seatsByParty;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ConstituencyVotesByParty
        {
            get
            {
                Dictionary<string, int> votesByParty = new Dictionary<string, int>();
                foreach (var region in Regions)
                {
                    foreach (var party in region.ConstituencyVotesByParty.Keys)
                    {
                        if (votesByParty.ContainsKey(party))
                            votesByParty[party] += region.ConstituencyVotesByParty[party];
                        else
                            votesByParty.Add(party, region.ConstituencyVotesByParty[party]);
                    }
                }
                return votesByParty;
            }
        }

        [XmlIgnore]
        public int TotalConstituencyVotes
        {
            get
            {
                return ConstituencyVotesByParty.Values.ToList().Sum();
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> ConstituencyPercentagesByParty
        {
            get
            {
                double multiplier = 100.0 / (double)TotalConstituencyVotes;
                Dictionary<string, double> percentagesByParty = new Dictionary<string, double>();
                foreach (var partyVote in ConstituencyVotesByParty)
                {
                    percentagesByParty.Add(partyVote.Key, partyVote.Value * multiplier);
                }
                return percentagesByParty;
            }
        }

        [XmlIgnore]
        public Dictionary<string, int> ConstituencySeatsByParty
        {
            get
            {
                Dictionary<string, int> seatsByParty = new Dictionary<string, int>();
                foreach (var region in Regions)
                {
                    foreach (var party in region.ConstituencySeatsByParty.Keys)
                    {
                        if (seatsByParty.ContainsKey(party))
                            seatsByParty[party] += region.ConstituencySeatsByParty[party];
                        else
                            seatsByParty.Add(party, region.ConstituencySeatsByParty[party]);
                    }
                }
                return seatsByParty;
            }
        }

        #endregion

        #region Constructor

        public ElectionResult()
        {
            Regions = new List<ElectoralRegion>();
        }

        #endregion

        #region Public Methods

        public string SerializeObject()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        #endregion
    }
}
