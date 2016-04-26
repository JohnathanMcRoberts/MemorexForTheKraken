using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class PartyForecast
    {
        #region Public Properties

        public string Name { get; private set; }
        public double PreviousPercentage { get; private set; }
        public double PredictedPercentage 
        { 
            get 
            { 
                return _predictedPercentage; 
            }
            set 
            { 
                _predictedPercentage = value;  
                _predictedSwing = PredictedPercentage - PreviousPercentage; 
            }
        }
        public double PredictedSwing
        {
            get
            {
                return _predictedSwing;
            }
            set
            {
                _predictedSwing = value;
                _predictedPercentage = PreviousPercentage + _predictedSwing;
            }
        }

        #endregion

        #region Constructor

        public PartyForecast(string name, double previousPercentage)
        {
            Name = name;
            _predictedPercentage = PreviousPercentage = previousPercentage;
            _predictedSwing = PredictedPercentage - PreviousPercentage;
        }

        #endregion

        #region Private Data

        private double _predictedPercentage;
        private double _predictedSwing;
        
        #endregion
    }
}
