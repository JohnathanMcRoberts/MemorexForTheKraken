using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace ElectionScotlandSwingWpfApp.Models
{
    [XmlType("ParliamentarySeat")] // define Type
    [XmlInclude(typeof(ConstituencyCandidate))] 
    public class ParliamentarySeat : ICloneable
    {
        #region Public Properties

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public int TotalVotesCast { get; set; }

        [XmlElement]
        public int TotalElectorate { get; set; }

        [XmlArray("Candidates")]
        [XmlArrayItem("ConstituencyCandidate")]
        public List<ConstituencyCandidate> Candidates { get; set; }

        #endregion

        #region Derived get only properties

        [XmlIgnore]
        public ConstituencyCandidate Victor
        {
            get { if (_victor == null) GetVictor(); return _victor; }
        }
        
        [XmlIgnore]
        public int MajorityVotes
        {
            get { if (_majorityVotes < 0) GetVictor(); return _majorityVotes; }
        }
        
        [XmlIgnore]
        public double MajorityPercentage
        {
            get
            {
                if (TotalVotesCast != 0)
                    return ((double)MajorityVotes * 100.0) / (double)TotalVotesCast;

                return 0.0;
            }
        }
        
        [XmlIgnore]
        public double TurnoutPercentage
        {
            get 
            { 
                if (TotalElectorate != 0) 
                    return ((double)TotalVotesCast * 100.0) / (double)TotalElectorate;

                return 0.0;
            }
        }

        #endregion

        #region Private Fields

        private int _majorityVotes;
        private ConstituencyCandidate _victor;

        #endregion

        #region Constructor

        public ParliamentarySeat()
        {
            Name = "Not set";
            TotalVotesCast = 0;
            _majorityVotes = -1;
            Candidates = new List<ConstituencyCandidate>();
        }
        
        #endregion

        #region Utility Functions
        
        public void GetVictor()
        {
            if (Candidates == null || Candidates.Count == 0)
            {
                _majorityVotes = -1;
                Candidates = new List<ConstituencyCandidate>();
                return;
            }
            
            // get the total votes
            TotalVotesCast =  (from c in Candidates select c.VotesFor).ToList().Sum();

            // get the winner
            var orderedCandidates = 
                (from c in Candidates orderby c.VotesFor descending select c).ToList();
            _victor = orderedCandidates[0];

            // get the majority
            if (orderedCandidates.Count < 2)
                _majorityVotes = _victor.VotesFor;
            else
            {
                var second = orderedCandidates[1];
                _majorityVotes = _victor.VotesFor - second.VotesFor;
            }

            // finally set up the percentages
            foreach(var c in Candidates)
                c.PercentageVote = (100.0 * (double)c.VotesFor)/((double)TotalVotesCast);

        }
        
        #endregion

        #region ICloneable

        public virtual object Clone()
        {
            var cloned = new ParliamentarySeat()
            {
                Name = this.Name,
                TotalVotesCast = this.TotalVotesCast,
                TotalElectorate = this.TotalElectorate,
                Candidates = new List<ConstituencyCandidate>()
            };

            foreach (var candidate in Candidates)
                cloned.Candidates.Add((ConstituencyCandidate)candidate.Clone());

            return cloned;
        }

        #endregion

        #region Public Methods

        public void ApplyPartySwingToConstituencyCandidates(
            string party, double percentageSwing)
        {
            int voteSwing = (int)Math.Round(((double)TotalVotesCast * percentageSwing) / 100.0);

            foreach (var candidate in Candidates)
            {
                if (candidate.Party == party)
                {
                    int newTotal = candidate.VotesFor + voteSwing;
                    if (newTotal < 0) newTotal = 0;
                    candidate.VotesFor = newTotal;
                    break;
                }
            }
        }

        #endregion
    }
}
