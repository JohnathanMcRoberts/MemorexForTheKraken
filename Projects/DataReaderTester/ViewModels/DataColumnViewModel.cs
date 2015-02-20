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
using TprFileReader;

namespace DataReaderTester.ViewModels
{
    public class DataColumnViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public DataColumnViewModel( ILog log)
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

        public TprColumnDefinition Column 
        { 
            get { return _column; }
            set
            {
                if (value == null) return;
                _column = value;
                OnPropertyChanged(() => Name);
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
                    return _column.Name;
                return "Not Set";
            }
        }
        public Double Min
        {
            get
            {
                if (_column != null)
                    return _column.ColumnNumerics.Min();
                return -1;
            }
        }
        public Double Max
        {
            get
            {
                if (_column != null)
                    return _column.ColumnNumerics.Max();
                return -1;
            }
        }
        public Int32 NumDataItems
        {
            get
            {
                if (_column != null)
                    return _column.ColumnNumerics.Count;
                return -1;
            }
        }
        public List<double> DataItems
        {
            get
            {
                if (_column != null)
                    return _column.ColumnNumerics;
                return new List<double>();
            }
        }
        
        #endregion

        private TprColumnDefinition _column;
    }
}
