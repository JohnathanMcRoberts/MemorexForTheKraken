using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

using System.Windows.Input;
using System.Windows.Forms;

using log4net;

using RotaMaker.Models;
using RotaMaker.ViewModels.Utilities;


namespace RotaMaker.ViewModels
{
    public class NurseShiftAvailabiltyViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public NurseShiftAvailabiltyViewModel(AllStaffViewModel parentVM, Nurse nurse, ILog log)
        {
            _parentVM = parentVM;
            Log = log;
            _nurse = nurse;
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        #region Early Shift Requirement VMs

        public Boolean AvailableMondayEarly 
        { 
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Monday);}
            set 
            { 
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Monday, value);
                OnPropertyChanged(() => AvailableMondayEarly);
            }
        }
        public Boolean AvailableTuesdayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Tuesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Tuesday, value);
                OnPropertyChanged(() => AvailableTuesdayEarly);
            }
        }
        public Boolean AvailableWednesdayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Wednesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Wednesday, value);
                OnPropertyChanged(() => AvailableWednesdayEarly);
            }
        }
        public Boolean AvailableThursdayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Thursday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Thursday, value);
                OnPropertyChanged(() => AvailableThursdayEarly);
            }
        }
        public Boolean AvailableFridayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Friday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Friday, value);
                OnPropertyChanged(() => AvailableFridayEarly);
            }
        }
        public Boolean AvailableSaturdayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Saturday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Saturday, value);
                OnPropertyChanged(() => AvailableSaturdayEarly);
            }
        }
        public Boolean AvailableSundayEarly
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Sunday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Sunday, value);
                OnPropertyChanged(() => AvailableSundayEarly);
            }
        }

        #endregion

        #region Late Shift Requirement VMs

        public Boolean AvailableMondayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Monday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Monday, value);
                OnPropertyChanged(() => AvailableMondayLate);
            }
        }
        public Boolean AvailableTuesdayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Tuesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Tuesday, value);
                OnPropertyChanged(() => AvailableTuesdayLate);
            }
        }
        public Boolean AvailableWednesdayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Wednesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Wednesday, value);
                OnPropertyChanged(() => AvailableWednesdayLate);
            }
        }
        public Boolean AvailableThursdayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Thursday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Thursday, value);
                OnPropertyChanged(() => AvailableThursdayLate);
            }
        }
        public Boolean AvailableFridayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Friday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Friday, value);
                OnPropertyChanged(() => AvailableFridayLate);
            }
        }
        public Boolean AvailableSaturdayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Saturday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Saturday, value);
                OnPropertyChanged(() => AvailableSaturdayLate);
            }
        }
        public Boolean AvailableSundayLate
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Sunday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Sunday, value);
                OnPropertyChanged(() => AvailableSundayLate);
            }
        }

        #endregion

        #region Night Shift Requirement VMs

        public Boolean AvailableMondayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Monday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Monday, value);
                OnPropertyChanged(() => AvailableMondayNight);
            }
        }
        public Boolean AvailableTuesdayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Tuesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Tuesday, value);
                OnPropertyChanged(() => AvailableTuesdayNight);
            }
        }
        public Boolean AvailableWednesdayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Wednesday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Wednesday, value);
                OnPropertyChanged(() => AvailableWednesdayNight);
            }
        }
        public Boolean AvailableThursdayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Thursday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Thursday, value);
                OnPropertyChanged(() => AvailableThursdayNight);
            }
        }
        public Boolean AvailableFridayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Friday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Friday, value);
                OnPropertyChanged(() => AvailableFridayNight);
            }
        }
        public Boolean AvailableSaturdayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Saturday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Saturday, value);
                OnPropertyChanged(() => AvailableSaturdayNight);
            }
        }
        public Boolean AvailableSundayNight
        {
            get { return _nurse.GetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Sunday); }
            set
            {
                _nurse.SetAvailability(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Sunday, value);
                OnPropertyChanged(() => AvailableSundayNight);
            }
        }

        #endregion

        #endregion

        #region Member variables

        private AllStaffViewModel _parentVM;
        private Nurse _nurse;

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
