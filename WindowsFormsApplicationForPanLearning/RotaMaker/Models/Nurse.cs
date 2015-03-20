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
        #region Properties

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlIgnore]
        public string Name { get { return FirstName + " " + LastName; } }
        
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

        [XmlIgnore]
        public bool IsTrained { get { return (Band >= 5); } }

        #endregion

        #region Constructor

        public Nurse()
        {
            Band = 2;
            HolidaysPerYear = 30;

            AvailableShifts = new List<ShiftTime>();
            ForthcomingHolidays = new List<BookedHoliday>();
            ShiftsWorked = new List<WorkedShift>();
        }

        #endregion

        #region Constants

        [XmlIgnore]
        public readonly int MaxBand = 8;

        [XmlIgnore]
        public readonly int MaxHolidaysPerYear = 45;

        #endregion

        #region Public Functions

        public void InitialiseAllShifts()
        {
            AvailableShifts = new List<ShiftTime>();
            for (int day = 0; day < 7; ++day)
                for (ShiftTime.Shift time = ShiftTime.Shift.Early; time <= ShiftTime.Shift.Late; time++)
                    AvailableShifts.Add(new ShiftTime(time, day));
        }

        public bool AvailableForShift(ShiftTime.Shift shiftTime, DateTime date)
        {
            // To do
            return GetAvailability(shiftTime, ShiftTime.ToShiftDay(date.DayOfWeek));
        }

        public int HolidaysRemaining 
        {
            get { return HolidaysPerYear - HolidaysTaken(); }
        }

        public int HolidaysTaken()
        {
            double holidaysTaken = 0;
            foreach (BookedHoliday holiday in ForthcomingHolidays)
                if (holiday.StartDate < DateTime.Now)
                    holidaysTaken += holiday.NumberOfDaysLong();

            return (int)Math.Round(holidaysTaken);
        }

        public bool GetAvailability(ShiftTime.Shift shiftTime, ShiftTime.ShiftDay shiftDay)
        {
            foreach(var shift in AvailableShifts)
                if (shift.Day == (int)shiftDay &&  shift.Time == shiftTime)
                    return true;
            return false;
        }

        public void SetAvailability(ShiftTime.Shift shiftTime, ShiftTime.ShiftDay shiftDay, bool available)
        {
            if (available == true && GetAvailability(shiftTime, shiftDay) == false)
            {
                AvailableShifts.Add(new ShiftTime(shiftTime, (int)shiftDay));
            }
            else if (available == false && GetAvailability(shiftTime, shiftDay) == true)
            {
                for (int i = 0; i < AvailableShifts.Count; ++i)
                {
                    if (AvailableShifts[i].Day == (int)shiftDay && AvailableShifts[i].Time == shiftTime)
                    {
                        AvailableShifts.RemoveAt(i);
                        break;
                    }
                }
            }
        }


        public bool AvailableForShift(RotaShift shift)
        {
            // TODO : handle holidays???
            
            return AvailableForShift(shift.Time, shift.DateStarted);
        }

        #endregion

        #region Static Constructor

        public static Nurse CreateDummyNurse()
        {
            Nurse newNurse = new Nurse() { FirstName = "Dummy", LastName = "Nurse" };
            newNurse.InitialiseAllShifts();
            return newNurse;
        }

        #endregion

    }
}
