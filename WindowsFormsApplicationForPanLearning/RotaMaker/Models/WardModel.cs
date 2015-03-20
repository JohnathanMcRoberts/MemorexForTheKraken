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

        #region Public Static Methods

        public static WardModel OpenWardFile(string fileName, ILog log)
        {
            WardModel wardModel = null; 
            if (File.Exists(fileName))
            {
                wardModel = Serializers.DeserializeFromXML<WardModel>(fileName);
                if (wardModel != null)
                {
                    wardModel.BackupFileName = fileName;
                    wardModel.IsFileOpened = true;
                    wardModel.Log = log;
                }
            }


            if (wardModel == null)
            {
                wardModel = new WardModel(log)
                {
                    IsFileOpened = false, 
                    BackupFileName = fileName, 
                    WardName = "Dummy Ward"
                };
                wardModel.StaffingRequirement.InitialiseMinimums();
            }

            if (wardModel.Staff.Count == 0)
                wardModel.Staff.Add(Nurse.CreateDummyNurse());

            return wardModel;
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

        public static DateTime GetMondayBeforeDate(DateTime dateSelected)
        {
            DateTime mondayDate = dateSelected;

            while (mondayDate.Day > 0)
            {
                if (mondayDate.DayOfWeek == DayOfWeek.Monday)
                    break;
                mondayDate = new DateTime(mondayDate.Year, mondayDate.Month, mondayDate.Day - 1);
            }
            return mondayDate;
        }

        #endregion

        #region Public Methods

        public List<RotaShift> GetWeeksRotaForDate(DateTime dateSelected)
        {
            List<RotaShift> returnedShifts = new List<RotaShift>();

            DateTime mondayOfWeek = GetMondayBeforeDate(dateSelected);
            DateTime mondayOfNextWeek = GetMondayBeforeDate(dateSelected.AddDays(7));

            // add all the existing items for this week 
            GetExistingRotaShiftsForWeek(returnedShifts, mondayOfWeek, mondayOfNextWeek);

            // add missing shifts for the week
            AddMissingShiftsForWeek(returnedShifts, mondayOfWeek);

            // then sort them
            List<string> sortedList = new List<string>();
            returnedShifts = returnedShifts.OrderBy(i => i.DateStarted.Year).
                                ThenBy(i => i.DateStarted.DayOfYear).
                                ThenBy(i => i.Time).ToList();

            return returnedShifts;
        }

        public List<Nurse> GetAvailableNursesForShift(RotaShift shift, bool trained)
        {
            List<Nurse> availableNurses = new List<Nurse>();
            foreach(var nurse in Staff)
            {
                if (nurse.IsTrained != trained) continue;
                if (nurse.AvailableForShift(shift))
                    availableNurses.Add(nurse);
            }

            return availableNurses;
        }
        #endregion

        #region Private Utility Methods

        private void AddMissingShiftsForWeek(List<RotaShift> returnedShifts, DateTime mondayOfWeek)
        {
            for (var day = 0; day < 7; ++day)
            {
                DateTime shiftDay = mondayOfWeek.AddDays(day);

                for (ShiftTime.Shift time = ShiftTime.Shift.Early; time <= ShiftTime.Shift.Night; ++time)
                {
                    bool shiftIsThere = false;
                    foreach (var shift in returnedShifts)
                        if (shift.DateStarted.Year == shiftDay.Year &&
                            shift.DateStarted.DayOfYear == shiftDay.DayOfYear &&
                            shift.Time == time)
                            shiftIsThere = true;

                    if (!shiftIsThere)
                    {
                        // make up the blank shift
                        var requirement =
                            StaffingRequirement.MinimumStaffing[GetShiftIndex(time, day)];
                        RotaShift blankShift = new RotaShift(shiftDay, time, requirement);

                        // add it to the returned list and main list
                        returnedShifts.Add(blankShift);
                        RotaShifts.Add(blankShift);
                    }
                }
            }
        }

        private void GetExistingRotaShiftsForWeek(List<RotaShift> returnedShifts, DateTime mondayOfWeek, DateTime mondayOfNextWeek)
        {
            foreach (var rotaShift in RotaShifts)
            {
                // ignore if not in the right year
                if (mondayOfWeek.Year != rotaShift.DateStarted.Year &&
                    mondayOfNextWeek.Year != rotaShift.DateStarted.Year)
                    continue;

                // ignore if before the starting monday 
                if (mondayOfWeek.Year == rotaShift.DateStarted.Year &&
                   rotaShift.DateStarted.DayOfYear < mondayOfWeek.DayOfYear)
                    continue;

                // ignore if equal or after the next weeks monday 
                if (mondayOfNextWeek.Year == rotaShift.DateStarted.Year &&
                    rotaShift.DateStarted.DayOfYear >= mondayOfNextWeek.DayOfYear)
                    continue;

                returnedShifts.Add(rotaShift);
            }
        }

        #endregion

    }
}
