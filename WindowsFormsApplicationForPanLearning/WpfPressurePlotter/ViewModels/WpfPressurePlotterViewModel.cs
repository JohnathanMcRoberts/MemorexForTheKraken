using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using log4net;

using WpfPressurePlotter.Models;
using WpfPressurePlotter.Models.GeoData;
using WpfPressurePlotter.ViewModels.Utilities;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace WpfPressurePlotter.ViewModels
{
    public class WpfPressurePlotterViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public WpfPressurePlotterViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainModel = new MainPressurePlotterModel(log);

            SelectedLasTimeColumnVM = new LasCurveViewModel(log);
            SelectedLasPressureColumnVM = new LasCurveViewModel(log);
            SelectedLasRateColumnVM = new LasCurveViewModel(log);

            CountriesPlotVM = new CartesianCountriesViewModel(_mainWindow, log);

            CountiesPlotVM = new CountiesMapViewModel(_mainWindow, log);

            ConstituenciesPlotVM = new ConstituenciesMapViewModel(_mainWindow, log, CountriesPlotVM);
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

        public CartesianCountriesViewModel CountriesPlotVM { get; set; }

        public CountiesMapViewModel CountiesPlotVM { get; set; }

        public ConstituenciesMapViewModel ConstituenciesPlotVM { get; set; }

        

        #region LAS

        public Boolean IsFileOpened { get; private set; }

        public String LasFileName { get { return _mainModel.LasFileName; } }

        public List<string> LasColumnNames
        {
            get
            {
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasColumnNames == null)
                    return new List<string>();
                return _mainModel.LasColumnNames;
            }
        }
        public LasCurveViewModel SelectedLasTimeColumnVM { get; private set; }
        public LasCurveViewModel SelectedLasPressureColumnVM { get; private set; }
        public LasCurveViewModel SelectedLasRateColumnVM { get; private set; }

        public int SelectedLasTimeColumnIndex
        {
            get
            {
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null || _mainModel.LasFileTimeColumn == -1)
                    return 0;
                return _mainModel.LasFileTimeColumn;
            }
            set
            {
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null)
                    return;
                _mainModel.LasFileTimeColumn = value;

                OnPropertyChanged(() => SelectedLasTimeColumnIndex);

                if (_mainModel != null && _mainModel.LasFile != null &&
                    _mainModel.LasFileTimeColumn == value)
                {
                    SelectedLasTimeColumnVM.Column =
                        _mainModel.LasFile.DataCurves[_mainModel.LasFileTimeColumn];
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
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null || _mainModel.LasFilePressureColumn == -1)
                    return 0;
                return _mainModel.LasFilePressureColumn;
            }
            set
            {
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null)
                    return;
                _mainModel.LasFilePressureColumn = value;

                OnPropertyChanged(() => SelectedLasPressureColumnIndex);

                if (_mainModel != null && _mainModel.LasFile != null &&
                    _mainModel.LasFilePressureColumn == value)
                {
                    SelectedLasPressureColumnVM.Column =
                        _mainModel.LasFile.DataCurves[value];
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
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null || _mainModel.LasFileRateColumn == -1)
                    return 0;
                return _mainModel.LasFileRateColumn;
            }
            set
            {
                if (_mainModel == null || _mainModel.LasFile == null ||
                    _mainModel.LasFile.DataCurves == null)
                    return;
                _mainModel.LasFileRateColumn = value;

                OnPropertyChanged(() => SelectedLasRateColumnIndex);

                if (_mainModel != null && _mainModel.LasFile != null &&
                    _mainModel.LasFileRateColumn == value)
                {
                    SelectedLasRateColumnVM.Column =
                        _mainModel.LasFile.DataCurves[value];
                    OnPropertyChanged(() => SelectedLasRateColumnVM);
                }
                else
                    OnPropertyChanged(() => SelectedLasRateColumnIndex);
            }
        }

        #endregion

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private MainPressurePlotterModel _mainModel;
        private ICommand _chooseLasFileCommand;

        #endregion

        #region Commands

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

        public void ChooseLasFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainModel.LasFileName;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|LAS files (*.las)|*.las";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.LasFileName = fileDialog.FileName;

                Properties.Settings.Default.LasFile =
                    _mainModel.LasFileName;
                Properties.Settings.Default.Save();

                using (new WaitCursor())
                {
                    _mainModel.OpenSimpleLas();

                    IsFileOpened = true;
                    OnPropertyChanged(() => IsFileOpened);
                    OnPropertyChanged(() => LasColumnNames);
                    OnPropertyChanged(() => SelectedLasTimeColumnIndex);
                    OnPropertyChanged(() => SelectedLasPressureColumnIndex);


                    if (_mainModel != null && _mainModel.LasFile != null &&
                        _mainModel.LasFile.DataCurves != null &&
                        _mainModel.LasFile.DataCurves.Count > 0)
                    {
                        _mainModel.LasFileTimeColumn = 0;
                        SelectedLasTimeColumnVM.Column =
                            _mainModel.LasFile.DataCurves[_mainModel.LasFileTimeColumn];
                        OnPropertyChanged(() => SelectedLasTimeColumnIndex);
                        OnPropertyChanged(() => SelectedLasTimeColumnVM);
                    }
                }
            }
        }

        #endregion

    }
}
