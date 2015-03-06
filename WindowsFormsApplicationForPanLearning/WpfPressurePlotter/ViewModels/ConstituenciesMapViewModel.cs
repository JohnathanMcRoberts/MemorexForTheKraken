using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Xml;
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
    public class ConstituenciesMapViewModel : INotifyPropertyChanged
    {
        public ConstituenciesMapViewModel(
            MainWindow mainWindow, log4net.ILog log, CartesianCountriesViewModel countriesVM)
        {
            _mainWindow = mainWindow;
            Log = log;
            _countriesVM = countriesVM;

            _oxySelectedConstituencyColour = OxyColors.Maroon;
            _oxyNeighbourConstituenciesColour = OxyColors.LightBlue;

            Constituencies = new ObservableCollection<ConstituencyViewModel>();
            _selectedConstituency = null;

            List<IGeographicEntity> countries =
                (from c in countriesVM.AllCountriesModel.Countries select c as IGeographicEntity).ToList();

            _constituenciesModel = new ConstituenciesModel(log, countries);
            foreach (var constituency in _constituenciesModel.Constituencies)
            {
                ConstituencyViewModel constituencyVM = 
                    new ConstituencyViewModel(_mainWindow, constituency, log);

                Constituencies.Add(constituencyVM);
                if (_selectedConstituency == null)
                    _selectedConstituency = constituencyVM;
            }
            UkConstituencies = new AllConstituencesViewModel(_mainWindow, _constituenciesModel, log);
            SelectedConstituency = _selectedConstituency;

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

        #region Utility Functions

        private void InitialiseChartViewModel()
        {
            _plotModel = new PlotModel();
            SetUpModel(_plotModel, SelectedConstituency);
            LoadData();
            OnPropertyChanged(() => ConstituencyPlot);
        }

        private void SetUpModel(PlotModel plotModel, IGeographicalEntityViewModel geography)
        {
            plotModel.LegendTitle = geography.Name;
            plotModel.LegendOrientation = LegendOrientation.Horizontal;
            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendPosition = LegendPosition.TopRight;
            plotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            plotModel.LegendBorder = OxyColors.Black;
            plotModel.PlotType = PlotType.Cartesian;


            double minX = SelectedConstituency.MinLongitude;
            double minY = SelectedConstituency.MinLatitude;

            PolygonPoint minPt = new PolygonPoint(geography.MinLongitude, geography.MinLatitude);
            minPt.GetCoordinates(out minX, out minY);


            double maxX = SelectedConstituency.MinLongitude;
            double maxY = SelectedConstituency.MinLatitude;

            PolygonPoint maxPt = new PolygonPoint(geography.MaxLongitude, geography.MaxLatitude);
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
            plotModel.Axes.Add(eastAxis);

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
            plotModel.Axes.Add(northAxis);
        }

        private void LoadData()
        {
            double startPointEast;
            double startPointNorth;
            SetupMasterPoint(SelectedConstituency, out startPointEast, out startPointNorth);
            _startPointEast = startPointEast;
            _startPointNorth = startPointNorth;

            double minEast;
            double minNorth;
            double maxEast;
            double maxNorth;

            AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth,
                SelectedConstituency, OxyColors.Blue, _plotModel, ConstituencyReduction);

        }

        private void SetupMasterPoint(IGeographicalEntityViewModel constituency, out double startPointEast, out double startPointNorth)
        {
            double centreX = 0;
            double centreY = 0;
            PolygonPoint centre = new PolygonPoint(constituency.CentralLongitude, constituency.CentralLatitude);
            centre.GetCoordinates(out centreX, out centreY);

            startPointEast = centreX;
            startPointNorth = centreY;
        }

        private void AddAreaSeriesToPlot(out double minEast, out double minNorth, out double maxEast, 
            out double maxNorth, IGeographicalEntityViewModel constituency,
            OxyPlot.OxyColor colour, PlotModel plotModel, int reduction, 
            bool addToLegend = false, string legendText="")
        {
            minEast = constituency.MinLongitude;
            minNorth = constituency.MinLatitude;

            maxEast = constituency.MaxLongitude;
            maxNorth = constituency.MaxLatitude;

            int i = 0;
            bool addedLegend = false;
            foreach (var boundary in constituency.LandBlocks)
            {
                if (boundary.Points.Count > reduction)
                {
                    var areaSeries = new AreaSeries
                    {
                        Color = colour,
                        ToolTip = constituency.Name,
                        StrokeThickness = 1.0
                    };

                    if (addToLegend && legendText !="" && !addedLegend)
                    {
                        areaSeries.Title = legendText;
                        addedLegend = true;
                    }

                    int j = 0;
                    foreach (var point in boundary.Points)
                    {
                        j++;

                        if (1 != (j % reduction)) continue;

                        double ptX = 0;
                        double ptY = 0;
                        point.GetCoordinates(out ptX, out ptY);
                        DataPoint dataPoint = new DataPoint(ptX, ptY);

                        areaSeries.Points.Add(dataPoint);
                    }
                    plotModel.Series.Add(areaSeries);
                }

                i++;
            }
        }

        private string GetStringForNumber(int i)
        {
            string ith ="";

            if (i == 1) return ith;
            if (i == 0) return "0th";
            if (i > 4 && i < 21) return string.Format("{0}th", i);
            if ((i % 10) == 2) return string.Format("{0}nd", i);
            if ((i % 10) == 3) return string.Format("{0}rd", i);
            return string.Format("{0}th", i);
        }

        private void InitialiseAllConstituenciesChartViewModel()
        {
            _plotModelAllConstuencies = new PlotModel();
            SetUpModel(_plotModelAllConstuencies, _allConstituences);
            _plotModelAllConstuencies.LegendTitle =
                _allConstituences.Name + " by " + GetStringForNumber(1 + _nearestCountryCount) + " nearest country";

            double startPointEast;
            double startPointNorth;
            SetupMasterPoint(_allConstituences, out startPointEast, out startPointNorth);

            double minEast;
            double minNorth;
            double maxEast;
            double maxNorth;

            List<string> countriesInLegend = new List<string>();

            foreach (var constituency in Constituencies)
            {
                if (constituency.Name == _selectedConstituency.Name)
                    continue;

                bool addToLegend = false;
                OxyColor color;
                string legendText;
                GetConstuencyColourAndLegendFlag(constituency,
                    ref countriesInLegend, out color, out addToLegend, out legendText);
                
                AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth,
                    constituency, color, _plotModelAllConstuencies,
                    UkReduction, addToLegend, legendText);
            }


            AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth,
                _selectedConstituency, _oxySelectedConstituencyColour, _plotModelAllConstuencies, 
                UkReduction);

            OnPropertyChanged(() => UkConstituenciesPlot);
        }

        private void GetConstuencyColourAndLegendFlag(ConstituencyViewModel constituency, 
            ref List<string> countriesInLegend, out OxyColor color, out bool addToLegend,
            out string legendText)
        {
            // get the nearest country form the list
            IGeographicEntity nearest = GetNearestCountry(constituency);

            color = _defaultColors[0];
            addToLegend = false;
            int countryColourIndex = -1;
            legendText = "";
            if (nearest == null) return;

            // see if it is to be added to the legend
            addToLegend = true;
            for(int i = 0; i < countriesInLegend.Count; ++i)
            {
                var countryInLegend = countriesInLegend[i];
                if (countryInLegend == nearest.Name)
                {
                    addToLegend = false;
                    countryColourIndex = i;
                }
            }
            if (addToLegend)
            {
                countryColourIndex = countriesInLegend.Count;
                legendText = nearest.Name;
                countriesInLegend.Add(nearest.Name);
            }
            if (countryColourIndex < 0)
                countryColourIndex = 0;
            countryColourIndex = countryColourIndex % _defaultColors.Count;

            color = _defaultColors[countryColourIndex];
        }

        private IGeographicEntity GetNearestCountry(ConstituencyViewModel constituency)
        {
            var nearestCountries = _constituenciesModel.NearestCountries[_nearestCountryCount];
            IGeographicEntity nearest = null;
            int validCountryCount = 0;
            foreach (var neigbour in constituency.Geography.Neighbours)
            {
                foreach (var country in nearestCountries)
                {
                    if (neigbour.Neighbour.Name == country.Name)
                    {
                        validCountryCount++;
                        if (validCountryCount > _nearestCountryCount)
                        {
                            nearest = country;
                            break;
                        }
                    }
                }

                if (nearest != null)
                    break;
            }
            return nearest;
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public ObservableCollection<ConstituencyViewModel> Constituencies { get; set; }

        public ConstituencyViewModel SelectedConstituency
        {
            get
            {
                return _selectedConstituency;
            }
            set
            {
                _selectedConstituency = value;
                InitialiseChartViewModel();
                OnPropertyChanged(() => SelectedConstituency);
                OnPropertyChanged(() => ConstituencyPlot);
                InitialiseAllConstituenciesChartViewModel();
                OnPropertyChanged(() => UkConstituenciesPlot);
            }
        }

        public AllConstituencesViewModel UkConstituencies
        {
            get
            {
                return _allConstituences;
            }
            set
            {
                _allConstituences = value;
                InitialiseAllConstituenciesChartViewModel();
                OnPropertyChanged(() => UkConstituenciesPlot);
            }
        }

        public PlotModel ConstituencyPlot
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged(() => ConstituencyPlot); }
        }
        public PlotModel UkConstituenciesPlot
        {
            get { return _plotModelAllConstuencies; }
            set { _plotModelAllConstuencies = value; OnPropertyChanged(() => UkConstituenciesPlot); }
        }
        
        public OxyColor OxySelectedConstituencyColour
        {
            get { return _oxySelectedConstituencyColour; }
            set
            {
                _oxySelectedConstituencyColour = value;
                OnPropertyChanged(() => OxySelectedConstituencyColour);
                OnPropertyChanged(() => SelectedConstituencyColour);
            }
        }
        public Color SelectedConstituencyColour
        {
            get
            {
                return Color.FromArgb(
                    _oxySelectedConstituencyColour.A,
                    _oxySelectedConstituencyColour.R,
                    _oxySelectedConstituencyColour.G,
                    _oxySelectedConstituencyColour.B);
            }
            set
            {
                _oxySelectedConstituencyColour = OxyPlot.OxyColor.FromArgb(value.A, value.R, value.G, value.B);
                OnPropertyChanged(() => OxySelectedConstituencyColour);
                OnPropertyChanged(() => SelectedConstituencyColour);
                InitialiseAllConstituenciesChartViewModel();
                OnPropertyChanged(() => UkConstituenciesPlot);
            }
        }

        public OxyColor OxyNeighbourConstituenciesColour
        {
            get { return _oxyNeighbourConstituenciesColour; }
            set
            {
                _oxyNeighbourConstituenciesColour = value;
                OnPropertyChanged(() => OxyNeighbourConstituenciesColour);
                OnPropertyChanged(() => NeighbourConstituenciesColour);
            }
        }
        public Color NeighbourConstituenciesColour
        {
            get
            {
                return Color.FromArgb(
                    _oxyNeighbourConstituenciesColour.A,
                    _oxyNeighbourConstituenciesColour.R,
                    _oxyNeighbourConstituenciesColour.G,
                    _oxySelectedConstituencyColour.B);
            }
            set
            {
                _oxyNeighbourConstituenciesColour = OxyPlot.OxyColor.FromArgb(value.A, value.R, value.G, value.B);
                OnPropertyChanged(() => OxyNeighbourConstituenciesColour);
                OnPropertyChanged(() => NeighbourConstituenciesColour);
                InitialiseAllConstituenciesChartViewModel();
                OnPropertyChanged(() => UkConstituenciesPlot);
            }
        }

        public int NearestCountries
        {
            get
            {
                return 1 + _nearestCountryCount;
            }
            set
            {
                if (value < 1 || value >= _constituenciesModel.MaxNearCountrySets) return;
                _nearestCountryCount = value -1;
                InitialiseChartViewModel();
                OnPropertyChanged(() => SelectedConstituency);
                OnPropertyChanged(() => ConstituencyPlot);
                InitialiseAllConstituenciesChartViewModel();
                OnPropertyChanged(() => UkConstituenciesPlot);
            }
        }
        #endregion

        #region Member variables

        protected PlotModel _plotModel;
        protected PlotModel _plotModelAllConstuencies;

        private MainWindow _mainWindow;
        private ConstituenciesModel _constituenciesModel;

        private double _startPointEast = 0;
        private double _startPointNorth = 0;

        private int _nearestCountryCount = 0;

        private ICommand _printToPngCommand;
        private ICommand _printUkToPngCommand;
        private ConstituencyViewModel _selectedConstituency;
        private AllConstituencesViewModel _allConstituences;

        private OxyColor _oxySelectedConstituencyColour;
        private OxyColor _oxyNeighbourConstituenciesColour;

        private CartesianCountriesViewModel _countriesVM;

        private OxyPlot.Axes.Axis _eastAxis;
        private OxyPlot.Axes.Axis _northAxis;


        protected readonly List<OxyColor> _defaultColors = new List<OxyColor>
                                            {
                                                OxyColors.Green,
                                                OxyColors.Blue,
                                                OxyColors.Red,
                                                OxyColors.Yellow,
                                                OxyColors.DarkSlateGray,
                                                OxyColors.Coral,
                                                OxyColors.Chartreuse,
                                                OxyColors.Thistle,
                                                OxyColors.OrangeRed,
                                                OxyColors.Plum,
                                                OxyColors.DarkCyan,
                                                OxyColors.Magenta,
                                                OxyColors.LimeGreen
                                            };

        private readonly int UkReduction = 50;
        private readonly int ConstituencyReduction = 5;

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

        public ICommand PrintUkToPngCommand
        {
            get
            {
                return _printUkToPngCommand ??
                    (_printUkToPngCommand = new CommandHandler(() => PrintUkToPngCommandAction(), true));
            }
        }
        #endregion

        #region Command Handlers

        public void PrintToPngCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = _constituenciesModel.LastPngFile;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|PNG files (*.png)|*.png";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _constituenciesModel.LastPngFile = fileDialog.FileName;

                Properties.Settings.Default.LastPngFile =
                    _constituenciesModel.LastPngFile;
                Properties.Settings.Default.Save();

                using (var stream = File.Create(fileDialog.FileName))
                {
                    OxyPlot.Wpf.PngExporter.Export(_plotModel, stream, 800, 600, OxyColors.White);
                }
            }
        }

        public void PrintUkToPngCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = _constituenciesModel.LastPngFile;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|PNG files (*.png)|*.png";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _constituenciesModel.LastPngFile = fileDialog.FileName;

                Properties.Settings.Default.LastPngFile =
                    _constituenciesModel.LastPngFile;
                Properties.Settings.Default.Save();

                using (var stream = File.Create(fileDialog.FileName))
                {
                    OxyPlot.Wpf.PngExporter.Export(_plotModelAllConstuencies, stream, 800, 600, OxyColors.White);
                }
            }
        }

        #endregion

    }
}
