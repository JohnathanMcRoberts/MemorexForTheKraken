using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;
using MongoDbBooks.ViewModels.PlotGenerators;

namespace MongoDbBooks.ViewModels
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
        private MainBooksModel _mainModel;
        private MainViewModel _parent;

        private PlotModel _plotOverallBookAndPageTallies;
        private PlotModel _plotDaysPerBook;
        private PlotModel _plotPageRate;
        private PlotModel _plotPagesPerBook;
        private PlotModel _plotBooksInTranslation;
        private PlotModel _plotDaysPerBookWithTime;
        private PlotModel _plotPagesPerDayWithTime;
        private PlotModel _plotAverageDaysPerBook;
        private PlotModel _plotPercentageBooksReadByLanguage;
        private PlotModel _plotTotalBooksReadByLanguage;
        private PlotModel _plotPercentagePagesReadByLanguage;
        private PlotModel _plotTotalPagesReadByLanguage;
        private PlotModel _plotPercentageBooksReadByCountry;
        private PlotModel _plotTotalBooksReadByCountry;
        private PlotModel _plotPercentagePagesReadByCountry;
        private PlotModel _plotTotalPagesReadByCountry;
        private PlotModel _plotBooksAndPagesThisYear;

        #endregion

        #region Public properties

        public PlotModel PlotOverallBookAndPageTalliesModel
        {
            get { return _plotOverallBookAndPageTallies; }
            private set { _plotOverallBookAndPageTallies = value; }
        }
        public IPlotController PlotOverallBookAndPageTalliesViewController { get; private set; }

        public PlotModel PlotDaysPerBookModel
        {
            get { return _plotDaysPerBook; }
            private set { _plotDaysPerBook = value; }
        }
        public IPlotController PlotDaysPerBookViewController { get; private set; }


        public PlotModel PlotPageRateModel
        {
            get { return _plotPageRate; }
            private set { _plotPageRate = value; }
        }
        public IPlotController PlotPageRateViewController { get; private set; }


        public PlotModel PlotPagesPerBookModel
        {
            get { return _plotPagesPerBook; }
            private set { _plotPagesPerBook = value; }
        }
        public IPlotController PlotPagesPerBookViewController { get; private set; }


        public PlotModel PlotBooksInTranslationModel
        {
            get { return _plotBooksInTranslation; }
            private set { _plotBooksInTranslation = value; }
        }
        public IPlotController PlotBooksInTranslationViewController { get; private set; }


        public PlotModel PlotDaysPerBookWithTimeModel
        {
            get { return _plotDaysPerBookWithTime; }
            private set { _plotDaysPerBookWithTime = value; }
        }
        public IPlotController PlotDaysPerBookWithTimeViewController { get; private set; }


        public PlotModel PlotPagesPerDayWithTimeModel
        {
            get { return _plotPagesPerDayWithTime; }
            private set { _plotPagesPerDayWithTime = value; }
        }
        public IPlotController PlotPagesPerDayWithTimeViewController { get; private set; }


        public PlotModel PlotAverageDaysPerBookModel
        {
            get { return _plotAverageDaysPerBook; }
            private set { _plotAverageDaysPerBook = value; }
        }
        public IPlotController PlotAverageDaysPerBookViewController { get; private set; }


        public PlotModel PlotPercentageBooksReadByLanguageModel
        {
            get { return _plotPercentageBooksReadByLanguage; }
            private set { _plotPercentageBooksReadByLanguage = value; }
        }
        public IPlotController PlotPercentageBooksReadByLanguageViewController { get; private set; }


        public PlotModel PlotTotalBooksReadByLanguageModel
        {
            get { return _plotTotalBooksReadByLanguage; }
            private set { _plotTotalBooksReadByLanguage = value; }
        }
        public IPlotController PlotTotalBooksReadByLanguageViewController { get; private set; }


        public PlotModel PlotPercentagePagesReadByLanguageModel
        {
            get { return _plotPercentagePagesReadByLanguage; }
            private set { _plotPercentagePagesReadByLanguage = value; }
        }
        public IPlotController PlotPercentagePagesReadByLanguageViewController { get; private set; }


        public PlotModel PlotTotalPagesReadByLanguageModel
        {
            get { return _plotTotalPagesReadByLanguage; }
            private set { _plotTotalPagesReadByLanguage = value; }
        }
        public IPlotController PlotTotalPagesReadByLanguageViewController { get; private set; }


        public PlotModel PlotPercentageBooksReadByCountryModel
        {
            get { return _plotPercentageBooksReadByCountry; }
            private set { _plotPercentageBooksReadByCountry = value; }
        }
        public IPlotController PlotPercentageBooksReadByCountryViewController { get; private set; }


        public PlotModel PlotTotalBooksReadByCountryModel
        {
            get { return _plotTotalBooksReadByCountry; }
            private set { _plotTotalBooksReadByCountry = value; }
        }
        public IPlotController PlotTotalBooksReadByCountryViewController { get; private set; }


        public PlotModel PlotPercentagePagesReadByCountryModel
        {
            get { return _plotPercentagePagesReadByCountry; }
            private set { _plotPercentagePagesReadByCountry = value; }
        }
        public IPlotController PlotPercentagePagesReadByCountryViewController { get; private set; }


        public PlotModel PlotTotalPagesReadByCountryModel
        {
            get { return _plotTotalPagesReadByCountry; }
            private set { _plotTotalPagesReadByCountry = value; }
        }
        public IPlotController PlotTotalPagesReadByCountryViewController { get; private set; }


        public PlotModel PlotBooksAndPagesThisYearModel
        {
            get { return _plotBooksAndPagesThisYear; }
            private set { _plotBooksAndPagesThisYear = value; }
        }
        public IPlotController PlotBooksAndPagesThisYearViewController { get; private set; }

        #endregion

        #region Constructor

        public ChartsViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;


            PlotOverallBookAndPageTalliesViewController =
                InitialisePlotModelAndController(ref _plotOverallBookAndPageTallies, "OverallBookAndPage");
            PlotDaysPerBookViewController =
                InitialisePlotModelAndController(ref _plotDaysPerBook, "DaysPerBook");
            PlotPageRateViewController =
                InitialisePlotModelAndController(ref _plotPageRate, "PageRate");
            PlotPagesPerBookViewController =
                InitialisePlotModelAndController(ref _plotPagesPerBook, "PagesPerBook");
            PlotBooksInTranslationViewController =
                InitialisePlotModelAndController(ref _plotBooksInTranslation, "BooksInTranslation");
            PlotDaysPerBookWithTimeViewController =
                InitialisePlotModelAndController(ref _plotDaysPerBookWithTime, "DaysPerBook");
            PlotPagesPerDayWithTimeViewController =
                InitialisePlotModelAndController(ref _plotPagesPerDayWithTime, "PagesPerDayWithTime");
            PlotAverageDaysPerBookViewController =
                InitialisePlotModelAndController(ref _plotAverageDaysPerBook, "AverageDaysPerBook");
            PlotPercentageBooksReadByLanguageViewController =
                InitialisePlotModelAndController(ref _plotPercentageBooksReadByLanguage, "PercentageBooksReadByLanguage");
            PlotTotalBooksReadByLanguageViewController =
                InitialisePlotModelAndController(ref _plotTotalBooksReadByLanguage, "TotalBooksReadByLanguage");
            PlotPercentagePagesReadByLanguageViewController =
                InitialisePlotModelAndController(ref _plotPercentagePagesReadByLanguage, "PercentagePagesReadByLanguage");
            PlotTotalPagesReadByLanguageViewController =
                InitialisePlotModelAndController(ref _plotTotalPagesReadByLanguage, "TotalPagesReadByLanguage");
            PlotPercentageBooksReadByCountryViewController =
                InitialisePlotModelAndController(ref _plotPercentageBooksReadByCountry, "PercentageBooksReadByCountry");
            PlotTotalBooksReadByCountryViewController =
                InitialisePlotModelAndController(ref _plotTotalBooksReadByCountry, "TotalBooksReadByCountry");
            PlotPercentagePagesReadByCountryViewController =
                InitialisePlotModelAndController(ref _plotPercentagePagesReadByCountry, "PercentagePagesReadByCountry");
            PlotTotalPagesReadByLanguageViewController =
                InitialisePlotModelAndController(ref _plotTotalPagesReadByCountry, "TotalPagesReadByCountry");
            PlotBooksAndPagesThisYearViewController =
                InitialisePlotModelAndController(ref _plotBooksAndPagesThisYear, "BooksAndPagesThisYear");

        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            PlotOverallBookAndPageTalliesModel = (new OverallBookAndPageTalliesPlotGenerator()).SetupPlot(_mainModel);
            PlotDaysPerBookModel = (new DaysPerBookPlotGenerator()).SetupPlot(_mainModel);
            PlotPageRateModel = (new PageRatePlotGenerator()).SetupPlot(_mainModel);
            PlotPagesPerBookModel = (new PagesPerBookPlotGenerator()).SetupPlot(_mainModel);
            PlotBooksInTranslationModel = (new BooksInTranslationPlotGenerator()).SetupPlot(_mainModel);
            PlotDaysPerBookWithTimeModel = (new DaysPerBookWithTimePlotGenerator()).SetupPlot(_mainModel);
            PlotPagesPerDayWithTimeModel = (new PagesPerDayWithTimePlotGenerator()).SetupPlot(_mainModel);
            PlotAverageDaysPerBookModel = (new AverageDaysPerBookPlotGenerator()).SetupPlot(_mainModel);
            PlotPercentageBooksReadByLanguageModel = (new PercentageBooksReadByLanguagePlotGenerator()).SetupPlot(_mainModel);
            PlotTotalBooksReadByLanguageModel = (new TotalBooksReadByLanguagePlotGenerator()).SetupPlot(_mainModel);
            PlotPercentagePagesReadByLanguageModel = (new PercentagePagesReadByLanguagePlotGenerator()).SetupPlot(_mainModel);
            PlotTotalPagesReadByLanguageModel = (new TotalPagesReadByLanguagePlotGenerator()).SetupPlot(_mainModel);
            PlotPercentageBooksReadByCountryModel = (new PercentageBooksReadByCountryPlotGenerator()).SetupPlot(_mainModel);
            PlotTotalBooksReadByCountryModel = (new TotalBooksReadByCountryPlotGenerator()).SetupPlot(_mainModel);
            PlotPercentagePagesReadByCountryModel = (new PercentagePagesReadByCountryPlotGenerator()).SetupPlot(_mainModel);
            PlotTotalPagesReadByCountryModel = (new TotalPagesReadByCountryPlotGenerator()).SetupPlot(_mainModel);
            PlotBooksAndPagesThisYearModel = (new BooksAndPagesThisYearPlotGenerator()).SetupPlot(_mainModel);

            OnPropertyChanged("");
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

        #region Common Plotting Functions

        private IPlotController InitialisePlotModelAndController(ref PlotModel plot, string title)
        {
            // Create the plot model & controller 
            var tmp = new PlotModel { Title = title, Subtitle = "using OxyPlot only" };

            // Set the Model property, the INotifyPropertyChanged event will 
            //  make the WPF Plot control update its content
            plot = tmp;
            return new CustomPlotController();
        }

        #endregion
        
    }
}
