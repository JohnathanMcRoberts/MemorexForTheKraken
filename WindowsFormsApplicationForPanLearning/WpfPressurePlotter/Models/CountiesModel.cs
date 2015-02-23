using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using WpfPressurePlotter.Models.GeoData;

namespace WpfPressurePlotter.Models
{
    public class CountiesModel
    {
        #region Constructor

        public CountiesModel(log4net.ILog log)
        {
            Log = log;
            LastPngFile =  Properties.Settings.Default.LastPngFile;
            
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(Properties.Resources.CountiesOfEngland));
            ParseCounties(doc);

            foreach (var county in Counties)
                county.SetupNeighbours(Counties);

        }

        #endregion

        #region Properties

        public List<string> CountyNames { get { return _counties.Keys.ToList(); } }
        public List<CountyGeography> Counties { get { return _counties.Values.OrderBy(x => x.Name).ToList(); } }
        
        public string LastPngFile { get; set; }

        #endregion

        #region Member Data

        private log4net.ILog Log;
        private readonly Dictionary<string, CountyGeography> _counties = new Dictionary<string, CountyGeography>();

        #endregion

        #region Utility Functions

        private void ParseCounties(XmlDocument doc)
        {
            var nameNodes = doc.SelectNodes("//Document/Placemark/name");

            var placemarkNodes = doc.SelectNodes("//Document/Placemark");

            if (placemarkNodes == null) return;

            foreach (var node in placemarkNodes)
            {
                XmlElement element = (XmlElement)node;
                CountyGeography country = CountyGeography.Create(element);
                _counties.Add(country.Name, country);
            }
        }

        #endregion
    }
}
