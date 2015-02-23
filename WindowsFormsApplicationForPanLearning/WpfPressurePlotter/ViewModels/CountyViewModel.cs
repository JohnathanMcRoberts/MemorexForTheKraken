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
    public class CountyViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public CountyViewModel(MainWindow mainWindow, CountyGeography geography, ILog log)
        {
            _mainWindow = mainWindow;
            Log = log;
            _geography = geography;
            _neighbourVMs = null;
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
        
        public double CentralLongitude { get { return _geography.CentroidLongitude; } }
        public double CentralLatitude { get { return _geography.CentroidLatitude; } }

        public double MinLongitude { get { return _geography.MinLongitude; } }
        public double MinLatitude { get { return _geography.MinLatitude; } }

        public double MaxLongitude { get { return _geography.MaxLongitude; } }
        public double MaxLatitude { get { return _geography.MaxLatitude; } }


        public string CentralLongitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(CentralLongitude); } }
        public string CentralLatitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(CentralLatitude); } }

        public string MinLongitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(MinLongitude); } }
        public string MinLatitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(MinLatitude); } }

        public string MaxLongitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(MaxLongitude); } }
        public string MaxLatitudeDms { get { return CountryViewModel.ToDegreesMinutesSeconds(MaxLatitude); } }

        public int NumberLandBlocks { get { return _geography.LandBlocks.Count; } }

        public List<PolygonBoundary> LandBlocks { get { return _geography.LandBlocks; } }


        public List<CountyViewModel> Neighbours 
        { 
            get 
            { 
                if (_neighbourVMs != null) return _neighbourVMs;
                _neighbourVMs = new List<CountyViewModel>();
                foreach (var neighbour in _geography.Neighbours)
                    _neighbourVMs.Add(new CountyViewModel(_mainWindow, neighbour.County, Log));
                return _neighbourVMs; 
            } 
        }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private CountyGeography _geography;
        private List<CountyViewModel> _neighbourVMs;

        #endregion
        
    }
}