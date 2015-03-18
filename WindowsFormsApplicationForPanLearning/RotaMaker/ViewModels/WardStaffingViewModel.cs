using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using log4net;

using RotaMaker.Models;

namespace RotaMaker.ViewModels
{
    public class WardStaffingViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public WardStaffingViewModel(RotaMakerViewModel mainWindow, WardModel mainModel, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _mainModel = mainModel;
            // TODO: Complete member initialization
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }

        #endregion

        #region Member variables

        private RotaMakerViewModel _mainWindow;
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
    }
}
