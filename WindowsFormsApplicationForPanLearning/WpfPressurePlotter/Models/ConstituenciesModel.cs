using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using WpfPressurePlotter.Models.GeoData;


namespace WpfPressurePlotter.Models
{
    public class ConstituenciesModel
    {
        #region Constructor

        public ConstituenciesModel(log4net.ILog log)
        {
            Log = log;
            LastPngFile =  Properties.Settings.Default.LastPngFile;
            ReadConstituenciesFromXml(log);            
        }

        #endregion
            
        #region Properties

        public List<string> ConstituencyNames { get { return _constituencies.Keys.ToList(); } }
        public List<ConstituencyGeography> Constituencies { get { return _constituencies.Values.OrderBy(x => x.Name).ToList(); } }
        
        public string LastPngFile { get; set; }

        #endregion

        #region Member Data

        private log4net.ILog Log;
        private readonly Dictionary<string, ConstituencyGeography> _constituencies = new Dictionary<string, ConstituencyGeography>();

        #endregion

        #region Utility Functions

        private void ReadConstituenciesFromXml(log4net.ILog log)
        {
            XmlReader xReader = XmlReader.Create(new StringReader(Properties.Resources.UkConstuencies));

            int i = 0;
            string constName = "";
            ConstituencyGeography constituency = null;
            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xReader.Name == "name")
                        {
                            constName = xReader.ReadInnerXml();
                            constituency = new ConstituencyGeography();
                            constituency.Name = constName;
                        }
                        if (xReader.Name == "coordinates")
                        {
                            string coords = xReader.ReadInnerXml();

                            if (constituency != null)
                                constituency.LandBlocks.Add(new PolygonBoundary(coords));
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (xReader.Name == "Placemark")
                        {
                            log.Debug("Adding constituency " + constName);
                            ++i;
                            if (constituency != null)
                            {
                                constituency.UpdateLatLongs();
                                _constituencies.Add(constituency.Name, constituency);
                            }
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
