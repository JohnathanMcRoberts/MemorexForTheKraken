using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using ElectionScotlandSwingWpfApp.Models;

namespace ElectionScotlandSwingWpfApp.ViewModels
{
    public class MainViewModel
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

        #region Private data
        
        private MainModel _mainModel;
        private MainWindow _mainWindow;
        private log4net.ILog _log;

        private DataLoaderViewModel _dataLoaderVM;
        private DataGridsViewModel _dataGridsVM;

        private MajorPartyForecastsViewModel _listMajorPartyForecastsVM;
        private MajorPartyForecastsViewModel _constituencyMajorPartyForecastsVM;

        #endregion

        #region Public Properties

        public log4net.ILog Log { get { return _log; } }

        public DataLoaderViewModel DataLoaderVM
        {
            get { return _dataLoaderVM; }
        }

        public DataGridsViewModel DataGridsVM
        {
            get { return _dataGridsVM; }
        }

        public MajorPartyForecastsViewModel ListMajorPartyForecastsVM
        {
            get { return _listMajorPartyForecastsVM; }
        }

        public MajorPartyForecastsViewModel ConstituencyMajorPartyForecastsVM
        {
            get { return _constituencyMajorPartyForecastsVM; }
        }

        #endregion

        #region Constructor

        public MainViewModel(MainWindow mainWindow, log4net.ILog log)
        {
            _mainWindow = mainWindow;
            _log = log;

            _mainModel = new MainModel(log);

            _dataLoaderVM = new DataLoaderViewModel(_mainWindow, log, _mainModel, this);
            _dataGridsVM = new DataGridsViewModel(_mainWindow, log, _mainModel, this);
            InitialiseMajorPartyForecasts(log);
        }

        private void InitialiseMajorPartyForecasts(log4net.ILog log)
        {
            _listMajorPartyForecastsVM =
                new MajorPartyForecastsViewModel(_mainWindow, log, _mainModel,
                    _mainModel.PartyListForecasts, this);
            _constituencyMajorPartyForecastsVM =
                new MajorPartyForecastsViewModel(_mainWindow, log, _mainModel,
                    _mainModel.PartyConstituencyForecasts, this);
        }

        #endregion

        internal void UpdateData()
        {
            _dataGridsVM.UpdateData();

            _listMajorPartyForecastsVM.UpdateData(_mainModel.PartyListForecasts);
            _constituencyMajorPartyForecastsVM.UpdateData(_mainModel.PartyConstituencyForecasts);

            OnPropertyChanged("");
            OnPropertyChanged(() => ListMajorPartyForecastsVM);
            OnPropertyChanged(() => ConstituencyMajorPartyForecastsVM);
            //throw new NotImplementedException();
        }
    }
}
