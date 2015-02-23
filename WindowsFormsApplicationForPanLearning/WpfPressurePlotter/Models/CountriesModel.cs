using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfPressurePlotter.Models.GeoData;

namespace WpfPressurePlotter.Models
{
    public class CountriesModel
    {
        private log4net.ILog Log;

        public CountriesModel(log4net.ILog log)
        {
            Log = log;
            LastPngFile =  Properties.Settings.Default.LastPngFile;

            _countries = new CountriesData();

        }

        private CountriesData _countries;

        public List<string> CountryNames
        {
            get { return _countries.CountryNames; }
        }
        public List<CountryGeography> Countries
        {
            get { return _countries.Countries; }
        }

        public string LastPngFile { get; set; }

    }
}
