using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("WorkedShift")] // define Type
    [XmlInclude(typeof(ShiftTime))]  
    public class WorkedShift
    {
        [XmlElement("DateStarted")]
        public DateTime DateStarted { get; set; }
        [XmlElement("Time")]
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
