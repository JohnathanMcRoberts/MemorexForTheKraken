﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WpfPressurePlotter.Models.GeoData
{
    public class CountyGeography : IGeographicEntity
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

        public CountyGeography()
        {
            LandBlocks = new List<PolygonBoundary>();
            Neighbours = new List<NeighbouringCounty>();
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
                country.CentroidLatitude += landBlock.CentroidLatitude * landBlock.TotalArea;

                totalArea += landBlock.TotalArea;
            }
            country.CentroidLongitude /= totalArea;
            country.CentroidLatitude /= totalArea;

            return country;
        }

        public class NeighbouringCounty
        {
            public CountyGeography County { get; set; }
            public double CentreToCentreDistance { get; set; }
        }

        public List<NeighbouringCounty> Neighbours { get; set; }

        public void SetupNeighbours(List<CountyGeography> counties)
        {
            List<NeighbouringCounty> neighbours = new List<NeighbouringCounty>();

            foreach (var county in counties)
            {
                if (county.Name == Name) continue;

                neighbours.Add(
                    new NeighbouringCounty() 
                    { 
                        County = county,
                        CentreToCentreDistance = NeighbouringGeography.CentreToCentreDistanceBetweenGeographicEntities(this, county)
                    }
                    );
            }
            Neighbours = neighbours.OrderBy(x => x.CentreToCentreDistance).ToList();
        }
    }
}
