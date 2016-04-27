using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Data;

using ElectionScotlandSwingWpfApp.ViewModels.Utilities;
using ElectionScotlandSwingWpfApp.Models;

namespace ElectionScotlandSwingWpfApp.ViewModels
{
    public class DataGridsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private Models.MainModel _mainModel;
        private MainViewModel _parent;

        
        #endregion

        #region Constructor

        public DataGridsViewModel(MainWindow mainWindow, log4net.ILog log, 
            Models.MainModel mainModel, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = mainViewModel;

            ConstituencySeats = new ObservableCollection<ConstituencySeatResult>();
            ListSeats = new ObservableCollection<ListSeatResult>();
            PartyConstituencyResults = new ObservableCollection<NationalPartyResult>();
            PartyListResults = new ObservableCollection<NationalPartyResult>();

            PredictedConstituencySeats = new ObservableCollection<ConstituencySeatResult>();
            PredictedListSeats = new ObservableCollection<ListSeatResult>();
            PredictedPartyConstituencyResults = new ObservableCollection<NationalPartyResult>();
            PredictedPartyListResults = new ObservableCollection<NationalPartyResult>();
        }

        #endregion

        #region Nested Classes

        public class ConstituencySeatPartyResult
        {
            public string Name {get;set;}

            public int Votes { get; set; }

            public double Percentage { get; set; }

        }
        
        public class SeatResult
        {
            public string Region { get; set; }

            public string WinningParty { get; set; }

            public int Majority { get; set; }

            public double MajorityPercentage { get; set; }

            public ObservableCollection<ConstituencySeatPartyResult> Parties { get; set; }
        }

        public class ConstituencySeatResult : SeatResult
        {
            public string Constituency {get;set;}
        
            public int Electorate {get;set;}
            
            public double TurnoutPercentage {get;set;}
        }

        public class ListSeatResult : SeatResult
        {
            public string Name { get; set; }

        }

        public class NationalPartyResult
        {
            public string Name { get; set; }
            public double Percentage { get; set; }
            public int Votes { get; set; }
            public int Seats { get; set; }
        }

        #endregion

        #region Public Data

        public ObservableCollection<ConstituencySeatResult> ConstituencySeats { get; set; }

        public ObservableCollection<ListSeatResult> ListSeats { get; set; }

        public ObservableCollection<NationalPartyResult> PartyConstituencyResults { get; set; }

        public ObservableCollection<NationalPartyResult> PartyListResults { get; set; }


        public ObservableCollection<ConstituencySeatResult> PredictedConstituencySeats { get; set; }

        public ObservableCollection<ListSeatResult> PredictedListSeats { get; set; }

        public ObservableCollection<NationalPartyResult> PredictedPartyConstituencyResults { get; set; }

        public ObservableCollection<NationalPartyResult> PredictedPartyListResults { get; set; }


        #endregion

        #region Public Methods

        public void UpdateData()
        {
            UpdateConstituencySeats();

            UpdateListSeats();

            UpdateNationalSeats();

            OnPropertyChanged("");

        }

        private void UpdateNationalSeats()
        {
            List<NationalPartyResult> results = new List<NationalPartyResult>();

            var electionResult = _mainModel.CurrentResult;

            PartyConstituencyResults.Clear();
            GetNationalConstituencyResults(results, electionResult);          
            foreach(var result in 
                (from res in results orderby res.Percentage descending select res).ToList())
                PartyConstituencyResults.Add(result);

            PartyListResults.Clear();
            results = GetNationalListResults(results, electionResult);
            foreach(var result in 
                (from res in results orderby res.Percentage descending select res).ToList())
                PartyListResults.Add(result);


            electionResult = _mainModel.PredictedResult;

            PredictedPartyConstituencyResults.Clear();
            GetNationalConstituencyResults(results, electionResult);
            foreach (var result in
                (from res in results orderby res.Percentage descending select res).ToList())
                PredictedPartyConstituencyResults.Add(result);

            PredictedPartyListResults.Clear();
            results = GetNationalListResults(results, electionResult);
            foreach (var result in
                (from res in results orderby res.Percentage descending select res).ToList())
                PredictedPartyListResults.Add(result);
        }

        private static List<NationalPartyResult> GetNationalListResults(
            List<NationalPartyResult> results, ElectionResult electionResult)
        {
            results = new List<NationalPartyResult>();
            foreach (var party in electionResult.ListVotesByParty.Keys)
            {
                NationalPartyResult listResult = new NationalPartyResult()
                {
                    Name = party,
                    Percentage = electionResult.ListPercentagesByParty[party],
                    Votes = electionResult.ListVotesByParty[party],
                    Seats = electionResult.ListSeatsByParty.ContainsKey(party) ?
                                electionResult.ListSeatsByParty[party] : 0
                };
                results.Add(listResult);
            }
            return results;
        }

        private static void GetNationalConstituencyResults(
            List<NationalPartyResult> results, ElectionResult electionResult)
        {
            results.Clear();
            foreach (var party in electionResult.ConstituencyVotesByParty.Keys)
            {
                NationalPartyResult constituencyResult = new NationalPartyResult()
                {
                    Name = party,
                    Percentage = electionResult.ConstituencyPercentagesByParty[party],
                    Votes = electionResult.ConstituencyVotesByParty[party],
                    Seats = electionResult.ConstituencySeatsByParty.ContainsKey(party) ?
                                electionResult.ConstituencySeatsByParty[party] : 0
                };

                results.Add(constituencyResult);
            }
        }


        private void UpdateListSeats()
        {
            // first the currect results
            ListSeats.Clear();
            List<ListSeatResult> listSeatResults = new List<ListSeatResult>();
            var regions = _mainModel.CurrentResult.Regions;
            GetListSeatsFromRegions(listSeatResults, regions);
            foreach (var res in listSeatResults)
                ListSeats.Add(res);

            // then the predicted results
            PredictedListSeats.Clear();
            listSeatResults = new List<ListSeatResult>();
            regions = _mainModel.PredictedResult.Regions;
            GetListSeatsFromRegions(listSeatResults, regions);
            foreach (var res in listSeatResults)
                PredictedListSeats.Add(res);
        }

        private static void GetListSeatsFromRegions(
            List<ListSeatResult> listSeatResults, List<ElectoralRegion> regions)
        {
            listSeatResults.Clear();
            foreach (var region in regions)
            {
                foreach (var seat in region.ListSeats)
                {
                    var result = new ListSeatResult()
                    {
                        Name = seat.Name,
                        Region = region.Name,
                        Majority = seat.MajorityVotes,
                        MajorityPercentage = seat.MajorityPercentage,
                        WinningParty = seat.Victor.Party,
                        Parties = new ObservableCollection<ConstituencySeatPartyResult>()

                    };

                    // sort the candidates
                    var candidates =
                        (from c in seat.Candidates orderby c.VotesFor descending select c).ToList();

                    // then add them
                    foreach (var candidate in candidates)
                    {
                        if (candidate.VotesFor < 1) continue;

                        result.Parties.Add(
                            new ConstituencySeatPartyResult()
                            {
                                Name = candidate.Party,
                                Votes = candidate.VotesFor,
                                Percentage = candidate.PercentageVote
                            });
                    }
                    listSeatResults.Add(result);
                }
            }
        }


        private void UpdateConstituencySeats()
        {
            ConstituencySeats.Clear();
            List<ConstituencySeatResult> constituencySeatResults =
                new List<ConstituencySeatResult>();
            var regions = _mainModel.CurrentResult.Regions;
            GetConstituencySeatsFromRegions(constituencySeatResults, regions);
            foreach (var res in constituencySeatResults)
                ConstituencySeats.Add(res);

            PredictedConstituencySeats.Clear();
            constituencySeatResults =
                new List<ConstituencySeatResult>();
            regions = _mainModel.CurrentResult.Regions;
            GetConstituencySeatsFromRegions(constituencySeatResults, regions);
            foreach (var res in constituencySeatResults)
                PredictedConstituencySeats.Add(res);

        }

        private static void GetConstituencySeatsFromRegions(
            List<ConstituencySeatResult> constituencySeatResults, List<ElectoralRegion> regions)
        {
            constituencySeatResults.Clear();
            foreach (var region in regions)
            {
                foreach (var seat in region.ConstituencySeats)
                {
                    var result = new ConstituencySeatResult()
                    {
                        Constituency = seat.Name,
                        Region = region.Name,
                        Electorate = seat.TotalElectorate,
                        Majority = seat.MajorityVotes,
                        MajorityPercentage = seat.MajorityPercentage,
                        TurnoutPercentage = seat.TurnoutPercentage,
                        WinningParty = seat.Victor.Party,
                        Parties = new ObservableCollection<ConstituencySeatPartyResult>()

                    };

                    // sort the candidates
                    var candidates =
                        (from c in seat.Candidates orderby c.VotesFor descending select c).ToList();

                    // then add them
                    foreach (var candidate in seat.Candidates)
                    {
                        result.Parties.Add(
                            new ConstituencySeatPartyResult()
                            {
                                Name = candidate.Party,
                                Votes = candidate.VotesFor,
                                Percentage = candidate.PercentageVote
                            });
                    }
                    constituencySeatResults.Add(result);

                }
            }
        }

        #endregion

    }
}
