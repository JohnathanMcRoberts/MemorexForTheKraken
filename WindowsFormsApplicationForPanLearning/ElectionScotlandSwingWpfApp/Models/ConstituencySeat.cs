using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class ConstituencySeat
    {
        #region Public Properties

        public string Name { get; set; }
        public int TotalVotesCast { get; set; }
        public int TotalElectorate { get; set; }
        public List<ConstituencyCandidate> Candidates { get; set; }

        public Dictionary<string, int> PartyListVotes { get; set; }

        #endregion

        #region Derived get only properties

        public ConstituencyCandidate Victor
        {
            get { if (_victor == null) GetVictor(); return _victor; }
        }
        public int MajorityVotes
        {
            get { if (_majorityVotes < 0) GetVictor(); return _majorityVotes; }
        }
        public double MajorityPercentage
        {
            get
            {
                if (TotalVotesCast != 0)
                    return ((double)MajorityVotes * 100.0) / (double)TotalVotesCast;

                return 0.0;
            }
        }
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
        
        public ConstituencySeat()
        {
            Name = "Not set";
            TotalVotesCast = 0;
            _majorityVotes = -1;
            Candidates = new List<ConstituencyCandidate>();
            PartyListVotes = new Dictionary<string, int>();
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

    }
}
