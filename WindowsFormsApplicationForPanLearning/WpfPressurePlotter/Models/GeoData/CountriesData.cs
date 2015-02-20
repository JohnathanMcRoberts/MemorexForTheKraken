using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace WpfPressurePlotter.Models.GeoData
{
    public class CountriesData
    {
        public CountriesData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(Properties.Resources.countries_world));
            ParseCountries(doc);
        }
        private readonly Dictionary<string, CountryGeography> _countries = new Dictionary<string, CountryGeography>();
        private void ParseCountries(XmlDocument doc)
        {
            var nameNodes = doc.SelectNodes("//Document/Placemark/name");
            
            var placemarkNodes = doc.SelectNodes("//Document/Placemark");

            if (placemarkNodes == null) return;

            foreach (var node in placemarkNodes)
            {
                XmlElement element = (XmlElement)node;
                CountryGeography country = CountryGeography.Create(element);
                _countries.Add(country.Name, country);
            }
        }

        public List<string> CountryNames { get { return _countries.Keys.ToList(); } }
        public List<CountryGeography> Countries { get { return _countries.Values.ToList(); } }
    }
}
