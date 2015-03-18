using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using log4net;
using RotaMaker.Models.Utilities;

namespace RotaMaker.Models
{
    [XmlType("Ward")] // define Type
    [XmlInclude(typeof(Nurse)), XmlInclude(typeof(ShiftRequirement)), XmlInclude(typeof(RotaShift))]  
    public class WardModel
    {
        #region Properties

        [XmlElement("WardName")]
        public string WardName { get; set; }

        [XmlArray("Staff")]
        [XmlArrayItem("Nurse")]
        public List<Nurse> Staff { get; set; }

        [XmlElement("StaffingRequirement")]
        public MinimumStaffingRequirement StaffingRequirement { get; set; }

        [XmlArray("RotaShifts")]
        [XmlArrayItem("RotaShift")]
        public List<RotaShift> RotaShifts { get; set; }


        [XmlIgnoreAttribute]
        public ILog Log;

        [XmlIgnoreAttribute]
        public string BackupFileName { get; set; }

        [XmlIgnoreAttribute]
        public bool IsFileOpened { get; set; }

        #endregion

        #region Constructors

        public WardModel(ILog log)
        {
            Log = log;
            Staff = new List<Nurse>();
            StaffingRequirement = new MinimumStaffingRequirement();
            RotaShifts = new List<RotaShift>();

            BackupFileName = Properties.Settings.Default.BackupFileName;
        }

        public WardModel()
        {
            Log = Logger.Logger.Create(
                Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                Log.Info("Start up");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Staff = new List<Nurse>();
            StaffingRequirement = new MinimumStaffingRequirement();
            RotaShifts = new List<RotaShift>();

            BackupFileName = Properties.Settings.Default.BackupFileName;
        }

        #endregion

        public static WardModel OpenWardFile(string fileName, ILog log)
        {
            if (File.Exists(fileName))
            {
                WardModel model = Serializers.DeserializeFromXML <WardModel>(fileName);
                if (model != null)
                {
                    model.BackupFileName = fileName;
                    model.IsFileOpened = true;
                    model.Log = log;

                    return model;
                }
            }
            return null;
        }

        public static void SaveToFile(WardModel model, string fileName)
        {
            Serializers.SerializeToXml<WardModel>(model, fileName);
        }

        public static int GetShiftIndex(ShiftTime.Shift time, int day)
        {
            return day * (1 + (int)ShiftTime.Shift.Night) + (int)time;

        }
        public static int GetShiftIndex(ShiftTime.Shift time, ShiftTime.ShiftDay day)
        {
            return GetShiftIndex(time, (int)day);

        }
    }
}
