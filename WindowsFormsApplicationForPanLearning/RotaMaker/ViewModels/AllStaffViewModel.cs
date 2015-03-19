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
    public class AllStaffViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public AllStaffViewModel(RotaMakerViewModel mainWindow, WardModel mainModel, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainModel = mainModel;
            // TODO: Complete member initialization

            _nurseVMs = new ObservableCollection<NurseViewModel>();
            foreach (var nurse in _mainModel.Staff)
                _nurseVMs.Add(new NurseViewModel(nurse,this,log));
            if (_nurseVMs.Count > 0)
                SelectedNurseVM = _nurseVMs[0];
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public ObservableCollection<NurseViewModel> Nurses
        {
            get { return _nurseVMs; }
        }

        public NurseViewModel SelectedNurseVM
        {
            get { return _selectedNurseVM; }
            set
            {
                if (value != _selectedNurseVM)
                {
                    _selectedNurseVM = value;
                    OnPropertyChanged(() => SelectedNurseVM);
                }
            }
        }

        public int SelectedNurseIndex
        {
            get { return _selectedNurseIndex; }
            set 
            { 
                if (value != _selectedNurseIndex) 
                { 
                    _selectedNurseIndex = value; 
                    OnPropertyChanged(() => SelectedNurseIndex); 
                } 
            }
        }

        #endregion

        #region Member variables

        private RotaMakerViewModel _mainWindow;
        private WardModel _mainModel;

        private ObservableCollection<string> _nurseNames;
        private ObservableCollection<NurseViewModel> _nurseVMs;
        private NurseViewModel _selectedNurseVM;
        private int _selectedNurseIndex;


        private ICommand _addNewNurseCommand;
        private ICommand _saveToFileCommand;

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

        public ICommand AddNewNurseCommand
        {
            get
            {
                return _addNewNurseCommand ??
                    (_addNewNurseCommand = new CommandHandler(() => AddNewNurseCommandAction(), true));
            }
        }

        public ICommand RemoveSelectedNurseCommand
        {
            get
            {
                return _saveToFileCommand ??
                    (_saveToFileCommand = new CommandHandler(() => SaveToFileCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void AddNewNurseCommandAction()
        {

        }

        public void SaveToFileCommandAction()
        {

        }

        #endregion
    }
}
