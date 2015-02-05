using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using log4net;

using DataReaderTester.Models;
using DataReaderTester.ViewModels.Utilities;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace DataReaderTester.ViewModels
{
    public class DataReaderMainViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public DataReaderMainViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainDataReaderModel = new MainDataReaderModel(log);

            //CreateTabViewModels();

            //_selectWellpathsEnabled = false;
            //_plotViewsEnabled = false;
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

        #region Properties

        public ILog Log { get; set; }

        public Boolean IsFileOpened { get; private set; }

        public String TprFileName { get { return _mainDataReaderModel.TprFileName; } }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private MainDataReaderModel _mainDataReaderModel;
        private ICommand _chooseTprFileCommand;

        #endregion

        #region Commands

        public ICommand ChooseTprFileCommand
        {
            get
            {
                return _chooseTprFileCommand ??
                    (_chooseTprFileCommand = new CommandHandler(() => ChooseTprFileCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void ChooseTprFileCommandAction()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileName = _mainDataReaderModel.TprFileName;

            // TODO - get the file types from the available serialisers
            fileDialog.Filter = @"All files (*.*)|*.*|TPR files (*.tpr)|*.tpr";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainDataReaderModel.TprFileName = fileDialog.FileName;

                Properties.Settings.Default.TprFile =
                    _mainDataReaderModel.TprFileName;
                Properties.Settings.Default.Save();

                using (new WaitCursor())
                {
                    _mainDataReaderModel.OpenSimpleTpr();

                    IsFileOpened = true;
                    OnPropertyChanged(() => IsFileOpened);
                }
            }
        }

        #endregion
    }
}
