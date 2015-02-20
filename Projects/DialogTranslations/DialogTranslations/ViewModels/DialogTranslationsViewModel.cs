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

using DialogTranslations.Model;
using DialogTranslations.ViewModels.Utilities;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace DialogTranslations.ViewModels
{
    public class DialogTranslationsViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public DialogTranslationsViewModel(MainWindow mainWindow, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _dialogTranslationsModel = new DialogTranslationsModel(log);

            //CreateTabViewModels();

            //_selectWellpathsEnabled = false;
            //_plotViewsEnabled = false;
        }
        
        #endregion

        #region Member Data

        private MainWindow _mainWindow;

        private ICommand _selectEnglishDirectoryCommand;
        private ICommand _selectTranslationDirectoryCommand;
        private ICommand _generateTranslationFileCommand;

        private ICommand _chooseEnglishDirectoryCommand;
        private ICommand _chooseTranslationDirectoryCommand;
        private ICommand _generateCompleteTranslationFileCommand;

        private DialogTranslationsModel _dialogTranslationsModel;

        #endregion

        #region Properties

        public ILog Log { get; set; }

        public Boolean IsFileOpened { get; set; }

        public string EnglishDirectoryName
        {
            get
            {
                return _dialogTranslationsModel.EnglishDirectoryName;
            }
        }
        public string TranslationDirectoryName
        {
            get
            {
                return _dialogTranslationsModel.TranslationDirectoryName;
            }
        }

        public string CompleteEnglishDirectoryName
        {
            get
            {
                return _dialogTranslationsModel.CompleteEnglishDirectoryName;
            }
        }
        public string CompleteTranslationDirectoryName
        {
            get
            {
                return _dialogTranslationsModel.CompleteTranslationDirectoryName;
            }
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

        #region Commands

        public ICommand GenerateTranslationFileCommand
        {
            get
            {
                return _generateTranslationFileCommand ??
                    (_generateTranslationFileCommand = new CommandHandler(() => GenerateTranslationFileCommandAction(), true));
            }
        }
        public ICommand SelectEnglishDirectoryCommand
        {
            get
            {
                return _selectEnglishDirectoryCommand ??
                    (_selectEnglishDirectoryCommand = new CommandHandler(() => SelectEnglishDirectoryCommandAction(), true));
            }
        }
        public ICommand SelectTranslationDirectoryCommand
        {
            get
            {
                return _selectTranslationDirectoryCommand ??
                    (_selectTranslationDirectoryCommand = new CommandHandler(() => SelectTranslationDirectoryCommandAction(), true));
            }
        }

        public ICommand GenerateCompleteTranslationFileCommand
        {
            get
            {
                return _generateCompleteTranslationFileCommand ??
                    (_generateCompleteTranslationFileCommand = new CommandHandler(() => GenerateCompleteTranslationFileCommandAction(), true));
            }
        }
        public ICommand ChooseEnglishDirectoryCommand
        {
            get
            {
                return _chooseEnglishDirectoryCommand ??
                    (_chooseEnglishDirectoryCommand = new CommandHandler(() => ChooseEnglishDirectoryCommandAction(), true));
            }
        }
        public ICommand ChooseTranslationDirectoryCommand
        {
            get
            {
                return _chooseTranslationDirectoryCommand ??
                    (_chooseTranslationDirectoryCommand = new CommandHandler(() => ChooseTranslationDirectoryCommandAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public void SelectEnglishDirectoryCommandAction()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = _dialogTranslationsModel.EnglishDirectoryName;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.EnglishDirectoryName = fbd.SelectedPath;
                Properties.Settings.Default.EnglishDirectory = _dialogTranslationsModel.EnglishDirectoryName;
                Properties.Settings.Default.Save(); // Saves settings in application configuration file

                OnPropertyChanged(() => EnglishDirectoryName);
            }

        }

        public void SelectTranslationDirectoryCommandAction()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = _dialogTranslationsModel.TranslationDirectoryName;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.TranslationDirectoryName = fbd.SelectedPath;

                Properties.Settings.Default.TranslationDirectory = _dialogTranslationsModel.TranslationDirectoryName;
                Properties.Settings.Default.Save(); // Saves settings in application configuration file

                OnPropertyChanged(() => TranslationDirectoryName);
            }

        }

        public void GenerateTranslationFileCommandAction()
        {
            if (!_dialogTranslationsModel.EnglishDirectoryNameSet)
            {
                MessageBox.Show("Must set an English directory");
                return;
            }
            if (!_dialogTranslationsModel.TranslationDirectoryNameSet)
            {
                MessageBox.Show("Must set a Translation directory");
                return;
            }


            SaveFileDialog fileDialog = new SaveFileDialog();

            // TODO - get the file types from the available serialisers
            fileDialog.Filter = @"All files (*.*)|*.*|csv files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.OutputFileName = fileDialog.FileName;

                Properties.Settings.Default.OutputFile = 
                    _dialogTranslationsModel.OutputFileName;
                Properties.Settings.Default.Save(); 

                using (new WaitCursor())
                {
                    _dialogTranslationsModel.GenerateTranslationCsv();

                    IsFileOpened = true;
                }
            }
        }


        public void ChooseEnglishDirectoryCommandAction()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = _dialogTranslationsModel.CompleteEnglishDirectoryName;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.CompleteEnglishDirectoryName = fbd.SelectedPath;
                Properties.Settings.Default.CompleteEnglishDirectory = 
                    _dialogTranslationsModel.CompleteEnglishDirectoryName;
                Properties.Settings.Default.Save(); 

                OnPropertyChanged(() => CompleteEnglishDirectoryName);
            }

        }

        public void ChooseTranslationDirectoryCommandAction()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = _dialogTranslationsModel.CompleteTranslationDirectoryName;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.CompleteTranslationDirectoryName = fbd.SelectedPath;

                Properties.Settings.Default.CompleteTranslationDirectory = 
                    _dialogTranslationsModel.CompleteTranslationDirectoryName;
                Properties.Settings.Default.Save(); 

                OnPropertyChanged(() => CompleteTranslationDirectoryName);
            }

        }

        public void GenerateCompleteTranslationFileCommandAction()
        {
            if (!_dialogTranslationsModel.CompleteEnglishDirectoryNameSet)
            {
                MessageBox.Show("Must set an English directory");
                return;
            }
            if (!_dialogTranslationsModel.CompleteTranslationDirectoryNameSet)
            {
                MessageBox.Show("Must set a Translation directory");
                return;
            }


            SaveFileDialog fileDialog = new SaveFileDialog();

            // TODO - get the file types from the available serialisers
            fileDialog.Filter = @"All files (*.*)|*.*|csv files (*.csv)|*.csv";
            fileDialog.FilterIndex = 4;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _dialogTranslationsModel.OutputFileName = fileDialog.FileName;

                Properties.Settings.Default.OutputFile = _dialogTranslationsModel.OutputFileName;
                Properties.Settings.Default.Save(); // Saves settings in application configuration file

                using (new WaitCursor())
                {
                    _dialogTranslationsModel.GenerateCompleteTranslationCsv();

                    IsFileOpened = true;
                }
            }
        }

        #endregion
    }
}
