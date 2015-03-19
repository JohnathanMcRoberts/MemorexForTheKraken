using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("MinimumStaffingRequirement")] // define Type // define Type
    [XmlInclude(typeof(ShiftTime)), XmlInclude(typeof(ShiftRequirement))] 
    public class MinimumStaffingRequirement
    {

        [XmlArray("MinimumStaffing")]
        [XmlArrayItem("ShiftRequirement")]
        public List<ShiftRequirement> MinimumStaffing { get; set; }

        public MinimumStaffingRequirement()
        {
            MinimumStaffing = new List<ShiftRequirement>();
        }

        public void InitialiseMinimums()
        {
            for (int day = 0; day < DaysPerWeek; ++day)
                for (ShiftTime.Shift time = ShiftTime.Shift.Early; time <= ShiftTime.Shift.Late; time++)
                    MinimumStaffing.Add(new ShiftRequirement(){Time=time,Day= day,MinimumTrained=1, MimimumTotal=5});
        }


        [XmlIgnoreAttribute]
        public readonly int DaysPerWeek = 7; 
        [XmlIgnoreAttribute]
        public readonly int NumberOfShifts = 21; // 7 * 3
    }
}
