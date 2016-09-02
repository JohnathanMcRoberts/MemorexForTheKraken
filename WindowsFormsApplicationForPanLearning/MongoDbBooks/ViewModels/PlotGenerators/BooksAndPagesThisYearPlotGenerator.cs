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

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class BooksAndPagesThisYearPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupBooksAndPagesThisYearPlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupBooksAndPagesThisYearPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Books and Pages per Year Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Books and Pages per Year Plot");
            SetupBookAndPagesPerYearVsTimeAxes(newPlot);

            if (_mainModel.BookPerYearDeltas.Count < 1)
                return newPlot;

            // create series and add them to the plot
            LineSeries booksReadSeries;
            LineSeries booksReadTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out booksReadSeries, ChartAxisKeys.DateKey, ChartAxisKeys.BooksReadKey, "Books Read", 1);
            OxyPlotUtilities.CreateLineSeries(out booksReadTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.BooksReadKey, "Books Read Trendline", 4);
            
            ICurveFitter curveFitterBooks;
            GetBooksReadWithTimeCurveFitter(out curveFitterBooks);
 

            LineSeries pagesReadSeries;
            LineSeries pagesReadTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out pagesReadSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesPerDayKey, "Pages Read", 0);
            OxyPlotUtilities.CreateLineSeries(out pagesReadTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.PagesPerDayKey, "Pages Read Trendline", 3
                );
            
            ICurveFitter curveFitterPages;
            GetPagesReadWithTimeCurveFitter(out curveFitterPages);


            DateTime start = _mainModel.BookPerYearDeltas[0].Date;
            foreach (var delta in _mainModel.BookPerYearDeltas)
            {
                int daysSinceStart = (delta.Date - start).Days;

                double trendBooks = curveFitterBooks.EvaluateYValueAtPoint(daysSinceStart);
                double trendPages = curveFitterPages.EvaluateYValueAtPoint(daysSinceStart);

                booksReadSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.TotalBooks));
                booksReadTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendBooks));

                pagesReadSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.PageRate));
                pagesReadTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendPages));
            }

            OxyPlotUtilities.AddLineSeriesToModel(newPlot,
                new LineSeries[] { booksReadSeries, booksReadTrendlineSeries, 
                    pagesReadSeries, pagesReadTrendlineSeries }
                );


            // finally update the model with the new plot
            return newPlot;
        }

        private void SetupBookAndPagesPerYearVsTimeAxes(PlotModel newPlot)
        {
            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = ChartAxisKeys.DateKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                StringFormat = "yyyy-MM-dd"
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Books Read",
                Key = ChartAxisKeys.BooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);

            var rhsAxis = new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Pages Read Per Day",
                Key = ChartAxisKeys.PagesPerDayKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(rhsAxis);
        }

        private void GetBooksReadWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            DateTime start = _mainModel.BookPerYearDeltas[0].Date;

            foreach (var delta in _mainModel.BookPerYearDeltas)
            {
                int daysSinceStart = (delta.Date - start).Days;
                xVals.Add(daysSinceStart);
                yVals.Add(delta.OverallTally.TotalBooks);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }

        private void GetPagesReadWithTimeCurveFitter(out ICurveFitter curveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yVals = new List<double>();

            DateTime start = _mainModel.BookPerYearDeltas[0].Date;

            foreach (var delta in _mainModel.BookPerYearDeltas)
            {
                int daysSinceStart = (delta.Date - start).Days;
                xVals.Add(daysSinceStart);
                yVals.Add(delta.OverallTally.PageRate);
            }

            curveFitter = new QuadraticCurveFitter(xVals, yVals);
        }
    }
}
