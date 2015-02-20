using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WpfPressurePlotter.Models.GeoData
{
    public class CountryGeography
    {
        public string Name {get;set;}
        public string Description { get; set; } // eg "ISO_A2=SE : ISO_N3=752.0"
        public string ISO_A2 
        {
            get 
            { 
                if (!Description.Contains("ISO_A2=")) return "";
                string[] elements = Description.Substring(7).Split(' ');
                return elements[0];
            }
        }
        public string ISO_N3
        {
            get
            {
                if (!Description.Contains("ISO_N3=")) return "";
                string[] elements = Description.Split('=');
                return elements[elements.Length -1];
            }
        }

        public double CentralLongitude {get;set;}
        public double CentralLatitude {get;set;}

        public double MinLongitude { get; private set; }
        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        public double MaxLatitude { get; private set; }   

        public List<PolygonBoundary> LandBlocks { get; set; }

        public CountryGeography()
        {
            LandBlocks = new List<PolygonBoundary>();
        }

        public static CountryGeography Create(XmlElement element)
        {
            CountryGeography country = new CountryGeography();

            country.Name = element.SelectSingleNode("name").InnerText;
            country.Description = element.SelectSingleNode("description").InnerText;

            var lookatNode = element.SelectSingleNode("LookAt");

            country.CentralLatitude =
                Double.Parse(lookatNode.SelectSingleNode("latitude").InnerText);
            country.CentralLongitude =
                Double.Parse(lookatNode.SelectSingleNode("longitude").InnerText);

            var boundaryRingCoordinates =
                element.SelectNodes("MultiGeometry/Polygon/outerBoundaryIs/LinearRing");

            country.MinLongitude = country.MaxLongitude = country.CentralLongitude;
            country.MinLatitude = country.MaxLatitude = country.CentralLatitude;

            foreach (var boundary in boundaryRingCoordinates)
            {
                PolygonBoundary landBlock = new PolygonBoundary(
                        ((XmlElement)boundary).SelectSingleNode("coordinates").InnerText);

                country.LandBlocks.Add(landBlock);

                country.MinLongitude = Math.Min(landBlock.MinLongitude, country.MinLongitude);
                country.MaxLongitude = Math.Max(landBlock.MaxLongitude, country.MaxLongitude);

                country.MinLatitude = Math.Min(landBlock.MinLatitude, country.MinLatitude);
                country.MaxLatitude = Math.Max(landBlock.MaxLatitude, country.MaxLatitude);
            }

            return country;
        }
    }
}
