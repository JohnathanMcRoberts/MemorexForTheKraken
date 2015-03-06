using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPressurePlotter.Models.GeoData
{
    public class NeighbouringGeography
    {
        #region Properties
        
        public IGeographicEntity Neighbour { get; set; }
        public double CentreToCentreDistanceSquared { get; set; }
        public double EdgeToEdgeDistanceSquared { get; set; }
        public double CentreToCentreDistance 
        { 
            get {return Math.Sqrt(CentreToCentreDistanceSquared); }
            set { CentreToCentreDistanceSquared = value * value; }
        }
        public double EdgeToEdgeDistance
        {
            get { return Math.Sqrt(EdgeToEdgeDistanceSquared); }
            set { EdgeToEdgeDistanceSquared = value * value; }
        }
        
        #endregion

        #region Static Functions
        
        public static double DistanceBetweenLatLongs(
            double latC1, double longC1, double latC2, double longC2)
        {
            return Math.Sqrt(DistanceSquaredBetweenLatLongs(latC1, longC1, latC2, longC2));
        }

        public static double DistanceSquaredBetweenLatLongs(
            double latC1, double longC1, double latC2, double longC2)
        {
            double deltaLat = latC1 - latC2;
            double deltaLong = longC1 - longC2;
            double distance = (deltaLat * deltaLat) + (deltaLong * deltaLong);
            return distance;
        }

        public static double CentreToCentreDistanceSquaredBetweenGeographicEntities(
            IGeographicEntity c1, IGeographicEntity c2)
        {
            return DistanceSquaredBetweenLatLongs(
                c1.CentroidLatitude, c1.CentroidLongitude, c2.CentroidLatitude, c2.CentroidLongitude);
        }

        public static double CentreToCentreDistanceBetweenGeographicEntities(
            IGeographicEntity c1, IGeographicEntity c2)
        {
            return DistanceBetweenLatLongs(
                c1.CentroidLatitude, c1.CentroidLongitude, c2.CentroidLatitude, c2.CentroidLongitude);
        }

        public static double EdgeToEdgeDistanceSquaredBetweenGeographicEntities(
            IGeographicEntity c1, IGeographicEntity c2)
        {
            Double minDist = Double.MaxValue;

            var landBlocksC1 = (from b in c1.LandBlocks
                                orderby b.TotalArea
                                select b).Take(5);
            foreach (var landBlockC1 in landBlocksC1)
            {
                foreach (var pointC1 in landBlockC1.Points)
                {
                    foreach (var landBlockC2 in c2.LandBlocks)
                        foreach (var pointC2 in landBlockC2.Points)
                            minDist = Math.Min(minDist,
                                DistanceSquaredBetweenLatLongs(
                                    pointC1.Latitude, pointC1.Longitude,
                                    pointC2.Latitude, pointC2.Longitude)
                                    );

                }
            }

            return minDist;
        }

        public static List<NeighbouringGeography> SetupNeighbours(
            IGeographicEntity source, IList<IGeographicEntity> neighbourItems)
        {
            List<NeighbouringGeography> neighbours = new List<NeighbouringGeography>();

            foreach (var neighbour in neighbourItems)
            {
                if (neighbour.Name == source.Name) continue;

                neighbours.Add(
                    new NeighbouringGeography()
                    {
                        Neighbour = neighbour,
                        CentreToCentreDistanceSquared =
                            NeighbouringGeography.CentreToCentreDistanceSquaredBetweenGeographicEntities(source, neighbour),
                        EdgeToEdgeDistanceSquared =
                            NeighbouringGeography.EdgeToEdgeDistanceSquaredBetweenGeographicEntities(source, neighbour)
                    }
                    );
            }
            return neighbours.OrderBy(x => x.EdgeToEdgeDistance).ToList();
        }

        #endregion
    }
}
