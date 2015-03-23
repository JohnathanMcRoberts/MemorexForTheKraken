using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using log4net;

using RotaMaker.Models;
using RotaMaker.ViewModels.Utilities;

namespace RotaMaker.ViewModels
{
    public class WardStaffingViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public WardStaffingViewModel(RotaMakerViewModel mainWindow, WardModel mainModel, ILog log)
        {
            _mainWindowVM = mainWindow;
            Log = log;
            _mainModel = mainModel;
            _selectedWeekDate = WardModel.GetMondayBeforeDate(DateTime.Now);
            InitialiseRotaVms();
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public DateTime SelectedWeekDate 
        { 
            get {return _selectedWeekDate;}
            set
            {
                if (_selectedWeekDate.Year != value.Year || _selectedWeekDate.DayOfYear != value.DayOfYear)
                {
                    _selectedWeekDate = value;
                    InitialiseRotaVms();
                }
            }
        }

        public Boolean AllShiftsAreFilled
        {
            get
            {
                foreach (var shift in _shiftVMs)
                    if (!shift.RotaShift.IsRequirementMet())
                        return false;
                return true;
            }
        }

        #region Early Shift Requirement VMs

        public RotaShiftViewModel MondayEarlyVM { get { return _mondayEarlyVM; } }
        public RotaShiftViewModel TuesdayEarlyVM { get { return _tuesdayEarlyVM; } }
        public RotaShiftViewModel WednesdayEarlyVM { get { return _wednesdayEarlyVM; } }
        public RotaShiftViewModel ThursdayEarlyVM { get { return _thursdayEarlyVM; } }
        public RotaShiftViewModel FridayEarlyVM { get { return _fridayEarlyVM; } }
        public RotaShiftViewModel SaturdayEarlyVM { get { return _saturdayEarlyVM; } }
        public RotaShiftViewModel SundayEarlyVM { get { return _sundayEarlyVM; } }

        #endregion

        #region Late Shift Requirement VMs

        public RotaShiftViewModel MondayLateVM { get { return _mondayLateVM; } }
        public RotaShiftViewModel TuesdayLateVM { get { return _tuesdayLateVM; } }
        public RotaShiftViewModel WednesdayLateVM { get { return _wednesdayLateVM; } }
        public RotaShiftViewModel ThursdayLateVM { get { return _thursdayLateVM; } }
        public RotaShiftViewModel FridayLateVM { get { return _fridayLateVM; } }
        public RotaShiftViewModel SaturdayLateVM { get { return _saturdayLateVM; } }
        public RotaShiftViewModel SundayLateVM { get { return _sundayLateVM; } }

        #endregion

        #region Night Shift Requirement VMs

        public RotaShiftViewModel MondayNightVM { get { return _mondayNightVM; } }
        public RotaShiftViewModel TuesdayNightVM { get { return _tuesdayNightVM; } }
        public RotaShiftViewModel WednesdayNightVM { get { return _wednesdayNightVM; } }
        public RotaShiftViewModel ThursdayNightVM { get { return _thursdayNightVM; } }
        public RotaShiftViewModel FridayNightVM { get { return _fridayNightVM; } }
        public RotaShiftViewModel SaturdayNightVM { get { return _saturdayNightVM; } }
        public RotaShiftViewModel SundayNightVM { get { return _sundayNightVM; } }

        #endregion

        public ObservableCollection<NurseOffDutyViewModel> NurseOffDutyVMs { get { return GetOffDutyViewModels(); } }

        #endregion

        #region Member variables

        private RotaMakerViewModel _mainWindowVM;
        private WardModel _mainModel;
        private DateTime _selectedWeekDate ;
        private List<RotaShift> _shiftsForSelectedWeek;

        
        private List<RotaShiftViewModel> _shiftVMs;

        #region Early Shift Requirements VMs

        private RotaShiftViewModel _mondayEarlyVM;
        private RotaShiftViewModel _tuesdayEarlyVM;
        private RotaShiftViewModel _wednesdayEarlyVM;
        private RotaShiftViewModel _thursdayEarlyVM;
        private RotaShiftViewModel _fridayEarlyVM;
        private RotaShiftViewModel _saturdayEarlyVM;
        private RotaShiftViewModel _sundayEarlyVM;

        #endregion

        #region Late Shift Requirements VMs

        private RotaShiftViewModel _mondayLateVM;
        private RotaShiftViewModel _tuesdayLateVM;
        private RotaShiftViewModel _wednesdayLateVM;
        private RotaShiftViewModel _thursdayLateVM;
        private RotaShiftViewModel _fridayLateVM;
        private RotaShiftViewModel _saturdayLateVM;
        private RotaShiftViewModel _sundayLateVM;

        #endregion

        #region Night Shift Requirements VMs

        private RotaShiftViewModel _mondayNightVM;
        private RotaShiftViewModel _tuesdayNightVM;
        private RotaShiftViewModel _wednesdayNightVM;
        private RotaShiftViewModel _thursdayNightVM;
        private RotaShiftViewModel _fridayNightVM;
        private RotaShiftViewModel _saturdayNightVM;
        private RotaShiftViewModel _sundayNightVM;

        #endregion

        private ICommand _completeAndPrintCommand;

        #endregion
        
        #region Utility functions
        
        private  ObservableCollection<NurseOffDutyViewModel> GetOffDutyViewModels()
        {
            ObservableCollection<NurseOffDutyViewModel> nurseOffDutyVMs = 
                new ObservableCollection<NurseOffDutyViewModel>();

            foreach (var nurse in _mainModel.Staff)
            {
                NurseOffDutyViewModel nurseOffDuty = new NurseOffDutyViewModel(nurse, _shiftsForSelectedWeek, Log);
                nurseOffDutyVMs.Add(nurseOffDuty);
            }
            return nurseOffDutyVMs;
        } 

        void InitialiseRotaVms()
        {
            // get this weeks shifts
            _shiftsForSelectedWeek = _mainModel.GetWeeksRotaForDate(_selectedWeekDate);

            // reset the VMs
            _shiftVMs = new List<RotaShiftViewModel>();
            InitialiseEarlyShiftViewModels();
            InitialiseLateShiftViewModels();
            InitialiseNightShiftViewModels();

            // refresh everything
            OnPropertyChanged("");
        }       

        private void InitialiseEarlyShiftViewModels()
        {
            _mondayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayEarlyVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Sunday)],
                Log);
            _shiftVMs.Add(_mondayEarlyVM);
            _shiftVMs.Add(_tuesdayEarlyVM);
            _shiftVMs.Add(_wednesdayEarlyVM);
            _shiftVMs.Add(_thursdayEarlyVM);
            _shiftVMs.Add(_fridayEarlyVM);
            _shiftVMs.Add(_saturdayEarlyVM);
            _shiftVMs.Add(_sundayEarlyVM);
        }
        private void InitialiseLateShiftViewModels()
        {
            _mondayLateVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayLateVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayLateVM = new RotaShiftViewModel(
                this, _mainModel,
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayLateVM = new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayLateVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayLateVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayLateVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Sunday)],
                Log);
            
            _shiftVMs.Add(_mondayLateVM);
            _shiftVMs.Add(_tuesdayLateVM);
            _shiftVMs.Add(_wednesdayLateVM);
            _shiftVMs.Add(_thursdayLateVM);
            _shiftVMs.Add(_fridayLateVM);
            _shiftVMs.Add(_saturdayLateVM);
            _shiftVMs.Add(_sundayLateVM);
        }
        private void InitialiseNightShiftViewModels()
        {
            _mondayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayNightVM =  new RotaShiftViewModel(
                this, _mainModel, 
                _shiftsForSelectedWeek[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Sunday)],
                Log);
            _shiftVMs.Add(_mondayNightVM);
            _shiftVMs.Add(_tuesdayNightVM);
            _shiftVMs.Add(_wednesdayNightVM);
            _shiftVMs.Add(_thursdayNightVM);
            _shiftVMs.Add(_fridayNightVM);
            _shiftVMs.Add(_saturdayNightVM);
            _shiftVMs.Add(_sundayNightVM);
        }

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

        #region Public functions 

        public void UpdateNurseOffDuty()
        {
            OnPropertyChanged(() => NurseOffDutyVMs);
            OnPropertyChanged(() => AllShiftsAreFilled);            
        }

        #endregion

        #region Commands

        public ICommand CompleteAndPrintCommand
        {
            get
            {
                return _completeAndPrintCommand ??
                    (_completeAndPrintCommand = new CommandHandler(() => CompleteAndPrintCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void CompleteAndPrintCommandAction()
        {
            
            // do stuff
        }

        #endregion
    }
}
