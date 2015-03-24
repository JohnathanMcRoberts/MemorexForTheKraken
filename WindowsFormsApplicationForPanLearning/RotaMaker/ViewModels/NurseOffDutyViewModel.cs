using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using log4net;

using RotaMaker.Models;


namespace RotaMaker.ViewModels
{
    public class NurseOffDutyViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public NurseOffDutyViewModel(Nurse nurse, List<RotaShift> shiftsForSelectedWeek, ILog log)
        {
            _nurse = nurse;
            _shiftsForSelectedWeek = shiftsForSelectedWeek;
            Log = log;
            _nurseOffDuty = new NurseOffDuty(nurse, shiftsForSelectedWeek);
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

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

        public string MondayShifts { get { return _nurseOffDuty.MondayShifts; } }
        public string TuesdayShifts { get { return _nurseOffDuty.TuesdayShifts; } }
        public string WednesdayShifts { get { return _nurseOffDuty.WednesdayShifts; } }
        public string ThursdayShifts { get { return _nurseOffDuty.ThursdayShifts; } }
        public string FridayShifts { get { return _nurseOffDuty.FridayShifts; } }
        public string SaturdayShifts { get { return _nurseOffDuty.SaturdayShifts; } }
        public string SundayShifts { get { return _nurseOffDuty.SundayShifts; } }

        public double TotalWorkedForWeek { get { return _nurseOffDuty.TotalWorkedForWeek; } }
        public double ExpectedHours { get { return _nurseOffDuty.ExpectedHours; } }
        public double BalanceHours { get { return _nurseOffDuty.BalanceHours; } }
        
        #endregion

        #region Member variables

        private List<RotaShift> _shiftsForSelectedWeek;
        private Nurse _nurse;
        private NurseOffDuty _nurseOffDuty;

        #endregion

        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
