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

            AllStaffVM = new AllStaffViewModel(this, _mainModel, log);
            WardStaffingVM = new WardStaffingViewModel(this, _mainModel, log);
            OffDutyVM = new MonthlySummaryViewModel(this, _mainModel, log);
            WardDetailsVM = new WardDetailsViewModel(this, _mainModel, log);
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public AllStaffViewModel AllStaffVM { get; set; }
        public WardStaffingViewModel WardStaffingVM { get; set; }
        public MonthlySummaryViewModel OffDutyVM { get; set; }
        public WardDetailsViewModel WardDetailsVM { get; set; }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private WardModel _mainModel;

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

        public void RefreshForNewWard()
        {
            OnPropertyChanged(() => AllStaffVM);
            OnPropertyChanged(() => WardStaffingVM);
            OnPropertyChanged(() => OffDutyVM);
        }
    }
}
