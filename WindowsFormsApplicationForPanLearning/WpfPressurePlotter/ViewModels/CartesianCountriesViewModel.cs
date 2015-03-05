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
    public class CartesianCountriesViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public CartesianCountriesViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _countriesModel = new CountriesModel(log);

            Countries = new ObservableCollection<CountryViewModel>();
            _selectedCountry = null;
            foreach (var country in _countriesModel.Countries)
            {
                CountryViewModel countryVM = new CountryViewModel(_mainWindow, country, log);
                Countries.Add(countryVM);
                if (_selectedCountry == null)
                    _selectedCountry = countryVM;
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


            var eastAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "East",
                PositionAtZeroCrossing = false,
                Maximum = maxX + (xRange * 0.1),
                Minimum = minX - (xRange * 0.1),
                Position = AxisPosition.Bottom
            };
            _plotModel.Axes.Add(eastAxis);

            var northAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "North",
                PositionAtZeroCrossing = false,
                Maximum = maxY + (yRange * 0.1),
                Minimum = minY - (yRange * 0.1),
                Position = AxisPosition.Left
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

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        public CountriesModel AllCountriesModel
        {
            get
            {
                return _countriesModel;
            }
        }

        #endregion

        #region Member variables

        protected PlotModel _plotModel;

        private MainWindow _mainWindow;
        private CountriesModel _countriesModel;

        private double _startPointEast = 0;
        private double _startPointNorth = 0;

        private ICommand _printToPngCommand;
        private CountryViewModel _selectedCountry;

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
            fileDialog.FileName = _countriesModel.LastPngFile;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|PNG files (*.png)|*.png";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _countriesModel.LastPngFile = fileDialog.FileName;

                Properties.Settings.Default.LastPngFile =
                    _countriesModel.LastPngFile;
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
