using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class CountiesMapViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public CountiesMapViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _countiesModel = new CountiesModel(log);

            Counties = new ObservableCollection<CountyViewModel>();
            _selectedCounty = null;
            foreach (var county in _countiesModel.Counties)
            {
                CountyViewModel countyVM = new CountyViewModel(_mainWindow, county, log);
                Counties.Add(countyVM);
                if (_selectedCounty == null)
                    _selectedCounty = countyVM;
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

        #endregion

        #region Utility Functions

        private void InitialiseChartViewModel()
        {
            _plotModel = new PlotModel();
            SetUpModel();
            LoadData();
            OnPropertyChanged(() => PlotModel);
        }

        private void SetUpModel()
        {
            _plotModel.LegendTitle = SelectedCounty.Name;
            _plotModel.LegendOrientation = LegendOrientation.Horizontal;
            _plotModel.LegendPlacement = LegendPlacement.Outside;
            _plotModel.LegendPosition = LegendPosition.TopRight;
            _plotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            _plotModel.LegendBorder = OxyColors.Black;
            _plotModel.PlotType = PlotType.Cartesian;


            double minX = _selectedCounty.MinLongitude;
            double minY = _selectedCounty.MinLatitude;

            PolygonPoint minPt = new PolygonPoint(_selectedCounty.MinLongitude, _selectedCounty.MinLatitude);
            minPt.GetCoordinates(out minX, out minY);


            double maxX = _selectedCounty.MinLongitude;
            double maxY = _selectedCounty.MinLatitude;

            PolygonPoint maxPt = new PolygonPoint(_selectedCounty.MaxLongitude, _selectedCounty.MaxLatitude);
            maxPt.GetCoordinates(out maxX, out maxY);

            double xRange = maxX - minX;
            double yRange = maxY - minY;


            var eastAxis = new LinearAxis(AxisPosition.Bottom, 0)
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "East",
                PositionAtZeroCrossing = false,
                Maximum = maxX + (xRange * 0.1),
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

        private void SetupMasterPoint()
        {
            double centreX = 0;
            double centreY = 0;
            PolygonPoint centre = new PolygonPoint(_selectedCounty.CentralLongitude, _selectedCounty.CentralLatitude);
            centre.GetCoordinates(out centreX, out centreY);

            _startPointEast = centreX;
            _startPointNorth = centreY;
        }

        private void AddAreaSeriesToPlot(
            out double minEast, out double minNorth, out double maxEast, out double maxNorth)
        {

            SetupMasterPoint();

            minEast = _selectedCounty.MinLongitude;
            minNorth = _selectedCounty.MinLatitude;

            maxEast = _selectedCounty.MaxLongitude;
            maxNorth = _selectedCounty.MaxLatitude;


            int i = 0;
            foreach (var boundary in _selectedCounty.LandBlocks)
            {
                var areaSeries = new AreaSeries
                {
                    Color = OxyColors.Blue,
                };

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

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public ObservableCollection<CountyViewModel> Counties { get; set; }

        public CountyViewModel SelectedCounty
        {
            get
            {
                return _selectedCounty;
            }
            set
            {
                _selectedCounty = value;
                OnPropertyChanged(() => SelectedCounty);
                InitialiseChartViewModel();
            }
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        #endregion

        #region Member variables

        protected PlotModel _plotModel;

        private MainWindow _mainWindow;
        private CountiesModel _countiesModel;

        private double _startPointEast = 0;
        private double _startPointNorth = 0;

        private ICommand _printToPngCommand;
        private CountyViewModel _selectedCounty;

        #endregion

        #region Commands

        public ICommand PrintToPngCommand
        {
            get
            {
                return _printToPngCommand ??
                    (_printToPngCommand = new CommandHandler(() => PrintToPngCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void PrintToPngCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = _countiesModel.LastPngFile;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|PNG files (*.png)|*.png";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _countiesModel.LastPngFile = fileDialog.FileName;

                Properties.Settings.Default.LastPngFile =
                    _countiesModel.LastPngFile;
                Properties.Settings.Default.Save();

                using (var stream = File.Create(fileDialog.FileName))
                {
                    OxyPlot.Wpf.PngExporter.Export(_plotModel, stream, 800, 600, OxyColors.White);
                }
            }
        }

        #endregion
    }
}
