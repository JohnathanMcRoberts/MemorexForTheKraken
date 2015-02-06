using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TprFileReader;
using TprFileReader.LAS;

namespace DataReaderTester.Models
{
    public class MainDataReaderModel
    {
        private log4net.ILog Log;

        public MainDataReaderModel(log4net.ILog log)
        {
            Log = log;
            TprFileName = Properties.Settings.Default.TprFile;
            LasFileName = Properties.Settings.Default.LasFile;
            _iTprFile = null;
        }

        #region TPR

        public string TprFileName { get; set; }

        private ITprFile _iTprFile;

        public ITprFile TprFile { get { return _iTprFile; } }

        public void OpenSimpleTpr()
        {
            _iTprFile = new SimplestTprFile(TprFileName, Log);
        }

        #endregion

        #region LAS

        public string LasFileName { get; set; }

        private LasFile _lasFile;

        public LasFile LasFile { get { return _lasFile; } }

        public void OpenSimpleLas()
        {
            _lasFile = new LasFile(LasFileName, Log);
        }

        #endregion
    }
}
