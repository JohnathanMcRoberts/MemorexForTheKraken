using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using log4net;

namespace RotaMaker.Models
{
    [XmlType("Ward")] // define Type
    [XmlInclude(typeof(Nurse)), XmlInclude(typeof(ShiftRequirement)), XmlInclude(typeof(RotaShift))]  
    public class WardModel
    {
        #region Properties

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlArray("Staff")]
        [XmlArrayItem("Nurse")]
        public List<Nurse> Staff { get; set; }

        [XmlArray("MinimumStaffing")]
        [XmlArrayItem("ShiftRequirement")]
        public List<ShiftRequirement> MinimumStaffing { get; set; }

        [XmlArray("RotaShifts")]
        [XmlArrayItem("RotaShift")]
        public List<RotaShift> RotaShifts { get; set; }
        public ILog Log;

        #endregion

        #region Constructor

        public WardModel(ILog log)
        {
            Log = log;
            Staff = new List<Nurse>();
            MinimumStaffing = new List<ShiftRequirement>();
            RotaShifts = new List<RotaShift>();
        }
        #endregion


    }
}
