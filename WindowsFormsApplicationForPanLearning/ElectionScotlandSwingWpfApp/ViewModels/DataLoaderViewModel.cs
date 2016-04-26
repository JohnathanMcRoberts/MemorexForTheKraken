using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;

using ElectionScotlandSwingWpfApp.ViewModels.Utilities;
using ElectionScotlandSwingWpfApp.Models;

using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace ElectionScotlandSwingWpfApp.ViewModels
{
    public class DataLoaderViewModel : INotifyPropertyChanged
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

        private ICommand _openConstituenciesResultsCommand;
        private ICommand _openRegionsResultsCommand;
        private ICommand _openElectoralResultsCommand;
        private ICommand _saveElectionAsXmlCommand;
        private ICommand _connectToDatabaseCommand;

        private bool _regionsDataLoaded;
        private bool _constituenciesDataLoaded;

        private bool _electionDataLoaded;
        
        #endregion

        #region Constructor

        public DataLoaderViewModel(MainWindow mainWindow, log4net.ILog log, 
            Models.MainModel mainModel, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = mainViewModel;
            _regionsDataLoaded = _constituenciesDataLoaded = _electionDataLoaded = false;
        }

        #endregion

        #region Commands

        public ICommand OpenConstituenciesResultsCommand
        {
            get
            {
                return _openConstituenciesResultsCommand ??
                    (_openConstituenciesResultsCommand =
                        new CommandHandler(() => OpenConstituenciesResultsCommandAction(), true));
            }
        }

        public ICommand OpenRegionsResultsCommand
        {
            get
            {
                return _openRegionsResultsCommand ??
                    (_openRegionsResultsCommand =
                        new CommandHandler(() => OpenRegionsResultsCommandAction(), true));
            }
        }

        public ICommand SaveElectionAsXmlCommand
        {
            get
            {
                return _saveElectionAsXmlCommand ??
                    (_saveElectionAsXmlCommand =
                        new CommandHandler(() => SaveElectionAsXmlCommandAction(), true));
            }
        }

        public ICommand ConnectToDatabaseCommand
        {
            get
            {
                return _connectToDatabaseCommand ??
                    (_connectToDatabaseCommand =
                        new CommandHandler(() => ConnectToDatabaseCommandAction(), true));
            }
        }

        public ICommand OpenElectoralResultsCommand
        {
            get
            {
                return _openElectoralResultsCommand ??
                    (_openElectoralResultsCommand =
                        new CommandHandler(() => OpenElectoralResultsCommandAction(), true));
            }
        }

        

        #endregion

        #region Command Handlers

        public void OpenConstituenciesResultsCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.FileName = _mainModel.InputFilePath;
            fileDialog.Title = "Open Constituencies Results";

            fileDialog.Filter = @"All files (*.*)|*.*|CSV Files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.OpenConstituenciesResults(fileDialog.FileName);

                ConstituenciesDataLoaded = true;
                _parent.UpdateData();
                OnPropertyChanged("");
            }
        }

        public void OpenRegionsResultsCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.FileName = _mainModel.InputFilePath;

            fileDialog.Title = "Open Regions Results";

            fileDialog.Filter = @"All files (*.*)|*.*|CSV Files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.OpenRegionsResults(fileDialog.FileName);

                RegionsDataLoaded = true;
                _parent.UpdateData();
                OnPropertyChanged("");
            }
        }

        public void SaveElectionAsXmlCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            //fileDialog.FileName = _mainModel.OutputFilePath;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|XML files (*.XML)|*.xml";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _mainModel.WriteElectionResultToFile(fileDialog.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }
        }

        public void OpenElectoralResultsCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.FileName = _mainModel.InputFilePath;

            fileDialog.Title = "Open Electoral Results XML";

            fileDialog.Filter = @"All files (*.*)|*.*|XML files (*.XML)|*.xml";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.OpenElectoralResults(fileDialog.FileName);

                RegionsDataLoaded = true;
                _parent.UpdateData();
                OnPropertyChanged("");
            }
        }

        public void ConnectToDatabaseCommandAction()
        {
            //using (new WaitCursor())
            //{
            //    string errorMsg = "";

            //    GotAllRateEstimatesSuccessfully =
            //        _mainModel.PerformAllRateEstimates(out errorMsg);

            //    if (!GotAllRateEstimatesSuccessfully)
            //    {
            //        MessageBox.Show(errorMsg);
            //    }


            //    CalculatedRateEstimates.Clear();
            //    foreach (var est in _mainModel.AllCalculatedRateEstimates)
            //        CalculatedRateEstimates.Add(est);

            //    OnPropertyChanged(() => CalculatedRateEstimates);
            //    //UpdateOnNewEspData();
            //    OnPropertyChanged("");
            //}
        }

        #endregion

        #region Properties

        public bool RegionsDataLoaded 
        { 
            get { return _regionsDataLoaded; }
            private set { _regionsDataLoaded = value; if (_regionsDataLoaded && _constituenciesDataLoaded) _electionDataLoaded = true; }
        }

        public bool ConstituenciesDataLoaded
        {
            get { return _constituenciesDataLoaded; }
            private set { _constituenciesDataLoaded = value; if (_regionsDataLoaded && _constituenciesDataLoaded) _electionDataLoaded = true; }
        }

        public bool ElectionDataLoaded
        {
            get { return _electionDataLoaded; }
            private set { _electionDataLoaded = value; }
        }

        #endregion
    }
}
