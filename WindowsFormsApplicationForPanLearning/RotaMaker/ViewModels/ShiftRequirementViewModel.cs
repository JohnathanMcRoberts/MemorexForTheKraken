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
    public class ShiftRequirementViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public ShiftRequirementViewModel(ShiftRequirement shiftRequirement, ILog log)
        {
            Log = log;
            _shiftRequirement = shiftRequirement;
            // TODO: Complete member initialization
        }

        #endregion

        #region Properties

        public ILog Log { get; set; }
        public string RequirementSummary { get { return "Trained=" + TrainedMinumum + ", Overall=" + OverallMinumum; } }
        public int TrainedMinumum
        {
            get { return _shiftRequirement.MinimumTrained; }
            set 
            { 
                if (value != _shiftRequirement.MinimumTrained)
                { 
                    _shiftRequirement.MinimumTrained = value; 
                    OnPropertyChanged(() => TrainedMinumum); 
                    OnPropertyChanged(() => RequirementSummary); 
                } 
            }
        }
        public int OverallMinumum
        {
            get { return _shiftRequirement.MimimumTotal; }
            set
            {
                if (value != _shiftRequirement.MimimumTotal)
                {
                    _shiftRequirement.MimimumTotal = value;
                    OnPropertyChanged(() => OverallMinumum);
                    OnPropertyChanged(() => RequirementSummary);
                }
            }
        }
        #endregion

        #region Member variables

        private ShiftRequirement _shiftRequirement;

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
