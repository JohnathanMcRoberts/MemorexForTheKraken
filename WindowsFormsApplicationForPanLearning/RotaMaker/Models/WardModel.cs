using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class WardModel
    {
        public List<Nurse> Staff { get; set; }
        public List<ShiftRequirement> MinimumStaffing { get; set; }
        public List<RotaShift> RotaShifts { get; set; }

        public WardModel()
        {
            Staff = new List<Nurse>();
            MinimumStaffing = new List<ShiftRequirement>();
            RotaShifts = new List<RotaShift>();
        }
    }
}
