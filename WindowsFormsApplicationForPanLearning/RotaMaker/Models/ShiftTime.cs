﻿using System;
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

        public static ShiftDay ToShiftDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return ShiftDay.Monday;
                case DayOfWeek.Tuesday: return ShiftDay.Tuesday;
                case DayOfWeek.Wednesday: return ShiftDay.Wednesday;
                case DayOfWeek.Thursday: return ShiftDay.Thursday;
                case DayOfWeek.Friday: return ShiftDay.Friday;
                case DayOfWeek.Saturday: return ShiftDay.Saturday;
                case DayOfWeek.Sunday: return ShiftDay.Sunday;
            }
            return ShiftDay.Sunday;
        }
        public static ShiftDay ToShiftDay(int day)
        {
            switch (day)
            {
                case 0: return ShiftDay.Monday;
                case 1: return ShiftDay.Tuesday;
                case 2: return ShiftDay.Wednesday;
                case 3: return ShiftDay.Thursday;
                case 4: return ShiftDay.Friday;
                case 5: return ShiftDay.Saturday;
                case 6: return ShiftDay.Sunday;
            }
            return ShiftDay.Sunday;
        }


        public static double GetHoursForShift(ShiftTime.Shift shift)
        {
            if (shift == Shift.Night) return 10.25;
            return 7.5;
        }
    }
}
