using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Forms;
using log4net;

using DataReaderTester.Models;
using DataReaderTester.ViewModels.Utilities;
using TprFileReader.LAS;

namespace DataReaderTester.ViewModels
{
    public class LasCurveViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public LasCurveViewModel(ILog log)
        {
            Log = log;
            _column = null;
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

        public LasCurve Column 
        { 
            get { return _column; }
            set
            {
                if (value == null) return;
                _column = value;
                OnPropertyChanged(() => Name);
                OnPropertyChanged(() => Unit);
                OnPropertyChanged(() => Api);
                OnPropertyChanged(() => Description);

                OnPropertyChanged(() => Min);
                OnPropertyChanged(() => Max);
                OnPropertyChanged(() => NumDataItems);
                OnPropertyChanged(() => DataItems);
            }
        }


        public String Name
        {
            get
            {
                if (_column != null)
                    return _column.Descriptor.Mnemonic;
                return "Not Set";
            }
        }
        public String Unit
        {
            get
            {
                if (_column != null)
                    return _column.Descriptor.Unit;
                return "Not Set";
            }
        }
        public String Api
        {
            get
            {
                if (_column != null)
                    return _column.Descriptor.DataValue;
                return "Not Set";
            }
        }
        public String Description
        {
            get
            {
                if (_column != null)
                    return _column.Descriptor.Description;
                return "Not Set";
            }
        }

        public Double Min
        {
            get
            {
                if (_column != null)
                    return _column.LogDataDoubles.Min();
                return -1;
            }
        }
        public Double Max
        {
            get
            {
                if (_column != null)
                    return _column.LogDataDoubles.Max();
                return -1;
            }
        }
        public Int32 NumDataItems
        {
            get
            {
                if (_column != null)
                    return _column.LogDataDoubles.Count;
                return -1;
            }
        }
        public List<double> DataItems
        {
            get
            {
                if (_column != null)
                    return _column.LogDataDoubles;
                return new List<double>();
            }
        }
        
        #endregion

        private LasCurve _column;
    }
}
