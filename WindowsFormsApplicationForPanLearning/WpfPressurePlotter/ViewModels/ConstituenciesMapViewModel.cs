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
        public ConstituenciesMapViewModel(MainWindow mainWindow, log4net.ILog log, CartesianCountriesViewModel countriesVM)
        {
            _mainWindow = mainWindow;
            Log = log;
            _constituenciesModel = new ConstituenciesModel(log);
            _countriesVM = countriesVM;

            _oxySelectedConstituencyColour = OxyColors.Green;
            _oxyNeighbourConstituenciesColour = OxyColors.LightBlue;

            Constituencies = new ObservableCollection<ConstituencyViewModel>();
            _selectedConstituency = null;

            List<IGeographicEntity> countries = new List<IGeographicEntity>();
            foreach (var country in countriesVM.AllCountriesModel.Countries)
                countries.Add(country);

            bool isNeighbourDistancesFile = false;

            // make this a function
            isNeighbourDistancesFile = true;

            foreach (var constituency in _constituenciesModel.Constituencies)
            {
                if (!isNeighbourDistancesFile)
                {
                    constituency.Neighbours =
                       NeighbouringGeography.SetupNeighbours(constituency, countries);
                }
                ConstituencyViewModel constituencyVM = new ConstituencyViewModel(_mainWindow, constituency, log);

                if (!isNeighbourDistancesFile)
                {
                    for (int i = 0; i < 40 && i < constituency.Neighbours.Count; ++i)
                        log.Debug(
                            "Country " + (1 + i) + "th nearest (" + constituency.Name
                            + ") is " + constituency.Neighbours[i].Neighbour.Name + "\n  @ C2C=" +
                            constituency.Neighbours[i].CentreToCentreDistance + "\n  @ E2E=" +
                            constituency.Neighbours[i].EdgeToEdgeDistance);
                }

                Constituencies.Add(constituencyVM);
                if (_selectedConstituency == null)
                    _selectedConstituency = constituencyVM;
            }
            UkConstituencies = new AllConstituencesViewModel(_mainWindow, _constituenciesModel, log);
            SelectedConstituency = _selectedConstituency;
            if (!isNeighbourDistancesFile)
                WriteOutConstituencyNeighbourDistances();

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

        private void AddAreaSeriesToPlot(
            out double minEast, out double minNorth, out double maxEast, out double maxNorth, IGeographicalEntityViewModel constituency,
            OxyPlot.OxyColor colour, PlotModel plotModel,  int reduction)
        {
            minEast = constituency.MinLongitude;
            minNorth = constituency.MinLatitude;

            maxEast = constituency.MaxLongitude;
            maxNorth = constituency.MaxLatitude;


            int i = 0;
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

        private void InitialiseAllConstituenciesChartViewModel()
        {
            _plotModelAllConstuencies = new PlotModel();
            SetUpModel(_plotModelAllConstuencies, _allConstituences);


            double startPointEast;
            double startPointNorth;
            SetupMasterPoint(_allConstituences, out startPointEast, out startPointNorth);

            double minEast;
            double minNorth;
            double maxEast;
            double maxNorth;

            foreach (var constituency in Constituencies)
            {
                if (constituency.Name == _selectedConstituency.Name)
                    continue ;

                AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth,
                    constituency, _oxyNeighbourConstituenciesColour, _plotModelAllConstuencies, UkReduction);
            }


            AddAreaSeriesToPlot(out minEast, out minNorth, out maxEast, out maxNorth,
                _selectedConstituency, _oxySelectedConstituencyColour, _plotModelAllConstuencies, UkReduction);

            OnPropertyChanged(() => UkConstituenciesPlot);
        }

        private void WriteOutConstituencyNeighbourDistances()
        {
            using (XmlWriter writer = XmlWriter.Create("ConstituencyAndNeighbourDistances.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Constituencies");

                foreach (var constituency in _constituenciesModel.Constituencies)
                {
                    writer.WriteStartElement("Constituency");

                    writer.WriteElementString("Name", constituency.Name);

                    writer.WriteStartElement("Neighbours");
                    foreach (var neighbour in constituency.Neighbours)
                    {
                        writer.WriteStartElement("Neighbour");

                        writer.WriteElementString("Name", neighbour.Neighbour.Name);
                        writer.WriteElementString("TotalArea",
                            neighbour.Neighbour.TotalArea.ToString());
                        writer.WriteElementString("CentroidLatitude",
                            neighbour.Neighbour.CentroidLatitude.ToString());
                        writer.WriteElementString("CentroidLongitude",
                            neighbour.Neighbour.CentroidLongitude.ToString());
                        writer.WriteElementString("EdgeToEdgeDistance",
                            neighbour.EdgeToEdgeDistance.ToString());
                        writer.WriteElementString("CentreToCentreDistance",
                            neighbour.CentreToCentreDistance.ToString());

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
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

        #endregion

        #region Member variables

        protected PlotModel _plotModel;
        protected PlotModel _plotModelAllConstuencies;

        private MainWindow _mainWindow;
        private ConstituenciesModel _constituenciesModel;

        private double _startPointEast = 0;
        private double _startPointNorth = 0;

        private ICommand _printToPngCommand;
        private ConstituencyViewModel _selectedConstituency;
        private AllConstituencesViewModel _allConstituences;

        private OxyColor _oxySelectedConstituencyColour;
        private OxyColor _oxyNeighbourConstituenciesColour;

        private CartesianCountriesViewModel _countriesVM;

        private OxyPlot.Axes.Axis _eastAxis;
        private OxyPlot.Axes.Axis _northAxis;


        protected readonly List<OxyColor> _defaultColors = new List<OxyColor>
                                            {
                                                OxyColors.Blue,
                                                OxyColors.Red,
                                                //OxyColors.Green,
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

        #endregion

    }
}
