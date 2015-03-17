using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class RotaShift
    {
        public DateTime DateStarted { get; set; }
        public ShiftTime.Shift Time { get; set; }
        public ShiftRequirement Requirement { get; set; }
        public List<Nurse> AssignedStaff { get; set; }

        public RotaShift(DateTime date, ShiftTime.Shift time, ShiftRequirement requirement)
        {

        }

        public bool IsRequirementMet()
        { 
            // To do
            return true; 
        }

    }
}
