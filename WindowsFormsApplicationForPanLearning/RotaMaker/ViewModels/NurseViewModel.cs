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
    public class NurseViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public NurseViewModel(Nurse nurse, AllStaffViewModel allStaffViewModel, ILog log)
        {
            _nurse = nurse;
            _allStaffViewModel = allStaffViewModel;
            Log = log;

            _shiftAvailabilityVM = new NurseShiftAvailabiltyViewModel(allStaffViewModel, nurse, log);
            // TODO: Complete member initialization
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public string Name 
        { 
            get { return _nurse.Name; } 
        }

        public string FirstName
        {
            get { return _nurse.FirstName; }
            set
            {
                if (value != _nurse.FirstName)
                {
                    _nurse.FirstName = value;
                    OnPropertyChanged(() => Name);
                    OnPropertyChanged(() => FirstName);
                }
            }
        }
        public string LastName
        {
            get { return _nurse.LastName; }
            set
            {
                if (value != _nurse.LastName)
                {
                    _nurse.LastName = value;
                    OnPropertyChanged(() => Name);
                    OnPropertyChanged(() => LastName);
                }
            }
        }

        public int Band
        {
            get { return _nurse.Band; }
            set
            {
                if (value != _nurse.Band)
                {
                    _nurse.Band = value;
                    OnPropertyChanged(() => Band);
                }
            }
        }

        public int HolidaysPerYear
        {
            get { return _nurse.HolidaysPerYear; }
            set
            {
                if (value != _nurse.HolidaysPerYear)
                {
                    _nurse.HolidaysPerYear = value;
                    OnPropertyChanged(() => HolidaysPerYear);
                    OnPropertyChanged(() => HolidaysRemaining);
                }
            }
        }

        public int HolidaysRemaining
        {
            get { return _nurse.HolidaysRemaining; }
        }

        public int TotalShiftsWorked
        {
            get { return _nurse.ShiftsWorked.Count(); }
        }

        public int MaxBand
        {
            get { return _nurse.MaxBand; }
        }

        public int MaxHolidaysPerYear
        {
            get { return _nurse.MaxHolidaysPerYear; }
        }

        public NurseShiftAvailabiltyViewModel ShiftAvailabiltyVM
        {
            get { return _shiftAvailabilityVM; }
        }

        public Nurse Nurse { get { return _nurse; } }

        #endregion

        #region Member variables

        private Nurse _nurse;
        private AllStaffViewModel _allStaffViewModel;

        private NurseShiftAvailabiltyViewModel _shiftAvailabilityVM;
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
