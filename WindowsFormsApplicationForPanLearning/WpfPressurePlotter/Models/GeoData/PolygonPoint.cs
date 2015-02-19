using System;

namespace WpfPressurePlotter.Models.GeoData
{
    public class PolygonPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public PolygonPoint()
        { 
            Latitude = Longitude = 0.0f; 
        }

        public PolygonPoint(string latLongPair)
        {
            string[] coords = latLongPair.Split(',');
            double coord = 0.0f;
            if (Double.TryParse(coords[0], out coord))
                Longitude = coord;
            if (Double.TryParse(coords[1], out coord))
                Latitude = coord;
        }

        const double DegreesToRadians = 180.0 / Math.PI;

        public void GetCoordinates(out double x, out double y)
        {
            x = Longitude;
            if ((Math.Abs(Latitude) - 90.0) < 0.01) 
                y = 0;
            else
            {
                double latInRads = Latitude * DegreesToRadians;
                double loxodrome = Math.Tan( (Math.PI/4) + latInRads/2);
                y = Math.Log (loxodrome);
            }
        }
    }
}
