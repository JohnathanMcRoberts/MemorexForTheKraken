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
    public class MajorPartyForecastsViewModel : INotifyPropertyChanged
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

        public MajorPartyForecastsViewModel(MainWindow mainWindow, log4net.ILog log,
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

        public double PreviousSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PreviousPercentage;
            }
        }

        public double PredictedSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedSNP);
                OnPropertyChanged(() => SwingSNP);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingSNP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.SNP].PredictedSwing = value;
                OnPropertyChanged(() => PredictedSNP);
                OnPropertyChanged(() => SwingSNP);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
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

        public double PreviousLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PreviousPercentage;
            }
        }

        public double PredictedLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedLabour);
                OnPropertyChanged(() => SwingLabour);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingLabour
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Labour].PredictedSwing = value;
                OnPropertyChanged(() => PredictedLabour);
                OnPropertyChanged(() => SwingLabour);
                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
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

        public double PreviousConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PreviousPercentage;
            }
        }

        public double PredictedConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedConservative);
                OnPropertyChanged(() => SwingConservative);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingConservative
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Conservative].PredictedSwing = value;
                OnPropertyChanged(() => PredictedConservative);
                OnPropertyChanged(() => SwingConservative);
                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
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

        public double PreviousLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PreviousPercentage;
            }
        }

        public double PredictedLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedLiberalDemocrat);
                OnPropertyChanged(() => SwingLiberalDemocrat);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingLiberalDemocrat
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat].PredictedSwing = value;
                OnPropertyChanged(() => PredictedLiberalDemocrat);
                OnPropertyChanged(() => SwingLiberalDemocrat);
                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
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

        public double PreviousGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PreviousPercentage;
            }
        }

        public double PredictedGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedGreen);
                OnPropertyChanged(() => SwingGreen);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingGreen
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.Green].PredictedSwing = value;
                OnPropertyChanged(() => PredictedGreen);
                OnPropertyChanged(() => SwingGreen);
                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
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

        public double PreviousUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PreviousPercentage;
            }
        }

        public double PredictedUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PredictedPercentage;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PredictedPercentage = value;
                OnPropertyChanged(() => PredictedUKIP);
                OnPropertyChanged(() => SwingUKIP);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        public double SwingUKIP
        {
            get
            {
                return _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PredictedSwing;
            }
            set
            {
                _partyForecasts[(int)MainModel.MajorNamePartyLookup.UKIP].PredictedSwing = value;
                OnPropertyChanged(() => PredictedUKIP);
                OnPropertyChanged(() => SwingUKIP);

                OnPropertyChanged(() => PredictedTotal);
                OnPropertyChanged(() => SwingTotal);
            }
        }

        #endregion

        #region Totals

        public double PreviousTotal
        {
            get
            { 
                return PreviousConservative + PreviousGreen + PreviousLabour + 
                    PreviousLiberalDemocrat + PreviousSNP + PreviousUKIP;
            }
        }

        public double PredictedTotal
        {
            get
            {
                return PredictedConservative + PredictedGreen + PredictedLabour +
                    PredictedLiberalDemocrat + PredictedSNP + PredictedUKIP;
            }
        }

        public double SwingTotal
        {
            get
            {
                return SwingConservative + SwingGreen + SwingLabour +
                    SwingLiberalDemocrat + SwingSNP + SwingUKIP;
            }
        }
        #endregion

        public Dictionary<string,double> PartySwings
        {
            get
            {
                Dictionary<string, double> swings = new Dictionary<string, double>();
                swings.Add(NameSNP, SwingSNP);
                swings.Add(NameLabour, SwingLabour);
                swings.Add(NameConservative, SwingConservative);
                swings.Add(NameLiberalDemocrat, SwingLiberalDemocrat);
                swings.Add(NameGreen, SwingGreen);
                swings.Add(NameUKIP, SwingUKIP);
                return swings;
            }
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
