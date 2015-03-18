using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace RotaMaker.Models
{
    public class WardModel
    {
        #region Constructor

        public WardModel(ILog log)
        {
            Log = log;
            Staff = new List<Nurse>();
            MinimumStaffing = new List<ShiftRequirement>();
            RotaShifts = new List<RotaShift>();
        }
        #endregion

        #region Properties
        
        public List<Nurse> Staff { get; set; }
        public List<ShiftRequirement> MinimumStaffing { get; set; }
        public List<RotaShift> RotaShifts { get; set; }
        public ILog Log;

        #endregion


    }
}
