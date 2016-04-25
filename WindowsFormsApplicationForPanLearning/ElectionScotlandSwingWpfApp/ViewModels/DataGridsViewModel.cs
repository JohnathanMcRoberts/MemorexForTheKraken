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
        }

        #endregion

        #region Nested Classes

        public class ConstituencySeatPartyResult
        {
            public string Name {get;set;}

            public int Votes { get; set; }

            public double Percentage { get; set; }

        }
        
        public class ConstituencySeatResult
        {
            public string Constituency {get;set;}
        
            public string Region {get;set;}
        
            public int Electorate {get;set;}
            
            public double TurnoutPercentage {get;set;}

            public string WinningParty {get;set;}

            public int  Majority {get;set;}
            
            public double  MajorityPercentage{get;set;}

            public ObservableCollection<ConstituencySeatPartyResult> Parties { get; set; }

        }

        #endregion

        #region Public Data

        public ObservableCollection<ConstituencySeatResult> ConstituencySeats { get; set; }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            ConstituencySeats.Clear();

            foreach (var region in _mainModel.CurrentResult.Regions)
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
                    ConstituencySeats.Add(result);

                }
            }

            OnPropertyChanged("");

        }

        #endregion

    }
}
