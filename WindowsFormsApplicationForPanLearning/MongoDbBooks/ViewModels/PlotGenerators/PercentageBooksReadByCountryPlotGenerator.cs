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
    public class PercentageBooksReadByCountryPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupPercentageBooksReadByCountryPlot();
        }

        private Models.MainBooksModel _mainModel;

        private PlotModel SetupPercentageBooksReadByCountryPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Percentage Books Read by Country With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Percentage Books Read by Country With Time Plot");
            SetupPercentageBooksReadKeyVsTimeAxes(newPlot);

            // get the countries (in order)
            BooksDelta.DeltaTally latestTally = _mainModel.BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> countriesSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < countries.Count; i++)
            {
                LineSeries countrySeries;
                OxyPlotUtilities.CreateLongLineSeries(out countrySeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.PercentageBooksReadKey, countries[i], i);
                countriesSeries.Add(
                    new KeyValuePair<string, LineSeries>(countries[i], countrySeries));
            }

            // loop through the deltas adding points for each of the items
            foreach (var delta in _mainModel.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var countryLine in countriesSeries)
                {
                    double percentage = 0.0;
                    foreach (var countryTotal in delta.OverallTally.CountryTotals)
                        if (countryTotal.Item1 == countryLine.Key)
                            percentage = countryTotal.Item3;
                    countryLine.Value.Points.Add(new DataPoint(date, percentage));
                }
            }

            // add them to the plot
            foreach (var countryItems in countriesSeries)
                newPlot.Series.Add(countryItems.Value);

            // finally update the model with the new plot
            return newPlot;
        }

        private void SetupPercentageBooksReadKeyVsTimeAxes(PlotModel newPlot)
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
                Title = "% of Books Read",
                Key = ChartAxisKeys.PercentageBooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}
