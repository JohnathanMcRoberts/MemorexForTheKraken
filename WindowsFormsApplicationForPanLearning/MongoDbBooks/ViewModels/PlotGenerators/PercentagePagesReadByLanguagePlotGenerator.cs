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
    public class PercentagePagesReadByLanguagePlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupPercentagePagesReadByLanguagePlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupPercentagePagesReadByLanguagePlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Percentage Pages Read by Language With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Percentage Pages Read by Language With Time Plot");
            SetupPercentagePagesReadKeyVsTimeAxes(newPlot);

            // get the languages (in order)
            BooksDelta.DeltaTally latestTally = _mainModel.BookDeltas.Last().OverallTally;
            List<string> languages = (from item in latestTally.LanguageTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, LineSeries>> languagesSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < languages.Count; i++)
            {
                LineSeries languageSeries;
                OxyPlotUtilities.CreateLongLineSeries(out languageSeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.PercentagePagesReadKey, languages[i], i);
                languagesSeries.Add(
                    new KeyValuePair<string, LineSeries>(languages[i], languageSeries));
            }

            // loop through the deltas adding points for each of the items
            foreach (var delta in _mainModel.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var languageLine in languagesSeries)
                {
                    double percentage = 0.0;
                    foreach (var langTotal in delta.OverallTally.LanguageTotals)
                        if (langTotal.Item1 == languageLine.Key)
                            percentage = langTotal.Item5;
                    languageLine.Value.Points.Add(new DataPoint(date, percentage));
                }
            }

            // add them to the plot
            foreach (var languagesItems in languagesSeries)
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
