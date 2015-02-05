using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TprFileReader;

namespace DataReaderTester.Models
{
    public class MainDataReaderModel
    {
        private log4net.ILog Log;

        public MainDataReaderModel(log4net.ILog log)
        {
            Log = log;
            TprFileName = Properties.Settings.Default.TprFile;
            _iTprFile = null;
        }

        public string TprFileName { get; set; }

        public void OpenSimpleTpr()
        {
            _iTprFile = new SimplestTprFile(TprFileName, Log);
        }

        private ITprFile _iTprFile;

        public ITprFile TprFile { get {return _iTprFile;} }
    }
}
