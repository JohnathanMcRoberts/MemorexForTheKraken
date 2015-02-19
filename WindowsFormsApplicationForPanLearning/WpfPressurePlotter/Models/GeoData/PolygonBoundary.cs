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
        public PolygonBoundary() { Points = new List<PolygonPoint>(); }
        public PolygonBoundary(string coordinates) 
        { 
            Points = new List<PolygonPoint>();

            string[] latLongPairs = coordinates.Split(' ');
            foreach (var latLongPair in latLongPairs)
                Points.Add(new PolygonPoint(latLongPair));
        }
    }
}
