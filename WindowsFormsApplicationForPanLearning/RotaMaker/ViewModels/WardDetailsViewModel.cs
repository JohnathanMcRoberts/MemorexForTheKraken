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

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace RotaMaker.ViewModels
{
    public class WardDetailsViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public WardDetailsViewModel(RotaMakerViewModel mainWindow, WardModel mainModel, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainModel = mainModel;
            InitialiseShiftRequirementsViewModels();
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public Boolean IsFileOpened { get { return _mainModel.IsFileOpened; } }

        public String BackupFileName { get { return _mainModel.BackupFileName; } }

        public String WardName { get { return _mainModel.WardName; } }
        
        #region Early Shift Requirement VMs

        public ShiftRequirementViewModel MondayEarlyVM { get {return _mondayEarlyVM;}}
        public ShiftRequirementViewModel TuesdayEarlyVM { get { return _tuesdayEarlyVM; } }
        public ShiftRequirementViewModel WednesdayEarlyVM { get { return _wednesdayEarlyVM; } }
        public ShiftRequirementViewModel ThursdayEarlyVM { get { return _thursdayEarlyVM; } }
        public ShiftRequirementViewModel FridayEarlyVM { get { return _fridayEarlyVM; } }
        public ShiftRequirementViewModel SaturdayEarlyVM { get { return _saturdayEarlyVM; } }
        public ShiftRequirementViewModel SundayEarlyVM { get { return _sundayEarlyVM; } }

        #endregion

        #region Late Shift Requirement VMs

        public ShiftRequirementViewModel MondayLateVM { get { return _mondayLateVM; } }
        public ShiftRequirementViewModel TuesdayLateVM { get { return _tuesdayLateVM; } }
        public ShiftRequirementViewModel WednesdayLateVM { get { return _wednesdayLateVM; } }
        public ShiftRequirementViewModel ThursdayLateVM { get { return _thursdayLateVM; } }
        public ShiftRequirementViewModel FridayLateVM { get { return _fridayLateVM; } }
        public ShiftRequirementViewModel SaturdayLateVM { get { return _saturdayLateVM; } }
        public ShiftRequirementViewModel SundayLateVM { get { return _sundayLateVM; } }

        #endregion

        #region Night Shift Requirement VMs

        public ShiftRequirementViewModel MondayNightVM { get { return _mondayNightVM; } }
        public ShiftRequirementViewModel TuesdayNightVM { get { return _tuesdayNightVM; } }
        public ShiftRequirementViewModel WednesdayNightVM { get { return _wednesdayNightVM; } }
        public ShiftRequirementViewModel ThursdayNightVM { get { return _thursdayNightVM; } }
        public ShiftRequirementViewModel FridayNightVM { get { return _fridayNightVM; } }
        public ShiftRequirementViewModel SaturdayNightVM { get { return _saturdayNightVM; } }
        public ShiftRequirementViewModel SundayNightVM { get { return _sundayNightVM; } }

        #endregion

        #endregion

        #region Member variables

        private RotaMakerViewModel _mainWindow;
        private WardModel _mainModel;


        private ICommand _loadFromFileCommand;
        private ICommand _saveToFileCommand;

        #region Early Shift Requirements VMs

        private ShiftRequirementViewModel _mondayEarlyVM;
        private ShiftRequirementViewModel _tuesdayEarlyVM;
        private ShiftRequirementViewModel _wednesdayEarlyVM;
        private ShiftRequirementViewModel _thursdayEarlyVM;
        private ShiftRequirementViewModel _fridayEarlyVM;
        private ShiftRequirementViewModel _saturdayEarlyVM;
        private ShiftRequirementViewModel _sundayEarlyVM;

        #endregion

        #region Late Shift Requirements VMs

        private ShiftRequirementViewModel _mondayLateVM;
        private ShiftRequirementViewModel _tuesdayLateVM;
        private ShiftRequirementViewModel _wednesdayLateVM;
        private ShiftRequirementViewModel _thursdayLateVM;
        private ShiftRequirementViewModel _fridayLateVM;
        private ShiftRequirementViewModel _saturdayLateVM;
        private ShiftRequirementViewModel _sundayLateVM;

        #endregion

        #region Night Shift Requirements VMs

        private ShiftRequirementViewModel _mondayNightVM;
        private ShiftRequirementViewModel _tuesdayNightVM;
        private ShiftRequirementViewModel _wednesdayNightVM;
        private ShiftRequirementViewModel _thursdayNightVM;
        private ShiftRequirementViewModel _fridayNightVM;
        private ShiftRequirementViewModel _saturdayNightVM;
        private ShiftRequirementViewModel _sundayNightVM;

        #endregion

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

        #region Commands

        public ICommand LoadFromFileCommand
        {
            get
            {
                return _loadFromFileCommand ??
                    (_loadFromFileCommand = new CommandHandler(() => LoadFromFileCommandAction(), true));
            }
        }

        public ICommand SaveToFileCommand
        {
            get
            {
                return _saveToFileCommand ??
                    (_saveToFileCommand = new CommandHandler(() => SaveToFileCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void LoadFromFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainModel.BackupFileName;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|XML files (*.xml)|*.xml";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.BackupFileName = fileDialog.FileName;

                Properties.Settings.Default.BackupFileName =
                    _mainModel.BackupFileName;
                Properties.Settings.Default.Save();

                using (new WaitCursor())
                {
                    _mainModel = WardModel.OpenWardFile(_mainModel.BackupFileName,Log);

                    if (_mainModel == null)
                    {
                        _mainModel = new WardModel(Log);
                        _mainModel.StaffingRequirement.InitialiseMinimums();
                        _mainModel.IsFileOpened = false;
                        _mainModel.BackupFileName = fileDialog.FileName;
                        _mainModel.WardName = "Dummy Ward";
                    }

                    if (_mainModel.Staff.Count == 0)                        
                        _mainModel.Staff.Add( Nurse.CreateDummyNurse() );
                    
                    OnPropertyChanged(() => IsFileOpened);
                    OnPropertyChanged(() => BackupFileName);
                    OnPropertyChanged(() => WardName);

                    InitialiseShiftRequirementsViewModels();

                    OnPropertyChanged("");
                    _mainWindow.RefreshForNewWard(_mainModel);
                }
            }
        }

        public void SaveToFileCommandAction()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = _mainModel.BackupFileName;

            // TODO - get the file types from the available serializers
            fileDialog.Filter = @"All files (*.*)|*.*|XML files (*.xml)|*.xml";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainModel.BackupFileName = fileDialog.FileName;

                Properties.Settings.Default.BackupFileName =
                    _mainModel.BackupFileName;
                Properties.Settings.Default.Save();

                WardModel.SaveToFile(_mainModel, _mainModel.BackupFileName);

                //using (var stream = File.Create(fileDialog.FileName))
                //{
                //    OxyPlot.Wpf.PngExporter.Export(_plotModelAllConstuencies, stream, 800, 600, OxyColors.White);
                //}

                OnPropertyChanged(() => BackupFileName);
            }
        }

        #endregion

        #region Utility functions

        private void InitialiseShiftRequirementsViewModels()
        {
            InitialiseEarlyShiftViewModels();
            InitialiseLateShiftViewModels();
            InitialiseNightShiftViewModels();
        }

        private void InitialiseEarlyShiftViewModels()
        {
            _mondayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayEarlyVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Early, ShiftTime.ShiftDay.Sunday)],
                Log);
        }
        private void InitialiseLateShiftViewModels()
        {
            _mondayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayLateVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Late, ShiftTime.ShiftDay.Sunday)],
                Log);
        }
        private void InitialiseNightShiftViewModels()
        {
            _mondayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Monday)],
                Log);
            _tuesdayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Tuesday)],
                Log);
            _wednesdayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Wednesday)],
                Log);
            _thursdayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Thursday)],
                Log);
            _fridayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Friday)],
                Log);
            _saturdayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Saturday)],
                Log);
            _sundayNightVM = new ShiftRequirementViewModel(
                _mainModel.StaffingRequirement.MinimumStaffing[WardModel.GetShiftIndex(ShiftTime.Shift.Night, ShiftTime.ShiftDay.Sunday)],
                Log);
        }

        #endregion
    }
}
