using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Data;
using System.Windows.Input;

using ElectionScotlandSwingWpfApp.ViewModels.Utilities;
using ElectionScotlandSwingWpfApp.Models;

namespace ElectionScotlandSwingWpfApp.ViewModels
{
    public class PredictionsSetupViewModel : INotifyPropertyChanged
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

        private MajorPartyForecastsViewModel _listMajorPartyForecastsVM;
        private MajorPartyForecastsViewModel _constituencyMajorPartyForecastsVM;

        private ICommand _predictFromUniformNationalSwingCommand;

        #endregion

        #region Constructor

        public PredictionsSetupViewModel(MainWindow mainWindow, log4net.ILog log,
            Models.MainModel mainModel, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = mainViewModel;

            _listMajorPartyForecastsVM =
                new MajorPartyForecastsViewModel(_mainWindow, log, _mainModel,
                    _mainModel.PartyListForecasts, mainViewModel);
            _constituencyMajorPartyForecastsVM =
                new MajorPartyForecastsViewModel(_mainWindow, log, _mainModel,
                    _mainModel.PartyConstituencyForecasts, mainViewModel);
        }

        #endregion

        #region Public Data

        public MajorPartyForecastsViewModel ListMajorPartyForecastsVM
        {
            get { return _listMajorPartyForecastsVM; }
        }

        public MajorPartyForecastsViewModel ConstituencyMajorPartyForecastsVM
        {
            get { return _constituencyMajorPartyForecastsVM; }
        }

        #endregion

        #region Public Methods

        internal void UpdateData()
        {
            _listMajorPartyForecastsVM.UpdateData(_mainModel.PartyListForecasts);
            _constituencyMajorPartyForecastsVM.UpdateData(_mainModel.PartyConstituencyForecasts);
            OnPropertyChanged(() => ListMajorPartyForecastsVM);
            OnPropertyChanged(() => ConstituencyMajorPartyForecastsVM);
        }

        #endregion

        #region Commands

        public ICommand PredictFromUniformNationalSwingCommand
        {
            get
            {
                return _predictFromUniformNationalSwingCommand ??
                    (_predictFromUniformNationalSwingCommand =
                        new CommandHandler(() => PredictFromUniformNationalSwingCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void PredictFromUniformNationalSwingCommandAction()
        {
            _mainModel.PredictUsingUniformNationalSwing(
                _listMajorPartyForecastsVM.PartySwings,
                _constituencyMajorPartyForecastsVM.PartySwings
                );

            _parent.UpdateData();
            OnPropertyChanged("");

        }


        #endregion
    }
}
