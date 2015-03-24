using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using log4net;

using RotaMaker.Models;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace RotaMaker.ViewModels
{
    public class RotaMakerViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public RotaMakerViewModel(MainWindow mainWindow, ILog log)
        {
            Log = log;

            _mainWindow = mainWindow;
            _mainModel = new WardModel(log);
            if (_mainModel.BackupFileName != "")
                _mainModel = WardModel.OpenWardFile(_mainModel.BackupFileName, Log);

            _allStaffVM = new AllStaffViewModel(this, _mainModel, log);
            _wardStaffingVM = new WardStaffingViewModel(this, _mainModel, log);
            _monthlySummaryVM = new MonthlySummaryViewModel(this, _mainModel, log);
            _wardDetailsVM = new WardDetailsViewModel(this, _mainModel, log);
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public AllStaffViewModel AllStaffVM { get {return _allStaffVM;} }
        public WardStaffingViewModel WardStaffingVM { get { return _wardStaffingVM; } }
        public MonthlySummaryViewModel MonthlySummaryVM { get { return _monthlySummaryVM; } }
        public WardDetailsViewModel WardDetailsVM { get { return _wardDetailsVM; } }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private WardModel _mainModel;


        private AllStaffViewModel _allStaffVM;
        private WardStaffingViewModel _wardStaffingVM;
        private MonthlySummaryViewModel _monthlySummaryVM;
        private WardDetailsViewModel _wardDetailsVM;

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

        public void RefreshForNewWard(WardModel model)
        {
            _mainModel = model;

            _allStaffVM = new AllStaffViewModel(this, _mainModel, Log);
            _wardStaffingVM = new WardStaffingViewModel(this, _mainModel, Log);
            _monthlySummaryVM = new MonthlySummaryViewModel(this, _mainModel, Log);
            _wardDetailsVM = new WardDetailsViewModel(this, _mainModel, Log);

            OnPropertyChanged(() => AllStaffVM);
            OnPropertyChanged(() => WardStaffingVM);
            OnPropertyChanged(() => MonthlySummaryVM);
            OnPropertyChanged(() => WardDetailsVM);
        }
    }
}
