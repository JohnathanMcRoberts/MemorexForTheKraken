using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaMaker.Models
{
    public class ShiftTime
    {
        public enum Shift { Early, Late, Night };

        public Shift Time { get; set; }

        public int Day { get; set; }

        public ShiftTime(Shift shift, int day)
        {
            Time = shift;
            if (day >= 7)
                day %= 7;
            Day = day;
        }
    }
}
