using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("Nurse")] // define Type
    [XmlInclude(typeof(ShiftTime)), XmlInclude(typeof(BookedHoliday)), XmlInclude(typeof(WorkedShift))]  
    public class Nurse
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Band")]
        public int Band { get; set; }
        [XmlElement("HolidaysPerYear")]
        public int HolidaysPerYear { get; set; }

        [XmlArray("AvailableShifts")]
        [XmlArrayItem("ShiftTime")]
        public List<ShiftTime> AvailableShifts { get; set; }

        [XmlArray("ForthcomingHolidays")]
        [XmlArrayItem("BookedHoliday")]
        public List<BookedHoliday> ForthcomingHolidays { get; set; }

        [XmlArray("ShiftsWorked")]
        [XmlArrayItem("WorkedShift")]
        public List<WorkedShift> ShiftsWorked { get; set; } 

        public Nurse()
        {
            Band = 2;
            HolidaysPerYear = 30;

            AvailableShifts = new List<ShiftTime>();
            for(int day =0; day < 7; ++day)
                for(ShiftTime.Shift time = ShiftTime.Shift.Early; time<= ShiftTime.Shift.Late; time++)
                    AvailableShifts.Add(new ShiftTime(time, day));

            ForthcomingHolidays = new List<BookedHoliday>();
            ShiftsWorked = new List<WorkedShift>();
        }

        bool IsTrained { get { return (Band >= 5); } }
        bool AvailableForShift(ShiftTime.Shift shiftTime, DateTime date)
        {
            // To do
            return true;
        }
    }
}
