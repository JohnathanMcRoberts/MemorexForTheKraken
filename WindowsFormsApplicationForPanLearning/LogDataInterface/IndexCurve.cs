using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogDataInterface
{
    public class IndexCurve
    {
        public IndexCurve() { }

        public IndexCurve(string mnemonic, int columnIndex)
        {
            Mnemonic = mnemonic;
            ColumnIndex = columnIndex;
        }

        // use this to create a date indexCurve element
        public static IndexCurve createDateTimeIndexCurve(int columnIndex)
        {
            return new IndexCurve(FileSystemDataLog.DateTimeIndexMnemonic, columnIndex);
        }

        // use this to create a depth indexCurve element
        public static IndexCurve createMeasuredDepthIndexCurve(int columnIndex)
        {
            return new IndexCurve(FileSystemDataLog.MeasuredDepthIndexMnemonic, columnIndex);
        }

        public string Mnemonic { get; set; }
        public int ColumnIndex { get; set; }
    }
}
