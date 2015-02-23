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
            Longitude = Latitude = Double.NaN;
            string[] coords = latLongPair.Split(',');

            if (coords.Length < 2)
                return;

            double coord = 0.0f;
            if (Double.TryParse(coords[0], out coord))
                Longitude = coord;
            if (Double.TryParse(coords[1], out coord))
                Latitude = coord;
        }

        public PolygonPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        const double DegreesPerRadians = 180.0 / Math.PI;

        public void GetCoordinates(out double x, out double y)
        {
            x = Longitude;
            if ((90.0 - Math.Abs(Latitude)) < 0.01) 
                y = 0;
            else
            {
                double latInRads = Latitude / DegreesPerRadians;
                double latTan = Math.Tan( (Math.PI/4) + latInRads/2);
                y = Math.Log(latTan) * DegreesPerRadians;
            }
        }

        public static PolygonPoint Create(string latLongPair, out bool isValid)
        {
            isValid = false;
            PolygonPoint pt = new PolygonPoint();
            pt.Longitude = pt.Latitude = Double.NaN;
            string[] coords = latLongPair.Split(',');

            if (coords.Length < 2)
                return pt;

            isValid = true;
            double coord = 0.0f;
            if (Double.TryParse(coords[0], out coord))
                pt.Longitude = coord;
            else
                isValid = false;
            if (Double.TryParse(coords[1], out coord))
                pt.Latitude = coord;
            else
                isValid = false;
            return pt;
        }
    }
}
