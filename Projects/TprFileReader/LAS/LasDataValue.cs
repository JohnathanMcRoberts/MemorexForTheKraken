using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TprFileReader.LAS
{
    public class LasDataValue
    {
        public bool IsDateTime { get; set; }
        public double DoubleValue { get; set; }
        public DateTime DateTimeValue { get; set; }

        public LasDataValue(double doubleValue)
        {
            DoubleValue = doubleValue;
            IsDateTime = false;
        }
        public LasDataValue(DateTime dateTimeValue)
        {
            DateTimeValue = dateTimeValue;
            IsDateTime = true;
        }
        public LasDataValue()
        {
            IsDateTime = false;
            DoubleValue = 0;
            DateTimeValue = DateTime.MinValue;
        }

        public LasDataValue(string dataElement, LasWellInfoItem descriptor)
        {
            double dValue = 0;
            if (double.TryParse(dataElement, out dValue))
            {
                DoubleValue = dValue;
                IsDateTime = false;
            }
            else if (descriptor.Unit != "")
            {
                DateTime dateTimeValue = DateTime.MinValue;
                if (DateTime.TryParse(dataElement, out dateTimeValue))
                    IsDateTime = true;
            };
        }

    }
}
