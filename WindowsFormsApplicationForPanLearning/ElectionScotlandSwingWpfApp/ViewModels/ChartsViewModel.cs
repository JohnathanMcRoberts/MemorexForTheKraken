using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Data;

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

        private Dictionary<string, OxyColor> _partyNameToColorLookup = null;

        private PlotModel _plotPreviousOverallSeat;
        private PlotModel _plotPredictedOverallSeat;
        
        #endregion

        #region Constructor

        public ChartsViewModel(MainWindow mainWindow, log4net.ILog log, 
            Models.MainModel mainModel, MainViewModel mainViewModel)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = mainViewModel;

            CreatePreviousOverallPlotModel();
            CreatePredictedOverallPlotModel();
        }

        private void CreatePredictedOverallPlotModel()
        {
            // Create the plot model & controller 
            var tmp =
                new PlotModel
                {
                    Title = "Predicted Election seats distribution",
                    Subtitle = "using OxyPlot only"
                };
            // Set the Model property, the INotifyPropertyChanged event will 
            //  make the WPF Plot control update its content
            PlotPredictedOverallSeatsModel = tmp;
            PlotPredictedOverallSeatsViewController = new CustomPlotController();
        }

        private void CreatePreviousOverallPlotModel()
        {
            // Create the plot model & controller 
            var tmp =
                new PlotModel
                {
                    Title = "Previous Election seats distribution",
                    Subtitle = "using OxyPlot only"
                };
            // Set the Model property, the INotifyPropertyChanged event will 
            //  make the WPF Plot control update its content
            PlotPreviousOverallSeatsModel = tmp;
            PlotPreviousOverallSeatsViewController = new CustomPlotController();
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

        #endregion

        #region Public properties

        public PlotModel PlotPreviousOverallSeatsModel
        {
            get { return _plotPreviousOverallSeat; }
            private set { _plotPreviousOverallSeat = value; }
        }

        public IPlotController PlotPreviousOverallSeatsViewController { get; private set; }


        public PlotModel PlotPredictedOverallSeatsModel
        {
            get { return _plotPredictedOverallSeat; }
            private set { _plotPredictedOverallSeat = value; }
        }

        public IPlotController PlotPredictedOverallSeatsViewController { get; private set; }

        public Dictionary<string, OxyColor> PartyNameToColorLookup
        {
            get
            {
                if (_partyNameToColorLookup == null)
                    SetupPartyNameToColorLookup();
                return _partyNameToColorLookup;
            }
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            ResetPlot();
            OnPropertyChanged("");
        }

        #endregion

        #region Utility Functions for Plots

        private void ResetPlot()
        {
            GeneratePlotForPlotPreviousOverallSeatsModel();
            GeneratePlotForPlotPredictedOverallSeatsModel();
        }

        private void SetupPartyNameToColorLookup()
        {
            _partyNameToColorLookup = new Dictionary<string, OxyColor>();
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.SNP], OxyColors.Yellow);
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.Labour], OxyColors.Red);
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.Conservative], OxyColors.Blue);
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.LiberalDemocrat], OxyColors.Orange);
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.Green], OxyColors.Green);
            _partyNameToColorLookup.Add(
                MainModel.MajorPartyNames[(int)MainModel.MajorNamePartyLookup.UKIP], OxyColors.Purple);
        }

        private void GeneratePlotForPlotPreviousOverallSeatsModel()
        {
            var modelP1 = new PlotModel { Title = "Previous Election Overall seats" };

            dynamic seriesP1 = new PieSeries 
                { 
                    StrokeThickness = 2.0, 
                    InsideLabelPosition = 0.8, 
                                
                    AngleSpan = 360, 
                    StartAngle = 90,        
                 
                    InsideLabelFormat = "{0}", 
                    OutsideLabelFormat = "{1}",                                
                    TrackerFormatString = "{1} {2:0.0}",                                
                    LabelField = "{0} {1} {2:0.0}"
                };

            foreach (var party in _mainModel.PartyOverallForecasts)
            {
                string name = party.Name;
                int previousSeats = party.PreviousSeats;
                OxyColor color = PartyNameToColorLookup.ContainsKey(name) ?
                    PartyNameToColorLookup[name] : OxyColors.Gray;
                bool isExploded = (previousSeats < 20);

                if (previousSeats > 0)
                    seriesP1.Slices.Add(
                        new PieSlice(name, previousSeats) { IsExploded = isExploded, Fill = color });

            }

            modelP1.Series.Add(seriesP1);

            PlotPreviousOverallSeatsModel = modelP1;
        }

        private void GeneratePlotForPlotPredictedOverallSeatsModel()
        {
            var modelP1 = new PlotModel { Title = "Predicted Election Overall seats" };

            dynamic seriesP1 = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,

                AngleSpan = 360,
                StartAngle = 90,

                InsideLabelFormat = "{0}",
                OutsideLabelFormat = "{1}",
                TrackerFormatString = "{1} {2:0.0}",
                LabelField = "{0} {1} {2:0.0}"
            };

            foreach (var party in _mainModel.PartyOverallForecasts)
            {
                string name = party.Name;
                int seats = party.PredictedSeats;
                OxyColor color = PartyNameToColorLookup.ContainsKey(name) ?
                    PartyNameToColorLookup[name] : OxyColors.Gray;
                bool isExploded = (seats < 20);

                if (seats > 0)
                    seriesP1.Slices.Add(
                        new PieSlice(name, seats) { IsExploded = isExploded, Fill = color });
            }

            modelP1.Series.Add(seriesP1);

            PlotPredictedOverallSeatsModel = modelP1;
        }

        #endregion

    }
}
