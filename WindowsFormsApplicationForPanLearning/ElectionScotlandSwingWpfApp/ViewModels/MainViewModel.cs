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

        private PredictionsSetupViewModel _predictionsSetupVM;
        private ChartsViewModel _mainChartsVM;

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

        public PredictionsSetupViewModel PredictionsSetupVM
        {
            get { return _predictionsSetupVM; }
        }

        public ChartsViewModel MainChartsVM
        {
            get { return _mainChartsVM; }
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
            _predictionsSetupVM = new PredictionsSetupViewModel(_mainWindow, log, _mainModel, this);
            _mainChartsVM = new ChartsViewModel(_mainWindow, log, _mainModel, this);
        }


        #endregion

        #region Public Methods

        public void UpdateData()
        {
            _dataGridsVM.UpdateData();

            _predictionsSetupVM.UpdateData();

            OnPropertyChanged("");
            //throw new NotImplementedException();
        }

        #endregion
    }
}
