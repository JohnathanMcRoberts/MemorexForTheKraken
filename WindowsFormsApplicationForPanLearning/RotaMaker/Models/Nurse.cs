using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class Nurse
    {
        public string Name { get; set; }
        public int Band { get; set; }
        public int HolidaysPerYear { get; set; }

        public List<ShiftTime> AvailableShifts { get; set; }
        public List<BookedHoliday> ForthcomingHolidays { get; set; }
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
