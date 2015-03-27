using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogDataInterface
{
    public interface ILogCurveInfo
    {
        string Mnemonic { get; set; }
        string UnitOfMeasure { get; set; }
        int ColumnIndex { get; set; }
        uint NumberOfValuesPerPoint { get; set; }
    }
}
