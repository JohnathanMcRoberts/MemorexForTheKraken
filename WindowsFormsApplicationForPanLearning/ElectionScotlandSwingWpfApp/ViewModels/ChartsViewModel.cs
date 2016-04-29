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

        #region Public properties

        public PlotModelAndController PlotPreviousOverallSeats { get; private set; }

        public PlotModelAndController PlotPredictedOverallSeats { get; private set; }

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

        #endregion
    }
}
