using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RotaMaker.Models
{
    [XmlType("BookedHoliday")] // define Type
    public class BookedHoliday
    {
        [XmlElement("StartDate")]
        DateTime StartDate { get; set; }

        [XmlElement("EndDate")]
        DateTime EndDate { get; set; }
    }
}
