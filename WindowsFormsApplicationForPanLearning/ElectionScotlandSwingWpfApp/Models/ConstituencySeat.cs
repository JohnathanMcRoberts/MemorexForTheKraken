using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

using ElectionScotlandSwingWpfApp.Utilities;

namespace ElectionScotlandSwingWpfApp.Models
{

    [XmlType("ConstituencySeat")] // define Type
    public class ConstituencySeat : ParliamentarySeat, ICloneable
    {
        #region Public Properties

        public SerializableDictionary<string, int> PartyListVotes { get; set; }

        #endregion

        #region Constructor
        
        public ConstituencySeat()
        {
            PartyListVotes = new SerializableDictionary<string, int>();
        }
        
        #endregion

        #region ICloneable

        public override object Clone()
        {
            ConstituencySeat cloned = new ConstituencySeat()
            {
                Name = this.Name,
                TotalVotesCast = this.TotalVotesCast,
                TotalElectorate = this.TotalElectorate,
                Candidates = new List<ConstituencyCandidate>()
            };

            foreach (var candidate in Candidates)
                cloned.Candidates.Add((ConstituencyCandidate)candidate.Clone());

            cloned.PartyListVotes = new SerializableDictionary<string,int>();
            foreach(var party in PartyListVotes.Keys)
                cloned.PartyListVotes.Add(party, this.PartyListVotes[party]);

            return cloned;
        }

        #endregion

        #region Public Methods

        public void ApplyPartySwingToList(string party, double percentageSwing)
        {
            int ttlListVotes = PartyListVotes.Values.ToList().Sum();
            int voteSwing = (int)Math.Round(((double)ttlListVotes * percentageSwing) / 100.0);
            if (PartyListVotes.ContainsKey(party))
            {
                int newTotal = PartyListVotes[party] + voteSwing;
                if (newTotal < 0) newTotal = 0;
                PartyListVotes[party] = newTotal;
            }
        }

        #endregion
    }
}
