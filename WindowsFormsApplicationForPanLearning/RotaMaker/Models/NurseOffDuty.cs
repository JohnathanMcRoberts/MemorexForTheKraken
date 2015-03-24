using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RotaMaker.Models
{
    public class NurseOffDuty
    {
        #region Constructors

        public NurseOffDuty(Nurse nurse, List<RotaShift> shiftsForSelectedWeek)
        {
            // TODO: Complete member initialization
            _nurse = nurse;
            _shiftsForSelectedWeek = shiftsForSelectedWeek;

            InitialiseDisplayParameters();
        }

        private void InitialiseDisplayParameters()
        {
            ExpectedHours = _nurse.StandardHoursPerWeek;
            TotalWorkedForWeek = 0;

            MondayShifts = "";
            TuesdayShifts = "";
            WednesdayShifts = "";
            ThursdayShifts = "";
            FridayShifts = "";
            SaturdayShifts = "";
            SundayShifts = "";

            // loop through the roat shifts and update shifts strings & hours accordingly
            foreach (var shift in _shiftsForSelectedWeek)
            {
                foreach (var nurse in shift.AssignedStaff)
                {
                    if (nurse.Name != _nurse.Name) continue;

                    string shiftName = shift.Time.ToString();
                    TotalWorkedForWeek += ShiftTime.GetHoursForShift(shift.Time);

                    // get the day for this shift and append the shift
                    switch (shift.DateStarted.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            if (MondayShifts != "") MondayShifts += ", ";
                            MondayShifts += shiftName;
                            break;
                        case DayOfWeek.Tuesday:
                            if (TuesdayShifts != "") TuesdayShifts += ", ";
                            TuesdayShifts += shiftName;
                            break;
                        case DayOfWeek.Wednesday:
                            if (WednesdayShifts != "") WednesdayShifts += ", ";
                            WednesdayShifts += shiftName;
                            break;
                        case DayOfWeek.Thursday:
                            if (ThursdayShifts != "") ThursdayShifts += ", ";
                            ThursdayShifts += shiftName;
                            break;
                        case DayOfWeek.Friday:
                            if (FridayShifts != "") FridayShifts += ", ";
                            FridayShifts += shiftName;
                            break;
                        case DayOfWeek.Saturday:
                            if (SaturdayShifts != "") SaturdayShifts += ", ";
                            SaturdayShifts += shiftName;
                            break;
                        case DayOfWeek.Sunday:
                            if (SundayShifts != "") SundayShifts += ", ";
                            SundayShifts += shiftName;
                            break;
                    }
                    // finally get the balance

                }
                BalanceHours = TotalWorkedForWeek - ExpectedHours;
            }
        }

        #endregion

        #region Properties
        
        public string Name
        {
            get { return _nurse.Name; }
        }

        public string AdditionalRules
        {
            get { return _nurse.AdditionalRules; }
        }

        public string Band
        {
            get { return _nurse.Band.ToString(); }
        }

        public string MondayShifts { get; private set; }
        public string TuesdayShifts { get; private set; }
        public string WednesdayShifts { get; private set; }
        public string ThursdayShifts { get; private set; }
        public string FridayShifts { get; private set; }
        public string SaturdayShifts { get; private set; }
        public string SundayShifts { get; private set; }

        public double TotalWorkedForWeek { get; private set; }
        public double ExpectedHours { get; private set; }
        public double BalanceHours { get; private set; }
        
        #endregion

        #region Member variables

        private List<RotaShift> _shiftsForSelectedWeek;
        private Nurse _nurse;

        #endregion
    }
}
