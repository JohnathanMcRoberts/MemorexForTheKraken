using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("ShiftRequirement")] // define Type // define Type
    [XmlInclude(typeof(ShiftTime))] 
    public class ShiftRequirement
    {
        [XmlElement("Time")]
        public ShiftTime.Shift Time { get; set; }
        [XmlElement("Day")]
        public int Day { get; set; }
        [XmlElement("MinimumTrained")]
        public int MinimumTrained { get; set; }
        [XmlElement("MimimumTotal")]
        public int MimimumTotal { get; set; }
    }
}
