using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;


using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;

namespace MongoDbBooks.ViewModels
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
        private MainBooksModel _mainModel;
        private MainViewModel _parent;

        private ICommand _openTextCommand;
        private ICommand _saveTextCommand;
        private ICommand _connectToDatabaseCommand;

        private bool _dataLoaded = false;
        private bool _connectedToDatabaseSuccessfully = false;


        #endregion

        #region Public Properties

        public bool IsDataLoaded
        {
            get { return _dataLoaded; }
            private set { _dataLoaded = value; OnPropertyChanged(() => IsDataLoaded); }
        }

        public bool ConnectedToDatabaseSuccessfully
        {
            get { return _connectedToDatabaseSuccessfully; }
            private set { _connectedToDatabaseSuccessfully = value; OnPropertyChanged(() => ConnectedToDatabaseSuccessfully); }
        }

        #endregion

        #region Constructor

        public DataLoaderViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            IsDataLoaded = (_mainModel.BooksRead.Count > 0);
        }

        #endregion

        #region Commands

        public ICommand OpenTextCommand
        {
            get
            {
                return _openTextCommand ??
                    (_openTextCommand =
                        new CommandHandler(() => OpenTextCommandAction(), true));
            }
        }

        public ICommand SaveTextCommand
        {
            get
            {
                return _saveTextCommand ??
                    (_saveTextCommand =
                        new CommandHandler(() => SaveTextCommandAction(), true));
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

        #endregion

        #region Command Handlers

        public void OpenTextCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainModel.InputFilePath;

            fileDialog.Filter = @"All files (*.*)|*.*|CSV Files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.ReadBooksFromFile(fileDialog.FileName);

                IsDataLoaded = true;
                _parent.UpdateData();
                OnPropertyChanged("");
            }
        }

        public void SaveTextCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = _mainModel.OutputFilePath;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|CSV files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _mainModel.WriteBooksToFile(fileDialog.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }
        }

        public void ConnectToDatabaseCommandAction()
        {
            using (new WaitCursor())
            {
                string errorMsg = "";

                ConnectedToDatabaseSuccessfully =
                    _mainModel.ConnectToDatabase(out errorMsg);


                if (ConnectedToDatabaseSuccessfully &&
                    _mainModel.BooksRead != null &&
                    _mainModel.BooksRead.Count != 0)
                {
                    IsDataLoaded = true;
                    _parent.UpdateData();
                    OnPropertyChanged("");
                }
            }
        }

        #endregion

    }
}
