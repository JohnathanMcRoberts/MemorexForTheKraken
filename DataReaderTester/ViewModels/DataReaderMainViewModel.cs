using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using log4net;

using DataReaderTester.Models;
using DataReaderTester.ViewModels.Utilities;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace DataReaderTester.ViewModels
{
    public class DataReaderMainViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public DataReaderMainViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainDataReaderModel = new MainDataReaderModel(log);

            
            SelectedTimeColumnVM = new  DataColumnViewModel(log);
            SelectedPressureColumnVM = new  DataColumnViewModel(log);


            SelectedLasTimeColumnVM = new LasCurveViewModel(log);
            SelectedLasPressureColumnVM = new LasCurveViewModel(log);
            SelectedLasRateColumnVM = new LasCurveViewModel(log);
        }

        #endregion

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

        #region Properties

        public ILog Log { get; set; }

        public Boolean IsFileOpened { get; private set; }

        public String TprFileName { get { return _mainDataReaderModel.TprFileName; } }

        public String LasFileName { get { return _mainDataReaderModel.LasFileName; } }

        public List<string> ColumnNames 
        { 
            get 
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.TprFile == null ||
                    _mainDataReaderModel.TprFile.ColumnNames == null)
                    return new List<string>();
                return _mainDataReaderModel.TprFile.ColumnNames; 
            } 
        }

        public int SelectedTimeColumnIndex
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.TprFile == null ||
                    _mainDataReaderModel.TprFile.ColumnNames == null || _mainDataReaderModel.TprFile.TimeColumn == -1)
                    return 0;
                return _mainDataReaderModel.TprFile.TimeColumn;
            }
            set
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.TprFile == null ||
                    _mainDataReaderModel.TprFile.ColumnNames == null )
                    return;
                _mainDataReaderModel.TprFile.SelectTimeColumn(value);
                OnPropertyChanged(() => SelectedTimeColumnIndex);
                if (_mainDataReaderModel != null && _mainDataReaderModel.TprFile != null &&
                    _mainDataReaderModel.TprFile.TimeColumn >= 0)
                {
                    SelectedTimeColumnVM.Column =
                        _mainDataReaderModel.TprFile.ColumnDefinitions[_mainDataReaderModel.TprFile.TimeColumn];
                    OnPropertyChanged(() => SelectedTimeColumnVM);
                }
            }
        }
        public int SelectedPressureColumnIndex
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.TprFile == null ||
                    _mainDataReaderModel.TprFile.ColumnNames == null || _mainDataReaderModel.TprFile.PressureColumn == -1)
                    return 0;
                return _mainDataReaderModel.TprFile.PressureColumn;
            }
            set
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.TprFile == null ||
                    _mainDataReaderModel.TprFile.ColumnNames == null)
                    return;
                _mainDataReaderModel.TprFile.SelectPressureColumn(value);
                OnPropertyChanged(() => SelectedPressureColumnIndex);

                if (_mainDataReaderModel != null && _mainDataReaderModel.TprFile != null &&
                    _mainDataReaderModel.TprFile.PressureColumn >= 0)
                {
                    SelectedPressureColumnVM.Column =
                        _mainDataReaderModel.TprFile.ColumnDefinitions[_mainDataReaderModel.TprFile.PressureColumn];
                    OnPropertyChanged(() => SelectedPressureColumnVM);
                }
            }
        }
        
        public DataColumnViewModel SelectedTimeColumnVM { get; private set;}
        public DataColumnViewModel SelectedPressureColumnVM { get; private set;}

        public List<string> LasColumnNames
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasColumnNames == null)
                    return new List<string>();
                return _mainDataReaderModel.LasColumnNames;
            }
        }
        public LasCurveViewModel SelectedLasTimeColumnVM { get; private set; }
        public LasCurveViewModel SelectedLasPressureColumnVM { get; private set; }
        public LasCurveViewModel SelectedLasRateColumnVM { get; private set; }

        public int SelectedLasTimeColumnIndex
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null || _mainDataReaderModel.LasFileTimeColumn == -1)
                    return 0;
                return _mainDataReaderModel.LasFileTimeColumn;
            }
            set
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null)
                    return;
                _mainDataReaderModel.LasFileTimeColumn = value;

                OnPropertyChanged(() => SelectedLasTimeColumnIndex);
                
                if (_mainDataReaderModel != null && _mainDataReaderModel.LasFile != null &&
                    _mainDataReaderModel.LasFileTimeColumn == value)
                {
                    SelectedLasTimeColumnVM.Column =
                        _mainDataReaderModel.LasFile.DataCurves[_mainDataReaderModel.LasFileTimeColumn];
                    OnPropertyChanged(() => SelectedLasTimeColumnVM);
                }
                else
                    OnPropertyChanged(() => SelectedLasTimeColumnIndex);
            }
        }


        public int SelectedLasPressureColumnIndex
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null || _mainDataReaderModel.LasFilePressureColumn == -1)
                    return 0;
                return _mainDataReaderModel.LasFilePressureColumn;
            }
            set
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null)
                    return;
                _mainDataReaderModel.LasFilePressureColumn = value;

                OnPropertyChanged(() => SelectedLasPressureColumnIndex);
                
                if (_mainDataReaderModel != null && _mainDataReaderModel.LasFile != null &&
                    _mainDataReaderModel.LasFilePressureColumn == value)
                {
                    SelectedLasPressureColumnVM.Column =
                        _mainDataReaderModel.LasFile.DataCurves[value];
                    OnPropertyChanged(() => SelectedLasPressureColumnVM);
                }
                else
                    OnPropertyChanged(() => SelectedLasPressureColumnIndex);
            }
        }

        public int SelectedLasRateColumnIndex
        {
            get
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null || _mainDataReaderModel.LasFileRateColumn == -1)
                    return 0;
                return _mainDataReaderModel.LasFileRateColumn;
            }
            set
            {
                if (_mainDataReaderModel == null || _mainDataReaderModel.LasFile == null ||
                    _mainDataReaderModel.LasFile.DataCurves == null)
                    return;
                _mainDataReaderModel.LasFileRateColumn = value;

                OnPropertyChanged(() => SelectedLasRateColumnIndex);

                if (_mainDataReaderModel != null && _mainDataReaderModel.LasFile != null &&
                    _mainDataReaderModel.LasFileRateColumn == value)
                {
                    SelectedLasRateColumnVM.Column =
                        _mainDataReaderModel.LasFile.DataCurves[value];
                    OnPropertyChanged(() => SelectedLasRateColumnVM);
                }
                else
                    OnPropertyChanged(() => SelectedLasRateColumnIndex);
            }
        }


        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private MainDataReaderModel _mainDataReaderModel;
        private ICommand _chooseTprFileCommand;
        private ICommand _chooseLasFileCommand;

        #endregion

        #region Commands

        public ICommand ChooseTprFileCommand
        {
            get
            {
                return _chooseTprFileCommand ??
                    (_chooseTprFileCommand = new CommandHandler(() => ChooseTprFileCommandAction(), true));
            }
        }

        public ICommand ChooseLasFileCommand
        {
            get
            {
                return _chooseLasFileCommand ??
                    (_chooseLasFileCommand = new CommandHandler(() => ChooseLasFileCommandAction(), true));
            }
        }
        #endregion

        #region Command Handlers

        public void ChooseTprFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainDataReaderModel.TprFileName;

            // TODO - get the file types from the available serialisers
            fileDialog.Filter = @"All files (*.*)|*.*|TPR files (*.tpr)|*.tpr";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainDataReaderModel.TprFileName = fileDialog.FileName;

                Properties.Settings.Default.TprFile =
                    _mainDataReaderModel.TprFileName;
                Properties.Settings.Default.Save();

                using (new WaitCursor())
                {
                    _mainDataReaderModel.OpenSimpleTpr();

                    IsFileOpened = true;
                    OnPropertyChanged(() => IsFileOpened);
                    OnPropertyChanged(() => ColumnNames);
                    OnPropertyChanged(() => SelectedTimeColumnIndex);
                    OnPropertyChanged(() => SelectedPressureColumnIndex);

                    if (_mainDataReaderModel != null && _mainDataReaderModel.TprFile != null &&
                        _mainDataReaderModel.TprFile.TimeColumn >= 0)
                    {
                        SelectedTimeColumnVM.Column =
                            _mainDataReaderModel.TprFile.ColumnDefinitions[_mainDataReaderModel.TprFile.TimeColumn];
                        OnPropertyChanged(() => SelectedTimeColumnVM);
                    }
                    
                    if (_mainDataReaderModel != null && _mainDataReaderModel.TprFile != null &&
                        _mainDataReaderModel.TprFile.PressureColumn >= 0)
                    {
                        SelectedPressureColumnVM.Column =
                            _mainDataReaderModel.TprFile.ColumnDefinitions[_mainDataReaderModel.TprFile.PressureColumn];
                        OnPropertyChanged(() => SelectedPressureColumnVM);
                    }
                }
            }
        }

        public void ChooseLasFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainDataReaderModel.LasFileName;

            // TODO - get the file types from the available serialisers
            fileDialog.Filter = @"All files (*.*)|*.*|LAS files (*.las)|*.las";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainDataReaderModel.LasFileName = fileDialog.FileName;

                Properties.Settings.Default.LasFile =
                    _mainDataReaderModel.LasFileName;
                Properties.Settings.Default.Save();

                using (new WaitCursor())
                {
                    _mainDataReaderModel.OpenSimpleLas();

                    IsFileOpened = true;
                    OnPropertyChanged(() => IsFileOpened);
                    OnPropertyChanged(() => LasColumnNames);
                    OnPropertyChanged(() => SelectedTimeColumnIndex);
                    OnPropertyChanged(() => SelectedPressureColumnIndex);


                    if (_mainDataReaderModel != null && _mainDataReaderModel.LasFile != null &&
                        _mainDataReaderModel.LasFile.DataCurves != null && 
                        _mainDataReaderModel.LasFile.DataCurves.Count > 0)
                    {
                        _mainDataReaderModel.LasFileTimeColumn = 0;
                        SelectedLasTimeColumnVM.Column =
                            _mainDataReaderModel.LasFile.DataCurves[_mainDataReaderModel.LasFileTimeColumn];
                        OnPropertyChanged(() => SelectedLasTimeColumnIndex);
                        OnPropertyChanged(() => SelectedLasTimeColumnVM);
                    }
                }
            }
        }
        #endregion
    }
}
