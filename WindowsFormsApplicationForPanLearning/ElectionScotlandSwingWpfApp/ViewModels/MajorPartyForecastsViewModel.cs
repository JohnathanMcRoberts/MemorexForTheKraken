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
            }
        }

        #endregion

        internal void UpdateData(List<PartyForecast> partyForecasts)
        {
            _partyForecasts = partyForecasts;
            OnPropertyChanged("");
        }
    }
}
