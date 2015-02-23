using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WpfPressurePlotter.Models.GeoData
{
    public class CountyGeography
    {
        public string Name {get;set;}

        public double CentroidLongitude { get; private set; }
        public double CentroidLatitude { get; private set; }

        public double MinLongitude { get; private set; }
        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        public double MaxLatitude { get; private set; }   

        public List<PolygonBoundary> LandBlocks { get; set; }

        public CountyGeography()
        {
            LandBlocks = new List<PolygonBoundary>();
        }

        public static CountyGeography Create(XmlElement element)
        {
            CountyGeography country = new CountyGeography();

            country.Name = element.SelectSingleNode("name").InnerText;

            var boundaryRingCoordinates =
                element.SelectNodes("Polygon/outerBoundaryIs/LinearRing");

            if (boundaryRingCoordinates.Count < 1)
                boundaryRingCoordinates = element.SelectNodes("MultiGeometry/Polygon/outerBoundaryIs/LinearRing");

            country.MinLongitude = country.MinLatitude = Double.MaxValue;
            country.MaxLongitude = country.MaxLatitude = Double.MinValue;

            country.CentroidLongitude = 0;
            country.CentroidLatitude = 0;

            double totalArea = 0;

            foreach (var boundary in boundaryRingCoordinates)
            {
                PolygonBoundary landBlock = new PolygonBoundary(
                        ((XmlElement)boundary).SelectSingleNode("coordinates").InnerText);

                country.LandBlocks.Add(landBlock);

                country.MinLongitude = Math.Min(landBlock.MinLongitude, country.MinLongitude);
                country.MaxLongitude = Math.Max(landBlock.MaxLongitude, country.MaxLongitude);

                country.MinLatitude = Math.Min(landBlock.MinLatitude, country.MinLatitude);
                country.MaxLatitude = Math.Max(landBlock.MaxLatitude, country.MaxLatitude);

                country.CentroidLongitude += landBlock.CentroidLongitude * landBlock.TotalArea;
                country.CentroidLatitude += landBlock.CentroidLongitude * landBlock.TotalArea;

                totalArea += landBlock.TotalArea;
            }
            country.CentroidLongitude /= totalArea;
            country.CentroidLatitude /= totalArea;

            return country;
        }
    }
}
