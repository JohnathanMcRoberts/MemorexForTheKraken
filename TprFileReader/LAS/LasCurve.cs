using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TprFileReader.LAS
{
    public class LasCurve
    {
        public LasWellInfoItem Descriptor { get; set; }
        public List<LasDataValue> LogData { get; set; }

        public List<double> LogDataDoubles
        {
            get
            {
                List<double> doubles = new List<double>();
                foreach (var logValue in LogData)
                    if (!logValue.IsDateTime)
                        doubles.Add(logValue.DoubleValue);
                return doubles;
            }
        }

        public LasCurve(LasWellInfoItem descriptor)
        {
            Descriptor = descriptor;
            LogData = new List<LasDataValue>();
        }
    }
}
