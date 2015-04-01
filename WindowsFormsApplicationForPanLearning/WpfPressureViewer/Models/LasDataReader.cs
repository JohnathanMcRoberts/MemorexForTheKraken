using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfPressureViewer.Utilities;
using LogDataInterface;

namespace WpfPressureViewer.Models
{
    public class LasDataReader : IFileDataReader
    {
        public List<string> FileExtensions
        {
            get { return new List<string>(){"LAS"}; }
        }


        public bool GetDataLog(string fileName, out FileSystemDataLog dataLog)
        {
            TprFileReader.LAS.LasFile lasFile = 
                new TprFileReader.LAS.LasFile(fileName, DebugLoggingUtilities.Log);

            dataLog = new FileSystemDataLog();

            if (lasFile.ReadSuccessfully)
            {
                return ConvertTprToDataLoglocalLasFile(lasFile, dataLog);
            }

            return false;
        }

        private bool ConvertTprToDataLoglocalLasFile(TprFileReader.LAS.LasFile lasFile, FileSystemDataLog dataLog)
        {
            return true;
        }
    }
}
