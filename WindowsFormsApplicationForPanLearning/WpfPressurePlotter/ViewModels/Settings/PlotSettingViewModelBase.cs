using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using WpfPressurePlotter.Models;

namespace WpfPressurePlotter.ViewModels.Settings
{
    public abstract class PlotSettingViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        protected MainPressurePlotterModel _mainPlotData;
        protected PlotViewModelBase _parentView;
        protected bool _isCurrentSetting;

        #endregion // Fields

        #region Constructor

        protected PlotSettingViewModelBase(MainPressurePlotterModel wellPlotData, PlotViewModelBase parentView)
        {
            _mainPlotData = wellPlotData;
            _parentView = parentView;
        }

        #endregion // Constructor

        #region Properties

        public MainPressurePlotterModel MainPlotData
        {
            get { return _mainPlotData; }
        }

        public abstract string DisplayName { get; }

        public bool IsCurrentSetting
        {
            get { return _isCurrentSetting; }
            set
            {
                if (value == _isCurrentSetting)
                    return;

                _isCurrentSetting = value;
                OnPropertyChanged("IsCurrentSetting");
            }
        }

        public virtual void ActivateSetting()
        {
        }

        public virtual void LeaveSetting()
        {
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Returns true if the user has filled in this setting properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        #endregion // Methods

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
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

        public static Dictionary<string, int> ToList<T>() where T : struct
        {
            return ((IEnumerable<T>)Enum.GetValues(typeof(T))).ToDictionary(item => item.ToString(), item => Convert.ToInt32(item));
        }
    }
}
