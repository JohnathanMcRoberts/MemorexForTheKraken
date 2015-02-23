using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TprFileReader;
using TprFileReader.LAS;
using WpfPressurePlotter.Models.GeoData;

namespace WpfPressurePlotter.Models
{
    public class MainPressurePlotterModel
    {
        private log4net.ILog Log;

        public MainPressurePlotterModel(log4net.ILog log)
        {
            Log = log;
            LasFileName = Properties.Settings.Default.LasFile;
        }
        
        #region LAS

        public string LasFileName { get; set; }

        private LasFile _lasFile;

        public LasFile LasFile { get { return _lasFile; } }

        public void OpenSimpleLas()
        {
            _lasFile = new LasFile(LasFileName, Log);
        }

        public int LasFileTimeColumn
        { get { return _lasFileTimeColumn; } set { if (value != 0) return; _lasFileTimeColumn = value; } }


        public readonly string[] PressureUnits = { "psi", "PSI", "PSIA", "PSIG", "kPa" };
        public readonly string[] RateUnits = { "GPM", "cc/s" };

        public int LasFilePressureColumn
        {
            get { return _lasFilePressureColumn; }
            set
            {
                if (value >= _lasFile.DataCurves.Count) return;
                if (_lasFile.DataCurves[value].Descriptor.Unit == null)
                    return;
                if (!PressureUnits.Contains(_lasFile.DataCurves[value].Descriptor.Unit))
                    return;
                _lasFilePressureColumn = value;
            }
        }
        public int LasFileRateColumn
        {
            get { return _lasFileRateColumn; }
            set
            {
                if (value >= _lasFile.DataCurves.Count) return;
                if (_lasFile.DataCurves[value].Descriptor.Unit == null)
                    return;
                if (!RateUnits.Contains(_lasFile.DataCurves[value].Descriptor.Unit))
                    return;
                _lasFileRateColumn = value;
            }
        }

        public List<string> LasColumnNames
        {
            get { return _lasFile.DataCurves.Select(e => e.Descriptor.Mnemonic).ToList(); }
        }


        public List<string> LasPressureColumnNames
        {
            get
            {
                List<string> pressureCols = new List<string>();
                foreach (var curve in _lasFile.DataCurves)
                    if (curve.Descriptor.Unit != null && PressureUnits.Contains(curve.Descriptor.Unit))
                        pressureCols.Add(curve.Descriptor.Mnemonic);
                return pressureCols;
            }
        }
        public List<string> LasRateColumnNames
        {
            get
            {
                List<string> rateCols = new List<string>();
                foreach (var curve in _lasFile.DataCurves)
                    if (curve.Descriptor.Unit != null && RateUnits.Contains(curve.Descriptor.Unit))
                        rateCols.Add(curve.Descriptor.Mnemonic);
                return rateCols;
            }
        }

        private int _lasFileTimeColumn = 0;
        private int _lasFilePressureColumn = 0;
        private int _lasFileRateColumn = 0;
        #endregion

    }
}
