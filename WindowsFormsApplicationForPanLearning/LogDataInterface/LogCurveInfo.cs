using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogDataInterface
{
    public class LogCurveInfo : ILogCurveInfo
    {
        #region Implementation of ILogCurveInfo


        public string Mnemonic { get; set; }
        public string UnitOfMeasure { get; set; }
        public int ColumnIndex { get; set; }
        public uint NumberOfValuesPerPoint { get; set; }

        #endregion

        public static LogCurveInfo createIndexLogCurveInfo(IndexCurve indexCurve)
        {
            return new LogCurveInfo
            {
                Mnemonic = indexCurve.Mnemonic,
                ColumnIndex = indexCurve.ColumnIndex,
                NumberOfValuesPerPoint = 1
            };
        }
    }
}
