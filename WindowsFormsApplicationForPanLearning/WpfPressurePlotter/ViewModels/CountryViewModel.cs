using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using log4net;

using WpfPressurePlotter.Models;
using WpfPressurePlotter.ViewModels.Utilities;
using WpfPressurePlotter.Models.GeoData;


namespace WpfPressurePlotter.ViewModels
{
    public class CountryViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public CountryViewModel(MainWindow mainWindow, CountryGeography geography, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _geography = geography;

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

        public string Name { get { return _geography.Name; } }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private CountryGeography _geography;

        #endregion
    }
}
