using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace MongoDbBooks.ViewModels.Utilities
{
    public class OxyPlotUtilities
    {
        #region Constants

        // taken from http://dmcritchie.mvps.org/excel/colors.htm
        static readonly List<Tuple<byte, byte, byte>> _standardColours =
            new List<Tuple<byte, byte, byte>>()
        {
                                    // R    G    B
            new Tuple<byte,byte,byte>(255,	0   ,0),
            new Tuple<byte,byte,byte>(0,	255 ,0),
            new Tuple<byte,byte,byte>(0,	0	,255),
            new Tuple<byte,byte,byte>(255,	255	,0),
            new Tuple<byte,byte,byte>(255,	0	,255),
            new Tuple<byte,byte,byte>(0,	255	,255),
            new Tuple<byte,byte,byte>(128,	0	,0),
            new Tuple<byte,byte,byte>(0,	128	,0),
            new Tuple<byte,byte,byte>(0,	0,	128),
            new Tuple<byte,byte,byte>(128,	128	,0),
            new Tuple<byte,byte,byte>(128,	0	,128),
            new Tuple<byte,byte,byte>(0,	128	,128),
            new Tuple<byte,byte,byte>(192,	192	,192),
            new Tuple<byte,byte,byte>(128,	128	,128),
            new Tuple<byte,byte,byte>(153,	153	,255),
            new Tuple<byte,byte,byte>(153,	51	,102),
            new Tuple<byte,byte,byte>(255,	255	,204),
            new Tuple<byte,byte,byte>(204,	255	,255),
            new Tuple<byte,byte,byte>(102,	0	,102),
            new Tuple<byte,byte,byte>(255,	128	,128),
            new Tuple<byte,byte,byte>(0,	102	,204),
            new Tuple<byte,byte,byte>(204,	204	,255),
            new Tuple<byte,byte,byte>(0,	0	,128),
            new Tuple<byte,byte,byte>(255,	0	,255),
            new Tuple<byte,byte,byte>(255,	255	,0),
            new Tuple<byte,byte,byte>(0,	255	,255),
            new Tuple<byte,byte,byte>(128,	0	,128),
            new Tuple<byte,byte,byte>(128,	0	,0),
            new Tuple<byte,byte,byte>(0,	128	,128),
            new Tuple<byte,byte,byte>(0,	0	,255),
            new Tuple<byte,byte,byte>(0,	204	,255),
            new Tuple<byte,byte,byte>(204,	255	,255),
            new Tuple<byte,byte,byte>(204,	255	,204),
            new Tuple<byte,byte,byte>(255,	255	,153),
            new Tuple<byte,byte,byte>(153,	204	,255),
            new Tuple<byte,byte,byte>(255,	153	,204),
            new Tuple<byte,byte,byte>(204,	153	,255),
            new Tuple<byte,byte,byte>(255,	204	,153),
            new Tuple<byte,byte,byte>(51,	102	,255),
            new Tuple<byte,byte,byte>(51,	204	,204),
            new Tuple<byte,byte,byte>(153,	204	,0),
            new Tuple<byte,byte,byte>(255,	204	,0),
            new Tuple<byte,byte,byte>(255,	153	,0),
            new Tuple<byte,byte,byte>(255,	102	,0),
            new Tuple<byte,byte,byte>(102,	102	,153),
            new Tuple<byte,byte,byte>(150,	150	,150),
            new Tuple<byte,byte,byte>(0,	51	,102),
            new Tuple<byte,byte,byte>(51,	153	,102),
            new Tuple<byte,byte,byte>(0,	51	,0),
            new Tuple<byte,byte,byte>(51,  51	,0),
            new Tuple<byte,byte,byte>(153,	51	,0),
            new Tuple<byte,byte,byte>(153,	51	,102),
            new Tuple<byte,byte,byte>(51,	51	,153),
            new Tuple<byte,byte,byte>(51,	51	,51),
        };

        #endregion

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


        public static void AddLineSeriesToModel(PlotModel newPlot, LineSeries[] lineSeries)
        {
            foreach (var series in lineSeries)
                newPlot.Series.Add(series);
        }

        public static void PopulateVerticalLineSeries(LineSeries series, double xValue, double yMin, double yMax)
        {
            series.Points.Add(new DataPoint(xValue, yMin));
            series.Points.Add(new DataPoint(xValue, yMax));
        }

        public static void CreateVerticalLineSeries(out LineSeries seriesEspMin, out LineSeries seriesEspMax,
            out LineSeries seriesOperatingPoint, out LineSeries seriesBestEfficiencyPoint,
            out LineSeries seriesMaxProductionPoint,
            out LineSeries seriesCatalogueCurveOperatingPoint,
            out LineSeries seriesCatalogueCurveBestEfficiencyPoint,
            string xAxisKey, string yAxisKey = "Head")
        {
            seriesEspMin = new LineSeries
            {
                Title = "ESP Min Flow",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Cyan,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            seriesEspMax = new LineSeries
            {
                Title = "ESP Max Flow",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.YellowGreen,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            seriesOperatingPoint = new LineSeries
            {
                Title = "Operating Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Brown,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.LongDash,
                StrokeThickness = 2
            };
            seriesBestEfficiencyPoint = new LineSeries
            {
                Title = "Best Efficiency Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Blue,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Dash,
                StrokeThickness = 2
            };
            seriesMaxProductionPoint = new LineSeries
            {
                Title = "Max Production Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Black,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            seriesCatalogueCurveOperatingPoint = new LineSeries
            {
                Title = "Catalogue Operating Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.OrangeRed,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.DashDotDot,
                StrokeThickness = 2
            };
            seriesCatalogueCurveBestEfficiencyPoint = new LineSeries
            {
                Title = "Catalogue Best Efficiency Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.RosyBrown,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.DashDotDot,
                StrokeThickness = 2
            };
        }


        public static void CreateVerticalLineSeries(out LineSeries seriesEspMin, out LineSeries seriesEspMax,
            out LineSeries seriesOperatingPoint, out LineSeries seriesBestEfficiencyPoint,
            string xAxisKey, string yAxisKey = "Head")
        {
            seriesEspMin = new LineSeries
            {
                Title = "ESP Min Flow",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Cyan,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            seriesEspMax = new LineSeries
            {
                Title = "ESP Max Flow",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.YellowGreen,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            seriesOperatingPoint = new LineSeries
            {
                Title = "Operating Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Brown,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.LongDash,
                StrokeThickness = 2
            };
            seriesBestEfficiencyPoint = new LineSeries
            {
                Title = "Best Efficiency Point",
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = OxyColors.Orange,
                MarkerType = MarkerType.None,
                LineStyle = LineStyle.Dash,
                StrokeThickness = 2
            };
        }



        public static void CreateLineSeries(out LineSeries series,
            string xAxisKey, string yAxisKey, string title, int colourIndex)
        {
            List<OxyColor> coloursArray = new List<OxyColor>()
            {
                OxyColors.Red,
                OxyColors.Blue,
                OxyColors.Green,
                OxyColors.PaleVioletRed,
                OxyColors.LightBlue,
                OxyColors.LightGreen
            };

            int index = colourIndex % coloursArray.Count;
            var colour = coloursArray[index];

            series = new LineSeries
            {
                Title = title,
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = colour
            };
        }

        public static void LinearRegression(List<double> xVals, List<double> yVals,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = xVals.Count;
            rsquared = yintercept = slope = 0.0;
            if (xVals.Count != yVals.Count || xVals.Count < 1) return;

            for (int ctr = 0; ctr < count; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        public static List<OxyColor> SetupStandardColourSet(byte aValue = 225)
        {
            List<OxyColor> standardColours = new List<OxyColor>();

            foreach (var colour in _standardColours)
                standardColours.Add(OxyColor.FromArgb(aValue, colour.Item1, colour.Item2, colour.Item3));
            return standardColours;
        }

        public static void CreateLongLineSeries(out LineSeries series, string xAxisKey,
            string yAxisKey, string title, int colourIndex, byte aValue = 225)
        {
            List<OxyColor> coloursArray = SetupStandardColourSet(aValue);

            int index = colourIndex % coloursArray.Count;
            var colour = coloursArray[index];

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
            List<OxyColor> coloursArray = SetupStandardColourSet(aValue);

            int index = colourIndex % coloursArray.Count;
            var colour = coloursArray[index];

            series = new AreaSeries
            {
                Title = title,
                XAxisKey = xAxisKey,
                YAxisKey = yAxisKey,
                Color = colour,
                Color2 = OxyColors.Transparent
            };
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

    }
}
