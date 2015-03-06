using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using WpfPressurePlotter.Models.GeoData;


namespace WpfPressurePlotter.Models
{
    public class ConstituenciesModel
    {
        #region Constructor

        public ConstituenciesModel(log4net.ILog log, List<IGeographicEntity> countries)
        {
            Log = log;
            LastPngFile =  Properties.Settings.Default.LastPngFile;
            ReadConstituenciesFromXml(log);

            LoadededNeighbours = false;

            var neighbourDistancesFile = Properties.Settings.Default.NeighbourDistancesFile;
            if (!ReadNeighbourDistancesFile(countries, neighbourDistancesFile))
            {
                SetupNeighboursFile(countries);
            }
            SetupNeighbouringDistances();
        }

        #endregion

        #region Properties

        public List<string> ConstituencyNames { get { return _constituencies.Keys.ToList(); } }
        public List<ConstituencyGeography> Constituencies { get { return _constituencies.Values.OrderBy(x => x.Name).ToList(); } }
        
        public string LastPngFile { get; set; }

        public bool LoadededNeighbours { get; private set; }

        public List<IGeographicEntity> NearestCountries { get; private set; }

        #endregion

        #region Member Data

        private log4net.ILog Log;
        private readonly Dictionary<string, ConstituencyGeography> _constituencies = 
            new Dictionary<string, ConstituencyGeography>();        
        private readonly Dictionary<string, IGeographicEntity> _countryLookup = 
            new Dictionary<string, IGeographicEntity>();

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
        
        private void WriteOutConstituencyNeighbourDistances()
        {
            using (XmlWriter writer = XmlWriter.Create("ConstituencyAndNeighbourDistances.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Constituencies");

                foreach (var constituency in Constituencies)
                {
                    writer.WriteStartElement("Constituency");

                    writer.WriteElementString("Name", constituency.Name);

                    writer.WriteStartElement("Neighbours");
                    foreach (var neighbour in constituency.Neighbours)
                    {
                        writer.WriteStartElement("Neighbour");

                        writer.WriteElementString("Name", neighbour.Neighbour.Name);
                        writer.WriteElementString("TotalArea",
                            neighbour.Neighbour.TotalArea.ToString());
                        writer.WriteElementString("CentroidLatitude",
                            neighbour.Neighbour.CentroidLatitude.ToString());
                        writer.WriteElementString("CentroidLongitude",
                            neighbour.Neighbour.CentroidLongitude.ToString());
                        writer.WriteElementString("EdgeToEdgeDistance",
                            neighbour.EdgeToEdgeDistance.ToString());
                        writer.WriteElementString("CentreToCentreDistance",
                            neighbour.CentreToCentreDistance.ToString());

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void SetupNeighboursFile(List<IGeographicEntity> countries)
        {
            foreach (var constituency in Constituencies)
            {
                constituency.Neighbours =
                   NeighbouringGeography.SetupNeighbours(constituency, countries);
                for (int i = 0; i < 40 && i < constituency.Neighbours.Count; ++i)
                    Log.Debug(
                        "Country " + (1 + i) + "th nearest (" + constituency.Name
                        + ") is " + constituency.Neighbours[i].Neighbour.Name + "\n  @ C2C=" +
                        constituency.Neighbours[i].CentreToCentreDistance + "\n  @ E2E=" +
                        constituency.Neighbours[i].EdgeToEdgeDistance);
            }
            WriteOutConstituencyNeighbourDistances();
        }

        private bool ReadNeighbourDistancesFile(List<IGeographicEntity> countries, string neighbourDistancesFile)
        {
            // first make up a dictionary of the countries for speed
            foreach (var country in countries)
                _countryLookup.Add(country.Name, country);

            try
            {
                string constName = "";
                string neighbourName = "";
                ConstituencyGeography constituency = null;
                NeighbouringGeography neighbour = null;
                IGeographicEntity neighbourCountry = null;
                bool readingConstituency = false;
                bool readingNeighbour = false;
                bool readingEdgeToEdgeDistance = false;
                bool readingCentreToCentreDistance = false;
                bool readingName = false;

                XmlReader xReader = XmlReader.Create(neighbourDistancesFile);
                while (xReader.Read())
                {
                    switch (xReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            SetReadNeighboursReadElementFlags(
                                ref constituency, ref neighbour, ref readingConstituency, 
                                ref readingNeighbour, ref readingEdgeToEdgeDistance, 
                                ref readingCentreToCentreDistance, ref readingName, xReader);
                            break;

                        case XmlNodeType.Text:
                            ProcessReadNeighboursText(_countryLookup, ref constName, 
                                ref neighbourName, ref constituency, ref neighbour, 
                                ref neighbourCountry, readingConstituency, readingNeighbour, 
                                readingEdgeToEdgeDistance, readingCentreToCentreDistance, 
                                readingName, xReader);
                            break;
                        case XmlNodeType.EndElement:
                            ClearReadNeighboursEndElementFlags(constName, neighbourName, 
                                constituency, neighbour, ref readingConstituency, 
                                ref readingNeighbour, ref readingEdgeToEdgeDistance, 
                                ref readingCentreToCentreDistance, ref readingName, xReader);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(" could not read distances : " + e.Message + " : " + e.StackTrace);
                return false;
            }

            return true;
        }

        private void ClearReadNeighboursEndElementFlags(string constName, string neighbourName, 
            ConstituencyGeography constituency, NeighbouringGeography neighbour, 
            ref bool readingConstituency, ref bool readingNeighbour, ref bool readingEdgeToEdgeDistance, 
            ref bool readingCentreToCentreDistance, ref bool readingName, XmlReader xReader)
        {
            if (xReader.Name == "Constituency")
            {
                readingConstituency = false;
                readingNeighbour = false;
                //Log.Debug("Finished constituency " + constName);
            }
            else if (xReader.Name == "Neighbours")
            {
                readingNeighbour = false;
                //Log.Debug("Finished Neighbours " + constName);
            }
            else if (xReader.Name == "Neighbour")
            {
                readingNeighbour = false;
                if (constituency != null && neighbour != null)
                    constituency.Neighbours.Add(neighbour);

                //Log.Debug("  -> Finished Neighbour " + neighbourName);
            }
            else if (xReader.Name == "Name")
            {
                readingName = false;
            }
            else if (xReader.Name == "EdgeToEdgeDistance")
            {
                readingEdgeToEdgeDistance = false;
            }
            else if (xReader.Name == "CentreToCentreDistance")
            {
                readingCentreToCentreDistance = false;
            }
        }

        private void ProcessReadNeighboursText(Dictionary<string, IGeographicEntity> countryLookup, 
            ref string constName, ref string neighbourName, ref ConstituencyGeography constituency, 
            ref NeighbouringGeography neighbour, ref IGeographicEntity neighbourCountry, 
            bool readingConstituency, bool readingNeighbour, bool readingEdgeToEdgeDistance, 
            bool readingCentreToCentreDistance, bool readingName, XmlReader xReader)
        {
            if (readingName)
            {
                var name = xReader.Value;
                if (readingNeighbour)
                {
                    if (countryLookup.ContainsKey(name))
                    {
                        neighbourCountry = countryLookup[name];
                        neighbourName = name;
                        neighbour = new NeighbouringGeography();
                        neighbour.Neighbour = neighbourCountry;
                    }
                    else
                        neighbourCountry = null;
                }
                else if (readingConstituency)
                {
                    if (_constituencies.ContainsKey(name))
                    {
                        constituency = _constituencies[name];
                        constName = name;
                    }
                    else
                        constituency = null;
                }
                else
                    Log.Debug("Unexpected Name " + name);
            }
            else if (readingEdgeToEdgeDistance)
            {
                if (readingNeighbour && neighbour != null)
                    neighbour.EdgeToEdgeDistance = Double.Parse(xReader.Value);
            }
            else if (readingCentreToCentreDistance)
            {
                if (readingNeighbour && neighbour != null)
                    neighbour.CentreToCentreDistance = Double.Parse(xReader.Value);
            }
        }

        private static void SetReadNeighboursReadElementFlags(
            ref ConstituencyGeography constituency, ref NeighbouringGeography neighbour, 
            ref bool readingConstituency, ref bool readingNeighbour, ref bool readingEdgeToEdgeDistance, 
            ref bool readingCentreToCentreDistance, ref bool readingName, XmlReader xReader)
        {
            if (xReader.Name == "Constituency")
            {
                readingConstituency = true;
                readingNeighbour = false;
                constituency = null;
            }
            else if (xReader.Name == "Neighbours")
            {
                readingNeighbour = false;
            }
            else if (xReader.Name == "Neighbour")
            {
                readingNeighbour = true;
                neighbour = null;
            }
            else if (xReader.Name == "Name")
            {
                readingName = true;
            }
            else if (xReader.Name == "EdgeToEdgeDistance")
            {
                readingEdgeToEdgeDistance = true;
            }
            else if (xReader.Name == "CentreToCentreDistance")
            {
                readingCentreToCentreDistance = true;
            }
        }

        private void SetupNeighbouringDistances()
        {
            string currentCountry = "United Kingdom";


            Dictionary<string, CountryTotalDistance> countriesTotalDistances =
                new Dictionary<string, CountryTotalDistance>();

            foreach (var constituency in _constituencies)
            {
                var neighbours = (from n in constituency.Value.Neighbours
                                  where ((n.Neighbour.TotalArea > 1) && (n.Neighbour.Name != currentCountry))
                                  orderby n.EdgeToEdgeDistance
                                  select n).ToList();

                for (int i = 0; i < neighbours.Count(); ++i)
                {
                    var neigbour = neighbours[i];

                    if (!countriesTotalDistances.ContainsKey(neigbour.Neighbour.Name))
                        countriesTotalDistances.Add(
                            neigbour.Neighbour.Name, new CountryTotalDistance(neigbour.Neighbour));

                    var neighbouringCounty = countriesTotalDistances[neigbour.Neighbour.Name];

                    neighbouringCounty.TotalDistance
                        += neigbour.EdgeToEdgeDistance;

                    if (!neighbouringCounty.PositionCounts.ContainsKey(1 + i))
                        neighbouringCounty.PositionCounts.Add(1 + i, 0);

                    neighbouringCounty.PositionCounts[1 + i]++;

                    if (i == 0)
                    {
                        Log.Debug(" Nearest country to " + constituency.Value.Name + " is " +
                            neighbouringCounty.Country.Name + " it is #1 for " +
                            neighbouringCounty.PositionCounts[1 + i] + " constituencies");
                    }
                }
            }
            var countryTotalDist =
                (from c in countriesTotalDistances.Values
                 orderby c.NumberAtPosition(1) descending
                 select c).ToList();

            NearestCountries = (from c in countriesTotalDistances.Values
                                where ( (c.NumberAtPosition(1) > 0) ||  (c.NumberAtPosition(2) > 0))
                                orderby c.NumberAtPosition(1) descending
                                select c.Country).ToList();


        }

        #endregion

        #region Helper classes

        public class CountryTotalDistance
        {
            public IGeographicEntity Country { get; set; }
            public double TotalDistance { get; set; }
            public Dictionary<int, int> PositionCounts { get; set; }

            public CountryTotalDistance(IGeographicEntity country)
            {
                Country = country;
                TotalDistance = 0.0;
                PositionCounts = new Dictionary<int, int>();
            }

            public int NumberAtPosition(int position)
            {
                if (!PositionCounts.ContainsKey(position)) return 0;
                return PositionCounts[position];
            }

            public int PositionTotal()
            {
                int total = 0;
                foreach (var positionIndex in PositionCounts.Keys)
                    total += positionIndex * PositionCounts[positionIndex];
                return total;
            }

        }

        #endregion

    }
}
