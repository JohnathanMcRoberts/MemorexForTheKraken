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
    public class AverageDaysPerBookPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupAverageDaysPerBookPlot();
        }
        private Models.MainBooksModel _mainModel;


        private PlotModel SetupAverageDaysPerBookPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Average Days Per Book Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Average Days Per Book Plot");
            SetupDaysPerBookVsTimeAxes(newPlot);

            // create series and add them to the plot
            LineSeries overallSeries;
            LineSeries lastTenSeries;
            LineSeries overallTrendlineSeries;
            LineSeries lastTenTrendlineSeries;
            OxyPlotUtilities.CreateLineSeries(out overallSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall", 1);
            OxyPlotUtilities.CreateLineSeries(out overallTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Overall Trendline", 4);
            OxyPlotUtilities.CreateLineSeries(out lastTenSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Last 10", 0);
            OxyPlotUtilities.CreateLineSeries(out lastTenTrendlineSeries, ChartAxisKeys.DateKey, ChartAxisKeys.DaysPerBookKey, "Last 10 Trendline", 3);

            ICurveFitter lastTenCurveFitter;
            ICurveFitter overallCurveFitter;
            GetAverageDaysPerBookCurveFitters(out lastTenCurveFitter, out overallCurveFitter);


            foreach (var delta in _mainModel.BookDeltas)
            {
                double trendOverallDaysPerBook =
                    overallCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);
                double trendLastTenDaysPerBook =
                    lastTenCurveFitter.EvaluateYValueAtPoint(delta.DaysSinceStart);

                overallSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.OverallTally.DaysPerBook));
                lastTenSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), delta.LastTenTally.DaysPerBook));

                overallTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendOverallDaysPerBook));
                lastTenTrendlineSeries.Points.Add(
                    new DataPoint(DateTimeAxis.ToDouble(delta.Date), trendLastTenDaysPerBook));
            }


            OxyPlotUtilities.AddLineSeriesToModel(newPlot,
                new LineSeries[] { overallSeries, lastTenSeries, overallTrendlineSeries, lastTenTrendlineSeries }
                );


            // finally update the model with the new plot
            return newPlot;
        }

        private void GetAverageDaysPerBookCurveFitters(
            out ICurveFitter lastTenCurveFitter, out ICurveFitter overallCurveFitter)
        {
            List<double> xVals = new List<double>();
            List<double> yValsLastTen = new List<double>();
            List<double> yValsOverall = new List<double>();

            foreach (var delta in _mainModel.BookDeltas)
            {
                xVals.Add(delta.DaysSinceStart);
                yValsLastTen.Add(delta.LastTenTally.DaysPerBook);
                yValsOverall.Add(delta.OverallTally.DaysPerBook);
            }

            //lastTenCurveFitter = new LinearCurveFitter(xVals, yValsLastTen);
            lastTenCurveFitter = new QuadraticCurveFitter(xVals, yValsLastTen);
            overallCurveFitter = new QuadraticCurveFitter(xVals, yValsOverall);
        }

        private void GetAverageDaysPerBookLinearTrendlineParameters(out double yintercept, out double slope)
        {
            double rsquared;

            List<double> overallDays = new List<double>();
            List<double> overallDaysPerBook = new List<double>();

            foreach (var delta in _mainModel.BookDeltas)
            {
                overallDays.Add(delta.DaysSinceStart);
                overallDaysPerBook.Add(delta.OverallTally.DaysPerBook);
            }
            OxyPlotUtilities.LinearRegression(overallDays, overallDaysPerBook, out  rsquared, out  yintercept, out  slope);
        }

        private void SetupDaysPerBookVsTimeAxes(PlotModel newPlot)
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
                Title = "Days per Book",
                Key = ChartAxisKeys.DaysPerBookKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Minimum = 0
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}
