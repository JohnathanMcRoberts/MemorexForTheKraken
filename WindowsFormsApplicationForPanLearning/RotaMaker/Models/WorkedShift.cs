using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class WorkedShift
    {
        public DateTime DateStarted { get; set; }
        public ShiftTime.Shift Time { get; set; }

        public double HoursWorked
        {
            get
            {
                if (Time == ShiftTime.Shift.Early) return 7.5;
                if (Time == ShiftTime.Shift.Late) return 7.5;
                return 10.75; 
            }
        }
    }
}
