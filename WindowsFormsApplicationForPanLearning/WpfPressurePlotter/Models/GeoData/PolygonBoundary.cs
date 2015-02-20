using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPressurePlotter.Models.GeoData
{
    public class PolygonBoundary
    {
        public List<PolygonPoint> Points { get; set; }


        public double MinLongitude { get; private set; }
        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        public double MaxLatitude { get; private set; }   

        public PolygonBoundary()
        {
            MinLongitude = MinLatitude = Double.MaxValue;
            MaxLongitude = MaxLatitude = Double.MinValue;
            Points = new List<PolygonPoint>(); 
        }
        public PolygonBoundary(string coordinates) 
        {
            Points = new List<PolygonPoint>();
            MinLongitude = MinLatitude = Double.MaxValue;
            MaxLongitude = MaxLatitude = Double.MinValue;

            string[] latLongPairs = coordinates.Split(' ');
            foreach (var latLongPair in latLongPairs)
            {
                PolygonPoint point = new PolygonPoint(latLongPair);
                Points.Add(point);

                MinLongitude = Math.Min(MinLongitude, point.Longitude);
                MaxLongitude = Math.Max(MaxLongitude, point.Longitude);

                MinLatitude = Math.Min(MinLatitude, point.Latitude);
                MaxLatitude = Math.Max(MaxLatitude, point.Latitude);
            }
        }
    }
}
