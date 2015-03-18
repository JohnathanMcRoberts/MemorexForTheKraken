using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("ShiftTime")] // define Type
    public class ShiftTime
    {
        [Serializable]
        public enum Shift
        {
            [XmlEnum(Name = "Early")]
            Early,
            [XmlEnum(Name = "Late")]
            Late,
            [XmlEnum(Name = "Night")]
            Night
        };
        [Serializable]
        public enum ShiftDay
        {
            [XmlEnum(Name = "Monday")]
            Monday = 0,
            [XmlEnum(Name = "Tuesday")]
            Tuesday = 1,
            [XmlEnum(Name = "Wednesday")]
            Wednesday = 2,
            [XmlEnum(Name = "Thursday")]
            Thursday = 3,
            [XmlEnum(Name = "Friday")]
            Friday = 4,
            [XmlEnum(Name = "Saturday")]
            Saturday = 5,
            [XmlEnum(Name = "Sunday")]
            Sunday = 6
        };

        [XmlElement("Time")]
        public Shift Time { get; set; }

        [XmlElement("Day")]
        public int Day { get; set; }

        public ShiftTime(Shift shift, int day)
        {
            Time = shift;
            if (day >= 7)
                day %= 7;
            Day = day;
        }
        public ShiftTime()
        {
            Time = Shift.Early;
            Day = 1;
        }
    }
}
