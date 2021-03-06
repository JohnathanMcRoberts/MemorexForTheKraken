﻿using System;
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
        public string ISO_A2 { get { return _geography.ISO_A2; } }
        public string ISO_N3 { get { return _geography.ISO_N3; } }
        
        public double CentralLongitude { get { return _geography.CentralLongitude; } }
        public double CentralLatitude { get { return _geography.CentralLatitude; } }

        public double MinLongitude { get { return _geography.MinLongitude; } }
        public double MinLatitude { get { return _geography.MinLatitude; } }

        public double MaxLongitude { get { return _geography.MaxLongitude; } }
        public double MaxLatitude { get { return _geography.MaxLatitude; } }


        public string CentralLongitudeDms { get { return ToDegreesMinutesSeconds(CentralLongitude); } }
        public string CentralLatitudeDms { get { return ToDegreesMinutesSeconds(CentralLatitude); } }

        public string MinLongitudeDms { get { return ToDegreesMinutesSeconds(MinLongitude); } }
        public string MinLatitudeDms { get { return ToDegreesMinutesSeconds(MinLatitude); } }

        public string MaxLongitudeDms { get { return ToDegreesMinutesSeconds(MaxLongitude); } }
        public string MaxLatitudeDms { get { return ToDegreesMinutesSeconds(MaxLatitude); } }

        public int NumberLandBlocks { get { return _geography.LandBlocks.Count; } }

        public List<PolygonBoundary> LandBlocks { get { return _geography.LandBlocks; } }

        #endregion

        #region Member variables

        private MainWindow _mainWindow;
        private CountryGeography _geography;

        #endregion

        public static string ToDegreesMinutesSeconds(double latDegrees)
        {
            int degInt = Math.Abs((int)latDegrees);
            double minutes = (Math.Abs(latDegrees) - degInt) * 60;
            int minInt = (int)minutes;
            double seconds = (minutes - minInt) * 60;
            //dms[0] = latDegrees;
            //dms[1] = minutes;
            //dms[2] = seconds;
            int degrees = (int)latDegrees;
            if (latDegrees < 0 && degInt == 0 && minInt == 0)
                seconds *= -1;
            else if (latDegrees < 0 && degInt == 0)
                minutes *= -1;

            string dms = String.Format("{0}\u00B0 {1}' {2:0.00}\"", degrees, minInt, seconds);
            return dms;
        }


    }
}
