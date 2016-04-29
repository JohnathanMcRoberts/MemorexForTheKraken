using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using ElectionScotlandSwingWpfApp.Models;

namespace ElectionScotlandSwingWpfApp.ViewModels.Utilities
{
    public class ChartUtilities
    {
        #region Public Static Properties

        public static Dictionary<string, OxyColor> PartyNameToColorLookup
        {
            get
            {
                if (_partyNameToColorLookup == null)
                    SetupPartyNameToColorLookup();
                return _partyNameToColorLookup;
            }
        }
        
        #endregion

        #region Private Static Data
        
        private static Dictionary<string, OxyColor> _partyNameToColorLookup = null;
        
        #endregion

        #region Public Static Functions

        public static void GetNewModelForPieSeries(
            out PlotModel modelP1, out dynamic seriesP1, string title)
        {
            modelP1 = new PlotModel { Title = "Previous Election Overall seats" };

            seriesP1 = new PieSeries
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
        }

        public static void AddPartyResultsToPieChart(
            dynamic seriesP1, List<KeyValuePair<string, int>> partyResults)
        {
            foreach (var partyResult in partyResults)
            {
                string name = partyResult.Key;
                int seats = partyResult.Value;
                OxyColor color = ChartUtilities.PartyNameToColorLookup.ContainsKey(name) ?
                    ChartUtilities.PartyNameToColorLookup[name] : OxyColors.Gray;
                bool isExploded = (seats < 20);

                if (seats > 0)
                    seriesP1.Slices.Add(
                        new PieSlice(name, seats) { IsExploded = isExploded, Fill = color });
            }
        }

        public static PlotModel CreatePieSeriesModelForPartyResults(
            List<KeyValuePair<string, int>> partyResults, string title)
        {
            PlotModel modelP1;
            dynamic seriesP1;
            ChartUtilities.GetNewModelForPieSeries(out modelP1, out seriesP1, title);

            ChartUtilities.AddPartyResultsToPieChart(seriesP1, partyResults);

            modelP1.Series.Add(seriesP1);
            return modelP1;
        }

        #endregion

        #region Private Utilty Functions

        private static void SetupPartyNameToColorLookup()
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
        
        #endregion

    }
}
