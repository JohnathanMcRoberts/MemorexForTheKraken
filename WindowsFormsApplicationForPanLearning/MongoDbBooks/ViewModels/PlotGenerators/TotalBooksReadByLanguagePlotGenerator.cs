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
    public class TotalBooksReadByLanguagePlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupTotalBooksReadByLanguagePlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupTotalBooksReadByLanguagePlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Total Books Read by Language With Time Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Total Books Read by Language With Time Plot");
            SetupTotalBooksReadKeyVsTimeAxes(newPlot);

            // get the languages (in order)
            BooksDelta.DeltaTally latestTally = _mainModel.BookDeltas.Last().OverallTally;
            List<string> languages = (from item in latestTally.LanguageTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // create the series for the languages
            List<KeyValuePair<string, AreaSeries>> languagesSeries =
                new List<KeyValuePair<string, AreaSeries>>();
            List<KeyValuePair<string, LineSeries>> languagesLineSeries =
                new List<KeyValuePair<string, LineSeries>>();

            for (int i = 0; i < languages.Count; i++)
            {
                AreaSeries languageSeries;
                OxyPlotUtilities.CreateLongAreaSeries(out languageSeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.TotalBooksReadKey, languages[i], i, 128);
                languagesSeries.Add(
                    new KeyValuePair<string, AreaSeries>(languages[i], languageSeries));

                LineSeries languageLineSeries;
                OxyPlotUtilities.CreateLongLineSeries(out languageLineSeries,
                    ChartAxisKeys.DateKey, ChartAxisKeys.TotalBooksReadKey, languages[i], i, 128);
                languagesLineSeries.Add(
                    new KeyValuePair<string, LineSeries>(languages[i], languageLineSeries));

                //if (i > 1) break;
            }

            // loop through the deltas adding points for each of the items to the lines
            foreach (var delta in _mainModel.BookDeltas)
            {
                var date = DateTimeAxis.ToDouble(delta.Date);
                foreach (var languageLine in languagesLineSeries)
                {
                    double ttl = 0.0;
                    foreach (var langTotal in delta.OverallTally.LanguageTotals)
                        if (langTotal.Item1 == languageLine.Key)
                            ttl = langTotal.Item2;
                    languageLine.Value.Points.Add(new DataPoint(date, ttl));
                }
            }

            IList<LineSeries> lineSeriesSet =
                (from item in languagesLineSeries select item.Value).ToList();
            var stackSeries = OxyPlotUtilities.StackLineSeries(lineSeriesSet);

            // add them to the plot
            foreach (var languageItems in stackSeries)
                newPlot.Series.Add(languageItems);

            // finally update the model with the new plot
            return newPlot;
        }

        private void SetupTotalBooksReadKeyVsTimeAxes(PlotModel newPlot)
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
                Title = "Total Books Read",
                Key = ChartAxisKeys.TotalBooksReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }
    }
}
