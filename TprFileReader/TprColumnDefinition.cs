using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TprFileReader
{
    public class TprColumnDefinition
    {
        private List<string> column;

        public TprColumnDefinition(List<string> column, int index)
        {
            // TODO: Complete member initialization
            this.column = column;
            Index = index;
            Name = Index.ToString();
            NumRows = column.Count ;
            AllNumeric = true;
            List<double> colNumerics = new List<double>(NumRows);
            int rowIndex = 0;
            foreach (var textItem in column)
            {
                double numericVal;
                if (Double.TryParse(textItem, out numericVal))
                    colNumerics.Insert(rowIndex, numericVal);
                else
                    AllNumeric = false;

                ++rowIndex;
            }
            ColumnNumerics = colNumerics;
            Increasing = true;
            Varying = false;
            if (AllNumeric && NumRows > 1)
            {
                for (int i = 1; i < NumRows; ++i)
                {
                    if (!Varying && ColumnNumerics[i] != ColumnNumerics[i - 1])
                        Varying = true;

                    if (ColumnNumerics[i] <= ColumnNumerics[i - 1])
                    {
                        Increasing = false;
                        break;
                    }
                }
            }


        }
        public bool AllNumeric { get; set; }

        public string Name { get; set; }

        public int Index { get; set; }

        public List<double> ColumnNumerics { get; set; }

        public int NumRows { get; set; }

        public bool Increasing { get; set; }

        public bool Varying { get; set; }
    }
}
