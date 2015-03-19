using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("BookedHoliday")] // define Type
    public class BookedHoliday
    {
        [XmlElement("StartDate")]
        public DateTime StartDate { get; set; }

        [XmlElement("EndDate")]
        public DateTime EndDate { get; set; }

        [XmlElement("IsHalf")]
        public bool IsHalf { get; set; }

        public double NumberOfDaysLong()
        {

            if (StartDate.DayOfYear == EndDate.DayOfYear && IsHalf)
                return 0.5;
            return EndDate.DayOfYear - StartDate.DayOfYear;
        }

    }
}
