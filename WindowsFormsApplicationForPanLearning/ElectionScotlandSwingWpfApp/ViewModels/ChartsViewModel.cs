using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Data;
using System.Windows.Input;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using ElectionScotlandSwingWpfApp.ViewModels.Utilities;
using ElectionScotlandSwingWpfApp.Models;

namespace ElectionScotlandSwingWpfApp.ViewModels
{
    public class ChartsViewModel : INotifyPropertyChanged
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

        private ICommand _generatePairPredictionCommand;
        
        #endregion

        #region Constructor

        public ChartsViewModel(MainWindow mainWindow, log4net.ILog log, 
            Models.MainModel mainModel, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = mainViewModel;

            PlotPreviousOverallSeats = 
                new PlotModelAndController("Previous Election seats distribution");
            PlotPredictedOverallSeats =
                new PlotModelAndController("Predicted Election seats distribution");

            PlotPreviousConstituencySeats = 
                new PlotModelAndController("Previous Election seats distribution");
            PlotPredictedConstituencySeats =
                new PlotModelAndController("Predicted Election seats distribution");

            PlotPreviousListSeats = 
                new PlotModelAndController("Previous Election seats distribution");
            PlotPredictedListSeats =
                new PlotModelAndController("Predicted Election seats distribution");

            PartyNames = new List<string>();
            foreach(var party in MainModel.MajorPartyNames)
                PartyNames.Add(party);
            FirstPartyName = PartyNames[0];
            SecondPartyName = PartyNames[1];
            PlotPairPrediction =
                new PlotModelAndController("Predicted Party Split seats distribution");

        }

        #endregion

        #region Nested Classes

        public class CustomPlotController : PlotController
        {
            public CustomPlotController()
            {
                this.BindKeyDown(OxyKey.Left, PlotCommands.PanRight);
                this.BindKeyDown(OxyKey.Right, PlotCommands.PanLeft);
            }
        }

        public class PlotModelAndController : INotifyPropertyChanged
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

            #region Constructor
            public PlotModelAndController(string title)
            {
                // Create the plot model & controller 
                var tmp =
                    new PlotModel { Title = title, Subtitle = "using OxyPlot only" };

                // Set the Model property, the INotifyPropertyChanged event will 
                //  make the WPF Plot control update its content
                Model = tmp;
                ViewController = new CustomPlotController();
            }

            #endregion

            #region Private Fields

            private PlotModel _model;

            #endregion

            #region Public Properties

            public PlotModel Model
            {
                get { return _model; }
                set { _model = value; }
            }

            public IPlotController ViewController { get; private set; }

            #endregion
        }

        #endregion

        #region Public Properties

        #region Pie Charts Data
        public PlotModelAndController PlotPreviousOverallSeats { get; private set; }

        public PlotModelAndController PlotPredictedOverallSeats { get; private set; }


        public PlotModelAndController PlotPreviousConstituencySeats { get; private set; }

        public PlotModelAndController PlotPredictedConstituencySeats { get; private set; }


        public PlotModelAndController PlotPreviousListSeats { get; private set; }

        public PlotModelAndController PlotPredictedListSeats { get; private set; }

        #endregion

        #region Paired Voting Chart

        public List<string> PartyNames { get; private set; }
        public string FirstPartyName { get; set; }
        public string SecondPartyName { get; set; }

        public PlotModelAndController PlotPairPrediction { get; private set; }

        #endregion

        #endregion

        #region Public Methods

        public void Refresh()
        {
            ResetPlot();
            OnPropertyChanged("");
        }

        #endregion

        #region Private Functions for Plots

        private void ResetPlot()
        {
            GeneratePlotForPlotPreviousOverallSeatsModel();
            GeneratePlotForPlotPredictedOverallSeatsModel();

            GeneratePlotForPlotPreviousConstituencySeatsModel();
            GeneratePlotForPlotPredictedConstituencySeatsModel();

            GeneratePlotForPlotPreviousListSeatsModel();
            GeneratePlotForPlotPredictedListSeatsModel();
        }

        private void GeneratePlotForPlotPreviousOverallSeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyOverallForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PreviousSeats));

            PlotPreviousOverallSeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Previous Election Overall seats");
        }

        private void GeneratePlotForPlotPredictedOverallSeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyOverallForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PredictedSeats));

            PlotPredictedOverallSeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Predicted Election Overall seats");
        }

        private void GeneratePlotForPlotPreviousConstituencySeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyConstituencyForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PreviousSeats));

            PlotPreviousConstituencySeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Previous Election Constituency seats");
        }

        private void GeneratePlotForPlotPredictedConstituencySeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyConstituencyForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PredictedSeats));

            PlotPredictedConstituencySeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Predicted Election Constituency seats");
        }

        private void GeneratePlotForPlotPreviousListSeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyListForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PreviousSeats));

            PlotPreviousListSeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Previous Election List seats");
        }

        private void GeneratePlotForPlotPredictedListSeatsModel()
        {
            List<KeyValuePair<string, int>> partyResults = new List<KeyValuePair<string, int>>();
            foreach (var party in _mainModel.PartyListForecasts)
                partyResults.Add(new KeyValuePair<string, int>(party.Name, party.PredictedSeats));

            PlotPredictedListSeats.Model =
                ChartUtilities.CreatePieSeriesModelForPartyResults(
                    partyResults, "Predicted Election List seats");
        }

        #endregion

        #region Commands

        public ICommand GeneratePairPredictionCommand
        {
            get
            {
                return _generatePairPredictionCommand ??
                    (_generatePairPredictionCommand =
                        new CommandHandler(() => GeneratePairPredictionCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void GeneratePairPredictionCommandAction()
        {
            using (new WaitCursor())
            {
                _mainModel.GeneratePairPrediction(
                    FirstPartyName, SecondPartyName
                    );

                _parent.UpdateData();

                GeneratePlotForPairedPrediction();

                OnPropertyChanged("");
            }

        }


        public static IEnumerable<AreaSeries> StackLineSeries(IList<LineSeries> series)
        {
            double[] total = new double[series[0].Points.Count];

            LineSeries lineSeries;
            AreaSeries areaSeries;

            for (int s = 0; s < series.Count; s++)
            {
                lineSeries = series[s];
                areaSeries = new AreaSeries()
                {
                    Title = lineSeries.Title,
                    Color = lineSeries.Color,
                };

                for (int p = 0; p < lineSeries.Points.Count; p++)
                {
                    double x = lineSeries.Points[p].X;
                    double y = lineSeries.Points[p].Y;

                    areaSeries.Points.Add(new DataPoint(x, total[p]));
                    total[p] += y;
                    areaSeries.Points2.Add(new DataPoint(x, total[p]));
                }

                yield return areaSeries;
            }
        }

        public static void SetupPlotLegend(PlotModel newPlot,
            string title = "Performance Curves")
        {
            newPlot.LegendTitle = title;
            newPlot.LegendOrientation = LegendOrientation.Horizontal;
            newPlot.LegendPlacement = LegendPlacement.Outside;
            newPlot.LegendPosition = LegendPosition.TopRight;
            newPlot.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            newPlot.LegendBorder = OxyColors.Black;
        }

        const string VoteKey = "FirstPartyVoteKey";
        const string SeatsKey = "SeatsKey";

        private void SetupTotalPagesReadKeyVsTimeAxes(PlotModel newPlot)
        {
            //var xAxis = new DateTimeAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    Title = "FirstPartyVoteKey",
            //    Key = VoteKey,
            //    MajorGridlineStyle = LineStyle.Solid,
            //    MinorGridlineStyle = LineStyle.None,
            //    StringFormat = "yyyy-MM-dd"
            //}; 
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "First Party Vote",
                Key = VoteKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                //StringFormat = "{0} %"
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Total Seats",
                Key = SeatsKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }

        public static void CreateLongLineSeries(out LineSeries series, string xAxisKey,
            string yAxisKey, string title, int colourIndex, byte aValue = 225)
        {
            //List<OxyColor> coloursArray = SetupStandardColourSet(aValue);

            //int index = colourIndex % coloursArray.Count;
            var colour = ChartUtilities.PartyNameToColorLookup[title];

            series = new LineSeries
            {
                Title = title,
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = colour
            };
        }

        public static void CreateLongAreaSeries(out AreaSeries series, string xAxisKey,
            string yAxisKey, string title, int colourIndex, byte aValue = 225)
        {
            //List<OxyColor> coloursArray = SetupStandardColourSet(aValue);

            //int index = colourIndex % coloursArray.Count;
            var colour = ChartUtilities.PartyNameToColorLookup[title];

            series = new AreaSeries
            {
                Title = title,
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = colour,
                Color2 = OxyColors.Transparent
            };
        }

        private void GeneratePlotForPairedPrediction()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Total Pages Read by Country With Time Plot" };
            SetupPlotLegend(newPlot, "Total Pages Read by Country With Time Plot");
            SetupTotalPagesReadKeyVsTimeAxes(newPlot);

            // get the parties (in order) - need to fix!!!
            List<string> parties = (MainModel.MajorPartyNames).ToList();

            // create the series for the partie
            List<KeyValuePair<string, LineSeries>> partiesLineSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < parties.Count; i++)
            {
                LineSeries lineSeries;
                CreateLongLineSeries(out lineSeries,
                    VoteKey, SeatsKey, parties[i], i, 128);
                partiesLineSeries.Add(
                    new KeyValuePair<string, LineSeries>(parties[i], lineSeries));

                //if (i > 1) break;
            }

            // loop through the deltas adding points for each of the items to the lines
            foreach (var pair in _mainModel.FirstPartyVotesAndPairedPredictionTotals)
            {
                var vote = pair.Key;
                var partyResults = pair.Value;
                foreach (var partyLine in partiesLineSeries)
                {
                    double ttlSeats = 0.0;
                    foreach (var partyResult in partyResults)
                        if (partyResult.Name == partyLine.Key)
                            ttlSeats = partyResult.PredictedSeats;
                    partyLine.Value.Points.Add(new DataPoint(vote, ttlSeats));
                }
            }

            IList<LineSeries> lineSeriesSet =
                (from item in partiesLineSeries select item.Value).ToList();
            var stackSeries = StackLineSeries(lineSeriesSet);

            // add them to the plot
            foreach (var languageItems in stackSeries)
                newPlot.Series.Add(languageItems);

            PlotPairPrediction.Model = newPlot;
        }

        #endregion

    }
}
