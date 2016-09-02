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
    public class PercentagePagesReadByCountryPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupPercentagePagesReadByCountryPlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupPercentagePagesReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Percentage Pages Read by Country With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Percentage Pages Read by Country With Time Plot");
            SetupPercentagePagesReadKeyVsTimeAxes(newPlot);

            // get the countries (in order)
            BooksDelta.DeltaTally latestTally = _mainModel.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item4 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> countriesSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < countries.Count; i++)
            {
                LineSeries lineSeries;
                OxyPlotUtilities.CreateLongLineSeries(out lineSeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.PercentagePagesReadKey, countries[i], i);
                countriesSeries.Add(
                    new KeyValuePair<string, LineSeries>(countries[i], lineSeries));
            }

            // loop through the deltas adding points for each of the items
            foreach (var delta in _mainModel.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var line in countriesSeries)
                {
                    double percentage = 0.0;
                    foreach (var langTotal in delta.OverallTally.CountryTotals)
                        if (langTotal.Item1 == line.Key)
                            percentage = langTotal.Item3;
                    line.Value.Points.Add(new DataPoint(date, percentage));
                }
            }

            // add them to the plot
            foreach (var languagesItems in countriesSeries)
                newPlot.Series.Add(languagesItems.Value);

            // finally update the model with the new plot
            return newPlot;
        }

        private void SetupPercentagePagesReadKeyVsTimeAxes(PlotModel newPlot)
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
                Title = "% of Pages Read",
                Key = ChartAxisKeys.PercentagePagesReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}