using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace TprFileReader
{
    public class SimplestTprFile : ITprFile
    {
        #region Constructors

        public SimplestTprFile(string fileName, ILog log)
        {
            FileName = fileName;
            Log = log;

            // check that can open the file
            if (!File.Exists(fileName))
            {
                Log.Debug("No such file as : "+ fileName);
                return;
            }

            // set up the data elements 
            HeaderComments = new List<string>();
            List<List<string>> textDataElements = new List<List<string>>();
            GetTextDataElements(fileName, ref textDataElements);
            Log.Debug("Loaded " + textDataElements.Count + " elements for  = " + fileName);

            // setup data as column counts
            int numColumns = -1;
            int numRows = 0;
            bool multipleColumnCountsPerRow = false;
            GetRowColumnCounts(ref numColumns, ref numRows, 
                ref multipleColumnCountsPerRow, textDataElements);
            if (numColumns <= 0)
            {
                Log.Debug("No column data read from : " + fileName);
                return;
            }

            if (multipleColumnCountsPerRow)
            {
                Log.Debug("Multiple Column Counts Per Row  in : " + fileName);
            }

            // set up the the columns
            List<List<string>> textDataColumns = new List<List<string>>();
            CopyTextFromRowsToColumns(textDataElements, numColumns, numRows, 
                ref textDataColumns);
            Log.Debug("textDataColumns cols = " + textDataElements.Count + 
                " elements per col  = " + textDataElements[0].Count);

            // set up the column headers
            ColumnDefinitions = new List<TprColumnDefinition>();

            SetTime = false;
            SetPressure = false;

            for (int i = 0; i < numColumns; ++i)
            {
                var column = textDataColumns[i];

                TprColumnDefinition columnDef = new TprColumnDefinition(column, i);
                ColumnDefinitions.Add(columnDef);

                if (!SetTime && columnDef.AllNumeric && columnDef.Increasing)
                {
                    TimeColumn = i;
                    SetTime = true;
                    continue;
                }
                else if (SetTime && !SetPressure && columnDef.AllNumeric && columnDef.Varying)
                {
                    PressureColumn = i;
                    SetPressure = true;
                    continue;
                }

            }

        }


        #endregion

        #region Utility Functions

        private static void CopyTextFromRowsToColumns(List<List<string>> textDataElements, int numColumns, int numRows,
            ref List<List<string>> textDataColumns)
        {
            for (int i = 0; i < numColumns; ++i)
            {
                textDataColumns.Add(new List<string>(numRows));
            }

            // copy the data into the columns
            int rowCount = 0;
            foreach (var row in textDataElements)
            {
                int colCount = 0;
                foreach (var colText in row)
                {
                    textDataColumns[colCount].Insert(rowCount, colText);
                    ++colCount;
                }
                ++rowCount;
            }
        }

        private void GetRowColumnCounts( ref int numColumns, ref int numRows, 
            ref bool multipleColumnCountsPerRow, List<List<string>> textDataElements)
        {
            foreach (var row in textDataElements)
            {
                if (row.Count != numColumns && numColumns != -1)
                    multipleColumnCountsPerRow = true;
                if (row.Count > numColumns)
                    numColumns = row.Count;
                ++numRows;
            }
        }

        private void GetTextDataElements(string fileName, ref List<List<string>> textDataElements)
        {
            // read the file
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            char[] separators = new char[] { ',', ' ' };
            while ((line = file.ReadLine()) != null)
            {
                List<string> lineElements = new List<string>();
                foreach (var separator in separators)
                {
                    if (line.Contains(separator))
                    {
                        string[] words = line.Split(separator);

                        if (words.Length > 1)
                        {
                            foreach (var word in words)
                                lineElements.Add(word);
                        }
                        break;
                    }
                }
                if (lineElements.Count > 0)
                    textDataElements.Add(lineElements);

            }
            file.Close();
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }
        public string FileName { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public List<string> HeaderComments { get; private set; }
        public List<TprColumnDefinition> ColumnDefinitions { get; private set; }
        public int TimeColumn { get; private set; }
        public int PressureColumn { get; private set; }

        #endregion


        public void SelectPressureColumn(int column)
        {
            if (ColumnDefinitions.Count <= column)
                Log.Debug("Cannot select " + column + 
                    " for Pressure as only " + ColumnDefinitions.Count + " columns.");
            if (ColumnDefinitions[column].AllNumeric && ColumnDefinitions[column].Varying)
            {
                PressureColumn = column;
                SetPressure = true; 
            }
            else
                Log.Debug("Cannot select " + column +
                    " for Pressure as must be numeric and varying.");
        }

        public void SelectTimeColumn(int column)
        {
            if (ColumnDefinitions.Count <= column)
                Log.Debug("Cannot select " + column +
                    " for Time as only " + ColumnDefinitions.Count + " columns.");
            if (ColumnDefinitions[column].AllNumeric && ColumnDefinitions[column].Increasing)
            {
                TimeColumn = column;
                SetTime = true;
            }
            else
                Log.Debug("Cannot select " + column +
                    " for Time as must be numeric and increasing.");
        }

        public void SelectTimeColumnsAndFormat(List<int> column, string timeFormat)
        {
            throw new NotImplementedException();
        }

        public List<double> Times
        {
            get 
            { 
                if (SetTime) return ColumnDefinitions[TimeColumn].ColumnNumerics;
                Log.Debug("No Time column set - return an empty list.");
                return new List<double>();
            }
        }

        public List<double> Pressures
        {
            get
            {
                if (SetPressure) return ColumnDefinitions[PressureColumn].ColumnNumerics;
                Log.Debug("No Pressure column set - return an empty list.");
                return new List<double>();
            }
        }

        public bool SetTime { get; private set; }

        public bool SetPressure { get; private set; }
    }
}
