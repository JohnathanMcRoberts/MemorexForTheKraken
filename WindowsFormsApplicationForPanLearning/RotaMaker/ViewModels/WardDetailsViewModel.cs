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
            // TODO: Complete member initialization
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public Boolean IsFileOpened { get { return _mainModel.IsFileOpened; } }

        public String BackupFileName { get { return _mainModel.BackupFileName; } }

        public String WardName { get { return _mainModel.WardName; } }
        
        #endregion

        #region Member variables

        private RotaMakerViewModel _mainWindow;
        private WardModel _mainModel;

        private ICommand _loadFromFileCommand;
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

                    OnPropertyChanged(() => IsFileOpened);
                    OnPropertyChanged(() => BackupFileName);
                    OnPropertyChanged(() => WardName);

                    _mainWindow.RefreshForNewWard();

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
            }
        }

        #endregion

    }
}
