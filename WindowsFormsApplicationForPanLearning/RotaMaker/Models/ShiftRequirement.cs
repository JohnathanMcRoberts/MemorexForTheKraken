using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class ShiftRequirement
    {
        public ShiftTime.Shift Time { get; set; }
        public int Day { get; set; }
        public int MinimumTrained { get; set; }
        public int MimimumTotal { get; set; }
    }
}
