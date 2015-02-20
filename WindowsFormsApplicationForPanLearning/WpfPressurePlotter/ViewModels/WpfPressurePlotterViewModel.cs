using System;
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

            Countries = new ObservableCollection<CountryViewModel>();
            _selectedCountry = null;
            foreach (var country in _mainModel.Countries)
            {
                CountryViewModel countryVM = new CountryViewModel(_mainWindow, country, log);
                Countries.Add(countryVM);
                if (_selectedCountry == null)
                    _selectedCountry = countryVM;
            }
        }

        #endregion

        protected PlotModel _plotModel;


        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        internal void InitialiseChartViewModel()
        {
            _plotModel = new PlotModel();
            SetUpModel();
            LoadData();
            OnPropertyChanged(() => PlotModel);
        }


        private void SetUpModel()
        {
            _plotModel.LegendTitle = SelectedCountry.Name;
            _plotModel.LegendOrientation = LegendOrientation.Horizontal;
            _plotModel.LegendPlacement = LegendPlacement.Outside;
            _plotModel.LegendPosition = LegendPosition.TopRight;
            _plotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            _plotModel.LegendBorder = OxyColors.Black;
            _plotModel.PlotType = PlotType.Cartesian;


            double minX = _selectedCountry.MinLongitude;
            double minY = _selectedCountry.MinLatitude;

            PolygonPoint minPt = new PolygonPoint(_selectedCountry.MinLongitude, _selectedCountry.MinLatitude);
            minPt.GetCoordinates(out minX, out minY);


            double maxX = _selectedCountry.MinLongitude;
            double maxY = _selectedCountry.MinLatitude;

            PolygonPoint maxPt = new PolygonPoint(_selectedCountry.MaxLongitude, _selectedCountry.MaxLatitude);
            maxPt.GetCoordinates(out maxX, out maxY);

            double xRange = maxX - minX;
            double yRange = maxY - minY;


            var eastAxis = new LinearAxis(AxisPosition.Bottom, 0)
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "East",
                PositionAtZeroCrossing = false,
                Maximum = maxX  + (xRange* 0.1),
                Minimum = minX - (xRange * 0.1)
            };
            _plotModel.Axes.Add(eastAxis);

            var northAxis = new LinearAxis(AxisPosition.Left, 0)
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "North",
                PositionAtZeroCrossing = false,
                Maximum = maxY + (yRange * 0.1),
                Minimum = minY - (yRange * 0.1)
            };
            _plotModel.Axes.Add(northAxis);

        }

        private void LoadData()
        {
            double minEast;
            double minNorth;
            double maxEast;
            double maxNorth;
            AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth);
        }

        
        private double _startPointEast = 0;
        private double _startPointNorth = 0;

        private void SetupMasterPoint()
        {
            double centreX = 0;
            double centreY = 0;
            PolygonPoint centre = new PolygonPoint(_selectedCountry.CentralLongitude, _selectedCountry.CentralLatitude);
            centre.GetCoordinates(out centreX, out centreY);

            _startPointEast = centreX;
            _startPointNorth = centreY;
        }


        private void AddAreaSeriesToPlot(
            out double minEast, out double minNorth, out double maxEast, out double maxNorth)
        {

            SetupMasterPoint();

            minEast = _selectedCountry.MinLongitude;
            minNorth = _selectedCountry.MinLatitude;

            maxEast = _selectedCountry.MaxLongitude;
            maxNorth = _selectedCountry.MaxLatitude;


            int i = 0;
            foreach (var boundary in _selectedCountry.LandBlocks)
            {
                var areaSeries = new AreaSeries
                {
                    Color = OxyColors.Blue,
                };
#if aaa
                double eastOffset =
                    wellpath.PlanSurvey.ParentWell.ParentSlot.CoordEasting - _startPointEast;
                double northOffset =
                    wellpath.PlanSurvey.ParentWell.ParentSlot.CoordNorthing - _startPointNorth;

                int pointIndex = 0;

                List<Annotation> annotations = new List<Annotation>();
#endif
                foreach (var point in boundary.Points)
                {

                    double ptX = 0;
                    double ptY = 0;
                    point.GetCoordinates(out ptX, out ptY);
                    DataPoint dataPoint = new DataPoint(ptX, ptY);

                    areaSeries.Points.Add(dataPoint);

                }


                _plotModel.Series.Add(areaSeries);

                i++;
            }
        }


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

        public ObservableCollection<CountryViewModel> Countries { get; set; }

        public CountryViewModel SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(() => SelectedCountry);
                InitialiseChartViewModel();
            }
        }


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
        private CountryViewModel _selectedCountry;

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
