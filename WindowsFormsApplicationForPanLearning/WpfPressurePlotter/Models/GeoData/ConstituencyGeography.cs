using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace WpfPressurePlotter.Models.GeoData
{
    public class ConstituencyGeography : IGeographicEntity
    {
        public string Name {get;set;}

        public double CentroidLongitude { get; private set; }
        public double CentroidLatitude { get; private set; }

        public double MinLongitude { get; private set; }
        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        public double MaxLatitude { get; private set; }

        public double TotalArea
        {
            get
            {
                double ttl = 0;
                foreach (var block in LandBlocks) ttl += block.TotalArea;
                return ttl;
            }
        }   

        public List<PolygonBoundary> LandBlocks { get; set; }

        public ConstituencyGeography()
        {
            LandBlocks = new List<PolygonBoundary>();
            Neighbours = new List<NeighbouringGeography>();
        }

        public void UpdateLatLongs()
        {
            MinLongitude = MinLatitude = Double.MaxValue;
            MaxLongitude = MaxLatitude = Double.MinValue;

            CentroidLongitude = 0;
            CentroidLatitude = 0;

            double totalArea = 0;

            foreach (var landBlock in LandBlocks)
            {
                MinLongitude = Math.Min(landBlock.MinLongitude, MinLongitude);
                MaxLongitude = Math.Max(landBlock.MaxLongitude, MaxLongitude);

                MinLatitude = Math.Min(landBlock.MinLatitude, MinLatitude);
                MaxLatitude = Math.Max(landBlock.MaxLatitude, MaxLatitude);

                CentroidLongitude += landBlock.CentroidLongitude * landBlock.TotalArea;
                CentroidLatitude += landBlock.CentroidLatitude * landBlock.TotalArea;

                totalArea += landBlock.TotalArea;
            }
            CentroidLongitude /= totalArea;
            CentroidLatitude /= totalArea;
        }

        public static ConstituencyGeography Create(XmlElement element)
        {
            ConstituencyGeography constituency = new ConstituencyGeography();

            constituency.Name = element.SelectSingleNode("name").InnerText;

            var boundaryRingCoordinates =
                element.SelectNodes("Polygon/outerBoundaryIs/LinearRing");

            if (boundaryRingCoordinates.Count < 1)
                boundaryRingCoordinates = element.SelectNodes("MultiGeometry/Polygon/outerBoundaryIs/LinearRing");


            foreach (var boundary in boundaryRingCoordinates)
            {
                PolygonBoundary landBlock = new PolygonBoundary(
                        ((XmlElement)boundary).SelectSingleNode("coordinates").InnerText);

            }
            constituency.UpdateLatLongs();

            return constituency;
        }

        public List<NeighbouringGeography> Neighbours { get; set; }
    }
}
