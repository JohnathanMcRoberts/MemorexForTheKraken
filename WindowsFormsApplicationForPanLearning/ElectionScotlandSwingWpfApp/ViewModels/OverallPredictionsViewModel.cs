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
    public class OverallPredictionsViewModel : INotifyPropertyChanged
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
        private List<PartyForecast> _partyForecasts;
        private MainViewModel _parent;
        
        #endregion

        #region Constructor

        public OverallPredictionsViewModel(MainWindow mainWindow, log4net.ILog log,
            MainModel mainModel, List<PartyForecast> partyForecasts, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _partyForecasts = partyForecasts;
            _parent = mainViewModel;
        }

        #endregion

        #region Public Data

        #region SNP

        public string NameSNP 
        { 
            get 
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].Name;
            } 
        }
        
        public int PreviousSeatsSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PreviousSeats;
            }
        }

        public int PredictedSeatsSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PredictedSeats;
            }
        }

        public int SwingSeatsSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].SeatsSwing;
            }
        }

        #endregion

        #region Labour

        public string NameLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].Name;
            }
        }
        
        public int PreviousSeatsLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PreviousSeats;
            }
        }

        public int PredictedSeatsLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PredictedSeats;
            }
        }

        public int SwingSeatsLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].SeatsSwing;
            }
        }

        #endregion

        #region Conservative

        public string NameConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].Name;
            }
        }
        
        public int PreviousSeatsConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PreviousSeats;
            }
        }

        public int PredictedSeatsConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PredictedSeats;
            }
        }

        public int SwingSeatsConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].SeatsSwing;
            }
        }

        #endregion

        #region LiberalDemocrat

        public string NameLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].Name;
            }
        }
        
        public int PreviousSeatsLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PreviousSeats;
            }
        }

        public int PredictedSeatsLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PredictedSeats;
            }
        }

        public int SwingSeatsLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].SeatsSwing;
            }
        }

        #endregion

        #region Green

        public string NameGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].Name;
            }
        }

        public int PreviousSeatsGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PreviousSeats;
            }
        }

        public int PredictedSeatsGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PredictedSeats;
            }
        }

        public int SwingSeatsGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].SeatsSwing;
            }
        }

        #endregion

        #region UKIP

        public string NameUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].Name;
            }
        }
        
        public int PreviousSeatsUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PreviousSeats;
            }
        }

        public int PredictedSeatsUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PredictedSeats;
            }
        }

        public int SwingSeatsUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].SeatsSwing;
            }
        }

        #endregion

        #region Totals

        public int PreviousSeatsTotal
        {
            get
            {
                return PreviousSeatsConservative + PreviousSeatsGreen + PreviousSeatsLabour +
                    PreviousSeatsLiberalDemocrat + PreviousSeatsSNP + PreviousSeatsUKIP;
            }
        }

        public int PredictedSeatsTotal
        {
            get
            {
                return PredictedSeatsConservative + PredictedSeatsGreen + PredictedSeatsLabour +
                    PredictedSeatsLiberalDemocrat + PredictedSeatsSNP + PredictedSeatsUKIP;
            }
        }

        public int SwingSeatsTotal
        {
            get
            {
                return SwingSeatsConservative + SwingSeatsGreen + SwingSeatsLabour +
                    SwingSeatsLiberalDemocrat + SwingSeatsSNP + SwingSeatsUKIP;
            }
        }

        #endregion
        
        public List<PartyForecast> PartyForecasts
        {
            get { return _partyForecasts; }
            set { _partyForecasts = value; }
        }
        
        #endregion

        #region Public Methods

        public void UpdateData(List<PartyForecast> partyForecasts)
        {
            _partyForecasts = partyForecasts;

            OnPropertyChanged("");
        }

        #endregion
    }
}
