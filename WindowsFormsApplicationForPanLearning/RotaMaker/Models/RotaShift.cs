using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("RotaShift")] // define Type // define Type
    [XmlInclude(typeof(ShiftTime)), XmlInclude(typeof(ShiftRequirement)), XmlInclude(typeof(Nurse))] 
    public class RotaShift
    {
        [XmlElement("DateStarted")]
        public DateTime DateStarted { get; set; }
        [XmlElement("Time")]
        public ShiftTime.Shift Time { get; set; }
        [XmlElement("Requirement")]
        public ShiftRequirement Requirement { get; set; }

        [XmlArray("AssignedStaff")]
        [XmlArrayItem("Nurse")]
        public List<Nurse> AssignedStaff { get; set; }

        public RotaShift(DateTime date, ShiftTime.Shift time, ShiftRequirement requirement)
        {

        }

        public RotaShift()
        {
            DateStarted = DateTime.Now;
            Time = ShiftTime.Shift.Early;
            Requirement = new ShiftRequirement();
            AssignedStaff = new List<Nurse>();
        }

        public bool IsRequirementMet()
        { 
            // To do
            return true; 
        }

    }
}
