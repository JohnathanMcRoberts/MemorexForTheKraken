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

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace WpfPressureViewer.ViewModels
{
    public class MainPressureViewModel : INotifyPropertyChanged
    {

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

        private MainWindow mainWindow;
        private log4net.ILog log;

        public MainPressureViewModel(MainWindow mainWindow, log4net.ILog log)
        {
            // TODO: Complete member initialization
            this.mainWindow = mainWindow;
            this.log = log;
        }
        
        public ReadDataViewModel ReadDataVM {get;set;}

        public SelectTestsAndChannelsViewModel SelectTestsAndChannelsVM {get;set;}

        public TestDesignViewModel TestDesignVM { get; set; }

        public LogLogViewModel LogLogVM {get;set;}

    }
}
