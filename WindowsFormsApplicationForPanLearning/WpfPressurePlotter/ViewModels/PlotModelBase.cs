using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

using log4net;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using WpfPressurePlotter.ViewModels.Utilities;
using WpfPressurePlotter.ViewModels.Settings;

namespace WpfPressurePlotter.ViewModels
{
    public abstract class PlotViewModelBase : INotifyPropertyChanged
    {
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

        #region Properties

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }


        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        public PlotSettingViewModelBase CurrentPage
        {
            get { return _currentPage; }
            protected set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentSetting = false;

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentSetting = true;

                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("IsOnLastPage");
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return CurrentPageIndex == Pages.Count - 1; }
        }
        int CurrentPageIndex
        {
            get
            {

                if (CurrentPage == null)
                {
                    Debug.Fail("Why is the current page null?");
                }

                return Pages.IndexOf(CurrentPage);
            }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public ReadOnlyCollection<PlotSettingViewModelBase> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new ReadOnlyCollection<PlotSettingViewModelBase>(GetSettings());
                }

                return _pages;
            }
        }


        #endregion

        #region Private data

        private ReadOnlyCollection<PlotSettingViewModelBase> _pages;

        private PlotSettingViewModelBase _currentPage;

        protected PlotModel _plotModel;

        protected WpfPressurePlotterViewModel _plotterViewModel;

        protected readonly List<OxyColor> colors = new List<OxyColor>
                                            {
                                                OxyColors.Blue,
                                                OxyColors.Red,
                                                OxyColors.Green,
                                                OxyColors.Yellow,
                                                OxyColors.DarkSlateGray,
                                                OxyColors.Coral,
                                                OxyColors.Chartreuse,
                                                OxyColors.Thistle,
                                                OxyColors.OrangeRed,
                                                OxyColors.Plum,
                                                OxyColors.DarkCyan,
                                                OxyColors.Magenta,
                                                OxyColors.LimeGreen
                                            };

        protected readonly List<MarkerType> markerTypes = new List<MarkerType>
                                                   {
                                                       MarkerType.Plus,
                                                       MarkerType.Star,
                                                       MarkerType.Diamond,
                                                       MarkerType.Triangle,
                                                       MarkerType.Cross
                                                   };

        private ICommand _applySettingsCommand;
        private ICommand _hideSettingsCommand;

        #endregion

        #region Utility functions
        protected static double GetMajorStepForRange(double majorRange)
        {
            double step = majorRange / 5;
            step = GetRoundedInterval(step);
            return step;
        }

        protected static double GetRoundedInterval(double step)
        {
            bool isNegative = false;
            if (step < 0)
            {
                isNegative = true;
                step *= -1.0;
            }


            if (step < 1)
                step = 1;
            else if (step < 5)
                step = 5;
            else if (step < 10)
                step = 10;
            else if (step < 25)
                step = 25;
            else if (step < 50)
                step = 50;
            else if (step < 75)
                step = 75;
            else if (step < 100)
                step = 100;
            else if (step < 250)
                step = 250;
            else if (step < 500)
                step = 500;
            else if (step < 750)
                step = 750;
            else if (step < 1000)
                step = 1000;
            else if (step < 2000)
                step = 2000;
            else if (step < 5000)
                step = 5000;
            else if (step < 7500)
                step = 7500;
            else if (step < 10000)
                step = 10000;

            if (isNegative)
                step *= -1;

            return step;
        }

        protected double GetRoundedInterval(double minMax, double step)
        {
            int rounded = (int)minMax;
            bool isNegative = false;
            if (minMax < 0)
            {
                rounded *= -1;
                isNegative = true;
            }

            int numSteps = 1 + (rounded / ((int)step));
            if (isNegative) numSteps *= -1;

            return numSteps * step;
        }



        protected void SetDefaultRangesForPlot(
            double minEast, double minNorth, double maxEast, double maxNorth,
            out double step, out double roundedMinEast, out double roundedMaxEast,
            out double roundedMinNorth, out double roundedMaxNorth)
        {

            double eastRange = maxEast - minEast;
            double northRange = maxNorth - minNorth;

            double majorRange = eastRange;
            if (northRange > eastRange)
                majorRange = northRange;

            step = GetMajorStepForRange(majorRange);

            roundedMinEast = GetRoundedInterval(minEast - (eastRange * 0.1), step);
            roundedMaxEast = GetRoundedInterval(maxEast + (eastRange * 0.1), step);

            _plotModel.Axes[0].Minimum = roundedMinEast;
            _plotModel.Axes[0].Maximum = roundedMaxEast;
            _plotModel.Axes[0].MajorStep = step;

            roundedMinNorth = GetRoundedInterval(
                minNorth - (northRange * 0.1), step);
            roundedMaxNorth = GetRoundedInterval(maxNorth + (northRange * 0.1), step);
            _plotModel.Axes[1].Minimum = roundedMinNorth;
            _plotModel.Axes[1].Maximum = roundedMaxNorth;
            _plotModel.Axes[1].MajorStep = step;
        }


        #endregion


        #region Commands

        public ICommand ApplySettingsCommand
        {
            get
            {
                return _applySettingsCommand ??
                    (_applySettingsCommand = new CommandHandler(() => ApplySettingsAction(), true));
            }
        }

        public ICommand HideSettingsCommand
        {
            get
            {
                return _hideSettingsCommand ??
                    (_hideSettingsCommand = new CommandHandler(() => HideSettingsAction(), true));
            }
        }

        #endregion

        #region Command Handlers

        public abstract void ApplySettingsAction();
        public abstract void HideSettingsAction();

        #endregion

        /// <summary>
        /// Returns true if the user has filled in this setting properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract List<PlotSettingViewModelBase> GetSettings();

        internal void RefreshSettings()
        {
            OnPropertyChanged(() => CurrentPage);
        }
    }
}
