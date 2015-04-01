using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace TprFileReader.LAS
{
    public class LasFile
    {
        public ILog Log { get; set; }
        public string FileName { get; set; }
        public double LASVersion { get; set; }
        public bool Wrap { get; set; }
        public List<LasWellInfoItem> WellInfoItems { get; set; }
        public List<LasWellInfoItem> WellParameters { get; set; }
        public List<LasCurve> DataCurves { get; set; }
        public bool ReadSuccessfully { get; private set; }

        public LasFile(string fileName, ILog log)
        {
            ReadSuccessfully = false;
            FileName = fileName;
            Log = log;

            // check that can open the file
            if (!File.Exists(fileName))
            {
                Log.Debug("No such file as : " + fileName);
                return;
            }

            WellInfoItems = new List<LasWellInfoItem>();
            WellParameters = new List<LasWellInfoItem>();
            DataCurves = new List<LasCurve>();

            // open the file 
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                ParseLasStream(file);
                ReadSuccessfully = true;
            }
            catch (Exception e)
            {
                Log.Debug("File Read exception : " + e.ToString());
            }
        }

        public enum LASSection
        {
            NotSet = -1,
            DoNothing, 
            VersionX, 
            Well, 
            Curves, 
            Parameters, 
            Comments, 
            LogData 
        };

        private void ParseLasStream(StreamReader reader)
        {
            LASSection currentSection = LASSection.DoNothing;
            List<string> logDataElements = new List<string>();
            string line = "";
            do
            {
                line = reader.ReadLine();
                if (IsLineComment(line)) continue;

                if (line.IndexOf("~") >= 0)
                {
                    currentSection = GetSection(line);
                    Log.Debug("Read a Section header - Now =" + currentSection.ToString() 
                        + " : from " + line);
                }
                else
                {
                    switch (currentSection)
                    {
                        case LASSection.NotSet:
                        case LASSection.DoNothing:
                            break;
                        case LASSection.VersionX:
                            ProcessVersionSectionLine(line);
                            break;
                        case LASSection.Well:
                            ProcessWellSectionLine(line);
                            break;
                        case LASSection.Curves:
                            ProcessCurveInformationSectionLine(line);
                            break;
                        case LASSection.Parameters:
                            ProcessParameterSectionLine(line);
                            break;
                        case LASSection.Comments:
                            ProcessCommentsSectionLine(line);
                            break;
                        case LASSection.LogData:
                            ProcessLogDataSectionLine(line, ref logDataElements);
                            break;
                    }
                }
            }
            while (reader.Peek() != -1);
        }

        private string GetStringValue(string str, int wordIndex, int wordLen)
        {
            int colonIndex = str.IndexOf(":");
            return str.Substring(wordIndex + wordLen,
                colonIndex - (wordIndex + wordLen + 1)).Trim();
        }

        #region Version Section

        private void ProcessVersionSectionLine(string str)
        {
            int index = str.IndexOf("VERS.");
            if (index >= 0)
            {
                LASVersion = GetLASVersion(str, index, 5);
            }
            index = str.IndexOf("WRAP.");
            if (index >= 0)
            {
                Wrap = GetWrap(str, index, 5);
            }
        }

        private double GetLASVersion(string str, int wordIndex, int wordLen)
        {
            double dVersion = 0;
            str = GetStringValue(str, wordIndex, wordLen);
            double.TryParse(str, out dVersion);
            return dVersion;
        }

        private bool GetWrap(string str, int wordIndex, int wordLen)
        {
            str = GetStringValue(str, wordIndex, wordLen);
	        return (str.ToUpper() == "YES");
        }

        #endregion

        private void ProcessWellSectionLine(string str)
        {
            int separatorIndex = str.IndexOf(".");

            if (separatorIndex < 1) return;

            LasWellInfoItem infoItem = new LasWellInfoItem(str);

            WellInfoItems.Add(infoItem);
        }

        private void ProcessCurveInformationSectionLine(string str)
        {
            int separatorIndex = str.IndexOf(".");

            if (separatorIndex < 1) return;

            LasWellInfoItem infoItem = new LasWellInfoItem(str);
            
            DataCurves.Add(new LasCurve(infoItem));
        }

        private void ProcessParameterSectionLine(string str)
        {
            int separatorIndex = str.IndexOf(".");

            if (separatorIndex < 1) return;

            LasWellInfoItem parameterItem = new LasWellInfoItem(str);

            WellParameters.Add(parameterItem);
        }

        private void ProcessCommentsSectionLine(string line)
        {
            // ignore for the moment
        }

        private void ProcessLogDataSectionLine(string line, ref List<string> logDataElements)
        {
            if (line.Contains(" "))
            {
                var elements = line.Split(' ');
                foreach (var dataElement in elements)
                    if (dataElement.Length > 0)
                        logDataElements.Add(dataElement.Trim());
            }
            else
                logDataElements.Add(line.Trim());

            if (logDataElements.Count == DataCurves.Count)
            {
                for(int i = 0; i < DataCurves.Count; ++i)
                {
                    string dataElement = logDataElements[i];
                    DataCurves[i].LogData.Add(
                        new LasDataValue(dataElement, DataCurves[i].Descriptor));
                }
                logDataElements.Clear();
            }
        }

        private LASSection GetSection(string line)
        {	
            string sSection = line.Substring(1,1);
	        char ch;
	        LASSection iSection = LASSection.NotSet;

            ch = sSection[0];
	        switch (ch)
	        {
	          case 'V': // version & wrap info
                    iSection = LASSection.VersionX;
		        break;
	          case 'W': // well identification
                iSection = LASSection.Well;
		        break;
	          case 'C':// curve information
                iSection = LASSection.Curves;
		        break;
	          case 'P':// parameters or constants
                iSection = LASSection.Parameters;
		        break;
	          case 'O': // other information such as comments
                iSection = LASSection.Comments;
		        break;
	          case 'A':// ASCII log data
                iSection = LASSection.LogData;
		        break;
	        }

	        if (iSection < 0)
	        {
                if (line.ToUpper().IndexOf("PARAMETER") > 0)
		        {
                    iSection = LASSection.NotSet;
		        }
                if (line.ToUpper().IndexOf("DEFINITION") > 0)
		        {
                    iSection = LASSection.Curves;
		        }
                if (line.ToUpper().IndexOf("DATA") > 0)
		        {
                    iSection = LASSection.LogData;
		        }
	        }
	        return iSection;
        }

        private bool IsLineComment(string line)
        {
            try
            {
                return (line.IndexOf("#") == 0);
            }
            catch (Exception e)
            {
                Log.Debug("Exception getting index of  : " + e.ToString());
            }
            return true;
        }
    }
}
