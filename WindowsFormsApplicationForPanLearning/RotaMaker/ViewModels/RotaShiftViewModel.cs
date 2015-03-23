using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Input;

using log4net;

using RotaMaker.Models;
using RotaMaker.ViewModels.Utilities;

namespace RotaMaker.ViewModels
{
    public class RotaShiftViewModel : INotifyPropertyChanged
    {
        public RotaShiftViewModel(WardStaffingViewModel parentVM, WardModel mainModel, RotaShift shift, ILog log)
        {
            _parentVM = parentVM;
            Log = log;
            _mainModel = mainModel;
            _rotaShift = shift;
            
            _trainedNursesForShift = new ObservableCollection<Nurse>();
            _availableTrainedNurses = new ObservableCollection<Nurse>();
            _nursesForShift = new ObservableCollection<Nurse>();
            _availableNurses = new ObservableCollection<Nurse>();

            foreach (var nurse in shift.AssignedStaff)
                if (nurse.IsTrained)
                    _trainedNursesForShift.Add(nurse);
                else
                    _nursesForShift.Add(nurse);

            _availableTrainedNurses = new ObservableCollection<Nurse>();
            foreach (var nurse in mainModel.GetAvailableNursesForShift(shift, true))
            {
                bool alreadAdded = false;
                foreach (var assigned in _trainedNursesForShift)
                {
                    if (nurse.Name == assigned.Name)
                        alreadAdded = true;
                }
                if(!alreadAdded)
                   _availableTrainedNurses.Add(nurse);
            }

            _availableNurses = new ObservableCollection<Nurse>();
            foreach (var nurse in mainModel.GetAvailableNursesForShift(shift, false))
            {
                bool alreadAdded = false;
                foreach (var assigned in _nursesForShift)
                {
                    if (nurse.Name == assigned.Name)
                        alreadAdded = true;
                }
                if(!alreadAdded)
                   _availableNurses.Add(nurse);
            }

            _selectedAvailableTrainedNurse = null;
            _selectedTrainedNurseForShift = null;
            _selectedAvailableNurse = null;
            _selectedNurseForShift = null;
        }

        #region Properties

        public ILog Log { get; set; }

        public Brush ValidityColour
        {
            get
            {
                if (_rotaShift.IsRequirementMet()) return new SolidColorBrush(Colors.LightGreen);
                else return new SolidColorBrush(Colors.OrangeRed);
            }
        }

        public string RequirementSummary 
        { 
            get 
            { 
                string summary = _rotaShift.DateStarted.ToShortDateString() +
                " Assigned=" + _rotaShift.AssignedStaff.Count;

                if (_trainedNursesForShift.Count > 0)
                {
                    summary += "\nTrained:";
                    for (int i = 0; i < _trainedNursesForShift.Count; ++i)
                    {
                        if (i > 0) summary += ", ";
                        summary += _trainedNursesForShift[i].FirstName;
                    }
                    summary += " ";
                }

                if (_nursesForShift.Count > 0)
                {
                    summary += "\nUntrained:";
                    for (int i = 0; i < _nursesForShift.Count; ++i)
                    {
                        if (i > 0) summary += ", ";
                        summary += _nursesForShift[i].FirstName;
                    }
                    summary += " ";

                }

                if (_rotaShift.AssignedStaff.Count < _rotaShift.Requirement.MimimumTotal)
                {
                    summary += "\nRequired=" + _rotaShift.Requirement.MimimumTotal;
                }

                return summary;
            }
        }

        public string MinimumTrained { get { return "Minimum of " + _rotaShift.Requirement.MinimumTrained; } }

        public string MinimumOverall { get { return "Minimum of " +
            (_rotaShift.Requirement.MimimumTotal - _trainedNursesForShift.Count);
        }
        }

        public ObservableCollection<Nurse> TrainedNursesForShift 
        {
            get { return _trainedNursesForShift; }
        }
        public ObservableCollection<Nurse> AvailableTrainedNurses
        {
            get { return _availableTrainedNurses; }
        }
        public ObservableCollection<Nurse> NursesForShift
        {
            get { return _nursesForShift; }
        }
        public ObservableCollection<Nurse> AvailableNurses
        {
            get { return _availableNurses; }
        }        

        public Nurse SelectedAvailableTrainedNurse 
        { 
            get {return _selectedAvailableTrainedNurse;} 
            set 
            {
                _selectedAvailableTrainedNurse = value; 
                OnPropertyChanged(() => SelectedAvailableTrainedNurse);
                OnPropertyChanged(() => IsSelectedAvailableTrainedNurse);
            }
        }
        public Boolean  IsSelectedAvailableTrainedNurse {get {return (_selectedAvailableTrainedNurse != null);}}
        
        public Nurse SelectedTrainedNurseForShift 
        { 
            get {return _selectedTrainedNurseForShift;} 
            set 
            {
                _selectedTrainedNurseForShift = value; 
                OnPropertyChanged(() => SelectedTrainedNurseForShift);
                OnPropertyChanged(() => IsSelectedTrainedNurseForShift);
            }
        }
        public Boolean  IsSelectedTrainedNurseForShift {get {return (_selectedTrainedNurseForShift != null);}}
        
        public Nurse SelectedAvailableNurse 
        { 
            get {return _selectedAvailableNurse;} 
            set 
            {
                _selectedAvailableNurse = value; 
                OnPropertyChanged(() => SelectedAvailableNurse);
                OnPropertyChanged(() => IsSelectedAvailableNurse);
            }
        }
        public Boolean  IsSelectedAvailableNurse {get {return (_selectedAvailableNurse != null);}}
        
        public Nurse SelectedNurseForShift 
        { 
            get {return _selectedNurseForShift;} 
            set 
            {
                _selectedNurseForShift = value; 
                OnPropertyChanged(() => SelectedNurseForShift);
                OnPropertyChanged(() => IsSelectedNurseForShift);
            }
        }
        public Boolean  IsSelectedNurseForShift {get {return (_selectedNurseForShift != null);}}

        public RotaShift RotaShift { get { return _rotaShift; } }

        #endregion
        
        #region Command Handlers

        public void AssignSelectedAvailableTrainedNurseCommandAction()
        {
            _rotaShift.AssignedStaff.Add(_selectedAvailableTrainedNurse);
            _trainedNursesForShift.Add(_selectedAvailableTrainedNurse);
            _availableTrainedNurses.Remove(_selectedAvailableTrainedNurse);
            _selectedAvailableTrainedNurse = null;
            OnPropertyChanged("");
            _parentVM.UpdateNurseOffDuty();
        }
        public void FreeSelectedTrainedNurseCommandAction()
        {
            Nurse nurseToFree = _selectedTrainedNurseForShift;
            _rotaShift.AssignedStaff.Remove(_selectedTrainedNurseForShift);
            _trainedNursesForShift.Remove(_selectedTrainedNurseForShift);
            _availableTrainedNurses.Add(nurseToFree);
            _selectedTrainedNurseForShift = null;
            OnPropertyChanged("");
            _parentVM.UpdateNurseOffDuty();
        }
        public void AssignSelectedAvailableNurseCommandAction()
        {
            _rotaShift.AssignedStaff.Add(_selectedAvailableNurse);
            _nursesForShift.Add(_selectedAvailableNurse);
            _availableNurses.Remove(_selectedAvailableNurse);
            _selectedAvailableNurse = null;
            OnPropertyChanged("");
            _parentVM.UpdateNurseOffDuty();
        }
        public void FreeSelectedNurseCommandAction()
        {
            Nurse nurseToFree = _selectedNurseForShift;
            _rotaShift.AssignedStaff.Remove(_selectedNurseForShift);
            _nursesForShift.Remove(_selectedNurseForShift);
            _availableNurses.Add(nurseToFree);
            _selectedNurseForShift = null;
            OnPropertyChanged("");
            _parentVM.UpdateNurseOffDuty();
        }

        #endregion

        #region Commands

        public ICommand AssignSelectedAvailableTrainedNurseCommand
        {
            get
            {
                return _assignSelectedAvailableTrainedNurseCommand ??
                    (_assignSelectedAvailableTrainedNurseCommand = 
                        new CommandHandler(() => AssignSelectedAvailableTrainedNurseCommandAction(), true));
            }
        }
        public ICommand FreeSelectedTrainedNurseCommand
        {
            get
            {
                return _freeSelectedTrainedNurseCommand ??
                    (_freeSelectedTrainedNurseCommand = new CommandHandler(() => FreeSelectedTrainedNurseCommandAction(), true));
            }
        }
        public ICommand AssignSelectedAvailableNurseCommand
        {
            get
            {
                return _assignSelectedAvailableNurseCommand ??
                    (_assignSelectedAvailableNurseCommand = new CommandHandler(() => AssignSelectedAvailableNurseCommandAction(), true));
            }
        }
        public ICommand FreeSelectedNurseCommand
        {
            get
            {
                return _freeSelectedNurseCommand ??
                    (_freeSelectedNurseCommand = new CommandHandler(() => FreeSelectedNurseCommandAction(), true));
            }
        }

        #endregion

        #region Member variables

        private WardStaffingViewModel _parentVM;
        private WardModel _mainModel;
        private RotaShift _rotaShift;

        private ObservableCollection<Nurse> _trainedNursesForShift;
        private ObservableCollection<Nurse> _availableTrainedNurses;
        private ObservableCollection<Nurse> _nursesForShift;
        private ObservableCollection<Nurse> _availableNurses;

        private Nurse _selectedAvailableTrainedNurse;
        private Nurse _selectedTrainedNurseForShift; 
        private Nurse _selectedAvailableNurse;
        private Nurse _selectedNurseForShift;

        private ICommand _assignSelectedAvailableTrainedNurseCommand;
        private ICommand _freeSelectedTrainedNurseCommand;
        private ICommand _assignSelectedAvailableNurseCommand;
        private ICommand _freeSelectedNurseCommand;

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
